using System;
using System.IO;
using System.Text;

namespace Acme.Module.Helpers.Xml
{
    /// <summary>   A string writer with encoding. This class cannot be inherited. </summary>
    ///
    /// <seealso cref="StringWriter"/>

    public sealed class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding encoding;

        /// <summary>
        /// Gets the <see cref="System.Text.Encoding" /> in which the output is written.
        /// </summary>
        ///
        /// <value> The <see langword="Encoding" /> in which the output is written. </value>
        ///
        /// <seealso cref="System.IO.StringWriter.Encoding"/>

        public override Encoding Encoding => encoding;

        /// <summary>   Default constructor. </summary>
        public StringWriterWithEncoding() : this(Encoding.Default) { }

        /// <summary>   Constructor. </summary>
        ///
        /// <param name="encoding"> The encoding. </param>

        public StringWriterWithEncoding(Encoding encoding) => this.encoding = encoding;
    }
}
