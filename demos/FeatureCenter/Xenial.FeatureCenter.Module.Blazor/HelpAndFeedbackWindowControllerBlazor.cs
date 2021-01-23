using System;

using DevExpress.ExpressApp.Blazor;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Xenial.FeatureCenter.Module.Blazor
{
    public sealed class HelpAndFeedbackWindowControllerBlazor : HelpAndFeedbackWindowControllerBase
    {
        protected override void OpenHelpAndFeedbackLink(string uri)
        {
            if (Application is BlazorApplication blazorApplication)
            {
                var navigationManager = blazorApplication.ServiceProvider.GetRequiredService<NavigationManager>();
                navigationManager.NavigateTo(uri, forceLoad: true);
            }
        }
    }
}
