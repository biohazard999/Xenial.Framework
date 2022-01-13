using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Standard.Licensing;
using Standard.Licensing.Validation;

#nullable enable

namespace __NAMESPACE__
{
    /// <summary>
    /// Class Xenial__XenialModule__LicenseCheck.
    /// Used to unlock license restrictions
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class XenialLicenseCheck
    {
        private class XenialLicenseFatalFailureException : Exception
        {
            public XenialLicenseFatalFailureException(string? hint = null) : base(FormatMessage(hint)) { }
            public XenialLicenseFatalFailureException(string? hint, Exception innerException) : base(FormatMessage(hint), innerException) { }

            private static string FormatMessage(string? hint) => $"You must call {nameof(XenialLicenseCheck)}.{nameof(XenialLicenseCheck.LoadLicense)} before doing any license checking. Hint: {hint}";
        }

        private static License? license;
        internal static License License
        {
            get
            {
                _ = license ?? throw new XenialLicenseFatalFailureException($"Don't use {nameof(License)} before calling {nameof(LoadLicense)}");
                return license;
            }
            set => license = value;
        }

        /// <summary>
        /// Unlock the license using a string
        /// </summary>
        /// <param name="licenseBase64"></param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void LoadLicense(string licenseBase64)
        {
            try
            {
                License = License.Load(Base64Decode(licenseBase64));
            }
            catch (Exception ex) { throw new XenialLicenseFatalFailureException("License is malformed", ex); }

            static string Base64Decode(string base64EncodedData)
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
        }

        internal static bool IsTrial => license?.Type != LicenseType.Standard;

        private static string ProductName => "%ProductName%";
        private static string PulicKeyToken => "%PublicKeyToken%";

        internal static bool IsSignatureValid =>
            !Validations.OfType<InvalidSignatureValidationFailure>().Any();

        internal static bool ContainsProduct =>
            !Validations.OfType<ProductMissingValidationFailure>().Any();

        internal static bool IsValid =>
            !IsTrial
            && IsSignatureValid
            && ContainsProduct;

        private class ProductMissingValidationFailure : GeneralValidationFailure
        {
            public ProductMissingValidationFailure(string productName)
            {
                Message = $"License does not include Product: '{productName}'";
                HowToResolve = $"Obtain a license that contains the Product: '{productName}'";
            }
        }

        private static IEnumerable<IValidationFailure> Validations =>
            License
            .Validate()
            .AssertThat(p => p.ProductFeatures.Contains(ProductName), new ProductMissingValidationFailure(ProductName))
            .And()
            .Signature(PulicKeyToken)
            .AssertValidLicense();
    }
}
