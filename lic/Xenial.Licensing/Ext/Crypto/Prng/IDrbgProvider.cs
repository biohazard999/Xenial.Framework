using System;

using Xenial.Licensing.Ext.Crypto.Prng.Drbg;

namespace Xenial.Licensing.Ext.Crypto.Prng
{
    internal interface IDrbgProvider
    {
        ISP80090Drbg Get(IEntropySource entropySource);
    }
}
