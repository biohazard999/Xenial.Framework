using System;

using Xenial.Licensing.Ext.Asn1;
using Xenial.Licensing.Ext.Utilities.Collections;

namespace Xenial.Licensing.Ext.X509
{
	public interface IX509Extension
	{
		/// <summary>
		/// Get all critical extension values, by oid
		/// </summary>
		/// <returns>IDictionary with string (OID) keys and Asn1OctetString values</returns>
		ISet GetCriticalExtensionOids();

		/// <summary>
		/// Get all non-critical extension values, by oid
		/// </summary>
		/// <returns>IDictionary with string (OID) keys and Asn1OctetString values</returns>
		ISet GetNonCriticalExtensionOids();

		[Obsolete("Use version taking a DerObjectIdentifier instead")]
		Asn1OctetString GetExtensionValue(string oid);

		Asn1OctetString GetExtensionValue(DerObjectIdentifier oid);
	}
}
