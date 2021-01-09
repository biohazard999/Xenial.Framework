using System.ComponentModel;

using DevExpress.ExpressApp.DC;

using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Xenial.Framework.Base;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
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
        [StepProgressEnumEditor]
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
        [StepProgressEnumEditor]
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
