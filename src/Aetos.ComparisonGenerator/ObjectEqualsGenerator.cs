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
    internal partial class ObjectEqualsGenerator : GeneratorBase
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

        string nullableObjectTypeName = sourceTypeInfo.NullableAnnotationsEnabled
            ? "object?"
            : "object";

this.Write("partial ");

this.Write(this.ToStringHelper.ToStringWithCulture(typeKind));

this.Write(" ");

this.Write(this.ToStringHelper.ToStringWithCulture(typeName));

this.Write("\r\n{\r\n    public override bool Equals(\r\n        ");

this.Write(this.ToStringHelper.ToStringWithCulture(nullableObjectTypeName));

this.Write(" other)\r\n    {\r\n        if (other is not ");

this.Write(this.ToStringHelper.ToStringWithCulture(typeName));

this.Write(" other2)\r\n        {\r\n            return false;\r\n        }\r\n\r\n        return __Equ" +
        "alsCore(this, other2);\r\n    }\r\n}\r\n");


    }

    }
}
