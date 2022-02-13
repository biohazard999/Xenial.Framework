
using Xenial.Licensing.Ext.Utilities;

using System;

namespace Xenial.Licensing.Ext.Cms
{
    internal class BaseDigestCalculator
        : IDigestCalculator
    {
        private readonly byte[] digest;

        internal BaseDigestCalculator(
            byte[] digest)
        {
            this.digest = digest;
        }

        public byte[] GetDigest()
        {
            return Arrays.Clone(digest);
        }
    }
}
