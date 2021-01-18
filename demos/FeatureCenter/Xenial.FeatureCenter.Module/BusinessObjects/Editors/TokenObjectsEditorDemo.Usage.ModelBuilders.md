
```cs
[Persistent]
public class TokenObjectsEditorDemo : FeatureCenterDemoBaseObjectId
{
    public TokenObjectsEditorDemo(Session session) : base(session) { }
    
    [Association]
    public XPCollection<TokenObjectsEditorDemoTokens> Tokens => GetCollection<TokenObjectsEditorDemoTokens>();
}
[Persistent]
public class TokenObjectsEditorDemoTokens : FeatureCenterBaseObjectId
{
    private string? name;
    
    public TokenObjectsEditorDemoTokens(Session session) : base(session) { }
    
    public string? Name { get => name; set => SetPropertyValue(ref name, value); }
    
    [Association]
    public XPCollection<TokenObjectsEditorDemo> TokenObjectsEditorDemos => GetCollection<TokenObjectsEditorDemo>();
}
public class TokenObjectsEditorDemoModelBuilder 
    : ModelBuilder<TokenObjectsEditorDemo>
{
    public TokenObjectsEditorDemoModelBuilder(ITypeInfo typeInfo) 
        : base(typeInfo) { }
    
    public override void Build()
    {
        base.Build();
        
        For(m => m.Tokens)
            .UseTokenObjectsPropertyEditor();
    }
}
```
