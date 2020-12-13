using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
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
        public TokenEditorPersistentDemo(Session session) : base(session) { }

        [Association]
        [TokenObjectsEditor]
        public XPCollection<TokenEditorPersistentTokens> TokenEditorPersistentTokens => GetCollection<TokenEditorPersistentTokens>();
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
