using Krypton.Certificates;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace Krypton
{
    /// <summary>
    /// Class for digitally sign XML documents (XMLDsig format).
    /// </summary>
    public class XmlSignatureSha512
    {
        /// <summary>
        /// Gets or sets the digital signature certificate.
        /// </summary>
        public X509Certificate2 DigitalSignatureCertificate
        {
            get;
            set;
        }

        /// <summary>
        /// Include the key information (RSA DSA modulus/exponent) in the digital signature. The default value is true.
        /// </summary>
        public bool IncludeKeyInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Include the signing certificate in the digital signature. The default value is true.
        /// </summary>
        public bool IncludeSignatureCertificate
        {
            get;
            set;
        }

        /// <summary>
        /// Remove white spaces and signature visual formatting. The default value is true.
        /// </summary>
        public bool RemoveWhitespaces
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of XMLSign class.
        /// </summary>
        /// <param name="librarySerialNumberLicense">The serial number provided to register the library.</param>
        public XmlSignatureSha512(string librarySerialNumberLicense)
        {
            try
            {
                Licensing.CheckLicense(librarySerialNumberLicense);
            }
            catch (Exception exception)
            {
                throw new ArgumentException(string.Concat("Licensing error: ", exception.Message));
            }
            try
            {
                CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA512SignatureDescription), new string[] { "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512" });
            }
            catch
            {
            }
            this.IncludeKeyInfo = true;
            this.IncludeSignatureCertificate = true;
            this.RemoveWhitespaces = true;
        }

        /// <summary>
        /// Digitally sign an XML document.
        /// </summary>
        /// <param name="inputFile">Path to the unsigned XML document.</param>
        /// <param name="outputFile">Path to the signed XML document.</param>
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
                if (this.GetNumberOfSignatures(inputFile) >= 1)
                {
                    throw new CryptographicException(CustomExceptions.XMLNoMoreOneDigitalSignature);
                }
                this.SignXml(inputFile, outputFile);
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

        public X509Certificate2 GetDigitalSignatureCertificate(string inputFile)
        {
            X509Certificate2 x509Certificate2;
            try
            {
                if (!File.Exists(inputFile))
                {
                    throw new FileNotFoundException();
                }
                XmlDocument xmlDocument = new XmlDocument()
                {
                    PreserveWhitespace = this.RemoveWhitespaces
                };
                xmlDocument.Load(inputFile);
                SignedXml signedXml = new SignedXml(xmlDocument);
                XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("Signature");
                if (elementsByTagName.Count <= 0)
                {
                    throw new CryptographicException(CustomExceptions.XMLSignatureNoDigitalSignatures);
                }
                signedXml.LoadXml((XmlElement)elementsByTagName[0]);
                X509Certificate2 x509Certificate21 = new X509Certificate2(Encoding.UTF8.GetBytes(signedXml.Signature.KeyInfo.GetXml().GetElementsByTagName("X509Certificate").Item(0).InnerText));
                x509Certificate21.GetSerialNumber();
                x509Certificate2 = x509Certificate21;
            }
            catch
            {
                throw;
            }
            return x509Certificate2;
        }

        /// <summary>
        /// Get the number of signatures from the selected XML document.
        /// </summary>
        /// <param name="inputFile">Path to the document.</param>
        /// <returns>Number of digital signatures.</returns>
        public int GetNumberOfSignatures(string inputFile)
        {
            int count;
            try
            {
                if (!File.Exists(inputFile))
                {
                    throw new FileNotFoundException();
                }
                XmlDocument xmlDocument = new XmlDocument()
                {
                    PreserveWhitespace = this.RemoveWhitespaces
                };
                xmlDocument.Load(inputFile);
                SignedXml signedXml = new SignedXml(xmlDocument);
                count = xmlDocument.GetElementsByTagName("Signature").Count;
            }
            catch
            {
                throw;
            }
            return count;
        }

        public string GetSignatureAlgorithm(string inputFile)
        {
            string upper;
            try
            {
                if (!File.Exists(inputFile))
                {
                    throw new FileNotFoundException();
                }
                XmlDocument xmlDocument = new XmlDocument()
                {
                    PreserveWhitespace = this.RemoveWhitespaces
                };
                xmlDocument.Load(inputFile);
                SignedXml signedXml = new SignedXml(xmlDocument);
                XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("Signature");
                if (elementsByTagName.Count <= 0)
                {
                    throw new CryptographicException(CustomExceptions.XMLSignatureNoDigitalSignatures);
                }
                signedXml.LoadXml((XmlElement)elementsByTagName[0]);
                upper = signedXml.SignedInfo.SignatureMethod.Replace("http://www.w3.org/2000/09/xmldsig#", string.Empty).Replace("http://www.w3.org/2001/04/xmldsig-more#", string.Empty).ToUpper();
            }
            catch
            {
                throw;
            }
            return upper;
        }

        private void SignXml(string inputFile, string outputFile)
        {
            try
            {
                Licensing.ShowDemoMessage();
                DigitalCertificate.LogOnEToken(this.DigitalSignatureCertificate);
                XmlDocument xmlDocument = new XmlDocument()
                {
                    PreserveWhitespace = this.RemoveWhitespaces
                };
                xmlDocument.Load(inputFile);
                SignedXml signedXml = new SignedXml(xmlDocument);
                RSACryptoServiceProvider rSACryptoServiceProvider = null;
                try
                {
                    string xmlString = this.DigitalSignatureCertificate.PrivateKey.ToXmlString(true);
                    rSACryptoServiceProvider = new RSACryptoServiceProvider(new CspParameters(24))
                    {
                        PersistKeyInCsp = false
                    };
                    rSACryptoServiceProvider.FromXmlString(xmlString);
                    signedXml.SigningKey = rSACryptoServiceProvider;
                }
                catch
                {
                    rSACryptoServiceProvider = this.DigitalSignatureCertificate.PrivateKey as RSACryptoServiceProvider;
                    signedXml.SigningKey = rSACryptoServiceProvider;
                }
                signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512";
                if (this.IncludeKeyInfo || this.IncludeSignatureCertificate)
                {
                    KeyInfo keyInfos = new KeyInfo();
                    if (this.IncludeKeyInfo)
                    {
                        keyInfos.AddClause(new RSAKeyValue(rSACryptoServiceProvider));
                    }
                    if (this.IncludeSignatureCertificate)
                    {
                        keyInfos.AddClause(new KeyInfoX509Data(this.DigitalSignatureCertificate.GetRawCertData()));
                    }
                    signedXml.KeyInfo = keyInfos;
                }
                Reference reference = new Reference();
                reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                reference.AddTransform(new XmlDsigExcC14NTransform());
                reference.Uri = "";
                reference.DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha512";
                signedXml.AddReference(reference);
                signedXml.ComputeSignature();
                SignatureDescription signatureDescription = new SignatureDescription();
                XmlElement xml = signedXml.GetXml();
                xmlDocument.DocumentElement.AppendChild(xmlDocument.ImportNode(xml, true));
                xmlDocument.Save(outputFile);
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
        /// <returns>Returns true if the signature is valid.</returns>
        public bool VerifyDigitalSignature(string inputFile)
        {
            bool flag;
            try
            {
                Licensing.ShowDemoMessage();
                if (!File.Exists(inputFile))
                {
                    throw new FileNotFoundException();
                }
                XmlDocument xmlDocument = new XmlDocument()
                {
                    PreserveWhitespace = this.RemoveWhitespaces
                };
                xmlDocument.Load(inputFile);
                SignedXml signedXml = new SignedXml(xmlDocument);
                XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("Signature");
                if (elementsByTagName.Count <= 0)
                {
                    throw new CryptographicException(CustomExceptions.XMLSignatureNoDigitalSignatures);
                }
                if (elementsByTagName.Count >= 2)
                {
                    throw new CryptographicException(CustomExceptions.XMLSignatureMoreThanOneSignature);
                }
                signedXml.LoadXml((XmlElement)elementsByTagName[0]);
                flag = signedXml.CheckSignature();
            }
            catch
            {
                throw;
            }
            return flag;
        }

        /// <summary>
        /// Verifies a digital signature attached to the document.
        /// </summary>
        /// <param name="inputFile">Path to the document.</param>
        /// <param name="certificate">The signer certificate.</param>
        /// <returns>Returns true if the signature is valid.</returns>
        public bool VerifyDigitalSignature(string inputFile, X509Certificate2 certificate)
        {
            bool flag;
            try
            {
                Licensing.ShowDemoMessage();
                if (!File.Exists(inputFile))
                {
                    throw new FileNotFoundException();
                }
                XmlDocument xmlDocument = new XmlDocument()
                {
                    PreserveWhitespace = this.RemoveWhitespaces
                };
                xmlDocument.Load(inputFile);
                SignedXml signedXml = new SignedXml(xmlDocument);
                XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("Signature");
                if (elementsByTagName.Count <= 0)
                {
                    throw new CryptographicException(CustomExceptions.XMLSignatureNoDigitalSignatures);
                }
                if (elementsByTagName.Count >= 2)
                {
                    throw new CryptographicException(CustomExceptions.XMLSignatureMoreThanOneSignature);
                }
                signedXml.LoadXml((XmlElement)elementsByTagName[0]);
                flag = signedXml.CheckSignature(certificate, true);
            }
            catch
            {
                throw;
            }
            return flag;
        }
    }
}
