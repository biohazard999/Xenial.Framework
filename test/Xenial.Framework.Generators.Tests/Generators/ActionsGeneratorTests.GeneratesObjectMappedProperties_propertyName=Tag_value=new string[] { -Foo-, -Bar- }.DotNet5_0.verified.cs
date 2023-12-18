﻿//HintName: GeneratesSimpleActionWhenDefined.GeneratesSimpleActionWhenDefinedController.g.cs
// <auto-generated />

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace MyActions
{
    [CompilerGenerated]
    public partial class GeneratesSimpleActionWhenDefinedController : DevExpress.ExpressApp.ViewController
    {
        public DevExpress.ExpressApp.Actions.SimpleAction GeneratesSimpleActionWhenDefinedSimpleAction { get; private set; }
        
        public GeneratesSimpleActionWhenDefinedController()
        {
            this.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.GeneratesSimpleActionWhenDefinedSimpleAction = new DevExpress.ExpressApp.Actions.SimpleAction(this, "MyActions.GeneratesSimpleActionWhenDefinedSimpleAction", "Edit");
            this.GeneratesSimpleActionWhenDefinedSimpleAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.GeneratesSimpleActionWhenDefinedSimpleAction.Tag = new string[] { "Foo", "Bar" };
        }
        
        partial void OnActivatedCore();
        
        partial void OnDeactivatedCore();
        
        protected override void OnActivated()
        {
            base.OnActivated();
            this.GeneratesSimpleActionWhenDefinedSimpleAction.Execute -= GeneratesSimpleActionWhenDefinedSimpleActionExecute;
            this.GeneratesSimpleActionWhenDefinedSimpleAction.Execute += GeneratesSimpleActionWhenDefinedSimpleActionExecute;
            this.OnActivatedCore();
        }
        
        protected override void OnDeactivated()
        {
            this.GeneratesSimpleActionWhenDefinedSimpleAction.Execute -= GeneratesSimpleActionWhenDefinedSimpleActionExecute;
            this.OnDeactivatedCore();
            base.OnDeactivated();
        }
        
        protected MyActions.GeneratesSimpleActionWhenDefined CreateGeneratesSimpleActionWhenDefinedAction()
        {
            this.CreateGeneratesSimpleActionWhenDefinedActionCore();
            MyActions.GeneratesSimpleActionWhenDefined action = new MyActions.GeneratesSimpleActionWhenDefined();
            return action;
        }
        
        partial void CreateGeneratesSimpleActionWhenDefinedActionCore();
        
        private void GeneratesSimpleActionWhenDefinedSimpleActionExecute(object sender, DevExpress.ExpressApp.Actions.SimpleActionExecuteEventArgs e)
        {
        }
    }
}
