﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン: 16.0.0.0
//  
//     このファイルへの変更は、正しくない動作の原因になる可能性があり、
//     コードが再生成されると失われます。
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Aetos.ComparisonGenerator
{
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    internal partial class EquatableGenerator : GeneratorBase
    {
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {

    base.TransformText();

            return this.GenerationEnvironment.ToString();
        }

    protected override void WriteCode()
    {
        var sourceTypeInfo = this.SourceTypeInfo;
        var type = sourceTypeInfo.TypeSymbol;

        string typeName = type.Name;
        string typeKind = GetTypeKind(type);

        var options = sourceTypeInfo.GenerateOptions;

        var isValueType = sourceTypeInfo.IsValueType;
        var nullableAnnotationEnabled = sourceTypeInfo.NullableAnnotationsEnabled;

        var parameterTypeName = (isValueType, nullableAnnotationEnabled) switch {

            (true, _) => typeName,
            (false, false) => typeName,
            (false, true) => $"{typeName}?"

        };

        string nullableObjectTypeName = nullableAnnotationEnabled
            ? "object?"
            : "object";

        string virtualModifier =
            !sourceTypeInfo.IsValueType && options.GenerateMethodsAsVirtual ?
                " virtual" : "";

this.Write("partial ");

this.Write(this.ToStringHelper.ToStringWithCulture(typeKind));

this.Write(" ");

this.Write(this.ToStringHelper.ToStringWithCulture(typeName));

this.Write(" :\r\n    IEquatable<");

this.Write(this.ToStringHelper.ToStringWithCulture(typeName));

this.Write(">\r\n{\r\n    public");

this.Write(this.ToStringHelper.ToStringWithCulture(virtualModifier));

this.Write(" bool Equals(\r\n        ");

this.Write(this.ToStringHelper.ToStringWithCulture(parameterTypeName));

this.Write(" other)\r\n    {\r\n");


        if (sourceTypeInfo.OverridesObjectEquals)
        {

this.Write("        return this.Equals((");

this.Write(this.ToStringHelper.ToStringWithCulture(nullableObjectTypeName));

this.Write(")other);\r\n");


        }
        else if (sourceTypeInfo.IsGenericComparable)
        {

this.Write("        return ((IComparable<");

this.Write(this.ToStringHelper.ToStringWithCulture(typeName));

this.Write(">)this).CompareTo(other) == 0;\r\n");


        }
        else if (sourceTypeInfo.IsNonGenericComparable)
        {

this.Write("        return ((IComparable)this).CompareTo(other) == 0;\r\n");


        }
        else
        {

this.Write("        return __EqualsCore(this, other);\r\n");


        }

this.Write("    }\r\n}\r\n");


    }

    }
}
