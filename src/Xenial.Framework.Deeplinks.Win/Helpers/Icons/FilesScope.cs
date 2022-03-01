using System;
using System.Collections.Generic;
using System.Linq;


namespace Xenial.Framework.Deeplinks.Win.Helpers.Icons;

/// <summary>
/// 
/// </summary>
public sealed class FilesScope : IDisposable
{
    private readonly List<FileScope> scopes = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="files"></param>
    public FilesScope(IEnumerable<string> files!!)
    {
        foreach (var file in files)
        {
            scopes.Add(new FileScope(file));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<string> FilesNames => scopes.Select(m => m.FileName);

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        foreach (var fileScope in scopes)
        {
            fileScope.Dispose();
        }
    }
}
