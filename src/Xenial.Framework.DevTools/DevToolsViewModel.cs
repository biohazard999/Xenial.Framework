using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.DC;

using Xenial.Framework.Images;
using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.ModelBuilders;

namespace Xenial.Framework.DevTools;

/// <summary>
/// 
/// </summary>
[DomainComponent]
[DetailViewLayoutBuilder(typeof(DevToolsViewModelLayoutBuilder))]
public class DevToolsViewModel : DevExpress.ExpressApp.NonPersistentBaseObject
{
    private string? viewId;
    /// <summary>
    /// 
    /// </summary>
    public string? ViewId
    {
        get => viewId;
        set => SetPropertyValue(ref viewId, value);
    }

    private string? xafml;
    /// <summary>
    /// 
    /// </summary>
    public string? Xafml
    {
        get => xafml;
        set => SetPropertyValue(ref xafml, value);
    }

    private string? code;
    /// <summary>
    /// 
    /// </summary>
    public string? Code
    {
        get => code;
        set => SetPropertyValue(ref code, value);
    }
}

/// <summary>
/// 
/// </summary>
public partial class DevToolsViewModelLayoutBuilder : LayoutBuilder<DevToolsViewModel>
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Layout BuildLayout() => new(new()
    {
        EnableLayoutGroupImages = true
    })
    {
        Editor.ViewId,
        TabbedGroup(
            Tab("Xafml", "Action_EditModel", Editor.Xafml with { ShowCaption = false }),
            Tab("Code", XenialImages.Action_Xenial_Code, Editor.Code with { ShowCaption = false })
        )
    };
}

/// <summary>
/// 
/// </summary>
public partial class DevToolsViewModelBuilder : ModelBuilder<DevToolsViewModel>
{
    /// <summary>   Initializes a new instance of the <see cref="DevToolsViewModelBuilder`1"/> class. </summary>
    /// <param name="typeInfo"> The type information. </param>
    public DevToolsViewModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

    /// <summary>
    /// 
    /// </summary>
    public override void Build()
    {
        base.Build();

        Xafml.UseWebViewHtmlStringPropertyEditor();
        Code.UseWebViewHtmlStringPropertyEditor();
    }
}
