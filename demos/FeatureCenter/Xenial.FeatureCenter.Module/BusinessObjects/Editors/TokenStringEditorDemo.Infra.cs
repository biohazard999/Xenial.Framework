using System;
using System.Collections.Generic;
using System.Text;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    public partial class TokenStringEditorDemo
    {
        protected override IEnumerable<RequiredNuget> GetRequiredModules() => new[]
        {
            new RequiredNuget("TokenEditors"),
            new RequiredNuget("TokenEditors", AvailablePlatform.Win),
            new RequiredNuget("TokenEditors", AvailablePlatform.Blazor),
        };
    }
}
