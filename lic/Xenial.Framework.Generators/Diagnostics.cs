﻿using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;

namespace Xenial.Framework.Generators;

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

    internal static DiagnosticDescriptor ClassNeedsToBePartialWhenUsingAttribute(string attributeName) => new(
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

    internal static DiagnosticDescriptor ClassShouldBeInNamespaceWhenDerivingFrom(string classType) => new(
       "XENGEN0101",
       $"The class deriving from [{classType}] should be in a namespace",
       $"The class deriving from [{classType}] should be in a namespace",
       category,
       DiagnosticSeverity.Warning,
       isEnabledByDefault: true,
       description: $"The class deriving from [{classType}] should be in a namespace"
   );

    internal static DiagnosticDescriptor ClassShouldBePartialWhenDerivingFrom(string classType) => new(
        "XENGEN0102",
        $"The class deriving from [{classType}] should be partial",
        $"The class deriving from [{classType}] should be partial",
        category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: $"The class deriving from [{classType}] should be partial"
    );

    internal static DiagnosticDescriptor ClassShouldBePartial(string attributeName) => new(
        "XENGEN0102",
        $"The class using the [{attributeName}] should be partial",
        $"The class using the [{attributeName}] should be partial",
        category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: $"The class using the [{attributeName}] should be partial"
    );

    internal static DiagnosticDescriptor ConflictingAttributes(string attributeName, IEnumerable<string> conflictingAttributeProperties) => new(
        "XENGEN0200",
        $"When using the [{attributeName}] you shall not use conflicting properties, use only one of the following: {string.Join(" | ", conflictingAttributeProperties)}",
        $"When using the [{attributeName}] you shall not use conflicting properties, use only one of the following: {string.Join(" | ", conflictingAttributeProperties)}",
        category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"When using the [{attributeName}] you shall not use conflicting properties, use only one of the following: {string.Join(" | ", conflictingAttributeProperties)}"
    );

    internal static DiagnosticDescriptor ConflictingClasses(string attributeName, string conflictingClass) => new(
        "XENGEN0201",
        $"When using the [{attributeName}] you must make sure it's only used once per Action, check for multiple partial implementations of {conflictingClass}",
        $"When using the [{attributeName}] you must make sure it's only used once per Action, check for multiple partial implementations of {conflictingClass}",
        category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"When using the [{attributeName}] you must make sure it's only used once per Action, check for multiple partial implementations of {conflictingClass}"
    );

    internal static DiagnosticDescriptor ConflictingPartialImplementation(string methodName, string conflictingReturnType, string conflictingModifier) => new(
        "XENGEN0202",
        $"When implementing the partial method [{methodName}] you must make sure it returns [{conflictingReturnType}] and have the visibility [{conflictingModifier}]",
        $"When implementing the partial method [{methodName}] you must make sure it returns [{conflictingReturnType}] and have the visibility [{conflictingModifier}]",
        category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"When implementing the partial method [{methodName}] you must make sure it returns [{conflictingReturnType}] and have the visibility [{conflictingModifier}]"
    );
}