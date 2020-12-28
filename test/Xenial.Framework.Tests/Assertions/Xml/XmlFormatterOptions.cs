namespace Xenial.Framework.Tests.Assertions.Xml
{
    public record XmlFormatterOptions
    {
        public int IndentLength { get; set; } = 2;

        public bool UseSelfClosingTags { get; set; } = true;

        public bool UseSingleQuotes
        {
            get;
            set;
        }
    }
}
