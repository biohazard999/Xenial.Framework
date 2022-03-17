﻿using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

using Xenial.Framework.Deeplinks.Generators;
using Xenial.Framework.Images;

namespace Xenial.Framework.Deeplinks.Model;

/// <summary>
/// 
/// </summary>
[ImageName(XenialImages.Model_Jumplists_CustomCategory)]
[ModelNodesGenerator(typeof(ModelJumplistCustomCategoriesGenerator))]
public interface IModelJumplistCustomCategories : IModelNode, IModelList<IModelJumplistCustomCategory>
{

}