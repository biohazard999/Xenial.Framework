using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

using Xenial.Framework.Images;

namespace Xenial.Framework.Deeplinks.Model;

/// <summary>
/// 
/// </summary>
[ImageName(XenialImages.Model_Jumplists_CustomCategory)]
public interface IModelJumplistTaskCategory : IModelNode, IModelList<IModelJumplistItem>
{

}
