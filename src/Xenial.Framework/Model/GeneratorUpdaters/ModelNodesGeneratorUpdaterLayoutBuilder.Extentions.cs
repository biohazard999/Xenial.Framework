using System;

using Xenial.Framework.Model.GeneratorUpdaters;

namespace DevExpress.ExpressApp.Model.Core
{
    /// <summary>
    /// Class ModelNodesGeneratorUpdaterLayoutBuilderExtentions.
    /// </summary>
    public static partial class ModelNodesGeneratorUpdaterLayoutBuilderExtentions
    {
        /// <summary>
        /// Uses the detail view layout builders.
        /// </summary>
        /// <param name="updaters">The updaters.</param>
        /// <returns>ModelNodesGeneratorUpdaters.</returns>
        public static ModelNodesGeneratorUpdaters UseDetailViewLayoutBuilders(this ModelNodesGeneratorUpdaters updaters)
        {
            _ = updaters ?? throw new ArgumentNullException(nameof(updaters));

            updaters.Add(new ModelNodesGeneratorUpdaterLayoutBuilder());
            updaters.Add(new ModelDetailViewLayoutNodesGeneratorUpdater());

            return updaters;
        }
    }
}
