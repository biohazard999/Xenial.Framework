using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor;
using DevExpress.Persistent.Base;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Xenial.FeatureCenter.Module.Blazor
{
    public sealed class DownloadWindowsFormsDemoWindowController : WindowController
    {
        public SimpleAction DownloadWindowsFormsDemoSimpleAction { get; }

        public DownloadWindowsFormsDemoWindowController()
        {
            DownloadWindowsFormsDemoSimpleAction = new SimpleAction(this, nameof(DownloadWindowsFormsDemoSimpleAction), PredefinedCategory.RecordsNavigation)
            {
                Caption = "Download Winforms Demo",
                ImageName = "MoveDown",
                PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.CaptionAndImage
            };

            DownloadWindowsFormsDemoSimpleAction.Execute += DownloadWindowsFormsDemoSimpleAction_Execute;
        }

        private void DownloadWindowsFormsDemoSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (Application is BlazorApplication blazorApplication)
            {
                var navigationManager = blazorApplication.ServiceProvider.GetRequiredService<NavigationManager>();
                navigationManager.NavigateTo($"https://github.com/xenial-io/Xenial.Framework/releases/download/v{XenialVersion.Version}/Xenial.FeatureCenter.Win.v{XenialVersion.Version}.AnyCPU.zip", forceLoad: true);
            }
        }
    }
}
