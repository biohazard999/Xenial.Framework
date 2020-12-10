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
    //[CompilerGenerated]
    //[AttributeUsage(AttributeTargets.Assembly)]
    //public class XenialPublicKeyAttribute : Attribute
    //{
    //    public string PublicKey { get; }
    //    public XenialPublicKeyAttribute(string publicKey)
    //    {

    //    }
    //}
    [Generator]
    public class LicenseGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var publicKeyAttributeSyntaxWriter = new CurlyIndenter(new System.CodeDom.Compiler.IndentedTextWriter(new StringWriter()));
            // #if DEBUG
            //             if (!Debugger.IsAttached)
            //             {
            //                 Debugger.Launch();
            //             }
            // #endif

            publicKeyAttributeSyntaxWriter.WriteLine("using System;");
            publicKeyAttributeSyntaxWriter.WriteLine("using System.Runtime.CompilerServices;");
            publicKeyAttributeSyntaxWriter.WriteLine();
            publicKeyAttributeSyntaxWriter.WriteLine("namespace Xenial;");
            publicKeyAttributeSyntaxWriter.OpenBrace();
            publicKeyAttributeSyntaxWriter.WriteLine();
            publicKeyAttributeSyntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Assembly)]");
            publicKeyAttributeSyntaxWriter.WriteLine("[CompilerGenerated]");
            publicKeyAttributeSyntaxWriter.WriteLine("public class XenialPublicKeyAttribute : Attribute");
            publicKeyAttributeSyntaxWriter.OpenBrace();
            publicKeyAttributeSyntaxWriter.WriteLine("public string PublicKey { get; }");
            publicKeyAttributeSyntaxWriter.WriteLine("public XenialPublicKeyAttribute(string publicKey)");
            publicKeyAttributeSyntaxWriter.Indent();
            publicKeyAttributeSyntaxWriter.WriteLine("=> PublicKey = publicKey;");
            publicKeyAttributeSyntaxWriter.UnIndent();

            publicKeyAttributeSyntaxWriter.CloseBrace();
            publicKeyAttributeSyntaxWriter.CloseBrace();

#if DEBUG
            while (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif

            var syntax = publicKeyAttributeSyntaxWriter.ToString();

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
