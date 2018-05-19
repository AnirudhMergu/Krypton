extern alias itext;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using Krypton;
using Krypton.Certificates;
using Krypton.Timestamping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using itext::iTextSharp.text.pdf;

namespace Krypton.Pdf
{
    /// <summary>
    /// Class for digitally sign PDF documents
    /// </summary>
    public class PdfSignatureExt
    {
        private const string _ID_TIME_STAMP_TOKEN_ = "1.2.840.113549.1.9.16.2.14";

        private const string _PKCS7_DATA_ = "1.2.840.113549.1.7.1";

        private const int _SIGBLOCKSIZE_ = 20480;

        private const int _MAXCRLSIZE_ = 1048576;

        private List<byte[]> CRLRevocationInfo;

        private List<byte[]> OCSPRevocationInfo;

        private bool crlIsDownloaded;

        private int CRLSize;

        private PdfReader reader;

        private itext.iTextSharp.text.Rectangle signatureRectangle;

        private int textRunDirection;

        private int certifyDocument = 2147483647;

        /// <summary>
        /// Digital signature is appended to the document in order to add multiple signatures to the document. The default value is true.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool AppendSignature
        {
            get;
            set;
        }

        /// <summary>
        /// Certifies the signed document. Permitted actions after signing: No changes allowed, Form filling, Annotoations and form filling.
        /// </summary>
        public CertifyMethod CertifySignature
        {
            set
            {
                if (value == CertifyMethod.NoChangesAllowed)
                {
                    this.certifyDocument = 1;
                }
                if (value == CertifyMethod.AnnotationsAndFormFilling)
                {
                    this.certifyDocument = 3;
                }
                if (value == CertifyMethod.FormFilling)
                {
                    this.certifyDocument = 2;
                }
            }
        }

        /// <summary>
        /// Gets or sets the digital signature certificate. Use in conjunction with Certificates.DigitalCertificates class.
        /// </summary>
        public X509Certificate2 DigitalSignatureCertificate
        {
            get;
            set;
        }

        /// <summary>
        /// PDF document information (number of pages, number of digital signatures).
        /// </summary>
        public PdfDocumentProperties DocumentProperties
        {
            get;
            set;
        }

        /// <summary>
        /// Adds encryption and digital signature to the loaded document.
        /// </summary>
        public PdfEncryptionSettings Encryption
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the path to the custom signature font. For example: "C:\\WINDOWS\\FONTS\\times.TTF".
        /// </summary>
        public string FontFile
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the embedded font name. The default font is Helvetica.
        /// </summary>
        public FontName FontName
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the signature font zise.
        /// </summary>
        public int FontSize
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the hash algorithm used for digital signature creation. The default value is SHA1.
        /// </summary>
        public HashAlgorithm HashAlgorithm
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum CRL size. The default is 1 MB.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int MaxCrlSize
        {
            get;
            set;
        }

        /// <summary>
        /// Sets if the digital signature rectangle will appear using old style Adobe Acrobat format (question mark for unknown signatures or check for valid signatures). The dafault value is false.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool OldStyleAdobeSignature
        {
            get;
            set;
        }

        /// <summary>
        /// Embed the revocation information on the PDF signature for later verification (PAdES-LTV). By default the CRLs up to 1MB and the OCSP responses are included, if available.
        /// </summary>
        public PadesLtvLevel PadesLtvLevel
        {
            get;
            set;
        }

        /// <summary>
        /// PDF/A-1B ISO Name: ISO 19005-1. Digital signture font (and all other fonts) must be embedded in order to use this format. The default value is false.
        /// </summary>
        public bool SaveAsPdfA
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the signature rectangle position. This can be a System.Drawing.Rectangle.
        /// </summary>
        public itext.iTextSharp.text.Rectangle SignatureAdvancedPosition
        {
            set
            {
                itext.iTextSharp.text.Rectangle rectangle = value;
                this.signatureRectangle = new itext.iTextSharp.text.Rectangle(value);
                //this.signatureRectangle = new itext.iTextSharp.text.Rectangle((float)rectangle.X, (float)rectangle.Y, (float)(rectangle.X + rectangle.Width), (float)(rectangle.Y + rectangle.Height));
            }
        }

        /// <summary>
        /// Signs all pages from the PDF document. The default value is false.
        /// </summary>
        public bool SignatureAppearsOnAllPages
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the allocated space for the digital signature. The default value is 16384 bytes.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int SignatureByteBlockSize
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the signing date. If the time stamping option is used the value could be ignored by the PDF readers. The default value is DateTime.Now.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DateTime SignatureDate
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the signature image file. It can be loaded from a file using Sytem.IO.File.ReadAllBytes(string path).
        /// </summary>
        public byte[] SignatureImage
        {
            get;
            set;
        }

        /// <summary>
        /// Signature can be added to the signature rectangle in different ways. The dafault value is ImageAndText.
        /// </summary>
        public SignatureImageType SignatureImageType
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the signature page number where the signature rectangle will appear. The default value is 1.
        /// </summary>
        public int SignaturePage
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the signature rectangle position. This can be TopRight, TopMiddle, TopLeft, BottomRight, BottomMiddle, BottomLeft. The default value is TopRight.
        /// </summary>
        public SignaturePosition SignaturePosition
        {
            get;
            set;
        }

