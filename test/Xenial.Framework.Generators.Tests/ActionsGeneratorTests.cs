﻿using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp;

using VerifyTests;

using VerifyXunit;

using Xunit;

namespace Xenial.Framework.Generators.Tests;

[UsesVerify]
public class ActionsGeneratorTests : BaseGeneratorTests<XenialActionGenerator>
{
    protected override string GeneratorEmitProperty => XenialActionGenerator.GenerateXenialActionAttributeMSBuildProperty;

    protected Task RunSourceTest(string fileName, string source, Action<VerifySettings>? verifySettings = null)
        => RunTest(
            options => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(GeneratorEmitProperty), "false")),
            compilationOptions: compilation => compilation.AddInlineXenialActionsAttribute(),
            syntaxTrees: () => new[]
            {
                BuildSyntaxTree(fileName, source)
            },
            verifySettings: verifySettings);

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
}");

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
}}", verifySettings: settings => settings.UseParameters(propertyName, value));

    [Theory]
    [InlineData("QuickAccess", "false")]
    [InlineData("QuickAccess", "true")]
    public Task GeneratesSimpleBoolMappingProperties(string propertyName, string value)
        => RunSourceTest("GeneratesSimpleActionWhenDefined",
$@"namespace MyActions
{{
    [Xenial.XenialAction({propertyName} = {value})]
    public partial class GeneratesSimpleActionWhenDefined {{ }}
}}", verifySettings: settings => settings.UseParameters(propertyName, value));

    public enum TestEnumeration
    {
        MyEnumValue
    }

    [Theory]
    [InlineData("Tag", "true")]
    [InlineData("Tag", "false")]
    [InlineData("Tag", "\"SomeString\"")]
    [InlineData("Tag", "42")]
    [InlineData("Tag", "typeof(TestEnumeration)")] //Seams to work fine even without adding the type to the compilation
    //Tricky ones, Maybe throw not supported exception?
    //[InlineData("Tag", "new int[] { 1, 2 }")]
    //[InlineData("Tag", "new string[] { \"Foo\", \"Bar\" }")]
    //[InlineData("Tag", "TestEnumeration.MyEnumValue")] //This may require adding the enumeration to the compilation
    public Task GeneratesObjectMappedProperties(string propertyName, string value)
        => RunSourceTest("GeneratesSimpleActionWhenDefined",
$@"namespace MyActions
{{
    [Xenial.XenialAction({propertyName} = {value})]
    public partial class GeneratesSimpleActionWhenDefined {{ }}
}}", verifySettings: settings => settings.UseParameters(propertyName, value));




    //private static readonly string[] stringActionAttributeNames = new[]
    //{
    //    "Id",
    //    "Category",
    //    ["PredefinedCategory"] = "XenialPredefinedCategory",

    //private static readonly string[] typeActionAttributeNames = new[]
    //{
    //    "TargetObjectType",
    //    "TypeOfView",
    //};

    //private static readonly string[] objectActionAttributeNames = new[]
    //{
    //    "Tag",
    //};

    //private static readonly Dictionary<string, string> enumActionAttributeNames = new()
    //{
    //    ["SelectionDependencyType"] = "XenialSelectionDependencyType",
    //    ["ActionMeaning"] = "XenialActionMeaning",
    //    ["TargetViewType"] = "XenialViewType",
    //    ["TargetViewNesting"] = "XenialNesting",
    //    ["TargetObjectsCriteriaMode"] = "XenialTargetObjectsCriteriaMode",
    //    ["PaintStyle"] = "XenialActionItemPaintStyle",
    //};

}

internal static partial class CompilationHelpers
{
    public static CSharpCompilation AddInlineXenialActionsAttribute(this CSharpCompilation compilation, string visibility = "internal")
    {
        (_, var syntaxTree) = XenialActionGenerator.GenerateXenialActionsAttribute(visibility: visibility);

        return compilation.AddSyntaxTrees(syntaxTree);
    }
}
