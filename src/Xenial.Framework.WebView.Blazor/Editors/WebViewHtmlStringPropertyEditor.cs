﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.ExpressApp.Blazor.Components;
using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;

using Microsoft.AspNetCore.Components;

using Xenial.Framework.WebView.Blazor.Editors;

namespace Xenial.Framework.WebView.Blazor.Editors
{
    /// <summary>
    /// Class WebViewHtmlStringPropertyEditor. Implements the
    /// <see cref="DevExpress.ExpressApp.Blazor.Editors.BlazorPropertyEditorBase" />
    /// </summary>
    ///
    /// <seealso cref="BlazorPropertyEditorBase"/>
    /// <seealso cref="DevExpress.ExpressApp.Blazor.Editors.BlazorPropertyEditorBase">  <autogeneratedoc /></seealso>

    [XenialCheckLicence]
    public sealed partial class WebViewHtmlStringPropertyEditor : BlazorPropertyEditorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebViewHtmlStringPropertyEditor"/> class.
        /// </summary>
        ///
        /// <param name="objectType">   Type of the object. </param>
        /// <param name="model">        The model. </param>

        public WebViewHtmlStringPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        /// <summary>   Determines whether [is member setter required]. </summary>
        ///
        /// <returns>   <c>true</c> if [is member setter required]; otherwise, <c>false</c>. </returns>

        protected override bool IsMemberSetterRequired() => false;

        /// <summary>   Creates the component adapter. </summary>
        ///
        /// <returns>   IComponentAdapter. </returns>

