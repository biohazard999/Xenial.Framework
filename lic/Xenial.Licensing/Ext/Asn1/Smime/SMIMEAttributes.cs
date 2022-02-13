using Xenial.Licensing.Ext.Asn1;
using Xenial.Licensing.Ext.Asn1.Pkcs;

namespace Xenial.Licensing.Ext.Asn1.Smime
{
    public abstract class SmimeAttributes
    {
        public static readonly DerObjectIdentifier SmimeCapabilities = PkcsObjectIdentifiers.Pkcs9AtSmimeCapabilities;
        public static readonly DerObjectIdentifier EncrypKeyPref = PkcsObjectIdentifiers.IdAAEncrypKeyPref;
    }
}
