using System;
using System.Linq;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;

using Xenial.Framework.TokenEditors.Model;
using Xenial.Framework.TokenEditors.Win.Editors;

namespace Xenial.Framework.TokenEditors.Win.Editors
{
    /// <summary>
    /// Class TokenStringPropertyEditor. Implements the
    /// <see cref="DevExpress.ExpressApp.Win.Editors.DXPropertyEditor" />
    /// </summary>
    ///
    /// <seealso cref="DXPropertyEditor"/>
    /// <seealso cref="DevExpress.ExpressApp.Win.Editors.DXPropertyEditor"/>

    [XenialCheckLicence]
    public sealed partial class TokenStringPropertyEditor : DXPropertyEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenStringPropertyEditor"/> class.
        /// </summary>
        ///
        /// <param name="objectType">   Type of the object. </param>
        /// <param name="model">        The model. </param>

        public TokenStringPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
            => ImmediatePostData = true; //We only support immediate post data for now.

        /// <summary>   Creates the repository item. </summary>
        ///
        /// <returns>   RepositoryItem. </returns>

        protected override RepositoryItem CreateRepositoryItem() => ApplyModelOptions(new RepositoryItemTokenEdit());

        private RepositoryItemTokenEdit ApplyModelOptions(RepositoryItemTokenEdit tokenEdit)
        {
            if (Model is IXenialTokenStringModelPropertyEditor model)
            {
                tokenEdit.DropDownShowMode = model.XenialTokenStringDropDownShowMode switch
                {
                    TokenDropDownShowMode.Default => TokenEditDropDownShowMode.Default,
                    TokenDropDownShowMode.Regular => TokenEditDropDownShowMode.Regular,
                    TokenDropDownShowMode.Outlook => TokenEditDropDownShowMode.Outlook,
                    _ => tokenEdit.DropDownShowMode
                };

                tokenEdit.PopupFilterMode = model.XenialTokenStringPopupFilterMode switch
                {
                    TokenPopupFilterMode.StartsWith => TokenEditPopupFilterMode.StartWith,
                    TokenPopupFilterMode.Contains => TokenEditPopupFilterMode.Contains,
                    _ => tokenEdit.PopupFilterMode
                };

                if (model.XenialTokenStringAllowUserDefinedTokens == true)
                {
                    tokenEdit.ValidateToken -= TokenEdit_ValidateToken;
                    tokenEdit.ValidateToken += TokenEdit_ValidateToken;

                    void TokenEdit_ValidateToken(object? _, TokenEditValidateTokenEventArgs e)
                    {
                        if (!string.IsNullOrEmpty(e.Description))
                        {
                            e.Description = e.Description.Trim();
                            e.IsValid = true;
                        }
                    }
                }
            }
            return tokenEdit;
        }

        /// <summary>   Creates the control core. </summary>
        ///
        /// <returns>   System.Object. </returns>

        protected override object CreateControlCore()
        {
            var edit = new TokenEdit();
            edit.ValidateToken -= Edit_ValidateToken;
            edit.ValidateToken += Edit_ValidateToken;
            return edit;
        }

        /// <summary>   Breaks the links to control. </summary>
        ///
        /// <param name="unwireEventsOnly"> The unwire events only. </param>

        public override void BreakLinksToControl(bool unwireEventsOnly)
        {
            if (Control is not null)
            {
                Control.ValidateToken -= Edit_ValidateToken;
            }
            base.BreakLinksToControl(unwireEventsOnly);
        }

