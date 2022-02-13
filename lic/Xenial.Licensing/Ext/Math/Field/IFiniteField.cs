using System;

namespace Xenial.Licensing.Ext.Math.Field
{
    public interface IFiniteField
    {
        BigInteger Characteristic { get; }

        int Dimension { get; }
    }
}
