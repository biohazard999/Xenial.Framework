using System;

namespace Xenial.Licensing.Ext.Utilities.IO.Pem
{
	public interface PemObjectGenerator
	{
		/// <returns>
		/// A <see cref="PemObject"/>
		/// </returns>
		/// <exception cref="PemGenerationException"></exception>
		PemObject Generate();
	}
}
