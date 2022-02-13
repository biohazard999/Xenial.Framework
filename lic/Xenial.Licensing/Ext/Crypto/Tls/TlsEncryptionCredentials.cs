using System;
using System.IO;

namespace Xenial.Licensing.Ext.Crypto.Tls
{
    public interface TlsEncryptionCredentials
        :   TlsCredentials
    {
        /// <exception cref="IOException"></exception>
        byte[] DecryptPreMasterSecret(byte[] encryptedPreMasterSecret);
    }
}
