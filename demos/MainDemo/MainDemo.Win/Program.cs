using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

using Demos.Data;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.Bars.ActionControls;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Internal;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;

namespace MainDemo.Win
{
    public static class Program
    {
#if NETCOREAPP
        [SupportedOSPlatform("windows7.0")]
#endif
        [STAThread]
        public static void Main(string[] arguments)
        {
            DevExpress.ExpressApp.FrameworkSettings.DefaultSettingsCompatibilityMode = DevExpress.ExpressApp.FrameworkSettingsCompatibilityMode.Latest;
            WindowsFormsSettings.LoadApplicationSettings();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DevExpress.Utils.ToolTipController.DefaultController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;

            if (Tracing.GetFileLocationFromSettings() == FileLocation.CurrentUserApplicationDataFolder)
            {
                Tracing.LocalUserAppDataPath = Application.LocalUserAppDataPath;
            }
            Tracing.Initialize();
            using var winApplication = new MainDemoWinApplication()
            {
                IgnoreUserModelDiffs = true
            };

            DevExpress.ExpressApp.Utils.ImageLoader.Instance.UseSvgImages = true;
#if DEBUG
            DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif
            winApplication.CustomizeFormattingCulture += new EventHandler<CustomizeFormattingCultureEventArgs>(winApplication_CustomizeFormattingCulture);
            winApplication.LastLogonParametersReading += new EventHandler<LastLogonParametersReadingEventArgs>(winApplication_LastLogonParametersReading);
            winApplication.CustomizeTemplate += new EventHandler<CustomizeTemplateEventArgs>(WinApplication_CustomizeTemplate);
            winApplication.GetSecurityStrategy().RegisterXPOAdapterProviders();
            var connectionStringSettings = ConfigurationManager.ConnectionStrings["ConnectionString"];
            if (connectionStringSettings != null)
            {
                winApplication.ConnectionString = connectionStringSettings.ConnectionString;
            }
            if (string.IsNullOrEmpty(winApplication.ConnectionString) && winApplication.Connection == null)
            {
                connectionStringSettings = ConfigurationManager.ConnectionStrings["SqlExpressConnectionString"];
                if (connectionStringSettings != null)
                {
                    var connectionString = connectionStringSettings.ConnectionString;
                    if (connectionString != InMemoryDataStoreProvider.ConnectionString)
                    {
                        connectionString = DemoDbEngineDetectorHelper.PatchSQLConnectionString(connectionString);
                        if (connectionString == DemoDbEngineDetectorHelper.AlternativeConnectionString)
                        {
                            connectionString = InMemoryDataStoreProvider.ConnectionString;
                            UseSQLAlternativeInfoSingleton.Instance.FillFields(DemoDbEngineDetectorHelper.SQLServerIsNotFoundMessage, DemoXPODatabaseHelper.AlternativeName, DemoXPODatabaseHelper.InMemoryDatabaseUsageMessage);
                        }
                    }
                    winApplication.ConnectionString = connectionString;
                }
            }
#if DEBUG
            foreach (var argument in arguments)
            {
                if (argument.StartsWith("-connectionString:"))
                {
                    var connectionString = argument.Replace("-connectionString:", "");
                    winApplication.ConnectionString = connectionString;
                }
            }
#endif
            if (System.Diagnostics.Debugger.IsAttached && winApplication.CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema)
            {
                winApplication.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            }
            try
            {
                winApplication.Setup();
                winApplication.Start();
            }
            catch (Exception e)
            {
                winApplication.StopSplash();
                winApplication.HandleException(e);
            }
        }

        private static void WinApplication_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e)
        {
            if (e.Context == TemplateContext.ApplicationWindow || e.Context == TemplateContext.View)
            {
                var ribbonForm = e.Template as RibbonForm;
                var actionControlsSite = ribbonForm as IActionControlsSite;
                if ((ribbonForm != null) && (actionControlsSite != null))
                {
                    var filtersActionControlContainer = actionControlsSite.ActionContainers.FirstOrDefault<IActionControlContainer>(x => x.ActionCategory == "Filters");
                    if (filtersActionControlContainer is BarLinkActionControlContainer)
                    {
                        var barFiltersActionControlContainer = (BarLinkActionControlContainer)filtersActionControlContainer;
                        var barFiltersItem = barFiltersActionControlContainer.BarContainerItem;
                        var ribbonControl = ribbonForm.Ribbon;
                        foreach (RibbonPage page in ribbonControl.Pages)
                        {
                            foreach (RibbonPageGroup group in page.Groups)
                            {
                                var barFiltersItemLink = group.ItemLinks.FirstOrDefault<BarItemLink>(x => x.Item == barFiltersItem);
                                if (barFiltersItemLink != null)
                                {
                                    group.ItemLinks.Remove(barFiltersItemLink);
                                }
                            }
                        }
                        ribbonForm.Ribbon.PageHeaderItemLinks.Add(barFiltersItem);
                    }

                }
            }
            else if ((e.Context == TemplateContext.LookupControl) || (e.Context == TemplateContext.LookupWindow))
            {
                var lookupControlTemplate = e.Template as LookupControlTemplate;
                if (lookupControlTemplate == null && e.Template is LookupForm)
                {
                    lookupControlTemplate = ((LookupForm)e.Template).FrameTemplate;
                }
                if (lookupControlTemplate != null)
                {
                    lookupControlTemplate.ObjectsCreationContainer.ContainerId = "LookupNew";
                    lookupControlTemplate.SearchActionContainer.ContainerId = "LookupFullTextSearch";
                }
            }
        }

#if NETCOREAPP
        [SupportedOSPlatform("windows7.0")]
        private
#else
        private
#endif
        static void winApplication_CustomizeFormattingCulture(object sender, CustomizeFormattingCultureEventArgs e) => e.FormattingCulture = CultureInfo.GetCultureInfo("en-US");

        private static void winApplication_LastLogonParametersReading(object sender, LastLogonParametersReadingEventArgs e)
        {
            if (string.IsNullOrEmpty(e.SettingsStorage.LoadOption("", "UserName")))
            {
                e.SettingsStorage.SaveOption("", "UserName", "Sam");
            }
        }
    }
}
