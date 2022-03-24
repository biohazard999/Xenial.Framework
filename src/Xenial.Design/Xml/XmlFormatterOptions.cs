namespace Xenial.Cli.Xml
{
    /// <summary>   (Immutable) an XML formatter options. </summary>
    public record XmlFormatterOptions
    {
        /// <summary>   Gets or sets the indent length. </summary>
        ///
        /// <value> The length of the indent. </value>

        public int IndentLength { get; set; } = 2;

        /// <summary>   Gets or sets a value indicating whether this object use self closing tags. </summary>
        ///
        /// <value> True if use self closing tags, false if not. </value>

        public bool UseSelfClosingTags { get; set; } = true;

        /// <summary>   Gets or sets a value indicating whether this object use single quotes. </summary>
        ///
        /// <value> True if use single quotes, false if not. </value>

        public bool UseSingleQuotes
        {
            get;
            set;
        }
    }
}
