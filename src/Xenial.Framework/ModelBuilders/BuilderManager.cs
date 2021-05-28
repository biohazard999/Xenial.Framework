using System;
using System.Collections.Generic;

namespace Xenial.Framework.ModelBuilders
{
    /// <summary>   Manager for builders. </summary>
    ///
    /// <seealso cref="IBuilderManager"/>

    public class BuilderManager : IBuilderManager
    {
        private readonly List<IBuilder> builders = new();

        /// <summary>   Adds an builder. </summary>
        ///
        /// <param name="builder">  The builder. </param>
        ///
        /// <returns>   An IBuilderManager. </returns>

        public virtual IBuilderManager Add(IBuilder builder)
        {
            builders.Add(builder);
            return this;
        }

        /// <summary>   Adds the builders. </summary>
        ///
        /// <param name="builders"> The builders. </param>
        ///
        /// <returns>   An IBuilderManager. </returns>

        public virtual IBuilderManager Add(IEnumerable<IBuilder> builders)
        {
            this.builders.AddRange(builders);
            return this;
        }

        /// <summary>   Gets the builders. </summary>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the builders in this collection.
        /// </returns>

        protected virtual IEnumerable<IBuilder> GetBuilders() => builders;

        /// <summary>   Builds this instance. </summary>
        public virtual void Build()
        {
            foreach (var builder in GetBuilders())
            {
                BuildBuilder(builder);
            }
        }

        /// <summary>   Builds the builder. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="builder">  The builder. </param>

        protected virtual void BuildBuilder(IBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            builder.Build();
        }
    }
}
