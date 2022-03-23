using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Xenial.Cli.Xml
{
    /// <summary>   An XML formatter. </summary>
    public class XmlFormatter
    {
        private int currentAttributeSpace;

        private int currentStartLength;

        private XmlNodeType lastNodeType;

        private XmlFormatterOptions currentOptions = new();

        private static XmlDocument ConvertToXMLDocument(string input)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(input);
            return xmlDocument;
        }

        /// <summary>   Formats. </summary>
        ///
        /// <param name="input">                The input. </param>
        /// <param name="formattingOptions">    (Optional) Options for controlling the formatting. </param>
        ///
        /// <returns>   The formatted value. </returns>

        public string Format(string input, XmlFormatterOptions? formattingOptions = null)
        {
            string str;

            if (formattingOptions is not null)
            {
                currentOptions = currentOptions with
                {
                    IndentLength = formattingOptions.IndentLength,
                    UseSelfClosingTags = formattingOptions.UseSelfClosingTags,
                    UseSingleQuotes = formattingOptions.UseSingleQuotes,
                };
            }
            str = FormatXMLDocument(ConvertToXMLDocument(input));

            return str;
        }

        private string FormatXMLDocument(XmlDocument xml)
        {
            var stringBuilder = new StringBuilder();
            var xmlDeclaration = xml.ChildNodes.OfType<XmlDeclaration>().FirstOrDefault();
            if (xmlDeclaration != null)
            {
                lastNodeType = XmlNodeType.XmlDeclaration;
                stringBuilder.Append(string.Concat(xmlDeclaration.OuterXml, XmlFormatterConstants.Newline));
            }
            if (xml.DocumentType != null)
            {
                stringBuilder.Append(string.Concat(xml.DocumentType.OuterXml, XmlFormatterConstants.Newline));
            }
            var documentElement = xml.DocumentElement;
            lastNodeType = XmlNodeType.Document;
            if (documentElement is not null)
            {
                PrintNode(documentElement, stringBuilder);
            }
            return stringBuilder.ToString();
        }

        /// <summary>   Minimizes. </summary>
        ///
        /// <param name="xmlString">    The XML string. </param>
        ///
        /// <returns>   A string. </returns>

        public static string Minimize(string xmlString)
        {
            StringWriter stringWriter;
            string? encoding;
            var xMLDocument = ConvertToXMLDocument(xmlString);
            var xmlDeclaration = xMLDocument.ChildNodes.OfType<XmlDeclaration>().FirstOrDefault();
            if (xmlDeclaration is not null)
            {
                encoding = xmlDeclaration.Encoding;
            }
            else
            {
                encoding = null;
            }

            stringWriter = string.IsNullOrEmpty(encoding)
                ? new StringWriterWithEncoding()
                : new StringWriterWithEncoding(Encoding.GetEncoding(xmlDeclaration!.Encoding));

            using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings()
            {
                Indent = false,
                IndentChars = string.Empty,
                NewLineChars = string.Empty,
                NewLineHandling = NewLineHandling.Entitize,
                NewLineOnAttributes = false,
                NamespaceHandling = NamespaceHandling.OmitDuplicates
            }))
            {
                xMLDocument.Save(xmlWriter);
            }
            return stringWriter.ToString();
        }

        private void PrintNode(XmlNode node, StringBuilder sb)
        {
            char space;
            string str;
            bool count;
            string str1;
            var xmlNodeType = lastNodeType;
            lastNodeType = node.NodeType;
            switch (node.NodeType)
            {
                case XmlNodeType.None:
                case XmlNodeType.Element:
                case XmlNodeType.Attribute:
                case XmlNodeType.Entity:
                case XmlNodeType.Document:
                case XmlNodeType.DocumentFragment:
                case XmlNodeType.Notation:
                case XmlNodeType.EndElement:
                case XmlNodeType.EndEntity:
                case XmlNodeType.XmlDeclaration:
                    {
                        str = xmlNodeType != XmlNodeType.Text ? new string(XmlFormatterConstants.Space, currentStartLength) : string.Empty;
                        var str2 = str;
                        sb.Append(string.Concat(str2, "<", node.Name));
                        var attributes = node.Attributes;
                        if (attributes != null)
                        {
                            count = attributes.Count > 0;
                        }
                        else
                        {
                            count = false;
                        }
                        if (count)
                        {
                            sb.Append(XmlFormatterConstants.Space);
                            currentAttributeSpace = currentStartLength + node.Name.Length + 2;
                            if (node.Attributes is not null)
                            {
                                for (var i = 0; i < node.Attributes.Count; i++)
                                {
                                    var itemOf = node.Attributes[i];
                                    var flag = i == node.Attributes.Count - 1;
                                    var stringBuilder = sb;
                                    var name = new string[]
                                    {
                                        itemOf.Name,
                                        currentOptions.UseSingleQuotes ? "='" : "=\"",
                                        itemOf.Value,
                                        currentOptions.UseSingleQuotes ? "'" : "\"",
                                        flag ? string.Empty : Environment.NewLine
                                    };

                                    stringBuilder.Append(string.Concat(name));
                                    if (!flag)
                                    {
                                        sb.Append(new string(XmlFormatterConstants.Space, currentAttributeSpace));
                                    }
                                    else if (node.HasChildNodes)
                                    {
                                        sb.Append('>');
                                    }
                                }
                            }
                        }
                        else if (node.HasChildNodes)
                        {
                            sb.Append('>');
                        }
                        if (!node.HasChildNodes)
                        {
                            if (currentOptions.UseSelfClosingTags)
                            {
                                space = XmlFormatterConstants.Space;
                                sb.Append(string.Concat(space.ToString(), "/>"));
                                return;
                            }
                            sb.AppendFormat(CultureInfo.InvariantCulture, string.Concat("></", node.Name, ">"), Array.Empty<object>());
                        }
                        else
                        {
                            if (!(node.ChildNodes.Cast<XmlNode>().First() is XmlText))
                            {
                                currentStartLength += currentOptions.IndentLength;
                            }
                            for (var j = 0; j < node.ChildNodes.Count; j++)
                            {
                                var xmlNodes = node.ChildNodes[j];
                                if (xmlNodes is not null
                                    && xmlNodes.NodeType != XmlNodeType.Text
                                    && xmlNodes.NodeType != XmlNodeType.CDATA
                                    && xmlNodes.NodeType != XmlNodeType.EntityReference
                                    && lastNodeType != XmlNodeType.Text
                                    && xmlNodes.NodeType != XmlNodeType.SignificantWhitespace
                                    && xmlNodes.NodeType != XmlNodeType.Whitespace
                                )
                                {
                                    sb.Append(XmlFormatterConstants.Newline);
                                }
                                if (xmlNodes is not null)
                                {
                                    PrintNode(xmlNodes, sb);
                                }
                            }
                            if (node.NodeType != XmlNodeType.Comment && node.NodeType != XmlNodeType.CDATA && node.NodeType != XmlNodeType.DocumentType && node.NodeType != XmlNodeType.Text)
                            {
                                if (currentStartLength >= currentOptions.IndentLength && lastNodeType != XmlNodeType.Text && lastNodeType != XmlNodeType.CDATA && lastNodeType != XmlNodeType.DocumentType && lastNodeType != XmlNodeType.EntityReference)
                                {
                                    currentStartLength -= currentOptions.IndentLength;
                                }
                                var str4 = lastNodeType == XmlNodeType.Text || lastNodeType == XmlNodeType.CDATA || lastNodeType == XmlNodeType.EntityReference ? string.Empty : XmlFormatterConstants.Newline;
                                var str5 = lastNodeType == XmlNodeType.Text || lastNodeType == XmlNodeType.EntityReference ? string.Empty : new string(XmlFormatterConstants.Space, currentStartLength);
                                sb.Append(string.Concat(new string[] { str4, str5, "</", node.Name, ">" }));
                                lastNodeType = node.NodeType;
                                return;
                            }
                        }
                        return;
                    }
                case XmlNodeType.Text:
                    {
                        sb.Append(node.OuterXml);
                        return;
                    }
                case XmlNodeType.CDATA:
                    {
                        var str6 = xmlNodeType == XmlNodeType.Text ? string.Empty : Environment.NewLine;
                        var str7 = xmlNodeType == XmlNodeType.Text ? string.Empty : new string(XmlFormatterConstants.Space, currentStartLength);
                        sb.Append(string.Concat(new string?[] { str6, str7, XmlFormatterConstants.CDataStart, node.Value, XmlFormatterConstants.CDataEnd }));
                        return;
                    }
                case XmlNodeType.EntityReference:
                    {
                        sb.Append(node.OuterXml);
                        return;
                    }
                case XmlNodeType.ProcessingInstruction:
                    {
                        sb.Append(string.Concat(new string?[] { "<?", node.Name, " ", node.Value, "?>" }));
                        return;
                    }
                case XmlNodeType.Comment:
                    {
                        var stringBuilder1 = sb;
                        var value = node.Value;
                        if (value != null)
                        {
                            str1 = value.Trim();
                        }
                        else
                        {
                            str1 = null!;
                        }

                        var strArrays = new string[]
                        {
                            new string(XmlFormatterConstants.Space, currentStartLength),
                            "<!--",
                            XmlFormatterConstants.Space.ToString(),
                            str1!,
                            XmlFormatterConstants.Space.ToString(),
                            "-->"
                        };
                        stringBuilder1.Append(string.Concat(strArrays));
                        return;
                    }
                case XmlNodeType.DocumentType:
                    {
                        space = XmlFormatterConstants.Space;
                        sb.Append(string.Concat(XmlFormatterConstants.DocTypeStart, space.ToString(), XmlFormatterConstants.DocTypeEnd(node.Value)));
                        return;
                    }
                case XmlNodeType.Whitespace:
                    {
                        return;
                    }
                case XmlNodeType.SignificantWhitespace:
                    {
                        return;
                    }
                default:
                    {
                        goto case XmlNodeType.XmlDeclaration;
                    }
            }
        }
    }
}
