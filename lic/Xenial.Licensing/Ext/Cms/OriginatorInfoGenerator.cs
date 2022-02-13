using System;
using System.Collections;

using Xenial.Licensing.Ext.Asn1;
using Xenial.Licensing.Ext.Asn1.Cms;
using Xenial.Licensing.Ext.Utilities;
using Xenial.Licensing.Ext.X509;
using Xenial.Licensing.Ext.X509.Store;

namespace Xenial.Licensing.Ext.Cms
{
    public class OriginatorInfoGenerator
    {
        private readonly IList origCerts;
        private readonly IList origCrls;

        public OriginatorInfoGenerator(X509Certificate origCert)
        {
            this.origCerts = Platform.CreateArrayList(1);
            this.origCrls = null;
            origCerts.Add(origCert.CertificateStructure);
        }

        public OriginatorInfoGenerator(IX509Store origCerts)
            : this(origCerts, null)
        {
        }

        public OriginatorInfoGenerator(IX509Store origCerts, IX509Store origCrls)
        {
            this.origCerts = CmsUtilities.GetCertificatesFromStore(origCerts);
            this.origCrls = origCrls == null ? null : CmsUtilities.GetCrlsFromStore(origCrls);
        }

        public virtual OriginatorInfo Generate()
        {
            Asn1Set certSet = CmsUtilities.CreateDerSetFromList(origCerts);
            Asn1Set crlSet = origCrls == null ? null : CmsUtilities.CreateDerSetFromList(origCrls);
            return new OriginatorInfo(certSet, crlSet);
        }
    }
}
