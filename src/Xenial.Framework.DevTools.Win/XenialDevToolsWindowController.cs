using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Acme.Module.Helpers;

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
public class XenialDevToolsController : DialogController
{
    /// <summary>
    /// 
    /// </summary>
    public SimpleAction AlwaysOnTopSimpleAction { get; }

    /// <summary>
    /// 
    /// </summary>
    public SimpleAction OpacitySimpleAction { get; }

    /// <summary>
    /// 
    /// </summary>
    public SimpleAction PrettyPrintXmlSimpleAction { get; }

    /// <summary>
    /// 
    /// </summary>
    public XenialDevToolsController()
    {
        AlwaysOnTopSimpleAction = new SimpleAction(this, nameof(AlwaysOnTopSimpleAction), DevExpress.Persistent.Base.PredefinedCategory.ObjectsCreation)
        {
            ImageName = XenialImages.Action_Xenial_AlwaysOnTop,
            PaintStyle = ActionItemPaintStyle.Image,
            ToolTip = "Controls if the Xenial-DevTools window is displayed over the current application"
        };

        OpacitySimpleAction = new SimpleAction(this, nameof(OpacitySimpleAction), DevExpress.Persistent.Base.PredefinedCategory.ObjectsCreation)
        {
            ImageName = XenialImages.Action_Xenial_Opacity,
            PaintStyle = ActionItemPaintStyle.Image,
            ToolTip = "Controls if the Xenial-DevTools window is displayed opaque"
        };

        PrettyPrintXmlSimpleAction = new SimpleAction(this, nameof(PrettyPrintXmlSimpleAction), DevExpress.Persistent.Base.PredefinedCategory.ObjectsCreation)
        {
            ImageName = XenialImages.Action_Xenial_Xml,
            PaintStyle = ActionItemPaintStyle.Image,
            ToolTip = "Pretty print Xafml XML output"
        };

        AlwaysOnTopSimpleAction.Execute += AlwaysOnTopSimpleAction_Execute;
        OpacitySimpleAction.Execute += OpacitySimpleAction_Execute;
        PrettyPrintXmlSimpleAction.Execute += PrettyPrintXmlSimpleAction_Execute;

        AlwaysOnTopSimpleAction.CustomizeControl += AlwaysOnTopSimpleAction_CustomizeControl;
        OpacitySimpleAction.CustomizeControl += AlwaysOnTopSimpleAction_CustomizeControl;
        PrettyPrintXmlSimpleAction.CustomizeControl += AlwaysOnTopSimpleAction_CustomizeControl;
    }
    /// <summary>
    /// 
    /// </summary>
    public bool PrettyPrintXml { get; set; } = true;
    private void PrettyPrintXmlSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        => PrettyPrintXml = !PrettyPrintXml;

    private void AlwaysOnTopSimpleAction_CustomizeControl(object sender, CustomizeControlEventArgs e)
    {
        if (e.Control is BarButtonItem button)
        {
            button.ButtonStyle = BarButtonStyle.Check;
            button.Down = true;
        }
    }

    private System.Windows.Forms.Form? ownerForm;

    private void AlwaysOnTopSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
    {
        if (Frame is WinWindow winWindow && winWindow.Form is not null)
        {
            if (winWindow.Form.Owner is null)
            {
                winWindow.Form.Owner = ownerForm;
            }

            winWindow.Form.TopMost = !winWindow.Form.TopMost;
            ownerForm = winWindow.Form.Owner;

            if (!winWindow.Form.TopMost)
            {
                winWindow.Form.Owner = null;
            }
        }
    }

    private bool shouldBeTransparent;
    private bool opacitySimpleActionDown = true;
    private bool Transparent => shouldBeTransparent && opacitySimpleActionDown;

    private void OpacitySimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
    {
        opacitySimpleActionDown = !opacitySimpleActionDown;
        shouldBeTransparent = !shouldBeTransparent;
        MouseDown(null, EventArgs.Empty);
    }

    private void MouseDown(object? sender, EventArgs e)
    {
        if (template is not null)
        {
            if (!Transparent)
            {
                template.Opacity = 1;
            }
            else
            {
                if (shouldBeTransparent)
                {
                    template.Opacity = 1;
                }
                else
                {
                    template.Opacity = 0.75;
                }
            }
        }
    }

