using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MainDemo.Module.BusinessObjects {
    [DefaultClassOptions]
    public class Employee : Person, IMapsMarker {
        private string webPageAddress;
        private Employee manager;
        private string nickName;
        private string spouseName;
        private TitleOfCourtesy titleOfCourtesy;
        private string notes;
        private DateTime? anniversary;
        public Employee(Session session) :
            base(session) {
        }
        [RuleRegularExpression(@"(((http|https)\://)|(www.)[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;amp;%\$#\=~])*)|([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6})", CustomMessageTemplate = @"Invalid ""Web Page Address"".")]
        public string WebPageAddress {
            get {
                return webPageAddress;
            }
            set {
                SetPropertyValue(nameof(WebPageAddress), ref webPageAddress, value);
            }
        }
        [DataSourceProperty("Department.Employees", DataSourcePropertyIsNullMode.SelectAll)]
        [DataSourceCriteria("Position.Title = 'Manager' AND Oid != '@This.Oid'")]
        public Employee Manager {
            get {
                return manager;
            }
            set {
                SetPropertyValue(nameof(Manager), ref manager, value);
            }
        }
        public string NickName {
            get {
                return nickName;
            }
            set {
                SetPropertyValue(nameof(NickName), ref nickName, value);
            }
        }
        public string SpouseName {
            get {
                return spouseName;
            }
            set {
                SetPropertyValue(nameof(SpouseName), ref spouseName, value);
            }
        }
        public TitleOfCourtesy TitleOfCourtesy {
            get {
                return titleOfCourtesy;
            }
            set {
                SetPropertyValue(nameof(TitleOfCourtesy), ref titleOfCourtesy, value);
            }
        }
        public DateTime? Anniversary {
            get {
                return anniversary;
            }
            set {
                SetPropertyValue(nameof(Anniversary), ref anniversary, value);
            }
        }
        [Size(4096)]
        public string Notes {
            get {
                return notes;
            }
            set {
                SetPropertyValue(nameof(Notes), ref notes, value);
            }
        }
        private Department department;
        [Association("Department-Employees"), ImmediatePostData]
        public Department Department {
            get {
                return department;
            }
            set {
                SetPropertyValue(nameof(Department), ref department, value);
                if(!IsLoading) {
                    Position = null;
                    if(Manager != null && Manager.Department != value) {
                        Manager = null;
                    }
                }
            }
        }
        private Position position;
        public Position Position {
            get {
                return position;
            }
            set {
                SetPropertyValue(nameof(Position), ref position, value);
            }
        }
        [Association("Employee-DemoTask")]
        public XPCollection<DemoTask> Tasks {
            get {
                return GetCollection<DemoTask>(nameof(Tasks));
            }
        }
        private XPCollection<AuditDataItemPersistent> changeHistory;
        [CollectionOperationSet(AllowAdd = false, AllowRemove = false)]
        public XPCollection<AuditDataItemPersistent> ChangeHistory {
            get {
                if(changeHistory == null) {
                    changeHistory = AuditedObjectWeakReference.GetAuditTrail(Session, this);
                }
                return changeHistory;
            }
        }

        private Location location;
        [Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never), VisibleInListView(false)]
        public Location Location {
            get {
                return location;
            }
            set {
                SetPropertyValue(nameof(Location), ref location, value);
            }
        }
        [VisibleInListView(false), VisibleInDetailView(false)]
        [PersistentAlias("FullName")]
        [System.ComponentModel.Browsable(false)]
        public string Title {
            get { return Convert.ToString(EvaluateAlias(nameof(Title))); }
        }
        [VisibleInListView(false), VisibleInDetailView(false)]
        [PersistentAlias("Location.Latitude")]
        public double Latitude {
            get { return Convert.ToDouble(EvaluateAlias(nameof(Latitude))); }
        }
        [VisibleInListView(false), VisibleInDetailView(false)]
        [PersistentAlias("Location.Longitude")]
        public double Longitude {
            get { return Convert.ToDouble(EvaluateAlias(nameof(Longitude))); }
        }

        public override void AfterConstruction() {
            base.AfterConstruction();

            location = new Location(Session);
            location.Employee = this;
        }
    }
    public enum TitleOfCourtesy {
        Dr,
        Miss,
        Mr,
        Mrs,
        Ms
    };
}
