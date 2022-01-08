using System;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

using Xenial.Framework.Layouts;

namespace MainDemo.Module.BusinessObjects
{
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty(nameof(Employee))]
    [RuleCriteria("Payroll_Hours_PayPeriod_Range", DefaultContexts.Save, "DateDiffHour(PayPeriodStart, PayPeriodEnd) >= [Hours] + [OvertimeHours]", CustomMessageTemplate = @"Sum of ""Hours"" and ""Overtime hours"" must be less than or equal to the difference between ""Pay Period End"" and ""Pay Period Start"" in hours.")]
    [DetailViewLayoutBuilder(typeof(PaycheckLayoutBuilder))]
    [ListViewColumnsBuilder(typeof(PaycheckColumnsBuilder))]
    public class Paycheck : BaseObject
    {
        private Employee employee;
        private int payPeriod;
        private DateTime paymentDate;
        private DateTime payPeriodEnd;
        private DateTime payPeriodStart;
        private decimal payRate;
        private int hours;
        private decimal overtimePayRate;
        private int overtimeHours;
        private double taxRate;
        private string notes;

        public Paycheck(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            var now = DateTime.Now;
            payPeriod = (2 * (now.Month - 1)) + (now.Day > 15 ? 2 : 1);
            payPeriodStart = new DateTime(day: 1, month: now.Month, year: now.Year);
            payPeriodEnd = new DateTime(day: (now.Day > 15 ? DateTime.DaysInMonth(now.Year, now.Month) : 15), month: now.Month, year: now.Year);
            paymentDate = payPeriodEnd;
        }
        [RuleRequiredField]
        [ImmediatePostData]
        public Employee Employee
        {
            get => employee;
            set => SetPropertyValue(nameof(Employee), ref employee, value);
        }
        [RuleRange(DefaultContexts.Save, 0, 26)]
        public int PayPeriod
        {
            get => payPeriod;
            set => SetPropertyValue(nameof(PayPeriod), ref payPeriod, value);
        }
        [RuleRequiredField]
        public DateTime PayPeriodStart
        {
            get => payPeriodStart;
            set => SetPropertyValue(nameof(PayPeriodStart), ref payPeriodStart, value);
        }
        [RuleRequiredField]
        [RuleValueComparison("Payroll_PeriodStart_PeriodEnd", DefaultContexts.Save, ValueComparisonType.GreaterThanOrEqual, nameof(PayPeriodStart), ParametersMode.Expression)]
        [RuleValueComparison("Payroll_PaymentDate_PeriodEnd", DefaultContexts.Save, ValueComparisonType.GreaterThanOrEqual, nameof(PaymentDate), ParametersMode.Expression)]
        public DateTime PayPeriodEnd
        {
            get => payPeriodEnd;
            set => SetPropertyValue(nameof(PayPeriodEnd), ref payPeriodEnd, value);
        }
        public DateTime PaymentDate
        {
            get => paymentDate;
            set => SetPropertyValue(nameof(PaymentDate), ref paymentDate, value);
        }
        [ImmediatePostData]
        [RuleValueComparison("Payroll_PayRate", DefaultContexts.Save, ValueComparisonType.GreaterThan, 0)]
        public decimal PayRate
        {
            get => payRate;
            set
            {
                if (SetPropertyValue(nameof(PayRate), ref payRate, value))
                {
                    NotifyCalculatedPropertiesChanged();
                }
            }
        }
        [ImmediatePostData]
        [RuleValueComparison("Payroll_Hours", DefaultContexts.Save, ValueComparisonType.GreaterThanOrEqual, 0)]
        public int Hours
        {
            get => hours;
            set
            {
                if (SetPropertyValue(nameof(Hours), ref hours, value))
                {
                    NotifyCalculatedPropertiesChanged();
                }
            }
        }
        [ImmediatePostData]
        [RuleValueComparison("Payroll_OvertimePayRate", DefaultContexts.Save, ValueComparisonType.GreaterThan, 0)]
        public decimal OvertimePayRate
        {
            get => overtimePayRate;
            set
            {
                if (SetPropertyValue(nameof(OvertimePayRate), ref overtimePayRate, value))
                {
                    NotifyCalculatedPropertiesChanged();
                }
            }
        }
        [ImmediatePostData]
        [RuleValueComparison("Payroll_OvertimeHours", DefaultContexts.Save, ValueComparisonType.GreaterThanOrEqual, 0)]
        public int OvertimeHours
        {
            get => overtimeHours;
            set
            {
                if (SetPropertyValue(nameof(OvertimeHours), ref overtimeHours, value))
                {
                    NotifyCalculatedPropertiesChanged();
                }
            }
        }
        [ImmediatePostData]
        [RuleRange(DefaultContexts.Save, 0, 100)]
        public double TaxRate
        {
            get => taxRate;
            set
            {
                if (SetPropertyValue(nameof(TaxRate), ref taxRate, value))
                {
                    NotifyCalculatedPropertiesChanged();
                }
            }
        }
        [Size(4096)]
        public string Notes
        {
            get => notes;
            set => SetPropertyValue(nameof(Notes), ref notes, value);
        }
        private void NotifyCalculatedPropertiesChanged()
        {
            OnChanged(nameof(TotalTax));
            OnChanged(nameof(GrossPay));
            OnChanged(nameof(NetPay));
        }
        [PersistentAlias("ToDecimal(((PayRate*Hours)+(OvertimePayRate*OvertimeHours))*TaxRate)")]
        public decimal TotalTax => (decimal)EvaluateAlias(nameof(TotalTax));
        [PersistentAlias("ToDecimal(((PayRate*Hours)+(OvertimePayRate*OvertimeHours)))")]
        public decimal GrossPay => (decimal)EvaluateAlias(nameof(GrossPay));
        [PersistentAlias("GrossPay-TotalTax")]
        public decimal NetPay => (decimal)EvaluateAlias(nameof(NetPay));
    }
}