        private void Edit_ValidateToken(object sender, TokenEditValidateTokenEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Description))
            {
                e.Description = e.Description.Trim();
                e.IsValid = true;
            }
        }

        /// <summary>
        /// Provides access to the control that represents the current Property Editor in a UI.
        /// </summary>
        ///
        /// <value>
        /// A DevExpress.XtraEditors.BaseEdit object representing a control used to display the current
        /// Property Editor in a UI.
        /// </value>

        public new TokenEdit Control => (TokenEdit)base.Control;

        /// <summary>   Called when [current object changed]. </summary>
        protected override void OnCurrentObjectChanged()
        {
            base.OnCurrentObjectChanged();
            _ = true;
        }

        /// <summary>   Reads the value core. </summary>
        protected override void ReadValueCore()
        {
            if (CurrentObject is not null && Control is not null && !IsDisposed)
            {
                InitTokens(Control.Properties);
            }
            base.ReadValueCore();
            _ = true;
        }

        /// <summary>   Setups the repository item. </summary>
        ///
        /// <param name="item"> The item. </param>

        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            if (item is RepositoryItemTokenEdit tokenEdit)
            {
                tokenEdit.BeginUpdate();
                try
                {
                    ApplyModelOptions(tokenEdit);
                    tokenEdit.EditValueType = TokenEditValueType.String;
                    tokenEdit.ShowDropDown = true;
                    tokenEdit.EditMode = TokenEditMode.Manual;

                    tokenEdit.EditValueSeparatorChar = ';';
                    tokenEdit.Separators.Clear();
                    tokenEdit.Separators.Add(";");
                    tokenEdit.Separators.Add(",");

                    InitTokens(tokenEdit);
                }
                finally
                {
                    tokenEdit.EndUpdate();
                }


            }
        }

        private void InitTokens(RepositoryItemTokenEdit tokenEdit)
        {
            InitTokensPredefinedValues(tokenEdit);

            if (CurrentObject is not null)
            {
                var currentValue = MemberInfo.GetValue(CurrentObject);
                if (currentValue is string currentValueString)
                {
                    InitTokensEditValue(tokenEdit, currentValueString);
                }
            }
        }

        private void InitTokensEditValue(RepositoryItemTokenEdit tokenEdit, string value)
        {
            tokenEdit.Tokens.BeginUpdate();
            try
            {
                foreach (var val in
                    value.Split(';')
                        .Where(v => !string.IsNullOrEmpty(v))
                        .Select(v => v.Trim())
                    )
                {
                    if (!tokenEdit.Tokens.Any(t => t.Description == val))
                    {
                        tokenEdit.Tokens.AddToken(val);
                    }
                }
            }
            finally
            {
                tokenEdit.Tokens.EndUpdate();
            }
        }

        private void InitTokensPredefinedValues(RepositoryItemTokenEdit tokenEdit)
        {
            tokenEdit.Tokens.BeginUpdate();
            try
            {
                if (Model is not null && !string.IsNullOrEmpty(Model.PredefinedValues))
                {
                    foreach (var predefinedValue in
                        Model.PredefinedValues
                            .Split(';')
                            .Where(v => !string.IsNullOrEmpty(v))
                            .Select(v => v.Trim())
                        )
                    {
                        if (!tokenEdit.Tokens.Any(t => t.Description == predefinedValue))
                        {
                            tokenEdit.Tokens.AddToken(predefinedValue);
                        }
                    }
                }
            }
            finally
            {
                tokenEdit.Tokens.EndUpdate();
            }
        }
    }
}

namespace DevExpress.ExpressApp.Editors
{
    /// <summary>   Class P. </summary>
    public static class TokenStringPropertyEditorWinExtensions
    {
        /// <summary>   Uses the token objects property editor. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>
        ///
        /// <returns>   EditorDescriptorsFactory. </returns>

        public static EditorDescriptorsFactory UseTokenStringPropertyEditorsWin(this EditorDescriptorsFactory editorDescriptorsFactory)
        {
            _ = editorDescriptorsFactory ?? throw new ArgumentNullException(nameof(editorDescriptorsFactory));

            editorDescriptorsFactory.RegisterPropertyEditor(
                Xenial.Framework.TokenEditors.PubTernal.TokenEditorAliases.TokenStringPropertyEditor,
                typeof(string),
                typeof(TokenStringPropertyEditor),
                false
            );

            return editorDescriptorsFactory;
        }
    }
}
