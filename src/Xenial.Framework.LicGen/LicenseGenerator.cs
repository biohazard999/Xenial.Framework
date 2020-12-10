using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Xenial.Framework.LicGen
{
    [Generator]
    public class LicenseGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            AddPublicKeyTokenAttribute(context);
//#if DEBUG
//            while (!Debugger.IsAttached)
//            {
//                Debugger.Launch();
//            }
//#endif
            AddCheckLicenceAttribute(context);


            if (context.SyntaxReceiver is SyntaxReceiver syntaxReceiver)
            {
                foreach (var canidate in syntaxReceiver.Canidates)
                {
                    if (context.Compilation is CSharpCompilation csharpCompilation)
                    {

                        var model = context.Compilation.GetSemanticModel(canidate.SyntaxTree);
                        var symbol = model.GetSymbolInfo(canidate);
                        if (symbol.Symbol != null)
                        {
                            var attributes = context.Compilation.Assembly.GetAttributes();
                            //var attributes = symbol.Symbol.GetAttributes();
                        }

                        // if (!Debugger.IsAttached)
                        // {
                        //     Debugger.Launch();
                        // }
                    }
                }
            }
        }

        private static void AddPublicKeyTokenAttribute(GeneratorExecutionContext context)
        {
            var syntaxWriter = new CurlyIndenter(new System.CodeDom.Compiler.IndentedTextWriter(new StringWriter()));
            syntaxWriter.WriteLine("using System;");
            syntaxWriter.WriteLine("using System.Runtime.CompilerServices;");
            syntaxWriter.WriteLine();
            syntaxWriter.WriteLine("namespace Xenial");
            syntaxWriter.OpenBrace();
            syntaxWriter.WriteLine();
            syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Assembly)]");
            syntaxWriter.WriteLine("[CompilerGenerated]");
            syntaxWriter.WriteLine("internal class XenialPublicKeyAttribute : Attribute");
            syntaxWriter.OpenBrace();
            syntaxWriter.WriteLine("public string PublicKey { get; }");
            syntaxWriter.WriteLine("public XenialPublicKeyAttribute(string publicKey)");
            syntaxWriter.Indent();
            syntaxWriter.WriteLine("=> PublicKey = publicKey;");
            syntaxWriter.UnIndent();
            syntaxWriter.UnIndent();

            syntaxWriter.CloseBrace();
            syntaxWriter.CloseBrace();


            var syntax = syntaxWriter.ToString();
            var source = Microsoft.CodeAnalysis.Text.SourceText.From(syntax, Encoding.UTF8);
            context.AddSource("XenialPublicKeyAttribute.g.cs", source);
        }

        private static void AddCheckLicenceAttribute(GeneratorExecutionContext context)
        {
            var syntaxWriter = new CurlyIndenter(new System.CodeDom.Compiler.IndentedTextWriter(new StringWriter()));
            syntaxWriter.WriteLine("using System;");
            syntaxWriter.WriteLine("using System.Runtime.CompilerServices;");
            syntaxWriter.WriteLine();
            syntaxWriter.WriteLine("namespace Xenial");
            syntaxWriter.OpenBrace();
            syntaxWriter.WriteLine();
            syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Class)]");
            syntaxWriter.WriteLine("[CompilerGenerated]");
            syntaxWriter.WriteLine("internal class XenialCheckLicenceAttribute : Attribute");
            syntaxWriter.OpenBrace();
            syntaxWriter.WriteLine("public XenialCheckLicenceAttribute() { }");
            syntaxWriter.UnIndent();

            syntaxWriter.CloseBrace();
            syntaxWriter.CloseBrace();


            var syntax = syntaxWriter.ToString();
            var source = Microsoft.CodeAnalysis.Text.SourceText.From(syntax, Encoding.UTF8);
            context.AddSource("XenialCheckLicenceAttribute.g.cs", source);
        }

        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

        private class SyntaxReceiver : ISyntaxReceiver
        {
            public List<AttributeSyntax> Canidates { get; } = new List<AttributeSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                // We grab all assembly level attributes
                if (syntaxNode is CompilationUnitSyntax cus)
                {
                    foreach (var attributeList in cus.AttributeLists)
                    {
                        if (attributeList.Attributes.Count > 0
                            && attributeList.Target is AttributeTargetSpecifierSyntax atss
                            && atss.Identifier.IsKind(SyntaxKind.AssemblyKeyword)
                        )
                        {
                            foreach (var attribute in attributeList.Attributes)
                            {
                                Canidates.Add(attribute);
                            }
                        }
                    }
                }
            }
        }
    }
}
