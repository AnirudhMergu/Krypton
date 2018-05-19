using System;

namespace Krypton.Pdf
{
    /// <summary>
    /// Encryption Method
    /// </summary>
    public enum PdfEncryptionMethod
    {
        /// <summary>
        /// The document will be encrypted using a digital certificate. The document cannot be opened if the private key of the certificate is not available on the machine where de document will be opened.
        /// </summary>
        CertificateSecurity,
        /// <summary>
        /// The document will be encrypted using passwords.
        /// </summary>
        PasswordSecurity,
        /// <summary>
        /// No encryption will be added to the document.
        /// </summary>
        NoEncryption
    }
}