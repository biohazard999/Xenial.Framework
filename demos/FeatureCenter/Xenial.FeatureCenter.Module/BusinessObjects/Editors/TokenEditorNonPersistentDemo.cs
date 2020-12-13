using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

using Xenial.Framework.Base;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    [DomainComponent]
    [DevExpress.Persistent.Base.DefaultClassOptions]
    [Singleton(AutoCommit = true)]
    public class TokenEditorNonPersistentDemo : NonPersistentBaseObject
    {
        [EditorAlias(Xenial.Framework.TokenEditors.EditorAliases.TokenObjectsPropertyEditor)]
        public BindingList<TokenEditorNonPersistentTokens> Tokens { get; set; } = new BindingList<TokenEditorNonPersistentTokens>();

        [TokenObjectsEditor]
        public BindingList<TokenEditorNonPersistentTokens> Tokens2 { get; set; } = new BindingList<TokenEditorNonPersistentTokens>();

        public BindingList<TokenEditorNonPersistentTokens> Tokens3 { get; set; } = new BindingList<TokenEditorNonPersistentTokens>();
    }

    [DomainComponent]
    public class TokenEditorNonPersistentTokens : NonPersistentBaseObject
    {
        private string? name;
        public string? Name { get => name; set => SetPropertyValue(ref name, value); }
    }
}
