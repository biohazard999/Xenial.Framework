using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

using Xenial.Licensing.Ext.Asn1;
using Xenial.Licensing.Ext.Asn1.CryptoPro;
using Xenial.Licensing.Ext.Asn1.Pkcs;
using Xenial.Licensing.Ext.Asn1.X509;
using Xenial.Licensing.Ext.Asn1.X9;
using Xenial.Licensing.Ext.Crypto;
using Xenial.Licensing.Ext.Crypto.Generators;
using Xenial.Licensing.Ext.Crypto.Parameters;
using Xenial.Licensing.Ext.Math;
using Xenial.Licensing.Ext.Pkcs;
using Xenial.Licensing.Ext.Security;
using Xenial.Licensing.Ext.Utilities.Encoders;
using Xenial.Licensing.Ext.Utilities.IO.Pem;
using Xenial.Licensing.Ext.X509;

namespace Xenial.Licensing.Ext.OpenSsl
{
	/// <remarks>General purpose writer for OpenSSL PEM objects.</remarks>
	public class PemWriter
		: Xenial.Licensing.Ext.Utilities.IO.Pem.PemWriter
	{
		/// <param name="writer">The TextWriter object to write the output to.</param>
		public PemWriter(
			TextWriter writer)
			: base(writer)
		{
		}

		public void WriteObject(
			object obj) 
		{
			try
			{
				base.WriteObject(new MiscPemGenerator(obj));
			}
			catch (PemGenerationException e)
			{
				if (e.InnerException is IOException)
					throw (IOException)e.InnerException;

				throw e;
			}
		}

		public void WriteObject(
			object			obj,
			string			algorithm,
			char[]			password,
			SecureRandom	random)
		{
			base.WriteObject(new MiscPemGenerator(obj, algorithm, password, random));
		}
	}
}
