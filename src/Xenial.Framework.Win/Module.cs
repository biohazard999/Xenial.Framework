using System;

using DevExpress.XtraLayout;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.LeafNodes;
using Xenial.Framework.Model.GeneratorUpdaters;

namespace Xenial.Framework.Win;

internal static class XenialWindowsFormsModuleInitializer
{
#if NET5_0_OR_GREATER
    [System.Runtime.CompilerServices.ModuleInitializer]
#endif
    internal static void Initialize()
    {
        MappingFactory
            .RegisterDetailOptionsMapper((options, model) =>
            {
                if (options is DetailViewOptionsWin winOptions)
                {
                    new ViewOptionsMapper()
                        .Map(winOptions, model);
                }
            });


        XenialDetailViewLayoutNodesGeneratorUpdater.NodeFactory
            .Register<LayoutSplitterItem>((parent, node) => parent.AddNode<DevExpress.ExpressApp.Win.SystemModule.IModelSplitter>())
            .RegisterAutoId<LayoutSplitterItem>((node, index) => $"Splitter-{index}")
            .Register<LayoutSeparatorItem>((parent, node) => parent.AddNode<DevExpress.ExpressApp.Win.SystemModule.IModelSeparator>())
            .RegisterAutoId<LayoutSeparatorItem>((node, index) => $"Separator-{index}");
    }
}
