using System;

using Xenial.Framework.Model.GeneratorUpdaters;

namespace DevExpress.ExpressApp.Model.Core
{
    /// <summary>
    /// Class ModelNodesGeneratorUpdatersExtentions.
    /// </summary>
    public static partial class ModelNodesGeneratorUpdaterLayoutBuilderExtentions
    {
        /// <summary>
        /// Uses the navigation options.
        /// </summary>
        /// <param name="updaters">The updaters.</param>
        /// <param name="options">The options.</param>
        /// <returns>ModelNodesGeneratorUpdaters.</returns>
        public static ModelNodesGeneratorUpdaters UseNavigationOptions(this ModelNodesGeneratorUpdaters updaters, NavigationOptions options)
        {
            _ = updaters ?? throw new ArgumentNullException(nameof(updaters));
            _ = options ?? throw new ArgumentNullException(nameof(options));
            updaters.Add(new ModelNavigationItemNodesGeneratorUpdater(options));
            return updaters;
        }

        /// <summary>
        /// Uses the application options.
        /// </summary>
        /// <param name="updaters">The updaters.</param>
        /// <param name="options">The options.</param>
        /// <returns>ModelNodesGeneratorUpdaters.</returns>
        public static ModelNodesGeneratorUpdaters UseNavigationOptions(this ModelNodesGeneratorUpdaters updaters, Func<NavigationOptions, NavigationOptions> options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            return updaters.UseNavigationOptions(options(new()));
        }
    }
}
