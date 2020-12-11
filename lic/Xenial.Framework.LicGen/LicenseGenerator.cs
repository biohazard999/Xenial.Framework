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
using Microsoft.CodeAnalysis.Text;

namespace Xenial.Framework.LicGen
{
    [Generator]
    public class LicenseGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            AddPublicKeyTokenAttribute(context);
            AddCheckLicenceAttribute(context);
            AddLicenseCheck(context);

            //if (context.SyntaxReceiver is SyntaxReceiver syntaxReceiver)
            //{
            //    foreach (var canidate in syntaxReceiver.Canidates)
            //    {
            //        if (context.Compilation is CSharpCompilation csharpCompilation)
            //        {

            //            var model = context.Compilation.GetSemanticModel(canidate.SyntaxTree);
            //            var symbol = model.GetSymbolInfo(canidate);
            //            if (symbol.Symbol != null)
            //            {
            //                var attributes = context.Compilation.Assembly.GetAttributes();
            //                //var attributes = symbol.Symbol.GetAttributes();
            //            }
            //        }
            //    }
            //}
        }

        private static void AddPublicKeyTokenAttribute(GeneratorExecutionContext context)
        {
            var syntaxWriter = new CurlyIndenter(new System.CodeDom.Compiler.IndentedTextWriter(new StringWriter()));
            syntaxWriter.WriteLine("using System;");
            syntaxWriter.WriteLine("using System.Runtime.CompilerServices;");
            syntaxWriter.WriteLine();
            syntaxWriter.WriteLine("namespace Xenial");
            syntaxWriter.OpenBrace();
            syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Assembly)]");
            syntaxWriter.WriteLine("[CompilerGenerated]");
            syntaxWriter.WriteLine("internal class XenialPublicKeyAttribute : Attribute");
            syntaxWriter.OpenBrace();
            syntaxWriter.WriteLine("public string PublicKey { get; }");
            syntaxWriter.WriteLine("public XenialPublicKeyAttribute(string publicKey)");
            syntaxWriter.Indent();
            syntaxWriter.WriteLine("=> PublicKey = publicKey;");
            syntaxWriter.UnIndent();

            syntaxWriter.CloseBrace();
            syntaxWriter.CloseBrace();


            var syntax = syntaxWriter.ToString();
            var source = SourceText.From(syntax, Encoding.UTF8);
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
            syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Class)]");
            syntaxWriter.WriteLine("[CompilerGenerated]");
            syntaxWriter.WriteLine("internal class XenialCheckLicenceAttribute : Attribute");
            syntaxWriter.OpenBrace();
            syntaxWriter.WriteLine("public XenialCheckLicenceAttribute() { }");

            syntaxWriter.CloseBrace();
            syntaxWriter.CloseBrace();

            var syntax = syntaxWriter.ToString();
            var source = SourceText.From(syntax, Encoding.UTF8);
            context.AddSource("XenialCheckLicenceAttribute.g.cs", source);
        }

        private void AddLicenseCheck(GeneratorExecutionContext context)
        {
            var xenialPublicKey = GetXenialPublicKey(context);
            var xenialProduct = GetXenialProduct(context);

            var manifestResourceStreamName = $"{GetType().Assembly.GetName().Name}.XenialLicenseCheck.template.cs";
            var checkStream = GetType().Assembly.GetManifestResourceStream(manifestResourceStreamName);
            var reader = new StreamReader(checkStream);
            var checkTemplate = reader.ReadToEnd();
            var syntax = checkTemplate
                .Replace("__NAMESPACE__", context.Compilation.AssemblyName)
                .Replace("%ProductName%", xenialProduct)
                .Replace("%PulicKeyToken%", xenialPublicKey);

            var source = SourceText.From(syntax, Encoding.UTF8);
            context.AddSource("XenialLicenseCheck.g.cs", source);
        }

        private const string category = "Usage";

#pragma warning disable RS2008 // Enable analyzer release tracking
#pragma warning disable RS1033 // Define diagnostic description correctly
        private static readonly DiagnosticDescriptor cannotFindPublicKeyRule = new(
            "XENLIC0001",
            "Cannot find XenialPublicKey",
            "Make sure you made XenialPublicKey visible to the compiler",
            category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Make sure you made XenialPublicKey visible to the compiler"
        );

        private static readonly DiagnosticDescriptor cannotFindProductRule = new(
            "XENLIC0002",
            "Cannot find PackageId",
            "Make sure you made PackageId visible to the compiler",
            category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Make sure you made PackageId visible to the compiler"
        );

        private static readonly DiagnosticDescriptor cannotFindXenialModule = new(
            "XENLIC0003",
            "Cannot find XenialModule",
            "Make sure you made XenialModule visible to the compiler",
            category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Make sure you made XenialModule visible to the compiler"
        );

        private static string GetXenialPublicKey(GeneratorExecutionContext context)
        {
            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.XenialPublicKey", out var xenialPublicKey))
            {
                if (string.IsNullOrEmpty(xenialPublicKey))
                {
                    context.ReportDiagnostic(Diagnostic.Create(cannotFindPublicKeyRule, Location.None));
                }
                return xenialPublicKey ?? string.Empty;
            }
            context.ReportDiagnostic(Diagnostic.Create(cannotFindPublicKeyRule, Location.None));
            return string.Empty;
        }

        private static string GetXenialProduct(GeneratorExecutionContext context)
        {
            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.PackageId", out var xenialProduct))
            {
                if (string.IsNullOrEmpty(xenialProduct))
                {
                    context.ReportDiagnostic(Diagnostic.Create(cannotFindProductRule, Location.None));
                }
                return xenialProduct ?? string.Empty;
            }
            context.ReportDiagnostic(Diagnostic.Create(cannotFindProductRule, Location.None));
            return string.Empty;
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
