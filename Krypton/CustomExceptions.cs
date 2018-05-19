using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krypton
{
    /// <summary>
    /// Some exceptions messages trown by the library.
    /// </summary>
    internal class CustomExceptions
    {
        /// <summary>
        /// Thrown when the custom image is null (not loaded).
        /// </summary>
        public static string CustomImageIsNull
        {
            get
            {
                return "Custom image is null.";
            }
        }

        /// <summary>
        /// Thrown when the certificate is not loaded from Microsoft Store or file.
        /// </summary>
        public static string DigitalCertificateIsNotSet
        {
            get
            {
                return "Digital certificate is not set.";
            }
        }

        /// <summary>
        /// Thrown when the PDF document is not loaded.
        /// </summary>
        public static string DocumentIsNotLoaded
        {
            get
            {
                return "Document is not loaded.";
            }
        }

        /// <summary>
        /// Thrown when encryption must be set in order to encrypt th PDF file.
        /// </summary>
        public static string EncryptionTypeMustBeSet
        {
            get
            {
                return "Encryption type must be set.";
            }
        }

        /// <summary>
        /// Thrown when an invalid page number is specified (e.g. greater than the total number of document number of pages).
        /// </summary>
        public static string InvalidPageNumber
        {
            get
            {
                return "Invalid signature page number.";
            }
        }

        /// <summary>
        /// Thrown when the time stamping server is not available or the time stamping response is invalid (e.g. the server is not RFC 3161 compatible).
        /// </summary>
        public static string InvalidTimeStampingResponse
        {
            get
            {
                return "Invalid time stamping response. ";
            }
        }

        /// <summary>
        /// Thrown when the PDF file is encrypted and the opening password is invalid
        /// </summary>
        public static string PDFBadUserPassword
        {
            get
            {
                return "Bad user password";
            }
        }

        /// <summary>
        /// Thrown when an invalid PDF document is loaded
        /// </summary>
        public static string PDFHeaderSignatureNotFound
        {
            get
            {
                return "PDF header signature not found.";
            }
        }

        /// <summary>
        /// Thrown when the PDF file is encrypted and the opening password is not set
        /// </summary>
        public static string PDFReaderNotOpenedOwnerPassword
        {
            get
            {
                return "PdfReader not opened with owner password";
            }
        }

        /// <summary>
        /// Thrown when AppendSignature is true but must be set to false. 
        /// </summary>
        public static string SignatureAppendMode
        {
            get
            {
                return "Append mode does not support changing the encryption status.";
            }
        }

        /// <summary>
        /// Thrown when the XML file is already signed.
        /// </summary>
        public static string XMLNoMoreOneDigitalSignature
        {
            get
            {
                return "The XML file cannot contains more than one digital signature.";
            }
        }

        /// <summary>
        /// Thrown when more than one digital signature exists on the XML file.
        /// </summary>
        public static string XMLSignatureMoreThanOneSignature
        {
            get
            {
                return "Verification failed: More that one digital signature was found for the document.";
            }
        }

        /// <summary>
        /// Thrown when no digital signatures are on the XML file.
        /// </summary>
        public static string XMLSignatureNoDigitalSignatures
        {
            get
            {
                return "Verification failed: No digital signature was found in the document.";
            }
        }

        /// <summary>
        /// Thrown when an invalid algorithm is used for signing XML files.
        /// </summary>
        public static string XMLSigningAlgorithmNotSupported
        {
            get
            {
                return "Algorithm not supported (RSA or DSA only).";
            }
        }

        public CustomExceptions()
        {
        }
    }
}
