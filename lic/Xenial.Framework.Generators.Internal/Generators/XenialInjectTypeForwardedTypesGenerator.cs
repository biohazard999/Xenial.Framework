using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators.Internal.Generators;

internal class XenialInjectTypeForwardedTypesGenerator : XenialBaseGenerator, IXenialSourceGenerator
{
    public XenialInjectTypeForwardedTypesGenerator(IDictionary<string, string>? constantsToInject) : base(constantsToInject) { }

    public Compilation Execute(GeneratorExecutionContext context, Compilation compilation, IList<TypeDeclarationSyntax> types)
    {
        var builder = CurlyIndenter.Create();

        builder.WriteLine("using System;");
        builder.WriteLine("using System.Collections.Generic;");

        using (builder.OpenBrace("namespace Xenial"))
        {
            using (builder.OpenBrace("public static class TypeForwardedTypes"))
            {
                builder.WriteLine($"public static IDictionary<string, string> TypeForwards {{ get; }} = new Dictionary<string, string>()");
                builder.WriteLine("{");
                builder.Indent();
                foreach (var pair in ConstantsToInject)
                {
                    var escapedValue = Base64Encode(pair.Value);
                    builder.WriteLine($"[\"{pair.Key}\"] = @\"{escapedValue}\",");
                }
                builder.UnIndent();
                builder.WriteLine("};");
            }
        }

        return AddSource(context, compilation, builder, "TypeForwardedTypes");
    }

    private static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }
}