    private void MouseUp(object? sender, EventArgs e)
    {
        if (template is not null)
        {
            if (!Transparent)
            {
                template.Opacity = 1;
            }
            else
            {
                if (shouldBeTransparent)
                {
                    template.Opacity = 0.75;
                }
                else
                {
                    template.Opacity = 1;
                }
            }
        }
    }

    private System.Windows.Forms.Form? template;

    private void Attach()
    {
        Detach();
        if (template is not null)
        {
            template.Activated += MouseDown;
            template.GotFocus += MouseDown;
            template.MouseDown += MouseDown;

            template.Deactivate += MouseUp;
            template.MouseUp += MouseUp;
            template.LostFocus += MouseUp;

            template.FormClosed += TemplateDisposed;
            template.Disposed += TemplateDisposed;

            if (Application.MainWindow is WinWindow winWindow)
            {
                template.Owner = winWindow.Form;
                template.TopMost = true;
            }
        }
    }

    private void Detach()
    {
        if (template is not null)
        {
            template.Activated -= MouseDown;
            template.GotFocus -= MouseDown;
            template.MouseDown -= MouseDown;

            template.Deactivate -= MouseUp;
            template.MouseUp -= MouseUp;
            template.LostFocus -= MouseUp;
        }
    }

    private void TemplateDisposed(object? sender, EventArgs e)
    {
        if (template is not null)
        {
            template.FormClosed -= TemplateDisposed;
            template.Disposed -= TemplateDisposed;
        }
        Detach();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            TemplateDisposed(null, EventArgs.Empty);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void OnActivated()
    {
        base.OnActivated();

        if (Frame?.Template is System.Windows.Forms.Form template)
        {
            this.template = template;
            shouldBeTransparent = true;
            MouseUp(null, EventArgs.Empty);
        }

        Attach();
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void OnDeactivated()
    {
        base.OnDeactivated();
        Detach();
    }
}

/// <summary>
/// 
/// </summary>
public class XenialDevToolsWindowController : WindowController
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
            view.SaveModel();
            devToolsViewModel.ViewId = view.Id;

            var id = $"{view.Model.Id}_{Guid.NewGuid()}";
            var modelViews = view.Model.Application.Views;
            var copy = ((ModelNode)modelViews).AddClonedNode((ModelNode)view.Model, id);

            if (copy is not null)
            {
                foreach (var property in copy.GetType().GetProperties())
                {
                    var info = copy.GetValueInfo(property.Name);
                    if (info is not null)
                    {
                        var attributes = property
                            .GetCustomAttributes(typeof(System.ComponentModel.DefaultValueAttribute), false)
                            .OfType<System.ComponentModel.DefaultValueAttribute>();

                        foreach (var attribute in attributes)
                        {
                            var value = copy.GetValue(property.Name);
                            if (attribute.Value.Equals(value))
                            {
                                copy.ClearValue(property.Name);
                            }
                            else
                            {
                                value = ((ModelNode)view.Model).GetValue(property.Name);
                                if (attribute.Value.Equals(value))
                                {
                                    copy.ClearValue(property.Name);
                                }
                                else
                                {
                                    var viewValue = property.GetValue(view.Model);
                                    var copyValue = property.GetValue(copy);
                                    if (viewValue is not null)
                                    {
                                        if (viewValue.Equals(copyValue))
                                        {
                                            copy.ClearValue(property.Name);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var node = VisualizeNodeHelper.PrintModelNode(copy);

            //TODO: use xml to replace id
            node = node.Replace(id, view.Model.Id); //Patch ViewId

            var doc = new System.Xml.XmlDocument();
            doc.LoadXml(node);
            var root = doc.FirstChild;

            CleanNodes(root);

            static void CleanNodes(System.Xml.XmlNode node)
            {
                if (node.Attributes["IsNewNode"] != null)
                {
                    node.Attributes.Remove(node.Attributes["IsNewNode"]);
                }

                if (node.Attributes["ShowCaption"] != null)
                {
                    var value = node.Attributes["ShowCaption"].Value;
                    if (string.IsNullOrEmpty(value)) //Empty boolean should be ignored
                    {
                        node.Attributes.Remove(node.Attributes["ShowCaption"]);
                    }
                }

                foreach (System.Xml.XmlNode child in node.ChildNodes)
                {
                    CleanNodes(child);
                }
            }

            var xml = doc.OuterXml;

            var controller = DevToolsWindow.GetController<XenialDevToolsController>();

            static string BuildXafmlHtml(string xml, bool prettyPrint)
            {
                var node = VisualizeNodeHelper.PrettyPrint(xml, prettyPrint);

                var code = new HtmlBuilder.CodeBlock("xml", node);

                return HtmlBuilder.BuildHtml("Xafml", $"{code}");
            }

            devToolsViewModel.Xafml = BuildXafmlHtml(xml, controller.PrettyPrintXml);

            void PrettyPrint(object? s, EventArgs e)
                => devToolsViewModel.Xafml = BuildXafmlHtml(xml, controller.PrettyPrintXml);

            controller.PrettyPrintXmlSimpleAction.Executed -= PrettyPrint;
            controller.PrettyPrintXmlSimpleAction.Executed += PrettyPrint;

            static string ListViewBuilderCode(string xml, IModelListView modelListViewModel)
            {
                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);
                var root = doc.FirstChild;
                static string ListViewOptionsCode(XmlNode node)
                {
                    var sb = CurlyIndenter.Create();

                    var ignoredAttributes = new[] { "Id", "ClassName" };
                    var attributes = node.Attributes
                        .OfType<XmlAttribute>()
                        .Where(n => !ignoredAttributes.Contains(n.Name?.ToString()))
                        .ToList();

                    var members = typeof(ListViewOptions).GetProperties();

                    using (sb.OpenBrace($"new {nameof(ListViewOptions)}"))
                    {
                        foreach (var attribute in attributes)
                        {
                            var member = members.FirstOrDefault(m => m.Name == attribute.Name);
                            if (member is not null)
                            {
                                var value = attribute.Value;
                                var valueToWrite = value?.ToString();
                                if (member.PropertyType == typeof(string))
                                {
                                    valueToWrite = $"\"{valueToWrite}\"";
                                }
                                if (member.PropertyType == typeof(bool))
                                {
                                    valueToWrite = $"{bool.Parse(valueToWrite)}".ToLowerInvariant();
                                }
                                sb.WriteLine($"{member.Name} = {valueToWrite},");
                            }
                        }
                    }

                    return sb.ToString();
                }

                static string ListViewBuildersCode(XmlNode node)
                {
                    var sb = CurlyIndenter.Create();
                    var options = ListViewOptionsCode(node).TrimEnd();

                    using (sb.OpenBrace($"public Columns BuildColumns() => new Columns({options})", ";"))
                    {
                        foreach (var columns in node.ChildNodes.OfType<XmlNode>().Where(m => m.Name == nameof(IModelListView.Columns)))
                        {
                            var indexOffset = 0;

                            var columnNodes = columns
                                .ChildNodes
                                .OfType<XmlNode>()
                                .Where(m => m.Name == "ColumnInfo")
                                .ToList();


                            foreach (var column in columnNodes)
                            {
                                sb.Write($"Column.{GetAttribute(column, nameof(IModelColumn.Id))}");

                                (indexOffset, var propertiesToWrite) = MapAttributes<Column>(indexOffset, column, columnNodes);

                                if (propertiesToWrite.Count > 0)
                                {
                                    using (sb.OpenBrace(" with ", ","))
                                    {
                                        foreach (var property in propertiesToWrite)
                                        {
                                            sb.WriteLine($"{property.Key} = {property.Value},");
                                        }
                                    }
                                }
                                else
                                {
                                    sb.Write(",");
                                    sb.WriteLine();
                                }
                            }
                        }
                    }

                    return sb.ToString();
                }

                return ListViewBuildersCode(root);
            }

            static string DetailViewBuilderCode(string xml, IModelDetailView detailViewModel)
            {
                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);
                var root = doc.FirstChild;

                static string DetailViewOptionsCode(XmlNode node)
                {
                    var sb = CurlyIndenter.Create();

                    var ignoredAttributes = new[] { "Id", "ClassName" };

                    using (sb.OpenBrace($"new {nameof(DetailViewOptions)}"))
                    {
                        var mappedItems = MapAttributes(typeof(DetailViewOptions), node, ignoredAttributes);

                        foreach (var attribute in mappedItems)
                        {
                            sb.WriteLine($"{attribute.Key} = {attribute.Value},");
                        }
                    }

                    return sb.ToString();
                }

                static string LayoutBuildersCode(XmlNode node)
                {
                    var sb = CurlyIndenter.Create();
                    var options = DetailViewOptionsCode(node).TrimEnd();

                    var itemsNode = node.ChildNodes.OfType<XmlNode>().First(m => m.Name == "Items");
                    var layoutNode = node.ChildNodes.OfType<XmlNode>().First(m => m.Name == "Layout");

                    //We ignore the Main Node
                    var rootLayoutNode = layoutNode.ChildNodes.Count == 1
                        ? layoutNode.ChildNodes[0]
                        : layoutNode;

                    static (bool hasAttribute, string attributeName, string? attributeValue) GetAttributeInfo(XmlNode node, string attributeName)
                    {
                        if (node == null)
                        {
                            return (false, attributeName, null);
                        }

                        var attribute = node.Attributes.GetNamedItem(attributeName);

                        return (attribute != null, attributeName, attribute?.Value);
                    }

                    static Type GetTargetType(XmlNode node)
                        => node switch
                        {
                            { Name: "LayoutGroup", ParentNode: { Name: "TabbedGroup" } } =>
                                GetAttributeInfo(node, "Direction") switch
                                {
                                    (true, _, "Horizontal") => typeof(HorizontalLayoutTabGroupItem),
                                    //Default is Vertical
                                    _ => typeof(VerticalLayoutTabGroupItem),
                                },
                            { Name: "LayoutGroup" } =>
                                GetAttributeInfo(node, "Direction") switch
                                {
                                    (true, _, "Horizontal") => typeof(HorizontalLayoutGroupItem),
                                    //Default is Vertical
                                    _ => typeof(VerticalLayoutGroupItem),
                                },
                            { Name: "TabbedGroup" } => typeof(LayoutTabbedGroupItem),
                            { Name: "LayoutItem" } =>
                                GetAttributeInfo(node, "ViewItem") switch
                                {
                                    (true, _, _) => typeof(LayoutViewItem),
                                    _ => typeof(LayoutEmptySpaceItem)
                                },
                            { Name: "SplitterItem" } => typeof(LayoutSplitterItem),
                            { Name: "SeparatorItem" } => typeof(LayoutSeparatorItem),
                            _ => throw new ArgumentOutOfRangeException(nameof(node), node, "Could not find node type")
                        };

                    var items = new List<LayoutGeneratorInfo>();

                    static void CollectNodeTree(XmlNode node, IList<LayoutGeneratorInfo> children)
                    {
                        var targetType = GetTargetType(node);
                        var item = new LayoutGeneratorInfo(targetType, node);
                        children.Add(item);
                        foreach (var childNode in node.ChildNodes.OfType<XmlNode>())
                        {
                            CollectNodeTree(childNode, item.Children);
                        }
                    }

                    foreach (var childNode in rootLayoutNode.ChildNodes.OfType<XmlNode>())
                    {
                        CollectNodeTree(childNode, items);
                    }

                    using (sb.OpenBrace($"public Layout BuildLayout() => new Layout({options})", ";"))
                    {
                        foreach (var item in items)
                        {
                            PrintNode(sb, item, itemsNode);

                            if (items.LastOrDefault() != item)
                            {
                                sb.WriteLine(",");
                            }
                            else
                            {
                                sb.WriteLine();
                            }
                        }
                    }

                    static void PrintNode(CurlyIndenter sb, LayoutGeneratorInfo item, XmlNode itemsNode)
                    {
                        if (!item.IsLeaf)
                        {
                            var methodName = item.TargetNodeType switch
                            {
                                Type t when t == typeof(HorizontalLayoutGroupItem) => "HorizontalGroup",
                                Type t when t == typeof(VerticalLayoutGroupItem) => "VerticalGroup",
                                Type t when t == typeof(LayoutTabbedGroupItem) => "TabbedGroup",
                                Type t when typeof(LayoutTabGroupItem).IsAssignableFrom(t) => "Tab",
                                _ => throw new ArgumentOutOfRangeException(nameof(item), item, "Could not find method for type")
                            };

                            using (sb.OpenBrace($"{methodName}(", openBrace: "", closeBrace: ")", writeLine: false))
                            {
                                foreach (var child in item.Children)
                                {
                                    PrintNode(sb, child, itemsNode);
                                    if (item.Children.LastOrDefault() != child)
                                    {
                                        sb.WriteLine(",");
                                    }
                                    else
                                    {
                                        sb.WriteLine();
                                    }
                                }
                            }

                            (var indexOffset, var propertiesToWrite) = MapAttributes(item.TargetNodeType, 0, item.Node, item.Node.ParentNode.ChildNodes.OfType<XmlNode>().ToList());

                            if (propertiesToWrite.Count > 0)
                            {
                                sb.WriteLine(" with ");
                                sb.WriteLine("{");
                                sb.Indent();

                                foreach (var property in propertiesToWrite)
                                {
                                    sb.WriteLine($"{property.Key} = {property.Value},");
                                }

                                sb.UnIndent();
                                sb.Write("}");
                            }
                        }
                        else
                        {
                            PrintLeafNode(sb, item, itemsNode);
                        }
                    }

                    static void PrintLeafNode(CurlyIndenter sb, LayoutGeneratorInfo item, XmlNode itemsNode)
                    {
                        if (item.TargetNodeType == typeof(LayoutEmptySpaceItem))
                        {
                            sb.Write("EmptySpace()");
                            return;
                        }
                        if (typeof(LayoutViewItem).IsAssignableFrom(item.TargetNodeType))
                        {
                            var (hasViewItem, _, viewItemId) = GetAttributeInfo(item.Node, "ViewItem");
                            if (hasViewItem && viewItemId is not null)
                            {
                                var xmlViewItemNode = itemsNode.ChildNodes
                                    .OfType<XmlNode>()
                                    .FirstOrDefault(m =>
                                        m.Attributes
                                        .OfType<XmlAttribute>()
                                        .Any(m => m.Name == "Id" && m.Value == viewItemId)
                                    );

                                if (xmlViewItemNode is not null)
                                {
                                    //TODO: ExpandMemberAttribute
                                    //We replace dots with underscore because how
                                    //SourceGenerators define property trains
                                    sb.Write($"Editor.{viewItemId.Replace(".", "._")}");

                                    (var indexOffset, var propertiesToWrite) = MapAttributes<LayoutPropertyEditorItem>(0, item.Node, item.Node.ParentNode.ChildNodes.OfType<XmlNode>().ToList());
                                    (_, var propertiesToWrite2) = MapAttributes<LayoutPropertyEditorItem>(0, xmlViewItemNode, xmlViewItemNode.ParentNode.ChildNodes.OfType<XmlNode>().ToList());

                                    foreach (var pair in propertiesToWrite2)
                                    {
                                        propertiesToWrite.Add(pair.Key, pair.Value);
                                    }

                                    if (propertiesToWrite.Count > 0)
                                    {
                                        sb.WriteLine(" with ");
                                        sb.WriteLine("{");
                                        sb.Indent();

                                        foreach (var property in propertiesToWrite)
                                        {
                                            sb.WriteLine($"{property.Key} = {property.Value},");
                                        }

                                        sb.UnIndent();
                                        sb.Write("}");
                                    }
                                }
                            }
                        }
                    }

                    return sb.ToString();
                }

                return LayoutBuildersCode(root);
            }

            ((IModelView)copy).Remove();

            if (view.Model is IModelListView listViewModel)
            {
                devToolsViewModel.Code = HtmlBuilder.BuildHtml("Code", $"{new HtmlBuilder.CodeBlock("csharp", ListViewBuilderCode(node, listViewModel))}");
            }
            if (view.Model is IModelDetailView detailViewModel)
            {
                devToolsViewModel.Code = HtmlBuilder.BuildHtml("Code", $"{new HtmlBuilder.CodeBlock("csharp", DetailViewBuilderCode(node, detailViewModel))}");
            }
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

    private static (int indexOffset, Dictionary<string, string> propertiesToWrite) MapAttributes<TTargetObject>(int indexOffset, XmlNode node, IList<XmlNode> neighborNodes)
        => MapAttributes(typeof(TTargetObject), indexOffset, node, neighborNodes);

    private static (int indexOffset, Dictionary<string, string> propertiesToWrite) MapAttributes(Type targetObjectType, int indexOffset, XmlNode column, IList<XmlNode> neighborNodes)
    {
        var propertiesToWrite = new Dictionary<string, string>();
        var ignoredAttributes = new[]
        {
            nameof(IModelColumn.Id),
            nameof(IModelColumn.PropertyName),
            nameof(IModelColumn.Index),
        };

        var attributes = column.Attributes.OfType<XmlAttribute>()
            .Where(m => !ignoredAttributes.Contains(m.Name))
            .ToList();

        var shouldWriteIndex = false;
        var indexToWrite = 0;

        var index = GetAttribute(column, nameof(IModelColumn.Index));

        if (!string.IsNullOrEmpty(index))
        {
            if (int.TryParse(index, out var indexInt))
            {
                var indexInList = neighborNodes.IndexOf(column);
                var indexWithOffset = indexInList - indexOffset;
                if (indexInt < 0)
                {
                    shouldWriteIndex = true;
                    indexToWrite = indexInt;
                    indexOffset++;
                }
                if (indexInt > 0 && indexInt != indexWithOffset)
                {
                    indexToWrite = indexInt;
                    shouldWriteIndex = true;
                }
            }
        }

        if (shouldWriteIndex)
        {
            propertiesToWrite[nameof(IModelNode.Index)] = indexToWrite.ToString();
        }

        foreach (var item in MapAttributes(targetObjectType, column))
        {
            if (!propertiesToWrite.ContainsKey(item.Key))
            {
                propertiesToWrite.Add(item.Key, item.Value);
            }
        }

        return (indexOffset, propertiesToWrite);
    }

    private static Dictionary<string, string> MapAttributes(Type targetObjectType, XmlNode node, IEnumerable<string>? ignored = null)
    {
        if (ignored is null)
        {
            ignored = Enumerable.Empty<string>();
        }

        var propertiesToWrite = new Dictionary<string, string>();
        var ignoredAttributes = new[]
        {
            nameof(IModelColumn.Id),
            nameof(IModelColumn.PropertyName),
            nameof(IModelColumn.Index),
        }.Concat(ignored);

        var attributes = node.Attributes.OfType<XmlAttribute>()
            .Where(m => !ignoredAttributes.Contains(m.Name))
            .ToList();

        var members = targetObjectType.GetProperties();

        foreach (var attribute in attributes)
        {
            var member = members.FirstOrDefault(m => m.Name == attribute.Name);
            if (member is not null)
            {
                var value = attribute.Value;
                var valueToWrite = value?.ToString();
                if (member.PropertyType == typeof(string))
                {
                    valueToWrite = $"\"{valueToWrite}\"";
                }
                if (member.PropertyType == typeof(bool))
                {
                    valueToWrite = ParseBoolValue(valueToWrite);
                }

                static string ParseBoolValue(string? valueToWrite)
                    => bool.TryParse(valueToWrite, out var boolValue)
                        ? $"{boolValue}".ToLowerInvariant()
                        : "false";

                if (member.PropertyType.IsGenericType || member.PropertyType.IsEnum)
                {
                    var type = member.PropertyType.IsGenericType
                        ? Nullable.GetUnderlyingType(member.PropertyType)
                        : member.PropertyType;

                    if (type.IsEnum)
                    {
                        valueToWrite = $"{type.Name}.{valueToWrite}";
                    }
                    if (type == typeof(bool))
                    {
                        valueToWrite = ParseBoolValue(valueToWrite);
                    }
                }
                propertiesToWrite[member.Name] = valueToWrite;
            }
        }

        return propertiesToWrite;
    }

    private static string? GetAttribute(XmlNode node, string name)
    {
        var attribute = node.Attributes.OfType<XmlAttribute>()
            .FirstOrDefault(m => m.Name == name);
        if (attribute is not null)
        {
            return attribute.Value;
        }
        return null;
    }


    internal record LayoutGeneratorInfo(Type TargetNodeType, XmlNode Node)
    {
        public IList<LayoutGeneratorInfo> Children { get; } = new List<LayoutGeneratorInfo>();

        public bool IsLeaf => typeof(LayoutItemLeaf).IsAssignableFrom(TargetNodeType);
    }

}
