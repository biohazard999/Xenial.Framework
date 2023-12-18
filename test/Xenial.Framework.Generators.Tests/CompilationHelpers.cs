
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Xenial.Framework.Generators.Tests;

internal static partial class CompilationHelpers
{
    public static (
        GeneratorDriver driver,
        ImmutableArray<Diagnostic> diagnostics,
        Exception? emitException,
        Type? generatedType
    ) CompileAndLoadType(
        this GeneratorDriver driver,
        Compilation compilation,
        string typeToLoad
    )
    {
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var compilation2, out var diag);

        using var stream = new MemoryStream();
        var emitResults = compilation2.Emit(stream);
        if (emitResults is not null)
        {
            stream.Position = 0;
#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                var assembly = Assembly.Load(stream.ToArray());

                return (driver, emitResults.Diagnostics, null, assembly.GetType(typeToLoad));
            }
            catch (Exception ex)
            {
                return (driver, emitResults.Diagnostics, ex, null);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        return (driver, ImmutableArray<Diagnostic>.Empty, null, null);

    }
}
