
using Microsoft.CodeAnalysis;

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using VerifyTests;

using VerifyXunit;

namespace Xenial.Framework.Generators.Tests.Base;

internal class GeneratorTest
{
    public static (
        GeneratorTestOptions options,
        Compilation compilation,
        XenialGenerator generator,
        GeneratorDriver driver
    ) PrepareTest(Func<GeneratorTestOptions, GeneratorTestOptions> optionsFunctions)
    {
        var options = optionsFunctions(new());
        var compilation = options.CreateCompilation(options);
        var generator = options.CreateGenerator(options);

        options = options with
        {
            MockOptionsProvider = options.MockOptions is not null
                ? options.MockOptions(options)
                : options.MockOptionsProvider
        };

        var driver = options.CreateDriver(options, generator);
        return (options, compilation, generator, driver);
    }

    public static async Task RunTest(
        Func<GeneratorTestOptions, GeneratorTestOptions> optionsFunctions,
        [CallerFilePath] string filePath = ""
    )
    {
        var (options, compilation, generator, driver) = PrepareTest(optionsFunctions);

        driver = driver.RunGenerators(compilation);

        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        if (!string.IsNullOrEmpty(filePath))
        {
            settings.UseDirectory(Path.GetDirectoryName(filePath)!);
        }
        options.VerifySettings?.Invoke(options, settings);
        await Verifier.Verify(driver, settings);
    }

    public static void Compile(Func<GeneratorTestOptions, GeneratorTestOptions> optionsFunctions)
    {
        var (options, compilation, generator, driver) = PrepareTest(optionsFunctions);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out compilation, out var diagnositcs);

        VerifyDiagnostics(diagnositcs, null);
    }

    private static void VerifyDiagnostics(ImmutableArray<Diagnostic> diagnostics, Exception? ex)
    {
        if (diagnostics.Length > 0)
        {
            var argumentException = new ArgumentException(string.Join(
                Environment.NewLine,
                diagnostics.Select(diag => new DiagnosticFormatter().Format(diag))
            ));

            throw ex is null
                ? new AggregateException(argumentException)
                : new AggregateException(argumentException, ex);
        }
    }
}
