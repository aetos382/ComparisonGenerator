﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン: 16.0.0.0
//  
//     このファイルへの変更は、正しくない動作の原因になる可能性があり、
//     コードが再生成されると失われます。
// </auto-generated>
// ------------------------------------------------------------------------------
namespace ComparableGenerator
{
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    internal partial class CommonGenerator : GeneratorBase
    {
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {

    base.TransformText();

            return this.GenerationEnvironment.ToString();
        }

    protected override void WriteUsings()
    {

this.Write("using System.Collections.Generic;\r\nusing System.ComponentModel;\r\n");


    }

    protected override void WriteCode()
    {
        var context = this.Context;
        var type = context.Type;

        string typeName = type.Name;
        string typeKind = GetTypeKind(type);

        var sourceType = context.SourceType;
        var options = context.Options;

        string nullableTypeName = context.NullableTypeName;

        var isValueType = sourceType.IsValueType;
        string dotValue = isValueType ? ".Value" : "";

this.Write("partial ");

this.Write(this.ToStringHelper.ToStringWithCulture(typeKind));

this.Write(" ");

this.Write(this.ToStringHelper.ToStringWithCulture(typeName));

this.Write("\r\n{\r\n    [EditorBrowsable(EditorBrowsableState.Advanced)]\r\n    private static boo" +
        "l __EqualsCore(\r\n        ");

this.Write(this.ToStringHelper.ToStringWithCulture(nullableTypeName));

this.Write(" left,\r\n        ");

this.Write(this.ToStringHelper.ToStringWithCulture(nullableTypeName));

this.Write(" right)\r\n    {\r\n");


        if (!isValueType)
        {

this.Write("        if (object.ReferenceEquals(left, right))\r\n        {\r\n            return t" +
        "rue;\r\n        }\r\n");


        }


        if (context.IsNullable)
        {

this.Write("        if (left is null || right is null)\r\n        {\r\n            return false;\r" +
        "\n        }\r\n");


            }

this.Write("\r\n        bool result;\r\n");


            foreach (var member in context.Members)
            {
                string memberName = member.Name;

this.Write("\r\n        result = EqualityComparer<");

this.Write(this.ToStringHelper.ToStringWithCulture(member.TypeName));

this.Write(">.Default.Equals(left");

this.Write(this.ToStringHelper.ToStringWithCulture(dotValue));

this.Write(".");

this.Write(this.ToStringHelper.ToStringWithCulture(memberName));

this.Write(", right");

this.Write(this.ToStringHelper.ToStringWithCulture(dotValue));

this.Write(".");

this.Write(this.ToStringHelper.ToStringWithCulture(memberName));

this.Write(");\r\n        if (!result)\r\n        {\r\n            return result;\r\n        }\r\n");


            }


this.Write("\r\n        return true;\r\n    }\r\n\r\n    [EditorBrowsable(EditorBrowsableState.Advanc" +
        "ed)]\r\n    private static int __CompareCore(\r\n        ");

this.Write(this.ToStringHelper.ToStringWithCulture(nullableTypeName));

this.Write(" left,\r\n        ");

this.Write(this.ToStringHelper.ToStringWithCulture(nullableTypeName));

this.Write(" right)\r\n    {\r\n");


            if (!isValueType)
            {

this.Write("        if (object.ReferenceEquals(left, right))\r\n        {\r\n            return 0" +
        ";\r\n        }\r\n");


            }

            if (context.IsNullable)
            {

this.Write("        if (left is null)\r\n        {\r\n            return int.MinValue;\r\n        }" +
        "\r\n\r\n        if (right is null)\r\n        {\r\n            return int.MaxValue;\r\n   " +
        "     }\r\n");


            }

this.Write("\r\n        int result;\r\n");


            foreach (var member in context.Members)
            {
                string memberName = member.Name;

this.Write("\r\n        result = Comparer<");

this.Write(this.ToStringHelper.ToStringWithCulture(member.TypeName));

this.Write(">.Default.Compare(left");

this.Write(this.ToStringHelper.ToStringWithCulture(dotValue));

this.Write(".");

this.Write(this.ToStringHelper.ToStringWithCulture(memberName));

this.Write(", right");

this.Write(this.ToStringHelper.ToStringWithCulture(dotValue));

this.Write(".");

this.Write(this.ToStringHelper.ToStringWithCulture(memberName));

this.Write(");\r\n        if (result != 0)\r\n        {\r\n            return result;\r\n        }\r\n");


            }


this.Write("\r\n        return 0;\r\n    }\r\n}\r\n");


    }

    }
}
