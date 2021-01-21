using System;

using Xenial.Framework.StepProgressEditors.Model.GeneratorUpdaters;

namespace DevExpress.ExpressApp.Model.Core
{
    /// <summary>
    /// Class StepProgressBarEnumModelGeneratorUpdaterExtentions.
    /// </summary>
    public static partial class StepProgressBarEnumModelGeneratorUpdaterExtentions
    {
        /// <summary>
        /// Uses the detail view layout builders.
        /// </summary>
        /// <param name="updaters">The updaters.</param>
        /// <returns>ModelNodesGeneratorUpdaters.</returns>
#if DX_GT_20_2_4
        [Obsolete("Starting with DevExpress >= 20.2.4 you don't need this call anymore")]
#endif
        public static ModelNodesGeneratorUpdaters UseStepProgressEnumPropertyEditors(this ModelNodesGeneratorUpdaters updaters)
        {
            _ = updaters ?? throw new ArgumentNullException(nameof(updaters));

            updaters.Add(new StepProgressBarEnumModelGeneratorUpdater());

            return updaters;
        }
    }
}
