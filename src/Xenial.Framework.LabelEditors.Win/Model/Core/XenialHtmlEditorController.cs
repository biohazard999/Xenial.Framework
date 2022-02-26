using System;
using System.Linq;

namespace Xenial.Framework.LabelEditors.Win.Model.Core;

/// <summary>
/// 
/// </summary>
/// <param name="Form"></param>
public sealed record XenialHtmlEditorController(XenialHtmlEditorForm Form) : IDisposable
{
    private bool disposedValue;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="htmlText"></param>
    /// <returns></returns>
    public string ShowEditor(string htmlText)
    {
        Form.HtmlCodeViewer.Template = htmlText;
        if (Form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            return Form.HtmlCodeViewer.Template;
        }
        return htmlText;
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (!Form.IsDisposed)
                {
                    Form.Dispose();
                }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    ~XenialHtmlEditorController()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
