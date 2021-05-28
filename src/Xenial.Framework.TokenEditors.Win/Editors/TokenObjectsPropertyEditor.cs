using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using DevExpress.Accessibility;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;

using Xenial.Framework.TokenEditors.Model;
using Xenial.Framework.TokenEditors.Win.Editors;

namespace Xenial.Framework.TokenEditors.Win.Editors
{
    /// <summary>
    /// Class TokenObjectsPropertyEditor. Implements the
    /// <see cref="DevExpress.ExpressApp.Win.Editors.DXPropertyEditor" />
    /// </summary>
    ///
    /// <seealso cref="DXPropertyEditor"/>
    /// <seealso cref="IComplexViewItem"/>
    /// <seealso cref="DevExpress.ExpressApp.Win.Editors.DXPropertyEditor"/>

    [XenialCheckLicence]
    public sealed partial class TokenObjectsPropertyEditor : DXPropertyEditor, IComplexViewItem
    {
        private IObjectSpace? objectSpace;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenObjectsPropertyEditor"/> class.
        /// </summary>
        ///
        /// <param name="objectType">   Type of the object. </param>
        /// <param name="model">        The model. </param>

        public TokenObjectsPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
            => ControlBindingProperty = nameof(Control.BindableEditValue);

        /// <summary>
        /// Passes the <see cref="T:DevExpress.ExpressApp.XafApplication" /> and
        /// <see cref="T:DevExpress.ExpressApp.IObjectSpace" /> objects to the current
        /// <see cref="T:DevExpress.ExpressApp.Editors.IComplexViewItem" />.
        /// </summary>
        ///
        /// <param name="objectSpace">  An <see cref="T:DevExpress.ExpressApp.IObjectSpace" /> object
        ///                             that provides methods to access the application database. </param>
        /// <param name="application">  An <see cref="T:DevExpress.ExpressApp.XafApplication" /> object
        ///                             that manages the current application. </param>

        public void Setup(IObjectSpace objectSpace, XafApplication application)
            => this.objectSpace = objectSpace;

        /// <summary>   Creates the repository item. </summary>
        ///
        /// <returns>   RepositoryItem. </returns>

        protected override RepositoryItem CreateRepositoryItem() => ApplyModelOptions(new XenialRepositoryItemTokenObjectsEdit
        {
            ObjectSpace = objectSpace
        });


        private XenialRepositoryItemTokenObjectsEdit ApplyModelOptions(XenialRepositoryItemTokenObjectsEdit tokenEdit)
        {
            if (Model is IXenialTokenObjectsModelPropertyEditor model)
            {
                tokenEdit.DropDownShowMode = model.XenialTokenObjectsDropDownShowMode switch
                {
                    TokenDropDownShowMode.Default => TokenEditDropDownShowMode.Default,
                    TokenDropDownShowMode.Regular => TokenEditDropDownShowMode.Regular,
                    TokenDropDownShowMode.Outlook => TokenEditDropDownShowMode.Outlook,
                    _ => tokenEdit.DropDownShowMode
                };

                tokenEdit.PopupFilterMode = model.XenialTokenObjectsPopupFilterMode switch
                {
                    TokenPopupFilterMode.StartsWith => TokenEditPopupFilterMode.StartWith,
                    TokenPopupFilterMode.Contains => TokenEditPopupFilterMode.Contains,
                    _ => tokenEdit.PopupFilterMode
                };
            }
            return tokenEdit;
        }

        /// <summary>   Creates the control core. </summary>
        ///
        /// <returns>   System.Object. </returns>

        protected override object CreateControlCore() => new XenialTokenObjectsEdit();

        /// <summary>   Determines whether [is member setter required]. </summary>
        ///
        /// <returns>   <c>true</c> if [is member setter required]; otherwise, <c>false</c>. </returns>

        protected override bool IsMemberSetterRequired() => false;

        /// <summary>   Setups the repository item. </summary>
        ///
        /// <param name="item"> The item. </param>

        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            if (item is XenialRepositoryItemTokenObjectsEdit tokenItem && objectSpace is not null)
            {
                tokenItem.BeginUpdate();
                ApplyModelOptions(tokenItem);
                try
                {
                    tokenItem.ObjectSpace = objectSpace;
                    tokenItem.MemberInfo = MemberInfo;
                    tokenItem.ImmediatePostData = ImmediatePostData;
                    tokenItem.RefreshDataSource();
                    tokenItem.Validating += TokenItem_Validating;
                }
                finally
                {
                    tokenItem.EndUpdate();
                }
            }
        }

