using System;
using System.Collections.Generic;
using System.Text;

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

            typeof(TokenEditorPersistentDemo),
            typeof(TokenEditorPersistentTokens),

            typeof(StepProgressBarEnumEditorDemo)
        };

        internal static readonly Type[] NonPersistentTypes = new[]
        {
            typeof(ModelBuilderBasicPropertiesDemo),

            typeof(WebViewEditorDemo),

            typeof(TokenEditorNonPersistentDemo),
            typeof(TokenEditorNonPersistentTokens)
        };
    }
}
