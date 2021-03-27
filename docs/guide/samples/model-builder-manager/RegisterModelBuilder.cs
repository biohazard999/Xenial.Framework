using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;

using Xenial.Framework;
using Xenial.Framework.ModelBuilders;

using MainDemo.Module.BusinessObjects;

namespace MyApplication.Module
{
    public class MyApplicationModule : ModuleBase
    {
        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);

            new MainDemoBuilderManager(typesInfo)
                .Build();
        }
    }
}
