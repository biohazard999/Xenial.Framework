using System;

namespace Xenial.Licensing.Ext.Crypto.Tls
{
    public interface TlsPskIdentityManager
    {
        byte[] GetHint();

        byte[] GetPsk(byte[] identity);
    }
}
