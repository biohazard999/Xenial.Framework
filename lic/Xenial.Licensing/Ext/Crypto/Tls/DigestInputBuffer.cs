using System;
using System.IO;

using Xenial.Licensing.Ext.Crypto;
using Xenial.Licensing.Ext.Crypto.IO;
using Xenial.Licensing.Ext.Utilities.IO;

namespace Xenial.Licensing.Ext.Crypto.Tls
{
    internal class DigestInputBuffer
        :   MemoryStream
    {
        internal void UpdateDigest(IDigest d)
        {
            WriteTo(new DigStream(d));
        }

        private class DigStream
            :   BaseOutputStream
        {
            private readonly IDigest d;

            internal DigStream(IDigest d)
            {
                this.d = d;
            }

            public override void WriteByte(byte b)
            {
                d.Update(b);
            }

            public override void Write(byte[] buf, int off, int len)
            {
                d.BlockUpdate(buf, off, len);
            }
        }
    }
}
