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

            var builder = ModelBuilder.Create<DemoTask>(typesInfo)
                .WithDefaultClassOptions()
                .HasCaption("Task");
            
            builder
                .For(m => m.Contacts)
                .HasTooltip("View, assign or remove contacts for the current task");

            builder.Build();
        }
    }
}
