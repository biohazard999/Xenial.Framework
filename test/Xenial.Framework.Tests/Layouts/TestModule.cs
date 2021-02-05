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

        public Action<ModelNodesGeneratorUpdaters>? CustomizeGeneratorUpdaters { get; set; }

        internal TestModule(
            IEnumerable<Type> boModelTypes,
            Action<ITypesInfo>? customizeTypesInfo = null
        )
        {
            this.boModelTypes = boModelTypes;
            this.customizeTypesInfo = customizeTypesInfo;
        }

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
