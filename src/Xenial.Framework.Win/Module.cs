using System;

using DevExpress.XtraLayout;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.LeafNodes;
using Xenial.Framework.Model.GeneratorUpdaters;

namespace Xenial.Framework.Win;

/// <summary>
/// 
/// </summary>

#if NET5_0_OR_GREATER
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
public static class XenialWindowsFormsModuleInitializer
{
    private static bool initialized;
#if NET5_0_OR_GREATER
    [System.Runtime.CompilerServices.ModuleInitializer]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
    /// <summary>
    /// 
    /// </summary>
    public static void Initialize()
    {
        if (initialized)
        {
            return;
        }
        initialized = true;
        MappingFactory
            .RegisterDetailOptionsMapper((options, model) =>
            {
                if (options is DetailViewOptionsWin winOptions)
                {
                    ViewOptionsMapper.Map(winOptions, model);
                }
            });

        XenialDetailViewLayoutNodesGeneratorUpdater.NodeFactory
            .Register<LayoutSplitterItem>((parent, node) => parent.AddNode<DevExpress.ExpressApp.Win.SystemModule.IModelSplitter>())
            .RegisterAutoId<LayoutSplitterItem>((node, index) => $"Splitter-{index}")
            .Register<LayoutSeparatorItem>((parent, node) => parent.AddNode<DevExpress.ExpressApp.Win.SystemModule.IModelSeparator>())
            .RegisterAutoId<LayoutSeparatorItem>((node, index) => $"Separator-{index}");
    }
}
