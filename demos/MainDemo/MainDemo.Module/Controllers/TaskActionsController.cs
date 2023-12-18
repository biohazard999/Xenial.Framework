using System;
using System.Collections;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base.General;

using MainDemo.Module.BusinessObjects;

namespace MainDemo.Module.Controllers
{
    public partial class TaskActionsController : ViewController
    {
        private readonly ChoiceActionItem setPriorityItem;
        private readonly ChoiceActionItem setStatusItem;
        private void UpdateSetTaskActionState()
        {
            var isGranted = true;

            var security = Application.GetSecurityStrategy();
            foreach (var selectedObject in View.SelectedObjects)
            {
                var isPriorityGranted = security.IsGranted(new PermissionRequest(ObjectSpace, typeof(DemoTask), SecurityOperations.Write, selectedObject, nameof(DemoTask.Priority)));
                var isStatusGranted = security.IsGranted(new PermissionRequest(ObjectSpace, typeof(DemoTask), SecurityOperations.Write, selectedObject, nameof(DemoTask.Status)));
                if (!isPriorityGranted || !isStatusGranted)
                {
                    isGranted = false;
                }
            }
            SetTaskAction.Enabled.SetItemValue("SecurityAllowance", isGranted);
        }
        public TaskActionsController()
        {
            TypeOfView = typeof(ObjectView);
            InitializeComponent();
            RegisterActions(components);

            setPriorityItem = new ChoiceActionItem(CaptionHelper.GetMemberCaption(typeof(DemoTask), "Priority"), null);
            SetTaskAction.Items.Add(setPriorityItem);
            FillItemWithEnumValues(setPriorityItem, typeof(Priority));
            setStatusItem = new ChoiceActionItem(CaptionHelper.GetMemberCaption(typeof(DemoTask), "Status"), null);
            SetTaskAction.Items.Add(setStatusItem);
            FillItemWithEnumValues(setStatusItem, typeof(TaskStatus));
        }
        private void FillItemWithEnumValues(ChoiceActionItem parentItem, Type enumType)
        {
            var ed = new EnumDescriptor(enumType);
            foreach (var current in Enum.GetValues(enumType))
            {
                var item = new ChoiceActionItem(ed.GetCaption(current), current);
                item.ImageName = ImageLoader.Instance.GetEnumValueImageName(current);
                parentItem.Items.Add(item);
            }
        }
        private void TaskActionsController_Activated(object sender, EventArgs e)
        {
            View.SelectionChanged += new EventHandler(View_SelectionChanged);
            UpdateSetTaskActionState();
        }

        private void View_SelectionChanged(object sender, EventArgs e) => UpdateSetTaskActionState();
        private DemoTask GetObject(DemoTask obj, IObjectSpace objectSpace, IObjectSpace newObjectSpace, ref int newObjectsCount)
        {
            if (objectSpace.IsNewObject(obj))
            {
                newObjectsCount++;
                return obj;
            }
            return (DemoTask)newObjectSpace.GetObject(obj);
        }
        private void SetTaskAction_Execute(object sender, SingleChoiceActionExecuteEventArgs args)
        {
            var objectSpace = View is ListView ? Application.CreateObjectSpace(typeof(DemoTask)) : View.ObjectSpace;
            var newObjectsCount = 0;
            var objectsToProcess = new ArrayList(args.SelectedObjects);
            if (args.SelectedChoiceActionItem.ParentItem == setPriorityItem)
            {
                foreach (var obj in objectsToProcess)
                {
                    var objInNewObjectSpace = GetObject((DemoTask)obj, View.ObjectSpace, objectSpace, ref newObjectsCount);
                    objInNewObjectSpace.Priority = (Priority)args.SelectedChoiceActionItem.Data;
                }
            }
            else if (args.SelectedChoiceActionItem.ParentItem == setStatusItem)
            {
                foreach (var obj in objectsToProcess)
                {
                    var objInNewObjectSpace = GetObject((DemoTask)obj, View.ObjectSpace, objectSpace, ref newObjectsCount);
                    objInNewObjectSpace.Status = (TaskStatus)args.SelectedChoiceActionItem.Data;
                }
            }
            if (View is DetailView && ((DetailView)View).ViewEditMode == ViewEditMode.View)
            {
                objectSpace.CommitChanges();
            }
            if ((View is ListView) && (newObjectsCount != objectsToProcess.Count))
            {
                objectSpace.CommitChanges();
                View.ObjectSpace.Refresh();
            }
        }
    }
}
