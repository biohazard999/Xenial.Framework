using System;
using System.Collections.Generic;
using System.Linq;

namespace Xenial.Framework.Deeplinks
{

    /// <summary>
    /// 
    /// </summary>
    public sealed class DeeplinkQueryArgumentsEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arguments"></param>
        public DeeplinkQueryArgumentsEventArgs(IList<string> arguments)
            => Arguments = arguments;

        /// <summary>
        /// 
        /// </summary>
        public IList<string> Arguments { get; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public bool Handled { get; set; }
    }
}
