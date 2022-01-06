using System;
using System.Collections.Generic;
using System.Text;

using Xenial.Framework.Layouts;

namespace MainDemo.Module.BusinessObjects;

[Xenial.XenialExpandMember(Constants.Employee)]
public partial class ResumeColumnsBuilder : ColumnsBuilder<Resume>
{
    public Columns BuildColumns() => new(new()
    {
        Caption = "Resumes"
    })
    {
        Column.Employee,
        Column.File,
        Column._Employee.Email,
        Column._Employee.Department,
        Column._Employee.Address1
    };
}
