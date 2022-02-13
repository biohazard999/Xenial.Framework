using System;

namespace Xenial.Licensing.Ext.OpenSsl
{
	public interface IPasswordFinder
	{
		char[] GetPassword();
	}
}
