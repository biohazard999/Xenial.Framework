
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using System.Runtime.CompilerServices;

namespace Xenial.Framework.Generators.Base;

public abstract record XenialPartialGenerator(bool AddSource = true) : XenialBaseGenerator(AddSource)
{
    public virtual IEnumerable<XenialAttributeGenerator> DependsOnGenerators { get; }
        = Enumerable.Empty<XenialAttributeGenerator>();


    protected static bool FindAttributeFromGenerator(
        Compilation compilation,
        XenialAttributeGenerator generator,
        out Compilation comp
    )
    {
        _ = compilation ?? throw new ArgumentNullException(nameof(compilation));
        _ = generator ?? throw new ArgumentNullException(nameof(generator));
        var attrib = compilation.GetTypeByMetadataName(generator.AttributeFullName);

        comp = compilation;

        if (attrib is not null)
        {
            //TODO: Warning Diagnostics for either setting the right MSBuild properties or referencing `Xenial.Framework.CompilerServices`
            return true;
        }
        return false;
    }

    protected static INamedTypeSymbol GetAttributeFromGenerator(
       Compilation compilation,
       XenialAttributeGenerator generator,
       [CallerFilePath] string filePath = "",
       [CallerLineNumber] int lineNumber = 0
    )
    {
        _ = compilation ?? throw new ArgumentNullException(nameof(compilation));
        _ = generator ?? throw new ArgumentNullException(nameof(generator));

        var attrib = compilation.GetTypeByMetadataName(generator.AttributeFullName);

        var message = $"Can not find requested {generator.AttributeName} from {filePath}:{lineNumber}";
        Debug.Assert(attrib is not null, message);
        _ = attrib ?? throw new InvalidOperationException(message);

        return attrib;
    }

    public static bool IsTargetPartial(GeneratorExecutionContext context, TargetSymbol symbol, INamedTypeSymbol attribute)
    {
        _ = symbol ?? throw new ArgumentNullException(nameof(symbol));
        _ = attribute ?? throw new ArgumentNullException(nameof(attribute));

        var isAttributeDeclared = symbol.Symbol.IsAttributeDeclared(attribute);

        if (isAttributeDeclared && !symbol.TypeDeclarationSyntax.HasModifier(SyntaxKind.PartialKeyword))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    GeneratorDiagnostics.ClassNeedsToBePartialWhenUsingAttribute(attribute.Name),
                    symbol.TypeDeclarationSyntax.GetLocation()
            ));

            return false;
        }
        return true;
    }

    protected static bool TryGetTarget(
        GeneratorExecutionContext context,
        Compilation compilation,
        TypeDeclarationSyntax @class,
        out TargetSymbol targetSymbol
    )
    {
        _ = compilation ?? throw new ArgumentNullException(nameof(compilation));
        _ = @class ?? throw new ArgumentNullException(nameof(@class));

        var semanticModel = compilation.GetSemanticModel(@class.SyntaxTree);
        if (semanticModel is null)
        {
            targetSymbol = null!;
            return false;
        }

        var symbol = semanticModel.GetDeclaredSymbol(@class, context.CancellationToken);

        if (symbol is null)
        {
            targetSymbol = null!;
            return false;
        }

        targetSymbol = new(symbol, @class);
        return true;
    }

    protected static bool TryGetTargetWithAttribute(
        GeneratorExecutionContext context,
        Compilation compilation,
        TypeDeclarationSyntax @class,
        INamedTypeSymbol attribute,
        out TargetSymbol targetSymbol
    )
    {
        if (TryGetTarget(context, compilation, @class, out targetSymbol))
        {
            if (targetSymbol.Symbol.IsAttributeDeclared(attribute))
            {
                return true;
            }
        }

        return false;
    }
}
