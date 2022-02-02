using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xenial.Framework.Generators.Internal;

public interface IXenialSourceGenerator
{
    bool AddSource { get; }
    Compilation Execute(
        GeneratorExecutionContext context,
        Compilation compilation,
        IList<TypeDeclarationSyntax> types,
        IList<string> addedSourceFiles
    );

    bool Accepts(TypeDeclarationSyntax typeDeclarationSyntax);
}
