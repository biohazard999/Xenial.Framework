using System;
using System.Collections.Generic;
using System.Text;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;

namespace MainDemo.Module.BusinessObjects;

public partial class DemoTaskLayoutBuilder : LayoutBuilder<DemoTask>
{
    public Layout BuildLayout() => new(new()
    {

    })
    {
        VerticalGroup(
        HorizontalGroup("Details",
            VerticalGroup(
                Editor.Subject,
                Editor.StartDate,
                Editor.DueDate
            ),
            VerticalGroup(
                Editor.AssignedTo,
                Editor.Status,
                Editor.Priority
            )
        ),
        HorizontalGroup("Additional Information",
            VerticalGroup(
                Editor.EstimatedWorkHours,
                Editor.ActualWorkHours
            ),
            VerticalGroup(
                Editor.PercentCompleted,
                Editor.DateCompleted
            )
        )
        ) with
        { RelativeSize = 25 },
        VerticalGroup("Description",
            Editor.Description with { ShowCaption = false, RelativeSize = 100 }
        ) with
        { RelativeSize = 14 },
        VerticalGroup("Employees",
            Editor.Employees with { RelativeSize = 100 }
        ) with
        { RelativeSize = 30 }
    };
}

