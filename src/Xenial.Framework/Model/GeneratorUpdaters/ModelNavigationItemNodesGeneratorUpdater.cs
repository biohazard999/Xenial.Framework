﻿using System;
using System.Linq;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;

namespace Xenial.Framework.Model.GeneratorUpdaters;

/// <summary>
/// Class ModelNavigationItemNodesGeneratorUpdater. Implements the
/// <see cref="DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater{DevExpress.ExpressApp.SystemModule.NavigationItemNodeGenerator}" />
/// </summary>
///
/// <seealso cref="ModelNodesGeneratorUpdater{NavigationItemNodeGenerator}"/>
/// <seealso cref="DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater{DevExpress.ExpressApp.SystemModule.NavigationItemNodeGenerator}"> <autogeneratedoc /></seealso>

[XenialCheckLicense]
public partial class ModelNavigationItemNodesGeneratorUpdater : ModelNodesGeneratorUpdater<NavigationItemNodeGenerator>
{
    /// <summary>   Gets the options. </summary>
    ///
    /// <value> The options. </value>

    protected virtual NavigationOptions Options { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelNavigationItemNodesGeneratorUpdater"/>
    /// class.
    /// </summary>
    ///
    /// <param name="options">  The options. </param>

    public ModelNavigationItemNodesGeneratorUpdater(NavigationOptions options)
        => Options = options ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// Updates the Application Model node content generated by the Nodes Generator, specified by the
    /// <see cref="T:DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater`1" /> class' type
    /// parameter.
    /// </summary>
    ///
    /// <param name="node"> A ModelNode Application Model node to be updated. </param>

    public override void UpdateNode(ModelNode node)
    {
        if (node is IModelRootNavigationItems modelRootNavigationItems)
        {
            modelRootNavigationItems.DefaultParentImageName = Options.DefaultParentImageName ??
                modelRootNavigationItems.DefaultParentImageName;

            modelRootNavigationItems.DefaultLeafImageName = Options.DefaultLeafImageName ??
                modelRootNavigationItems.DefaultLeafImageName;

            modelRootNavigationItems.NavigationStyle = Options.NavigationStyle ??
                modelRootNavigationItems.NavigationStyle;

            modelRootNavigationItems.DefaultChildItemsDisplayStyle = Options.DefaultChildItemsDisplayStyle ??
                modelRootNavigationItems.DefaultChildItemsDisplayStyle;

            modelRootNavigationItems.ShowImages = Options.ShowImages ??
                modelRootNavigationItems.ShowImages;

            if (Options.StartupNavigationItemId is not null)
            {
                modelRootNavigationItems.StartupNavigationItem = modelRootNavigationItems
                    .AllItems
                    .FirstOrDefault(m => m.Id == Options.StartupNavigationItemId);
            }

            if (Options.StartupNavigationItem is not null)
            {
                modelRootNavigationItems.StartupNavigationItem = modelRootNavigationItems
                    .AllItems
                    .FirstOrDefault(Options.StartupNavigationItem);
            }
        }
    }
}