        protected override IComponentAdapter CreateComponentAdapter() => new WebViewHtmlStringComponentAdapter(new WebViewHtmlStringInputModel
        {
            //DataSource = (Model.PredefinedValues ?? string.Empty).Split(";").ToList()
        });
    }

    /// <summary>
    /// Class WebViewHtmlStringInputModel.
    /// Implements the <see cref="DevExpress.ExpressApp.Blazor.Components.Models.ComponentModelBase" />
    /// </summary>
    /// <seealso cref="DevExpress.ExpressApp.Blazor.Components.Models.ComponentModelBase" />
    /// <autogeneratedoc />
    [XenialCheckLicence]
    public sealed partial class WebViewHtmlStringInputModel : ComponentModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebViewUriInputModel"/> class.
        /// </summary>

        public WebViewHtmlStringInputModel()
        {
        }

        /// <summary>   Gets or sets a value indicating whether [read only]. </summary>
        ///
        /// <value> <c>true</c> if [read only]; otherwise, <c>false</c>. </value>

        public bool ReadOnly { get => GetPropertyValue<bool>(); set => SetPropertyValue(value); }

        /// <summary>   Gets or sets the value. </summary>
        ///
        /// <value> The value. </value>

        public string? Value { get => GetPropertyValue<string?>(); set => SetPropertyValue(value); }

        private static string Base64Encode(string? plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText ?? string.Empty);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>   Gets the base64 value. </summary>
        ///
        /// <value> The base64 value. </value>

        public string? Base64Value => $"data:text/html;base64,{Base64Encode(Value)}";

        /// <summary>   Sets the value from UI. </summary>
        ///
        /// <param name="value">    The value. </param>

        public void SetValueFromUI(string? value)
        {
            SetPropertyValue(value, notify: false, nameof(Value));
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>   Occurs when [value changed]. </summary>
        public event EventHandler? ValueChanged;
    }

    /// <summary>
    /// Class TokenStringComponentAdapter.
    /// Implements the <see cref="DevExpress.ExpressApp.Blazor.Editors.Adapters.ComponentAdapterBase" />
    /// </summary>
    /// <seealso cref="DevExpress.ExpressApp.Blazor.Editors.Adapters.ComponentAdapterBase" />
    /// <autogeneratedoc />
    [XenialCheckLicence]
    public sealed partial class WebViewHtmlStringComponentAdapter : ComponentAdapterBase
    {
        /// <summary>   Gets the component model. </summary>
        ///
        /// <value> The component model. </value>

        public WebViewHtmlStringInputModel ComponentModel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebViewHtmlStringComponentAdapter"/> class.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="componentModel">   The component model. </param>
        ///
        /// ### <exception cref="System.ArgumentNullException"> componentModel. </exception>

        public WebViewHtmlStringComponentAdapter(WebViewHtmlStringInputModel componentModel)
        {
            ComponentModel = componentModel ?? throw new ArgumentNullException(nameof(componentModel));
            ComponentModel.ValueChanged += ComponentModel_ValueChanged;
        }

        private void ComponentModel_ValueChanged(object? sender, EventArgs? e) => RaiseValueChanged();

        /// <summary>   Gets the value. </summary>
        ///
        /// <returns>   System.Object. </returns>

        public override object? GetValue() => ComponentModel.Value;

        /// <summary>   Sets the value. </summary>
        ///
        /// <param name="value">    The value. </param>

        public override void SetValue(object? value)
        {
            if (value is string str)
            {
                ComponentModel.Value = str;
            }
            else
            {
                ComponentModel.Value = value?.ToString();
            }
        }

        /// <summary>   Sets the allow edit. </summary>
        ///
        /// <param name="allowEdit">    if set to <c>true</c> [allow edit]. </param>

        public override void SetAllowEdit(bool allowEdit) => ComponentModel.ReadOnly = !allowEdit;

        /// <summary>   Sets the allow null. </summary>
        ///
        /// <param name="allowNull">    if set to <c>true</c> [allow null]. </param>

        public override void SetAllowNull(bool allowNull) => _ = true;

        /// <summary>   Sets the display format. </summary>
        ///
        /// <param name="displayFormat">    The display format. </param>

        public override void SetDisplayFormat(string displayFormat) => _ = true;

        /// <summary>   Sets the edit mask. </summary>
        ///
        /// <param name="editMask"> The edit mask. </param>

        public override void SetEditMask(string editMask) => _ = true;

        /// <summary>   Sets the type of the edit mask. </summary>
        ///
        /// <param name="editMaskType"> Type of the edit mask. </param>

        public override void SetEditMaskType(EditMaskType editMaskType) => _ = true;

        /// <summary>   Sets the error icon. </summary>
        ///
        /// <param name="errorIcon">    The error icon. </param>

        public override void SetErrorIcon(ImageInfo errorIcon) => _ = true;

        /// <summary>   Sets the error message. </summary>
        ///
        /// <param name="errorMessage"> The error message. </param>

        public override void SetErrorMessage(string errorMessage) => _ = true;

        /// <summary>   Sets the is password. </summary>
        ///
        /// <param name="isPassword">   if set to <c>true</c> [is password]. </param>

        public override void SetIsPassword(bool isPassword) => _ = true;

        /// <summary>   Sets the maximum length. </summary>
        ///
        /// <param name="maxLength">    The maximum length. </param>

        public override void SetMaxLength(int maxLength) => _ = true;

        /// <summary>   Sets the null text. </summary>
        ///
        /// <param name="nullText"> The null text. </param>

        public override void SetNullText(string nullText) => _ = true;

        /// <summary>   Creates the component. </summary>
        ///
        /// <returns>   RenderFragment. </returns>

        protected override RenderFragment CreateComponent()
            => ComponentModelObserver.Create(ComponentModel, WebViewHtmlStringComponent.Create(ComponentModel));
    }
}

namespace DevExpress.ExpressApp.Editors
{
    /// <summary>
    /// Class WebViewPropertyEditorBlazorExtensions.
    /// </summary>
    /// <autogeneratedoc />
    public static partial class WebViewPropertyEditorBlazorExtensions
    {
        /// <summary>   Uses the web view HTML string property editors blazor. </summary>
        ///
        /// <exception cref="ArgumentNullException">    editorDescriptorsFactory. </exception>
        ///
        /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>
        ///
        /// <returns>   EditorDescriptorsFactory. </returns>

        public static EditorDescriptorsFactory UseWebViewHtmlStringPropertyEditorsBlazor(this EditorDescriptorsFactory editorDescriptorsFactory)
        {
            _ = editorDescriptorsFactory ?? throw new ArgumentNullException(nameof(editorDescriptorsFactory));

            editorDescriptorsFactory.RegisterPropertyEditor(
                Xenial.Framework.WebView.PubTernal.WebViewEditorAliases.WebViewHtmlStringPropertyEditor,
                typeof(string),
                typeof(WebViewHtmlStringPropertyEditor),
                false
            );

            return editorDescriptorsFactory;
        }
    }
}
