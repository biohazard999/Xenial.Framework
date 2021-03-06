﻿using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraEditors;

namespace Xenial.Framework.Win.SystemModule
{
    /// <summary>
    /// A controller for handling xenial advanced model editor actions views. This class cannot be
    /// inherited.
    /// </summary>
    ///
    /// <seealso cref="ViewController"/>

    [XenialCheckLicence]
    public sealed partial class XenialAdvancedModelEditorActionsViewController : ViewController
    {
        private const string actionCategory = "Diagnostic";
        private SimpleAction? editModelAction;

        /// <summary>   (Immutable) the action state key. </summary>
        public const string ActionStateKey = @"Same as EditModelAction";

        /// <summary>   Gets the open bo model in model editor simple action. </summary>
        ///
        /// <value> The open bo model in model editor simple action. </value>

        public SimpleAction OpenBOModelInModelEditorSimpleAction { get; }

        /// <summary>   Gets the open view in model editor simple action. </summary>
        ///
        /// <value> The open view in model editor simple action. </value>

        public SimpleAction OpenViewInModelEditorSimpleAction { get; }

        private string GetBOModelNodePath() => $@"BOModel\{View.ObjectTypeInfo.FullName}";
        private string GetViewNodePath() => $@"Views\{View.Id}";

        /// <summary>   Default constructor. </summary>
        public XenialAdvancedModelEditorActionsViewController()
        {
            OpenViewInModelEditorSimpleAction = new SimpleAction(this, nameof(OpenViewInModelEditorSimpleAction), actionCategory, (s, e) =>
            {
                ((IModelApplicationModelEditor)Application.Model).ModelEditorSettings.ModelEditorControl.FocusedObject = GetViewNodePath();
                editModelAction?.DoExecute();
            })
            {
                Caption = "Edit View",
                ImageName = "ModelEditor_Views",
                Shortcut = "CtrlShiftF2",
                ToolTip = "Opens the Model Editor and focuses the active View node.",
                PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Image
            };

            OpenBOModelInModelEditorSimpleAction = new SimpleAction(this, nameof(OpenBOModelInModelEditorSimpleAction), "Diagnostic", (s, e) =>
            {
                ((IModelApplicationModelEditor)Application.Model).ModelEditorSettings.ModelEditorControl.FocusedObject = GetBOModelNodePath();
                editModelAction?.DoExecute();
            })
            {
                Caption = "Edit BOModel",
                ImageName = "ModelEditor_Business_Object_Model",
                Shortcut = "CtrlShiftF3",
                ToolTip = "Opens the Model Editor and focuses the active BOModel node.",
                PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Image
            };

            OpenBOModelInModelEditorSimpleAction.CustomizeControl += OpenViewInModelEditorAction_CustomizeControl;
            OpenViewInModelEditorSimpleAction.CustomizeControl += OpenViewInModelEditorAction_CustomizeControl;
        }

        private void OpenViewInModelEditorAction_CustomizeControl(object? sender, CustomizeControlEventArgs e)
        {
            if (e.Control is SimpleButton simpleButton)
            {
                if (simpleButton.Parent is ButtonsContainer) //We are in a popup window
                {
                    var minSize = ((DevExpress.Utils.Controls.IXtraResizableControl)simpleButton).MinSize;
                    simpleButton.MinimumSize = minSize;
                    simpleButton.MaximumSize = minSize;
                }
            }
        }

        /// <summary>   Executes the 'activated' action. </summary>
        protected override void OnActivated()
        {
            base.OnActivated();
            editModelAction = Application.MainWindow.GetController<EditModelController>()?.EditModelAction;
            UpdateActionState();

            if (editModelAction != null)
            {
                editModelAction.Changed -= EditModelAction_Changed;
                editModelAction.Changed += EditModelAction_Changed;
            }
        }

        /// <summary>   Executes the 'deactivated' action. </summary>
        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            editModelAction = Application.MainWindow?.GetController<EditModelController>()?.EditModelAction;

            UpdateActionState();

            if (editModelAction is not null)
            {
                editModelAction.Changed -= EditModelAction_Changed;
            }

            editModelAction = null;
        }

        private void EditModelAction_Changed(object? sender, ActionChangedEventArgs e)
        {
            if (e.ChangedPropertyType == ActionChangedType.Active
                || e.ChangedPropertyType == ActionChangedType.Enabled
            )
            {
                UpdateActionState();
            }
        }

        private void UpdateActionState()
        {
            OpenViewInModelEditorSimpleAction.Active[ActionStateKey]
                = editModelAction is not null
                    && editModelAction.Active;

            OpenViewInModelEditorSimpleAction.Enabled[ActionStateKey]
                = editModelAction is not null
                    && editModelAction.Enabled;

            OpenBOModelInModelEditorSimpleAction.Active[ActionStateKey]
                = editModelAction is not null
                    && editModelAction.Active;

            OpenBOModelInModelEditorSimpleAction.Enabled[ActionStateKey]
                = editModelAction is not null
                    && editModelAction.Enabled;
        }
    }
}

namespace DevExpress.ExpressApp
{
    /// <summary>
    /// Class ControllerExtensions.
    /// </summary>
    /// <autogeneratedoc />
    public static partial class ControllerTypeListExtension
    {
        /// <summary>
        /// Uses the advanced model editor actions controller
        /// <see cref="Xenial.Framework.Win.SystemModule.XenialAdvancedModelEditorActionsViewController" />.
        /// </summary>
        ///
        /// <param name="types">    The types. </param>
        ///
        /// <returns>   IEnumerable&lt;Type&gt;. </returns>

        public static IEnumerable<Type> UseXenialAdvancedModelEditorActionsControllers(this IEnumerable<Type> types)
            => types.Concat(new[] { typeof(Xenial.Framework.Win.SystemModule.XenialAdvancedModelEditorActionsViewController) });
    }
}
