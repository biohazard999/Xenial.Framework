using System;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;

using MainDemo.Module.BusinessObjects;
using MainDemo.Module.CodeRules;
using MainDemo.Module.Reports;

using Xenial.Framework.Base;
using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.LeafNodes;
using Xenial.Framework.Model.GeneratorUpdaters;
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

            ModelBuilder.Create<Person>(typesInfo)
                //You can define the cloneable via ModelDefault
                //https://supportcenter.devexpress.com/ticket/details/t852289/suggestion-add-an-attribute-to-the-clone-module
                .WithModelDefault("IsCloneable", true)
                .Build();

            ModelBuilder.Create<PermissionPolicyUser>(typesInfo)
                .HasCaption("Base User")
                .Build();

            ModelBuilder.Create<Analysis>(typesInfo)
                .HasCaption("Analytics")
                .Build();

            ModelBuilder.Create<Task>(typesInfo)
                .HasCaption("Base Task")
                .WithModelDefault("IsCloneable", true)
                .Build();

            ModelBuilder.Create<Note>(typesInfo)
                .HasObjectCaptionFormat("")
                .Build();

            var phoneNumberBuilder = ModelBuilder.Create<PhoneNumber>(typesInfo);

            phoneNumberBuilder.For(m => m.Number)
                .HasEditMask("(000) 000-0000", null)
                .HasDisplayFormat("{0:(###) ###-####}");

            phoneNumberBuilder.Build();

            var partyBuilder = ModelBuilder.Create<Party>(typesInfo);

            partyBuilder.For(m => m.Address1)
                .HasCaption("Address");

            partyBuilder.Build();

            var personBuilder = ModelBuilder.Create<Person>(typesInfo);

            personBuilder.For(m => m.Birthday)
                .HasCaption("Birth Date");

            personBuilder.Build();

            var employeeBuilder = ModelBuilder.Create<Employee>(typesInfo);

            employeeBuilder
                .HasObjectCaptionFormat(m => m.FullName)
                .HasImage("BO_Employee")
            //TODO: Check why Navigation is weird when using default list view
            //.HasDefaultListViewId(ViewIds.Employee_ListView_Varied)
            ;

            employeeBuilder.For(m => m.Anniversary)
                .HasCaption("Wedding Date");

            employeeBuilder.For(m => m.SpouseName)
                .HasCaption("Spouse");

            employeeBuilder.For(m => m.TitleOfCourtesy)
                .HasCaption("Title");

            employeeBuilder.For(m => m.Position)
                .WithAttribute(
                    new DataSourcePropertyAttribute(
                        employeeBuilder.ExpressionHelper.Property(m => m.Position.Departments),
                        DataSourcePropertyIsNullMode.SelectAll
                    )
                );

            employeeBuilder.Build();

            var departmentBuilder = ModelBuilder.Create<Department>(typesInfo);

            departmentBuilder.HasImage("BO_Department");

            ((TypeInfo)departmentBuilder.TypeInfo)
                .CreateMember("NumberOfEmployees", typeof(int), "[Employees][].Count()");

            departmentBuilder.For("NumberOfEmployees")
                .HasCaption("Number Of Employees");

            departmentBuilder.For(m => m.Title)
                .HasCaption("Name");

            departmentBuilder.Build();

            typesInfo
                .CreateModelBuilder<DemoTaskModelBuilder>()
                .Build();

            var paycheckBuilder = ModelBuilder.Create<Paycheck>(typesInfo);

            paycheckBuilder
                .WithModelDefault("IsCloneable", true)
                .HasImage("BO_SaleItem");

            paycheckBuilder.ForProperties(
                m => m.GrossPay,
                m => m.NetPay,
                m => m.PayRate,
                m => m.TotalTax,
                m => m.OvertimePayRate
            ).HasDisplayFormat("${0:0.00}")
             .HasEditMask("\\$#,###,##0.00");

            paycheckBuilder
                .For(m => m.TaxRate)
                .HasDisplayFormat("{0:P}");

            paycheckBuilder.Build();

            ModelBuilder.Create<Position>(typesInfo)
                .HasImage("BO_Position")
                .Build();

            ModelBuilder.Create<Event>(typesInfo)
                .WithListViewColumns(() => Columns.Automatic(new()
                {
                    Caption = "Calendar"
                }));

            ModelBuilder.Create<Note>(typesInfo)
                .WithListViewColumns(() => Columns.Automatic(new()
                {
                    Caption = "Notes"
                }));

            ModelBuilder.Create<PermissionPolicyRole>(typesInfo)
                .WithListViewColumns(() => Columns.Automatic(new()
                {
                    Caption = "Role"
                }));

            ModelBuilder.Create<Position>(typesInfo)
                .WithListViewColumns(() => Columns.Automatic(new()
                {
                    Caption = "Positions"
                }));

            ModelBuilder.Create<Resume>(typesInfo)
                .WithListViewColumns(() => Columns.Automatic(new()
                {
                    Caption = "Resumes"
                }));

            ModelBuilder.Create<ApplicationUser>(typesInfo)
                .WithListViewColumns(() => Columns.Automatic(new()
                {
                    Caption = "Users"
                }))
                .WithDetailViewLayout(b => new()
                {
                    b.HorizontalGroup(
                        b.VerticalGroup(
                            b.PropertyEditor(m => m.UserName),
                            b.PropertyEditor(m => m.IsActive) with { ShowCaption = true },
                            b.PropertyEditor(m => m.ChangePasswordOnFirstLogon) with { ShowCaption = true }
                        ),
                        b.PropertyEditor(m => m.Photo) with { ShowCaption = false, RelativeSize = 15 }
                    ),
                    b.VerticalGroup("Roles", b.PropertyEditor(m => m.Roles))
                })
                .Build();

            //TODO: Why is this not working as expected?
            //Maybe we need to move that to platform specific module
            //because security or other module kicks in
            //ModelBuilder.Create<AuthenticationStandardLogonParameters>(typesInfo)
            //    .WithDetailViewLayout(b => new(new()
            //    {
            //        Caption = "Log In"
            //    })
            //    {
            //        b.HorizontalGroup(
            //            new LayoutStaticImageItem("Security_Image") with
            //            {
            //                //TODO: SizeMode
            //                //SizeConstraintsType
            //                VerticalAlign = DevExpress.ExpressApp.Editors.StaticVerticalAlign.Middle,
            //                ShowCaption = false
            //            },
            //            b.VerticalGroup(
            //                new LayoutStaticTextItem("Welcome to the eXpressApp Framework Main Demo"),
            //                new LayoutStaticTextItem("Enter your user name and password to proceed."),
            //                b.PropertyEditor(m => m.UserName),
            //                b.PropertyEditor(m => m.Password),
            //                new LayoutStaticTextItem("This demo app does not require a password for login")
            //            )
            //        )
            //    })
            //    .Build();
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            var predefinedReportsUpdater = new PredefinedReportsUpdater(Application, objectSpace, versionFromDB);
            predefinedReportsUpdater.AddPredefinedReport<EmployeeListReport>("Employee List Report", typeof(Employee), true);
            return new ModuleUpdater[] { updater, predefinedReportsUpdater };
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);

            updaters
                .UseAppOptions(new AppOptions()
                {
                    Title = "Main Demo",
                    Description = "eXpressApp Framework demo application",
                    Company = "Developer Express Inc.",
                    PreferredLanguage = "(Default language)"
                    //TODO: where is this model node defined?
                    //WebSite = "http://www.devexpress.com",
                })
                .UseApplicationOptions(new ApplicationOptions
                {
                    LookupSmallCollectionItemCount = 75
                })
                .UseXenialImages()
                .UseDeclareViewsGeneratorUpdater()
                .UseDetailViewLayoutBuilders()
                .UseListViewColumnBuilders();
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
            ResetViewSettingsController.DefaultAllowRecreateView = true;
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
