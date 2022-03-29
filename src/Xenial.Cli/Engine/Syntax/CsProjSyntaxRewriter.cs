using Buildalyzer;

using Microsoft.CodeAnalysis;

using Xenial.Cli.Engine;

public class CsProjSyntaxRewriter
{
    private readonly IProjectAnalyzer projectAnalyzer;
    private readonly IAnalyzerResult buildResult;
    private readonly Dictionary<string, (FileState state, SyntaxTree syntaxTree)> modifications;
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

        if (EnableDefaultItems(buildResult) || !modifications.Any((m) => m.Value.state == FileState.Added))
        {
            return (false, null);
        }


        return (true, null);
    }
}
