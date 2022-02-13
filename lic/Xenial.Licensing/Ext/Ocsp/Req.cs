using System;
using System.Collections;
using System.IO;

using Xenial.Licensing.Ext.Asn1;
using Xenial.Licensing.Ext.Asn1.Ocsp;
using Xenial.Licensing.Ext.Asn1.X509;
using Xenial.Licensing.Ext.X509;

namespace Xenial.Licensing.Ext.Ocsp
{
	public class Req
		: X509ExtensionBase
	{
		private Request req;

		public Req(
			Request req)
		{
			this.req = req;
		}

		public CertificateID GetCertID()
		{
			return new CertificateID(req.ReqCert);
		}

		public X509Extensions SingleRequestExtensions
		{
			get { return req.SingleRequestExtensions; }
		}

		protected override X509Extensions GetX509Extensions()
		{
			return SingleRequestExtensions;
		}
	}
}
