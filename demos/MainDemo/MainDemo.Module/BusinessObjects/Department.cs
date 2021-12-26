﻿using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MainDemo.Module.BusinessObjects {
    [DefaultClassOptions]
    [RuleCriteria("Department_PositionsIsNotEmpty", DefaultContexts.Save, "Positions.Count > 0", CustomMessageTemplate = "The Department must contain at least one position.")]
    [RuleCriteria("Department_EmployeesIsNotEmpty", DefaultContexts.Save, "Employees.Count > 0", CustomMessageTemplate = "The Department must contain at least one employee.")]
    [System.ComponentModel.DefaultProperty(nameof(Department.Title))]
    public class Department : BaseObject {
        private string title;
        private string description;
        private Employee departmentHead;
        private string office;
        private string location;
        public Department(Session session)
            : base(session) {
        }
        [RuleRequiredField]
        public string Title {
            get {
                return title;
            }
            set {
                SetPropertyValue(nameof(Title), ref title, value);
            }
        }
        [Size(4096)]
        public string Description {
            get {
                return description;
            }
            set {
                SetPropertyValue(nameof(Description), ref description, value);
            }
        }
        [DataSourceProperty("Employees", DataSourcePropertyIsNullMode.SelectAll)]
        [RuleRequiredField]
        public Employee DepartmentHead {
            get {
                return departmentHead;
            }
            set {
                SetPropertyValue(nameof(DepartmentHead), ref departmentHead, value);
            }
        }
        public string Location {
            get {
                return location;
            }
            set {
                SetPropertyValue(nameof(Location), ref location, value);
            }
        }
        public string Office {
            get {
                return office;
            }
            set {
                SetPropertyValue(nameof(Office), ref office, value);
            }
        }
        [Association("Department-Employees")]
        public XPCollection<Employee> Employees {
            get {
                return GetCollection<Employee>(nameof(Employees));
            }
        }
        [Association("Departments-Positions")]
        public XPCollection<Position> Positions {
            get {
                return GetCollection<Position>(nameof(Positions));
            }
        }
    }
}
