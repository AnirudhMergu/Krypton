using Krypton.Certificates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Packaging;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Krypton
{
    /// <summary>
    /// Class for digitally sign Office 2007, 2010 documents (docx, xlsx, pptx).
    /// </summary>
    public class OfficeSignature
    {
        private const string RT_OfficeDocument = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";

        /// <summary>
        /// Gets or sets the digital signature certificate.
        /// </summary>
        public X509Certificate2 DigitalSignatureCertificate
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of OfficeSign class.
        /// </summary>
        /// <param name="librarySerialNumberLicense">The serial number provided to register the library.</param>
        public OfficeSignature(string librarySerialNumberLicense)
        {
            try
            {
                Licensing.CheckLicense(librarySerialNumberLicense);
            }
            catch (Exception exception)
            {
                throw new ArgumentException(string.Concat("Licensing error: ", exception.Message));
            }
        }

        /// <summary>
        /// Digitally sign an Office 2007, 2010, 2013 document (docx, xlsx, pptx).
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
                Package package = Package.Open(outputFile);
                this.SignOpenOfficeXML(package);
                package.Close();
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
        /// Digitally sign an Office 2007, 2010, 2013 document (docx, xlsx, pptx).
        /// </summary>
        /// <param name="streamToSign">The stream that will be signed. The signed stream will be saved on the same stream.</param>
        public void ApplyDigitalSignature(Stream streamToSign)
        {
            try
            {
                if (this.DigitalSignatureCertificate == null)
                {
                    throw new NullReferenceException(CustomExceptions.DigitalCertificateIsNotSet);
                }
                Package package = Package.Open(streamToSign, FileMode.Open, FileAccess.ReadWrite);
                this.SignOpenOfficeXML(package);
                package.Close();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Digitally sign an Office 2007, 2010, 2013 document (docx, xlsx, pptx).
        /// </summary>
        /// <param name="inputArray">The input array that will be signed.</param>
        public byte[] ApplyDigitalSignature(byte[] inputArray)
        {
            byte[] array;
            try
            {
                if (this.DigitalSignatureCertificate == null)
                {
                    throw new NullReferenceException(CustomExceptions.DigitalCertificateIsNotSet);
                }
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.Write(inputArray, 0, (int)inputArray.Length);
                Package package = Package.Open(memoryStream, FileMode.Open, FileAccess.ReadWrite);
                this.SignOpenOfficeXML(package);
                package.Close();
                array = memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            return array;
        }

        private void CreateListOfSignableItems(PackageRelationship relationship, List<Uri> PartstobeSigned, List<PackageRelationshipSelector> SignableReleationships)
        {
            try
            {
                PackageRelationshipSelector packageRelationshipSelector = new PackageRelationshipSelector(relationship.SourceUri, PackageRelationshipSelectorType.Id, relationship.Id);
                SignableReleationships.Add(packageRelationshipSelector);
                if (relationship.TargetMode == TargetMode.Internal)
                {
                    PackagePart part = relationship.Package.GetPart(PackUriHelper.ResolvePartUri(relationship.SourceUri, relationship.TargetUri));
                    if (!PartstobeSigned.Contains(part.Uri))
                    {
                        PartstobeSigned.Add(part.Uri);
                        foreach (PackageRelationship packageRelationship in part.GetRelationships())
                        {
                            this.CreateListOfSignableItems(packageRelationship, PartstobeSigned, SignableReleationships);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Get the signing certificate used to perform the digital signature.
        /// </summary>
        /// <param name="inputFile">Path to the document.</param>
        /// <param name="signatureNumber">Signature number.</param>
        /// <returns></returns>
        public X509Certificate2 GetDigitalSignatureCertificate(string inputFile, int signatureNumber)
        {
            X509Certificate2 x509Certificate2;
            try
            {
                Licensing.ShowDemoMessage();
                if (!File.Exists(inputFile))
                {
                    throw new FileNotFoundException();
                }
                Package package = Package.Open(inputFile);
                PackageDigitalSignatureManager packageDigitalSignatureManager = new PackageDigitalSignatureManager(package);
                X509Certificate signer = packageDigitalSignatureManager.Signatures[signatureNumber - 1].Signer;
                package.Close();
                x509Certificate2 = new X509Certificate2(signer);
            }
            catch
            {
                throw;
            }
            return x509Certificate2;
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
                Package package = Package.Open(inputFile);
                int count = (new PackageDigitalSignatureManager(package)).Signatures.Count;
                package.Close();
                num = count;
            }
            catch
            {
                throw;
            }
            return num;
        }

        private void SignOpenOfficeXML(Package package)
        {
            try
            {
                Licensing.ShowDemoMessage();
                DigitalCertificate.LogOnEToken(this.DigitalSignatureCertificate);
                if (package == null)
                {
                    throw new ArgumentNullException();
                }
                List<Uri> uris = new List<Uri>();
                List<PackageRelationshipSelector> packageRelationshipSelectors = new List<PackageRelationshipSelector>();
                foreach (PackageRelationship relationshipsByType in package.GetRelationshipsByType("http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"))
                {
                    this.CreateListOfSignableItems(relationshipsByType, uris, packageRelationshipSelectors);
                }
                PackageDigitalSignatureManager packageDigitalSignatureManager = new PackageDigitalSignatureManager(package)
                {
                    CertificateOption = CertificateEmbeddingOption.InSignaturePart
                };
                packageDigitalSignatureManager.Sign(uris, this.DigitalSignatureCertificate, packageRelationshipSelectors);
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
                Package package = Package.Open(inputFile);
                PackageDigitalSignatureManager packageDigitalSignatureManager = new PackageDigitalSignatureManager(package);
                string str1 = packageDigitalSignatureManager.Signatures[signatureNumber - 1].Verify().ToString();
                package.Close();
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
