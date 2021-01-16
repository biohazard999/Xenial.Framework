
```cs
public class StepProgressBarEnumEditorDemo
{
     [StepProgressEnumEditor]
     public StepsEnum Steps { get; set; }
}
public enum StepsEnum
{
    [ImageName("Actions_User")]
    [XafDisplayName("Personal Info")]
    [DXDescription("Your name and email")]
    PersonalInfo,
    
    [ImageName("Shipment")]
    [XafDisplayName("Shipping Options")]
    [DXDescription("Shipping method and address")]
    ShippingOptions
}
```
