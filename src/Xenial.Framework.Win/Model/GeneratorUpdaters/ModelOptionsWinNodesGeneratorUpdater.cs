﻿using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win.SystemModule;

using Xenial.Framework.Model.GeneratorUpdaters;

namespace Xenial.Framework.Win.Model.GeneratorUpdaters;

/// <summary>
/// Class ModelOptionsWinNodesGeneratorUpdater. Implements the
/// <see cref="Xenial.Framework.Model.GeneratorUpdaters.ModelOptionsNodesGeneratorUpdater" />
/// </summary>
///
/// <seealso cref="ModelOptionsNodesGeneratorUpdater"/>
/// <seealso cref="Xenial.Framework.Model.GeneratorUpdaters.ModelOptionsNodesGeneratorUpdater"/>

[XenialCheckLicense]
public sealed partial class ModelOptionsWinNodesGeneratorUpdater : ModelOptionsNodesGeneratorUpdater
{
    private new ApplicationWinOptions Options => (ApplicationWinOptions)base.Options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelOptionsWinNodesGeneratorUpdater"/> class.
    /// </summary>
    ///
    /// <param name="options">  The options. </param>

    public ModelOptionsWinNodesGeneratorUpdater(ApplicationWinOptions options) : base(options) { }

    /// <summary>
    /// Updates the Application Model node content generated by the Nodes Generator, specified by the
    /// <see cref="T:DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater`1" /> class' type
    /// parameter.
    /// </summary>
    ///
    /// <param name="node"> A ModelNode Application Model node to be updated. </param>

    public override void UpdateNode(ModelNode node)
    {
        base.UpdateNode(node);

        if (node is IModelOptionsWin options)
        {
            options.FormStyle = Options.FormStyle
                ?? options.FormStyle;

            options.MdiDefaultNewWindowTarget = Options.MdiDefaultNewWindowTarget
                ?? options.MdiDefaultNewWindowTarget;

            options.ShowTabImage = Options.ShowTabImage
               ?? options.ShowTabImage;

            options.Messaging = Options.Messaging == null
                ? options.Messaging
                : Options.Messaging.FullName;

            options.RibbonOptions.MinimizeRibbon = Options.RibbonOptions.MinimizeRibbon
                ?? options.RibbonOptions.MinimizeRibbon;

            options.RibbonOptions.RibbonControlStyle = Options.RibbonOptions.RibbonControlStyle
                ?? options.RibbonOptions.RibbonControlStyle;
        }

        if (node is IModelOptionsEnableHtmlFormatting htmlFormattingNode)
        {
            htmlFormattingNode.EnableHtmlFormatting = Options.EnableHtmlFormatting
                ?? htmlFormattingNode.EnableHtmlFormatting;
        }
    }
}
