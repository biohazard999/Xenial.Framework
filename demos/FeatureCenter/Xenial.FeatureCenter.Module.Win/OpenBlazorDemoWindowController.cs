using System;
using System.Diagnostics;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace Xenial.FeatureCenter.Module.Win
{
    public sealed class OpenBlazorDemoWindowController : WindowController
    {
        public SimpleAction OpenBlazorDemoSimpleAction { get; }

        public OpenBlazorDemoWindowController()
        {
            OpenBlazorDemoSimpleAction = new SimpleAction(this, nameof(OpenBlazorDemoSimpleAction), PredefinedCategory.RecordsNavigation)
            {
                Caption = "Open Blazor Demo",
                ImageName = "Business_World",
                PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.CaptionAndImage
            };

            OpenBlazorDemoSimpleAction.Execute += OpenBlazorDemoSimpleAction_Execute;
        }

        private void OpenBlazorDemoSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
            => Process.Start("https://framework.featurecenter.xenial.io/");
    }
}
