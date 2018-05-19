extern alias itext;

using itext.iTextSharp.text.pdf;
using Org.BouncyCastle.X509;
using Krypton;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Krypton.Pdf
{
    /// <summary>
    /// Class for encrypt PDF documents
    /// </summary>
    public class PdfEncrypt
    {
        private PdfReader reader;

        /// <summary>
        /// Gets or sets the document restrictions. The default value is AllowNone (e.g. AllowPrinting | AllowContentCopyingForAccessibility | AllowFillingOfFormFields).
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
        /// Gets or sets the document encryption method.
        /// </summary>
        public PdfEncryptionMethod EncryptionMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the loaded PDF document password. The encryption cannot be made if the document password is unknown.
        /// </summary>
        public string LoadedDocumentPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Set the encryption owner password.
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
        /// Initializes a new instance of PDFEncrypt class.
        /// </summary>
        public PdfEncrypt()
        {
            this.EncryptionMethod = PdfEncryptionMethod.NoEncryption;
            this.EncryptionAlgorithm = PdfEncryptionAlgorithm.EnhancedEncryption128BitAES;
            this.DocumentRestrictions = PdfDocumentRestrictions.AllowNone;
        }

        /// <summary>
        /// Adds encryption to a PDF file.
        /// </summary>
        /// <returns>Ecrypted PDF file as byte[].</returns>
        /// <exception cref="T:System.NullReferenceException">Throws an exception if the document is not loaded or the encryption certificate is not set.</exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException"></exception>
        public byte[] EncryptPDFFile()
        {
            byte[] array;
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                try
                {
                    if (this.reader == null)
                    {
                        throw new NullReferenceException(CustomExceptions.DocumentIsNotLoaded);
                    }
                    if (this.EncryptionMethod == PdfEncryptionMethod.NoEncryption)
                    {
                        throw new ArgumentException("Encryption type must be set");
                    }
                    PdfStamper pdfStamper = new PdfStamper(this.reader, memoryStream);
                    if (this.EncryptionMethod == PdfEncryptionMethod.PasswordSecurity)
                    {
                        pdfStamper.SetEncryption((int)this.EncryptionAlgorithm, this.UserPassword, this.OwnerPassword, (int)this.DocumentRestrictions);
                    }
                    if (this.EncryptionMethod == PdfEncryptionMethod.CertificateSecurity)
                    {
                        if (this.EncryptionCertificate == null)
                        {
                            throw new NullReferenceException(CustomExceptions.DigitalCertificateIsNotSet);
                        }
                        itext.Org.BouncyCastle.X509.X509CertificateParser x509CertificateParser = new itext.Org.BouncyCastle.X509.X509CertificateParser();
                        itext.Org.BouncyCastle.X509.X509Certificate[] x509CertificateArray = new itext.Org.BouncyCastle.X509.X509Certificate[] { x509CertificateParser.ReadCertificate(this.EncryptionCertificate.RawData) };
                        itext.Org.BouncyCastle.X509.X509Certificate[] x509CertificateArray1 = x509CertificateArray;
                        int[] documentRestrictions = new int[] { (int)this.DocumentRestrictions };
                        pdfStamper.SetEncryption(x509CertificateArray1, documentRestrictions, (int)this.EncryptionAlgorithm);
                    }
                    pdfStamper.Close();
                    array = memoryStream.ToArray();
                }
                catch
                {
                    throw;
                }
            }
            finally
            {
                memoryStream.Close();
            }
            return array;
        }

        /// <summary>
        /// Loads the PDF document from a byte array.
        /// </summary>
        /// <param name="pdfArray">Document byte array.</param>
        /// <exception cref="T:System.Exception">Throws an exception if an error occured.</exception>
        public void LoadPdfDocument(byte[] pdfArray)
        {
            try
            {
                if (this.LoadedDocumentPassword != null)
                {
                    ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
                    this.reader = new PdfReader(pdfArray, aSCIIEncoding.GetBytes(this.LoadedDocumentPassword));
                }
                else
                {
                    this.reader = new PdfReader(pdfArray);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Loads the PDF document from a file.
        /// </summary>
        /// <param name="PdfFile">Full path to the PDF document.</param>
        /// <exception cref="T:System.Exception">Throws an exception if an error occured.</exception>
        public void LoadPdfDocument(string PdfFile)
        {
            try
            {
                if (!File.Exists(PdfFile))
                {
                    throw new FileNotFoundException();
                }
                if (this.LoadedDocumentPassword != null)
                {
                    ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
                    this.reader = new PdfReader(PdfFile, aSCIIEncoding.GetBytes(this.LoadedDocumentPassword));
                }
                else
                {
                    this.reader = new PdfReader(PdfFile);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Loads the PDF document from an URL.
        /// </summary>
        /// <param name="PdfUrl">URL to the PDF file</param>
        /// <exception cref="T:System.Exception">Throws an exception if an error occured.</exception>
        public void LoadPdfDocument(Uri PdfUrl)
        {
            try
            {
                if (this.LoadedDocumentPassword != null)
                {
                    ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
                    this.reader = new PdfReader(PdfUrl, aSCIIEncoding.GetBytes(this.LoadedDocumentPassword));
                }
                else
                {
                    this.reader = new PdfReader(PdfUrl);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}