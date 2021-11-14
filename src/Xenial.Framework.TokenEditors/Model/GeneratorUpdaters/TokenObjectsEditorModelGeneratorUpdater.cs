﻿using System;
using System.Linq;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Persistent.Base;

using Xenial.Framework.Model.Core;
using Xenial.Framework.StepProgressEditors.Model.GeneratorUpdaters;
using Xenial.Framework.TokenEditors.PubTernal;

namespace Xenial.Framework.StepProgressEditors.Model.GeneratorUpdaters
{
    /// <summary>
    /// Class TokenObjectsEditorModelGeneratorUpdater. Implements the
    /// <see cref="DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater{DevExpress.ExpressApp.Model.NodeGenerators.ModelBOModelMemberNodesGenerator}" />
    /// </summary>
    ///
    /// <seealso cref="ModelNodesGeneratorUpdater{ModelBOModelMemberNodesGenerator}"/>
    /// <seealso cref="DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater{DevExpress.ExpressApp.Model.NodeGenerators.ModelBOModelMemberNodesGenerator}">    <autogeneratedoc /></seealso>

    [XenialCheckLicense]
    public sealed partial class TokenObjectsEditorModelGeneratorUpdater : ModelNodesGeneratorUpdater<ModelBOModelMemberNodesGenerator>
    {
        /// <summary>
        /// Updates the Application Model node content generated by the Nodes Generator, specified by the
        /// <see cref="T:DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater`1" /> class' type
        /// parameter.
        /// </summary>
        ///
        /// <param name="node"> A ModelNode Application Model node to be updated. </param>

        public override void UpdateNode(ModelNode node)
        {
            if (node is IModelBOModelClassMembers membersNode)
            {
                var editorType = EditorTypeVisibilityCalculator.FindPropertyEditorType(node, TokenEditorAliases.TokenObjectsPropertyEditor);
                if (editorType is null)
                {
                    return;
                }

                var members = membersNode
                    .Select(m =>
                    {
                        var (memberInfo, attr) = (m, m.MemberInfo.FindAttribute<EditorAliasAttribute>(true));
                        return (memberInfo, attr);
                    })
                    .Where(info =>
                    {
                        var result = info.attr is not null && info.attr.Alias == TokenEditorAliases.TokenObjectsPropertyEditor;
                        if (result)
                        {
                            if (typeof(System.Collections.IList).IsAssignableFrom(info.memberInfo.Type))
                            {
                                return true;
                            }
                        }
                        return false;
                    });

                foreach (var member in members)
                {
                    member.memberInfo.PropertyEditorType = editorType;
                }
            }
        }
    }
}
