﻿using System;

namespace Xenial.Framework.Tests.Assertions.Xml
{
    public static class XmlFormatterConstants
    {
        public const string StartTagStart = "<";

        public const string StartTagEnd = ">";

        public const string EndTagStart = "</";

        public const string EndTagEnd = ">";

        public const string InlineEndTag = "/>";

        public static readonly char Space;

        public const string CommentTagStart = "<!--";

        public const string CommentTagEnd = "-->";

        public const string AssignmentStart = "=\"";

        public const string AssignmentEnd = "\"";

        public const string AssignmentStartSingleQuote = "='";

        public const string AssignmentEndSingleQuote = "'";

        public const string CDataStart = "<![CDATA[";

        public const string CDataEnd = "]]>";

        public const string DocTypeStart = "<!DOCTYPE";

        public static readonly string Newline;

        static XmlFormatterConstants()
        {
            Space = ' ';
            Newline = Environment.NewLine;
        }

        public static string DocTypeEnd(string? val)
            => $"[{val}]";
    }
}
