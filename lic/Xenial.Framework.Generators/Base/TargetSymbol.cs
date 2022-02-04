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
}
