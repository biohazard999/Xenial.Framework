using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

using Xenial.Framework.Deeplinks.Model;

namespace Xenial.Framework.Deeplinks.Generators;

/// <summary>
/// 
/// </summary>
[XenialModelOptions(typeof(IModelDeeplinkProtocols), IgnoredMembers = new[]
{
    nameof(IModelDeeplinkProtocols.Index),
})]
public partial record ModelDeeplinkProtocols
{
}

/// <summary>
/// 
/// </summary>
[XenialModelOptionsMapper(typeof(ModelDeeplinkProtocols))]
public sealed partial class ModelDeepLinkOptionsGeneratorUpdaters : ModelNodesGeneratorUpdater<ModelOptionsNodesGenerator>
{
    private readonly ModelDeeplinkProtocols options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public ModelDeepLinkOptionsGeneratorUpdaters(ModelDeeplinkProtocols options)
        => this.options = options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    public override void UpdateNode(ModelNode node)
    {
        if (node is IModelOptions modelOptions && modelOptions is IModelOptionsDeeplinkProtocols modelDeeplinkProtocols)
        {
            MapNode(options, modelDeeplinkProtocols.DeeplinkProtocols);
        }
    }
}


/// <summary>
/// 
/// </summary>
[XenialModelOptionsMapper(typeof(ModelDeeplinkProtocol))]
public sealed partial class ModelDeepLinkGeneratorUpdaters : ModelNodesGeneratorUpdater<ModelDeeplinkProtocolsGenerator>
{
    private readonly ModelDeeplinkProtocol[] options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public ModelDeepLinkGeneratorUpdaters(params ModelDeeplinkProtocol[] options)
        => this.options = options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    public override void UpdateNode(ModelNode node)
    {
        if (node is IModelDeeplinkProtocols modelDeeplinkProtocols)
        {
            foreach (var option in options.OrderBy(m => m.Index))
            {
                var protocolNode = modelDeeplinkProtocols.AddNode<IModelDeeplinkProtocol>(option.ProtocolName);
                MapNode(option, protocolNode);
            }
        }
    }
}

/// <summary>
/// 
/// </summary>
[XenialModelOptions(typeof(IModelDeeplinkProtocol))]
public partial record ModelDeeplinkProtocol
{
}

/// <summary>
/// 
/// </summary>
public sealed class ModelDeeplinkProtocolsGenerator : ModelNodesGeneratorBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    protected override void GenerateNodesCore(ModelNode node) { }
}