        private void TokenItem_Validating(object sender, CancelEventArgs e)
        {
            if (objectSpace is not null
                && sender is XenialTokenObjectsEdit edit
                && edit.Properties.WasModified
                )
            {
                objectSpace.SetModified(CurrentObject);
                edit.Properties.ResetModified();
            }
        }

        /// <summary>   Refreshes this instance. </summary>
        public override void Refresh()
        {
            if (Control is not null)
            {
                Control.Properties.ImmediatePostData = ImmediatePostData;
            }
            base.Refresh();
            ReloadCollectionSource();
        }

        /// <summary>   Refreshes the data source. </summary>
        public override void RefreshDataSource()
        {
            base.RefreshDataSource();
            ReloadCollectionSource();
        }

        private void ReloadCollectionSource()
        {
            if (Control is not null)
            {
                Control.Properties.RefreshDataSource();
            }
        }

        /// <summary>   Called when [current object changed]. </summary>
        protected override void OnCurrentObjectChanged()
        {
            base.OnCurrentObjectChanged();
            RefreshDataSource();
        }

        /// <summary>
        /// Provides access to the control that represents the current Property Editor in a UI.
        /// </summary>
        ///
        /// <value>
        /// A DevExpress.XtraEditors.BaseEdit object representing a control used to display the current
        /// Property Editor in a UI.
        /// </value>

