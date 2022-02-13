using System;
using System.IO;

namespace Xenial.Licensing.Ext.Cms
{
	public interface CmsReadable
	{
		Stream GetInputStream();
	}
}
