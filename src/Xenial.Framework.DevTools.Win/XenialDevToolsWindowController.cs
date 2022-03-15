using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Templates.Bars;
using DevExpress.XtraBars;

using Xenial.Framework.DevTools.Helpers;
using Xenial.Framework.Images;
using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.ColumnItems;
using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;
using Xenial.Framework.MsBuild;

namespace Xenial.Framework.DevTools.Win;

/// <summary>
/// 
/// </summary>
public sealed class XenialDevToolsWindowController : WindowController
{
    private WinWindow? DevToolsWindow { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public XenialDevToolsWindowController()
        => TargetWindowType = WindowType.Main;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="view"></param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "XAF is handling the template")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "It's just the caption for the dev tool")]
    public void OpenDevTools(View view)
    {

        if (DevToolsWindow is null)
        {
            DevToolsWindow = new WinWindow(Application, TemplateContext.View, new Controller[]
            {
                //TODO: make it possible to add custom controllers
                Application.CreateController<FillActionContainersController>(),
                Application.CreateController<ActionControlsSiteController>(),
                Application.CreateController<XenialDevToolsController>()
            }, false, false);

            DevToolsWindow.Disposed += DevToolsWindow_Disposed;

            void DevToolsWindow_Disposed(object sender, EventArgs e)
            {
                DevToolsWindow.Disposed -= DevToolsWindow_Disposed;
                DevToolsWindow = null;
            }

            var template = new DetailFormV2();

            var barManagerHolder = (IBarManagerHolder)template;

            barManagerHolder.BarManager.MainMenu.Visible = false;
            barManagerHolder.BarManager.StatusBar.Visible = false;
            barManagerHolder.BarManager.AllowCustomization = false;
            barManagerHolder.BarManager.AllowQuickCustomization = false;
            barManagerHolder.BarManager.AllowCustomization = false;

            foreach (Bar bar in barManagerHolder.BarManager.Bars)
            {
                bar.OptionsBar.DrawDragBorder = false;
            }

            DevToolsWindow.SetTemplate(template);

            template.ShowMdiChildCaptionInParentTitle = false;
            template.Text = "Xenial-DevTools";
        }

        var objectSpace = Application.CreateObjectSpace(typeof(DevToolsViewModel));
        var devToolsViewModel = objectSpace.CreateObject<DevToolsViewModel>();
        var detailView = Application.CreateDetailView(objectSpace, devToolsViewModel);

        if (view is not null)
        {
            devToolsViewModel.ViewId = view.Id;

            view.SaveModel();

            var xml = X2C.X2CEngine.LoadXml(view.Model);

            var controller = DevToolsWindow.GetController<XenialDevToolsController>();

            static string BuildXafmlHtml(string xml, bool prettyPrint)
            {
                var node = VisualizeNodeHelper.PrettyPrint(xml, prettyPrint);

                var code = new HtmlBuilder.CodeBlock("xml", node);

                return HtmlBuilder.BuildHtml("Xafml", $"{code}");
            }

            devToolsViewModel.Xafml = BuildXafmlHtml(xml.OuterXml, controller.PrettyPrintXml);

            void PrettyPrint(object? s, EventArgs e)
                => devToolsViewModel.Xafml = BuildXafmlHtml(xml.OuterXml, controller.PrettyPrintXml);

            controller.PrettyPrintXmlSimpleAction.Executed -= PrettyPrint;
            controller.PrettyPrintXmlSimpleAction.Executed += PrettyPrint;

            devToolsViewModel.Code = HtmlBuilder.BuildHtml("Code", $"{new HtmlBuilder.CodeBlock("csharp", X2C.X2CEngine.ConvertToCode(xml))}");
        }

        DevToolsWindow.SetView(detailView, true, null, true);

        DevToolsWindow.Show();

        //protected internal void SetFrame(Frame frame)
        var method = typeof(Controller).GetMethod("SetFrame", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        if (method is not null)
        {
            foreach (var controller in DevToolsWindow.Controllers)
            {
                method.Invoke(controller, new object[] { DevToolsWindow });
                if (controller is WindowController windowController)
                {
                    if (windowController.Window is null)
                    {
                        windowController.SetWindow(DevToolsWindow);
                    }
                }
            }
        }
    }
}
