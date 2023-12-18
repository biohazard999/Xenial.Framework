using System;

using Xenial.Licensing.Ext.Crypto.Parameters;
using Xenial.Licensing.Ext.Asn1;
using Xenial.Licensing.Ext.Asn1.X509;
using Xenial.Licensing.Ext.Asn1.X9;
using Xenial.Licensing.Ext.Crypto;
using Xenial.Licensing.Ext.Utilities;

namespace Xenial.Licensing.Ext.X509
{
    /// <summary>
    /// A factory to produce Public Key Info Objects.
    /// </summary>
    public sealed class SubjectPublicKeyInfoFactory
    {
        private SubjectPublicKeyInfoFactory()
        {
        }

        /// <summary>
        /// Create a Subject Public Key Info object for a given public key.
        /// </summary>
        /// <param name="key">One of ElGammalPublicKeyParameters, DSAPublicKeyParameter, DHPublicKeyParameters, RsaKeyParameters or ECPublicKeyParameters</param>
        /// <returns>A subject public key info object.</returns>
        /// <exception cref="Exception">Throw exception if object provided is not one of the above.</exception>
        public static SubjectPublicKeyInfo CreateSubjectPublicKeyInfo(
            AsymmetricKeyParameter key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (key.IsPrivate)
                throw new ArgumentException("Private key passed - public key expected.", "key");

            if (key is ECPublicKeyParameters)
            {
                ECPublicKeyParameters _key = (ECPublicKeyParameters)key;

                X962Parameters x962;
                if (_key.PublicKeyParamSet == null)
                {
                    ECDomainParameters kp = _key.Parameters;
                    X9ECParameters ecP = new X9ECParameters(kp.Curve, kp.G, kp.N, kp.H, kp.GetSeed());

                    x962 = new X962Parameters(ecP);
                }
                else
                {
                    x962 = new X962Parameters(_key.PublicKeyParamSet);
                }

                Asn1OctetString p = (Asn1OctetString)(new X9ECPoint(_key.Q).ToAsn1Object());

                AlgorithmIdentifier algID = new AlgorithmIdentifier(
                    X9ObjectIdentifiers.IdECPublicKey, x962.ToAsn1Object());

                return new SubjectPublicKeyInfo(algID, p.GetOctets());

            } // End of EC

            throw new ArgumentException("Class provided no convertible: " + Platform.GetTypeName(key));
        }
    }
}
