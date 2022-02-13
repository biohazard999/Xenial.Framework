using System;
using System.IO;

using Xenial.Licensing.Ext.Crypto;
using Xenial.Licensing.Ext.Crypto.IO;
using Xenial.Licensing.Ext.Utilities.IO;

namespace Xenial.Licensing.Ext.Crypto.Tls
{
    internal class SignerInputBuffer
        : MemoryStream
    {
        internal void UpdateSigner(ISigner s)
        {
            WriteTo(new SigStream(s));
        }

        private class SigStream
            : BaseOutputStream
        {
            private readonly ISigner s;

            internal SigStream(ISigner s)
            {
                this.s = s;
            }

            public override void WriteByte(byte b)
            {
                s.Update(b);
            }

            public override void Write(byte[] buf, int off, int len)
            {
                s.BlockUpdate(buf, off, len);
            }
        }
    }
}
