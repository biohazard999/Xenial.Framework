using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;

namespace Xenial.Framework.Generators;

public static class AttributeModifiers
{
    public const string InternalModifier = "internal";
    public const string PublicModifier = "public";

    private const string xenialAttributesModifierMSBuildProperty = "XenialAttributesModifier";

    public static string GetDefaultAttributeModifier(this GeneratorExecutionContext context)
    {
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{xenialAttributesModifierMSBuildProperty}", out var attributeModifier))
        {
            return attributeModifier;
        }

        return InternalModifier;
    }
}
