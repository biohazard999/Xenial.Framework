using System;
using System.CodeDom.Compiler;

namespace Xenial.Framework.MsBuild
{
    /// <summary>
    /// Takes care of opening and closing curly braces for code generation
    /// </summary>
    internal class CurlyIndenter
    {
        private readonly IndentedTextWriter indentedTextWriter;

        /// <summary>
        /// Default constructor that maked a tidies creation of the line before the opening curly
        /// </summary>
        /// <param name="indentedTextWriter">The writer to use</param>
        /// <param name="openingLine">any line to write before the curly</param>
        public CurlyIndenter(IndentedTextWriter indentedTextWriter)
            => this.indentedTextWriter = indentedTextWriter;

        public void Write(string val) => indentedTextWriter.Write(val);
        public void WriteLine(string val) => indentedTextWriter.WriteLine(val);
        public void WriteLine() => indentedTextWriter.WriteLine();
        public void Indent() => indentedTextWriter.Indent++;
        public void UnIndent() => indentedTextWriter.Indent--;

        public void OpenBrace()
        {
            WriteLine("{");
            Indent();
        }

        public void CloseBrace()
        {
            UnIndent();
            WriteLine("}");
        }

        public override string ToString() => indentedTextWriter.InnerWriter.ToString();
    }
}
