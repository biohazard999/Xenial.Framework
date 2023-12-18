using System;
using System.Collections.Generic;
using System.Text;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;

namespace MainDemo.Module.BusinessObjects;

[Xenial.XenialExpandMember(Constants.Employee)]
public partial class PaycheckLayoutBuilder : LayoutBuilder<Paycheck>
{
    public Layout BuildLayout() => new()
    {
        VerticalGroup("Employee Details",
            Editor.Employee,
            Editor._Employee.Email
        ),
        VerticalGroup("Period",
            HorizontalGroup(
                VerticalGroup(
                    Editor.PayPeriod,
                    Editor.PayPeriodStart
                ),
                VerticalGroup(
                    Editor.PaymentDate,
                    Editor.PayPeriodEnd
                )
            )
        ),
        HorizontalGroup(
            VerticalGroup("Total Payroll",
                Editor.GrossPay,
                Editor.TotalTax,
                Editor.NetPay
            ),
            VerticalGroup("Payment Details",
                Editor.TaxRate,
                Editor.PayRate,
                Editor.Hours
            )
        ),
        HorizontalGroup("Overtime Pay",
            Editor.OvertimePayRate,
            Editor.OvertimeHours
        ),
        VerticalGroup("Notes",
            Editor.Notes with { ShowCaption = false }
        )
    };
}
