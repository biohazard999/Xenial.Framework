using System;
using System.CodeDom.Compiler;
using System.IO;

namespace Xenial.Framework.MsBuild;

/// <summary>
/// Takes care of opening and closing curly braces for code generation
/// </summary>
public class CurlyIndenter
{
    internal static CurlyIndenter Create()
        => new CurlyIndenter(new IndentedTextWriter(new StringWriter()));

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

    internal record DisposableContext(CurlyIndenter Indenter, string? EndValue = null) : IDisposable
    {
        void IDisposable.Dispose()
            => Indenter.CloseBrace(EndValue);
    }

    public IDisposable OpenBrace(string val, string? endValue = null)
    {
        WriteLine(val);
        WriteLine("{");
        Indent();
        return new DisposableContext(this, endValue);
    }

    public IDisposable OpenBrace()
    {
        WriteLine("{");
        Indent();
        return new DisposableContext(this);
    }

    public void CloseBrace(string? endValue = null)
    {
        UnIndent();
        if (string.IsNullOrEmpty(endValue))
        {
            WriteLine("}");
        }
        else
        {
            Write("}");
            Write(endValue);
            WriteLine();
        }
    }

    public override string ToString() => indentedTextWriter.InnerWriter.ToString();
}
