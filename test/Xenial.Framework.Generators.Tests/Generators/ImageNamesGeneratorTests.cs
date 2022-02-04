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

    [UsesVerify]
    public class AttributeDrivenTests : PartialGeneratorTest<XenialImageNamesGenerator>
    {
        protected override XenialImageNamesGenerator CreateTargetGenerator()
            => new ImageNamesGeneratorTests().CreateTargetGenerator();

        [Fact]
        public Task SmartCommentsGeneration()
            => RunSourceTestWithAdditionalFiles(
                "ImageNamesWithSmartComments.cs",
@"namespace MyProject
    {
        [Xenial.XenialImageNames(SmartComments = true)]
        public partial class ImageNamesWithSmartComments{ }
    }",
new()
{
    new("XenialImageNames", new()
    {
        new("Images/MyImage.png"),
        new("Images/MyImage_32x32.png"),
        new("Images/MyImage_48x48.png")
    })
});

        [Fact]
        public Task ResourceAccessorsGeneration()
            => RunSourceTestWithAdditionalFiles(
                "ResourceAccessors.cs",
@"namespace MyProject
    {
        [Xenial.XenialImageNames(ResourceAccessors = true, SmartComments = true)]
        public partial class ImageNamesResourceAccessors { }
    }",
new()
{
    new("XenialImageNames", new()
    {
        new("Images/MyImage.png"),
        new("Images/MyImage_32x32.png"),
        new("Images/MyImage_48x48.png")
    })
});

        [Fact]
        public Task SizesGeneration()
            => RunSourceTestWithAdditionalFiles(
                "ImageNamesWithSizes.cs",
@"namespace MyProject
    {
        [Xenial.XenialImageNames(Sizes = true)]
        public partial class ImageNamesWithSizes { }
    }",
new()
{
    new("XenialImageNames", new()
    {

        new("Images/MyImage.png"),
        new("Images/MyImage_32x32.png"),
        new("Images/MyImage_48x48.png")
    })
});

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
new()
{
    new("XenialImageNames", new()
    {
        new("Images/MyImage.png"),
        new("Images/MySimpleFolder/MyImage.png"),
        new("Images/Images/MoreComplex/Folder/Inside/Folder/MyImage.png")
    })
});
    }
}
