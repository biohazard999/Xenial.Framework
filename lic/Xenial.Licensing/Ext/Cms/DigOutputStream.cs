using System;

using Xenial.Licensing.Ext.Crypto;
using Xenial.Licensing.Ext.Utilities.IO;

namespace Xenial.Licensing.Ext.Cms
{
	internal class DigOutputStream
		: BaseOutputStream
	{
		private readonly IDigest dig;

		internal DigOutputStream(IDigest dig)
		{
			this.dig = dig;
		}

		public override void WriteByte(byte b)
		{
			dig.Update(b);
		}

		public override void Write(byte[] b, int off, int len)
		{
			dig.BlockUpdate(b, off, len);
		}
	}
}
