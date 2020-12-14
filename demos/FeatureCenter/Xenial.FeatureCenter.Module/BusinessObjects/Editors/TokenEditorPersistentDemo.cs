using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Xenial.Framework.Base;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    [Persistent]
    [DefaultClassOptions]
    [Singleton(AutoCommit = true)]
    public class TokenEditorPersistentDemo : FeatureCenterBaseObjectId
    {
        private string? tokenEditorStringTokens;

        public TokenEditorPersistentDemo(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            TokenEditorStringTokens = "Xenial.Framework;Xenial.Framework.Win;";
        }

        [Association]
        [TokenObjectsEditor]
        public XPCollection<TokenEditorPersistentTokens> TokenEditorPersistentTokens => GetCollection<TokenEditorPersistentTokens>();

        [TokenStringEditor]
        [ModelDefault("PredefinedValues", "Xenial.Framework;Xenial.Framework.Win;Xenial.Framework.TokenEditors;Xenial.Framework.TokenEditors.Win;Xenial.Framework.WebView.Win;Xenial.Framework.WebView")]
        public string? TokenEditorStringTokens { get => tokenEditorStringTokens; set => SetPropertyValue(ref tokenEditorStringTokens, value); }
    }

    [Persistent]
    public class TokenEditorPersistentTokens : FeatureCenterBaseObjectId
    {
        private string? name;

        public TokenEditorPersistentTokens(Session session) : base(session) { }

        public string? Name { get => name; set => SetPropertyValue(ref name, value); }

        [Association]
        public XPCollection<TokenEditorPersistentDemo> TokenEditorPersistentDemos => GetCollection<TokenEditorPersistentDemo>();
    }
}
