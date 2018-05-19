using Krypton.Certificates;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Packaging;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Xps.Packaging;

namespace Krypton
{
    /// <summary>
    /// Class for digitally sign XPS documents.
    /// </summary>
    public class XpsSignature
    {
        /// <summary>
        /// Allow multiple signatures to be added on the document. The default value is false.
        /// </summary>
        public bool AllowMultipleSignatures
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the digital signature certificate.
        /// </summary>
        public X509Certificate2 DigitalSignatureCertificate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the "Intent:" text. This property is included on the signature information only if AllowMultipleSignatures is set to false.
        /// </summary>
        public string SigningIntent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the "Location" text. This property is included on the signature information only if AllowMultipleSignatures is set to false.
        /// </summary>
        public string SigningLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of XPSSign class.
        /// </summary>
        /// <param name="librarySerialNumberLicense">The serial number provided to register the library.</param>
        public XpsSignature(string librarySerialNumberLicense)
        {
            try
            {
                Licensing.CheckLicense(librarySerialNumberLicense);
            }
            catch (Exception exception)
            {
                throw new ArgumentException(string.Concat("Licensing error: ", exception.Message));
            }
            this.AllowMultipleSignatures = false;
        }

        /// <summary>
        /// Digitally sign an XPS document.
        /// </summary>
        /// <param name="inputFile">Path to the unsigned document.</param>
        /// <param name="outputFile">Path to the signed document.</param>
        public void ApplyDigitalSignature(string inputFile, string outputFile)
        {
            try
            {
                if (!File.Exists(inputFile))
                {
                    throw new FileNotFoundException();
                }
                if (this.DigitalSignatureCertificate == null)
                {
                    throw new NullReferenceException(CustomExceptions.DigitalCertificateIsNotSet);
                }
                File.Copy(inputFile, outputFile, true);
                this.SignXPS(outputFile);
            }
            catch
            {
                try
                {
                    File.Delete(outputFile);
                }
                catch
                {
                }
                throw;
            }
        }

        /// <summary>
        /// Digitally sign a XPS stream.
        /// </summary>
        /// <param name="unsignedStream">The stream that will be signed. The signed stream will be saved on the same stream.</param>
        public void ApplyDigitalSignature(Stream unsignedStream)
        {
            try
            {
                if (this.DigitalSignatureCertificate == null)
                {
                    throw new NullReferenceException(CustomExceptions.DigitalCertificateIsNotSet);
                }
                this.SignXPSStream(unsignedStream);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Digitally sign a XPS byte array.
        /// </summary>
        /// <param name="inputArray">The input array that will be signed.</param>
        public byte[] ApplyDigitalSignature(byte[] inputArray)
        {
            if (this.DigitalSignatureCertificate == null)
            {
                throw new NullReferenceException(CustomExceptions.DigitalCertificateIsNotSet);
            }
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(inputArray, 0, (int)inputArray.Length);
            this.SignXPSStream(memoryStream);
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Get the number of signatures from the selected document.
        /// </summary>
        /// <param name="inputFile">Path to the document.</param>
        /// <returns>Number of digital signatures.</returns>
        public int GetNumberOfSignatures(string inputFile)
        {
            int num;
            try
            {
                if (!File.Exists(inputFile))
                {
                    throw new FileNotFoundException();
                }
                XpsDocument xpsDocument = new XpsDocument(inputFile, FileAccess.Read);
                int count = xpsDocument.Signatures.Count;
                xpsDocument.Close();
                num = count;
            }
            catch
            {
                throw;
            }
            return num;
        }

        private void SignXPS(string inputFile)
        {
            try
            {
                Licensing.ShowDemoMessage();
                DigitalCertificate.LogOnEToken(this.DigitalSignatureCertificate);
                XpsDocument xpsDocument = new XpsDocument(inputFile, FileAccess.ReadWrite);
                if (!this.AllowMultipleSignatures)
                {
                    XpsSignatureDefinition xpsSignatureDefinition = new XpsSignatureDefinition()
                    {
                        Intent = this.SigningIntent,
                        SigningLocale = this.SigningLocation,
                        SpotId = new Guid?(Guid.NewGuid())
                    };
                    IXpsFixedDocumentReader item = xpsDocument.FixedDocumentSequenceReader.FixedDocuments[0];
                    item.AddSignatureDefinition(xpsSignatureDefinition);
                    item.CommitSignatureDefinition();
                    X509Certificate2 digitalSignatureCertificate = this.DigitalSignatureCertificate;
                    Guid? spotId = xpsSignatureDefinition.SpotId;
                    xpsDocument.SignDigitally(digitalSignatureCertificate, true, XpsDigSigPartAlteringRestrictions.SignatureOrigin, spotId.Value);
                }
                else
                {
                    xpsDocument.SignDigitally(this.DigitalSignatureCertificate, true, XpsDigSigPartAlteringRestrictions.None);
                }
                xpsDocument.Close();
            }
            catch
            {
                throw;
            }
        }

        private void SignXPSStream(Stream inputStream)
        {
            try
            {
                Licensing.ShowDemoMessage();
                DigitalCertificate.LogOnEToken(this.DigitalSignatureCertificate);
                using (Package package = Package.Open(inputStream, FileMode.Open, FileAccess.ReadWrite))
                {
                    string str = "memorystream://myXps.xps";
                    Uri uri = new Uri(str);
                    PackageStore.AddPackage(uri, package);
                    XpsDocument xpsDocument = new XpsDocument(package, CompressionOption.Maximum, str);
                    if (!this.AllowMultipleSignatures)
                    {
                        XpsSignatureDefinition xpsSignatureDefinition = new XpsSignatureDefinition()
                        {
                            Intent = this.SigningIntent,
                            SigningLocale = this.SigningLocation,
                            SpotId = new Guid?(Guid.NewGuid())
                        };
                        IXpsFixedDocumentReader item = xpsDocument.FixedDocumentSequenceReader.FixedDocuments[0];
                        item.AddSignatureDefinition(xpsSignatureDefinition);
                        item.CommitSignatureDefinition();
                        X509Certificate2 digitalSignatureCertificate = this.DigitalSignatureCertificate;
                        Guid? spotId = xpsSignatureDefinition.SpotId;
                        xpsDocument.SignDigitally(digitalSignatureCertificate, true, XpsDigSigPartAlteringRestrictions.SignatureOrigin, spotId.Value);
                    }
                    else
                    {
                        xpsDocument.SignDigitally(this.DigitalSignatureCertificate, true, XpsDigSigPartAlteringRestrictions.None);
                    }
                    PackageStore.RemovePackage(uri);
                    xpsDocument.Close();
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Verifies a digital signature attached to the document.
        /// </summary>
        /// <param name="inputFile">Path to the document.</param>
        /// <param name="signatureNumber">Signature number.</param>
        /// <returns></returns>
        public string VerifyDigitalSignature(string inputFile, int signatureNumber)
        {
            string str;
            try
            {
                Licensing.ShowDemoMessage();
                if (!File.Exists(inputFile))
                {
                    throw new FileNotFoundException();
                }
                XpsDocument xpsDocument = new XpsDocument(inputFile, FileAccess.Read);
                string str1 = xpsDocument.Signatures[signatureNumber - 1].Verify().ToString();
                xpsDocument.Close();
                str = str1;
            }
            catch
            {
                throw;
            }
            return str;
        }
    }
}
