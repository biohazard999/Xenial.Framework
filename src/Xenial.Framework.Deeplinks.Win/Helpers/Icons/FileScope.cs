using System;
using System.IO;

namespace Xenial.Framework.Deeplinks.Win.Helpers.Icons;

/// <summary>
/// 
/// </summary>
public sealed class FileScope : IDisposable
{
    private readonly string fileName;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    public FileScope(string fileName)
        => this.fileName = fileName;

    /// <summary>
    /// 
    /// </summary>
    public string FileName => fileName;

    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    public void Dispose()
    {
        try
        {
            File.Delete(FileName);
        }
        catch
        {

        }
    }
}
