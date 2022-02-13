using System;

using Xenial.Licensing.Ext.Utilities.Date;

namespace Xenial.Licensing.Ext.Pkix
{
    public class CertStatus
    {
        public const int Unrevoked = 11;

        public const int Undetermined = 12;

        private int status = Unrevoked;

        DateTimeObject revocationDate = null;

        /// <summary>
        /// Returns the revocationDate.
        /// </summary>
         public DateTimeObject RevocationDate
        {
            get { return revocationDate; }
            set { this.revocationDate = value; }
        }

		/// <summary>
        /// Returns the certStatus.
        /// </summary>
        public int Status
        {
            get { return status; }
            set { this.status = value; }
        }
    }
}
