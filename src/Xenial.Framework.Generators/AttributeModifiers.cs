using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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


public static class TypeSymbolExtensions
{
    internal static SymbolVisibility GetResultantVisibility(this ISymbol symbol)
    {
        _ = symbol ?? throw new ArgumentNullException(nameof(symbol));

        // Start by assuming it's visible.
        var visibility = SymbolVisibility.Public;

        switch (symbol.Kind)
        {
            case SymbolKind.Alias:
                // Aliases are uber private.  They're only visible in the same file that they
                // were declared in.
                return SymbolVisibility.Private;

            case SymbolKind.Parameter:
                // Parameters are only as visible as their containing symbol
                return GetResultantVisibility(symbol.ContainingSymbol);

            case SymbolKind.TypeParameter:
                // Type Parameters are private.
                return SymbolVisibility.Private;
        }

        while (symbol != null && symbol.Kind != SymbolKind.Namespace)
        {
            switch (symbol.DeclaredAccessibility)
            {
                // If we see anything private, then the symbol is private.
                case Accessibility.NotApplicable:
                case Accessibility.Private:
                    return SymbolVisibility.Private;

                // If we see anything internal, then knock it down from public to
                // internal.
                case Accessibility.Internal:
                case Accessibility.ProtectedAndInternal:
                    visibility = SymbolVisibility.Internal;
                    break;

                    // For anything else (Public, Protected, ProtectedOrInternal), the
                    // symbol stays at the level we've gotten so far.
            }

            symbol = symbol.ContainingSymbol;
        }

        return visibility;
    }

    public static bool IsAttributeDeclared(this INamedTypeSymbol symbol, INamedTypeSymbol attributeSymbol)
        => symbol == null ? false : symbol
            .GetAttributes()
            .Any(m =>
                m.AttributeClass is not null
                //We can safely compare with ToString because it represents just the NamedTypeSymbol, not the attributes or overloads
                && m.AttributeClass.ToString() == attributeSymbol.ToString()
             );

    public static AttributeData GetAttribute(this INamedTypeSymbol symbol, INamedTypeSymbol attributeSymbol)
    {
        _ = symbol ?? throw new ArgumentNullException(nameof(symbol));

        return symbol.GetAttributes().First(m =>
            m.AttributeClass is not null
            //We can safely compare with ToString because it represents just the NamedTypeSymbol, not the attributes or overloads
            && m.AttributeClass.ToString() == attributeSymbol.ToString()
        );
    }

    public static bool HasModifier(this ClassDeclarationSyntax @class, SyntaxKind kind)
        => @class == null
            ? false
            : @class.Modifiers.Any(mod => mod.Text == SyntaxFactory.Token(kind).Text);
}
