using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;

namespace Xenial.Framework.Generators
{
    internal static class GeneratorDiagnostics
    {
        private const string category = "Usage";

#pragma warning disable RS2008 // Enable analyzer release tracking
#pragma warning disable RS1033 // Define diagnostic description correctly
        internal static DiagnosticDescriptor InvalidBooleanMsBuildProperty(string msBuildPropertyName, string actualValue) => new(
            "XENGEN0010",
            $"Could not parse boolean MSBUILD variable ({msBuildPropertyName})",
            $"Could not parse boolean MSBUILD variable ({msBuildPropertyName}), make sure it's in a boolean format. Actual value: {actualValue}",
            category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"Could not parse boolean MSBUILD variable ({msBuildPropertyName}), make sure it's in a boolean format. Actual value: {actualValue}"
        );

        internal static DiagnosticDescriptor ClassNeedsToBePartial(string attributeName) => new(
            "XENGEN0100",
            $"The class using the [{attributeName}] needs to be partial",
            $"The class using the [{attributeName}] needs to be partial",
            category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"The class using the [{attributeName}] needs to be partial"
        );

        internal static DiagnosticDescriptor ClassNeedsToBeInNamespace(string attributeName) => new(
            "XENGEN0101",
            $"The class using the [{attributeName}] needs to be in a namespace",
            $"The class using the [{attributeName}] needs to be in a namespace",
            category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"The class using the [{attributeName}] needs to be namespace"
        );
    }
}
