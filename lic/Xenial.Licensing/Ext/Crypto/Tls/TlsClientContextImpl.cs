using System;

using Xenial.Licensing.Ext.Security;

namespace Xenial.Licensing.Ext.Crypto.Tls
{
    internal class TlsClientContextImpl
        :   AbstractTlsContext, TlsClientContext
    {
        internal TlsClientContextImpl(SecureRandom secureRandom, SecurityParameters securityParameters)
            :   base(secureRandom, securityParameters)
        {
        }

        public override bool IsServer
        {
            get { return false; }
        }
    }
}
