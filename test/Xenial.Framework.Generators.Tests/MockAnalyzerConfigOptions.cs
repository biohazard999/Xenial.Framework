using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

namespace Xenial.Framework.Generators.Tests;

internal sealed class MockAnalyzerConfigOptions : AnalyzerConfigOptions
{
    public static MockAnalyzerConfigOptions Empty { get; } = new MockAnalyzerConfigOptions();

    private MockAnalyzerConfigOptions()
    {
    }

    public MockAnalyzerConfigOptions(string key, string value)
        => (this.key, this.value) = (key, value);

    private readonly string? key;
    private readonly string? value;

    public override bool TryGetValue(string key,
#if !FULL_FRAMEWORK
        [NotNullWhen(true)]
#endif
    out string
        #if !FULL_FRAMEWORK
?
        #endif
        value
)
    {
        if (this.key is not null && key is not null && this.key == key && this.value is not null)
        {
            value = this.value;
            return true;
        }
        value = null;
        return false;
    }
}

