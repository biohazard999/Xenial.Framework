using System.ComponentModel;

using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

using Xenial.Framework.Images;

namespace Xenial.Framework.Deeplinks.Model;

/// <summary>
/// 
/// </summary>
public interface IModelOptionsJumplists : IModelNode
{
    /// <summary>
    /// 
    /// </summary>
    IModelJumplists Jumplists { get; }
}
