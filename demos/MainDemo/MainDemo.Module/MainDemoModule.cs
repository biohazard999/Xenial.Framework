using System;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

using MainDemo.Module.BusinessObjects;
using MainDemo.Module.CodeRules;
using MainDemo.Module.Reports;

using Xenial.Framework.ModelBuilders;

namespace MainDemo.Module
{
    public sealed partial class MainDemoModule : ModuleBase
    {
        public MainDemoModule() => InitializeComponent();
        public override void Setup(XafApplication application) => base.Setup(application);
        public override void Setup(ApplicationModulesManager moduleManager)
        {
            base.Setup(moduleManager);
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(RuleMemberPermissionsCriteria), typeof(IRuleBaseProperties));
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(RuleObjectPermissionsCriteria), typeof(IRuleBaseProperties));
        }
        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);

            typesInfo
                .CreateModelBuilder<DemoTaskModelBuilder>()
                .Build();
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            var predefinedReportsUpdater = new PredefinedReportsUpdater(Application, objectSpace, versionFromDB);
            predefinedReportsUpdater.AddPredefinedReport<EmployeeListReport>("Employee List Report", typeof(Employee), true);
            return new ModuleUpdater[] { updater, predefinedReportsUpdater };
        }
        static MainDemoModule()
        {
            /*Note that you can specify the required format in a configuration file:
            <appSettings>
               <add key="FullAddressFormat" value="{Country.Name} {City} {Street}">
               <add key="FullAddressPersistentAlias" value="Country.Name+City+Street">
               ...
            </appSettings>

            ... and set the specified format here in code:
            Address.SetFullAddressFormat(ConfigurationManager.AppSettings["FullAddressFormat"], ConfigurationManager.AppSettings["FullAddressPersistentAlias"]);
            */
            Address.SetFullAddressFormat("{Street}, {City}, {StateProvince} {ZipPostal}, {Country.Name}", "concat(Street, ' ', City, ' ', StateProvince, ' ', ZipPostal, ' ', Country.Name)");
            ResetViewSettingsController.DefaultAllowRecreateView = false;
        }
        private static Boolean? isSiteMode;
        public static Boolean IsSiteMode
        {
            get
            {
                if (isSiteMode == null)
                {
                    var siteMode = System.Configuration.ConfigurationManager.AppSettings["SiteMode"];
                    isSiteMode = ((siteMode != null) && (siteMode.ToLower() == "true"));
                }
                return isSiteMode.Value;
            }
        }
    }
}
