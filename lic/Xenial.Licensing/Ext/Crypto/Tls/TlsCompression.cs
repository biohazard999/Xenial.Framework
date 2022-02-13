using System;
using System.IO;

namespace Xenial.Licensing.Ext.Crypto.Tls
{
	public interface TlsCompression
	{
		Stream Compress(Stream output);

		Stream Decompress(Stream output);
	}
}
