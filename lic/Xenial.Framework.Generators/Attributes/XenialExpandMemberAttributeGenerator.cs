using System;

using Xenial.Framework.Generators.Base;
using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators.Attributes;

public record XenialExpandMemberAttributeGenerator(bool AddSources = true) : XenialAttributeGenerator(AddSources)
{
    protected override string AttributeName => "XenialExpandMemberAttribute";

    protected override CurlyIndenter CreateAttribute(CurlyIndenter syntaxWriter, string visibility)
    {
        _ = syntaxWriter ?? throw new ArgumentNullException(nameof(syntaxWriter));

        syntaxWriter.WriteLine($"using System;");
        syntaxWriter.WriteLine($"using System.ComponentModel;");
        syntaxWriter.WriteLine($"using System.Runtime.CompilerServices;");
        syntaxWriter.WriteLine();

        using (syntaxWriter.OpenBrace($"namespace {XenialNamespace}"))
        {
            syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]");
            syntaxWriter.WriteLine("[CompilerGenerated]");

            using (syntaxWriter.OpenBrace($"{visibility} sealed class XenialExpandMemberAttribute : Attribute"))
            {
                syntaxWriter.WriteLine($"public string ExpandMember {{ get; private set; }}");
                syntaxWriter.WriteLine();
                using (syntaxWriter.OpenBrace($"{visibility} XenialExpandMemberAttribute(string expandMember)"))
                {
                    syntaxWriter.WriteLine($"this.ExpandMember = expandMember;");
                }
            }
        }

        return syntaxWriter;
    }
}
