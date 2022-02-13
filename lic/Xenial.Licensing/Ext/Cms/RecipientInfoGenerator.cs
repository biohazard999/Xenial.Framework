using System;

using Xenial.Licensing.Ext.Asn1.Cms;
using Xenial.Licensing.Ext.Crypto.Parameters;
using Xenial.Licensing.Ext.Security;

namespace Xenial.Licensing.Ext.Cms
{
	interface RecipientInfoGenerator
	{
		/// <summary>
		/// Generate a RecipientInfo object for the given key.
		/// </summary>
		/// <param name="contentEncryptionKey">
		/// A <see cref="KeyParameter"/>
		/// </param>
		/// <param name="random">
		/// A <see cref="SecureRandom"/>
		/// </param>
		/// <returns>
		/// A <see cref="RecipientInfo"/>
		/// </returns>
		/// <exception cref="GeneralSecurityException"></exception>
		RecipientInfo Generate(KeyParameter contentEncryptionKey, SecureRandom random);
	}
}
