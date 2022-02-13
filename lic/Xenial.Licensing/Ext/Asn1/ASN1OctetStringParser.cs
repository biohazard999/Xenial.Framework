using System.IO;

namespace Xenial.Licensing.Ext.Asn1
{
	public interface Asn1OctetStringParser
		: IAsn1Convertible
	{
		Stream GetOctetStream();
	}
}
