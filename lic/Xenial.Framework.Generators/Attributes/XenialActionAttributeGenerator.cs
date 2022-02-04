using System;
using System.Collections.Generic;

using Xenial.Framework.Generators.Base;
using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators.Attributes;

public record XenialActionAttributeGenerator(bool AddSources = true) : XenialAttributeGenerator(AddSources)
{
    public override string AttributeName => "XenialActionAttribute";

    public static readonly Dictionary<string, string> ActionAttributeNames = new()
    {
        ["Caption"] = "string",
        ["ImageName"] = "string",
        ["Category"] = "string",
        ["DiagnosticInfo"] = "string",
        ["TargetViewId"] = "string",
        ["TargetViewIds"] = "string[]",
        ["TargetObjectsCriteria"] = "string",
        ["ConfirmationMessage"] = "string",
        ["ToolTip"] = "string",
        ["Shortcut"] = "string",

        ["Id"] = "string",

        ["TargetObjectType"] = "Type",
        ["TypeOfView"] = "Type",

        ["QuickAccess"] = "bool",

        ["Tag"] = "object",

        ["PredefinedCategory"] = "DevExpress.Persistent.Base.PredefinedCategory",
        ["SelectionDependencyType"] = "DevExpress.ExpressApp.Actions.SelectionDependencyType",
        ["ActionMeaning"] = "DevExpress.ExpressApp.Actions.ActionMeaning",
        ["TargetViewType"] = "DevExpress.ExpressApp.ViewType",
        ["TargetViewNesting"] = "DevExpress.ExpressApp.Nesting",
        ["TargetObjectsCriteriaMode"] = "DevExpress.ExpressApp.Actions.TargetObjectsCriteriaMode",
        ["PaintStyle"] = "DevExpress.ExpressApp.Templates.ActionItemPaintStyle",
    };

    protected override CurlyIndenter CreateAttribute(CurlyIndenter syntaxWriter, string visibility)
    {
        _ = syntaxWriter ?? throw new ArgumentNullException(nameof(syntaxWriter));

        syntaxWriter.WriteLine($"using System;");
        syntaxWriter.WriteLine();
        syntaxWriter.WriteLine("using Xenial.ExpressApp;");
        syntaxWriter.WriteLine("using Xenial.ExpressApp.Actions;");
        syntaxWriter.WriteLine("using Xenial.ExpressApp.Templates;");
        syntaxWriter.WriteLine("using Xenial.Persistent.Base;");
        syntaxWriter.WriteLine();

        using (syntaxWriter.OpenBrace($"namespace {AttributeNamespace}"))
        {
            syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Class, Inherited = false)]");
            using (syntaxWriter.OpenBrace($"{visibility} sealed class {AttributeName} : Attribute"))
            {
                syntaxWriter.WriteLine($"{visibility} {AttributeName}() {{ }}");

                foreach (var actionAttributePair in ActionAttributeNames)
                {
                    syntaxWriter.WriteLine($"public {actionAttributePair.Value} {actionAttributePair.Key} {{ get; set; }}");
                }
            }
        }

        return syntaxWriter;
    }
}
