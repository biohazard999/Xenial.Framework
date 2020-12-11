using System;
using System.Collections.Generic;
using System.Linq;

namespace Xenial.Framework.ModelBuilders
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IBuilder" />
    public interface IBuilderManager : IBuilder
    {
        /// <summary>
        /// Adds the builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        IBuilderManager Add(IBuilder builder);

        /// <summary>
        /// Adds multiple builders.
        /// </summary>
        /// <param name="builders">The builder.</param>
        /// <returns></returns>
        IBuilderManager Add(IEnumerable<IBuilder> builders);
    }
}
