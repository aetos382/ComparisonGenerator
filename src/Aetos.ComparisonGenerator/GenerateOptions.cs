﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Aetos.ComparisonGenerator
{
    internal class GenerateOptions
    {
        internal GenerateOptions(
            bool generateEquatable = true,
            bool generateGenericComparable = true,
            bool generateNonGenericComparable = true,
            bool overrideObjectMethods = true,
            bool generateEqualityContract = true,
            bool generateOperators = false,
            bool generateStructuralEquatable = false,
            bool generateStructuralComparable = false,
            bool generateMethodsAsVirtual = true,
            bool generateNullabilityAttributes = true,
            bool preferStructuralComparison = false)
        {
            this.GenerateEquatable = generateEquatable;
            this.GenerateGenericComparable = generateGenericComparable;
            this.GenerateNonGenericComparable = generateNonGenericComparable;
            this.OverrideObjectMethods = overrideObjectMethods;
            this.GenerateEqualityContract = generateEqualityContract;
            this.GenerateOperators = generateOperators;
            this.GenerateStructuralEquatable = generateStructuralEquatable;
            this.GenerateStructuralComparable = generateStructuralComparable;
            this.GenerateMethodsAsVirtual = generateMethodsAsVirtual;
            this.GenerateNullabilityAttributes = generateNullabilityAttributes;
            this.PreferStructuralComparison = preferStructuralComparison;
        }

        public GenerateOptions(
            GeneratorExecutionContext context,
            TypeDeclarationSyntax syntax,
            AttributeData attribute)
        {
            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            if (attribute is null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            var attributeDictionary = attribute.NamedArguments
                .ToDictionary(
                    x => x.Key,
                    x => x.Value,
                    StringComparer.OrdinalIgnoreCase);

            this.GenerateEquatable = LocalGetOption(nameof(this.GenerateEquatable)) ?? true;
            this.GenerateGenericComparable = LocalGetOption(nameof(this.GenerateGenericComparable)) ?? true;
            this.GenerateNonGenericComparable = LocalGetOption(nameof(this.GenerateNonGenericComparable)) ?? true;
            this.OverrideObjectMethods = LocalGetOption(nameof(this.OverrideObjectMethods)) ?? true;
            this.GenerateEqualityContract = LocalGetOption(nameof(this.GenerateEqualityContract)) ?? true;
            this.GenerateOperators = LocalGetOption(nameof(this.GenerateOperators)) ?? false;
            this.GenerateStructuralEquatable = LocalGetOption(nameof(this.GenerateStructuralEquatable)) ?? false;
            this.GenerateStructuralComparable = LocalGetOption(nameof(this.GenerateStructuralComparable)) ?? false;
            this.GenerateMethodsAsVirtual = LocalGetOption(nameof(this.GenerateMethodsAsVirtual)) ?? true;
            this.GenerateNullabilityAttributes = LocalGetOption(nameof(this.GenerateNullabilityAttributes)) ?? true;
            this.PreferStructuralComparison = LocalGetOption(nameof(this.PreferStructuralComparison)) ?? false;

            bool? LocalGetOption(string optionName)
            {
                return GetOption(context, syntax, attributeDictionary, optionName);
            }
        }

        public bool GenerateEquatable { get; }

        public bool GenerateGenericComparable { get; }

        public bool GenerateNonGenericComparable { get; }

        public bool OverrideObjectMethods { get; }

        public bool GenerateOperators { get; }

        public bool GenerateStructuralEquatable { get; }

        public bool GenerateStructuralComparable { get; }

        public bool GenerateEqualityContract { get; }

        public bool GenerateMethodsAsVirtual { get; }

        public bool GenerateNullabilityAttributes { get; }

        public bool PreferStructuralComparison { get; }

        private static bool? GetOption(
            GeneratorExecutionContext context,
            TypeDeclarationSyntax syntax,
            IDictionary<string, TypedConstant> attributeArguments,
            string optionName)
        {
            if (attributeArguments.TryGetValue(optionName, out var objValue) &&
                !objValue.IsNull &&
                objValue.Value is bool boolValue1)
            {
                return boolValue1;
            }

            var options = context.AnalyzerConfigOptions.GetOptions(syntax.SyntaxTree);
            bool value;

            // from ItemMetadata
            string buildMetadataName = $"build_metadata.Compile.{optionName}";
            if (options.TryGetBooleanOption(buildMetadataName, out value))
            {
                return value;
            }

            // from .editorconfig
            if (options.TryGetBooleanOption(optionName, out value))
            {
                return value;
            }

            // from PropertyGroup or CommandLine
            // build property に指定されたものも SyntaxTree ごとの options を通じて取得可能
            string buildPropertyName = $"build_property.{optionName}";
            if (options.TryGetBooleanOption(buildPropertyName, out value))
            {
                return value;
            }

            return null;
        }
    }
}
