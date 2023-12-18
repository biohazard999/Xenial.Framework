using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Xenial.Framework.LabelEditors.Win.Model.Core;

/// <summary>
/// 
/// </summary>
/// <param name="Form"></param>
public sealed record XenialCssEditorController(XenialCssEditorForm Form) : IDisposable
{
    private bool disposedValue;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="htmlText"></param>
    /// <returns></returns>
    public string ShowEditor(string htmlText)
    {
        Form.CssCodeViewer.Styles = htmlText;
        if (Form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            return Form.CssCodeViewer.Styles;
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
            disposedValue = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    ~XenialCssEditorController() =>
        Dispose(disposing: false);

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
