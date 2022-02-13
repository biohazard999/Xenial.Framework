using System;

using Xenial.Licensing.Ext.Asn1.X509;
using Xenial.Licensing.Ext.Crypto.Parameters;

namespace Xenial.Licensing.Ext.Cms
{
	internal interface CmsSecureReadable
	{
		AlgorithmIdentifier Algorithm { get; }
		object CryptoObject { get; }
		CmsReadable GetReadable(KeyParameter key);
	}
}
