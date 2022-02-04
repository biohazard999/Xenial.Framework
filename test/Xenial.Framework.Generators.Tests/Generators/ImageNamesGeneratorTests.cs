using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using VerifyXunit;

using Xenial.Framework.Generators.Partial;
using Xenial.Framework.Generators.Tests.Base;

using Xunit;

namespace Xenial.Framework.Generators.Tests.Generators;

[UsesVerify]
public class ImageNamesGeneratorTests : PartialGeneratorTest<XenialImageNamesGenerator>
{
    protected override XenialImageNamesGenerator CreateTargetGenerator()
        => new();

    //protected Task RunSourceTestWithAdditionalFiles(string fileName, string source, string[] additionalFiles, string? typeToLoad = null)
    //    => RunSourceTestWithAdditionalFiles(fileName, source, additionalFiles.Select(f => new MockAdditionalText(f)), typeToLoad);

    //protected Task RunSourceTestWithAdditionalFiles(string fileName, string source, IEnumerable<MockAdditionalText> additionalFiles, string? typeToLoad = null)
    //    => RunTest(
    //        options => options
    //            .WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(GeneratorEmitProperty), "false"))
    //            .WithAdditionalTreeOptions(
    //                additionalFiles.ToImmutableDictionary(k => (object)k, _ => (AnalyzerConfigOptions)new MockAnalyzerConfigOptions("build_metadata.AdditionalFiles.XenialImageNames", "true"))
    //            ),
    //        compilationOptions: compilation => compilation.AddInlineXenialImageNamesAttribute(),
    //        syntaxTrees: () => new[]
    //        {
    //            BuildSyntaxTree(fileName, source)
    //        },
    //        additionalTexts: () => additionalFiles,
    //        typeToLoad: typeToLoad);

    public Task RunSourceTest(string fileName, string source)
        => RunTest(o => o with
        {
            SyntaxTrees = o => new[]
            {
                o.BuildSyntaxTree(fileName, source)
            },
            Compile = false
        });

    public Task RunSourceTestWithAdditionalFiles(string fileName, string source, List<AdditionalFiles> additionalFiles)
        => RunTest(options => options with
        {
            SyntaxTrees = o => new[]
            {
                o.BuildSyntaxTree(fileName, source)
            },
            AdditionalFiles = additionalFiles,
            Compile = false
        });

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
new()
{
    new("XenialImageNames", new()
    {
        new("Images/MyPicture.png")
    })
});

    [Fact]
    public Task BasicConstantGenerationWithRecord()
        => RunSourceTestWithAdditionalFiles("BasicImageNamesWithRecord.cs",
@"namespace MyProject
{
    [Xenial.XenialImageNames]
    public partial record BasicImageNamesRecord { }
}",
new()
{
    new("XenialImageNames", new()
    {
        new("Images/MyPicture.png")
    })
});

    //    [UsesVerify]
    //    public class AttributeDrivenTests
    //    {
    //        private static Task RunSourceTestWithAdditionalFiles(string fileName, string source, string[] additionalFiles, string? typeToLoad = null)
    //            => new ImageNamesGeneratorTests()
    //                .RunSourceTestWithAdditionalFiles(fileName, source, additionalFiles, typeToLoad);

    //        [Fact]
    //        public Task SmartCommentsGeneration()
    //            => RunSourceTestWithAdditionalFiles(
    //                "ImageNamesWithSmartComments.cs",
    //@"namespace MyProject
    //{
    //    [Xenial.XenialImageNames(SmartComments = true)]
    //    public partial class ImageNamesWithSmartComments{ }
    //}",
    //                new[]
    //                {
    //                    "Images/MyImage.png",
    //                    "Images/MyImage_32x32.png",
    //                    "Images/MyImage_48x48.png"
    //                },
    //                "MyProject.ImageNamesWithSizes"
    //            );

    //        [Fact]
    //        public Task ResourceAccessorsGeneration()
    //            => RunSourceTestWithAdditionalFiles(
    //                "ResourceAccessors.cs",
    //@"namespace MyProject
    //{
    //    [Xenial.XenialImageNames(ResourceAccessors = true, SmartComments = true)]
    //    public partial class ImageNamesResourceAccessors { }
    //}",
    //                new[]
    //                {
    //                    "Images/MyImage.png",
    //                    "Images/MyImage_32x32.png",
    //                    "Images/MyImage_48x48.png"
    //                },
    //                "MyProject.ImageNamesResourceAccessors"
    //            );

    //        [Fact]
    //        public Task SizesGeneration()
    //            => RunSourceTestWithAdditionalFiles(
    //                "ImageNamesWithSizes.cs",
    //@"namespace MyProject
    //{
    //    [Xenial.XenialImageNames(Sizes = true)]
    //    public partial class ImageNamesWithSizes { }
    //}",
    //                new[]
    //                {
    //                    "Images/MyImage.png",
    //                    "Images/MyImage_32x32.png",
    //                    "Images/MyImage_48x48.png"
    //                },
    //                "MyProject.ImageNamesWithSizes"
    //            );

    //        [Fact(
    //            Skip = "Currently not Supported"
    //        )]
    //        public Task SubFolderBasic()
    //            => RunSourceTestWithAdditionalFiles(
    //                "ImageNamesWithSizes.cs",
    //@"namespace MyProject
    //{
    //    [Xenial.XenialImageNames(SmartComments = true)]
    //    public partial class SubFolderImages { }
    //}",
    //                new[]
    //                {
    //                    "Images/MyImage.png",
    //                    "Images/MySimpleFolder/MyImage.png",
    //                    "Images/Images/MoreComplex/Folder/Inside/Folder/MyImage.png"
    //                },
    //                "MyProject.SubFolderImages"
    //            );
    //    }
}
