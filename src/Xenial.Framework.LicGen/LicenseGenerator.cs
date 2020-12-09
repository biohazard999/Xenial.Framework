using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xenial.Framework.LicGen
{
    [Generator]
    public class LicenseGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            _ = new CurlyIndenter(new System.CodeDom.Compiler.IndentedTextWriter(new StringWriter()));
            // #if DEBUG
            //             if (!Debugger.IsAttached)
            //             {
            //                 Debugger.Launch();
            //             }
            // #endif
            if (context.SyntaxReceiver is SyntaxReceiver syntaxReceiver)
            {
                foreach (var _ in syntaxReceiver.Canidates)
                {

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
