using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Internal;

using System;
using System.Linq;
using System.Windows.Forms;

namespace Xenial.Framework.LabelEditors.Win.Model.Core;

/// <summary>
/// 
/// </summary>
public sealed partial class XenialCssEditorForm : XtraForm
{
    /// <summary>
    /// 
    /// </summary>
    public XenialCssEditorForm()
        => InitializeComponent();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        //TODO: make Shift Tab to unintend code
        if (keyData == (Keys.Tab | Keys.Shift))
        {
            return true;
        }
        else
        {
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public CssCodeViewer CssCodeViewer => cssCodeViewer;

    /// <summary>
    /// 
    /// </summary>
    public SimpleButton ButtonCancel => btnCancel;

    /// <summary>
    /// 
    /// </summary>
    public SimpleButton ButtonOk => btnOk;
}
