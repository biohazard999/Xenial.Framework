using System;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace Xenial.Framework.Generators;

public static class AttributeModifiers
{
    public const string InternalModifier = "internal";
    public const string PublicModifier = "public";

    private const string xenialAttributesVisibilityMSBuildProperty = "XenialAttributesVisibility";

    public static string GetDefaultAttributeModifier(this GeneratorExecutionContext context)
    {
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(
            $"build_property.{xenialAttributesVisibilityMSBuildProperty}",
            out var attributeModifier)
        )
        {
            return attributeModifier;
        }

        return InternalModifier;
    }
}

internal static class AttributeDataExtensions
{
    public static bool IsAttributeSet(this AttributeData attribute, string attributeName)
        => attribute.GetAttributeValue(attributeName, defaultValue: false);

    public static bool GetAttributeValue(this AttributeData attribute, string attributeName, bool defaultValue = false)
        => attribute.GetAttributeValue<bool>(attributeName, defaultValue);

    public static string GetAttributeValue(
        this AttributeData attribute,
        string attributeName,
        string defaultValue = ""
    ) => attribute.GetAttributeValue<string>(attributeName, defaultValue) ?? string.Empty;

    public static int GetAttributeValue(
        this AttributeData attribute,
        string attributeName,
        int defaultValue = 0
    ) => attribute.GetAttributeValue<int>(attributeName, defaultValue);

    public static TValue? GetAttributeValue<TValue>(
        this AttributeData attribute,
        string attributeName,
        TValue? defaultValue = default)
    {
        var namedArgument = attribute.NamedArguments.FirstOrDefault(argument => argument.Key == attributeName);

        if (namedArgument.Key == attributeName
            && namedArgument.Value.Kind is TypedConstantKind.Primitive
            && namedArgument.Value.Value is TValue value)
        {
            return value;
        }

        return defaultValue;
    }

}
