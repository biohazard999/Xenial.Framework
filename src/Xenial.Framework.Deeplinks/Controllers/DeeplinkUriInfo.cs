using DevExpress.ExpressApp;

using System;
using System.Linq;

using Xenial.Framework.Deeplinks.Model;

namespace Xenial.Framework.Deeplinks
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Application"></param>
    /// <param name="Protocol"></param>
    /// <param name="Uri"></param>
    public record DeeplinkUriInfo(XafApplication Application, IModelDeeplinkProtocol Protocol, Uri Uri)
    {
        /// <summary>
        /// 
        /// </summary>
        public string ProtocolName => Protocol.ProtocolName;
        /// <summary>
        /// 
        /// </summary>
        public string Verb => Uri.Host;
        /// <summary>
        /// 
        /// </summary>
        public string Query => Uri.Query;
        /// <summary>
        /// 
        /// </summary>
        public string Route => Uri.LocalPath.TrimStart('/');
    }
}
