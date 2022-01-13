using System;
using System.Collections.Generic;
using System.Text;

using Xenial.Framework.Images;
using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;

namespace MainDemo.Module.BusinessObjects;

[Xenial.XenialExpandMember(Constants.Department)]
public partial class EmployeeLayoutBuilder : LayoutBuilder<Employee>
{
    public Layout BuildLayout() => new(new()
    {
        Caption = "Hot Reload!!!",
        ImageName = XenialImages.Action_HotReload,
        //ImageName = "BO_Employee",
        EnableLayoutGroupImages = true
    })
    {
        HorizontalGroup("Details", "BO_Employee",
            HorizontalGroup(
                VerticalGroup(
                    Editor.FirstName,
                    Editor.MiddleName,
                    Editor.LastName,
                    Editor.FullName
                ) with
                { RelativeSize = 45 },
                VerticalGroup(
                    Editor.Email,
                    Editor.Birthday,
                    Editor.Address1 with { Caption = "Address" },
                    Editor.TitleOfCourtesy
                ) with
                { RelativeSize = 55 },
                HorizontalGroup(
                    Editor.Photo with { ShowCaption = false }
                ) with
                { RelativeSize = 15 }
            )
        ),
        HorizontalGroup("Additional Information", "Action_AboutInfo",
            VerticalGroup(
                Editor.NickName,
                Editor.SpouseName,
                Editor.Anniversary,
                Editor.WebPageAddress
            ) with
            { RelativeSize = 39 },
            VerticalGroup(
                Editor.Department,
                Editor._Department.Office,
                Editor.Position with { ToolTip = "Select a job position for the current employee from available positions in the selected department" },
                Editor.Manager with { ToolTip = "Select a manager for the current employee from available managers in the selected department" }
            ) with
            { RelativeSize = 61 }
        ),
        HorizontalGroup("Notes", "BO_Note",
            Editor.Notes with { ShowCaption = false }
        ) with
        { RelativeSize = 10 },
        TabbedGroup(
            Tab("Tasks", Editor.Tasks) with { ToolTip = "View, assign or remove tasks for the current employee" },
            Tab("Change History", "BO_Audit_ChangeHistory", Editor.ChangeHistory),
            Tab("Phone Numbers", "BO_Phone", Editor.PhoneNumbers)
        )
    };
}
