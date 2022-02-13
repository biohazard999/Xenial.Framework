using System;

using Xenial.Licensing.Ext.Asn1;
using Xenial.Licensing.Ext.Asn1.Cms;
using Xenial.Licensing.Ext.Asn1.Kisa;
using Xenial.Licensing.Ext.Asn1.Nist;
using Xenial.Licensing.Ext.Asn1.Ntt;
using Xenial.Licensing.Ext.Asn1.Pkcs;
using Xenial.Licensing.Ext.Asn1.X509;
using Xenial.Licensing.Ext.Crypto;
using Xenial.Licensing.Ext.Crypto.Parameters;
using Xenial.Licensing.Ext.Security;
using Xenial.Licensing.Ext.Utilities;

namespace Xenial.Licensing.Ext.Cms
{
	internal class KekRecipientInfoGenerator : RecipientInfoGenerator
	{
		private static readonly CmsEnvelopedHelper Helper = CmsEnvelopedHelper.Instance;

		private KeyParameter	keyEncryptionKey;
		// TODO Can get this from keyEncryptionKey?		
		private string			keyEncryptionKeyOID;
		private KekIdentifier	kekIdentifier;

		// Derived
		private AlgorithmIdentifier keyEncryptionAlgorithm;

		internal KekRecipientInfoGenerator()
		{
		}

		internal KekIdentifier KekIdentifier
		{
			set { this.kekIdentifier = value; }
		}

		internal KeyParameter KeyEncryptionKey
		{
			set
			{
				this.keyEncryptionKey = value;
				this.keyEncryptionAlgorithm = DetermineKeyEncAlg(keyEncryptionKeyOID, keyEncryptionKey);
			}
		}

		internal string KeyEncryptionKeyOID
		{
			set { this.keyEncryptionKeyOID = value; }
		}

		public RecipientInfo Generate(KeyParameter contentEncryptionKey, SecureRandom random)
		{
			byte[] keyBytes = contentEncryptionKey.GetKey();

            IWrapper keyWrapper = Helper.CreateWrapper(keyEncryptionAlgorithm.Algorithm.Id);
			keyWrapper.Init(true, new ParametersWithRandom(keyEncryptionKey, random));
        	Asn1OctetString encryptedKey = new DerOctetString(
				keyWrapper.Wrap(keyBytes, 0, keyBytes.Length));

			return new RecipientInfo(new KekRecipientInfo(kekIdentifier, keyEncryptionAlgorithm, encryptedKey));
		}

		private static AlgorithmIdentifier DetermineKeyEncAlg(
			string algorithm, KeyParameter key)
		{
			if (Platform.StartsWith(algorithm, "DES"))
			{
				return new AlgorithmIdentifier(
					PkcsObjectIdentifiers.IdAlgCms3DesWrap,
					DerNull.Instance);
			}
            else if (Platform.StartsWith(algorithm, "RC2"))
			{
				return new AlgorithmIdentifier(
					PkcsObjectIdentifiers.IdAlgCmsRC2Wrap,
					new DerInteger(58));
			}
			else if (Platform.StartsWith(algorithm, "AES"))
			{
				int length = key.GetKey().Length * 8;
				DerObjectIdentifier wrapOid;

				if (length == 128)
				{
					wrapOid = NistObjectIdentifiers.IdAes128Wrap;
				}
				else if (length == 192)
				{
					wrapOid = NistObjectIdentifiers.IdAes192Wrap;
				}
				else if (length == 256)
				{
					wrapOid = NistObjectIdentifiers.IdAes256Wrap;
				}
				else
				{
					throw new ArgumentException("illegal keysize in AES");
				}

				return new AlgorithmIdentifier(wrapOid);  // parameters absent
			}
			else if (Platform.StartsWith(algorithm, "SEED"))
			{
				// parameters absent
				return new AlgorithmIdentifier(KisaObjectIdentifiers.IdNpkiAppCmsSeedWrap);
			}
			else if (Platform.StartsWith(algorithm, "CAMELLIA"))
			{
				int length = key.GetKey().Length * 8;
				DerObjectIdentifier wrapOid;

				if (length == 128)
				{
					wrapOid = NttObjectIdentifiers.IdCamellia128Wrap;
				}
				else if (length == 192)
				{
					wrapOid = NttObjectIdentifiers.IdCamellia192Wrap;
				}
				else if (length == 256)
				{
					wrapOid = NttObjectIdentifiers.IdCamellia256Wrap;
				}
				else
				{
					throw new ArgumentException("illegal keysize in Camellia");
				}

				return new AlgorithmIdentifier(wrapOid); // parameters must be absent
			}
			else
			{
				throw new ArgumentException("unknown algorithm");
			}
		}
	}
}
