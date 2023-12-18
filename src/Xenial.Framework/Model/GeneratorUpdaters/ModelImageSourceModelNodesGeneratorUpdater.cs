using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

namespace Xenial.Framework.Model.GeneratorUpdaters;

/// <summary>
/// 
/// </summary>
[XenialCheckLicense]
public sealed partial class ModelImageSourceModelNodesGeneratorUpdater : ModelNodesGeneratorUpdater<ImageSourceNodesGenerator>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    public override void UpdateNode(ModelNode node)
    {
        if (node is IModelImageSources modelImageSources)
        {
            const string assemblyName = "Xenial.Framework.Images";
            var imageNode = modelImageSources.AddNode<IModelAssemblyResourceImageSource>(assemblyName);
            imageNode.AssemblyName = assemblyName;
            imageNode.Folder = "Images";
        }
    }
}
