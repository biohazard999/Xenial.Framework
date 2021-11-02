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
#if FULL_FRAMEWORK || NETCOREAPP3_1
        static HelloWorldGeneratorTests() => VerifySourceGenerators.Enable();
#endif

        [Fact]
        public async Task Run()
        {
            var compilation = CSharpCompilation.Create("name");
            HelloWorldGenerator generator = new();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGenerators(compilation);
            var settings = new VerifySettings();
            settings.UniqueForTargetFrameworkAndVersion();
            await Verifier.Verify(driver, settings);
        }
    }
}
