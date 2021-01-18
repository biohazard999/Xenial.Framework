using System;
using System.Linq;

using Xenial.Framework.StepProgressEditors.Model.GeneratorUpdaters;

namespace DevExpress.ExpressApp.Model.Core
{
    /// <summary>
    /// Class TokenObjectsEditorModelGeneratorUpdaterExtentions.
    /// </summary>
    public static partial class TokenObjectsEditorModelGeneratorUpdaterExtentions
    {
        /// <summary>
        /// Uses the detail view layout builders.
        /// </summary>
        /// <param name="updaters">The updaters.</param>
        /// <returns>ModelNodesGeneratorUpdaters.</returns>
        public static ModelNodesGeneratorUpdaters UseTokenObjectsPropertyEditors(this ModelNodesGeneratorUpdaters updaters)
        {
            _ = updaters ?? throw new ArgumentNullException(nameof(updaters));

            updaters.Add(new TokenObjectsEditorModelGeneratorUpdater());

            return updaters;
        }
    }
}
