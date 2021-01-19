using System;

using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Xenial.Framework.ModelBuilders;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    [Persistent]
    public partial class TokenStringEditorDemo : FeatureCenterEditorsBaseObject
    {
        private string? stringTokens;

        public TokenStringEditorDemo(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            StringTokens = "Xenial.Framework;Xenial.Framework.Win;";
        }

        [TokenStringEditor]
        [ModelDefault(ModelDefaults.PredefinedValues, "Xenial.Framework;Xenial.Framework.Win;Xenial.Framework.TokenEditors;Xenial.Framework.TokenEditors.Win;Xenial.Framework.WebView.Win;Xenial.Framework.WebView")]
        [Size(SizeAttribute.Unlimited)]
        public string? StringTokens { get => stringTokens; set => SetPropertyValue(ref stringTokens, value); }
    }
}
