using System;
using System.CodeDom.Compiler;
using System.IO;

namespace Xenial.Framework.MsBuild;

/// <summary>
/// Takes care of opening and closing curly braces for code generation
/// </summary>
internal class CurlyIndenter
{
    private readonly IndentedTextWriter indentedTextWriter;

    internal static CurlyIndenter Create()
        => new CurlyIndenter(new IndentedTextWriter(new StringWriter()));

    /// <summary>
    /// Default constructor that maked a tidies creation of the line before the opening curly
    /// </summary>
    /// <param name="indentedTextWriter">The writer to use</param>
    private CurlyIndenter(IndentedTextWriter indentedTextWriter)
        => this.indentedTextWriter = indentedTextWriter;

    internal void Write(string val) => indentedTextWriter.Write(val);
    internal void WriteLine(string val) => indentedTextWriter.WriteLine(val);
    internal void WriteLine() => indentedTextWriter.WriteLine();
    internal void Indent() => indentedTextWriter.Indent++;
    internal void UnIndent() => indentedTextWriter.Indent--;

    internal record DisposableContext(CurlyIndenter Indenter, string? EndValue = null, string CloseBrace = "}", bool WriteLine = true) : IDisposable
    {
        void IDisposable.Dispose()
            => Indenter.CloseBrace(EndValue, CloseBrace, WriteLine);
    }

    internal IDisposable OpenBrace(string val, string? endValue = null, string openBrace = "{", string closeBrace = "}", bool writeLine = true)
    {
        WriteLine(val);
        if (writeLine)
        {
            WriteLine(openBrace);
        }
        Indent();
        return new DisposableContext(this, endValue, closeBrace, writeLine);
    }

    internal IDisposable OpenBrace()
    {
        WriteLine("{");
        Indent();
        return new DisposableContext(this);
    }

    internal void CloseBrace(string? endValue = null, string closeBrace = "}", bool writeLine = true)
    {
        UnIndent();
        if (string.IsNullOrEmpty(endValue))
        {
            if (writeLine)
            {
                WriteLine(closeBrace);
            }
            else
            {
                Write(closeBrace);
            }
        }
        else
        {
            if (writeLine)
            {
                Write(closeBrace);
                Write(endValue);
                WriteLine();
            }
            else
            {
                Write(closeBrace);
                Write(endValue);
            }
        }
    }

    public override string ToString() => indentedTextWriter.InnerWriter.ToString();
}
