using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Krypton.Pdf
{
    /// <summary>
    /// Encryption properties.
    /// </summary>
    public class PdfEncryptionSettings
    {
        /// <summary>
        /// Gets or sets the document restrictions. The default value is AllowNone (e.g. AllowPrinting | AllowContentCopyingForAccessibility | AllowFillingOfFormFields)
        /// </summary>
        public PdfDocumentRestrictions DocumentRestrictions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the encryption algorthm. The default value is EnhancedEncryption128BitAES.
        /// </summary>
        public PdfEncryptionAlgorithm EncryptionAlgorithm
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the encryption certificate.
        /// </summary>
        public X509Certificate2 EncryptionCertificate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document encryption method. The default value is NoEncryption.
        /// </summary>
        public PdfEncryptionMethod EncryptionMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the encryption owner password.
        /// </summary>
        public string OwnerPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Set the encryption user password (required to open the document).
        /// </summary>
        public string UserPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes PDFEncryptionProperties class.
        /// </summary>
        public PdfEncryptionSettings()
        {
            this.EncryptionMethod = PdfEncryptionMethod.NoEncryption;
            this.EncryptionAlgorithm = PdfEncryptionAlgorithm.EnhancedEncryption128BitAES;
            this.DocumentRestrictions = PdfDocumentRestrictions.AllowNone;
        }
    }
}