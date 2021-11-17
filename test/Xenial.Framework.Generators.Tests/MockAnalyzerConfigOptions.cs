using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

namespace Xenial.Framework.Generators.Tests;

internal sealed class MockAnalyzerConfigOptions : AnalyzerConfigOptions
{
    public static MockAnalyzerConfigOptions Empty { get; } = new MockAnalyzerConfigOptions();

    private readonly string? key;
    private readonly string? value;

    private MockAnalyzerConfigOptions() { }

    public MockAnalyzerConfigOptions(string key, string value)
        => (this.key, this.value) = (key, value);

#if FULL_FRAMEWORK
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
#endif
    public override bool TryGetValue(string key,
#if !FULL_FRAMEWORK
        [NotNullWhen(true)]
#endif
        out string? value
    )
    {
        if (this.key is not null
            && key is not null
            && this.key == key
            && this.value is not null
        )
        {
            value = this.value;
            return true;
        }
        value = null;
        return false;
    }
#if FULL_FRAMEWORK
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
#endif
}

