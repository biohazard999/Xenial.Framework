using System;

namespace Xenial.Licensing.Ext.Crypto.Tls
{
    public abstract class AbstractTlsCredentials
        :   TlsCredentials
    {
        public abstract Certificate Certificate { get; }
    }
}
