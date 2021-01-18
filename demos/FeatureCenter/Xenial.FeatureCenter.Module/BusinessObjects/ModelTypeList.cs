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
            typeof(FeatureCenterDemoBaseObjectId),

            typeof(StepProgressBarEnumEditorDemo),

            typeof(TokenStringEditorDemo),
            typeof(TokenObjectsEditorDemo),
            typeof(TokenObjectsEditorDemoTokens),

            typeof(WebViewUriEditorDemo),
            typeof(WebViewHtmlStringEditorDemo),
        };

        internal static readonly Type[] NonPersistentTypes = new[]
        {
            typeof(ModelBuilderBasicPropertiesDemo),
        };
    }
}
