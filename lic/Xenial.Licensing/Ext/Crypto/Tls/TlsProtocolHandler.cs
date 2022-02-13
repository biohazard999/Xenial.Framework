using System;
using System.Collections;
using System.IO;
using System.Text;

using Xenial.Licensing.Ext.Asn1;
using Xenial.Licensing.Ext.Asn1.X509;
using Xenial.Licensing.Ext.Crypto.Agreement;
using Xenial.Licensing.Ext.Crypto.Agreement.Srp;
using Xenial.Licensing.Ext.Crypto.Digests;
using Xenial.Licensing.Ext.Crypto.Encodings;
using Xenial.Licensing.Ext.Crypto.Engines;
using Xenial.Licensing.Ext.Crypto.Generators;
using Xenial.Licensing.Ext.Crypto.IO;
using Xenial.Licensing.Ext.Crypto.Parameters;
using Xenial.Licensing.Ext.Crypto.Prng;
using Xenial.Licensing.Ext.Math;
using Xenial.Licensing.Ext.Security;
using Xenial.Licensing.Ext.Utilities;
using Xenial.Licensing.Ext.Utilities.Date;

namespace Xenial.Licensing.Ext.Crypto.Tls
{
    [Obsolete("Use 'TlsClientProtocol' instead")]
    public class TlsProtocolHandler
        :   TlsClientProtocol
    {
        public TlsProtocolHandler(Stream stream, SecureRandom secureRandom)
            :   base(stream, stream, secureRandom)
        {
        }

        /// <remarks>Both streams can be the same object</remarks>
        public TlsProtocolHandler(Stream input, Stream output, SecureRandom	secureRandom)
            :   base(input, output, secureRandom)
        {
        }
    }
}
