using System;

using Xenial.Licensing.Ext.Asn1;
using Xenial.Licensing.Ext.Asn1.Cms;
using Xenial.Licensing.Ext.Asn1.Pkcs;
using Xenial.Licensing.Ext.Asn1.X509;
using Xenial.Licensing.Ext.Crypto;
using Xenial.Licensing.Ext.Crypto.Parameters;
using Xenial.Licensing.Ext.Security;
using Xenial.Licensing.Ext.Utilities;

namespace Xenial.Licensing.Ext.Cms
{
	internal class PasswordRecipientInfoGenerator : RecipientInfoGenerator
	{
		private static readonly CmsEnvelopedHelper Helper = CmsEnvelopedHelper.Instance;

		private AlgorithmIdentifier	keyDerivationAlgorithm;
		private KeyParameter		keyEncryptionKey;
		// TODO Can get this from keyEncryptionKey?		
		private string				keyEncryptionKeyOID;

		internal PasswordRecipientInfoGenerator()
		{
		}

		internal AlgorithmIdentifier KeyDerivationAlgorithm
		{
			set { this.keyDerivationAlgorithm = value; }
		}

		internal KeyParameter KeyEncryptionKey
		{
			set { this.keyEncryptionKey = value; }
		}

		internal string KeyEncryptionKeyOID
		{
			set { this.keyEncryptionKeyOID = value; }
		}

		public RecipientInfo Generate(KeyParameter contentEncryptionKey, SecureRandom random)
		{
			byte[] keyBytes = contentEncryptionKey.GetKey();

			string rfc3211WrapperName = Helper.GetRfc3211WrapperName(keyEncryptionKeyOID);
			IWrapper keyWrapper = Helper.CreateWrapper(rfc3211WrapperName);

			// Note: In Java build, the IV is automatically generated in JCE layer
			int ivLength = Platform.StartsWith(rfc3211WrapperName, "DESEDE") ? 8 : 16;
			byte[] iv = new byte[ivLength];
			random.NextBytes(iv);

			ICipherParameters parameters = new ParametersWithIV(keyEncryptionKey, iv);
			keyWrapper.Init(true, new ParametersWithRandom(parameters, random));
        	Asn1OctetString encryptedKey = new DerOctetString(
				keyWrapper.Wrap(keyBytes, 0, keyBytes.Length));

			DerSequence seq = new DerSequence(
				new DerObjectIdentifier(keyEncryptionKeyOID),
				new DerOctetString(iv));

			AlgorithmIdentifier keyEncryptionAlgorithm = new AlgorithmIdentifier(
				PkcsObjectIdentifiers.IdAlgPwriKek, seq);

			return new RecipientInfo(new PasswordRecipientInfo(
				keyDerivationAlgorithm, keyEncryptionAlgorithm, encryptedKey));
		}
	}
}
