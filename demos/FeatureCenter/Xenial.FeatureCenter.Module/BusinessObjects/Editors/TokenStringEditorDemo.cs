using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using System;
using System.Collections.Generic;

using Xenial.Framework.Base;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    [Persistent]
    public partial class TokenStringEditorDemo : FeatureCenterDemoBaseObjectId
    {
        private string? tokenEditorStringTokens;

        public TokenStringEditorDemo(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            StringTokens = "Xenial.Framework;Xenial.Framework.Win;";
        }

        //[Association]
        //[TokenObjectsEditor]
        //[ImmediatePostData]
        //public XPCollection<TokenEditorPersistentTokens> TokenEditorPersistentTokens => GetCollection<TokenEditorPersistentTokens>();

        [TokenStringEditor]
        [ModelDefault("PredefinedValues", "Xenial.Framework;Xenial.Framework.Win;Xenial.Framework.TokenEditors;Xenial.Framework.TokenEditors.Win;Xenial.Framework.WebView.Win;Xenial.Framework.WebView")]
        [Size(SizeAttribute.Unlimited)]
        public string? StringTokens { get => tokenEditorStringTokens; set => SetPropertyValue(ref tokenEditorStringTokens, value); }
    }

    //[Persistent]
    //public class TokenEditorPersistentTokens : FeatureCenterBaseObjectId
    //{
    //    private string? name;

    //    public TokenEditorPersistentTokens(Session session) : base(session) { }

    //    public string? Name { get => name; set => SetPropertyValue(ref name, value); }

    //    [Association]
    //    public XPCollection<TokenStringEditorDemo> TokenEditorPersistentDemos => GetCollection<TokenStringEditorDemo>();
    //}
}
