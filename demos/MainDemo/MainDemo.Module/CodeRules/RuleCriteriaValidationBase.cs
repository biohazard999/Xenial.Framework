using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Validation;

namespace MainDemo.Module.CodeRules {
    internal abstract class RuleCriteriaValidationBase : RuleBase {
        IMemberInfo targetMember;
        public RuleCriteriaValidationBase(IRuleBaseProperties properties) : base(properties) { }
        protected RuleCriteriaValidationBase(string id, ContextIdentifiers targetContextIDs, Type targetType) :
            base(id, targetContextIDs, targetType) {
        }

        private IMemberInfo TargetMember {
            get {
                if(targetMember == null) {
                    ITypeInfo targetTypeInfo = targetObjectSpace.TypesInfo.FindTypeInfo(Properties.TargetType);
                    targetMember = targetTypeInfo.FindMember(TargetPropertyName);
                }
                return targetMember;
            }
        }

        protected abstract string TargetPropertyName { get; }
        protected override bool IsValidInternal(object target, out string errorMessageTemplate) {
            if(!new CriteriaPropertyEditorHelper(TargetMember).ValidateCriteria(target, out errorMessageTemplate)) {
                if(!string.IsNullOrEmpty(errorMessageTemplate)) {
                    errorMessageTemplate = errorMessageTemplate.Replace('{', '(').Replace('}', ')');
                }
                return false;
            }
            return true;
        }

        public override ReadOnlyCollection<string> UsedProperties {
            get {
                return new ReadOnlyCollection<string>(new string[] { TargetPropertyName });
            }
        }
    }
}
