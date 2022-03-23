using System;

namespace Xenial.Cli.Xml
{
    /// <summary>   An XML formatter constants. </summary>
    public static class XmlFormatterConstants
    {
        /// <summary>   (Immutable) the start tag start. </summary>
        public const string StartTagStart = "<";

        /// <summary>   (Immutable) the start tag end. </summary>
        public const string StartTagEnd = ">";

        /// <summary>   (Immutable) the end tag start. </summary>
        public const string EndTagStart = "</";

        /// <summary>   (Immutable) the end tag end. </summary>
        public const string EndTagEnd = ">";

        /// <summary>   (Immutable) the inline end tag. </summary>
        public const string InlineEndTag = "/>";

        /// <summary>   (Immutable) the space. </summary>
        public static readonly char Space = ' ';

        /// <summary>   (Immutable) the comment tag start. </summary>
        public const string CommentTagStart = "<!--";

        /// <summary>   (Immutable) the comment tag end. </summary>
        public const string CommentTagEnd = "-->";

        /// <summary>   (Immutable) . </summary>
        public const string AssignmentStart = "=\"";

        /// <summary>   (Immutable) the assignment end. </summary>
        public const string AssignmentEnd = "\"";

        /// <summary>   (Immutable) . </summary>
        public const string AssignmentStartSingleQuote = "='";

        /// <summary>   (Immutable) the assignment end single quote. </summary>
        public const string AssignmentEndSingleQuote = "'";

        /// <summary>   (Immutable) the data start. </summary>
        public const string CDataStart = "<![CDATA[";

        /// <summary>   (Immutable) the data end. </summary>
        public const string CDataEnd = "]]>";

        /// <summary>   (Immutable) the document type start. </summary>
        public const string DocTypeStart = "<!DOCTYPE";

        /// <summary>   (Immutable) the newline. </summary>
        public static readonly string Newline = Environment.NewLine;

        /// <summary>   Document type end. </summary>
        ///
        /// <param name="val">  The value. </param>
        ///
        /// <returns>   A string. </returns>

        public static string DocTypeEnd(string? val)
            => $"[{val}]";
    }
}
