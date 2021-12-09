
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

using Xenial.Framework.Generators.Internal;
using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators;

public record XenialActionGeneratorOutputOptions(
    bool Attribute = true,
    bool PartialBuddy = true,
    bool Controller = true
);

public record XenialActionGenerator(XenialActionGeneratorOutputOptions OutputOptions) : IXenialSourceGenerator
{
    private const string xenialActionAttributeName = "XenialActionAttribute";
    private const string xenialNamespace = "Xenial";
    private const string xenialActionAttributeFullName = $"{xenialNamespace}.{xenialActionAttributeName}";
    public const string GenerateXenialActionAttributeMSBuildProperty = $"Generate{xenialActionAttributeName}";

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
                        xenialActionAttributeName
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
                        xenialActionAttributeName
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
                        xenialActionAttributeName,
                        new[] { "Category", "PredefinedCategory" }
                    ), Class.GetLocation())
                );
            }
            if (HasConflictingTargetViewIdAttributes)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.ConflictingAttributes(
                        xenialActionAttributeName,
                        new[] { "TargetViewId", "TargetViewIds" }
                    ), Class.GetLocation())
                );
            }
            return compilation;
        }
    }

    public Compilation Execute(
        GeneratorExecutionContext context,
        Compilation compilation,
        IList<TypeDeclarationSyntax> types,
        IList<string> addedSourceFiles
    )
    {
        _ = compilation ?? throw new ArgumentNullException(nameof(compilation));
        _ = types ?? throw new ArgumentNullException(nameof(types));
        _ = addedSourceFiles ?? throw new ArgumentNullException(nameof(addedSourceFiles));

        compilation = GenerateAttribute(context, compilation);

        var generateXenialActionAttribute = compilation.GetTypeByMetadataName(xenialActionAttributeFullName);

        if (generateXenialActionAttribute is null)
        {
            //TODO: Warning Diagnostics for either setting the right MSBuild properties or referencing `Xenial.Framework.CompilerServices`
            return compilation;
        }

        var collectedContexts = new List<XenialActionGeneratorContext>();

        foreach (var @class in types)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var (semanticModel, @classSymbol, isAttributeDeclared) = TryGetTargetType(context, compilation, @class, generateXenialActionAttribute);
            if (!isAttributeDeclared || semanticModel is null || @classSymbol is null)
            {
                continue;
            }

            var @attribute = GetXenialActionAttribute(@classSymbol, generateXenialActionAttribute);

            var isGlobalNamespace = classSymbol.ContainingNamespace.ToString() == "<global namespace>";
            if (isGlobalNamespace)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.ClassNeedsToBeInNamespace(
                        xenialActionAttributeName
                    ), @class.GetLocation())
                );

                return compilation;
            }

            ITypeSymbol? GetTargetType()
            {
                var detailViewActionInterface = @classSymbol.AllInterfaces
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

            var possibleCtors = classSymbol.InstanceConstructors
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
                @classSymbol.ContainingNamespace,
                @class,
                @classSymbol,
                attribute,
                GetTargetType(),
                possibleCtor,
                XenialMethodGeneratorContext.CreateContext(context, semanticModel, executorMethod)
            );

            collectedContexts.Add(actionContext);
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
                controllerName = GenerateController(context, compilation, builder, actionContext);
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


    private static string GenerateController(GeneratorExecutionContext context, Compilation compilation, CurlyIndenter builder, XenialActionGeneratorContext actionContext)
    {
        var actionId = $"{actionContext.Namespace}.{actionContext.ClassSymbol.Name}SimpleAction";
        var actionName = $"{actionContext.ClassSymbol.Name}SimpleAction";
        var controllerName = $"{actionContext.ClassSymbol.Name}Controller";

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

                foreach (var mappingAttribute in actionAttributeNames.Where(m => !new[]
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

            using (builder.OpenBrace($"protected {actionContext.ClassSymbol.ToDisplayString()} Create{actionContext.ClassSymbol.Name}Action()"))
            {
                builder.WriteLine($"this.Create{actionContext.ClassSymbol.Name}ActionCore();");
                CreateActionCore(builder, actionContext, typeMap);
            }

            builder.WriteLine();

            //TODO: see if we can map those to controller code with intellisense
            builder.WriteLine($"partial void Create{actionContext.ClassSymbol.Name}ActionCore();");
            builder.WriteLine();


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

    private static Compilation GenerateAttribute(GeneratorExecutionContext context, Compilation compilation)
    {
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{GenerateXenialActionAttributeMSBuildProperty}", out var generateXenialActionAttrStr))
        {
            if (bool.TryParse(generateXenialActionAttrStr, out var generateXenialActionAttr))
            {
                if (!generateXenialActionAttr)
                {
                    return compilation;
                }
            }
            else
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.InvalidBooleanMsBuildProperty(
                            GenerateXenialActionAttributeMSBuildProperty,
                            generateXenialActionAttrStr
                        )
                        , null
                    ));

                return compilation;
            }
        }

        var (source, syntaxTree) = GenerateXenialActionsAttribute(
            (CSharpParseOptions)context.ParseOptions,
            context.GetDefaultAttributeModifier(),
            context.CancellationToken
        );

        var hintName = $"{xenialActionAttributeName}.g.cs";

        context.AddSource(hintName, source);

        return compilation.AddSyntaxTrees(syntaxTree);
    }

    private static readonly Dictionary<string, string> actionAttributeNames = new()
    {
        ["Caption"] = "string",
        ["ImageName"] = "string",
        ["Category"] = "string",
        ["DiagnosticInfo"] = "string",
        ["Id"] = "string",
        ["TargetViewId"] = "string",
        ["TargetViewIds"] = "string[]",
        ["TargetObjectsCriteria"] = "string",
        ["ConfirmationMessage"] = "string",
        ["ToolTip"] = "string",
        ["Shortcut"] = "string",

        ["TargetObjectType"] = "Type",
        ["TypeOfView"] = "Type",

        ["QuickAccess"] = "bool",

        ["Tag"] = "object",

        ["PredefinedCategory"] = "XenialPredefinedCategory",
        ["SelectionDependencyType"] = "XenialSelectionDependencyType",
        ["ActionMeaning"] = "XenialActionMeaning",
        ["TargetViewType"] = "XenialViewType",
        ["TargetViewNesting"] = "XenialNesting",
        ["TargetObjectsCriteriaMode"] = "XenialTargetObjectsCriteriaMode",
        ["PaintStyle"] = "XenialActionItemPaintStyle",
    };

    public static (SourceText source, SyntaxTree syntaxTree) GenerateXenialActionsAttribute(
        CSharpParseOptions? parseOptions = null,
        string visibility = "internal",
        CancellationToken cancellationToken = default
    )
    {
        parseOptions = parseOptions ?? CSharpParseOptions.Default;
        var builder = CurlyIndenter.Create();

        builder.WriteLine($"using System;");
        builder.WriteLine();
        builder.WriteLine("using Xenial.ExpressApp;");
        builder.WriteLine("using Xenial.ExpressApp.Actions;");
        builder.WriteLine("using Xenial.ExpressApp.Templates;");
        builder.WriteLine("using Xenial.Persistent.Base;");
        builder.WriteLine();

        using (builder.OpenBrace($"namespace {xenialNamespace}"))
        {
            builder.WriteLine("[AttributeUsage(AttributeTargets.Class, Inherited = false)]");
            using (builder.OpenBrace($"{visibility} sealed class {xenialActionAttributeName} : Attribute"))
            {
                builder.WriteLine($"{visibility} {xenialActionAttributeName}() {{ }}");

                foreach (var actionAttributePair in actionAttributeNames)
                {
                    builder.WriteLine($"public {actionAttributePair.Value} {actionAttributePair.Key} {{ get; set; }}");
                }
            }

            builder.WriteLine();

            builder.WriteLine($"{visibility} interface IDetailViewAction<T> {{ }}");
            builder.WriteLine();
            builder.WriteLine($"{visibility} interface IListViewAction<T> {{ }}");
        }

        var syntax = builder.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, parseOptions, cancellationToken: cancellationToken);
        return (source, syntaxTree);
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
                    xenialActionAttributeName,
                    @class.ToString()
                ), @class.GetLocation()
            ));

        return compilation;
    }

    private static AttributeData GetXenialActionAttribute(INamedTypeSymbol symbol, INamedTypeSymbol generateXenialActionAttribute)
        => symbol.GetAttribute(generateXenialActionAttribute);

    private static (SemanticModel? semanticModel, INamedTypeSymbol? @classSymbol, bool isAttributeDeclared) TryGetTargetType(
        GeneratorExecutionContext context,
        Compilation compilation,
        TypeDeclarationSyntax @class,
        INamedTypeSymbol generateXenialImageNamesAttribute
    )
    {
        var semanticModel = compilation.GetSemanticModel(@class.SyntaxTree);
        if (semanticModel is null)
        {
            return (semanticModel, null, false);
        }

        var symbol = semanticModel.GetDeclaredSymbol(@class, context.CancellationToken);

        if (symbol is null)
        {
            return (semanticModel, null, false);
        }

        var isAttributeDeclared = symbol.IsAttributeDeclared(generateXenialImageNamesAttribute);

        return (semanticModel, symbol, isAttributeDeclared);
    }

}


