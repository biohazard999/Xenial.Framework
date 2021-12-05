using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators.Internal.Generators;

public abstract class XenialBaseGenerator
{
    public IDictionary<string, string> ConstantsToInject { get; }

    protected XenialBaseGenerator(IDictionary<string, string>? constantsToInject)
    {
        _ = constantsToInject ?? throw new ArgumentNullException(nameof(constantsToInject));
        ConstantsToInject = constantsToInject;
    }

    protected const string XenialNamespace = "Xenial";

    protected static Compilation AddSource(
        GeneratorExecutionContext context,
        Compilation compilation,
        CurlyIndenter builder,
        string fileName,
        CSharpParseOptions? parseOptions = null)
    {
        try
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            _ = compilation ?? throw new ArgumentNullException(nameof(compilation));

            parseOptions = parseOptions ?? (CSharpParseOptions)context.ParseOptions;
            var syntax = builder.ToString().Replace("{visibility}", "internal");
            var source = SourceText.From(syntax, Encoding.UTF8);
            var syntaxTree = CSharpSyntaxTree.ParseText(syntax, parseOptions, cancellationToken: context.CancellationToken);

            context.AddSource($"{fileName}.g.cs", source);
            return compilation.AddSyntaxTrees(syntaxTree);
        }
        catch (ArgumentException ex)
        {
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
            Console.WriteLine(ex.ToString());
            throw;
        }
    }
}
