extern alias itext;

using itext.iTextSharp.text;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.X509;
using Krypton;
using Krypton.Timestamping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using itext::iTextSharp.text.pdf;
using itext::iTextSharp.text.pdf.security;

namespace Krypton.Pdf
{
    /// <summary>
    /// PDF document properties.
    /// </summary>
    public class PdfDocumentProperties
    {
        internal PdfReader PDFReader;

        /// <summary>
        /// Gets the digital signatures information from the PDF document.
        /// </summary>
        public List<PdfSignatureInfo> DigitalSignatures
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the file size in bytes.
        /// </summary>
        public long FileSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of pages from the loaded PDF file.
        /// </summary>
        public int NumberOfPages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the loaded PDF document password. The digital signature cannot be created if the document password is unknown.
        /// </summary>
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes PDFDocumentProperties class.
        /// </summary>
        public PdfDocumentProperties()
        {
            this.DigitalSignatures = new List<PdfSignatureInfo>();
        }

        /// <summary>
        /// Gets the page size of the loaded PDF document.
        /// </summary>
        /// <param name="pageNumber">The page from the loaded PDF document.</param>
        /// <returns>Coordinates of the top left point from the PDF document.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Throws an exception if the PDF document is not loaded or the document page is invalid.</exception>
        /// <exception cref="T:System.Exception">Throws an exception if the PDF document is not loaded or the document page is invalid.</exception>
        public Point DocumentPageSize(int pageNumber)
        {
            Point point;
            try
            {
                if (this.PDFReader == null)
                {
                    throw new ArgumentOutOfRangeException(CustomExceptions.DocumentIsNotLoaded);
                }
                if (pageNumber > this.NumberOfPages || pageNumber <= 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                itext.iTextSharp.text.Rectangle pageSizeWithRotation = this.PDFReader.GetPageSizeWithRotation(pageNumber);
                point = new Point((int)pageSizeWithRotation.Width, (int)pageSizeWithRotation.Height);
            }
            catch
            {
                throw;
            }
            return point;
        }

        internal void LoadPdfDocumentProperties(PdfReader reader)
        {
            this.PDFReader = reader;
            this.DigitalSignatures = new List<PdfSignatureInfo>();
            try
            {
                this.NumberOfPages = this.PDFReader.NumberOfPages;
            }
            catch
            {
                throw;
            }
            try
            {
                AcroFields acroFields = this.PDFReader.AcroFields;
                foreach (string signatureName in acroFields.GetSignatureNames())
                {
                    PdfSignatureInfo pdfSignatureInfo = new PdfSignatureInfo();
                    try
                    {
                        PdfPKCS7 pdfPKCS7 = acroFields.VerifySignature(signatureName);
                        pdfSignatureInfo.SignatureName = signatureName;
                        pdfSignatureInfo.HashAlgorithm = pdfPKCS7.GetHashAlgorithm();
                        pdfSignatureInfo.SigningLocation = pdfPKCS7.Location;
                        pdfSignatureInfo.SigningReason = pdfPKCS7.Reason;
                        pdfSignatureInfo.SignatureCertificate = new X509Certificate2(pdfPKCS7.SigningCertificate.GetEncoded());
                        if (pdfPKCS7.TimeStampDate != DateTime.MaxValue)
                        {
                            pdfSignatureInfo.SignatureTime = pdfPKCS7.TimeStampDate.ToLocalTime();
                            pdfSignatureInfo.TimestampInfo = new TimestampInfo(pdfPKCS7.TimeStampToken.GetEncoded());
                            pdfSignatureInfo.SignatureIsTimestamped = true;
                        }
                        else
                        {
                            pdfSignatureInfo.SignatureIsTimestamped = false;
                            pdfSignatureInfo.SignatureTime = pdfPKCS7.SignDate;
                        }
                        pdfSignatureInfo.SignatureIsValid = pdfPKCS7.Verify();
                        /*
                        try
                        {
                            pdfSignatureInfo.SignatureBytes = acroFields.GetSignatureBytes(signatureName);
                            foreach (SignerInformation signer in (new CmsSignedData(pdfSignatureInfo.SignatureBytes)).GetSignerInfos().GetSigners())
                            {
                                pdfSignatureInfo.SignatureHash = signer.GetSignature();
                            }
                        }
                        catch
                        {
                        }
                        */
                    }
                    catch
                    {
                    }
                    this.DigitalSignatures.Add(pdfSignatureInfo);
                }
            }
            catch
            {
                throw;
            }
            try
            {
                this.FileSize = (long)this.PDFReader.FileLength;
            }
            catch
            {
            }
        }
    }
}