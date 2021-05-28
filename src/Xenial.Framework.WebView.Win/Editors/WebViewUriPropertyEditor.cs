﻿using System;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;

using Xenial.Framework.WebView.Win.Editors;
using Xenial.Framework.WebView.Win.Helpers;

namespace Xenial.Framework.WebView.Win.Editors
{
    /// <summary>
    /// Class WebViewUriPropertyEditor. Implements the
    /// <see cref="DevExpress.ExpressApp.Win.Editors.WinPropertyEditor" />
    /// </summary>
    ///
    /// <seealso cref="WinPropertyEditor"/>
    /// <seealso cref="DevExpress.ExpressApp.Win.Editors.WinPropertyEditor">    <autogeneratedoc /></seealso>

    [XenialCheckLicence]
    public sealed partial class WebViewUriPropertyEditor : WinPropertyEditor
    {
        /// <summary>
        /// Indicates whether the caption of the current Property Editor should be visible in a UI.
        /// </summary>
        ///
        /// <value> true if the current Property's caption is visible; otherwise, false. </value>

        public override bool IsCaptionVisible => false;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebViewUriPropertyEditor"/> class.
        /// </summary>
        ///
        /// <param name="objectType">   Type of the object. </param>
        /// <param name="model">        The model. </param>

        public WebViewUriPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
        {
            ControlBindingProperty = nameof(Control.Source);
            ControlCreated -= WebViewUriPropertyEditor_ControlCreated;
            ControlCreated += WebViewUriPropertyEditor_ControlCreated;
        }

        /// <summary>   Determines whether [is member setter required]. </summary>
        ///
        /// <returns>   <c>true</c> if [is member setter required]; otherwise, <c>false</c>. </returns>

        protected override bool IsMemberSetterRequired() => false;

        private async void WebViewUriPropertyEditor_ControlCreated(object? sender, EventArgs e)
            => await Control.EnsureCoreWebView2AndInstallAsync().ConfigureAwait(false);


        protected override void Dispose(bool disposing)
        {
            ControlCreated -= WebViewUriPropertyEditor_ControlCreated;
            if (Control is not null && !Control.IsDisposed && !Control.Disposing)
            {
                //Fixes https://github.com/MicrosoftEdge/WebView2Feedback/issues/228
                //Hide before dispose or WebView throws NRE
                Control.Visible = false;
                Control.Parent = null;
                Control.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>   Creates the control core. </summary>
        ///
        /// <returns>   System.Object. </returns>

        protected override object CreateControlCore() => new Microsoft.Web.WebView2.WinForms.WebView2();

        /// <summary>
        /// Provides access to the control that represents the current Property Editor in a UI.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="T:System.Windows.Forms.Control" /> object used to display the current Property
        /// Editor in a UI.
        /// </value>

        public new Microsoft.Web.WebView2.WinForms.WebView2 Control => (Microsoft.Web.WebView2.WinForms.WebView2)base.Control;
    }
}

namespace DevExpress.ExpressApp.Editors
{
    /// <summary>
    /// Class WebViewPropertyEditorExtensions.
    /// </summary>
    /// <autogeneratedoc />
    public static partial class WebViewPropertyEditorExtensions
    {
        /// <summary>   Uses the web view URI property editor. </summary>
        ///
        /// <exception cref="ArgumentNullException">    editorDescriptorsFactory. </exception>
        ///
        /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>
        ///
        /// <returns>   EditorDescriptorsFactory. </returns>

        public static EditorDescriptorsFactory UseWebViewUriPropertyEditorWin(this EditorDescriptorsFactory editorDescriptorsFactory)
        {
            _ = editorDescriptorsFactory ?? throw new ArgumentNullException(nameof(editorDescriptorsFactory));

            editorDescriptorsFactory.RegisterPropertyEditor(
                Xenial.Framework.WebView.PubTernal.WebViewEditorAliases.WebViewUriPropertyEditor,
                typeof(Uri),
                typeof(WebViewUriPropertyEditor),
                false
            );

            return editorDescriptorsFactory;
        }
    }
}
