using System;
using System.IO;

using Xenial.Licensing.Ext.Asn1;
using Xenial.Licensing.Ext.Asn1.X509;
using Xenial.Licensing.Ext.Crypto;
using Xenial.Licensing.Ext.Crypto.Parameters;
using Xenial.Licensing.Ext.Security;
using Xenial.Licensing.Ext.Utilities.Date;
using Xenial.Licensing.Ext.Utilities.IO;

namespace Xenial.Licensing.Ext.Cms
{
	public class CmsAuthenticatedGenerator
		: CmsEnvelopedGenerator
	{
		/**
		* base constructor
		*/
		public CmsAuthenticatedGenerator()
		{
		}

		/**
		* constructor allowing specific source of randomness
		*
		* @param rand instance of SecureRandom to use
		*/
		public CmsAuthenticatedGenerator(
			SecureRandom rand)
			: base(rand)
		{
		}
	}
}
