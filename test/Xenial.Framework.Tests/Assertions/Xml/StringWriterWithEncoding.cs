using System;
using System.IO;
using System.Text;

namespace Xenial.Framework.Tests.Assertions.Xml
{
    public sealed class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding encoding;

        public override Encoding Encoding => encoding;

        public StringWriterWithEncoding() : this(Encoding.Default) { }

        public StringWriterWithEncoding(Encoding encoding) => this.encoding = encoding;
    }
}
