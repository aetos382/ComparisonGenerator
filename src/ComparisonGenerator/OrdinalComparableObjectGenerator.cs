﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ComparisonGenerator
{
    [Generator]
    public class OrdinalComparableObjectGenerator :
        ISourceGenerator
    {
        public OrdinalComparableObjectGenerator()
        {
        }

        private readonly GenerateOptions? _options;

        internal OrdinalComparableObjectGenerator(
            GenerateOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this._options = options;
        }

        public void Initialize(
            GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(
                () => new SyntaxReceiver());
        }

        public void Execute(
            GeneratorExecutionContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached &&
                context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.DebugGenerator", out var debugOption) &&
                bool.TryParse(debugOption, out var debug) &&
                debug)
            {
                var currentProcessName = Process.GetCurrentProcess().ProcessName;
                if (string.Equals(currentProcessName, "dotnet", StringComparison.OrdinalIgnoreCase))
                {
                    Debugger.Launch();
                }
            }
#endif

            if (context.Compilation.Language != LanguageNames.CSharp)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.LanguageNotSupported,
                        null,
                        context.Compilation.Language));

                return;
            }

            Attributes.AddToProject(context);

            if (context.SyntaxReceiver is not SyntaxReceiver receiver)
            {
                return;
            }

            var compilation = Attributes.AddToCompilation(context);
            var commonTypes = new CommonTypes(compilation);

            foreach (var syntax in receiver.CandidateSyntaxes)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                var semanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = semanticModel.GetDeclaredSymbol(syntax, context.CancellationToken);

                var comparableAttribute = commonTypes.GetComparableAttribute(symbol);
                if (comparableAttribute is null)
                {
                    continue;
                }
                
                var nullableContext =
                    semanticModel.GetNullableContext(syntax.Span.End);

                var options =
                    this._options ??
                    new GenerateOptions(
                        context,
                        syntax,
                        comparableAttribute);

                var sourceTypeInfo = new SourceTypeInfo(
                    symbol,
                    commonTypes,
                    nullableContext);

                var syntaxLocation = syntax.GetLocation();

                if (!syntax.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            DiagnosticDescriptors.TypeIsNotPartial,
                            syntaxLocation,
                            symbol.GetFullName()));

                    continue;
                }

                if (syntax.Modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            DiagnosticDescriptors.TypeIsStatic,
                            syntaxLocation));

                    continue;
                }

                var types = new List<INamedTypeSymbol>();
                types.Add(symbol);

                var enclosingType = symbol.ContainingType;

                TypeDeclarationSyntax? invalidSyntax = null;

                while (enclosingType is not null)
                {
                    invalidSyntax = enclosingType.DeclaringSyntaxReferences
                        .Select(x => x.GetSyntax(context.CancellationToken))
                        .OfType<TypeDeclarationSyntax>()
                        .FirstOrDefault(x => !x.Modifiers.Any(SyntaxKind.PartialKeyword));

                    if (invalidSyntax is not null)
                    {
                        break;
                    }

                    types.Add(enclosingType);
                    enclosingType = enclosingType.ContainingType;
                }

                if (invalidSyntax is not null)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            DiagnosticDescriptors.TypeIsNotPartial,
                            invalidSyntax.GetLocation()));

                    continue;
                }

                types.Reverse();

                string fullName = symbol.GetFullName(out var ns, out _);

                var members = new List<(SourceMemberInfo member, int order)>();

                foreach (var member in symbol.GetMembers())
                {
                    var order = commonTypes.GetComparisonOrder(member);
                    if (order is null)
                    {
                        continue;
                    }

                    var memberInfo = new SourceMemberInfo(member);

                    if (options.GenerateGenericComparable ||
                        options.GenerateNonGenericComparable ||
                        options.GenerateComparisonOperators)
                    {
                        var memberType = memberInfo.Type;

                        if (!commonTypes.IsGenericComparable(memberType) &&
                            !commonTypes.IsNonGenericComparable(memberType))
                        {
                            context.ReportDiagnostic(
                                Diagnostic.Create(
                                    DiagnosticDescriptors.TypeIsNotComparable,
                                    member.Locations[0],
                                    member.Locations.Skip(1),
                                    fullName,
                                    memberInfo.Name,
                                    memberInfo.TypeName));
                        }
                    }

                    members.Add((memberInfo, order.Value));
                }

                if (!members.Any())
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            DiagnosticDescriptors.NoMembers,
                            symbol.Locations[0],
                            symbol.Locations.Skip(1),
                            default,
                            default));

                    continue;
                }

                var ms = members
                    .OrderBy(x => x.order)
                    .ThenBy(x => x.member.Name)
                    .Select(x => x.member)
                    .ToArray();

                var c = new ComparisonGeneratorContext(
                    compilation,
                    ns,
                    types,
                    ms,
                    options,
                    commonTypes,
                    sourceTypeInfo,
                    nullableContext);

                GenerateCode(
                    context,
                    new CommonGenerator(c),
                    $"{fullName}_Common.cs");

                if (options.GenerateEquatable &&
                    !sourceTypeInfo.IsEquatable)
                {
                    GenerateCode(
                        context,
                        new EquatableGenerator(c),
                        $"{fullName}_Equatable.cs");
                }

                if (options.GenerateGenericComparable &&
                    !sourceTypeInfo.IsGenericComparable)
                {
                    GenerateCode(
                        context,
                        new GenericComparableGenerator(c),
                        $"{fullName}_GenericComparable.cs");
                }

                if (options.GenerateNonGenericComparable &&
                    !sourceTypeInfo.IsNonGenericComparable)
                {
                    GenerateCode(
                        context,
                        new NonGenericComparableGenerator(c),
                        $"{fullName}_NonGenericComparable.cs");
                }

                if (options.GenerateObjectEquals &&
                    !sourceTypeInfo.OverridesObjectEquals)
                {
                    GenerateCode(
                        context,
                        new ObjectEqualsGenerator(c),
                        $"{fullName}_ObjectEquals.cs");
                }

                if (options.GenerateEqualityOperators &&
                    !sourceTypeInfo.DefinedNullableEqualityOperators)
                {
                    GenerateCode(
                        context,
                        new EquatableOperatorsGenerator(c),
                        $"{fullName}_EqualityOperators.cs");
                }

                if (options.GenerateComparisonOperators &&
                    !sourceTypeInfo.DefinedNullableComparisonOperators)
                {
                    GenerateCode(
                        context,
                        new ComparisonOperatorsGenerator(c),
                        $"{fullName}_ComparisonOperators.cs");
                }
            }
        }

        private static void GenerateCode(
            GeneratorExecutionContext context,
            GeneratorBase generator,
            string hintName)
        {
            string code = generator.TransformText();
            context.AddSource(hintName, code);
        }

        private class SyntaxReceiver :
            ISyntaxReceiver
        {
            public readonly List<TypeDeclarationSyntax> CandidateSyntaxes = new();

            public void OnVisitSyntaxNode(
                SyntaxNode syntaxNode)
            {
                if (syntaxNode is null)
                {
                    throw new ArgumentNullException(nameof(syntaxNode));
                }

                if (syntaxNode is not TypeDeclarationSyntax typeDecl)
                {
                    return;
                }
                
                var attrs =
                    typeDecl.AttributeLists
                        .SelectMany(x => x.Attributes);

                if (!attrs.Any())
                {
                    return;
                }

                this.CandidateSyntaxes.Add(typeDecl);
            }
        }
    }
}