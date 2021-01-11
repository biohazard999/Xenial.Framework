using System;
using System.ComponentModel;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;

using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;


using Xenial.Framework.Base;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    [DomainComponent]
    [DevExpress.Persistent.Base.DefaultClassOptions]
    [Singleton(AutoCommit = true)]
    public class TokenEditorNonPersistentDemo : NonPersistentBaseObject
    {
        private string? tokenEditorStringTokens;

        public override void OnCreated()
        {
            base.OnCreated();
            TokenEditorStringTokens = "Xenial.Framework;Xenial.Framework.Win;";
        }

        [TokenObjectsEditor]
        public BindingList<TokenEditorNonPersistentTokens> Tokens { get; set; } = new BindingList<TokenEditorNonPersistentTokens>();

        [TokenStringEditor]
        [ModelDefault("PredefinedValues", "Xenial.Framework;Xenial.Framework.Win;Xenial.Framework.TokenEditors;Xenial.Framework.TokenEditors.Win;Xenial.Framework.WebView.Win;Xenial.Framework.WebView")]
        public string? TokenEditorStringTokens { get => tokenEditorStringTokens; set => SetPropertyValue(ref tokenEditorStringTokens, value); }
    }

    [DomainComponent]
    public class TokenEditorNonPersistentTokens : NonPersistentBaseObject
    {
        private string? name;
        public string? Name { get => name; set => SetPropertyValue(ref name, value); }
    }
}
