using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.Persistent.Base;

using Markdig;

using Xenial.Framework.Base;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    [DefaultClassOptions]
    [Singleton(AutoCommit = true)]
    public partial class StepProgressBarEnumEditorPersistentDemo
    {
        protected override IEnumerable<RequiredNuget> GetRequiredModules() => new[]
        {
            new RequiredNuget("StepProgressEditors"),
            new RequiredNuget("StepProgressEditors", AvailablePlatform.Win),
        };

        protected override void AddInstallationSection(StringBuilder sb)
        {
            base.AddInstallationSection(sb);

            sb.AppendLine(new TabGroup
            {
                Tabs = new()
                {
                    new("By Module", "fas fa-code")
                    {
                        MarkDown = @"

#### Common Module

```cs
public class MyProjectModule : ModuleBase
{
    protected override ModuleTypeList GetRequiredModuleTypesCore()
    {
        var moduleTypes = base.GetRequiredModuleTypesCore();
        
        moduleTypes.Add(typeof(XenialStepProgressEditorsModule));
        
        return moduleTypes;
    }
}
```

#### Windows Forms Module

```cs
public class MyProjectWindowsFormsModule : ModuleBase
{
    protected override ModuleTypeList GetRequiredModuleTypesCore()
    {
        var moduleTypes = base.GetRequiredModuleTypesCore();
        
        moduleTypes.Add(typeof(XenialStepProgressEditorsWindowsFormsModule));
        
        return moduleTypes;
    }
}
```

"
                    },
                    new("Feature Slice", "fas fa-pizza-slice")
                    {
                        MarkDown = ""
                    }
                }
            }.ToString());
        }
    }
}
