using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.ColumnItems;
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
    /// <param name="listViewXml"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public string ConvertToCode(string xml)
        => ListViewBuilderCodeClass(xml);

    public string ListViewBuilderCodeClass(string xml)
    {
        var doc = new System.Xml.XmlDocument();
        doc.LoadXml(xml);
        var root = doc.FirstChild;
        var className = GetAttribute(root, "ClassName");

        var (@namespace, @class) = className switch
        {
            string c when c.Contains('.') => (string.Join(".", c.Split('.').SkipLast()), c.Split('.').Last()),
            _ => throw new ArgumentOutOfRangeException(nameof(className), className, "Given name cannot be split into namespace and classname")
        };

        var sb = CurlyIndenter.Create();

        sb.WriteLine("using System;");
        sb.WriteLine("using System.Linq;");
        sb.WriteLine();
        sb.WriteLine("using Xenial.Framework.Layouts;");
        sb.WriteLine("using Xenial.Framework.Layouts.ColumnItems;");
        sb.WriteLine();
        using (sb.OpenBrace($"namespace {@namespace}"))
        using (sb.OpenBrace($"public sealed partial class {@class}ColumnsBuilder : ColumnsBuilder<{@class}>"))
        {
            ListViewBuilderCodeMethod(xml, sb);
        }

        return sb.ToString();
    }

    private static string ListViewBuilderCodeMethod(string xml, CurlyIndenter sb)
    {
        var doc = new System.Xml.XmlDocument();
        doc.LoadXml(xml);
        var root = doc.FirstChild;
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
