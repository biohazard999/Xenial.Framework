using System;
using System.Collections;

using Xenial.Licensing.Ext.Bcpg.Attr;
using Xenial.Licensing.Ext.Utilities;

namespace Xenial.Licensing.Ext.Bcpg.OpenPgp
{
	public class PgpUserAttributeSubpacketVectorGenerator
	{
		private IList list = Platform.CreateArrayList();

		public virtual void SetImageAttribute(
			ImageAttrib.Format	imageType,
			byte[]				imageData)
		{
			if (imageData == null)
				throw new ArgumentException("attempt to set null image", "imageData");

			list.Add(new ImageAttrib(imageType, imageData));
		}

        public virtual PgpUserAttributeSubpacketVector Generate()
		{
            UserAttributeSubpacket[] a = new UserAttributeSubpacket[list.Count];
            for (int i = 0; i < list.Count; ++i)
            {
                a[i] = (UserAttributeSubpacket)list[i];
            }
            return new PgpUserAttributeSubpacketVector(a);
		}
	}
}
