﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Xenial.Framework.Generators.Attributes;
using Xenial.Framework.Generators.Base;
using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators.Partial;

public record XenialActionGeneratorOutputOptions(
    bool Attribute = true,
    bool PartialBuddy = true,
    bool Controller = true
);

public record XenialActionGenerator(XenialActionGeneratorOutputOptions OutputOptions, bool AddSource = true)
     : XenialPartialGenerator(AddSource)
{
    public static XenialActionAttributeGenerator AttributeGenerator { get; } = new XenialActionAttributeGenerator(false);

    public override IEnumerable<XenialAttributeGenerator> DependsOnGenerators => new[]
    {
        AttributeGenerator
    };

    public override bool Accepts(TypeDeclarationSyntax typeDeclarationSyntax) => false;

    private record XenialMethodGeneratorContext(
        MethodDeclarationSyntax Syntax,
        IMethodSymbol Symbol,
        TypeInfo ReturnTypeInfo
    )
    {
        public static XenialMethodGeneratorContext? CreateContext(GeneratorExecutionContext context, SemanticModel semanticModel, MethodDeclarationSyntax? syntax)
        {
            if (syntax is null)
            {
                return null;
            }

            var method = semanticModel.GetDeclaredSymbol(syntax, context.CancellationToken);

            if (method is null)
            {
                return null;
            }

            var returnTypeInfo = semanticModel.GetTypeInfo(syntax.ReturnType, context.CancellationToken);

            return new(syntax, method, returnTypeInfo);
        }

        public SyntaxTokenList PartialModifiers => new SyntaxTokenList(
            Syntax.Modifiers.Where(t => !t.IsKind(SyntaxKind.AsyncKeyword))
        );
    }

    private record XenialActionGeneratorContext(
        INamespaceSymbol Namespace,
        TypeDeclarationSyntax Class,
        INamedTypeSymbol ClassSymbol,
        AttributeData ActionAttribute,
        ITypeSymbol? TargetType,
        IMethodSymbol? Constructor,
        XenialMethodGeneratorContext? Executor
    )
    {
        public bool IsPartial => Class.HasModifier(SyntaxKind.PartialKeyword);
        public bool IsGlobalNamespace => ClassSymbol.ContainingNamespace.ToString() == "<global namespace>";
        public bool HasConflictingCategoryAttributes => ActionAttribute.HasAttribute("Category") && ActionAttribute.HasAttribute("PredefinedCategory");
        public bool HasConflictingTargetViewIdAttributes => ActionAttribute.HasAttribute("TargetViewId") && ActionAttribute.HasAttribute("TargetViewIds");
        public bool HasConflictingAttributes => HasConflictingCategoryAttributes || HasConflictingTargetViewIdAttributes;
        public bool HasError => IsGlobalNamespace || !IsPartial || HasConflictingAttributes;

        public Compilation ReportClassNeedsToBeInNamespaceDiagnostic(GeneratorExecutionContext context, Compilation compilation)
        {
            if (IsGlobalNamespace)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.ClassNeedsToBeInNamespace(
                        AttributeGenerator.AttributeName
                    ), Class.GetLocation())
                );

            }
            return compilation;
        }

        public Compilation ReportClassShouldBePartialDiagnostic(GeneratorExecutionContext context, Compilation compilation)
        {
            if (!IsPartial)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.ClassShouldBePartial(
                        AttributeGenerator.AttributeName
                    ), Class.GetLocation())
                );

            }
            return compilation;
        }

        public Compilation ReportAttributesShouldNotConflictDiagnostic(GeneratorExecutionContext context, Compilation compilation)
        {
            if (HasConflictingCategoryAttributes)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.ConflictingAttributes(
                        AttributeGenerator.AttributeName,
                        new[] { "Category", "PredefinedCategory" }
                    ), Class.GetLocation())
                );
            }
            if (HasConflictingTargetViewIdAttributes)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.ConflictingAttributes(
                        AttributeGenerator.AttributeName,
                        new[] { "TargetViewId", "TargetViewIds" }
                    ), Class.GetLocation())
                );
            }
            return compilation;
        }
    }

    public override Compilation Execute(
        GeneratorExecutionContext context,
        Compilation compilation,
        IList<TypeDeclarationSyntax> types,
        IList<string> addedSourceFiles
    )
    {
        _ = compilation ?? throw new ArgumentNullException(nameof(compilation));
        _ = types ?? throw new ArgumentNullException(nameof(types));
        _ = addedSourceFiles ?? throw new ArgumentNullException(nameof(addedSourceFiles));

        if (!FindAttributeFromGenerator(compilation, AttributeGenerator, out compilation))
        {
            return compilation;
        }

        var attribute = GetAttributeFromGenerator(compilation, AttributeGenerator);

        var collectedContexts = new List<XenialActionGeneratorContext>();

        foreach (var @class in types)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            if (TryGetTargetWithAttribute(context, compilation, @class, attribute, out var targetSymbol))
            {
                if (IsInGlobalNamespace(context, compilation, targetSymbol.Symbol, attribute.Name, @class.GetLocation(), out compilation))
                {
                    continue;
                }

                var attrib = GetXenialActionAttribute(targetSymbol.Symbol, attribute);

                ITypeSymbol? GetTargetType()
                {
                    var detailViewActionInterface = targetSymbol.Symbol.AllInterfaces
                        .FirstOrDefault(i => i.OriginalDefinition.ToDisplayString() == "Xenial.IDetailViewAction<T>");

                    if (detailViewActionInterface is not null)
                    {
                        if (detailViewActionInterface.IsGenericType)
                        {
                            var targetType = detailViewActionInterface.TypeArguments.First();

                            return targetType;
                        }
                    }

                    return null;
                }

                var possibleCtors = targetSymbol.Symbol.InstanceConstructors
                    //record copy constructor are implicitly declared
                    .Where(ctor => !ctor.IsImplicitlyDeclared)
                    .GroupBy(ctor => ctor.Parameters.Length)
                    .Select(g => (lenght: g.Key, ctors: g.ToArray()))
                    .OrderByDescending(g => g.lenght)
                    .Select(g => g.ctors)
                    .FirstOrDefault();

                //TODO: error on conflicting ctor count
                var possibleCtor = possibleCtors?.FirstOrDefault();

                var executorMethod = @class.Members
                    .OfType<MethodDeclarationSyntax>()
                    .Where(method => method.Identifier.Text == "Execute")
                    .FirstOrDefault();

                var actionContext = new XenialActionGeneratorContext(
                    targetSymbol.Symbol.ContainingNamespace,
                    @class,
                    targetSymbol.Symbol,
                    attrib,
                    GetTargetType(),
                    possibleCtor,
                    XenialMethodGeneratorContext.CreateContext(context, compilation.GetSemanticModel(@class.SyntaxTree), executorMethod)
                );

                collectedContexts.Add(actionContext);
            }
        }

        foreach (var actionContext in collectedContexts)
        {
            compilation = actionContext.ReportClassNeedsToBeInNamespaceDiagnostic(context, compilation);
            compilation = actionContext.ReportClassShouldBePartialDiagnostic(context, compilation);
            compilation = actionContext.ReportAttributesShouldNotConflictDiagnostic(context, compilation);

            if (actionContext.HasError) { continue; }

            var builder = CurlyIndenter.Create();

            static void WriteBasicImports(CurlyIndenter builder)
            {
                builder.WriteLine("// <auto-generated />");
                builder.WriteLine();
                builder.WriteLine("using System;");
                builder.WriteLine("using System.IO;");
                builder.WriteLine("using System.Runtime.CompilerServices;");
                builder.WriteLine();
            }

            WriteBasicImports(builder);

            using (builder.OpenBrace($"namespace {actionContext.ClassSymbol.ContainingNamespace}"))
            {
                builder.WriteLine("[CompilerGenerated]");
                using (builder.OpenBrace($"partial {(actionContext.ClassSymbol.IsRecord ? "record" : "class")} {actionContext.ClassSymbol.Name}"))
                {
                    GeneratePartialMethodDeclaration(builder, actionContext);
                }

                builder.WriteLine();

            }

            compilation = AddGeneratedCode(
                context,
                compilation,
                actionContext.Class,
                builder,
                addedSourceFiles,
                emitFile: OutputOptions.PartialBuddy
            );

            builder = CurlyIndenter.Create();

            WriteBasicImports(builder);

            string? controllerName = null;
            using (builder.OpenBrace($"namespace {actionContext.ClassSymbol.ContainingNamespace}"))
            {
                controllerName = GenerateController(context, compilation, builder, actionContext, types);
            }

            compilation = AddGeneratedCode(
                context,
                compilation,
                actionContext.Class,
                builder,
                addedSourceFiles,
                controllerName,
                OutputOptions.Controller
            );
        }

        return compilation;
    }

    private static void GeneratePartialMethodDeclaration(CurlyIndenter builder, XenialActionGeneratorContext actionContext)
    {
        if (actionContext.Executor is null)
        {
            if (actionContext.TargetType is not null)
            {
                builder.WriteLine($"partial void Execute({actionContext.TargetType.ToDisplayString()} targetObject);");
            }
            else
            {
                builder.WriteLine($"partial void Execute(object targetObject);");
            }
        }
        else if (actionContext.Executor.ReturnTypeInfo.Type is not null)
        {
            var parameterString = string.Join(", ", actionContext.Executor.Symbol.Parameters.Select(p => $"{p} {p.Name}"));

            builder.WriteLine($"{actionContext.Executor.PartialModifiers} {actionContext.Executor.ReturnTypeInfo.Type.ToDisplayString()} Execute({parameterString});");
        }
        else
        {
            //builder.WriteLine($"{actionContext.Executor.PartialModifiers} {actionContext.Executor.ReturnTypeInfo.Type.ToDisplayString()} Execute({parameterString});");
        }
    }


    private static string GenerateController(
        GeneratorExecutionContext context,
        Compilation compilation,
        CurlyIndenter builder,
        XenialActionGeneratorContext actionContext,
        IList<TypeDeclarationSyntax> types
    )
    {
        var actionId = $"{actionContext.Namespace}.{actionContext.ClassSymbol.Name}SimpleAction";
        var actionName = $"{actionContext.ClassSymbol.Name}SimpleAction";
        var controllerName = $"{actionContext.ClassSymbol.Name}Controller";
        var controllerFullName = $"{actionContext.Namespace}.{controllerName}";

        static IEnumerable<(TypeDeclarationSyntax typeSyntax, INamedTypeSymbol typeSymbol)> FindPartialControllerType(GeneratorExecutionContext context, Compilation compilation, IList<TypeDeclarationSyntax> types, string controllerFullName)
        {
            foreach (var type in types)
            {
                var semanticModel = compilation.GetSemanticModel(type.SyntaxTree);
                if (semanticModel is not null)
                {
                    var symbol = semanticModel.GetDeclaredSymbol(type, context.CancellationToken);
                    if (symbol is not null)
                    {
                        if (symbol.ToDisplayString() == controllerFullName)
                        {
                            yield return (type, symbol);
                        }
                    }
                }
            }
        }

        var partialControllerTypes = FindPartialControllerType(context, compilation, types, controllerFullName).ToList();

        builder.WriteLine("[CompilerGenerated]");
        using (builder.OpenBrace($"public partial class {controllerName} : DevExpress.ExpressApp.ViewController"))
        {
            builder.WriteLine($"public DevExpress.ExpressApp.Actions.SimpleAction {actionName} {{ get; private set; }}");

            builder.WriteLine();

            using (builder.OpenBrace($"public {controllerName}()"))
            {
                builder.WriteLine("this.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;");

                if (actionContext.TargetType is not null)
                {
                    builder.WriteLine($"this.TargetObjectType = typeof({actionContext.TargetType.ToDisplayString()});");
                }

                if (actionContext.ActionAttribute.HasAttribute("TargetViewId"))
                {
                    var targetViewId = $"\"{actionContext.ActionAttribute.GetAttributeValue("TargetViewId", "") ?? ""}\"";

                    builder.WriteLine($"this.TargetViewId = {targetViewId};");
                }

                if (actionContext.ActionAttribute.HasAttribute("TargetViewIds"))
                {
                    var targetViewId = actionContext.ActionAttribute.GetAttribute("TargetViewIds");

                    if (targetViewId.HasValue && targetViewId.Value.Kind is TypedConstantKind.Array && targetViewId.Value.Type is IArrayTypeSymbol)
                    {
                        var targetViewIds = string.Join(";", targetViewId.Value.Values.Select(t => t.Value));
                        builder.WriteLine($"this.TargetViewId = \"{targetViewIds}\";");
                    }
                }

                var category = $"\"{actionContext.ActionAttribute.GetAttributeValue("Category", "Edit") ?? "Edit"}\"";
                actionId = actionContext.ActionAttribute.GetAttributeValue("Id", actionId) ?? actionId;

                var predefinedCategory = actionContext.ActionAttribute.GetAttribute("PredefinedCategory");

                if (predefinedCategory is not null)
                {
                    category = actionContext.ActionAttribute.GetTypeForwardedAttributeValue("PredefinedCategory");
                }

                //TODO: Action Category
                builder.WriteLine($"this.{actionName} = new DevExpress.ExpressApp.Actions.SimpleAction(this, \"{actionId}\", {category});");
                builder.WriteLine($"this.{actionName}.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;");

                foreach (var mappingAttribute in XenialActionAttributeGenerator.ActionAttributeNames.Where(m => !new[]
                {
                        "Id",
                        "Category",
                        "PredefinedCategory",
                        "TargetViewId",
                        "TargetViewIds",
                    }.Contains(m.Key)))
                {
                    MapAttribute(builder, actionContext.ActionAttribute, actionName, mappingAttribute.Key, typeForward: true);
                }
            }

            builder.WriteLine();
            builder.WriteLine("partial void OnActivatedCore();");
            builder.WriteLine();
            builder.WriteLine("partial void OnDeactivatedCore();");
            builder.WriteLine();

            using (builder.OpenBrace("protected override void OnActivated()"))
            {
                builder.WriteLine("base.OnActivated();");

                builder.WriteLine($"this.{actionName}.Execute -= {actionName}Execute;");
                builder.WriteLine($"this.{actionName}.Execute += {actionName}Execute;");

                builder.WriteLine("this.OnActivatedCore();");
            }
            builder.WriteLine();

            using (builder.OpenBrace("protected override void OnDeactivated()"))
            {
                builder.WriteLine($"this.{actionName}.Execute -= {actionName}Execute;");

                builder.WriteLine("this.OnDeactivatedCore();");
                builder.WriteLine("base.OnDeactivated();");
            }
            builder.WriteLine();

            var typeMap = new Dictionary<string, string>()
            {
                ["DevExpress.ExpressApp.XafApplication"] = "this.Application",
                ["DevExpress.ExpressApp.IObjectSpace"] = "this.ObjectSpace"
            };

            static void CreateActionCore(CurlyIndenter builder, XenialActionGeneratorContext actionContext, Dictionary<string, string> typeMap)
            {
                if (actionContext.Constructor is null)
                {
                    builder.WriteLine($"{actionContext.ClassSymbol.ToDisplayString()} action = new {actionContext.ClassSymbol.ToDisplayString()}();");
                }
                else
                {
                    builder.Write($"{actionContext.ClassSymbol.ToDisplayString()} action = new {actionContext.ClassSymbol.ToDisplayString()}(");

                    var parameters = new List<string>();
                    foreach (var parameter in actionContext.Constructor.Parameters)
                    {
                        if (typeMap.TryGetValue(parameter.ToString(), out var resovledValue))
                        {
                            parameters.Add(resovledValue);
                        }
                    }

                    builder.Write(string.Join(", ", parameters));

                    builder.WriteLine(");");
                }
                builder.WriteLine("return action;");
            }

            var partialCreationMethodName = $"Create{actionContext.ClassSymbol.Name}ActionCore";

            var wasFound = false;

            foreach (var partialControllerType in partialControllerTypes)
            {
                var method = partialControllerType.typeSyntax.Members
                    .OfType<MethodDeclarationSyntax>()
                    .Where(m => m.Identifier.Text == partialCreationMethodName)
                    .FirstOrDefault();

                if (method is not null)
                {
                    using (builder.OpenBrace($"protected {actionContext.ClassSymbol.ToDisplayString()} Create{actionContext.ClassSymbol.Name}Action()"))
                    {
                        builder.WriteLine($"return this.Create{actionContext.ClassSymbol.Name}ActionCore();");
                    }

                    builder.WriteLine();

                    builder.WriteLine($"private partial {actionContext.ClassSymbol.ToDisplayString()} Create{actionContext.ClassSymbol.Name}ActionCore();");

                    var semanticModel = compilation.GetSemanticModel(method.SyntaxTree);
                    if (semanticModel is not null)
                    {
                        var returnType = semanticModel.GetTypeInfo(method.ReturnType, context.CancellationToken);

                        if (!method.HasModifier(SyntaxKind.PrivateKeyword) || returnType.Type is null || returnType.Type.ToDisplayString() != actionContext.ClassSymbol.ToDisplayString())
                        {
                            context.ReportDiagnostic(
                                Diagnostic.Create(
                                    GeneratorDiagnostics.ConflictingPartialImplementation(
                                        partialCreationMethodName,
                                        actionContext.ClassSymbol.ToDisplayString(),
                                        "private partial"
                                    ),
                                method.GetLocation()
                            ));
                        }

                        wasFound = true;
                        break;
                    }
                }
            }

            if (!wasFound)
            {
                using (builder.OpenBrace($"protected {actionContext.ClassSymbol.ToDisplayString()} Create{actionContext.ClassSymbol.Name}Action()"))
                {
                    builder.WriteLine($"this.Create{actionContext.ClassSymbol.Name}ActionCore();");
                    CreateActionCore(builder, actionContext, typeMap);
                }

                builder.WriteLine();

                builder.WriteLine($"partial void Create{actionContext.ClassSymbol.Name}ActionCore();");
            }

            builder.WriteLine();

            using (builder.OpenBrace($"private void {actionName}Execute(object sender, DevExpress.ExpressApp.Actions.SimpleActionExecuteEventArgs e)"))
            {
                if (actionContext.TargetType is not null)
                {
                    using (builder.OpenBrace($"if(e.CurrentObject is {actionContext.TargetType.ToDisplayString()})"))
                    {
                        builder.WriteLine($"{actionContext.TargetType.ToDisplayString()} currentObject = ({actionContext.TargetType.ToDisplayString()})e.CurrentObject;");

                        builder.WriteLine($"{actionContext.ClassSymbol.ToDisplayString()} action = this.Create{actionContext.ClassSymbol.Name}Action();");

                        builder.WriteLine();
                        //builder.Write($"action.Execute(currentObject");

                        //if (actionContext.Executor is not null)
                        //{
                        //    foreach (var parameter in actionContext.Executor.Symbol.Parameters)
                        //    {
                        //        if (typeMap.TryGetValue(parameter.ToString(), out var resovledValue))
                        //        {
                        //            builder.Write($", {resovledValue}");
                        //        }
                        //    }
                        //}

                        //builder.WriteLine(");");
                    }
                }
                //TODO: type match failed for whatever reason
                //using (builder.OpenBrace("else"))
                //{

                //}
            }
        }

        return controllerName;
    }

    private static void MapAttribute(CurlyIndenter builder, AttributeData attribute, string actionName, string attributeName, bool typeForward = false)
    {
        var value = attribute.GetAttribute(attributeName);
        if (value.HasValue && value.Value.Kind == TypedConstantKind.Enum && typeForward)
        {
            MapTypeForwardedEnumAttribute(builder, attribute, actionName, attributeName);
        }
        else if (value.HasValue)
        {
            var val = value.Value.MapTypedConstant();
            builder.WriteLine($"this.{actionName}.{attributeName} = {val};");
        }
    }

    private static void MapTypeForwardedEnumAttribute(CurlyIndenter builder, AttributeData attribute, string actionName, string attributeName)
    {
        var value = attribute.GetTypeForwardedAttributeValue(attributeName);
        if (value is not null)
        {
            builder.WriteLine($"this.{actionName}.{attributeName} = {value};");
        }
    }

    private static Compilation AddGeneratedCode(
        GeneratorExecutionContext context,
        Compilation compilation,
        TypeDeclarationSyntax @class,
        CurlyIndenter builder,
        IList<string> addedSourceFiles,
        string? hintName = null,
        bool emitFile = true
    )
    {
        var syntax = builder.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);

        var fileName = Path.GetFileNameWithoutExtension(@class.SyntaxTree.FilePath);

        if (fileName is "")
        {
            fileName = Guid.NewGuid().ToString();
        }

        hintName = string.IsNullOrEmpty(hintName) ? $"{fileName}.{@class.Identifier}.g.cs" : $"{@class.Identifier}.{hintName}.g.cs";

        if (!addedSourceFiles.Contains(hintName))
        {
            addedSourceFiles.Add(hintName);
            if (emitFile)
            {
                context.AddSource(hintName, source);
            }

            var syntaxTree = CSharpSyntaxTree.ParseText(syntax, (CSharpParseOptions)context.ParseOptions, cancellationToken: context.CancellationToken);

            return compilation.AddSyntaxTrees(syntaxTree);
        }

        context.ReportDiagnostic(
            Diagnostic.Create(
                GeneratorDiagnostics.ConflictingClasses(
                    AttributeGenerator.AttributeName,
                    @class.ToString()
                ), @class.GetLocation()
            ));

        return compilation;
    }

    private static AttributeData GetXenialActionAttribute(INamedTypeSymbol symbol, INamedTypeSymbol generateXenialActionAttribute)
        => symbol.GetAttribute(generateXenialActionAttribute);
}


