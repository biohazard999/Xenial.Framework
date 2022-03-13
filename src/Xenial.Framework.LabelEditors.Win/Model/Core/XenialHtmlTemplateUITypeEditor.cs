
using DevExpress.LookAndFeel;

using System;
using System.ComponentModel;
using System.Drawing.Design;

using System.Linq;

namespace Xenial.Framework.LabelEditors.Win.Model.Core;

/// <summary>
/// 
/// </summary>
public class XenialHtmlTemplateUITypeEditor : UITypeEditor
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="provider"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public override object EditValue(ITypeDescriptorContext context!!, IServiceProvider provider, object value)
    {
        var htmlText = (string)context.PropertyDescriptor.GetValue(context.Instance);

        using var form = new XenialHtmlEditorForm();
        if (provider is ISupportLookAndFeel providerSupportLookAndFeel)
        {
            form.LookAndFeel.Assign(providerSupportLookAndFeel.LookAndFeel);
        }
        using XenialHtmlEditorController controller = new(form);
        return controller.ShowEditor(htmlText);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        => UITypeEditorEditStyle.Modal;
}
