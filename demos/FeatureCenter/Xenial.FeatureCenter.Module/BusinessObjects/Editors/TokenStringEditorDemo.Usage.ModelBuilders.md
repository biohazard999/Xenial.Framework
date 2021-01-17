
```cs
public class TokenStringEditorDemo
{
    public string Tokens { get; set; }
}
public class TokenStringEditorDemoModelBuilder 
    : ModelBuilder<TokenStringEditorDemo>
{
    public TokenStringEditorDemoModelBuilder(ITypeInfo typeInfo) 
        : base(typeInfo) { }
    
    public override void Build()
    {
        base.Build();
        
        For(m => m.Tokens)
            .WithPredefinedValues(new[]
            {
                "Value1",
                "Value2"
            })
            .UseTokenStringPropertyEditor();
    }
}
```
