using System;
using System.Collections.Generic;

namespace Xenial.Framework.ModelBuilders
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IBuilderManager" />
    public class BuilderManager : IBuilderManager
    {
        private readonly List<IBuilder> builders = new();

        /// <summary>
        /// Adds an builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public virtual IBuilderManager Add(IBuilder builder)
        {
            builders.Add(builder);
            return this;
        }

        /// <summary>
        /// Adds the builders.
        /// </summary>
        /// <param name="builders">The builders.</param>
        /// <returns></returns>
        public virtual IBuilderManager Add(IEnumerable<IBuilder> builders)
        {
            this.builders.AddRange(builders);
            return this;
        }

        /// <summary>
        /// Gets the builders.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<IBuilder> GetBuilders() => builders;

        /// <summary>
        /// Builds this instance.
        /// </summary>
        public virtual void Build()
        {
            foreach (var builder in GetBuilders())
            {
                BuildBuilder(builder);
            }
        }

        /// <summary>
        /// Builds the builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected virtual void BuildBuilder(IBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            builder.Build();
        }
    }
}
