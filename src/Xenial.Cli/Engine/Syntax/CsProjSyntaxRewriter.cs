using Buildalyzer;

using Microsoft.CodeAnalysis;

using Xenial.Cli.Engine;

using CsprojEditor = CsProjEditor.Project;

public class CsProjSyntaxRewriter
{
    private readonly IProjectAnalyzer projectAnalyzer;
    private readonly IAnalyzerResult buildResult;
    private readonly Dictionary<string, (FileState state, SyntaxTree syntaxTree)> modifications;

    private CsprojEditor? CsprojEditor { get; set; }

    public CsProjSyntaxRewriter(IProjectAnalyzer projectAnalyzer, IAnalyzerResult buildResult, Dictionary<string, (FileState state, SyntaxTree syntaxTree)> modifications)
    {
        this.projectAnalyzer = projectAnalyzer;
        this.buildResult = buildResult;
        this.modifications = modifications;
    }

    public async Task<(bool shouldRewrite, string? csprojFileName)> RewriteAsync()
    {
        if (projectAnalyzer.ProjectFile.RequiresNetFramework)
        {

        }
        static bool EnableDefaultItems(IAnalyzerResult buildResult)
        {
            //TODO: Old CS PROJ FORMAT
            if (bool.TryParse(buildResult.GetProperty("EnableDefaultItems"), out var enableDefaultItems))
            {
                return enableDefaultItems;
            }
            return true;
        }

        var addedFiles = modifications.Where((m) => m.Value.state == FileState.Added).ToArray();

        if (EnableDefaultItems(buildResult) || !addedFiles.Any())
        {
            return (false, null);
        }

        CsprojEditor = CsprojEditor.Load(projectAnalyzer.ProjectFile.Path);

        CsprojEditor.InsertGroup("ItemGroup");

        var folder = Path.GetDirectoryName(projectAnalyzer.ProjectFile.Path)!;

        foreach (var path in addedFiles)
        {
            var relativePath = path.Value.syntaxTree.FilePath;
            if (relativePath.StartsWith(folder, StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Substring(folder.Length);
                relativePath = relativePath.TrimStart('\\');
            }
            CsprojEditor.InsertAttribute("ItemGroup", "Compile", new CsProjEditor.CsProjAttribute("Include", relativePath), e => !e.HasAttributes);
        }


        return (true, projectAnalyzer.ProjectFile.Path);
    }

    public Task CommitAsync()
    {
        if (CsprojEditor is null
            || projectAnalyzer.ProjectFile.Path is null
            || !File.Exists(projectAnalyzer.ProjectFile.Path)
        )
        {
            return Task.CompletedTask;
        }

        CsprojEditor.Save(projectAnalyzer.ProjectFile.Path);

        return Task.CompletedTask;
    }
}
