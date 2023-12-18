using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using Demos.Data;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Xpo;

using Xenial.Framework.DevTools.Win;

namespace MainDemo.Win
{
    public partial class MainDemoWinApplication : WinApplication
    {
        public MainDemoWinApplication()
        {
            InitializeComponent();
            SplashScreen = new DevExpress.ExpressApp.Win.Utils.DXSplashScreen(typeof(Demos.Win.XafDemoSplashScreen), new DefaultOverlayFormOptions());
            securityModule1.NonSecureActionsInitializing += SecurityModule1_NonSecureActionsInitializing;
            DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderControllerBase.ScriptRecorderEnabled = true;
        }
        private void SecurityModule1_NonSecureActionsInitializing(object sender, NonSecureActionsInitializingEventArgs e) => e.NonSecureActions.Add("Demo About Info");
        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            var dataStoreProvider = DemoXPODatabaseHelper.GetDataStoreProvider(args.ConnectionString, args.Connection);
            args.ObjectSpaceProviders.Add(new SecuredObjectSpaceProvider((ISelectDataSecurityProvider)Security, dataStoreProvider, false, true));
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
        }
        private void MainDemoWinApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e)
        {
            e.Updater.Update();
            e.Handled = true;
        }
        private void MainDemoWinApplication_LastLogonParametersRead(object sender, LastLogonParametersReadEventArgs e)
        {
            // Just to read demo user name for logon.
            var logonParameters = e.LogonObject as AuthenticationStandardLogonParameters;
            if (logonParameters != null)
            {
                if (String.IsNullOrEmpty(logonParameters.UserName))
                {
                    logonParameters.UserName = "Sam";
                }
            }
        }
        protected override List<Controller> CreateLogonWindowControllers()
        {
            var controllers = base.CreateLogonWindowControllers();
            //controllers.Add(new XenialDevToolsController());
            controllers.Add(new XenialDevToolsViewController());
            return controllers;
        }
    }
}