        public new XenialTokenObjectsEdit Control => (XenialTokenObjectsEdit)base.Control;
    }

    /// <summary>
    /// Class XenialRepositoryItemTokenEdit.
    /// Implements the <see cref="DevExpress.XtraEditors.Repository.RepositoryItemTokenEdit" />
    /// </summary>
    /// <seealso cref="DevExpress.XtraEditors.Repository.RepositoryItemTokenEdit" />
    /// <autogeneratedoc />
    [UserRepositoryItem(nameof(RegisterCustomEdit))]
    [XenialCheckLicence]
    public sealed partial class XenialRepositoryItemTokenObjectsEdit : RepositoryItemTokenEdit
    {
        static XenialRepositoryItemTokenObjectsEdit() => RegisterCustomEdit();

        /// <summary>
        /// (Immutable)
        /// (Immutable)
        /// The custom edit name.
        /// </summary>

        public const string XenialTokenObjectsEditName = nameof(XenialTokenObjectsEdit);
        private IObjectSpace? objectSpace;
        private IMemberInfo? memberInfo;
        private bool immediatePostData;

        /// <summary>   Gets the was modified. </summary>
        ///
        /// <value> The was modified. </value>

        public bool WasModified { get; private set; }

        /// <summary>   Gets the name of the editor type. </summary>
        ///
        /// <value> The name of the editor type. </value>

        public override string EditorTypeName => XenialTokenObjectsEditName;

        /// <summary>   Gets the owner edit. </summary>
        ///
        /// <value> The owner edit. </value>

        public new XenialTokenObjectsEdit OwnerEdit => (XenialTokenObjectsEdit)base.OwnerEdit;

        /// <summary>   Registers the custom edit. </summary>
        public static void RegisterCustomEdit()
            => EditorRegistrationInfo.Default.Editors.Add(
                new EditorClassInfo(
                    XenialTokenObjectsEditName,
                    typeof(XenialTokenObjectsEdit),
                    typeof(XenialRepositoryItemTokenObjectsEdit),
                    typeof(TokenEditViewInfo),
                    new TokenEditPainter(),
                    false,
                    null,
                    typeof(TokenEditAccessible)
                )
            );

        /// <summary>   Gets or sets the object space. </summary>
        ///
        /// <value> The object space. </value>

        public IObjectSpace? ObjectSpace
        {
            get => objectSpace;
            set
            {
                if (objectSpace != value)
                {
                    objectSpace = value;
                    OnPropertiesChanged();
                }
            }
        }

        /// <summary>   Gets or sets the member information. </summary>
        ///
        /// <value> The member information. </value>

        public IMemberInfo? MemberInfo
        {
            get => memberInfo;
            set
            {
                if (memberInfo != value)
                {
                    memberInfo = value;
                    OnPropertiesChanged();
                }
            }
        }

        /// <summary>   Gets or sets the immediate post data. </summary>
        ///
        /// <value> The immediate post data. </value>

        public bool ImmediatePostData
        {
            get => immediatePostData;
            set
            {
                if (immediatePostData != value)
                {
                    immediatePostData = value;
                    OnPropertiesChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XenialRepositoryItemTokenObjectsEdit"/> class.
        /// </summary>

        public XenialRepositoryItemTokenObjectsEdit()
            => EditValueType = TokenEditValueType.List;

        /// <summary>   Assigns the specified item. </summary>
        ///
        /// <param name="item"> The item. </param>

        public override void Assign(RepositoryItem item)
        {
            BeginUpdate();
            try
            {
                base.Assign(item);
                if (item is XenialRepositoryItemTokenObjectsEdit source)
                {
                    objectSpace = source.ObjectSpace;
                    memberInfo = source.MemberInfo;
                    immediatePostData = source.ImmediatePostData;
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>   Refreshes the data source. </summary>
        public void RefreshDataSource()
        {
            BeginUpdate();
            try
            {
                if (ObjectSpace is not null && MemberInfo is not null)
                {
                    DataSource = ObjectSpace.GetObjects(MemberInfo.ListElementType);
                    ValueMember = ObjectSpace.GetKeyPropertyName(MemberInfo.ListElementType);
                    DisplayMember = MemberInfo.ListElementTypeInfo.DefaultMember?.BindingName;
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>   Raises the token added. </summary>
        ///
        /// <param name="e">    The <see cref="DevExpress.XtraEditors.TokenEditTokenAddedEventArgs"/>
        ///                     instance containing the event data. </param>

        protected override void RaiseTokenAdded(TokenEditTokenAddedEventArgs e)
        {
            base.RaiseTokenAdded(e);
            WasModified = true;
            if (OwnerEdit.EditValue is BindingList<object> list
                && objectSpace is not null
                && OwnerEdit.BindableEditValue is System.Collections.IList listToAdd
                && MemberInfo is not null)
            {
                var key = e.Token.Value;

                var objectToAdd = objectSpace.GetObjectByKey(MemberInfo.ListElementType, key);
                if (objectToAdd is null && DataSource is System.Collections.IList dataSource)
                {
                    objectToAdd = dataSource
                        .OfType<object>()
                        .FirstOrDefault(ds => objectSpace.GetKeyValue(ds)?.Equals(key) == true);
                }
                if (objectToAdd is not null)
                {
                    listToAdd.Add(objectToAdd);
                    if (ImmediatePostData)
                    {
                        DoValidate();
                    }
                }
            }
        }

        /// <summary>   Raises the token removed. </summary>
        ///
        /// <param name="e">    The <see cref="DevExpress.XtraEditors.TokenEditTokenRemovedEventArgs"/>
        ///                     instance containing the event data. </param>

        protected override void RaiseTokenRemoved(TokenEditTokenRemovedEventArgs e)
        {
            base.RaiseTokenRemoved(e);
            WasModified = true;
            if (OwnerEdit.EditValue is BindingList<object> list
                && objectSpace is not null
                && OwnerEdit.BindableEditValue is System.Collections.IList listToRemove
                && MemberInfo is not null)
            {
                var key = e.Token.Value;

                var objectToRemove = objectSpace.GetObjectByKey(MemberInfo.ListElementType, key);

                if (objectToRemove == null && DataSource is System.Collections.IList dataSource)
                {
                    objectToRemove = dataSource
                        .OfType<object>()
                        .FirstOrDefault(ds => objectSpace.GetKeyValue(ds)?.Equals(key) == true);
                }

                if (objectToRemove is not null)
                {
                    listToRemove.Remove(objectToRemove);
                    if (ImmediatePostData)
                    {
                        DoValidate();
                    }
                }
            }
        }

        private void DoValidate()
        {
            if (OwnerEdit is not null)
            {
                OwnerEdit.DoValidate();
            }
        }

        /// <summary>   Resets the modified. </summary>
        public void ResetModified()
            => WasModified = false;
    }

    /// <summary>
    /// Class XenialTokenEdit.
    /// Implements the <see cref="DevExpress.XtraEditors.TokenEdit" />
    /// </summary>
    /// <seealso cref="DevExpress.XtraEditors.TokenEdit" />
    /// <autogeneratedoc />
    [XenialCheckLicence]
    public sealed partial class XenialTokenObjectsEdit : TokenEdit
    {
        static XenialTokenObjectsEdit() => XenialRepositoryItemTokenObjectsEdit.RegisterCustomEdit();

        /// <summary>   Gets the name of the editor type. </summary>
        ///
        /// <value> The name of the editor type. </value>

        public override string EditorTypeName => XenialRepositoryItemTokenObjectsEdit.XenialTokenObjectsEditName;

        /// <summary>   Gets the properties. </summary>
        ///
        /// <value> The properties. </value>

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new XenialRepositoryItemTokenObjectsEdit Properties => (XenialRepositoryItemTokenObjectsEdit)base.Properties;

        private System.Collections.IList? bindableEditValue;

        /// <summary>   Gets or sets the bindable edit value. </summary>
        ///
        /// <value> The bindable edit value. </value>

#pragma warning disable CA2227 //By Design
        public System.Collections.IList? BindableEditValue
        {
            get => bindableEditValue;
            set
            {
                var equalRef = bindableEditValue == value;
                if (!equalRef)
                {
                    bindableEditValue = value;
                    if (value is System.Collections.IList list && Properties.ObjectSpace is not null)
                    {
                        if (EditValue == null || !equalRef)
                        {
                            EditValue = new BindingList<object>();
                            if (EditValue is System.Collections.IList editList)
                            {
                                foreach (var item in list)
                                {
                                    editList.Add(Properties.ObjectSpace.GetKeyValue(item));
                                }
                            }
                        }
                    }
                }
            }
        }
#pragma warning restore CA2227
    }
}

namespace DevExpress.ExpressApp.Editors
{
    /// <summary>   Class TokenObjectsPropertyEditorExtensions. </summary>
    public static class TokenObjectsPropertyEditorWinExtensions
    {
        /// <summary>   Uses the token objects property editor. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>
        ///
        /// <returns>   EditorDescriptorsFactory. </returns>

        public static EditorDescriptorsFactory UseTokenObjectsPropertyEditorsWin(this EditorDescriptorsFactory editorDescriptorsFactory)
        {
            _ = editorDescriptorsFactory ?? throw new ArgumentNullException(nameof(editorDescriptorsFactory));

            editorDescriptorsFactory.RegisterPropertyEditor(
                Xenial.Framework.TokenEditors.PubTernal.TokenEditorAliases.TokenObjectsPropertyEditor,
                typeof(IList<>),
                typeof(TokenObjectsPropertyEditor),
                false
            );

            return editorDescriptorsFactory;
        }

        /// <summary>   Uses the token objects property editor. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>
        ///
        /// <returns>   EditorDescriptorsFactory. </returns>

        public static EditorDescriptorsFactory UseTokenObjectsPropertyEditorsWin<T>(this EditorDescriptorsFactory editorDescriptorsFactory)
        {
            _ = editorDescriptorsFactory ?? throw new ArgumentNullException(nameof(editorDescriptorsFactory));

            editorDescriptorsFactory.RegisterPropertyEditor(
                Xenial.Framework.TokenEditors.PubTernal.TokenEditorAliases.TokenObjectsPropertyEditor,
                typeof(IList<T>),
                typeof(TokenObjectsPropertyEditor),
                false
            );

            editorDescriptorsFactory.RegisterPropertyEditor(
                Xenial.Framework.TokenEditors.PubTernal.TokenEditorAliases.TokenObjectsPropertyEditor,
                typeof(BindingList<T>),
                typeof(TokenObjectsPropertyEditor),
                false
            );

            return editorDescriptorsFactory;
        }

        /// <summary>   Uses the token objects property editor. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>
        ///
        /// <returns>   EditorDescriptorsFactory. </returns>

        public static EditorDescriptorsFactory UseTokenObjectsPropertyEditorsForTypeWin<T>(this EditorDescriptorsFactory editorDescriptorsFactory)
        {
            _ = editorDescriptorsFactory ?? throw new ArgumentNullException(nameof(editorDescriptorsFactory));

            editorDescriptorsFactory.RegisterPropertyEditor(
                Xenial.Framework.TokenEditors.PubTernal.TokenEditorAliases.TokenObjectsPropertyEditor,
                typeof(T),
                typeof(TokenObjectsPropertyEditor),
                false
            );

            return editorDescriptorsFactory;
        }
    }
}
