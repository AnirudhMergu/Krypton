using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Krypton;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Krypton.Certificates
{
    /// <summary>
    /// X509CertificateGenerator class.
    /// </summary>
    public class X509CertificateGenerator
    {
        private const long _DefaultSerialNumber_ = -9223372036854775808L;

        private IDictionary attrs;

        private IList ord;

        private X509Certificate rootCert;

        private AsymmetricCipherKeyPair rootKeyPair;

        /// <summary>
        /// Certificate extensions options.
        /// </summary>
        public Extensions Extensions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the certificate friendly name. The default value is not set.
        /// </summary>
        public string FriendlyName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key size of the key pair. The default value is KeySize1024Bit.
        /// </summary>
        public KeySize KeySize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the certificate serial number. If the property is not set a random value will be used.
        /// </summary>
        public long SerialNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the signature hash algorithm of the certificate. The default value is SHA1WithRSA.
        /// </summary>
        public SignatureAlgorithm SignatureAlgorithm
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Subject of the certificate.
        /// </summary>
        public string Subject
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the date in local time on which the certificate becomes valid. The default value is DateTime.Now.
        /// </summary>
        public DateTime ValidFrom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the date in local time after which a certificate is no longer valid. The default value is ValidFrom + 1 year. (On the demo version, the default value will be ValidFrom + 30 days)
        /// </summary>
        public DateTime ValidTo
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of X509CertificateGenerator class.
        /// </summary>
        /// <param name="librarySerialNumberLicense">The serial number provided to register the library.</param>
        public X509CertificateGenerator(string librarySerialNumberLicense)
        {
            try
            {
                Licensing.CheckLicense(librarySerialNumberLicense);
            }
            catch (Exception exception)
            {
                throw new ArgumentException(string.Concat("Licensing error: ", exception.Message));
            }
            this.Extensions = new Extensions();
            this.ValidFrom = DateTime.Now;
            this.ValidTo = this.ValidFrom.AddYears(1);
            this.KeySize = KeySize.KeySize1024Bit;
            this.SignatureAlgorithm = SignatureAlgorithm.SHA1WithRSA;
            this.FriendlyName = string.Empty;
            this.SerialNumber = -9223372036854775808L;
            this.rootCert = null;
            this.rootKeyPair = null;
            this.attrs = new Hashtable();
            this.ord = new ArrayList();
        }

        /// <summary>
        /// Adds an subject type to the certificate Subject (e.g. E=email@email.com, L=My locality). 
        /// </summary>
        /// <param name="subjectType">The subject type.</param>
        /// <param name="subjectTypeValue">The value of the subject type.</param>
        public void AddToSubject(SubjectType subjectType, string subjectTypeValue)
        {
            if (subjectType == SubjectType.C)
            {
                this.attrs[X509Name.C] = subjectTypeValue;
                this.ord.Add(X509Name.C);
            }
            if (subjectType == SubjectType.CN)
            {
                this.attrs[X509Name.CN] = subjectTypeValue;
                this.ord.Add(X509Name.CN);
            }
            if (subjectType == SubjectType.DC)
            {
                this.attrs[X509Name.DC] = subjectTypeValue;
                this.ord.Add(X509Name.DC);
            }
            if (subjectType == SubjectType.E)
            {
                this.attrs[X509Name.E] = subjectTypeValue;
                this.ord.Add(X509Name.E);
            }
            if (subjectType == SubjectType.EMAILADDRESS)
            {
                this.attrs[X509Name.EmailAddress] = subjectTypeValue;
                this.ord.Add(X509Name.EmailAddress);
            }
            if (subjectType == SubjectType.GIVENNAME)
            {
                this.attrs[X509Name.GivenName] = subjectTypeValue;
                this.ord.Add(X509Name.GivenName);
            }
            if (subjectType == SubjectType.INITIALS)
            {
                this.attrs[X509Name.Initials] = subjectTypeValue;
                this.ord.Add(X509Name.Initials);
            }
            if (subjectType == SubjectType.L)
            {
                this.attrs[X509Name.L] = subjectTypeValue;
                this.ord.Add(X509Name.L);
            }
            if (subjectType == SubjectType.NAME)
            {
                this.attrs[X509Name.Name] = subjectTypeValue;
                this.ord.Add(X509Name.Name);
            }
            if (subjectType == SubjectType.O)
            {
                this.attrs[X509Name.O] = subjectTypeValue;
                this.ord.Add(X509Name.O);
            }
            if (subjectType == SubjectType.OU)
            {
                this.attrs[X509Name.OU] = subjectTypeValue;
                this.ord.Add(X509Name.OU);
            }
            if (subjectType == SubjectType.PSEUDONYM)
            {
                this.attrs[X509Name.Pseudonym] = subjectTypeValue;
                this.ord.Add(X509Name.Pseudonym);
            }
            if (subjectType == SubjectType.SERIALNUMBER)
            {
                this.attrs[X509Name.SerialNumber] = subjectTypeValue;
                this.ord.Add(X509Name.SerialNumber);
            }
            if (subjectType == SubjectType.ST)
            {
                this.attrs[X509Name.ST] = subjectTypeValue;
                this.ord.Add(X509Name.ST);
            }
            if (subjectType == SubjectType.STREET)
            {
                this.attrs[X509Name.Street] = subjectTypeValue;
                this.ord.Add(X509Name.Street);
            }
            if (subjectType == SubjectType.SURNAME)
            {
                this.attrs[X509Name.Surname] = subjectTypeValue;
                this.ord.Add(X509Name.Surname);
            }
            if (subjectType == SubjectType.T)
            {
                this.attrs[X509Name.T] = subjectTypeValue;
                this.ord.Add(X509Name.T);
            }
            if (subjectType == SubjectType.UID)
            {
                this.attrs[X509Name.UID] = subjectTypeValue;
                this.ord.Add(X509Name.UID);
            }
        }

        /// <summary>
        /// Generate a PFX certificate.
        /// </summary>
        /// <param name="PFXFilePassword">PFX password to open the certificate file.</param>
        /// <returns>The PFX file as byte[] array.</returns>
        public byte[] GenerateCertificate(string PFXFilePassword)
        {
            return this.GenerateCertificate(PFXFilePassword, false);
        }

        /// <summary>
        /// Generate a PFX certificate.
        /// </summary>
        /// <param name="PFXFilePassword">PFX password to open the certificate file.</param>
        /// <param name="isRootCertificate">Set to true if the certificate is a Root certificate.</param>
        /// <returns>The PFX file as byte[] array.</returns>
        public byte[] GenerateCertificate(string PFXFilePassword, bool isRootCertificate)
        {
            X509Name x509Name;
            byte[] array;
            try
            {
                if (this.Subject == null && this.attrs.Count == 0)
                {
                    throw new CryptographicException("Certificate subject must be set.");
                }
                if (Licensing.demoVersion && this.ValidTo > this.ValidFrom.AddDays(30))
                {
                    Licensing.ShowDemoMessage();
                    this.ValidTo = this.ValidFrom.AddDays(30);
                }
                X509Certificate x509Certificate = null;
                AsymmetricCipherKeyPair asymmetricCipherKeyPair = this.MakeKeyPair(this.GetKeyLength(this.KeySize));
                AsymmetricKeyParameter @private = asymmetricCipherKeyPair.Private;
                x509Name = (string.IsNullOrEmpty(this.Subject) ? new X509Name(this.ord, this.attrs) : new X509Name(true, this.Subject));
                x509Certificate = (this.rootKeyPair == null || this.rootCert == null ? this.MakeCertificate(asymmetricCipherKeyPair, x509Name, asymmetricCipherKeyPair, x509Name, isRootCertificate, false) : this.MakeCertificate(asymmetricCipherKeyPair, x509Name, this.rootKeyPair, this.rootCert.SubjectDN, isRootCertificate, true));
                Pkcs12Store pkcs12Store = new Pkcs12Store();
                AsymmetricKeyEntry asymmetricKeyEntry = new AsymmetricKeyEntry(@private);
                X509CertificateEntry x509CertificateEntry = new X509CertificateEntry(x509Certificate);
                if (this.rootKeyPair == null || this.rootCert == null)
                {
                    X509CertificateEntry[] x509CertificateEntryArray = new X509CertificateEntry[] { x509CertificateEntry };
                    pkcs12Store.SetKeyEntry(this.FriendlyName, asymmetricKeyEntry, x509CertificateEntryArray);
                }
                else
                {
                    X509CertificateEntry x509CertificateEntry1 = new X509CertificateEntry(this.rootCert);
                    X509CertificateEntry[] x509CertificateEntryArray1 = new X509CertificateEntry[] { x509CertificateEntry, x509CertificateEntry1 };
                    pkcs12Store.SetKeyEntry(this.FriendlyName, asymmetricKeyEntry, x509CertificateEntryArray1);
                }
                MemoryStream memoryStream = new MemoryStream();
                pkcs12Store.Save(memoryStream, PFXFilePassword.ToCharArray(), this.GetSecureRandom());
                memoryStream.Flush();
                memoryStream.Close();
                array = memoryStream.ToArray();
            }
            catch (Exception exception)
            {
                throw new CryptographicException(string.Concat("Certificate generation error. ", exception.Message));
            }
            return array;
        }

        /// <summary>
        /// Generates the certificate based on CSR (Certificate Signing Request).
        /// </summary>
        /// <param name="CSR">The Certificate Signing Request.</param>
        /// <returns></returns>
        public byte[] GenerateCertificateFromCSR(string CSR)
        {
            byte[] numArray;
            try
            {
                if (CSR == null)
                {
                    throw new CryptographicException("CSR cannot be null.");
                }
                if (this.rootKeyPair == null || this.rootCert == null)
                {
                    throw new CryptographicException("Root certificate is not loaded.");
                }
                numArray = this.MakeCertificateFromCSR(CSR, this.rootKeyPair, this.rootCert.SubjectDN);
            }
            catch
            {
                throw;
            }
            return numArray;
        }

        private int GetKeyLength(KeySize keySize)
        {
            if (keySize == KeySize.KeySize512Bit)
            {
                return 512;
            }
            if (keySize == KeySize.KeySize1024Bit)
            {
                return 1024;
            }
            if (keySize == KeySize.KeySize2048Bit)
            {
                return 2048;
            }
            if (keySize == KeySize.KeySize4096Bit)
            {
                return 4096;
            }
            return 1024;
        }

        private string GetLicProc(string source)
        {
            return BitConverter.ToString((new SHA1CryptoServiceProvider()).ComputeHash(Encoding.Default.GetBytes(source))).Replace("-", string.Empty);
        }

        private SecureRandom GetSecureRandom()
        {
            RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
            byte[] numArray = new byte[100];
            rNGCryptoServiceProvider.GetBytes(numArray);
            return new SecureRandom(numArray);
        }

        /// <summary>
        /// Load the Root Certificate byte array to digitally sign other certificates.
        /// </summary>
        /// <param name="RootPFXCertificate">The byte array that contains the Root PFX file.</param>
        /// <param name="PFXFilePassword">The PFX password of the Root Certificate.</param>
        public void LoadRootCertificate(byte[] RootPFXCertificate, string PFXFilePassword)
        {
            if (RootPFXCertificate == null)
            {
                throw new Exception("Root PFX certificate cannot be null");
            }
            if (PFXFilePassword == null || PFXFilePassword == "")
            {
                throw new Exception("Root certificate password must be set!");
            }
            Pkcs12Store pkcs12Store = new Pkcs12Store(new MemoryStream(RootPFXCertificate), PFXFilePassword.ToCharArray());
            string current = null;
            IEnumerator enumerator = pkcs12Store.Aliases.GetEnumerator();
            do
            {
                if (!enumerator.MoveNext())
                {
                    break;
                }
                current = (string)enumerator.Current;
            }
            while (!pkcs12Store.IsKeyEntry(current));
            AsymmetricKeyParameter key = pkcs12Store.GetKey(current).Key;
            X509CertificateEntry[] certificateChain = pkcs12Store.GetCertificateChain(current);
            ArrayList arrayLists = new ArrayList();
            this.rootCert = certificateChain[0].Certificate;
            this.rootKeyPair = new AsymmetricCipherKeyPair(this.rootCert.GetPublicKey(), key);
        }

        private X509Certificate MakeCertificate(AsymmetricCipherKeyPair subjectKeyPair, X509Name certificateSubject, AsymmetricCipherKeyPair rootKeyPair, X509Name rootSubject, bool isRootCertificate, bool addAuthorityKeyIdentifier)
        {
            X509Certificate x509Certificate;
            try
            {
                AsymmetricKeyParameter @public = subjectKeyPair.Public;
                AsymmetricKeyParameter @private = rootKeyPair.Private;
                AsymmetricKeyParameter asymmetricKeyParameter = rootKeyPair.Public;
                X509V3CertificateGenerator x509V3CertificateGenerator = new X509V3CertificateGenerator();
                x509V3CertificateGenerator.Reset();
                if (this.SerialNumber != -9223372036854775808L)
                {
                    x509V3CertificateGenerator.SetSerialNumber(new BigInteger(this.SerialNumber.ToString()));
                }
                else
                {
                    DateTime now = DateTime.Now;
                    x509V3CertificateGenerator.SetSerialNumber(new BigInteger(128, new Random(now.Millisecond + Environment.TickCount)));
                }
                x509V3CertificateGenerator.SetIssuerDN(rootSubject);
                x509V3CertificateGenerator.SetNotBefore(this.ValidFrom.ToUniversalTime());
                x509V3CertificateGenerator.SetNotAfter(this.ValidTo.ToUniversalTime());
                x509V3CertificateGenerator.SetSubjectDN(certificateSubject);
                x509V3CertificateGenerator.SetPublicKey(@public);
                x509V3CertificateGenerator.SetSignatureAlgorithm(string.Concat(this.SignatureAlgorithm.ToString(), "Encryption"));
                int extensionType = 0;
                Asn1EncodableVector asn1EncodableVectors = new Asn1EncodableVector(new Asn1Encodable[0]);
                foreach (ExtensionInfo extension in this.Extensions.extensionInfo)
                {
                    if (!extension.ExtendedKeyUsage)
                    {
                        extensionType |= (int)extension.ExtensionType;
                    }
                    if (!extension.ExtendedKeyUsage)
                    {
                        continue;
                    }
                    asn1EncodableVectors.Add(new Asn1Encodable[] { (Asn1Encodable)extension.ExtensionType });
                }
                bool keyUsageIsCritical = this.Extensions.KeyUsageIsCritical;
                if (isRootCertificate)
                {
                    x509V3CertificateGenerator.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(true));
                    extensionType |= 6;
                    keyUsageIsCritical = true;
                }
                if (extensionType != 0)
                {
                    x509V3CertificateGenerator.AddExtension(X509Extensions.KeyUsage, keyUsageIsCritical, new KeyUsage(extensionType));
                }
                if (asn1EncodableVectors.Count > 0)
                {
                    x509V3CertificateGenerator.AddExtension(X509Extensions.ExtendedKeyUsage, this.Extensions.EnhancedKeyUsageIsCritical, ExtendedKeyUsage.GetInstance(new DerSequence(asn1EncodableVectors)));
                }
                x509V3CertificateGenerator.AddExtension(X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifier(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(@public)));
                if (addAuthorityKeyIdentifier)
                {
                    x509V3CertificateGenerator.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifier(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(asymmetricKeyParameter)));
                }
                X509Certificate x509Certificate1 = x509V3CertificateGenerator.Generate(@private, this.GetSecureRandom());
                x509Certificate1.Verify(asymmetricKeyParameter);
                x509Certificate = x509Certificate1;
            }
            catch
            {
                throw;
            }
            return x509Certificate;
        }

        private byte[] MakeCertificateFromCSR(string CSR, AsymmetricCipherKeyPair rootKeyPair, X509Name rootSubject)
        {
            byte[] encoded;
            try
            {
                Pkcs10CertificationRequest pkcs10CertificationRequest = (Pkcs10CertificationRequest)(new PemReader(new StringReader(CSR))).ReadObject();
                AsymmetricKeyParameter @private = rootKeyPair.Private;
                AsymmetricKeyParameter @public = rootKeyPair.Public;
                X509V3CertificateGenerator x509V3CertificateGenerator = new X509V3CertificateGenerator();
                x509V3CertificateGenerator.Reset();
                if (this.SerialNumber != -9223372036854775808L)
                {
                    x509V3CertificateGenerator.SetSerialNumber(new BigInteger(this.SerialNumber.ToString()));
                }
                else
                {
                    DateTime now = DateTime.Now;
                    x509V3CertificateGenerator.SetSerialNumber(new BigInteger(128, new Random(now.Millisecond + Environment.TickCount)));
                }
                x509V3CertificateGenerator.SetIssuerDN(rootSubject);
                x509V3CertificateGenerator.SetNotBefore(this.ValidFrom.ToUniversalTime());
                x509V3CertificateGenerator.SetNotAfter(this.ValidTo.ToUniversalTime());
                x509V3CertificateGenerator.SetSubjectDN(pkcs10CertificationRequest.GetCertificationRequestInfo().Subject);
                x509V3CertificateGenerator.SetPublicKey(pkcs10CertificationRequest.GetPublicKey());
                x509V3CertificateGenerator.SetSignatureAlgorithm(string.Concat(this.SignatureAlgorithm.ToString(), "Encryption"));
                x509V3CertificateGenerator.AddExtension(X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifier(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pkcs10CertificationRequest.GetPublicKey())));
                x509V3CertificateGenerator.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifier(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(@public)));
                int extensionType = 0;
                Asn1EncodableVector asn1EncodableVectors = new Asn1EncodableVector(new Asn1Encodable[0]);
                foreach (ExtensionInfo extension in this.Extensions.extensionInfo)
                {
                    if (!extension.ExtendedKeyUsage)
                    {
                        extensionType |= (int)extension.ExtensionType;
                    }
                    if (!extension.ExtendedKeyUsage)
                    {
                        continue;
                    }
                    asn1EncodableVectors.Add(new Asn1Encodable[] { (Asn1Encodable)extension.ExtensionType });
                }
                if (extensionType != 0)
                {
                    x509V3CertificateGenerator.AddExtension(X509Extensions.KeyUsage, this.Extensions.KeyUsageIsCritical, new KeyUsage(extensionType));
                }
                if (asn1EncodableVectors.Count > 0)
                {
                    x509V3CertificateGenerator.AddExtension(X509Extensions.ExtendedKeyUsage, this.Extensions.EnhancedKeyUsageIsCritical, ExtendedKeyUsage.GetInstance(new DerSequence(asn1EncodableVectors)));
                }
                X509Certificate x509Certificate = x509V3CertificateGenerator.Generate(@private, this.GetSecureRandom());
                x509Certificate.Verify(@public);
                encoded = x509Certificate.GetEncoded();
            }
            catch
            {
                throw;
            }
            return encoded;
        }

        private AsymmetricCipherKeyPair MakeKeyPair(int strenght)
        {
            AsymmetricCipherKeyPair asymmetricCipherKeyPair;
            try
            {
                IAsymmetricCipherKeyPairGenerator keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator("RSA");
                keyPairGenerator.Init(new RsaKeyGenerationParameters(BigInteger.ValueOf((long)65537), this.GetSecureRandom(), strenght, 25));
                asymmetricCipherKeyPair = keyPairGenerator.GenerateKeyPair();
            }
            catch
            {
                throw;
            }
            return asymmetricCipherKeyPair;
        }
    }
}
