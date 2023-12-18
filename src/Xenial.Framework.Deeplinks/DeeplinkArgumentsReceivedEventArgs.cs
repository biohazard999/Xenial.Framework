using System;
using System.Collections.Generic;
using System.Linq;

namespace Xenial.Framework.Deeplinks
{

    /// <summary>
    ///     Holds a list of arguments given to an application at startup.
    /// </summary>
    public sealed class DeeplinkArgumentsReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public IList<string> Arguments { get; } = new List<string>();
    }
}
