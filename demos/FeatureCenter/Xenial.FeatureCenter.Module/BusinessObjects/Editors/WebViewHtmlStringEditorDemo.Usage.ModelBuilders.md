
```cs
public class WebViewHtmlStringEditorDemo
{
    public string HtmlContent { get; set; } = "<html><head></head><body><h1>Hello from WebViewHtmlStringEditor</h1></body></html>";
}
public class WebViewHtmlStringEditorDemoModelBuilder 
    : ModelBuilder<WebViewHtmlStringEditorDemo>
{
    public WebViewHtmlStringEditorDemoModelBuilder(ITypeInfo typeInfo) 
        : base(typeInfo) { }
    
    public override void Build()
    {
        base.Build();
        
        For(m => m.HtmlContent)
            .UseWebViewHtmlStringPropertyEditor();
    }
}
```
