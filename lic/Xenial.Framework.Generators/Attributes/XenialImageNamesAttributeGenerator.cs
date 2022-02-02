using System;

using Xenial.Framework.Generators.Base;
using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators.Attributes;

public record XenialImageNamesAttributeGenerator(bool AddSources = true) : XenialAttributeGenerator(AddSources)
{
    public override string AttributeName => "XenialImageNamesAttribute";

    protected override CurlyIndenter CreateAttribute(CurlyIndenter syntaxWriter, string visibility)
    {
        _ = syntaxWriter ?? throw new ArgumentNullException(nameof(syntaxWriter));

        syntaxWriter.WriteLine($"using System;");
        syntaxWriter.WriteLine($"using System.ComponentModel;");
        syntaxWriter.WriteLine();

        using (syntaxWriter.OpenBrace($"namespace {XenialNamespace}"))
        {
            syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]");

            using (syntaxWriter.OpenBrace($"{visibility} sealed class {AttributeName} : Attribute"))
            {
                syntaxWriter.WriteLine($"{visibility} {AttributeName}() {{ }}");

                syntaxWriter.WriteLine();

                //Properties need to be public in order to be used
                syntaxWriter.WriteLine($"public bool {AttributeNames.Sizes} {{ get; set; }}");
                syntaxWriter.WriteLine($"public bool {AttributeNames.SmartComments} {{ get; set; }}");
                syntaxWriter.WriteLine($"public bool {AttributeNames.ResourceAccessors} {{ get; set; }}");

                syntaxWriter.WriteLine("[EditorBrowsable(EditorBrowsableState.Never)]");
                syntaxWriter.WriteLine($"public string {AttributeNames.DefaultImageSize} {{ get; set; }} = \"{AttributeNames.DefaultImageSizeValue}\";");
            }
        }

        return syntaxWriter;
    }
}
