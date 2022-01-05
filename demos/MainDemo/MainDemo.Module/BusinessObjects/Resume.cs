using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

using Xenial.Framework.Layouts;

namespace MainDemo.Module.BusinessObjects
{
    [FileAttachment(nameof(File))]
    [DefaultClassOptions, ImageName("BO_Resume")]
    [ListViewColumnsBuilder(typeof(ResumeColumnsBuilder))]
    public class Resume : BaseObject
    {
        private Employee employee;
        private FileData file;
        public Resume(Session session)
            : base(session)
        {
        }
        [Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        [RuleRequiredField]
        public FileData File
        {
            get => file;
            set => SetPropertyValue(nameof(File), ref file, value);
        }
        [RuleRequiredField]
        public Employee Employee
        {
            get => employee;
            set => SetPropertyValue(nameof(Employee), ref employee, value);
        }
        [Aggregated, Association("Resume-PortfolioFileData")]
        public XPCollection<PortfolioFileData> Portfolio => GetCollection<PortfolioFileData>(nameof(Portfolio));
    }
}
