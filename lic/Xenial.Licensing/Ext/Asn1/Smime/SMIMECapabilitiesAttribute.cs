using Xenial.Licensing.Ext.Asn1;
using Xenial.Licensing.Ext.Asn1.X509;

namespace Xenial.Licensing.Ext.Asn1.Smime
{
    public class SmimeCapabilitiesAttribute
        : AttributeX509
    {
        public SmimeCapabilitiesAttribute(
            SmimeCapabilityVector capabilities)
            : base(SmimeAttributes.SmimeCapabilities,
                    new DerSet(new DerSequence(capabilities.ToAsn1EncodableVector())))
        {
        }
    }
}
