using System;
using System.Linq;

using Bogus;

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
        private string? allowUserDefinedStringTokens;
        private string? dropDownShowModeOutlookStringTokens;
        private string? dropDownShowModeRegularStringTokens;
        private string? tokenPopupFilterModeContainsStringTokens;
        private string? tokenPopupFilterModeStartsWithStringTokens;

        public TokenStringEditorDemo(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            StringTokens = "Xenial.Framework;Xenial.Framework.Win;";

            DropDownShowModeOutlookStringTokens = PickRandomDemoTokens();
            DropDownShowModeRegularStringTokens = PickRandomDemoTokens();
            TokenPopupFilterModeContainsStringTokens = PickRandomDemoTokens();
            TokenPopupFilterModeStartsWithStringTokens = PickRandomDemoTokens();
        }

        [TokenStringEditor]
        [ModelDefault(ModelDefaults.PredefinedValues, "Xenial.Framework;Xenial.Framework.Win;Xenial.Framework.TokenEditors;Xenial.Framework.TokenEditors.Win;Xenial.Framework.WebView.Win;Xenial.Framework.WebView")]
        [Size(SizeAttribute.Unlimited)]
        public string? StringTokens { get => stringTokens; set => SetPropertyValue(ref stringTokens, value); }

        [TokenStringEditor(AllowUserDefinedTokens = true)]
        [Size(SizeAttribute.Unlimited)]
        public string? AllowUserDefinedStringTokens { get => allowUserDefinedStringTokens; set => SetPropertyValue(ref allowUserDefinedStringTokens, value); }

        #region TokenDropDownShowMode

        [TokenStringEditor(TokenDropDownShowMode.Outlook)]
        [Size(SizeAttribute.Unlimited)]
        public string? DropDownShowModeOutlookStringTokens { get => dropDownShowModeOutlookStringTokens; set => SetPropertyValue(ref dropDownShowModeOutlookStringTokens, value); }

        [TokenStringEditor(TokenDropDownShowMode.Regular)]
        [Size(SizeAttribute.Unlimited)]
        public string? DropDownShowModeRegularStringTokens { get => dropDownShowModeRegularStringTokens; set => SetPropertyValue(ref dropDownShowModeRegularStringTokens, value); }

        #endregion

        #region TokenPopupFilterMode

        [TokenStringEditor(TokenPopupFilterMode.Contains)]
        [Size(SizeAttribute.Unlimited)]
        public string? TokenPopupFilterModeContainsStringTokens { get => tokenPopupFilterModeContainsStringTokens; set => SetPropertyValue(ref tokenPopupFilterModeContainsStringTokens, value); }

        [TokenStringEditor(TokenPopupFilterMode.StartsWith)]
        [Size(SizeAttribute.Unlimited)]
        public string? TokenPopupFilterModeStartsWithStringTokens { get => tokenPopupFilterModeStartsWithStringTokens; set => SetPropertyValue(ref tokenPopupFilterModeStartsWithStringTokens, value); }

        #endregion
    }
}
