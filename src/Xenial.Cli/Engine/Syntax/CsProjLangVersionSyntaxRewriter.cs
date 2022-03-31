using Buildalyzer;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Xenial.Cli.Engine;

using CsprojEditor = CsProjEditor.Project;

public class CsProjLangVersionSyntaxRewriter
{
    private readonly IProjectAnalyzer projectAnalyzer;
    private readonly IAnalyzerResult buildResult;
    private readonly LanguageVersion newLangVersion;

    private CsprojEditor? CsprojEditor { get; set; }

    public CsProjLangVersionSyntaxRewriter(IProjectAnalyzer projectAnalyzer, IAnalyzerResult buildResult, LanguageVersion newLangVersion)
    {
        this.projectAnalyzer = projectAnalyzer;
        this.buildResult = buildResult;
        this.newLangVersion = newLangVersion;
    }

    public async Task<(bool shouldRewrite, string? csprojFileName)> RewriteAsync()
    {
        if (projectAnalyzer.ProjectFile.RequiresNetFramework)
        {

        }

        CsprojEditor = CsprojEditor.Load(projectAnalyzer.ProjectFile.Path);

        var langVersionStr = newLangVersion switch
        {
            LanguageVersion.CSharp10 => "10",
            LanguageVersion.CSharp9 => "9",
            LanguageVersion.Preview => "preview",
            var x => throw new ArgumentOutOfRangeException(nameof(newLangVersion), x, "Can't translate version to LangVersion"),
        };

        var oldValue = buildResult.GetProperty("LangVersion");

        CsprojEditor.ReplaceNodeValue("PropertyGroup", "LangVersion", oldValue, langVersionStr);

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
