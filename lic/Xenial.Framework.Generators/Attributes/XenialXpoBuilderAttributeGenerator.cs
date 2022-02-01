﻿using System;

using Xenial.Framework.Generators.Base;
using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators.Attributes;

public record XenialXpoBuilderAttributeGenerator(bool AddSources = true) : XenialAttributeGenerator(AddSources)
{
    protected override string AttributeName => "XenialXpoBuilderAttribute";

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
            }
        }

        return syntaxWriter;
    }
}
