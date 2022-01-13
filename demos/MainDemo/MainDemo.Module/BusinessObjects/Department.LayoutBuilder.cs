using System;
using System.Collections.Generic;
using System.Text;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;

namespace MainDemo.Module.BusinessObjects;

public partial class DepartmentLayoutBuilder : LayoutBuilder<Department>
{
    public Layout BuildLayout() => new()
    {

        VerticalGroup(
            Editor.Title,
            Editor.Location,
            Editor.Office,
            PropertyEditor("NumberOfEmployees"),
            Editor.DepartmentHead
        ),
        VerticalGroup(
            Editor.Description
        ),
        TabbedGroup(
            Tab(Editor.Employees),
            Tab(Editor.Positions)
        )
    };
}
