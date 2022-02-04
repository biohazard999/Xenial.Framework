using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp;

using VerifyTests;

using VerifyXunit;

using Xenial.Framework.Generators.Partial;

using Xunit;

namespace Xenial.Framework.Generators.Tests.Generators;

[UsesVerify]
public class ActionsGeneratorTests : PartialGeneratorTest<XenialActionGenerator>
{
    private static XenialActionGeneratorOutputOptions OnlyController => new(Attribute: false, PartialBuddy: false, Controller: true);
    private static XenialActionGeneratorOutputOptions OnlyDiagnostics => new(Attribute: false, PartialBuddy: false, Controller: false);
    private static XenialActionGeneratorOutputOptions PartialBuddyAndController => new(Attribute: false, PartialBuddy: true, Controller: true);

    protected override XenialActionGenerator CreateTargetGenerator() => new(new());

    public Task RunSourceTest(string fileName, string source, Action<VerifySettings>? verifySettings = null, XenialActionGeneratorOutputOptions? outputOptions = null, [CallerFilePath] string filePath = "")
        => RunTest(o => o with
        {
            SyntaxTrees = o => new[]
            {
                o.BuildSyntaxTree(fileName, source)
            },
            PrepareGenerator = o => o with { OutputOptions = outputOptions ?? o.OutputOptions },
            VerifySettings = (o, settings) => verifySettings?.Invoke(settings),
            Compile = false
        }, filePath);

