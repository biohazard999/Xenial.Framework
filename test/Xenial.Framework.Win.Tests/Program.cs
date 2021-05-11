using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Office.Utils;

using FakeItEasy;

using Shouldly;

using Xenial.Framework.Win.SystemModule;

using static Xenial.Tasty;

namespace Xenial.Framework.Win.Tests
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Describe(nameof(XenialAdvancedModelEditorActionsViewController), () =>
            {
                var application = A.Fake<XafApplication>();
                var window = new Window(application, TemplateContext.ApplicationWindow, new List<Controller>(), true, true);

                A.CallTo(() => application.MainWindow).Returns(window);

                var view = A.Fake<DetailView>();

                var controller = new XenialAdvancedModelEditorActionsViewController()
                {
                    Application = application
                };

                var editModelController = new EditModelController
                {
                    Application = application
                };

                window.RegisterController(editModelController);

                editModelController.SetWindow(window);
                var frame = new Frame(application, TemplateContext.View);

                frame.RegisterController(controller);
                frame.SetView(view);

                It("controller should be active", () =>
                {
                    controller.Active.ResultValue.ShouldBe(true);
                });

                It($"{nameof(XenialAdvancedModelEditorActionsViewController.OpenViewInModelEditorSimpleAction)} should be active by default", () =>
                {
                    controller.OpenViewInModelEditorSimpleAction.Active.ResultValue.ShouldBe(true);
                });

                It($"{nameof(XenialAdvancedModelEditorActionsViewController.OpenViewInModelEditorSimpleAction)} should be deactivated", () =>
                {
                    const string key = "Test";
                    editModelController.Active[key] = false;
                    controller.OpenViewInModelEditorSimpleAction.Active.ResultValue.ShouldBe(false);
                    editModelController.Active.RemoveItem(key);
                });
            });

            return await Run(args);
        }
    }
}
