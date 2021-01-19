using System;

using Xenial.FeatureCenter.Module.BusinessObjects.Editors;
using Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders;

namespace Xenial.FeatureCenter.Module.BusinessObjects
{
    internal class ModelTypeList
    {
        internal static readonly Type[] PersistentTypes = new[]
        {
            typeof(FeatureCenterBaseObject),
            typeof(FeatureCenterBaseObjectId),

#region Editors
            typeof(FeatureCenterEditorsBaseObject),

            typeof(StepProgressBarEnumEditorDemo),

            typeof(TokenStringEditorDemo),
            typeof(TokenObjectsEditorDemo),
            typeof(TokenObjectsEditorDemoTokens),

            typeof(WebViewUriEditorDemo),
            typeof(WebViewHtmlStringEditorDemo),
#endregion

#region ModelBuilders
            typeof(FeatureCenterModelBuildersBaseObject),

            typeof(ModelBuilderIntroductionDemo)
#endregion
        };

        internal static readonly Type[] NonPersistentTypes = Array.Empty<Type>();
    }
}