        /// <summary>
        /// Standard PAdES-Basic or PAdES-BES standard. The default is BasicSignature (PAdES-Basic) that is recognized by old versions of Adobe Reder.
        /// </summary>
        public SignatureStandard SignatureStandard
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the digital signature text that will appear on the signature rectangle. This will override default text.
        /// </summary>
        public string SignatureText
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the "Signed by:" property. This text will appear on the PDF digital signature properties on some PDF readers. The default value will be obtained from the signing certificate.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string SignedBy
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the "Signer's Contact Information" text. This text will appear on the PDF digital signature properties. The default value will be obtained from the signing certificate.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string SignerContactInformation
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the "Signing location" text.
        /// </summary>
        public string SigningLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the "Signing reason" text.
        /// </summary>
        public string SigningReason
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the signature text direction. The default value is Normal.
        /// </summary>
        public TextDirection TextDirection
        {
            set
            {
                if (value == TextDirection.Normal)
                {
                    this.textRunDirection = 0;
                }
                if (value == TextDirection.RightToLeft)
                {
                    this.textRunDirection = 3;
                }
            }
        }

        /// <summary>
        /// Time Stamping options.
        /// </summary>
        public TimestampSettings TimeStamping
        {
            get;
            set;
        }

        /// <summary>
        /// The signature rectangle will appear on the PDF document. The default value is true.
        /// </summary>
        public bool VisibleSignature
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of PDFSign class.
        /// </summary>
        /// <param name="librarySerialNumberLicense">The serial number provided to register the library.</param>
        public PdfSignatureExt(string librarySerialNumberLicense)
        {
            try
            {
                Licensing.CheckLicense(librarySerialNumberLicense);
            }
            catch (Exception exception)
            {
                throw new ArgumentException(string.Concat("Licensing error: ", exception.Message));
            }
            this.Encryption = new PdfEncryptionSettings();
            this.TimeStamping = new TimestampSettings();
            this.DocumentProperties = new PdfDocumentProperties();
            this.textRunDirection = 0;
            this.certifyDocument = 2147483647;
            this.SaveAsPdfA = false;
            this.SignaturePosition = SignaturePosition.TopRight;
            this.VisibleSignature = true;
            this.AppendSignature = true;
            this.OldStyleAdobeSignature = false;
            this.FontSize = -10;
            this.SignaturePage = 1;
            this.SignatureAppearsOnAllPages = false;
            this.SignatureImageType = SignatureImageType.ImageAndText;
            this.SignatureByteBlockSize = 20480;
            this.SignatureDate = DateTime.MinValue;
            this.HashAlgorithm = HashAlgorithm.SHA1;
            this.FontName = FontName.Helvetica;
            this.CRLRevocationInfo = new List<byte[]>();
            this.OCSPRevocationInfo = new List<byte[]>();
            this.PadesLtvLevel = PadesLtvLevel.IncludeCrlAndOcsp;
            this.crlIsDownloaded = false;
            this.CRLSize = 0;
            this.MaxCrlSize = 1048576;
            this.SignatureStandard = SignatureStandard.Pkcs7;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] AddSignatureField(string fieldName, int page, System.Drawing.Rectangle position)
        {
            MemoryStream memoryStream = new MemoryStream();
            PdfStamper pdfStamper = new PdfStamper(this.reader, memoryStream);
            PdfFormField pdfFormField = PdfFormField.CreateSignature(pdfStamper.Writer);
            pdfFormField.SetWidget(new itext.iTextSharp.text.Rectangle((float)position.X, (float)position.Y, (float)(position.X + position.Width), (float)(position.Y + position.Height)), null);
            pdfFormField.SetFieldFlags(4);
            pdfFormField.Put(PdfName.DA, new PdfString("/Helv 0 Tf 0 g"));
            pdfFormField.FieldName = fieldName;
            pdfFormField.Page = page;
            pdfStamper.AddAnnotation(pdfFormField, 1);
            pdfStamper.Close();
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Digitally sign the loaded PDF file using a digital certificate.
        /// </summary>
        /// <returns>The digitally signed PDF file as byte[].</returns>
        public byte[] ApplyDigitalSignature()
        {
            return this.ApplyDigitalSignature(null);
        }

        /// <summary>
        /// Digitally sign the loaded PDF file using a digital certificate.
        /// </summary>
        /// <param name="signatureField">Digitally sign an exisiting signature field.</param>
        /// <returns>The digitally signed PDF file as byte[].</returns>
        public byte[] ApplyDigitalSignature(string signatureField)
        {
            BaseFont baseFont;
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
                    if (this.DigitalSignatureCertificate == null)
                    {
                        throw new NullReferenceException(CustomExceptions.DigitalCertificateIsNotSet);
                    }
                    if (this.Encryption.EncryptionMethod != PdfEncryptionMethod.NoEncryption)
                    {
                        this.AppendSignature = false;
                    }
                    PdfStamper pdfStamper = PdfStamper.CreateSignature(this.reader, memoryStream, '\0', null, this.AppendSignature);
                    if (this.SaveAsPdfA)
                    {
                        pdfStamper.Writer.PDFXConformance = 4;
                        pdfStamper.Writer.CreateXmpMetadata();
                    }
                    if (this.Encryption.EncryptionMethod == PdfEncryptionMethod.PasswordSecurity)
                    {
                        pdfStamper.SetEncryption((int)this.Encryption.EncryptionAlgorithm, this.Encryption.UserPassword, this.Encryption.OwnerPassword, (int)this.Encryption.DocumentRestrictions);
                    }
                    if (this.Encryption.EncryptionMethod == PdfEncryptionMethod.CertificateSecurity)
                    {
                        if (this.Encryption.EncryptionCertificate == null)
                        {
                            throw new NullReferenceException(CustomExceptions.DigitalCertificateIsNotSet);
                        }
                        itext.Org.BouncyCastle.X509.X509CertificateParser x509CertificateParser = new itext.Org.BouncyCastle.X509.X509CertificateParser();
                        itext.Org.BouncyCastle.X509.X509Certificate[] x509CertificateArray = new itext.Org.BouncyCastle.X509.X509Certificate[] { x509CertificateParser.ReadCertificate(this.Encryption.EncryptionCertificate.RawData) };
                        itext.Org.BouncyCastle.X509.X509Certificate[] x509CertificateArray1 = x509CertificateArray;
                        int[] documentRestrictions = new int[] { (int)this.Encryption.DocumentRestrictions };
                        pdfStamper.SetEncryption(x509CertificateArray1, documentRestrictions, (int)this.Encryption.EncryptionAlgorithm);
                    }
                    PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
                    try
                    {
                        if (string.IsNullOrEmpty(this.FontFile))
                        {
                            baseFont = BaseFont.CreateFont(this.FontName.ToString().Replace("_", "-"), "Cp1250", true);
                        }
                        else
                        {
                            if (!File.Exists(this.FontFile))
                            {
                                throw new FileNotFoundException();
                            }
                            baseFont = BaseFont.CreateFont(this.FontFile, "Identity-H", true);
                        }
                        signatureAppearance.Layer2Font = new itext.iTextSharp.text.Font(baseFont, (float)this.FontSize);
                        signatureAppearance.RunDirection = this.textRunDirection;
                    }
                    catch
                    {
                        throw;
                    }
                    if (signatureField != null)
                    {
                        signatureAppearance.SetVisibleSignature(signatureField);
                    }
                    else if (this.VisibleSignature)
                    {
                        if (this.SignatureAppearsOnAllPages)
                        {
                            //signatureAppearance.SignAllPages = true;
                        }
                        if (this.SignaturePage == 2147483647)
                        {
                            this.SignaturePage = this.reader.NumberOfPages;
                        }
                        if (this.SignaturePage < 1)
                        {
                            throw new ArgumentOutOfRangeException(CustomExceptions.InvalidPageNumber);
                        }
                        if (this.SignaturePage != 2147483647 && this.SignaturePage > this.reader.NumberOfPages)
                        {
                            throw new ArgumentOutOfRangeException(CustomExceptions.InvalidPageNumber);
                        }
                        itext.iTextSharp.text.Rectangle pageSizeWithRotation = this.reader.GetPageSizeWithRotation(this.SignaturePage);
                        if (this.signatureRectangle == null)
                        {
                            int num = 50;
                            int num1 = 50;
                            int num2 = 150;
                            int num3 = 100;
                            if (this.SignaturePosition == SignaturePosition.TopRight)
                            {
                                signatureAppearance.SetVisibleSignature(new itext.iTextSharp.text.Rectangle(pageSizeWithRotation.Width - (float)num2, pageSizeWithRotation.Height - (float)num1, pageSizeWithRotation.Width - (float)num, pageSizeWithRotation.Height - (float)num3), this.SignaturePage, null);
                            }
                            if (this.SignaturePosition == SignaturePosition.TopMiddle)
                            {
                                signatureAppearance.SetVisibleSignature(new itext.iTextSharp.text.Rectangle((pageSizeWithRotation.Width - (float)num) / 2f, pageSizeWithRotation.Height - (float)num1, (pageSizeWithRotation.Width - (float)num2) / 2f + (float)num2, pageSizeWithRotation.Height - (float)num3), this.SignaturePage, null);
                            }
                            if (this.SignaturePosition == SignaturePosition.TopLeft)
                            {
                                signatureAppearance.SetVisibleSignature(new itext.iTextSharp.text.Rectangle((float)num, pageSizeWithRotation.Height - (float)num1, (float)num2, pageSizeWithRotation.Height - (float)num3), this.SignaturePage, null);
                            }
                            if (this.SignaturePosition == SignaturePosition.BottomRight)
                            {
                                signatureAppearance.SetVisibleSignature(new itext.iTextSharp.text.Rectangle(pageSizeWithRotation.Width - (float)num2, (float)num1, pageSizeWithRotation.Width - (float)num, (float)num3), this.SignaturePage, null);
                            }
                            if (this.SignaturePosition == SignaturePosition.BottomMiddle)
                            {
                                signatureAppearance.SetVisibleSignature(new itext.iTextSharp.text.Rectangle((pageSizeWithRotation.Width - (float)num) / 2f, (float)num1, (pageSizeWithRotation.Width - (float)num2) / 2f + (float)num2, (float)num3), this.SignaturePage, null);
                            }
                            if (this.SignaturePosition == SignaturePosition.BottomLeft)
                            {
                                signatureAppearance.SetVisibleSignature(new itext.iTextSharp.text.Rectangle((float)num, (float)num1, (float)num2, (float)num3), this.SignaturePage, null);
                            }
                        }
                        if (this.signatureRectangle != null)
                        {
                            signatureAppearance.SetVisibleSignature(this.signatureRectangle, this.SignaturePage, null);
                        }
                    }
                    if (this.SignatureDate != DateTime.MinValue)
                    {
                        signatureAppearance.SignDate = this.SignatureDate;
                    }
                    else
                    {
                        signatureAppearance.SignDate = DateTime.Now;
                    }
                    try
                    {
                        if (this.SignedBy != null)
                        {
                            signatureAppearance.Contact = this.SignedBy;
                        }
                        else
                        {
                            signatureAppearance.Contact = this.DigitalSignatureCertificate.GetNameInfo(X509NameType.EmailName, false);
                        }
                    }
                    catch
                    {
                    }
                    string nameInfo = this.DigitalSignatureCertificate.GetNameInfo(X509NameType.SimpleName, false);
                    DateTime signDate = signatureAppearance.SignDate;
                    string signatureText = string.Concat(nameInfo, "\n", signDate.ToString("yyyy.MM.dd HH:mm"));
                    if (this.SigningReason != null)
                    {
                        signatureAppearance.Reason = this.SigningReason;
                        signatureText = string.Concat(signatureText, "\n", this.SigningReason);
                    }
                    if (this.SigningLocation != null)
                    {
                        signatureAppearance.Location = this.SigningLocation;
                        signatureText = string.Concat(signatureText, "\n", this.SigningLocation);
                    }
                    if (this.SignatureText != null)
                    {
                        signatureText = this.SignatureText;
                    }
                    signatureAppearance.Layer2Text = signatureText;
                    if (this.SigningReason != null)
                    {
                        signatureAppearance.Reason = this.SigningReason;
                    }
                    if (this.SigningLocation != null)
                    {
                        signatureAppearance.Location = this.SigningLocation;
                    }
                    signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.DESCRIPTION;//SignatureRender.Description;
                    if (this.SignatureImage != null)
                    {
                        itext.iTextSharp.text.Image instance = itext.iTextSharp.text.Image.GetInstance(this.SignatureImage);
                        if (this.SignatureImageType == SignatureImageType.ImageAsBackground)
                        {
                            signatureAppearance.Image = instance;
                        }
                        if (this.SignatureImageType == SignatureImageType.ImageWithNoText)
                        {
                            signatureAppearance.Image = instance;
                            signatureAppearance.Layer2Text = "";
                        }
                        if (this.SignatureImageType == SignatureImageType.ImageAndText)
                        {
                            signatureAppearance.SignatureGraphic = instance;
                            signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION;
                        }
                    }
                    if (this.OldStyleAdobeSignature)
                    {
                        signatureAppearance.Acro6Layers = false;
                    }
                    else
                    {
                        signatureAppearance.Acro6Layers = true;
                    }
                    PdfName aDBEPKCS7DETACHED = PdfName.ADBE_PKCS7_DETACHED;
                    if (this.SignatureStandard == SignatureStandard.Cades)
                    {
                        aDBEPKCS7DETACHED = new PdfName("ETSI.CAdES.detached");
                    }
                    itext.iTextSharp.text.pdf.PdfSignature pdfSignatureExt = new itext.iTextSharp.text.pdf.PdfSignature(PdfName.ADOBE_PPKLITE, aDBEPKCS7DETACHED)
                    {
                        Date = new PdfDate(signatureAppearance.SignDate)
                    };
                    try
                    {
                        if (this.SignedBy != null)
                        {
                            pdfSignatureExt.Name = this.SignedBy;
                        }
                        else
                        {
                            pdfSignatureExt.Name = this.DigitalSignatureCertificate.GetNameInfo(X509NameType.SimpleName, false);
                        }
                    }
                    catch
                    {
                    }
                    if (this.SigningReason != null)
                    {
                        pdfSignatureExt.Reason = this.SigningReason;
                    }
                    if (this.SigningLocation != null)
                    {
                        pdfSignatureExt.Location = this.SigningLocation;
                    }
                    try
                    {
                        if (this.SignerContactInformation != null)
                        {
                            pdfSignatureExt.Contact = this.SignerContactInformation;
                        }
                        else
                        {
                            pdfSignatureExt.Contact = this.DigitalSignatureCertificate.GetNameInfo(X509NameType.EmailName, false);
                        }
                    }
                    catch
                    {
                    }
                    if (this.certifyDocument != 2147483647)
                    {
                        signatureAppearance.CertificationLevel = this.certifyDocument;
                    }
                    signatureAppearance.CryptoDictionary = pdfSignatureExt;
                    int signatureByteBlockSize = this.SignatureByteBlockSize;
                    if (this.PadesLtvLevel == PadesLtvLevel.IncludeCrl || this.PadesLtvLevel == PadesLtvLevel.IncludeCrlAndOcsp)
                    {
                        this.GetChainCRL(this.DigitalSignatureCertificate);
                        signatureByteBlockSize += this.CRLSize;
                    }
                    Hashtable hashtables = new Hashtable();
                    Dictionary<PdfName, int> dict = new Dictionary<PdfName, int>();
                    dict[PdfName.CONTENTS] = signatureByteBlockSize * 2 + 2;
                    signatureAppearance.PreClose(dict);//, this.reader.NumberOfPages);
                    Stream rangeStream = signatureAppearance.GetRangeStream();
                    MemoryStream memoryStream1 = new MemoryStream();
                    int num4 = 0;
                    byte[] numArray = new byte[8192];
                    while (true)
                    {
                        int num5 = rangeStream.Read(numArray, 0, 8192);
                        num4 = num5;
                        if (num5 <= 0)
                        {
                            break;
                        }
                        memoryStream1.Write(numArray, 0, num4);
                    }
                    byte[] numArray1 = null;
                    numArray1 = (this.TimeStamping.ServerUrl == null ? this.SignMsg(memoryStream1.ToArray(), this.DigitalSignatureCertificate) : this.SignMsgTSA(memoryStream1.ToArray(), this.DigitalSignatureCertificate));
                    byte[] numArray2 = new byte[signatureByteBlockSize];
                    PdfDictionary pdfDictionary = new PdfDictionary();
                    Array.Copy(numArray1, 0, numArray2, 0, (int)numArray1.Length);
                    pdfDictionary.Put(PdfName.CONTENTS, (new PdfString(numArray2)).SetHexWriting(true));
                    signatureAppearance.Close(pdfDictionary);
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
        /// Add a timestamp signature to a PDF document.
        /// </summary>
        public byte[] ApplyTimestampSignature()
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
                    if (this.TimeStamping.ServerUrl == null)
                    {
                        throw new NullReferenceException("Time Stamp Server URL must be set.");
                    }
                    Licensing.ShowDemoMessagePdf();
                    PdfStamper pdfStamper = PdfStamper.CreateSignature(this.reader, memoryStream, '\0', null, true);
                    this.TimestampForLtv(pdfStamper.SignatureAppearance);
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

        private byte[] ComputeHash(byte[] msg, HashAlgorithm hashAlgorithm)
        {
            byte[] numArray;
            try
            {
                System.Security.Cryptography.HashAlgorithm sHA1CryptoServiceProvider = null;
                if (hashAlgorithm == HashAlgorithm.SHA1)
                {
                    sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
                }
                if (hashAlgorithm == HashAlgorithm.SHA256)
                {
                    sHA1CryptoServiceProvider = new SHA256Managed();
                }
                if (hashAlgorithm == HashAlgorithm.SHA384)
                {
                    sHA1CryptoServiceProvider = new SHA384Managed();
                }
                if (hashAlgorithm == HashAlgorithm.SHA512)
                {
                    sHA1CryptoServiceProvider = new SHA512Managed();
                }
                numArray = sHA1CryptoServiceProvider.ComputeHash(msg);
            }
            catch
            {
                throw;
            }
            return numArray;
        }

        private byte[] DownloadBytesFromURL(Uri url)
        {
            byte[] numArray;
            try
            {
                WebRequest webRequest = WebRequest.Create(url);
                webRequest.Timeout = 30000;
                Stream responseStream = webRequest.GetResponse().GetResponseStream();
                byte[] byteArray = this.ToByteArray(responseStream);
                responseStream.Close();
                numArray = byteArray;
            }
            catch
            {
                numArray = null;
            }
            return numArray;
        }

        private byte[] DownloadDataFromHttp(Uri url)
        {
            byte[] array;
            try
            {
                WebRequest webRequest = WebRequest.Create(url);
                Stream responseStream = webRequest.GetResponse().GetResponseStream();
                webRequest.Timeout = 30000;
                byte[] numArray = new byte[1024];
                MemoryStream memoryStream = new MemoryStream();
                while (true)
                {
                    int num = responseStream.Read(numArray, 0, (int)numArray.Length);
                    if (num == 0)
                    {
                        break;
                    }
                    memoryStream.Write(numArray, 0, num);
                }
                responseStream.Close();
                memoryStream.Close();
                array = memoryStream.ToArray();
            }
            catch
            {
                array = null;
            }
            return array;
        }

        private static string GetCertCRL(X509Certificate2 cert, bool isLdapUrl)
        {
            string str;
            try
            {
                Asn1Object asn1Object = Asn1Object.FromByteArray(cert.RawData);
                X509Extensions extensions = X509CertificateStructure.GetInstance(asn1Object).TbsCertificate.Extensions;
                if (extensions != null)
                {
                    foreach (DerObjectIdentifier extensionOid in extensions.ExtensionOids)
                    {
                        Org.BouncyCastle.Asn1.X509.X509Extension extension = extensions.GetExtension(extensionOid);
                        Asn1Object asn1Object1 = Asn1Object.FromByteArray(extension.Value.GetOctets());
                        if (!extensionOid.Equals(X509Extensions.CrlDistributionPoints))
                        {
                            continue;
                        }
                        DistributionPoint[] distributionPoints = CrlDistPoint.GetInstance(asn1Object1).GetDistributionPoints();
                        for (int i = 0; i < (int)distributionPoints.Length; i++)
                        {
                            DistributionPointName distributionPointName = distributionPoints[i].DistributionPointName;
                            if (distributionPointName != null && distributionPointName.PointType == 0)
                            {
                                GeneralName[] names = GeneralNames.GetInstance(distributionPointName.Name).GetNames();
                                for (int j = 0; j < (int)names.Length; j++)
                                {
                                    if (names[j].TagNo == 6)
                                    {
                                        string str1 = DerIA5String.GetInstance(names[j].Name).GetString();
                                        if (str1.ToLower().StartsWith("http") && !isLdapUrl)
                                        {
                                            str = str1;
                                            return str;
                                        }
                                        else if (str1.ToLower().StartsWith("ldap") && isLdapUrl)
                                        {
                                            str = str1;
                                            return str;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    str = null;
                }
                else
                {
                    str = null;
                }
            }
            catch
            {
                str = null;
            }
            return str;
        }

        private X509Chain GetChain(X509Certificate2 signingCertificate)
        {
            X509Chain x509Chain;
            try
            {
                X509Chain x509Chain1 = new X509Chain();
                x509Chain1.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                x509Chain1.Build(signingCertificate);
                x509Chain = x509Chain1;
            }
            catch
            {
                x509Chain = null;
            }
            return x509Chain;
        }

        private void GetChainCRL(X509Certificate2 signerCert)
        {
            try
            {
                X509CertificateParser x509CertificateParser = new X509CertificateParser();
                X509Chain chain = this.GetChain(signerCert);
                if (this.PadesLtvLevel == PadesLtvLevel.IncludeCrlAndOcsp)
                {
                    this.OCSPRevocationInfo = new List<byte[]>();
                    try
                    {
                        byte[] oCSPResponse = DigitalCertificate.GetOCSPResponse(x509CertificateParser.ReadCertificate(chain.ChainElements[0].Certificate.RawData), x509CertificateParser.ReadCertificate(chain.ChainElements[1].Certificate.RawData));
                        if (oCSPResponse != null)
                        {
                            this.OCSPRevocationInfo.Add(oCSPResponse);
                        }
                    }
                    catch
                    {
                    }
                    try
                    {
                        byte[] numArray = DigitalCertificate.GetOCSPResponse(x509CertificateParser.ReadCertificate(chain.ChainElements[1].Certificate.RawData), x509CertificateParser.ReadCertificate(chain.ChainElements[2].Certificate.RawData));
                        if (numArray != null)
                        {
                            this.OCSPRevocationInfo.Add(numArray);
                        }
                    }
                    catch
                    {
                    }
                }
                if (!this.crlIsDownloaded)
                {
                    if (chain == null)
                    {
                        string certCRL = PdfSignatureExt.GetCertCRL(signerCert, false);
                        if (!string.IsNullOrEmpty(certCRL))
                        {
                            byte[] numArray1 = this.DownloadDataFromHttp(new Uri(certCRL)) ?? this.DownloadBytesFromURL(new Uri(certCRL));
                            if (numArray1 != null && (int)numArray1.Length < this.MaxCrlSize)
                            {
                                this.CRLRevocationInfo.Add(numArray1);
                            }
                        }
                    }
                    else
                    {
                        X509ChainElementEnumerator enumerator = chain.ChainElements.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            string str = PdfSignatureExt.GetCertCRL(enumerator.Current.Certificate, false);
                            if (string.IsNullOrEmpty(str))
                            {
                                continue;
                            }
                            byte[] numArray2 = this.DownloadDataFromHttp(new Uri(str)) ?? this.DownloadBytesFromURL(new Uri(str));
                            if (numArray2 == null || (int)numArray2.Length >= this.MaxCrlSize)
                            {
                                continue;
                            }
                            this.CRLRevocationInfo.Add(numArray2);
                        }
                    }
                    int length = 0;
                    foreach (byte[] cRLRevocationInfo in this.CRLRevocationInfo)
                    {
                        length = length + (int)cRLRevocationInfo.Length + 20;
                    }
                    foreach (byte[] oCSPRevocationInfo in this.OCSPRevocationInfo)
                    {
                        length = length + (int)oCSPRevocationInfo.Length + 20;
                    }
                    this.CRLSize = length;
                    this.crlIsDownloaded = true;
                }
            }
            catch
            {
            }
        }

        private byte[] GetTimestampToken(byte[] imprint)
        {
            byte[] encoded;
            try
            {
                string value = (new Oid(this.TimeStamping.HashAlgorithm.ToString())).Value;
                TimeStampRequestGenerator timeStampRequestGenerator = new TimeStampRequestGenerator();
                timeStampRequestGenerator.SetCertReq(true);
                if (this.TimeStamping.PolicyOid != null)
                {
                    timeStampRequestGenerator.SetReqPolicy(this.TimeStamping.PolicyOid.Value.ToString());
                }
                TimeStampRequest timeStampRequest = null;
                if (!this.TimeStamping.UseNonce)
                {
                    timeStampRequest = timeStampRequestGenerator.Generate(value, imprint);
                }
                else
                {
                    long tickCount = (long)Environment.TickCount;
                    DateTime now = DateTime.Now;
                    BigInteger bigInteger = BigInteger.ValueOf(tickCount + now.Ticks);
                    timeStampRequest = timeStampRequestGenerator.Generate(value, imprint, bigInteger);
                }
                TimeStampResponse timeStampResponse = new TimeStampResponse(this.GetTSAResponse(timeStampRequest.GetEncoded()));
                timeStampResponse.Validate(timeStampRequest);
                if ((timeStampResponse.GetFailInfo() == null ? 0 : 1) != 0)
                {
                    string[] invalidTimeStampingResponse = new string[] { CustomExceptions.InvalidTimeStampingResponse, "Status: ", null, null, null };
                    invalidTimeStampingResponse[2] = timeStampResponse.Status.ToString();
                    invalidTimeStampingResponse[3] = "; Status information : ";
                    invalidTimeStampingResponse[4] = timeStampResponse.GetStatusString();
                    throw new WebException(string.Concat(invalidTimeStampingResponse));
                }
                TimeStampToken timeStampToken = timeStampResponse.TimeStampToken;
                if (timeStampToken == null)
                {
                    throw new WebException(CustomExceptions.InvalidTimeStampingResponse);
                }
                encoded = timeStampToken.GetEncoded();
            }
            catch
            {
                throw;
            }
            return encoded;
        }

        private byte[] GetTSAResponse(byte[] requestBytes)
        {
            byte[] array;
            byte[] numArray;
            try
            {
                HttpWebRequest serverTimeout = (HttpWebRequest)WebRequest.Create(this.TimeStamping.ServerUrl);
                serverTimeout.Timeout = this.TimeStamping.ServerTimeout;
                serverTimeout.ContentType = "application/timestamp-query";
                serverTimeout.Method = "POST";
                if (!string.IsNullOrEmpty(this.TimeStamping.UserName))
                {
                    serverTimeout.Headers.Add("Authorization", string.Concat("Basic ", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Concat(this.TimeStamping.UserName, ":", this.TimeStamping.Password)))));
                }
                if (this.TimeStamping.AuthenticationCertificate != null)
                {
                    serverTimeout.ClientCertificates.Add(this.TimeStamping.AuthenticationCertificate);
                }
                serverTimeout.ContentLength = (long)((int)requestBytes.Length);
                Stream requestStream = serverTimeout.GetRequestStream();
                requestStream.Write(requestBytes, 0, (int)requestBytes.Length);
                requestStream.Close();
                WebResponse response = serverTimeout.GetResponse();
                Stream responseStream = response.GetResponseStream();
                MemoryStream memoryStream = new MemoryStream();
                byte[] numArray1 = new byte[4096];
                using (MemoryStream memoryStream1 = new MemoryStream())
                {
                    int num = 0;
                    do
                    {
                        num = responseStream.Read(numArray1, 0, (int)numArray1.Length);
                        memoryStream1.Write(numArray1, 0, num);
                    }
                    while (num != 0);
                    array = memoryStream1.ToArray();
                }
                response.Close();
                numArray = array;
            }
            catch
            {
                throw;
            }
            return numArray;
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
                if (this.DocumentProperties.Password != null)
                {
                    ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
                    this.reader = new PdfReader(pdfArray, aSCIIEncoding.GetBytes(this.DocumentProperties.Password));
                }
                else
                {
                    this.reader = new PdfReader(pdfArray);
                }
                this.DocumentProperties.LoadPdfDocumentProperties(this.reader);
                if (Licensing.demoVersion && this.DocumentProperties.DigitalSignatures.Count == 0)
                {
                    if (this.DocumentProperties.Password != null)
                    {
                        PdfInsertObject pdfInsertObject = new PdfInsertObject();
                        pdfInsertObject.DocumentProperties.Password = this.DocumentProperties.Password;
                        pdfInsertObject.LoadPdfDocument(pdfArray);
                        pdfInsertObject.AddText(Licensing.GetDemoMessageText());
                        this.reader = new PdfReader(pdfInsertObject.InsertObjects());
                        ASCIIEncoding aSCIIEncoding1 = new ASCIIEncoding();
                        this.reader = new PdfReader(pdfInsertObject.InsertObjects(), aSCIIEncoding1.GetBytes(this.DocumentProperties.Password));
                    }
                    else
                    {
                        PdfInsertObject pdfInsertObject1 = new PdfInsertObject();
                        pdfInsertObject1.LoadPdfDocument(pdfArray);
                        pdfInsertObject1.AddText(Licensing.GetDemoMessageText());
                        this.reader = new PdfReader(pdfInsertObject1.InsertObjects());
                    }
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
                this.LoadPdfDocument(File.ReadAllBytes(PdfFile));
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Loads the PDF document from an URL.
        /// </summary>
        /// <param name="PdfUrl">URL to the PDF file.</param>
        /// <exception cref="T:System.Exception">Throws an exception if an error occured.</exception>
        public void LoadPdfDocument(Uri PdfUrl)
        {
            try
            {
                this.LoadPdfDocument(this.DownloadDataFromHttp(PdfUrl));
            }
            catch
            {
                throw;
            }
        }

        private byte[] SignMsg(byte[] msg, X509Certificate2 signerCert)
        {
            byte[] encoded = null;
            try
            {
                Licensing.ShowDemoMessagePdf();
                DigitalCertificate.LogOnEToken(this.DigitalSignatureCertificate);
                X509CertificateParser x509CertificateParser = new X509CertificateParser();
                X509Chain chain = this.GetChain(signerCert);
                IList arrayLists = new ArrayList();
                if (chain == null)
                {
                    arrayLists.Add(x509CertificateParser.ReadCertificate(signerCert.RawData));
                }
                else
                {
                    X509ChainElementEnumerator enumerator = chain.ChainElements.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        X509ChainElement current = enumerator.Current;
                        arrayLists.Add(x509CertificateParser.ReadCertificate(current.Certificate.RawData));
                    }
                }
                IX509Store x509Store = X509StoreFactory.Create("Certificate/Collection", new X509CollectionStoreParameters(arrayLists));
                CmsSignedDataGenerator cmsSignedDataGenerator = new CmsSignedDataGenerator();
                cmsSignedDataGenerator.AddCertificates(x509Store);
                //cmsSignedDataGenerator.AddSigner(signerCert, x509CertificateParser.ReadCertificate(signerCert.RawData), (new Oid(this.HashAlgorithm.ToString())).Value, null, null);
                bool flag = false;
                if (this.SignatureStandard == SignatureStandard.Cades)
                {
                    flag = true;
                    //DefaultSignedAttributeTableGenerator.isPadesSignature = true;
                }
                //encoded = cmsSignedDataGenerator.Generate(signerCert, flag, "1.2.840.113549.1.7.1", new CmsProcessableByteArray(msg), false, this.CRLRevocationInfo, this.OCSPRevocationInfo).GetEncoded();
            }
            catch
            {
                throw;
            }
            return encoded;
        }

        private byte[] SignMsgTSA(byte[] msg, X509Certificate2 signerCert)
        {
            byte[] encoded;
            try
            {
                CmsSignedData cmsSignedDatum = new CmsSignedData(this.SignMsg(msg, signerCert));
                SignerInformationStore signerInfos = cmsSignedDatum.GetSignerInfos();
                byte[] signature = null;
                SignerInformation current = null;
                IEnumerator enumerator = signerInfos.GetSigners().GetEnumerator();
                try
                {
                    if (enumerator.MoveNext())
                    {
                        current = (SignerInformation)enumerator.Current;
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                signature = current.GetSignature();
                byte[] timestampToken = this.GetTimestampToken(this.ComputeHash(signature, this.TimeStamping.HashAlgorithm));
                Asn1InputStream asn1InputStream = new Asn1InputStream(new MemoryStream(timestampToken));
                Asn1EncodableVector asn1EncodableVectors = new Asn1EncodableVector(new Asn1Encodable[0]);
                Asn1EncodableVector asn1EncodableVectors1 = new Asn1EncodableVector(new Asn1Encodable[0])
                {
                    new Asn1Encodable[] { new DerObjectIdentifier("1.2.840.113549.1.9.16.2.14") }
                };
                Asn1Sequence asn1Sequences = (Asn1Sequence)asn1InputStream.ReadObject();
                asn1EncodableVectors1.Add(new Asn1Encodable[] { new DerSet(asn1Sequences) });
                asn1EncodableVectors.Add(new Asn1Encodable[] { new DerSequence(asn1EncodableVectors1) });
                current = SignerInformation.ReplaceUnsignedAttributes(current, new Org.BouncyCastle.Asn1.Cms.AttributeTable(asn1EncodableVectors));
                IList arrayLists = new ArrayList();
                arrayLists.Add(current);
                cmsSignedDatum = CmsSignedData.ReplaceSigners(cmsSignedDatum, new SignerInformationStore(arrayLists));
                encoded = cmsSignedDatum.GetEncoded();
            }
            catch
            {
                throw;
            }
            return encoded;
        }

        private void TimestampForLtv(PdfSignatureAppearance sap)
        {
            int signatureByteBlockSize = 6096;
            if (this.SignatureByteBlockSize != 20480)
            {
                signatureByteBlockSize = this.SignatureByteBlockSize;
            }
            //sap.writer.AddDeveloperExtension(new PdfDeveloperExtension(new PdfName("ESIC"), new PdfName("1.7"), 5));
            sap.SetVisibleSignature(new itext.iTextSharp.text.Rectangle(0f, 0f, 0f, 0f), 1, null);
            //PdfSignatureExt pdfSignatureExt = new PdfSignatureExt(PdfName.ADOBE_PPKLITE, new PdfName("ETSI.RFC3161"));
            //pdfSignatureExt.Put(PdfName.TYPE, new PdfName("DocTimeStamp"));
            //sap.CryptoDictionary = pdfSignatureExt;
            Hashtable hashtables = new Hashtable();
            Dictionary<PdfName, int> dict = new Dictionary<PdfName, int>();
            dict[PdfName.CONTENTS] = signatureByteBlockSize * 2 + 2;
            sap.PreClose(dict);
            Stream rangeStream = sap.GetRangeStream();
            MemoryStream memoryStream = new MemoryStream();
            int num = 0;
            byte[] numArray = new byte[8192];
            while (true)
            {
                int num1 = rangeStream.Read(numArray, 0, 8192);
                num = num1;
                if (num1 <= 0)
                {
                    break;
                }
                memoryStream.Write(numArray, 0, num);
            }
            byte[] timestampToken = this.GetTimestampToken(this.ComputeHash(memoryStream.ToArray(), this.TimeStamping.HashAlgorithm));
            byte[] numArray1 = new byte[signatureByteBlockSize];
            Array.Copy(timestampToken, 0, numArray1, 0, (int)timestampToken.Length);
            PdfDictionary pdfDictionary = new PdfDictionary();
            pdfDictionary.Put(PdfName.CONTENTS, (new PdfString(numArray1)).SetHexWriting(true));
            sap.Close(pdfDictionary);
        }

        private byte[] ToByteArray(Stream stream)
        {
            byte[] array;
            try
            {
                byte[] numArray = new byte[4096];
                MemoryStream memoryStream = new MemoryStream();
                int num = 0;
                while (true)
                {
                    int num1 = stream.Read(numArray, 0, (int)numArray.Length);
                    num = num1;
                    if (num1 <= 0)
                    {
                        break;
                    }
                    memoryStream.Write(numArray, 0, num);
                }
                array = memoryStream.ToArray();
            }
            catch
            {
                array = null;
            }
            return array;
        }
    }
}