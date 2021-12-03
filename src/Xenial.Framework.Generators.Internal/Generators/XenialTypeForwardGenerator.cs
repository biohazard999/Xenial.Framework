using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators.Internal.Generators;

public class XenialTypeForwardGenerator : XenialBaseGenerator, IXenialSourceGenerator
{
    public XenialTypeForwardGenerator(IDictionary<string, string>? constantsToInject) : base(constantsToInject) { }

    public Compilation Execute(GeneratorExecutionContext context, Compilation compilation, IList<TypeDeclarationSyntax> types)
        => AddTypeForwardAttribute(context, compilation);

    private Compilation AddTypeForwardAttribute(
         GeneratorExecutionContext context,
         Compilation compilation)
    {
        var builder = CurlyIndenter.Create();

        builder.WriteLine($"using System;");
        builder.WriteLine();

        using (builder.OpenBrace($"namespace {XenialNamespace}"))
        {
            builder.WriteLine("[AttributeUsage(AttributeTargets.All, Inherited = false)]");
            using (builder.OpenBrace($"internal sealed class XenialTypeForward : Attribute"))
            {
                builder.WriteLine($"public string TargetType {{ get; }}");
                builder.WriteLine();

                using (builder.OpenBrace($"internal XenialTypeForward(string targetType)"))
                {
                    builder.WriteLine("this.TargetType = targetType;");
                }

            }
        }

        ConstantsToInject[$"XenialTypeForward"] = builder.ToString();

        return AddSource(context, compilation, builder, "XenialTypeForward");
    }
}
