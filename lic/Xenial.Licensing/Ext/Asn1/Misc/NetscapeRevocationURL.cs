using Xenial.Licensing.Ext.Asn1;

namespace Xenial.Licensing.Ext.Asn1.Misc
{
    public class NetscapeRevocationUrl
        : DerIA5String
    {
        public NetscapeRevocationUrl(DerIA5String str)
			: base(str.GetString())
        {
        }

        public override string ToString()
        {
            return "NetscapeRevocationUrl: " + this.GetString();
        }
    }
}
