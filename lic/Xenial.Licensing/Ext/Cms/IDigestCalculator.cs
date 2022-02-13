using System;

namespace Xenial.Licensing.Ext.Cms
{
	internal interface IDigestCalculator
	{
		byte[] GetDigest();
	}
}
