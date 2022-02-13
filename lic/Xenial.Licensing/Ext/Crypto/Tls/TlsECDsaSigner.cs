using System;

using Xenial.Licensing.Ext.Crypto.Parameters;
using Xenial.Licensing.Ext.Crypto.Signers;

namespace Xenial.Licensing.Ext.Crypto.Tls
{
    public class TlsECDsaSigner
        :   TlsDsaSigner
    {
        public override bool IsValidPublicKey(AsymmetricKeyParameter publicKey)
        {
            return publicKey is ECPublicKeyParameters;
        }

        protected override IDsa CreateDsaImpl(byte hashAlgorithm)
        {
            return new ECDsaSigner(new HMacDsaKCalculator(TlsUtilities.CreateHash(hashAlgorithm)));
        }

        protected override byte SignatureAlgorithm
        {
            get { return Tls.SignatureAlgorithm.ecdsa; }
        }
    }
}
