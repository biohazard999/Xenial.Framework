<div class='block'>
<h2 class='subtitle'>Basic</h2>

<div class="tabs-wrapper">
<div class="tabs">
<ul>
<li class="is-active"><a><span class="icon is-small"><i class="fas fa-code"></i></span><span>Attributes</span></a></li>
<li><a><span class="icon is-small"><i class="fas fa-project-diagram"></i></span><span>Model-Builders</span></a></li>
<li><a><span class="icon is-small"><i class="fas fa-tools"></i></span><span>Model-Editor</span></a></li>
</ul>
</div>
<div class="tabs-content">
<ul>
<li class="is-active">

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

</li>
<li>

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

</li>
<li>
Model-Editor Content
</li>
</ul>
</div>
</div>
</div>
<div class='block'>
<h2 class='subtitle'>Options</h2>

<div class="tabs-wrapper">
<div class="tabs">
<ul>
<li class="is-active"><a><span class="icon is-small"><i class="fas fa-code"></i></span><span>Attributes</span></a></li>
<li><a><span class="icon is-small"><i class="fas fa-project-diagram"></i></span><span>Model-Builders</span></a></li>
<li><a><span class="icon is-small"><i class="fas fa-tools"></i></span><span>Model-Editor</span></a></li>
</ul>
</div>
<div class="tabs-content">
<ul>
<li class="is-active">

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

</li>
<li>

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

</li>
<li>
Model-Editor Content
</li>
</ul>
</div>
</div>
</div>
