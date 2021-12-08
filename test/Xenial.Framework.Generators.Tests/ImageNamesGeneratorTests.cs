using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using VerifyXunit;

using Xunit;

namespace Xenial.Framework.Generators.Tests;

[UsesVerify]
public class ImageNamesGeneratorTests : BaseGeneratorTests<XenialImageNamesGenerator>
{
    protected override XenialImageNamesGenerator CreateTargetGenerator() => new();

    protected override string GeneratorEmitProperty => XenialImageNamesGenerator.GenerateXenialImageNamesAttributeMSBuildProperty;

    protected Task RunSourceTest(string fileName, string source)
        => RunTest(
            options => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(GeneratorEmitProperty), "false")),
            compilationOptions: compilation => compilation.AddInlineXenialImageNamesAttribute(),
            syntaxTrees: () => new[]
            {
                BuildSyntaxTree(fileName, source)
            });

    protected Task RunSourceTestWithAdditionalFiles(string fileName, string source, string[] additionalFiles, string? typeToLoad = null)
        => RunSourceTestWithAdditionalFiles(fileName, source, additionalFiles.Select(f => new MockAdditionalText(f)), typeToLoad);

    protected Task RunSourceTestWithAdditionalFiles(string fileName, string source, IEnumerable<MockAdditionalText> additionalFiles, string? typeToLoad = null)
        => RunTest(
            options => options
                .WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(GeneratorEmitProperty), "false"))
                .WithAdditionalTreeOptions(
                    additionalFiles.ToImmutableDictionary(k => (object)k, _ => (AnalyzerConfigOptions)new MockAnalyzerConfigOptions("build_metadata.AdditionalFiles.XenialImageNames", "true"))
                ),
            compilationOptions: compilation => compilation.AddInlineXenialImageNamesAttribute(),
            syntaxTrees: () => new[]
            {
                BuildSyntaxTree(fileName, source)
            },
            additionalTexts: () => additionalFiles,
            typeToLoad: typeToLoad);

    [Fact]
    public Task DoesEmitDiagnosticIfNotPartial()
        => RunSourceTest("MyNonPartialClass.cs",
@"using Xenial;
namespace MyProject
{
    [XenialImageNames(Foo = 123)]
    public class MyNonPartialClass{ }
}");

    [Fact]
    public Task DoesEmitDiagnosticIfInGlobalNamespace()
        => RunSourceTest("MyNonPartialClass.cs",
@"using Xenial;
[XenialImageNames(Foo = 123)]
public partial class MyGlobalClass
{
}");

    [Fact]
    public Task DoesNotEmitDiagnosticIfPartial()
        => RunSourceTest("MyPartialClass.cs",
@"namespace MyProject
{
    [Xenial.XenialImageNames]
    public partial class MyPartialClass { }
}");

    [Fact]
    public Task DoesNotEmitDiagnosticIfAttributeIsNotApplied()
        => RunSourceTest(string.Empty,
@"namespace MyProject
{
    [System.Obsolete]
    public class MyPartialClassWithoutAttribute{ }
}");

    [Fact]
    public Task BasicConstantGeneration()
        => RunSourceTestWithAdditionalFiles("BasicImageNames.cs",
@"namespace MyProject
{
    [Xenial.XenialImageNames]
    public partial class BasicImageNames { }
}",
            new[]
            {
                "Images/MyPicture.png"
            });

    [Fact]
    public Task BasicConstantGenerationWithRecord()
        => RunSourceTestWithAdditionalFiles("BasicImageNamesWithRecord.cs",
@"namespace MyProject
{
    [Xenial.XenialImageNames]
    public partial record BasicImageNamesRecord { }
}",
            new[]
            {
                "Images/MyPicture.png"
            });

    [UsesVerify]
    public class AttributeDrivenTests
    {
        private static Task RunSourceTestWithAdditionalFiles(string fileName, string source, string[] additionalFiles, string? typeToLoad = null)
            => new ImageNamesGeneratorTests()
                .RunSourceTestWithAdditionalFiles(fileName, source, additionalFiles, typeToLoad);

        [Fact]
        public Task SmartCommentsGeneration()
            => RunSourceTestWithAdditionalFiles(
                "ImageNamesWithSmartComments.cs",
@"namespace MyProject
{
    [Xenial.XenialImageNames(SmartComments = true)]
    public partial class ImageNamesWithSmartComments{ }
}",
                new[]
                {
                    "Images/MyImage.png",
                    "Images/MyImage_32x32.png",
                    "Images/MyImage_48x48.png"
                },
                "MyProject.ImageNamesWithSizes"
            );

        [Fact]
        public Task ResourceAccessorsGeneration()
            => RunSourceTestWithAdditionalFiles(
                "ResourceAccessors.cs",
@"namespace MyProject
{
    [Xenial.XenialImageNames(ResourceAccessors = true, SmartComments = true)]
    public partial class ImageNamesResourceAccessors { }
}",
                new[]
                {
                    "Images/MyImage.png",
                    "Images/MyImage_32x32.png",
                    "Images/MyImage_48x48.png"
                },
                "MyProject.ImageNamesResourceAccessors"
            );

        [Fact]
        public Task SizesGeneration()
            => RunSourceTestWithAdditionalFiles(
                "ImageNamesWithSizes.cs",
@"namespace MyProject
{
    [Xenial.XenialImageNames(Sizes = true)]
    public partial class ImageNamesWithSizes { }
}",
                new[]
                {
                    "Images/MyImage.png",
                    "Images/MyImage_32x32.png",
                    "Images/MyImage_48x48.png"
                },
                "MyProject.ImageNamesWithSizes"
            );

        [Fact(
            Skip = "Currently not Supported"
        )]
        public Task SubFolderBasic()
            => RunSourceTestWithAdditionalFiles(
                "ImageNamesWithSizes.cs",
@"namespace MyProject
{
    [Xenial.XenialImageNames(SmartComments = true)]
    public partial class SubFolderImages { }
}",
                new[]
                {
                    "Images/MyImage.png",
                    "Images/MySimpleFolder/MyImage.png",
                    "Images/Images/MoreComplex/Folder/Inside/Folder/MyImage.png"
                },
                "MyProject.SubFolderImages"
            );
    }
}

internal static partial class CompilationHelpers
{
    public static CSharpCompilation AddInlineXenialImageNamesAttribute(this CSharpCompilation compilation, string visibility = "internal")
    {
        (_, var syntaxTree) = XenialImageNamesGenerator.GenerateXenialImageNamesAttribute(visibility: visibility);

        return compilation.AddSyntaxTrees(syntaxTree);
    }
}
