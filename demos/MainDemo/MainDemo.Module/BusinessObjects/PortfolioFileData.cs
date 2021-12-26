using System;
using System.Collections.Generic;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace MainDemo.Module.BusinessObjects
{
    [ImageName("BO_FileAttachment")]
    public class PortfolioFileData : FileAttachmentBase
    {
        public PortfolioFileData(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            documentType = DocumentType.Unknown;
        }
        protected Resume resume;
        [Association("Resume-PortfolioFileData")]
        public Resume Resume
        {
            get => resume;
            set => SetPropertyValue<Resume>(nameof(Resume), ref resume, value);
        }
        private DocumentType documentType;
        public DocumentType DocumentType
        {
            get => documentType;
            set => SetPropertyValue<DocumentType>(nameof(DocumentType), ref documentType, value);
        }
    }
    public enum DocumentType
    {
        SourceCode = 1,
        Tests = 2,
        Documentation = 3,
        Diagrams = 4,
        Screenshots = 5,
        Unknown = 6
    };
}
