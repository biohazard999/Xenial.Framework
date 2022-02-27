using System.ComponentModel;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

namespace Xenial.Framework.Deeplinks.Model;

/// <summary>
/// 
/// </summary>
[ModelAbstractClass]
public interface IModelJumplistItemBase : IModelJumplistItem
{
    /// <summary>
    /// 
    /// </summary>
    string Caption { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, DevExpress.Utils.ControlConstants.UITypeEditor)]
    string ImageName { get; set; }
}
