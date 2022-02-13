using System;

namespace Xenial.Licensing.Ext.Crypto.Modes.Gcm
{
	public interface IGcmMultiplier
	{
		void Init(byte[] H);
		void MultiplyH(byte[] x);
	}
}
