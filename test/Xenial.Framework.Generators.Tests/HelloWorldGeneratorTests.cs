using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using VerifyTests;

using VerifyXunit;

using Xunit;

namespace Xenial.Framework.Generators.Tests
{

    [UsesVerify]
    public class HelloWorldGeneratorTests
    {
        //static HelloWorldGeneratorTests() => VerifySourceGenerators.Enable();

        [Fact]
        public async Task Run()
        {
            var compilation = CSharpCompilation.Create("name");
            HelloWorldGenerator generator = new();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGenerators(compilation);

            await Verifier.Verify(driver);
        }
    }
}
