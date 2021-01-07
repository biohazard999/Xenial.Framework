using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Xenial.Framework.Base;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    [DomainComponent]
    [DevExpress.Persistent.Base.DefaultClassOptions]
    [Singleton(AutoCommit = true)]
    public partial class StepProgressBarEnumEditorDemo : NonPersistentBaseObject
    {
        public override void OnCreated()
        {
            base.OnCreated();
            Steps = StepsEnum.ShippingOptions;
        }
        protected override void OnObjectSpaceChanged()
        {
            base.OnObjectSpaceChanged();
            if (ObjectSpace is NonPersistentObjectSpace nos)
            {
                nos.AutoSetModifiedOnObjectChange = true;
            }
        }

        private StepsEnum steps = StepsEnum.ShippingOptions;
        [EditorAlias("Xenial.StepProgressBarEnumPropertyEditor")]
        public StepsEnum Steps { get => steps; set => SetPropertyValue(ref steps, value); }

    }

    [Persistent]
    [DefaultClassOptions]
    [Singleton(AutoCommit = true)]
    public class StepProgressBarEnumEditorPersistentDemo : FeatureCenterBaseObjectId
    {
        public StepProgressBarEnumEditorPersistentDemo(Session session) : base(session) { }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            Steps = StepsEnum.ShippingOptions;
        }

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
