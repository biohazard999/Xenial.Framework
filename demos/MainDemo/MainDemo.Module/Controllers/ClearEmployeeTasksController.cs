using System;
using System.Collections;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;

using MainDemo.Module.BusinessObjects;

namespace MainDemo.Module.Controllers
{
    public partial class ClearEmployeeTasksController : ViewController
    {
        public ClearEmployeeTasksController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        private void ClearTasksAction_Execute(Object sender, SimpleActionExecuteEventArgs e)
        {
            while (((Employee)View.CurrentObject).Tasks.Count > 0)
            {
                ((Employee)View.CurrentObject).Tasks.Remove(((Employee)View.CurrentObject).Tasks[0]);
            }
            ObjectSpace.SetModified(View.CurrentObject);
        }

        private void ClearTasksController_Activated(object sender, EventArgs e)
        {
            ClearTasksAction.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
            ((DetailView)View).ViewEditModeChanged += new EventHandler<EventArgs>(ClearTasksController_ViewEditModeChanged);
        }

        private void ClearTasksController_ViewEditModeChanged(object sender, EventArgs e) => ClearTasksAction.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
    }
}