    [Fact]
    public Task WarnsIfClassIsNotPartial()
        => RunSourceTest("ClassShouldBePartial",
@"namespace MyActions
{
    [Xenial.XenialAction]
    public class ClassShouldBePartial { }
}");

    [Fact]
    public Task GeneratesSimpleActionWhenDefined()
        => RunSourceTest("GeneratesSimpleActionWhenDefined",
@"namespace MyActions
{
    [Xenial.XenialAction]
    public partial class GeneratesSimpleActionWhenDefined { }
}", outputOptions: PartialBuddyAndController);

    [Theory]
    [InlineData("Caption", "MappedCaption")]
    [InlineData("ImageName", "MappedImageName")]
    [InlineData("DiagnosticInfo", "MappedDiagnosticInfo")]
    [InlineData("ConfirmationMessage", "MappedConfirmationMessage")]
    [InlineData("ToolTip", "MappedToolTip")]
    [InlineData("Shortcut", "MappedShortcut")]
    public Task GeneratesSimpleStringMappingProperties(string propertyName, string value)
    => RunSourceTest("GeneratesSimpleActionWhenDefined",
$@"namespace MyActions
{{
    [Xenial.XenialAction({propertyName} = ""{value}"")]
    public partial class GeneratesSimpleActionWhenDefined {{ }}
}}", verifySettings: settings => settings.UseParameters(propertyName, value), outputOptions: OnlyController);

    [Theory]
    [InlineData("QuickAccess", "false")]
    [InlineData("QuickAccess", "true")]
    public Task GeneratesSimpleBoolMappingProperties(string propertyName, string value)
        => RunSourceTest("GeneratesSimpleActionWhenDefined",
$@"namespace MyActions
{{
    [Xenial.XenialAction({propertyName} = {value})]
    public partial class GeneratesSimpleActionWhenDefined {{ }}
}}", verifySettings: settings => settings.UseParameters(propertyName, value), outputOptions: OnlyController);

    [Theory]
    [InlineData("Tag", "true")]
    [InlineData("Tag", "false")]
    [InlineData("Tag", "\"SomeString\"")]
    [InlineData("Tag", "42")]
    [InlineData("Tag", "typeof(TestEnumeration)")] //Seams to work fine even without adding the type to the compilation
    [InlineData("Tag", "new int[] { 1, 2 }")]
    [InlineData("Tag", "new string[] { \"Foo\", \"Bar\" }")]
    [InlineData("Tag", "new object[] { \"Foo\", 123 }")]
    [InlineData("Tag", "System.DateTimeKind.Local")]
    public Task GeneratesObjectMappedProperties(string propertyName, string value)
        => RunSourceTest("GeneratesSimpleActionWhenDefined",
$@"namespace MyActions
{{
    [Xenial.XenialAction({propertyName} = {value})]
    public partial class GeneratesSimpleActionWhenDefined {{ }}
}}", verifySettings: settings => settings.UseParameters(propertyName, value), outputOptions: OnlyController);

    [Fact]
    public Task IdIsGenerated()
        => RunSourceTest("GeneratesSimpleActionWhenDefined",
@"namespace MyActions
{
    [Xenial.XenialAction]
    public partial class GeneratesSimpleActionWhenDefined { }
}", outputOptions: OnlyController);

    [Fact]
    public Task IdIsImplicitlySet()
        => RunSourceTest("GeneratesSimpleActionWhenDefined",
@"namespace MyActions
{
    [Xenial.XenialAction(Id = ""MyActionId""]
    public partial class GeneratesSimpleActionWhenDefined { }
}", outputOptions: OnlyController);

    [Fact]
    public Task DefaultCategoryIsGenerated()
        => RunSourceTest("GeneratesSimpleActionWhenDefined",
@"namespace MyActions
{
    [Xenial.XenialAction]
    public partial class GeneratesSimpleActionWhenDefined { }
}", outputOptions: OnlyController);

    [Fact]
    public Task StringCategoryIsUsed()
    => RunSourceTest("GeneratesSimpleActionWhenDefined",
@"namespace MyActions
{
    [Xenial.XenialAction(Category = ""MyCat"")]
    public partial class GeneratesSimpleActionWhenDefined { }
}", outputOptions: OnlyController);

    [Fact]
    public Task PredefinedCategoryCategoryIsUsed()
=> RunSourceTest("GeneratesSimpleActionWhenDefined",
@"namespace MyActions
{
    [Xenial.XenialAction(PredefinedCategory = Xenial.Persistent.Base.XenialPredefinedCategory.View)]
    public partial class GeneratesSimpleActionWhenDefined { }
}", outputOptions: OnlyController);

    [Fact]
    public Task ConflictingCategoryAttributesCategoryShouldOutputDiagnostics()
        => RunSourceTest("GeneratesSimpleActionWhenDefined",
@"namespace MyActions
{
    [Xenial.XenialAction(Category = ""MyCat"", PredefinedCategory = Xenial.Persistent.Base.XenialPredefinedCategory.View)]
    public partial class GeneratesSimpleActionWhenDefined { }
}");

    [Fact]
    public Task MultipleConflictingAttributesShouldReportDiagnostics()
        => RunSourceTest("GeneratesSimpleActionWhenDefined",
@"namespace MyActions
{
    [Xenial.XenialAction(Category = ""MyCat"")]
    public partial class GeneratesSimpleActionWhenDefined { }

    [Xenial.XenialAction(Category = ""MyCat"")]
    public partial class GeneratesSimpleActionWhenDefined { }
}");

    [Fact]
    public Task TargetViewIdIsGenerated()
        => RunSourceTest("GeneratesSimpleActionWhenDefined",
@"namespace MyActions
    {
        [Xenial.XenialAction(TargetViewId = ""GeneratesSimpleActionWhenDefined_DetailView"")]
        public partial class GeneratesSimpleActionWhenDefined { }
    }", outputOptions: OnlyController);

    [Fact]
    public Task TargetViewIdsAreGenerated()
        => RunSourceTest("GeneratesSimpleActionWhenDefined",
@"namespace MyActions
    {
        [Xenial.XenialAction(TargetViewIds = new[] { ""GeneratesSimpleActionWhenDefined_DetailView"", ""GeneratesSimpleActionWhenDefined_NestedDetailView"" })]
        public partial class GeneratesSimpleActionWhenDefined { }
    }", outputOptions: OnlyController);


    [Fact]
    public Task ConflictingTargetViewIdAttributesShouldOutputDiagnostics()
        => RunSourceTest("GeneratesSimpleActionWhenDefined",
@"namespace MyActions
{
    [Xenial.XenialAction(TargetViewId = ""MyCat"", TargetViewIds = new [] { ""Foo"" )]
    public partial class GeneratesSimpleActionWhenDefined { }
}", outputOptions: OnlyDiagnostics);

    [Fact]
    public Task CtorReferenceWithClass()
        => RunSourceTest("GeneratesSimpleActionWhenDefined",
@"namespace MyActions
{
    [Xenial.XenialAction]
    public partial class GeneratesSimpleActionWhenDefined
    {
        public GeneratesSimpleActionWhenDefined(DevExpress.ExpressApp.XafApplication application) {}
    }
}", outputOptions: OnlyController);

    [Fact]
    public Task PartialControllerOverloadWithoutKeywordAndVoidReturnTypeReportsDiagnostics()
        => RunSourceTest("GeneratesSimpleActionWhenDefined",
@"namespace MyActions
{
    [Xenial.XenialAction]
    public partial class GeneratesSimpleActionWhenDefined
    {
        public GeneratesSimpleActionWhenDefined(DevExpress.ExpressApp.XafApplication application) {}
    }

    partial class GeneratesSimpleActionWhenDefinedController
    {
        partial void CreateGeneratesSimpleActionWhenDefinedActionCore()
        {
        }
    }

}", outputOptions: OnlyController);

    //    [Fact]
    //    public Task InjectIntoRecordCtor()
    //    => RunSourceTest("GeneratesSimpleActionWhenDefined",
    //@"namespace MyActions
    //{
    //    [Xenial.XenialAction]
    //    public partial class GeneratesSimpleActionWhenDefined(DevExpress.ExpressApp.XafApplication Application) { }
    //}");
    //private static readonly string[] typeActionAttributeNames = new[]
    //{
    //    "TargetObjectType", //this should come out of context
    //    "TypeOfView", //we should not need this, but maybe should consider it after talking to dennis
    //};

    //private static readonly Dictionary<string, string> enumActionAttributeNames = new()
    //{
    //    ["SelectionDependencyType"] = "XenialSelectionDependencyType", //this should come out of context
    //    ["ActionMeaning"] = "XenialActionMeaning", //I am not even sure what's that for
    //    ["TargetViewType"] = "XenialViewType", //this should come out of context
    //    ["TargetViewNesting"] = "XenialNesting", //this is a tricky one and could come out of context
    //    ["PaintStyle"] = "XenialActionItemPaintStyle", //this should come directly from the attribute
    // This should go in some way of INPC, at least let the new criteria syntax do it's job
    //    ["TargetObjectsCriteriaMode"] = "XenialTargetObjectsCriteriaMode", //
    //    ["TargetObjectsCriteria"] = "XenialTargetObjectsCriteriaMode", //
    //};
}
