using System;
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
public sealed class X2CEngine
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="view"></param>
    /// <returns></returns>
    public static (string className, string code) ConvertToCode(IModelView view!!)
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
    public static (string className, string code) ConvertToCode(string xml!!)
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
    public static (string className, string code) ConvertToCode(XmlNode root!!)
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


        return doc.FirstChild;
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

    private static (string className, string code) ListViewBuilderCodeClass(XmlNode root)
    {
        var className = GetAttribute(root, "ClassName");

        var (@namespace, @class) = className switch
        {
            string c when c.Contains('.') => (string.Join(".", c.Split('.').SkipLast()), c.Split('.').Last()),
            _ => throw new ArgumentOutOfRangeException(nameof(className), className, "Given name cannot be split into namespace and classname")
        };

        var resultClassName = $"{@class}ColumnsBuilder";

        var sb = CurlyIndenter.Create();

        sb.WriteLine("using System;");
        sb.WriteLine("using System.Linq;");
        sb.WriteLine();
        sb.WriteLine("using Xenial.Framework.Layouts;");
        sb.WriteLine("using Xenial.Framework.Layouts.ColumnItems;");
        sb.WriteLine();
        using (sb.OpenBrace($"namespace {@namespace}"))
        using (sb.OpenBrace($"public sealed partial class {resultClassName} : ColumnsBuilder<{@class}>"))
        {
            ListViewBuilderCodeMethod(root, sb);
        }

        return (resultClassName, sb.ToString());
    }

    private static string ListViewBuilderCodeMethod(XmlNode root, CurlyIndenter sb)
    {
        static string ListViewOptionsCode(XmlNode node, CurlyIndenter sb)
        {
            var ignoredAttributes = new[] { "Id", "ClassName" };
            var attributes = node.Attributes
                .OfType<XmlAttribute>()
                .Where(n => !ignoredAttributes.Contains(n.Name?.ToString()))
                .ToList();

            var members = typeof(ListViewOptions).GetProperties();

            using (sb.OpenBrace($"new {nameof(ListViewOptions)}", closeBrace: "})"))
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

        static string ListViewBuildersCode(XmlNode node, CurlyIndenter sb)
        {
            sb.Write("public Columns BuildColumns() => new Columns(");

            ListViewOptionsCode(node, sb);

            sb.WriteLine("{");
            sb.Indent();

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
            sb.UnIndent();
            sb.WriteLine("};");

            return sb.ToString();
        }

        return ListViewBuildersCode(root, sb);
    }

    private static (string className, string code) DetailViewBuilderCodeClass(XmlNode root)
    {
        var className = GetAttribute(root, "ClassName");

        var (@namespace, @class) = className switch
        {
            string c when c.Contains('.') => (string.Join(".", c.Split('.').SkipLast()), c.Split('.').Last()),
            _ => throw new ArgumentOutOfRangeException(nameof(className), className, "Given name cannot be split into namespace and classname")
        };

        var resultClassName = $"{@class}LayoutBuilder";

        var sb = CurlyIndenter.Create();

        sb.WriteLine("using System;");
        sb.WriteLine("using System.Linq;");
        sb.WriteLine();
        sb.WriteLine("using Xenial.Framework.Layouts;");
        sb.WriteLine("using Xenial.Framework.Layouts.ColumnItems;");
        sb.WriteLine("using Xenial.Framework.Layouts.Items.Base;");
        sb.WriteLine();
        using (sb.OpenBrace($"namespace {@namespace}"))
        using (sb.OpenBrace($"public sealed partial class {resultClassName} : LayoutBuilder<{@class}>"))
        {
            DetailViewBuilderCodeMethod(root, sb);
        }

        return (resultClassName, sb.ToString());
    }

    private static string DetailViewBuilderCodeMethod(XmlNode root, CurlyIndenter sb)
    {
        static string DetailViewOptionsCode(XmlNode node, CurlyIndenter sb)
        {
            var ignoredAttributes = new[] { "Id", "ClassName" };

            using (sb.OpenBrace($"new {nameof(DetailViewOptions)}", closeBrace: "})"))
            {
                var mappedItems = MapAttributes(typeof(DetailViewOptions), node, ignoredAttributes);

                foreach (var attribute in mappedItems)
                {
                    sb.WriteLine($"{attribute.Key} = {attribute.Value},");
                }
            }

            return sb.ToString();
        }

        static string LayoutBuildersCode(XmlNode node, CurlyIndenter sb)
        {
            var itemsNode = node.ChildNodes.OfType<XmlNode>().FirstOrDefault(m => m.Name == "Items");
            var layoutNode = node.ChildNodes.OfType<XmlNode>().FirstOrDefault(m => m.Name == "Layout");

            if (itemsNode is null || layoutNode is null)
            {
                return "/* ERROR: could not find a node named 'Items' or 'Layout' */";
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

            sb.Write("public Layout BuildLayout() => new Layout(");
            DetailViewOptionsCode(node, sb);
            sb.WriteLine("{");
            sb.Indent();

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

            sb.UnIndent();
            sb.WriteLine("};");


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
                        Type t when typeof(LayoutSeparatorItem).IsAssignableFrom(t) => "Separator",
                        Type t when typeof(LayoutSplitterItem).IsAssignableFrom(t) => "Splitter",
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

        return LayoutBuildersCode(root, sb);
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

}

internal record LayoutGeneratorInfo(Type TargetNodeType, XmlNode Node)
{
    public IList<LayoutGeneratorInfo> Children { get; } = new List<LayoutGeneratorInfo>();

    public bool IsLeaf => typeof(LayoutItemLeaf).IsAssignableFrom(TargetNodeType);
}
