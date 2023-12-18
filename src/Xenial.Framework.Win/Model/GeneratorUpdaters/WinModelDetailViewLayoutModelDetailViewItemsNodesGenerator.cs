using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.LeafNodes;
using Xenial.Framework.Model.GeneratorUpdaters;

namespace Xenial.Framework.Win.Model.GeneratorUpdaters;

using static Xenial.Framework.Model.GeneratorUpdaters.NodeVisitors;

/// <summary>
/// 
/// </summary>
public sealed class WinModelDetailViewLayoutModelDetailViewItemsNodesGenerator : ModelNodesGeneratorUpdater<ModelDetailViewItemsNodesGenerator>
{
    private MemberEditorInfoCalculator MemberEditorInfoCalculator { get; } = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    public override void UpdateNode(ModelNode node)
    {
        if (node is IModelViewItems viewItems)
        {
            if (viewItems.Parent is IModelDetailView modelDetailView)
            {
                var builder = XenialDetailViewLayoutNodesGeneratorUpdater.FindFunctor(modelDetailView);
                if (builder is null)
                {
                    return;
                }

                var layout = XenialDetailViewLayoutNodesGeneratorUpdater.InvokeBuilder(builder, modelDetailView);

                XenialDetailViewLayoutNodesGeneratorUpdater.MarkDuplicateNodes(layout);

                foreach (var layoutViewItemNode in VisitNodes<LayoutPropertyEditorItem>(layout))
                {
                    if (!string.IsNullOrEmpty(layoutViewItemNode.EditorAlias))
                    {
                        var viewItem = viewItems.OfType<IModelViewItem>().FirstOrDefault(m =>
                            m.Id == (layoutViewItemNode.IsDuplicate
                            ? layoutViewItemNode.Id
                            : layoutViewItemNode.ViewItemId)
                        );

                        if (viewItem is IModelPropertyEditor modelPropertyEditor)
                        {
                            modelPropertyEditor.PropertyEditorType
                                = MemberEditorInfoCalculator.GetEditorType(
                                    modelPropertyEditor.ModelMember,
                                    layoutViewItemNode.EditorAlias
                            );
                        }
                    }
                }
            }
        }
    }
}
