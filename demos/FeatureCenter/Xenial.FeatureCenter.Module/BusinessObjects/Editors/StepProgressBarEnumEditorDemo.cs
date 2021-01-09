
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;

using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using System;
using System.ComponentModel;

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

        private StepsEnum? nullableSteps;
        [EditorAlias("Xenial.StepProgressBarEnumPropertyEditor")]
        [DevExpress.ExpressApp.Model.ModelDefault("NullText", "Not Started")]
        public StepsEnum? NullableSteps { get => nullableSteps; set => SetPropertyValue(ref nullableSteps, value); }
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
            NormalSteps = StepsEnum.PaymentDetails;
            Steps = StepsEnum.ShippingOptions;
        }

        private StepsEnum normalSteps = StepsEnum.ShippingOptions;
        [ImmediatePostData]
        public StepsEnum NormalSteps
        {
            get => normalSteps;
            set
            {
                if (SetPropertyValue(ref normalSteps, value) && IsSaveForBusinessLogic)
                {
                    Steps = value;
                }
            }
        }

        private StepsEnum steps = StepsEnum.ShippingOptions;
        [EditorAlias("Xenial.StepProgressBarEnumPropertyEditor")]
        public StepsEnum Steps
        {
            get => steps;
            set
            {
                if (SetPropertyValue(ref steps, value) && IsSaveForBusinessLogic)
                {
                    NormalSteps = value;
                }
            }
        }

        private StepsEnum? nullableSteps;
        [EditorAlias("Xenial.StepProgressBarEnumPropertyEditor")]
        public StepsEnum? NullableSteps { get => nullableSteps; set => SetPropertyValue(ref nullableSteps, value); }
    }

    public enum StepsEnum
    {
        [ImageName("Actions_User")]
        [XafDisplayName("Personal Info")]
        [DXDescription("Your name and email")]
        PersonalInfo = 0,
        [ImageName("Shipment")]
        [XafDisplayName("Shipping Options")]
        [DXDescription("Shipping method and address")]
        ShippingOptions = 1,
        [ImageName("Business_CreditCard")]
        [XafDisplayName("Payment Details")]
        [DXDescription("Credit card or PayPal")]
        PaymentDetails = 2,
        [ImageName("Actions_Check")]
        [XafDisplayName("Confirmation")]
        [DXDescription("Confirm and pay")]
        Confirmation = 3
    }
}
