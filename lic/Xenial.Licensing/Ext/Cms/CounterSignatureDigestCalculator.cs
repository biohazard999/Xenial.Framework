using System;

using Xenial.Licensing.Ext.Crypto;
using Xenial.Licensing.Ext.Security;

namespace Xenial.Licensing.Ext.Cms
{
	internal class CounterSignatureDigestCalculator
		: IDigestCalculator
	{
		private readonly string alg;
		private readonly byte[] data;

		internal CounterSignatureDigestCalculator(
			string	alg,
			byte[]	data)
		{
			this.alg = alg;
			this.data = data;
		}

		public byte[] GetDigest()
		{
			IDigest digest = CmsSignedHelper.Instance.GetDigestInstance(alg);
			return DigestUtilities.DoFinal(digest, data);
		}
	}
}
