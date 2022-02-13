using System;

namespace Xenial.Licensing.Ext.Crypto.Tls
{
	public interface TlsCredentials
	{
		Certificate Certificate { get; }
	}
}
