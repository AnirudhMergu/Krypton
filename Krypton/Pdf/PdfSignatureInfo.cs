using Krypton.Timestamping;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Krypton.Pdf
{
    /// <summary>
    /// Class for obtaining the PDF digital signature properties.
    /// </summary>
    public class PdfSignatureInfo
    {
        /// <summary>
        /// Gets the digest algorithm used for the digital signature.
        /// </summary>
        public string HashAlgorithm
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the signature bytes as PKCS#7 signed message.
        /// </summary>
        public byte[] SignatureBytes
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the signing certificate used for the digital signature creation.
        /// </summary>
        public X509Certificate2 SignatureCertificate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the signature hash.
        /// </summary>
        public byte[] SignatureHash
        {
            get;
            internal set;
        }

        /// <summary>
        /// Check if the document is time stamped.
        /// </summary>
        public bool SignatureIsTimestamped
        {
            get;
            internal set;
        }

        /// <summary>
        /// Check if the signature is valid.
        /// </summary>
        public bool SignatureIsValid
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the signature name.
        /// </summary>
        public string SignatureName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the signing date of the signature.
        /// </summary>
        public DateTime SignatureTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the "Signing location" text.
        /// </summary>
        public string SigningLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the "Signing reason" text.
        /// </summary>
        public string SigningReason
        {
            get;
            set;
        }

        /// <summary>
        /// Get time timestamping information
        /// </summary>
        public TimestampInfo TimestampInfo
        {
            get;
            internal set;
        }

        /// <summary>
        /// Initializes PDFSignInformation class.
        /// </summary>
        public PdfSignatureInfo()
        {
        }
    }
}