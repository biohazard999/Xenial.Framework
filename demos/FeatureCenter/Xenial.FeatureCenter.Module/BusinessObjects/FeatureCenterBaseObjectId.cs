using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Markdig;

namespace Xenial.FeatureCenter.Module.BusinessObjects
{
    [NonPersistent]
    public abstract class FeatureCenterBaseObjectId : FeatureCenterBaseObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XenialLicenseBaseObjectId"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public FeatureCenterBaseObjectId(Session session) : base(session) { }

        [Key(AutoGenerate = true)]
        [Persistent("Id")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Needed by XPO")]
        private int id = -1;
        [PersistentAlias(nameof(id))]
        [Browsable(false)]
        public int Id => id;
    }
}
