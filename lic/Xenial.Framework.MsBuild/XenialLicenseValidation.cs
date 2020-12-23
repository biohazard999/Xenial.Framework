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
            "Fall back to trial mode",
            category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Make sure you made Xenial.PublicKey available" //TODO: Better messages and help
        );

        private static readonly DiagnosticDescriptor signitureIsInvalidRule = new(
            "XENLIC0011",
            "Xenial.Signature is invalid",
            "Fall back to trial mode",
            category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Make sure you made Xenial.PublicKey available" //TODO: Better messages and help
        );

        private static readonly DiagnosticDescriptor willBuildInTrialModeRule = new(
            "XENLIC0012",
            "Xenial will build in Trial mode",
            "Fall back to trial mode",
            category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Make sure you made Xenial.PublicKey available" //TODO: Better messages and help
        );

        public void Initialize(GeneratorInitializationContext context) { }
        public void Execute(GeneratorExecutionContext context)
        {
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }

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
                context.ReportDiagnostic(Diagnostic.Create(willBuildInTrialModeRule, Location.None));
            }

            var base64 = Base64Encode(xenialLicense);

            AddXenialLicence(context, base64);

            static string Base64Encode(string plainText)
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return System.Convert.ToBase64String(plainTextBytes);
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
            foreach (var xenialAssembly in context.Compilation.ReferencedAssemblyNames.Where(i => i.Name.StartsWith("Xenial.Framework")))
            {
                syntaxWriter.WriteLine($"{xenialAssembly.Name}.XenialLicenseCheck.LoadLicense(\"{license}\");");
            }
            syntaxWriter.CloseBrace();
            syntaxWriter.CloseBrace();
            syntaxWriter.CloseBrace();

            var syntax = syntaxWriter.ToString();
            var source = SourceText.From(syntax, Encoding.UTF8);
            context.AddSource($"XenialLicenseCheck{context.Compilation.AssemblyName}.g.cs", source);
        }
    }
}
