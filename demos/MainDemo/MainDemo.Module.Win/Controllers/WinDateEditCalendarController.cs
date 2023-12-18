using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraEditors;

using MainDemo.Module.BusinessObjects;

namespace MainDemo.Module.Win.Controllers
{
    public class WinDateEditCalendarController : ObjectViewController<DetailView, Employee>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            View.CustomizeViewItemControl(this, SetCalendarView, nameof(Employee.Birthday));
        }
        private void SetCalendarView(ViewItem viewItem)
        {
            var dateEdit = viewItem.Control as DateEdit;
            if (dateEdit != null)
            {
                dateEdit.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.TouchUI;
            }
        }
    }
}
