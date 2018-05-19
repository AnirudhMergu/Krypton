using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Krypton.Certificates
{
    /// <summary>
    /// Class for the extensions added to a certificate.
    /// </summary>
    public class Extensions
    {
        internal List<ExtensionInfo> extensionInfo = new List<ExtensionInfo>();

        /// <summary>
        /// Enhanced key usage extensions are marked as critical. The default value is false.
        /// </summary>
        public bool EnhancedKeyUsageIsCritical
        {
            get;
            set;
        }

        /// <summary>
        /// Key usage extensions are marked as critical. The default value is false.
        /// </summary>
        public bool KeyUsageIsCritical
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes Extensions class.
        /// </summary>
        public Extensions()
        {
            this.KeyUsageIsCritical = false;
            this.EnhancedKeyUsageIsCritical = false;
        }

        /// <summary>
        /// Add an enhanced key usage extension to the certificate.
        /// </summary>
        /// <param name="enhancedKeyUsage">Enhanced Key Usage value.</param>
        public void AddEnhancedKeyUsage(CertificateEnhancedKeyUsage enhancedKeyUsage)
        {
            try
            {
                if (enhancedKeyUsage == CertificateEnhancedKeyUsage.DocumentSigning)
                {
                    this.extensionInfo.Add(new ExtensionInfo(new DerObjectIdentifier("1.3.6.1.4.1.311.10.3.12"), this.EnhancedKeyUsageIsCritical, true));
                }
                else if (enhancedKeyUsage == CertificateEnhancedKeyUsage.NetscapeServerGatedCrypto)
                {
                    this.extensionInfo.Add(new ExtensionInfo(new DerObjectIdentifier("2.16.840.1.113730.4.1"), this.EnhancedKeyUsageIsCritical, true));
                }
                else if (enhancedKeyUsage != CertificateEnhancedKeyUsage.MicrosoftServerGatedCrypto)
                {
                    object anyExtendedKeyUsage = null;
                    if (enhancedKeyUsage == CertificateEnhancedKeyUsage.AnyPurpose)
                    {
                        anyExtendedKeyUsage = KeyPurposeID.AnyExtendedKeyUsage;
                    }
                    if (enhancedKeyUsage == CertificateEnhancedKeyUsage.ClientAuthentication)
                    {
                        anyExtendedKeyUsage = KeyPurposeID.IdKPClientAuth;
                    }
                    if (enhancedKeyUsage == CertificateEnhancedKeyUsage.CodeSigning)
                    {
                        anyExtendedKeyUsage = KeyPurposeID.IdKPCodeSigning;
                    }
                    if (enhancedKeyUsage == CertificateEnhancedKeyUsage.SecureEmail)
                    {
                        anyExtendedKeyUsage = KeyPurposeID.IdKPEmailProtection;
                    }
                    if (enhancedKeyUsage == CertificateEnhancedKeyUsage.IpsecEndSystem)
                    {
                        anyExtendedKeyUsage = KeyPurposeID.IdKPIpsecEndSystem;
                    }
                    if (enhancedKeyUsage == CertificateEnhancedKeyUsage.IpsecTunnel)
                    {
                        anyExtendedKeyUsage = KeyPurposeID.IdKPIpsecTunnel;
                    }
                    if (enhancedKeyUsage == CertificateEnhancedKeyUsage.IpsecUser)
                    {
                        anyExtendedKeyUsage = KeyPurposeID.IdKPIpsecUser;
                    }
                    if (enhancedKeyUsage == CertificateEnhancedKeyUsage.OcspSigning)
                    {
                        anyExtendedKeyUsage = KeyPurposeID.IdKPOcspSigning;
                    }
                    if (enhancedKeyUsage == CertificateEnhancedKeyUsage.ServerAuthentication)
                    {
                        anyExtendedKeyUsage = KeyPurposeID.IdKPServerAuth;
                    }
                    if (enhancedKeyUsage == CertificateEnhancedKeyUsage.SmartcardLogon)
                    {
                        anyExtendedKeyUsage = KeyPurposeID.IdKPSmartCardLogon;
                    }
                    if (enhancedKeyUsage == CertificateEnhancedKeyUsage.TimeStamping)
                    {
                        anyExtendedKeyUsage = KeyPurposeID.IdKPTimeStamping;
                    }
                    this.extensionInfo.Add(new ExtensionInfo(anyExtendedKeyUsage, this.EnhancedKeyUsageIsCritical, true));
                }
                else
                {
                    this.extensionInfo.Add(new ExtensionInfo(new DerObjectIdentifier("1.3.6.1.4.1.311.10.3.3"), this.EnhancedKeyUsageIsCritical, true));
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Add an extended key usage extension as dotted OID to the certificate (e.g. 1.3.6.1.4.1.311.10.3.12)
        /// </summary>
        /// <param name="extensionOID">The OID of the Extended Key Usage.</param>
        public void AddEnhancedKeyUsage(Oid extensionOID)
        {
            try
            {
                this.extensionInfo.Add(new ExtensionInfo(new DerObjectIdentifier(extensionOID.Value), this.EnhancedKeyUsageIsCritical, true));
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Add an key usage extension the the certificate.
        /// </summary>
        /// <param name="keyUsage">Key Usage value.</param>
        public void AddKeyUsage(CertificateKeyUsage keyUsage)
        {
            try
            {
                this.extensionInfo.Add(new ExtensionInfo((object)keyUsage, this.KeyUsageIsCritical, false));
            }
            catch
            {
                throw;
            }
        }
    }
}
