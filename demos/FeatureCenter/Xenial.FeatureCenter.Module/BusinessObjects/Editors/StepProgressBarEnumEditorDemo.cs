using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

using Xenial.Framework.Base;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    [DomainComponent]
    [DevExpress.Persistent.Base.DefaultClassOptions]
    [Singleton(AutoCommit = true)]
    public partial class StepProgressBarEnumEditorDemo : NonPersistentBaseObject
    {
        private StepsEnum steps = StepsEnum.ShippingOptions;
        [EditorAlias("Xenial.StepProgressBarEnumPropertyEditor")]
        public StepsEnum Steps { get => steps; set => SetPropertyValue(ref steps, value); }
    }

    public enum StepsEnum
    {
        PersonalInfo = 0,
        ShippingOptions = 1,
        PaymentDetails = 2,
        Confirmation = 3
    }
}
