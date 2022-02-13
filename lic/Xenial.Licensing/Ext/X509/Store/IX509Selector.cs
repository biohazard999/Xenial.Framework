using System;

namespace Xenial.Licensing.Ext.X509.Store
{
	public interface IX509Selector
#if !(SILVERLIGHT || PORTABLE)
		: ICloneable
#endif
	{
#if SILVERLIGHT || PORTABLE
        object Clone();
#endif
        bool Match(object obj);
	}
}
