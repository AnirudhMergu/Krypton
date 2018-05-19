extern alias itext;
using Org.BouncyCastle.Cms;
using Krypton.Certificates;
using System;
using System.IO;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Net;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Math;
using System.Collections;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using System.Collections.Generic;
using Org.BouncyCastle.X509;
using System.DirectoryServices;
using System.ComponentModel;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Crypto;

namespace Krypton.Certificates
{
    /// <summary>
    /// Class for digital certificates used for signing.
    /// </summary>
    public class DigitalCertificate
    {
        internal static bool useSmartCardPIN;

        /// <summary>
        /// Sets the PIN for the signing certificate stored on a smart card. Some smart cards not support this feature.
        /// </summary>
        public static string SmartCardPin
        {
            get;
            set;
        }

        /// <summary>
        /// Use an external signature provider (e.g. an external PKCS#11 module).
        /// </summary>
        public static IExternalSignature UseExternalSignatureProvider
        {
            set
            {
                //CmsSignedDataGenerator.SignerInf.externalSignatureObject = value;
            }
        }

        static DigitalCertificate()
        {
        }

        public DigitalCertificate()
        {
        }

        private static string cInfo(string InformationString, string ExpressionString)
        {
            string str;
            try
            {
                string[] strArrays = (new Regex(",", RegexOptions.IgnoreCase)).Split(InformationString);
                int num = 0;
                while (num < (int)strArrays.Length)
                {
                    string str1 = strArrays[num];
                    if (!str1.Trim().StartsWith(ExpressionString))
                    {
                        num++;
                    }
                    else
                    {
                        str = str1.Trim().Substring(ExpressionString.Length);
                        return str;
                    }
                }
                str = null;
            }
            catch
            {
                throw new FormatException();
            }
            return str;
        }

        private static byte[] DownloadBytesFromURL(Uri url)
        {
            byte[] numArray;
            try
            {
                WebRequest webRequest = WebRequest.Create(url);
                webRequest.Timeout = 30000;
                Stream responseStream = webRequest.GetResponse().GetResponseStream();
                byte[] byteArray = DigitalCertificate.ToByteArray(responseStream);
                responseStream.Close();
                numArray = byteArray;
            }
            catch
            {
                numArray = null;
            }
            return numArray;
        }

        private static byte[] DownloadDataFromHttp(Uri url)
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

        private static OcspReq GenerateOcspRequest(Org.BouncyCastle.X509.X509Certificate issuerCert, BigInteger serialNumber)
        {
            OcspReq ocspReq;
            try
            {
                ocspReq = DigitalCertificate.GenerateOcspRequest(new CertificateID("1.3.14.3.2.26", issuerCert, serialNumber));
            }
            catch
            {
                throw;
            }
            return ocspReq;
        }

        private static OcspReq GenerateOcspRequest(CertificateID id)
        {
            OcspReq ocspReq;
            try
            {
                OcspReqGenerator ocspReqGenerator = new OcspReqGenerator();
                ocspReqGenerator.AddRequest(id);
                BigInteger.ValueOf((new DateTime()).Ticks);
                ArrayList arrayLists = new ArrayList();
                Hashtable hashtables = new Hashtable();
                arrayLists.Add(OcspObjectIdentifiers.PkixOcsp);
                Asn1OctetString derOctetString = new DerOctetString(new DerOctetString(new byte[] { 1, 3, 6, 1, 5, 5, 7, 48, 1, 1 }));
                hashtables.Add(OcspObjectIdentifiers.PkixOcsp, new Org.BouncyCastle.Asn1.X509.X509Extension(false, derOctetString));
#pragma warning disable CS0612 // Type or member is obsolete
                ocspReqGenerator.SetRequestExtensions(new X509Extensions(arrayLists, hashtables));
#pragma warning restore CS0612 // Type or member is obsolete
                ocspReq = ocspReqGenerator.Generate();
            }
            catch
            {
                throw;
            }
            return ocspReq;
        }

        private static Uri GetAuthorityInformationAccessOcspUrl(Org.BouncyCastle.X509.X509Certificate cert)
        {
            Uri uri;
            try
            {
                Asn1Object extensionValue = DigitalCertificate.GetExtensionValue(cert, X509Extensions.AuthorityInfoAccess.Id);
                if (extensionValue != null)
                {
                    IEnumerator enumerator = ((Asn1Sequence)extensionValue).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Asn1Sequence current = (Asn1Sequence)enumerator.Current;
                        if (!((DerObjectIdentifier)current[0]).Id.Equals("1.3.6.1.5.5.7.48.1"))
                        {
                            continue;
                        }
                        GeneralName instance = GeneralName.GetInstance((Asn1TaggedObject)current[1]);
                        uri = new Uri(DerIA5String.GetInstance(instance.Name).GetString());
                        return uri;
                    }
                    uri = null;
                }
                else
                {
                    uri = null;
                }
            }
            catch
            {
                uri = null;
            }
            return uri;
        }

