using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;

namespace Xenial.Framework.Tests.Layouts
{
    internal sealed class TestModule : XenialModuleBase
    {
        private readonly IEnumerable<Type> boModelTypes;
        private readonly Action<ITypesInfo>? customizeTypesInfo;

        /// <summary>   Gets or sets the customize generator updaters. </summary>
        ///
        /// <value> The customize generator updaters. </value>

        public Action<ModelNodesGeneratorUpdaters>? CustomizeGeneratorUpdaters { get; set; }

        internal TestModule(
            IEnumerable<Type> boModelTypes,
            Action<ITypesInfo>? customizeTypesInfo = null
        )
        {
            this.boModelTypes = boModelTypes;
            this.customizeTypesInfo = customizeTypesInfo;
        }

        /// <summary>   returns empty types. </summary>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the declared exported types in this
        /// collection.
        /// </returns>
        ///
        /// <seealso cref="Xenial.Framework.XenialModuleBase.GetDeclaredExportedTypes()"/>

        protected override IEnumerable<Type> GetDeclaredExportedTypes()
            => base.GetDeclaredExportedTypes()
                .Concat(boModelTypes);

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);
            customizeTypesInfo?.Invoke(typesInfo);
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);
            updaters.UseDetailViewLayoutBuilders();
            updaters.UseListViewColumnBuilders();

            CustomizeGeneratorUpdaters?.Invoke(updaters);
        }
    }
}
