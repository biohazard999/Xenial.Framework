using System;

using Xenial.Licensing.Ext.Asn1.Pkcs;
using Xenial.Licensing.Ext.Asn1.Sec;
using Xenial.Licensing.Ext.Asn1.X509;
using Xenial.Licensing.Ext.Asn1.X9;
using Xenial.Licensing.Ext.Crypto;
using Xenial.Licensing.Ext.Crypto.Parameters;
using Xenial.Licensing.Ext.Security;
using Xenial.Licensing.Ext.Utilities;

namespace Xenial.Licensing.Ext.Pkcs
{
    public sealed class PrivateKeyInfoFactory
    {
        private PrivateKeyInfoFactory()
        {
        }

        public static PrivateKeyInfo CreatePrivateKeyInfo(
            AsymmetricKeyParameter key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (!key.IsPrivate)
                throw new ArgumentException("Public key passed - private key expected", "key");

            if (key is ECPrivateKeyParameters)
            {
                ECPrivateKeyParameters priv = (ECPrivateKeyParameters)key;
                ECDomainParameters dp = priv.Parameters;
                int orderBitLength = dp.N.BitLength;

                AlgorithmIdentifier algID;
                ECPrivateKeyStructure ec;


                X962Parameters x962;
                if (priv.PublicKeyParamSet == null)
                {
                    X9ECParameters ecP = new X9ECParameters(dp.Curve, dp.G, dp.N, dp.H, dp.GetSeed());
                    x962 = new X962Parameters(ecP);
                }
                else
                {
                    x962 = new X962Parameters(priv.PublicKeyParamSet);
                }

                // TODO Possible to pass the publicKey bitstring here?
                ec = new ECPrivateKeyStructure(orderBitLength, priv.D, x962);

                algID = new AlgorithmIdentifier(X9ObjectIdentifiers.IdECPublicKey, x962);

                return new PrivateKeyInfo(algID, ec);
            }

            throw new ArgumentException("Class provided is not convertible: " + Platform.GetTypeName(key));
        }

        public static PrivateKeyInfo CreatePrivateKeyInfo(
            char[] passPhrase,
            EncryptedPrivateKeyInfo encInfo)
        {
            return CreatePrivateKeyInfo(passPhrase, false, encInfo);
        }

        public static PrivateKeyInfo CreatePrivateKeyInfo(
            char[] passPhrase,
            bool wrongPkcs12Zero,
            EncryptedPrivateKeyInfo encInfo)
        {
            AlgorithmIdentifier algID = encInfo.EncryptionAlgorithm;

            IBufferedCipher cipher = PbeUtilities.CreateEngine(algID) as IBufferedCipher;
            if (cipher == null)
                throw new Exception("Unknown encryption algorithm: " + algID.Algorithm);

            ICipherParameters cipherParameters = PbeUtilities.GenerateCipherParameters(
                algID, passPhrase, wrongPkcs12Zero);
            cipher.Init(false, cipherParameters);
            byte[] keyBytes = cipher.DoFinal(encInfo.GetEncryptedData());

            return PrivateKeyInfo.GetInstance(keyBytes);
        }
    }
}
