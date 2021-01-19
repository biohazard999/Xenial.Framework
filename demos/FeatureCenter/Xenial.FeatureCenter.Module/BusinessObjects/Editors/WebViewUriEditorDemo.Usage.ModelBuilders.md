
```cs
public class WebViewUriEditorDemo
{
    public Uri Uri { get; set; }
}
public class WebViewUriEditorDemoModelBuilder 
    : ModelBuilder<WebViewUriEditorDemo>
{
    public WebViewUriEditorDemoModelBuilder(ITypeInfo typeInfo) 
        : base(typeInfo) { }
    
    public override void Build()
    {
        base.Build();
        
        For(m => m.Uri)
            .UseWebViewUriPropertyEditor();
    }
}
```
