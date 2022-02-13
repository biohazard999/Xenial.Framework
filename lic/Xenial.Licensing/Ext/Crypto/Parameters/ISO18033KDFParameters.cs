using System;
using Xenial.Licensing.Ext.Crypto;

namespace Xenial.Licensing.Ext.Crypto.Parameters
{
	/**
	* parameters for Key derivation functions for ISO-18033
	*/
	public class Iso18033KdfParameters
		: IDerivationParameters
	{
		byte[]  seed;

		public Iso18033KdfParameters(
			byte[]  seed)
		{
			this.seed = seed;
		}

		public byte[] GetSeed()
		{
			return seed;
		}
	}
}
