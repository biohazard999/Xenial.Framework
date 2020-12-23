using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using Newtonsoft.Json;

#nullable disable

namespace Xenial.Framework.MsBuild
{
    [Generator]
    public class XenialLicenseValidation : ISourceGenerator
    {
        private const string category = "Usage";

#pragma warning disable RS2008 // Enable analyzer release tracking
#pragma warning disable RS1033 // Define diagnostic description correctly
        private static readonly DiagnosticDescriptor cannotFindLicenseRule = new(
            "XENLIC0010",
            "Could not find Xenial.License or Xenial.PublicKey",
            "Could not find Xenial.License or Xenial.PublicKey, Build will be in TRIAL mode",
            category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Make sure you made Xenial.PublicKey available" //TODO: Better messages and help
        );

        private static readonly DiagnosticDescriptor signitureIsInvalidRule = new(
            "XENLIC0011",
            "Xenial.Signature is invalid",
            "The signature of the license is invalid, Build will be in TRIAL mode",
            category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Make sure you made Xenial.PublicKey available" //TODO: Better messages and help
        );

        private static readonly DiagnosticDescriptor willBuildInTrialModeRule = new(
            "XENLIC0012",
            "Xenial will build in Trial mode",
            "Current license is trial only, Build will be in TRIAL mode",
            category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Make sure you made Xenial.PublicKey available" //TODO: Better messages and help
        );

        private static readonly DiagnosticDescriptor couldNotFindLicenseNoCodeWillBeGeneratedRule = new(
            "XENLIC0013",
            "Could not find a license hence no code will be generated",
            "Could not find a license hence no code will be generated",
            category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Make sure you made Xenial.PublicKey available" //TODO: Better messages and help
        );

        public void Initialize(GeneratorInitializationContext context) { }
        public void Execute(GeneratorExecutionContext context)
        {
            //if (!Debugger.IsAttached)
            //{
            //    Debugger.Launch();
            //}

            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.CheckXenialLicense", out var checkLicenseStr)
                && bool.TryParse(checkLicenseStr, out var checkLicense)
                && !checkLicense)
            {
                return;
            }

            var xenialLicense = Environment.GetEnvironmentVariable("XENIAL_LICENSE");
            if (string.IsNullOrEmpty(xenialLicense))
            {
                var profileDirectory = GetProfileDirectory();
                var licPath = Path.Combine(profileDirectory, "License.xml");
                if (File.Exists(licPath))
                {
                    xenialLicense = File.ReadAllText(licPath);
                }
            }

            var xenialPublicKeys = Environment.GetEnvironmentVariable("XENIAL_LICENSE_PUBLIC_KEYS");
            if (string.IsNullOrEmpty(xenialPublicKeys))
            {
                var profileDirectory = GetProfileDirectory();
                var xenialPublicKeysPath = Path.Combine(profileDirectory, "License.PublicKeys.json");
                if (File.Exists(xenialPublicKeysPath))
                {
                    xenialPublicKeys = File.ReadAllText(xenialPublicKeysPath);
                }
            }

            var isTrial = true;

            if (string.IsNullOrEmpty(xenialLicense) || string.IsNullOrEmpty(xenialPublicKeys))
            {
                context.ReportDiagnostic(Diagnostic.Create(cannotFindLicenseRule, Location.None));
            }
            else
            {
                var license = Standard.Licensing.License.Load(xenialLicense);
                var publicKey = JsonConvert.DeserializeObject<Dictionary<string, string>>(xenialPublicKeys)["Xenial"];
                var isSignitureValid = license.VerifySignature(publicKey);
                if (!isSignitureValid)
                {
                    context.ReportDiagnostic(Diagnostic.Create(signitureIsInvalidRule, Location.None));
                }

                isTrial = license.Type == Standard.Licensing.LicenseType.Trial;
            }

            if (isTrial)
            {
                if (!string.IsNullOrEmpty(xenialLicense))
                {
                    context.ReportDiagnostic(Diagnostic.Create(willBuildInTrialModeRule, Location.None));
                }
            }

            if (!string.IsNullOrEmpty(xenialLicense))
            {
                var base64 = Base64Encode(xenialLicense);

                AddXenialLicence(context, base64);
            }
            else
            {
                context.ReportDiagnostic(Diagnostic.Create(couldNotFindLicenseNoCodeWillBeGeneratedRule, Location.None));
            }

            static string Base64Encode(string plainText)
            {
                var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(plainTextBytes);
            }

            static string GetProfileDirectory()
                => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".xenial");
        }

        private static void AddXenialLicence(GeneratorExecutionContext context, string license)
        {
            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.GenerateXenialLicense", out var generateLicenseStr)
                && bool.TryParse(generateLicenseStr, out var generateLicense)
                && !generateLicense)
            {
                return;
            }

            var syntaxWriter = new CurlyIndenter(new System.CodeDom.Compiler.IndentedTextWriter(new StringWriter()));
            syntaxWriter.WriteLine("using System;");
            syntaxWriter.WriteLine("using System.Runtime.CompilerServices;");
            syntaxWriter.WriteLine();
            syntaxWriter.WriteLine("namespace Xenial");
            syntaxWriter.OpenBrace();

            syntaxWriter.WriteLine("[CompilerGenerated]");
            syntaxWriter.WriteLine("internal static class XenialLicense");
            syntaxWriter.OpenBrace();
            syntaxWriter.WriteLine("internal static void Register()");
            syntaxWriter.OpenBrace();
            syntaxWriter.WriteLine("System.Console.WriteLine(\"INIT XENIAL LICENSE\")");
            foreach (var xenialAssembly in context.Compilation.ReferencedAssemblyNames.Where(i => i.Name.StartsWith("Xenial.Framework")))
            {
                syntaxWriter.WriteLine($"{xenialAssembly.Name}.XenialLicenseCheck.LoadLicense(\"{license}\");");
            }
            syntaxWriter.CloseBrace();
            syntaxWriter.CloseBrace();
            syntaxWriter.CloseBrace();

            syntaxWriter.WriteLine();
            syntaxWriter.WriteLine("#if _INJECT_XENIAL_MODULE_INIT");
            syntaxWriter.WriteLine();
            syntaxWriter.WriteLine("namespace System.Runtime.CompilerServices");
            syntaxWriter.OpenBrace();

            syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Method, Inherited = false)]");
            syntaxWriter.WriteLine("internal sealed class ModuleInitializerAttribute : Attribute");
            syntaxWriter.OpenBrace();
            syntaxWriter.WriteLine("public ModuleInitializerAttribute() { }");
            syntaxWriter.CloseBrace();

            syntaxWriter.CloseBrace();
            syntaxWriter.WriteLine();
            syntaxWriter.WriteLine("#endif");

            var syntax = syntaxWriter.ToString();
            var source = SourceText.From(syntax, Encoding.UTF8);
            context.AddSource($"XenialLicenseCheck{context.Compilation.AssemblyName}.g.cs", source);
        }
    }
}
