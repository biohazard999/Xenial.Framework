using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    [Persistent]
    public partial class TokenObjectsEditorDemo : FeatureCenterDemoBaseObjectId
    {
        public TokenObjectsEditorDemo(Session session) : base(session) { }

        [Association]
        [TokenObjectsEditor]
        public XPCollection<TokenObjectsEditorDemoTokens> Tokens => GetCollection<TokenObjectsEditorDemoTokens>();

        //[Association]
        //[TokenObjectsEditor]
        //[ImmediatePostData]
        //public XPCollection<TokenObjectsEditorDemoTokens> TokensImmediatePostData => GetCollection<TokenObjectsEditorDemoTokens>();
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
}
