﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

using Xenial.Framework.DevTools.Helpers;
using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.ColumnItems;
using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;
using Xenial.Framework.MsBuild;

namespace Xenial.Framework.DevTools.X2C;

/// <summary>
/// 
/// </summary>
/// <param name="TargetNameSpace"></param>
/// <param name="TargetClassName"></param>
/// <param name="Namespace"></param>
/// <param name="ClassName"></param>
/// <param name="MethodName"></param>
/// <param name="ViewId"></param>
/// <param name="Code"></param>
/// <param name="Xml"></param>
public record X2CCodeResult(string TargetNameSpace, string TargetClassName, string Namespace, string ClassName, string MethodName, string ViewId, string Code, string Xml)
{
    /// <summary>
    /// 
    /// </summary>
    public string TargetFullName => $"{TargetNameSpace}.{TargetClassName}";

    /// <summary>
    /// 
    /// </summary>
    public string FullName => $"{Namespace}.{ClassName}";
}

/// <summary>
/// 
/// </summary>
public sealed class X2CEngine
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="view"></param>
    /// <returns></returns>
    public static X2CCodeResult ConvertToCode(IModelView view!!)
    {
        var node = LoadXml(view);
        return ConvertToCode(node);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="view"></param>
    /// <returns></returns>
    public static string ConvertToXml(IModelView view!!)
    {
        var node = LoadXml(view);
        return VisualizeNodeHelper.PrettyPrint(node.OuterXml);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static X2CCodeResult ConvertToCode(string xml!!)
    {
        var root = LoadXml(xml);

        CleanNodes(root);

        return root.Name switch
        {
            "ListView" => ListViewBuilderCodeClass(root),
            "DetailView" => DetailViewBuilderCodeClass(root),
            _ => throw new ArgumentOutOfRangeException(nameof(xml), root.Name, "Code builder is not handled")
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static X2CCodeResult ConvertToCode(XmlNode root!!)
    {
        CleanNodes(root);

        return root.Name switch
        {
            "ListView" => ListViewBuilderCodeClass(root),
            "DetailView" => DetailViewBuilderCodeClass(root),
            _ => throw new ArgumentOutOfRangeException(nameof(root), root.Name, "Code builder is not handled")
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static XmlNode LoadXml(string xml!!)
    {
        var doc = new XmlDocument() { XmlResolver = null };
        var sreader = new System.IO.StringReader(xml);
        using var reader = XmlReader.Create(sreader, new XmlReaderSettings() { XmlResolver = null });
        doc.Load(reader);

        return doc.FirstChild ?? throw new InvalidOperationException($"{nameof(doc)}.{doc.FirstChild}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="view"></param>
    /// <returns></returns>
    public static XmlNode LoadXml(IModelView view!!)
    {
        var id = $"{view.Id}_{Guid.NewGuid()}";
        var modelViews = view.Application.Views;
        var copy = ((ModelNode)modelViews).AddClonedNode((ModelNode)view, id);

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
                            value = ((ModelNode)view).GetValue(property.Name);
                            if (attribute.Value.Equals(value))
                            {
                                copy.ClearValue(property.Name);
                            }
                            else
                            {
                                var viewValue = property.GetValue(view);
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
        node = node.Replace(id, view.Id); //Patch ViewId

        var xml = LoadXml(node);

        CleanNodes(xml);

        ((IModelView)copy).Remove();

        return xml;
    }

    private static X2CCodeResult ListViewBuilderCodeClass(XmlNode root)
    {
        var className = GetAttribute(root, "ClassName");
        var viewId = GetAttribute(root, "Id") ?? throw new InvalidOperationException("viewId");

        var (@namespace, @class) = className switch
        {
            string c when c.Contains('.'
#if NET5_0_OR_GREATER
                , StringComparison.OrdinalIgnoreCase
#endif
            ) => (string.Join(".", c.Split('.').SkipLast()), c.Split('.').Last()),
            _ => throw new ArgumentOutOfRangeException(nameof(className), className, "Given name cannot be split into namespace and classname")
        };

        static string DefaultListViewId(string @class) => $"{@class}{ListViewIdSuffix}";
        static string DefaultLookupListViewId(string @class) => $"{@class}{LookupListViewIdSuffix}";

        static string CustomLayoutMethodName(string @class, string viewId)
        {
            var isLookup = viewId.EndsWith(LookupListViewIdSuffix, StringComparison.OrdinalIgnoreCase);

            var newViewId = TrimEnd(TrimEnd(TrimStart(viewId, @class), ListViewIdSuffix), LookupListViewIdSuffix).Trim('_');

            if (isLookup)
            {
                while (newViewId.EndsWith("Columns", StringComparison.OrdinalIgnoreCase))
                {
                    newViewId = TrimEnd(newViewId, "Columns");
                }
            }

            var methodName = $"Build{newViewId}{(isLookup ? "Lookup" : "")}Columns";

            while (methodName.EndsWith("Columns", StringComparison.OrdinalIgnoreCase))
            {
                methodName = TrimEnd(methodName, "Columns");
            }

            return $"{methodName}Columns";
        }

        var methodName = viewId.Equals(DefaultListViewId(@class), StringComparison.OrdinalIgnoreCase)
            ? BuildColumnsMethodName
            : (viewId.Equals(DefaultLookupListViewId(@class), StringComparison.OrdinalIgnoreCase)
            ? BuildLookupColumnsMethodName
            : CustomLayoutMethodName(@class, viewId));

        var resultClassName = $"{@class}ColumnsBuilder";

        var methodSb = CurlyIndenter.Create();
        methodSb.Indent();
        methodSb.Indent();
        ListViewBuilderCodeMethod(root, methodSb, methodName, out var additionalUsings, out var additionalAttributes);

        var sb = CurlyIndenter.Create();

        sb.WriteLine("using System;");
        sb.WriteLine("using System.Linq;");
        sb.WriteLine();
        sb.WriteLine("using Xenial.Framework.Layouts;");
        sb.WriteLine("using Xenial.Framework.Layouts.ColumnItems;");
        sb.WriteLine();

        if (additionalUsings.Count > 0)
        {
            foreach (var additionalUsing in additionalUsings.Distinct().OrderBy(ns => ns.Length))
            {
                sb.WriteLine($"using {additionalUsing};");
            }
            sb.WriteLine();
        }

        using (sb.OpenBrace($"namespace {@namespace}"))
        {
            foreach (var additionalAttribute in additionalAttributes.Distinct().OrderByDescending(a => a))
            {
                sb.WriteLine(additionalAttribute);
            }

            using (sb.OpenBrace($"public sealed partial class {resultClassName} : ColumnsBuilder<{@class}>"))
            {
                sb.WriteLine(methodSb.ToString().TrimEnd());
            }
        }

        var result = new X2CCodeResult(@namespace, @class, @namespace, resultClassName, methodName, viewId, sb.ToString(), root.OuterXml);

        return result;
    }

    private static string ListViewBuilderCodeMethod(XmlNode root, CurlyIndenter sb, string methodName, out List<string> additionalUsings, out List<string> additionalAttributes)
    {
        additionalUsings = new();
        additionalAttributes = new();

        static string ListViewOptionsCode(XmlNode node, CurlyIndenter sb, List<string> additionalUsings)
        {
            var ignoredAttributes = new[] { "Id", "ClassName" };

            using (sb.OpenBrace($"new {nameof(ListViewOptions)}", closeBrace: "})"))
            {
                var propertiesToWrite = MapAttributes(typeof(ListViewOptions), node, ignoredAttributes, additionalUsings);

                foreach (var attribute in propertiesToWrite)
                {
                    sb.WriteLine($"{attribute.Key} = {attribute.Value},");
                }
            }

            return sb.ToString();
        }

        static string ListViewBuildersCode(XmlNode node, CurlyIndenter sb, string methodName, List<string> additionalUsings, List<string> additionalAttributes)
        {
            var cols = node.ChildNodes.OfType<XmlNode>().Where(m => m.Name == nameof(IModelListView.Columns));

            if (!cols.Any())
            {
                sb.WriteLine($"public Columns {methodName}() => new Columns();");

                return sb.ToString();
            }

            sb.Write($"public Columns {methodName}() => new Columns(");

            ListViewOptionsCode(node, sb, additionalUsings);

            sb.WriteLine("{");
            sb.Indent();

            foreach (var columns in cols)
            {
                var indexOffset = 0;

                var columnNodes = columns
                    .ChildNodes
                    .OfType<XmlNode>()
                    .Where(m => m.Name == "ColumnInfo")
                    .ToList();

                foreach (var column in columnNodes)
                {
                    //TODO: Empty Id?
                    var viewItemId = GetAttribute(column, nameof(IModelColumn.PropertyName))
                        ?? GetAttribute(column, nameof(IModelColumn.Id))
                        ?? "";

                    AddXenialExpandMemberAttributes(additionalUsings, additionalAttributes, viewItemId);
                    sb.Write(ViewItemPath("Column", viewItemId));

                    (indexOffset, var propertiesToWrite) = MapAttributes<Column>(indexOffset, column, columnNodes, additionalUsings);

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
            sb.UnIndent();
            sb.WriteLine("};");

            return sb.ToString();
        }

        return ListViewBuildersCode(root, sb, methodName, additionalUsings, additionalAttributes);
    }

    internal const string DetailViewIdSuffix = "_DetailView";
    internal const string ListViewIdSuffix = "_ListView";
    internal const string LookupListViewIdSuffix = "_LookupListView";

    internal static string GetNestedListViewId(Type type, string propertyName)
        => $"{type}_{propertyName}_ListView";

    internal const string BuildLayoutMethodName = "BuildLayout";
    internal const string BuildColumnsMethodName = "BuildColumns";
    internal const string BuildLookupColumnsMethodName = "BuildLookupColumns";

    private static string TrimStart(string source, string value)
    {
        if (!source.StartsWith(value, StringComparison.OrdinalIgnoreCase))
        {
            return source;
        }

        if (string.IsNullOrEmpty(value))
        {
            return source;
        }

        var result = source;
        while (result.StartsWith(value, StringComparison.OrdinalIgnoreCase))
        {
            result = result.Substring(value.Length);
        }

        return result;
    }

    private static string TrimEnd(string source, string value)
    {
        if (!source.EndsWith(value, StringComparison.OrdinalIgnoreCase))
        {
            return source;
        }

        return source.Remove(source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));
    }

    private static X2CCodeResult DetailViewBuilderCodeClass(XmlNode root)
    {
        var className = GetAttribute(root, "ClassName");
        var viewId = GetAttribute(root, "Id") ?? throw new InvalidOperationException("viewId");

        var (@namespace, @class) = className switch
        {
            string c when c.Contains('.'
#if NET5_0_OR_GREATER
                , StringComparison.OrdinalIgnoreCase
#endif
            ) => (string.Join(".", c.Split('.').SkipLast()), c.Split('.').Last()),
            _ => throw new ArgumentOutOfRangeException(nameof(className), className, "Given name cannot be split into namespace and classname")
        };

        static string DefaultDetailViewId(string @class) => $"{@class}{DetailViewIdSuffix}";

        static string CustomLayoutMethodName(string @class, string viewId)
        {
            var newViewId = TrimEnd(TrimStart(viewId, @class), DetailViewIdSuffix).Trim('_');

            var methodName = $"Build{newViewId}Layout";

            while (methodName.EndsWith("Layout", StringComparison.OrdinalIgnoreCase))
            {
                methodName = TrimEnd(methodName, "Layout");
            }

            return $"{methodName}Layout";
        }

        var methodName = viewId.Equals(DefaultDetailViewId(@class), StringComparison.OrdinalIgnoreCase)
            ? BuildLayoutMethodName
            : CustomLayoutMethodName(@class, viewId);

        var resultClassName = $"{@class}LayoutBuilder";

        var methodSb = CurlyIndenter.Create();
        methodSb.Indent();
        methodSb.Indent();
        DetailViewBuilderCodeMethod(root, methodSb, methodName, out var additionalUsings, out var additionalAttributes);

        var sb = CurlyIndenter.Create();

        sb.WriteLine("using System;");
        sb.WriteLine("using System.Linq;");
        sb.WriteLine();
        sb.WriteLine("using Xenial.Framework.Layouts;");
        sb.WriteLine("using Xenial.Framework.Layouts.ColumnItems;");
        sb.WriteLine("using Xenial.Framework.Layouts.Items.Base;");
        sb.WriteLine();

        if (additionalUsings.Count > 0)
        {
            foreach (var additionalUsing in additionalUsings.Distinct().OrderBy(ns => ns.Length))
            {
                sb.WriteLine($"using {additionalUsing};");
            }
            sb.WriteLine();
        }

        using (sb.OpenBrace($"namespace {@namespace}"))
        {
            foreach (var additionalAttribute in additionalAttributes.Distinct().OrderByDescending(a => a))
            {
                sb.WriteLine(additionalAttribute);
            }
            using (sb.OpenBrace($"public sealed partial class {resultClassName} : LayoutBuilder<{@class}>"))
            {
                sb.WriteLine(methodSb.ToString().TrimEnd());
            }
        }
        var result = new X2CCodeResult(@namespace, @class, @namespace, resultClassName, methodName, viewId, sb.ToString(), root.OuterXml);

        return result;
    }

    private static string ViewItemPath(string prefix, string viewItemId)
    {

        if (viewItemId.Contains('.'
#if NET5_0_OR_GREATER
                                , StringComparison.OrdinalIgnoreCase
#endif
                                )
        )
        {
            var viewItemIdParts = viewItemId.Split('.');
            var last = viewItemIdParts.Last();
            var previous = viewItemIdParts.SkipLast().Select(part => $"_{part}");
            var items = previous.Concat(new[] { last }).ToArray();

            var result = $"{prefix}.{string.Join(".", items)}";
            return result;
        }

        return $"{prefix}.{viewItemId}";
    }

    private static void AddXenialExpandMemberAttributes(List<string> additionalUsings, List<string> additionalAttributes, string? viewItemId)
    {
        if (viewItemId.Contains('.'
#if NET5_0_OR_GREATER
                                , StringComparison.OrdinalIgnoreCase
#endif
                                )
)
        {
            additionalUsings.Add("Xenial");

            //Add the addtional constant parts
            var parts = new Stack<string>(viewItemId.Split('.').SkipLast());

            while (parts.TryPop(out var part))
            {
                var prevPath = string.Join(".", parts.Concat(new[] { part }).ToArray());
                if (!string.IsNullOrEmpty(prevPath))
                {
                    additionalAttributes.Add($"[XenialExpandMember({ViewItemPath("Constants", prevPath)})]");
                }
            }

            additionalAttributes.Add($"[XenialExpandMember({ViewItemPath("Constants", viewItemId)})]");
        }
    }

    private static string DetailViewBuilderCodeMethod(XmlNode root, CurlyIndenter sb, string methodName, out List<string> additionalUsings, out List<string> additionalAttributes)
    {
        additionalUsings = new();
        additionalAttributes = new();

        static string DetailViewOptionsCode(XmlNode node, CurlyIndenter sb, List<string> additionalUsings)
        {
            var ignoredAttributes = new[] { "Id", "ClassName" };

            using (sb.OpenBrace($"new {nameof(DetailViewOptions)}", closeBrace: "})"))
            {
                var mappedItems = MapAttributes(typeof(DetailViewOptions), node, ignoredAttributes, additionalUsings);

                foreach (var attribute in mappedItems)
                {
                    sb.WriteLine($"{attribute.Key} = {attribute.Value},");
                }
            }

            return sb.ToString();
        }

        static string LayoutBuildersCode(XmlNode node, CurlyIndenter sb, string methodName, List<string> additionalUsings, List<string> additionalAttributes)
        {
            var itemsNode = node.ChildNodes.OfType<XmlNode>().FirstOrDefault(m => m.Name == "Items");
            var layoutNode = node.ChildNodes.OfType<XmlNode>().FirstOrDefault(m => m.Name == "Layout");

            if (itemsNode is null || layoutNode is null)
            {
                sb.WriteLine($"public Layout {methodName}() => new Layout();");

                return sb.ToString();
            }

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
                    { Name: "LayoutGroup", ParentNode.Name: "TabbedGroup" } =>
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
                    _ => throw new ArgumentOutOfRangeException(nameof(node), node, $"Could not find node type: '{node.Name}'{Environment.NewLine}{node.OuterXml}")
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

            sb.Write($"public Layout {methodName}() => new Layout(");
            DetailViewOptionsCode(node, sb, additionalUsings);
            sb.WriteLine("{");
            sb.Indent();

            foreach (var item in items)
            {
                PrintNode(sb, item, itemsNode, additionalUsings, additionalAttributes);

                if (items.LastOrDefault() != item)
                {
                    sb.WriteLine(",");
                }
                else
                {
                    sb.WriteLine();
                }
            }

            sb.UnIndent();
            sb.WriteLine("};");

            static void PrintNode(CurlyIndenter sb, LayoutGeneratorInfo item, XmlNode itemsNode, List<string> additionalUsings, List<string> additionalAttributes)
            {
                if (!item.IsLeaf)
                {
                    var methodName = item.TargetNodeType switch
                    {
                        Type t when t == typeof(HorizontalLayoutGroupItem) => "HorizontalGroup",
                        Type t when t == typeof(VerticalLayoutGroupItem) => "VerticalGroup",
                        Type t when t == typeof(LayoutTabbedGroupItem) => "TabbedGroup",
                        Type t when typeof(LayoutTabGroupItem).IsAssignableFrom(t) => "Tab",
                        Type t when typeof(LayoutSeparatorItem).IsAssignableFrom(t) => "Separator",
                        Type t when typeof(LayoutSplitterItem).IsAssignableFrom(t) => "Splitter",
                        _ => throw new ArgumentOutOfRangeException(nameof(item), item, "Could not find method for type")
                    };

                    using (sb.OpenBrace($"{methodName}(", openBrace: "", closeBrace: ")", writeLine: false))
                    {
                        foreach (var child in item.Children)
                        {
                            PrintNode(sb, child, itemsNode, additionalUsings, additionalAttributes);
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

                    (var indexOffset, var propertiesToWrite) = MapAttributes(item.TargetNodeType, 0, item.Node, item.Node.ParentNode.ChildNodes.OfType<XmlNode>().ToList(), additionalUsings);

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
                    PrintLeafNode(sb, item, itemsNode, additionalUsings, additionalAttributes);
                }
            }

            static void PrintLeafNode(CurlyIndenter sb, LayoutGeneratorInfo item, XmlNode itemsNode, List<string> additionalUsings, List<string> additionalAttributes)
            {
                if (item.TargetNodeType == typeof(LayoutEmptySpaceItem))
                {
                    sb.Write("EmptySpaceItem()");
                    return;
                }
                if (item.TargetNodeType == typeof(LayoutSplitterItem))
                {
                    sb.Write("Splitter()");
                    return;
                }
                if (item.TargetNodeType == typeof(LayoutSeparatorItem))
                {
                    sb.Write("Separator()");
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
                            //SourceGenerators define property trains
                            AddXenialExpandMemberAttributes(additionalUsings, additionalAttributes, viewItemId);
                            sb.Write(ViewItemPath("Editor", viewItemId));

                            (var indexOffset, var propertiesToWrite) = MapAttributes<LayoutPropertyEditorItem>(0, item.Node, item.Node.ParentNode.ChildNodes.OfType<XmlNode>().ToList(), additionalUsings);
                            (_, var propertiesToWrite2) = MapAttributes<LayoutPropertyEditorItem>(0, xmlViewItemNode, xmlViewItemNode.ParentNode.ChildNodes.OfType<XmlNode>().ToList(), additionalUsings);

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

        return LayoutBuildersCode(root, sb, methodName, additionalUsings, additionalAttributes);
    }

    private static void CleanNodes(XmlNode node)
    {
        if (node.Attributes["IsNewNode"] is not null)
        {
            node.Attributes.Remove(node.Attributes["IsNewNode"]);
        }

        if (node.Attributes["ShowCaption"] is not null)
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

    private static (int indexOffset, Dictionary<string, string> propertiesToWrite) MapAttributes<TTargetObject>(int indexOffset, XmlNode node, IList<XmlNode> neighborNodes, List<string>? additionalUsings = null)
        => MapAttributes(typeof(TTargetObject), indexOffset, node, neighborNodes, additionalUsings);

    private static (int indexOffset, Dictionary<string, string> propertiesToWrite) MapAttributes(Type targetObjectType, int indexOffset, XmlNode column, IList<XmlNode> neighborNodes, List<string>? additionalUsings = null)
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

        foreach (var item in MapAttributes(targetObjectType, column, additionalUsings: additionalUsings))
        {
            if (!propertiesToWrite.ContainsKey(item.Key))
            {
                propertiesToWrite.Add(item.Key, item.Value);
            }
        }

        return (indexOffset, propertiesToWrite);
    }

    private static Dictionary<string, string> MapAttributes(Type targetObjectType, XmlNode node, IEnumerable<string>? ignored = null, List<string>? additionalUsings = null)
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

            if (member is null)
            {
                member = members.Select(m => (member: m, mappedFrom: m.GetCustomAttributes(false).OfType<MappedFromModelNodeAttribute>().FirstOrDefault()))
                    .Where(m => m.mappedFrom is not null)
                    .Where(m => m.mappedFrom.ToNode == attribute.Name)
                    .Select(m => m.member)
                    .FirstOrDefault();
            }

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
                        additionalUsings?.Add(type.Namespace);
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

}

internal record LayoutGeneratorInfo(Type TargetNodeType, XmlNode Node)
{
    public IList<LayoutGeneratorInfo> Children { get; } = new List<LayoutGeneratorInfo>();

    public bool IsLeaf => typeof(LayoutItemLeaf).IsAssignableFrom(TargetNodeType);
}
