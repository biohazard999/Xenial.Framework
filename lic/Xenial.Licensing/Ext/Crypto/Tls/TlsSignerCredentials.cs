using System;
using System.IO;

namespace Xenial.Licensing.Ext.Crypto.Tls
{
    public interface TlsSignerCredentials
        :   TlsCredentials
    {
        /// <exception cref="IOException"></exception>
        byte[] GenerateCertificateSignature(byte[] hash);

        SignatureAndHashAlgorithm SignatureAndHashAlgorithm { get; }
    }
}
