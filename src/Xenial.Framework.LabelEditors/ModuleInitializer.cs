using Xenial.Framework.LabelEditors.Model;
using Xenial.Framework.LabelEditors.Layout;
using Xenial.Framework.Model.GeneratorUpdaters;

namespace DevExpress.ExpressApp;

/// <summary>
/// 
/// </summary>

#if NET5_0_OR_GREATER
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
public static class XenialDeeplinkModuleInitializer
{
    private static bool initialized;

    /// <summary>
    /// 
    /// </summary>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2255:The 'ModuleInitializer' attribute should not be used in libraries")]
    [System.Runtime.CompilerServices.ModuleInitializer]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
    public static void Initialize()
    {
        if (initialized)
        {
            return;
        }
        initialized = true;

        XenialModelDetailViewItemsNodesGenerator.NodeFactory
            .Register<HtmlContentLayoutViewItem>((parent, node) => parent.AddNode<IHtmlContentViewItem>())
        ;
    }
}
