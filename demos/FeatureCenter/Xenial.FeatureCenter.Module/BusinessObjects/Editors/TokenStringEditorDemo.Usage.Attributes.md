
```cs
public class TokenStringEditorDemo
{
    [TokenStringEditor]
    [ModelDefault(ModelDefaults.PredefinedValues, "Value1;Value2")]
    public string Tokens { get; set; }
}
```
