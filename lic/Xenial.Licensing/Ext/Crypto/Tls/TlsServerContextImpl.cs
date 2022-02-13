using System;

using Xenial.Licensing.Ext.Security;

namespace Xenial.Licensing.Ext.Crypto.Tls
{
    internal class TlsServerContextImpl
        : AbstractTlsContext, TlsServerContext
    {
        internal TlsServerContextImpl(SecureRandom secureRandom, SecurityParameters securityParameters)
            : base(secureRandom, securityParameters)
        {
        }

        public override bool IsServer
        {
            get { return true; }
        }
    }
}
