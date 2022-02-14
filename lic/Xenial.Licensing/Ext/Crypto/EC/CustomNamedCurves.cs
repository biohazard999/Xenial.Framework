using System;
using System.Collections;

using Xenial.Licensing.Ext.Asn1;
using Xenial.Licensing.Ext.Asn1.Sec;
using Xenial.Licensing.Ext.Asn1.X9;
using Xenial.Licensing.Ext.Math;
using Xenial.Licensing.Ext.Math.EC;

using Xenial.Licensing.Ext.Math.EC.Custom.Sec;

using Xenial.Licensing.Ext.Utilities;

using Xenial.Licensing.Ext.Utilities.Encoders;

namespace Xenial.Licensing.Ext.Crypto.EC
{
    public sealed class CustomNamedCurves
    {
        private CustomNamedCurves()
        {
        }

        private static BigInteger FromHex(string hex)
        {
            return new BigInteger(1, Hex.Decode(hex));
        }

        private static ECCurve ConfigureCurve(ECCurve curve)
        {
            return curve;
        }

        /*
         * secp256r1
         */
        internal class SecP256R1Holder
            : X9ECParametersHolder
        {
            private SecP256R1Holder() {}

            internal static readonly X9ECParametersHolder Instance = new SecP256R1Holder();

            protected override X9ECParameters CreateParameters()
            {
                byte[] S = Hex.Decode("C49D360886E704936A6678E1139D26B7819F7E90");
                ECCurve curve = ConfigureCurve(new SecP256R1Curve());
                X9ECPoint G = new X9ECPoint(curve, Hex.Decode("04"
                    + "6B17D1F2E12C4247F8BCE6E563A440F277037D812DEB33A0F4A13945D898C296"
                    + "4FE342E2FE1A7F9B8EE7EB4A7C0F9E162BCE33576B315ECECBB6406837BF51F5"));
                return new X9ECParameters(curve, G, curve.Order, curve.Cofactor, S);
            }
        }

        private static readonly IDictionary nameToCurve = Platform.CreateHashtable();
        private static readonly IDictionary nameToOid = Platform.CreateHashtable();
        private static readonly IDictionary oidToCurve = Platform.CreateHashtable();
        private static readonly IDictionary oidToName = Platform.CreateHashtable();
        private static readonly IList names = Platform.CreateArrayList();

        private static void DefineCurveWithOid(string name, DerObjectIdentifier oid, X9ECParametersHolder holder)
        {
            names.Add(name);
            oidToName.Add(oid, name);
            oidToCurve.Add(oid, holder);
            name = Platform.ToUpperInvariant(name);
            nameToOid.Add(name, oid);
            nameToCurve.Add(name, holder);
        }

        static CustomNamedCurves()
        {
            DefineCurveWithOid("secp256r1", SecObjectIdentifiers.SecP256r1, SecP256R1Holder.Instance);
        }

        /**
         * return the X9ECParameters object for the named curve represented by
         * the passed in object identifier. Null if the curve isn't present.
         *
         * @param oid an object identifier representing a named curve, if present.
         */
        public static X9ECParameters GetByOid(DerObjectIdentifier oid)
        {
            X9ECParametersHolder holder = (X9ECParametersHolder)oidToCurve[oid];
            return holder == null ? null : holder.Parameters;
        }
    }
}
