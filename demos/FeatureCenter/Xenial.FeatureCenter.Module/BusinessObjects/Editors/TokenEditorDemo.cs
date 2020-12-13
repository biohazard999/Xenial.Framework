using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;

using Xenial.Framework.Base;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    [DomainComponent]
    [DevExpress.Persistent.Base.DefaultClassOptions]
    [Singleton(AutoCommit = true)]
    public class TokenEditorDemo : NonPersistentBaseObject
    {
        public BindingList<TokenEditorDemoTokens> Tokens { get; set; } = new BindingList<TokenEditorDemoTokens>();
    }

    [DomainComponent]
    public class TokenEditorDemoTokens : NonPersistentBaseObject
    {
        private string? name;
        public string? Name { get => name; set => SetPropertyValue(ref name, value); }
    }
}
