using System;
using System.Collections;
using System.IO;
using System.Text;

using Xenial.Licensing.Ext.Asn1;
using Xenial.Licensing.Ext.Asn1.X509;
using Xenial.Licensing.Ext.Asn1.X9;
using Xenial.Licensing.Ext.Crypto;
using Xenial.Licensing.Ext.Crypto.Generators;
using Xenial.Licensing.Ext.Crypto.Parameters;
using Xenial.Licensing.Ext.Math;
using Xenial.Licensing.Ext.Math.EC;

namespace Xenial.Licensing.Ext.Security
{
    public sealed class PublicKeyFactory
    {
        private PublicKeyFactory()
        {
        }

        public static AsymmetricKeyParameter CreateKey(
            byte[] keyInfoData)
        {
            return CreateKey(
                SubjectPublicKeyInfo.GetInstance(
                    Asn1Object.FromByteArray(keyInfoData)));
        }

        public static AsymmetricKeyParameter CreateKey(
            Stream inStr)
        {
            return CreateKey(
                SubjectPublicKeyInfo.GetInstance(
                    Asn1Object.FromStream(inStr)));
        }

        public static AsymmetricKeyParameter CreateKey(
            SubjectPublicKeyInfo keyInfo)
        {
            AlgorithmIdentifier algID = keyInfo.AlgorithmID;
            DerObjectIdentifier algOid = algID.Algorithm;

            if (algOid.Equals(X9ObjectIdentifiers.IdECPublicKey))
            {
                X962Parameters para = new X962Parameters(algID.Parameters.ToAsn1Object());

                X9ECParameters x9;
                if (para.IsNamedCurve)
                {
                    x9 = ECKeyPairGenerator.FindECCurveByOid((DerObjectIdentifier)para.Parameters);
                }
                else
                {
                    x9 = new X9ECParameters((Asn1Sequence)para.Parameters);
                }

                Asn1OctetString key = new DerOctetString(keyInfo.PublicKeyData.GetBytes());
                X9ECPoint derQ = new X9ECPoint(x9.Curve, key);
                ECPoint q = derQ.Point;

                if (para.IsNamedCurve)
                {
                    return new ECPublicKeyParameters("EC", q, (DerObjectIdentifier)para.Parameters);
                }

                ECDomainParameters dParams = new ECDomainParameters(x9.Curve, x9.G, x9.N, x9.H, x9.GetSeed());
                return new ECPublicKeyParameters(q, dParams);
            }
            else
            {
                throw new SecurityUtilityException("algorithm identifier in key not recognised: " + algOid);
            }
        }

        private static bool IsPkcsDHParam(Asn1Sequence seq)
        {
            if (seq.Count == 2)
                return true;

            if (seq.Count > 3)
                return false;

            DerInteger l = DerInteger.GetInstance(seq[2]);
            DerInteger p = DerInteger.GetInstance(seq[0]);

            return l.Value.CompareTo(BigInteger.ValueOf(p.Value.BitLength)) <= 0;
        }
    }
}
