using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xenial.Framework.Generators.Base;
public record TargetSymbol(INamedTypeSymbol Symbol, TypeDeclarationSyntax TypeDeclarationSyntax)
{
    public INamespaceSymbol Namespace => Symbol.ContainingNamespace;

    public bool HasBaseClasses(IEnumerable<INamedTypeSymbol> baseTypes) =>
        baseTypes.Any(baseType => HasBaseClass(baseType));

    public bool HasBaseClass(INamedTypeSymbol baseType)
        => HasBase(Symbol, baseType);

    private static bool HasBase(INamedTypeSymbol symbol, INamedTypeSymbol baseType)
    {
        if (symbol.BaseType is null)
        {
            return false;
        }

        if (symbol.BaseType.ToString() == baseType.ToString())
        {
            return true;
        }
        return HasBase(symbol.BaseType, baseType);
    }
}