        internal static byte[] GetBasicOCSPResponseFromURL(Uri url, byte[] data, string contentType, string accept)
        {
            byte[] encoded;
            try
            {
                HttpWebRequest length = (HttpWebRequest)WebRequest.Create(url);
                length.Method = "POST";
                length.Timeout = 15000;
                length.ContentType = contentType;
                length.Accept = accept;
                length.ContentLength = (long)((int)data.Length);
                Stream requestStream = length.GetRequestStream();
                requestStream.Write(data, 0, (int)data.Length);
                requestStream.Close();
                Stream responseStream = ((HttpWebResponse)length.GetResponse()).GetResponseStream();
                encoded = ((BasicOcspResp)(new OcspResp(responseStream)).GetResponseObject()).GetEncoded();
            }
            catch
            {
                encoded = null;
            }
            return encoded;
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

        private static X509Certificate2 GetCertificate(bool validOnly, string issuerName, string windowTitle, string windowDescription, bool fromLocalMachine)
        {
            X509Certificate2 current;
            X509Store x509Store = null;
            try
            {
                try
                {
                    try
                    {
                        StoreLocation storeLocation = StoreLocation.CurrentUser;
                        if (fromLocalMachine)
                        {
                            storeLocation = StoreLocation.LocalMachine;
                        }
                        x509Store = new X509Store(StoreName.My, storeLocation);
                        x509Store.Open(OpenFlags.OpenExistingOnly);
                    }
                    catch
                    {
                        throw new CryptographicException("Signing certificates store cannot be opened.");
                    }
                    X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByIssuerName, issuerName, validOnly);
                    if (x509Certificate2Collection.Count != 0)
                    {
                        X509Certificate2Collection x509Certificate2Collection1 = X509Certificate2UI.SelectFromCollection(x509Certificate2Collection, windowTitle, windowDescription, X509SelectionFlag.SingleSelection);
                        if (x509Certificate2Collection1.Count <= 0)
                        {
                            current = null;
                        }
                        else
                        {
                            X509Certificate2Enumerator enumerator = x509Certificate2Collection1.GetEnumerator();
                            enumerator.MoveNext();
                            current = enumerator.Current;
                        }
                    }
                    else
                    {
                        current = null;
                    }
                }
                catch
                {
                    throw;
                }
            }
            finally
            {
                if (x509Store != null)
                {
                    x509Store.Close();
                }
            }
            return current;
        }

