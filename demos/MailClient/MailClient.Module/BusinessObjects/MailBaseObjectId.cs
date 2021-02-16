using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using DevExpress.Xpo;

namespace MailClient.Module.BusinessObjects
{
    [NonPersistent]
    public abstract class MailBaseObjectId : MailBaseObject
    {
        protected MailBaseObjectId(Session session) : base(session) { }

        [Key(AutoGenerate = true)]
        [Persistent("Id")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Needed by XPO")]
        private int id = -1;
        [PersistentAlias(nameof(id))]
        [Browsable(false)]
        public int Id => id;
    }
}
