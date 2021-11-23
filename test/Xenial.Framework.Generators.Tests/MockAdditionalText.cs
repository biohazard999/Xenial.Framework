
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Xenial.Framework.Generators.Tests;

public class MockAdditionalText : AdditionalText
{
    public MockAdditionalText(string path) => Path = path;

    public override string Path { get; }

    public override SourceText? GetText(CancellationToken cancellationToken = default) => throw new System.NotImplementedException();
}
