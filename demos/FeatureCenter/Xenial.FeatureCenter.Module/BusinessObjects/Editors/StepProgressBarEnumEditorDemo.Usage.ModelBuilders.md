
```cs
public class StepProgressBarEnumEditorDemo
{
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
public class StepProgressBarEnumEditorDemoModelBuilder 
    : ModelBuilder<StepProgressBarEnumEditorDemo>
{
    public StepProgressBarEnumEditorDemoModelBuilder(ITypeInfo typeInfo) 
        : base(typeInfo) { }
    
    public override void Build()
    {
        base.Build();
        
        For(m => m.Steps)
            .UseStepProgressEnumPropertyEditor();
    }
}
```