        private static X509Certificate2 GetCertificateWithCriteria(bool validOnly, string searchString, string criteria, bool fromLocalMachine)
        {
            X509Certificate2 x509Certificate2;
            X509Store x509Store = null;
            try
            {
                try
                {
                    try
                    {
                        StoreLocation storeLocation = StoreLocation.CurrentUser;
                        if (fromLocalMachine)
                        {
                            storeLocation = StoreLocation.LocalMachine;
                        }
                        x509Store = new X509Store(StoreName.My, storeLocation);
                        x509Store.Open(OpenFlags.OpenExistingOnly);
                    }
                    catch
                    {
                        throw new CryptographicException("Signing certificates store cannot be opened.");
                    }
                    X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByIssuerName, "", validOnly);
                    if (x509Certificate2Collection.Count != 0)
                    {
                        X509Certificate2Enumerator enumerator = x509Certificate2Collection.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            X509Certificate2 current = enumerator.Current;
                            if (criteria == "Thumbprint" && current.Thumbprint.ToUpper().Replace(" ", "") == searchString.ToUpper().Replace(" ", ""))
                            {
                                x509Certificate2 = current;
                                return x509Certificate2;
                            }
                            else if (!(criteria == "SerialNumber") || !(current.SerialNumber.ToUpper().Replace(" ", "") == searchString.ToUpper().Replace(" ", "")))
                            {
                                if (DigitalCertificate.cInfo(current.SubjectName.Name.Trim().ToUpper(), criteria.Trim().ToUpper()) == null || !DigitalCertificate.cInfo(current.SubjectName.Name.Trim().ToUpper(), criteria.Trim().ToUpper()).ToUpper().Trim().Contains(searchString.ToUpper().Trim()))
                                {
                                    continue;
                                }
                                x509Certificate2 = current;
                                return x509Certificate2;
                            }
                            else
                            {
                                x509Certificate2 = current;
                                return x509Certificate2;
                            }
                        }
                        x509Certificate2 = null;
                    }
                    else
                    {
                        x509Certificate2 = null;
                    }
                }
                catch
                {
                    throw;
                }
            }
            finally
            {
                if (x509Store != null)
                {
                    x509Store.Close();
                }
            }
            return x509Certificate2;
        }

        private static X509Chain GetChain(X509Certificate2 signingCertificate)
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

        private static Asn1Object GetExtensionValue(Org.BouncyCastle.X509.X509Certificate cert, string oid)
        {
            Asn1Object asn1Object;
            try
            {
                if (cert != null)
                {
                    byte[] octets = cert.GetExtensionValue(new DerObjectIdentifier(oid)).GetOctets();
                    if (octets != null)
                    {
                        asn1Object = (new Asn1InputStream(octets)).ReadObject();
                    }
                    else
                    {
                        asn1Object = null;
                    }
                }
                else
                {
                    asn1Object = null;
                }
            }
            catch
            {
                asn1Object = null;
            }
            return asn1Object;
        }

        public static List<CertificateKeyUsage> GetKeyUsage(X509Certificate2 certificate)
        {
            List<CertificateKeyUsage> certificateKeyUsages;
            List<CertificateKeyUsage> certificateKeyUsages1 = new List<CertificateKeyUsage>();
            try
            {
                Org.BouncyCastle.X509.X509Certificate x509Certificate = (new X509CertificateParser()).ReadCertificate(certificate.RawData);
                for (int i = 0; i < (int)x509Certificate.GetKeyUsage().Length; i++)
                {
                    if (i == 0 && x509Certificate.GetKeyUsage()[i])
                    {
                        certificateKeyUsages1.Add(CertificateKeyUsage.DigitalSignature);
                    }
                    if (i == 1 && x509Certificate.GetKeyUsage()[i])
                    {
                        certificateKeyUsages1.Add(CertificateKeyUsage.NonRepudiation);
                    }
                    if (i == 2 && x509Certificate.GetKeyUsage()[i])
                    {
                        certificateKeyUsages1.Add(CertificateKeyUsage.KeyEncipherment);
                    }
                    if (i == 3 && x509Certificate.GetKeyUsage()[i])
                    {
                        certificateKeyUsages1.Add(CertificateKeyUsage.DataEncipherment);
                    }
                    if (i == 4 && x509Certificate.GetKeyUsage()[i])
                    {
                        certificateKeyUsages1.Add(CertificateKeyUsage.KeyAgreement);
                    }
                    if (i == 5 && x509Certificate.GetKeyUsage()[i])
                    {
                        certificateKeyUsages1.Add(CertificateKeyUsage.CertificateSigning);
                    }
                    if (i == 6 && x509Certificate.GetKeyUsage()[i])
                    {
                        certificateKeyUsages1.Add(CertificateKeyUsage.CRLSigning);
                    }
                    if (i == 7 && x509Certificate.GetKeyUsage()[i])
                    {
                        certificateKeyUsages1.Add(CertificateKeyUsage.EncipherOnly);
                    }
                    if (i == 8 && x509Certificate.GetKeyUsage()[i])
                    {
                        certificateKeyUsages1.Add(CertificateKeyUsage.DecipherOnly);
                    }
                }
                certificateKeyUsages = certificateKeyUsages1;
            }
            catch
            {
                certificateKeyUsages = null;
            }
            return certificateKeyUsages;
        }

        internal static byte[] GetOCSPResponse(Org.BouncyCastle.X509.X509Certificate certificate, Org.BouncyCastle.X509.X509Certificate issuerCert)
        {
            Uri authorityInformationAccessOcspUrl = DigitalCertificate.GetAuthorityInformationAccessOcspUrl(certificate);
            if (authorityInformationAccessOcspUrl == null)
            {
                return null;
            }
            OcspReq ocspReq = DigitalCertificate.GenerateOcspRequest(issuerCert, certificate.SerialNumber);
            return DigitalCertificate.GetBasicOCSPResponseFromURL(authorityInformationAccessOcspUrl, ocspReq.GetEncoded(), "application/ocsp-request", "application/ocsp-response");
        }

        private static byte[] GetOCSPResponseFromURL(Uri url, byte[] data, string contentType, string accept)
        {
            byte[] numArray;
            try
            {
                HttpWebRequest length = (HttpWebRequest)WebRequest.Create(url);
                length.Method = "POST";
                length.Timeout = 15000;
                length.ContentType = contentType;
                length.Accept = accept;
                length.ContentLength = (long)((int)data.Length);
                Stream requestStream = length.GetRequestStream();
                requestStream.Write(data, 0, (int)data.Length);
                requestStream.Close();
                Stream responseStream = ((HttpWebResponse)length.GetResponse()).GetResponseStream();
                byte[] byteArray = DigitalCertificate.ToByteArray(responseStream);
                responseStream.Close();
                numArray = byteArray;
            }
            catch
            {
                numArray = null;
            }
            return numArray;
        }

        private static Org.BouncyCastle.X509.X509Certificate GetRootCertificate(X509Certificate2 signingCertificate)
        {
            Org.BouncyCastle.X509.X509Certificate x509Certificate;
            try
            {
                X509Chain chain = DigitalCertificate.GetChain(signingCertificate);
                if (chain.ChainElements.Count != 1)
                {
                    string nameInfo = signingCertificate.GetNameInfo(X509NameType.SimpleName, true);
                    X509ChainElementEnumerator enumerator = chain.ChainElements.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        X509ChainElement current = enumerator.Current;
                        if (current.Certificate.GetNameInfo(X509NameType.SimpleName, false) != nameInfo)
                        {
                            continue;
                        }
                        x509Certificate = (new X509CertificateParser()).ReadCertificate(current.Certificate.RawData);
                        return x509Certificate;
                    }
                    x509Certificate = null;
                }
                else
                {
                    x509Certificate = null;
                }
            }
            catch
            {
                x509Certificate = null;
            }
            return x509Certificate;
        }

        private static CertificateStatus IsRevoked(string crlUrl, Org.BouncyCastle.X509.X509Certificate revokedCert)
        {
            CertificateStatus certificateStatu;
            try
            {
                byte[] numArray = DigitalCertificate.DownloadDataFromHttp(new Uri(crlUrl)) ?? DigitalCertificate.DownloadBytesFromURL(new Uri(crlUrl));
                X509Crl x509Crl = new X509Crl(CertificateList.GetInstance(Asn1Object.FromByteArray(numArray)));
                x509Crl.GetRevokedCertificates();
                certificateStatu = (!x509Crl.IsRevoked(revokedCert) ? CertificateStatus.Valid : CertificateStatus.Revoked);// Revoked);
            }
            catch
            {
                certificateStatu = CertificateStatus.Valid;
            }
            return certificateStatu;
        }

        private static CertificateStatus IsRevokedLdap(string crlUrl, Org.BouncyCastle.X509.X509Certificate revokedCert)
        {
            CertificateStatus certificateStatu;
            try
            {
                byte[] value = null;
                Uri uri = new Uri(crlUrl);
                DirectorySearcher directorySearcher = new DirectorySearcher();
                string[] strArrays = new string[] { "certificateRevocationList;binary" };
                directorySearcher.SearchRoot = new DirectoryEntry(string.Concat("LDAP://", uri.GetComponents(UriComponents.Host, UriFormat.Unescaped), "/", uri.GetComponents(UriComponents.Path, UriFormat.Unescaped)), null, null, AuthenticationTypes.None);
                directorySearcher.SearchScope = SearchScope.Subtree;
                directorySearcher.PropertiesToLoad.AddRange(strArrays);
                foreach (SearchResult searchResult in directorySearcher.FindAll())
                {
                    value = (byte[])searchResult.GetDirectoryEntry().Properties["certificateRevocationList;binary"].Value;
                }
                X509Crl x509Crl = new X509Crl(CertificateList.GetInstance(Asn1Object.FromByteArray(value)));
                x509Crl.GetRevokedCertificates();
                certificateStatu = (!x509Crl.IsRevoked(revokedCert) ? CertificateStatus.Valid : CertificateStatus.Revoked); // Revoked);
            }
            catch
            {
                certificateStatu = CertificateStatus.Valid;
            }
            return certificateStatu;
        }

        /// <summary>
        /// Loads the digital certificate from a byte array (PFX, P12).
        /// </summary>
        /// <param name="certificate">The byte array that contains the certificate.</param>
        /// <param name="password">Certificate password.</param>
        /// <returns>Certificate as X509Certificate2.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">Throws an exception if the certificate cannot be loaded.</exception>
        public static X509Certificate2 LoadCertificate(byte[] certificate, string password)
        {
            X509Certificate2 x509Certificate2;
            try
            {
                x509Certificate2 = new X509Certificate2(certificate, password);
            }
            catch
            {
                try
                {
                    x509Certificate2 = new X509Certificate2(certificate, password, X509KeyStorageFlags.MachineKeySet);
                }
                catch
                {
                    throw;
                }
            }
            return x509Certificate2;
        }

        /// <summary>
        /// Loads the digital certificate from a file (PFX, P12).
        /// </summary>
        /// <param name="certificateFile">The file that contains the certificate.</param>
        /// <param name="password">Certificate password.</param>
        /// <returns>Certificate as X509Certificate2.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">Throws an exception if the certificate cannot be loaded.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">Throws an exception if the certificate file not exists.</exception>
        public static X509Certificate2 LoadCertificate(string certificateFile, string password)
        {
            X509Certificate2 x509Certificate2;
            try
            {
                if (!File.Exists(certificateFile))
                {
                    throw new FileNotFoundException();
                }
                x509Certificate2 = DigitalCertificate.LoadCertificate(File.ReadAllBytes(certificateFile), password);
            }
            catch
            {
                throw;
            }
            return x509Certificate2;
        }

        /// <summary>
        /// Loads the digital certificate from Microsoft Store.
        /// </summary>
        /// <param name="validOnly">Use only valid certificates.</param>
        /// <param name="issuerName">Issuer Name. Use "" for any issuer.</param>
        /// <param name="windowTitle">Certificate selection window title.</param>
        /// <param name="windowDescription">Certificate selection window description.</param>
        /// <param name="certificateScope">To add a digital signature to a file ForDigitalSignature must be used.</param>
        /// <returns>Certificate as X509Certificate2.</returns>
        /// <exception cref="T:System.InvalidOperationException">Throws an exception if the certificate is invalid.</exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">Throws an exception if the certificate cannot be loaded.</exception>
        public static X509Certificate2 LoadCertificate(bool validOnly, string issuerName, string windowTitle, string windowDescription)
        {
            X509Certificate2 certificate;
            try
            {
                certificate = DigitalCertificate.GetCertificate(validOnly, issuerName, windowTitle, windowDescription, false);
            }
            catch
            {
                throw;
            }
            return certificate;
        }

        /// <summary>
        /// Loads the digital certificate from Microsoft Store.
        /// </summary>
        /// <param name="validOnly">Use only valid certificates.</param>
        /// <param name="issuerName">Issuer Name. Use "" for any issuer.</param>
        /// <param name="windowTitle">Certificate selection window title.</param>
        /// <param name="windowDescription">Certificate selection window description.</param>
        /// <param name="fromLocalMachine">Loads the certificates installed on Local Machine instead of Current User.</param>
        /// <returns>Certificate as X509Certificate2.</returns>
        /// <exception cref="T:System.InvalidOperationException">Throws an exception if the certificate is invalid.</exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">Throws an exception if the certificate cannot be loaded.</exception>
        public static X509Certificate2 LoadCertificate(bool validOnly, string issuerName, string windowTitle, string windowDescription, bool fromLocalMachine)
        {
            X509Certificate2 certificate;
            try
            {
                certificate = DigitalCertificate.GetCertificate(validOnly, issuerName, windowTitle, windowDescription, fromLocalMachine);
            }
            catch
            {
                throw;
            }
            return certificate;
        }

        /// <summary>
        /// Gets the first signing certificate from Microsoft Store without user intervention using the specified criteria.
        /// </summary>
        /// <param name="validOnly">Use only valid certificates.</param>
        /// <param name="selectionType">Various criteria like E=,CN=, thumbprint or serial number.</param>
        /// <param name="searchString">Criteria string.</param>
        /// <param name="certificateScope">To add a digital signature to a file ForDigitalSignature must be used.</param>
        /// <param name="fromLocalMachine">Loads the certificates installed on Local Machine instead of Current User.</param>
        /// <returns>Certificate as X509Certificate2.</returns>
        /// <exception cref="T:System.FormatException">Throws an exception if the criteria in incorrec.t</exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">Throws an exception if the certificate cannot be loaded.</exception>
        public static X509Certificate2 LoadCertificate(bool validOnly, DigitalCertificateSearchCriteria selectionType, string searchString, bool fromLocalMachine)
        {
            X509Certificate2 certificateWithCriteria;
            try
            {
                string str = null;
                if (selectionType == DigitalCertificateSearchCriteria.CommonNameCN)
                {
                    str = "CN=";
                }
                if (selectionType == DigitalCertificateSearchCriteria.CountryC)
                {
                    str = "C=";
                }
                if (selectionType == DigitalCertificateSearchCriteria.EmailE)
                {
                    str = "E=";
                }
                if (selectionType == DigitalCertificateSearchCriteria.LocalityL)
                {
                    str = "L=";
                }
                if (selectionType == DigitalCertificateSearchCriteria.OrganizationO)
                {
                    str = "O=";
                }
                if (selectionType == DigitalCertificateSearchCriteria.OrganizationUnitOU)
                {
                    str = "OU=";
                }
                if (selectionType == DigitalCertificateSearchCriteria.StateS)
                {
                    str = "S=";
                }
                if (selectionType == DigitalCertificateSearchCriteria.TitleT)
                {
                    str = "T=";
                }
                if (selectionType == DigitalCertificateSearchCriteria.Thumbprint)
                {
                    str = "Thumbprint";
                }
                if (selectionType == DigitalCertificateSearchCriteria.SerialNumber)
                {
                    str = "SerialNumber";
                }
                certificateWithCriteria = DigitalCertificate.GetCertificateWithCriteria(validOnly, searchString, str, fromLocalMachine);
            }
            catch
            {
                throw;
            }
            return certificateWithCriteria;
        }

        public static X509Certificate2 LoadCertificate(bool validOnly, DigitalCertificateSearchCriteria selectionType, string searchString)
        {
            X509Certificate2 x509Certificate2;
            try
            {
                x509Certificate2 = DigitalCertificate.LoadCertificate(validOnly, selectionType, searchString, false);
            }
            catch
            {
                throw;
            }
            return x509Certificate2;
        }

        internal static void LogOnEToken(X509Certificate2 cert)
        {
            if (!string.IsNullOrEmpty(DigitalCertificate.SmartCardPin) && !DigitalCertificate.useSmartCardPIN)
            {
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
                try
                {
                    try
                    {
                        if (cert.HasPrivateKey)
                        {
                            rSACryptoServiceProvider = cert.PrivateKey as RSACryptoServiceProvider;
                            if (rSACryptoServiceProvider.CspKeyContainerInfo.HardwareDevice)
                            {
                                SecureString secureString = new SecureString();
                                string smartCardPin = DigitalCertificate.SmartCardPin;
                                for (int i = 0; i < smartCardPin.Length; i++)
                                {
                                    secureString.AppendChar(smartCardPin[i]);
                                }
                                CspParameters cspParameter = new CspParameters(rSACryptoServiceProvider.CspKeyContainerInfo.ProviderType, rSACryptoServiceProvider.CspKeyContainerInfo.ProviderName, rSACryptoServiceProvider.CspKeyContainerInfo.KeyContainerName, new CryptoKeySecurity(), secureString);
                                rSACryptoServiceProvider = new RSACryptoServiceProvider(cspParameter);
                                DigitalCertificate.SignTestMessage(cert);
                                DigitalCertificate.useSmartCardPIN = true;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        throw new CryptographicException(string.Concat("Bypassing PIN Exception: ", exception.Message));
                    }
                }
                finally
                {
                    rSACryptoServiceProvider.Clear();
                }
            }
        }

        private static CertificateStatus OCSPValidation(Org.BouncyCastle.X509.X509Certificate certificate, Org.BouncyCastle.X509.X509Certificate issuerCert, Uri ocspUrl)
        {
            CertificateStatus certificateStatu;
            try
            {
                OcspReq ocspReq = DigitalCertificate.GenerateOcspRequest(issuerCert, certificate.SerialNumber);
                byte[] oCSPResponseFromURL = DigitalCertificate.GetOCSPResponseFromURL(ocspUrl, ocspReq.GetEncoded(), "application/ocsp-request", "application/ocsp-response");
                certificateStatu = DigitalCertificate.ProcessOcspResponse(certificate, issuerCert, oCSPResponseFromURL);
            }
            catch
            {
                certificateStatu = CertificateStatus.Valid;
            }
            return certificateStatu;
        }

        private static CertificateStatus ProcessOcspResponse(Org.BouncyCastle.X509.X509Certificate eeCert, Org.BouncyCastle.X509.X509Certificate issuerCert, byte[] binaryResp)
        {
            CertificateStatus certificateStatu;
            try
            {
                OcspResp ocspResp = new OcspResp(binaryResp);
                if (ocspResp.Status != 0)
                {
                    certificateStatu = CertificateStatus.Valid;
                }
                else
                {
                    BasicOcspResp responseObject = (BasicOcspResp)ocspResp.GetResponseObject();
                    if ((int)responseObject.Responses.Length == 1)
                    {
                        SingleResp responses = responseObject.Responses[0];
                        DigitalCertificate.ValidateCertificateId(issuerCert, eeCert, responses.GetCertID());
                        object certStatus = responses.GetCertStatus();
                        if (certStatus is CertificateStatus.Valid)
                        {
                            certificateStatu = CertificateStatus.Valid;
                            return certificateStatu;
                        }
                        else if (certStatus is RevokedStatus)
                        {
                            certificateStatu = CertificateStatus.Revoked;
                            return certificateStatu;
                        }
                        else if (certStatus is UnknownStatus)
                        {
                            certificateStatu = CertificateStatus.Unknown;
                            return certificateStatu;
                        }
                        return CertificateStatus.Valid;
                    }
                    certificateStatu = CertificateStatus.Valid;
                }
            }
            catch
            {
                certificateStatu = CertificateStatus.Valid;
            }
            return certificateStatu;
        }

        public void ResetSmartCardPin()
        {
            DigitalCertificate.SmartCardPin = null;
            DigitalCertificate.useSmartCardPIN = false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static byte[] SignData(byte[] dataToSign, X509Certificate2 certificate, Oid hashAlgorithm)
        {
            byte[] signature;
            try
            {
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
                rSACryptoServiceProvider = certificate.PrivateKey as RSACryptoServiceProvider;
                signature = rSACryptoServiceProvider.SignData(dataToSign, hashAlgorithm.Value);
            }
            catch
            {
                try
                {
                    SignedCms signedCm = new SignedCms(new ContentInfo(dataToSign), true);
                    CmsSigner cmsSigner = new CmsSigner(certificate)
                    {
                        IncludeOption = X509IncludeOption.None,
                        DigestAlgorithm = new Oid(hashAlgorithm)
                    };
                    signedCm.ComputeSignature(cmsSigner, false);
                    SignerInformationStore signerInfos = (new CmsSignedData(signedCm.Encode())).GetSignerInfos();
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
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    if (!exception.Message.Contains("Invalid provider type specified."))
                    {
                        throw new CryptographicException(exception.Message);
                    }
                    throw new CryptographicException(string.Concat("CNG certificates and keys are not yet supported: ", exception.Message));
                }
            }
            return signature;
        }

        private static void SignTestMessage(X509Certificate2 cert)
        {
            try
            {
                SignedCms signedCm = new SignedCms(new ContentInfo(new byte[] { 166, 100, 234, 184, 137, 4, 194, 172, 72, 67, 65, 14 }));
                CmsSigner cmsSigner = new CmsSigner(cert)
                {
                    IncludeOption = X509IncludeOption.EndCertOnly
                };
                signedCm.ComputeSignature(cmsSigner, false);
                signedCm.Encode();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        private static byte[] ToByteArray(Stream stream)
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

        private static void ValidateCertificateId(Org.BouncyCastle.X509.X509Certificate issuerCert, Org.BouncyCastle.X509.X509Certificate certificate, CertificateID certificateId)
        {
            try
            {
                CertificateID certificateID = new CertificateID("1.3.14.3.2.26", issuerCert, certificate.SerialNumber);
                if (!certificateID.SerialNumber.Equals(certificateId.SerialNumber))
                {
                    throw new Exception("Invalid certificate ID in response");
                }
                if (!Arrays.AreEqual(certificateID.GetIssuerNameHash(), certificateId.GetIssuerNameHash()))
                {
                    throw new Exception("Invalid certificate Issuer in response");
                }
            }
            catch
            {
                throw;
            }
        }

        private void ValidateNextUpdate(SingleResp resp)
        {
            try
            {
                if (resp.NextUpdate != null)
                {
                    DateTime value = resp.NextUpdate.Value;
                    if (resp.NextUpdate.Value.Ticks <= DateTime.Now.Ticks)
                    {
                        throw new Exception("Invalid next update.");
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private void ValidateResponse(BasicOcspResp or, Org.BouncyCastle.X509.X509Certificate issuerCert)
        {
            try
            {
                this.ValidateResponseSignature(or, issuerCert.GetPublicKey());
                this.ValidateSignerAuthorization(issuerCert, or.GetCerts()[0]);
            }
            catch
            {
                throw;
            }
        }

        private void ValidateResponseSignature(BasicOcspResp or, AsymmetricKeyParameter asymmetricKeyParameter)
        {
            try
            {
                if (!or.Verify(asymmetricKeyParameter))
                {
                    throw new Exception("Invalid OCSP signature");
                }
            }
            catch
            {
                throw;
            }
        }

        private void ValidateSignerAuthorization(Org.BouncyCastle.X509.X509Certificate issuerCert, Org.BouncyCastle.X509.X509Certificate signerCert)
        {
            try
            {
                if (!issuerCert.IssuerDN.Equivalent(signerCert.IssuerDN) || !issuerCert.SerialNumber.Equals(signerCert.SerialNumber))
                {
                    throw new Exception("Invalid OCSP signer");
                }
            }
            catch
            {
                throw;
            }
        }

        private void ValidateThisUpdate(SingleResp resp)
        {
            try
            {
                if (Math.Abs(resp.ThisUpdate.Ticks - DateTime.Now.Ticks) > (long)36000000)
                {
                    throw new Exception("Max clock skew reached.");
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Verifies the certificate by local time, OCSP and CRL.
        /// </summary>
        /// <param name="certificate">X509Certificate2 certificate.</param>
        /// <returns>The certificate status.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">Returns an exception if an error occured.</exception>
        public static CertificateStatus VerifyDigitalCertificate(X509Certificate2 certificate, VerificationType verificationType)
        {
            CertificateStatus certificateStatu;
            try
            {
                if (verificationType != VerificationType.LocalTime)
                {
                    X509CertificateParser x509CertificateParser = new X509CertificateParser();
                    if (verificationType == VerificationType.OCSP)
                    {
                        try
                        {
                            Uri authorityInformationAccessOcspUrl = DigitalCertificate.GetAuthorityInformationAccessOcspUrl(x509CertificateParser.ReadCertificate(certificate.RawData));
                            if (authorityInformationAccessOcspUrl != null)
                            {
                                Org.BouncyCastle.X509.X509Certificate rootCertificate = DigitalCertificate.GetRootCertificate(certificate);
                                if (rootCertificate != null)
                                {
                                    certificateStatu = (rootCertificate == null ? CertificateStatus.Unknown : DigitalCertificate.OCSPValidation(x509CertificateParser.ReadCertificate(certificate.RawData), rootCertificate, authorityInformationAccessOcspUrl));
                                }
                                else
                                {
                                    certificateStatu = CertificateStatus.Valid;
                                }
                            }
                            else
                            {
                                certificateStatu = CertificateStatus.Valid;
                            }
                        }
                        catch
                        {
                            certificateStatu = CertificateStatus.Valid;
                        }
                    }
                    else if (verificationType != VerificationType.CRL)
                    {
                        if (verificationType != VerificationType.LDAP)
                        {
                            throw new CryptographicException("Certificate validation error");
                        }
                        try
                        {
                            string certCRL = DigitalCertificate.GetCertCRL(certificate, true);
                            certificateStatu = (certCRL != null ? DigitalCertificate.IsRevokedLdap(certCRL, x509CertificateParser.ReadCertificate(certificate.RawData)) : CertificateStatus.Valid);
                        }
                        catch
                        {
                            certificateStatu = CertificateStatus.Valid;
                        }
                    }
                    else
                    {
                        try
                        {
                            string str = DigitalCertificate.GetCertCRL(certificate, false);
                            certificateStatu = (str != null ? DigitalCertificate.IsRevoked(str, x509CertificateParser.ReadCertificate(certificate.RawData)) : CertificateStatus.Valid);
                        }
                        catch
                        {
                            certificateStatu = CertificateStatus.Valid;
                        }
                    }
                }
                else if (DateTime.Now <= certificate.NotAfter)
                {
                    certificateStatu = (DateTime.Now >= certificate.NotBefore ? CertificateStatus.Expired : CertificateStatus.Valid);
                }
                else
                {
                    certificateStatu = CertificateStatus.Valid;
                }
            }
            catch (Exception exception)
            {
                throw new CryptographicException(string.Concat("Certificate validation error: ", exception.Message));
            }
            return certificateStatu;
        }
    }
}
