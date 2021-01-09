using System;
using System.Linq;
using System.Reflection;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Xpo;
using DevExpress.XtraBars.Ribbon;

using Xenial.FeatureCenter.Module.BusinessObjects.Editors;
using Xenial.Framework;
using Xenial.Framework.TokenEditors;
using Xenial.Framework.TokenEditors.Win;
using Xenial.Framework.WebView.Win;

namespace Xenial.FeatureCenter.Module.Win
{
    public class StepProgressBarEnumModelGeneratorUpdater : ModelNodesGeneratorUpdater<ModelBOModelMemberNodesGenerator>
    {
        public override void UpdateNode(ModelNode node)
        {
            if (node is IModelBOModelClassMembers membersNode)
            {
                var members = membersNode
                    .Select(m =>
                    {
                        var (memberInfo, attr) = (m, m.MemberInfo.FindAttribute<DevExpress.Persistent.Base.EditorAliasAttribute>(true));
                        return (memberInfo, attr);
                    })
                    .Where(info =>
                    {
                        var result = info.attr is not null && info.attr.Alias == "Xenial.StepProgressBarEnumPropertyEditor";
                        if (result)
                        {
                            var underlyingType = Nullable.GetUnderlyingType(info.memberInfo.Type);
                            if (underlyingType != null && underlyingType.IsEnum)
                            {
                                return true;
                            }
                        }
                        return result;
                    });

                foreach (var member in members)
                {
                    UpdatePropertyEditorType(member.memberInfo);
                }
            }

        }

        private static void UpdatePropertyEditorType(IModelMember propertyNode)
        {
            var editorDescriptors = ((IModelSources)propertyNode.Application).EditorDescriptors;
            if (editorDescriptors != null)
            {
                foreach (var typeRegistration in editorDescriptors.PropertyEditorRegistrations)
                {
                    if (typeRegistration.Alias == "Xenial.StepProgressBarEnumPropertyEditor")
                    {
                        propertyNode.PropertyEditorType = typeRegistration.EditorType;
                    }
                }
            }
        }
    }

    public class FeatureCenterWindowsFormsModule : XenialModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore()
            .AndModuleTypes(
                typeof(FeatureCenterModule),
                typeof(XenialWebViewWindowsFormsModule),

                typeof(XenialTokenEditorsModule),
                typeof(XenialTokenEditorsWindowsFormsModule)
            );

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory)
        {
            base.RegisterEditorDescriptors(editorDescriptorsFactory);

            editorDescriptorsFactory.UseTokenObjectsPropertyEditorsWin<TokenEditorNonPersistentTokens>();
            editorDescriptorsFactory.UseTokenObjectsPropertyEditorsForTypeWin<XPCollection<TokenEditorPersistentTokens>>();

            editorDescriptorsFactory.RegisterPropertyEditor("Xenial.StepProgressBarEnumPropertyEditor", typeof(Enum), typeof(StepProgressBarEnumPropertyEditor), false);
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);

            updaters.Add(new StepProgressBarEnumModelGeneratorUpdater());
        }
    }
}
