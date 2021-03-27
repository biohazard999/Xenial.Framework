using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.DC;

using Xenial.Framework.ModelBuilders;

namespace MainDemo.Module.BusinessObjects
{
    public class MainDemoBuilderManager : XafBuilderManager
    {
        public MainDemoBuilderManager(ITypesInfo typesInfo)
            : base(typesInfo) { }

        public MainDemoBuilderManager(ITypesInfo typesInfo, IEnumerable<IBuilder> builders)
            : base(typesInfo, builders) { }

        protected override IEnumerable<IBuilder> GetBuilders() => new IBuilder[]
        {
            TypesInfo.CreateModelBuilder<DemoTaskModelBuilder>()
        };
    }
}
