using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp.Actions;
using DevExpress.XtraBars;

namespace Xenial.FeatureCenter.Module.Win
{
    public sealed class BadgesWindowsFormsFeatureController : BadgesFeatureController
    {
        public BadgesWindowsFormsFeatureController()
            => ToggleBadgesSimpleAction.CustomizeControl += ToggleBadgesSimpleAction_CustomizeControl;

        private void ToggleBadgesSimpleAction_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            if (e.Control is BarButtonItem barButtonItem)
            {
                barButtonItem.ButtonStyle = BarButtonStyle.Check;
            }
        }
    }
}
