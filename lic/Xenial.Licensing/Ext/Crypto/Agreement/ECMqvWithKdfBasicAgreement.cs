using System;
using System.Collections;

using Xenial.Licensing.Ext.Asn1;
using Xenial.Licensing.Ext.Asn1.Nist;
using Xenial.Licensing.Ext.Asn1.Pkcs;
using Xenial.Licensing.Ext.Asn1.X9;
using Xenial.Licensing.Ext.Crypto.Agreement.Kdf;
using Xenial.Licensing.Ext.Crypto.Parameters;
using Xenial.Licensing.Ext.Math;
using Xenial.Licensing.Ext.Security;

namespace Xenial.Licensing.Ext.Crypto.Agreement
{
	public class ECMqvWithKdfBasicAgreement
		: ECMqvBasicAgreement
	{
		private readonly string algorithm;
		private readonly IDerivationFunction kdf;

		public ECMqvWithKdfBasicAgreement(
			string				algorithm,
			IDerivationFunction	kdf)
		{
			if (algorithm == null)
				throw new ArgumentNullException("algorithm");
			if (kdf == null)
				throw new ArgumentNullException("kdf");

			this.algorithm = algorithm;
			this.kdf = kdf;
		}

		public override BigInteger CalculateAgreement(
			ICipherParameters pubKey)
		{
			// Note that the ec.KeyAgreement class in JCE only uses kdf in one
			// of the engineGenerateSecret methods.

			BigInteger result = base.CalculateAgreement(pubKey);

			int keySize = GeneratorUtilities.GetDefaultKeySize(algorithm);

			DHKdfParameters dhKdfParams = new DHKdfParameters(
				new DerObjectIdentifier(algorithm),
				keySize,
				BigIntToBytes(result));

			kdf.Init(dhKdfParams);

			byte[] keyBytes = new byte[keySize / 8];
			kdf.GenerateBytes(keyBytes, 0, keyBytes.Length);

			return new BigInteger(1, keyBytes);
		}

		private byte[] BigIntToBytes(BigInteger r)
		{
			int byteLength = X9IntegerConverter.GetByteLength(privParams.StaticPrivateKey.Parameters.Curve);
			return X9IntegerConverter.IntegerToBytes(r, byteLength);
		}
	}
}
