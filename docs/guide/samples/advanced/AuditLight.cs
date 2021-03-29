using DevExpress.ExpressApp;  
using DevExpress.ExpressApp.Model;  
using DevExpress.Persistent.Base;  
using DevExpress.Persistent.BaseImpl;  
using DevExpress.Persistent.BaseImpl.PermissionPolicy;  
using DevExpress.Xpo;  
using System;  
using System.ComponentModel;  

namespace YourSolutionName.Module.BusinessObjects {  
    [DefaultClassOptions]  
    public class Contact : BaseObject, IAuditableLight {  
        public Contact(Session session)  
            : base(session) {  
        }  
        PermissionPolicyUser GetCurrentUser() {  
            //return Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);  // In XAF apps for versions older than v20.1.7.
            return Session.FindObject<PermissionPolicyUser>(CriteriaOperator.Parse("Oid=CurrentUserId()"));  // In non-XAF apps where SecuritySystem.Instance is unavailable (v20.1.7+).
        }  
        public override void AfterConstruction() {  
            base.AfterConstruction();  
            CreatedOn = DateTime.Now;  
            CreatedBy = GetCurrentUser();  
        }  
        protected override void OnSaving() {  
            base.OnSaving();  
            UpdatedOn = DateTime.Now;  
            UpdatedBy = GetCurrentUser();  
        }  
        PermissionPolicyUser createdBy;  
        [ModelDefault("AllowEdit", "False")]  
        public PermissionPolicyUser CreatedBy {  
            get { return createdBy; }  
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }  
        }  
        DateTime createdOn;  
        [ModelDefault("AllowEdit", "False"), ModelDefault("DisplayFormat", "G")]  
        public DateTime CreatedOn {  
            get { return createdOn; }  
            set { SetPropertyValue("CreatedOn", ref createdOn, value); }  
        }  
        PermissionPolicyUser updatedBy;  
        [ModelDefault("AllowEdit", "False")]  
        public PermissionPolicyUser UpdatedBy {  
            get { return updatedBy; }  
            set { SetPropertyValue("UpdatedBy", ref updatedBy, value); }  
        }  
        DateTime updatedOn;  
        [ModelDefault("AllowEdit", "False"), ModelDefault("DisplayFormat", "G")]  
        public DateTime UpdatedOn {  
            get { return updatedOn; }  
            set { SetPropertyValue("UpdatedOn", ref updatedOn, value); }  
        }  
    }  

    public interface IAuditableLight {
        PermissionPolicyUser CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
        PermissionPolicyUser UpdatedBy { get; set; }
        DateTime UpdatedOn { get; set; }
    }
}  