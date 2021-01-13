using System.ComponentModel;

using Bogus;

using DevExpress.ExpressApp.DC;

using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    [Persistent]
    public partial class StepProgressBarEnumEditorPersistentDemo : FeatureCenterDemoBaseObjectId
    {
        public StepProgressBarEnumEditorPersistentDemo(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            NormalSteps = StepsEnum.PaymentDetails;
            Steps = StepsEnum.ShippingOptions;

            //Generate Random Values for the Demo
            WithoutDescription = new Faker().Random.Enum<StepsEnumWithoutDescription>();
            WithoutImages = new Faker().Random.Enum<StepsEnumWithoutImages>();
            CaptionOnly = new Faker().Random.Enum<StepsEnumCaptionOnly>();
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
        //Link NormalSteps and Steps together. When you change one value, the other will also change.
        //Demonstrates the interactive editor. You can provide additional business logic
        //when the step changes
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

        private StepsEnumWithoutDescription withoutDescription;
        [StepProgressEnumEditor]
        public StepsEnumWithoutDescription WithoutDescription { get => withoutDescription; set => SetPropertyValue(ref withoutDescription, value); }

        private StepsEnumWithoutImages withoutImages;
        [StepProgressEnumEditor]
        public StepsEnumWithoutImages WithoutImages { get => withoutImages; set => SetPropertyValue(ref withoutImages, value); }

        private StepsEnumCaptionOnly captionOnly;
        [StepProgressEnumEditor]
        public StepsEnumCaptionOnly CaptionOnly { get => captionOnly; set => SetPropertyValue(ref captionOnly, value); }
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

    public enum StepsEnumWithoutDescription
    {
        [ImageName("Actions_User")]
        [XafDisplayName("Personal Info")]
        PersonalInfo = 0,
        [ImageName("Shipment")]
        [XafDisplayName("Shipping Options")]
        ShippingOptions = 1,
        [ImageName("Business_CreditCard")]
        [XafDisplayName("Payment Details")]
        PaymentDetails = 2,
        [ImageName("Actions_Check")]
        [XafDisplayName("Confirmation")]
        Confirmation = 3
    }

    public enum StepsEnumWithoutImages
    {
        [XafDisplayName("Personal Info")]
        [DXDescription("Your name and email")]
        PersonalInfo = 0,
        [XafDisplayName("Shipping Options")]
        [DXDescription("Shipping method and address")]
        ShippingOptions = 1,
        [XafDisplayName("Payment Details")]
        [DXDescription("Credit card or PayPal")]
        PaymentDetails = 2,
        [XafDisplayName("Confirmation")]
        [DXDescription("Confirm and pay")]
        Confirmation = 3
    }

    public enum StepsEnumCaptionOnly
    {
        [XafDisplayName("Personal Info")]
        PersonalInfo = 0,
        [XafDisplayName("Shipping Options")]
        ShippingOptions = 1,
        [XafDisplayName("Payment Details")]
        PaymentDetails = 2,
        [XafDisplayName("Confirmation")]
        Confirmation = 3
    }
}
