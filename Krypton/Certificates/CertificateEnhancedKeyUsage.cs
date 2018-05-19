using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krypton.Certificates
{
    /// <summary>
    /// Certificate Extended Key Usage extensions.
    /// </summary>
    public enum CertificateEnhancedKeyUsage
    {
        /// <summary>
        /// The certificate will have any extended key usage.
        /// </summary>
        AnyPurpose,
        /// <summary>
        /// The certificate can be used for authenticating a client.
        /// </summary>
        ClientAuthentication,
        /// <summary>
        /// The certificate can be used for signing code.
        /// </summary>
        CodeSigning,
        /// <summary>
        /// The certificate can be used to encrypt email messages.
        /// </summary>
        SecureEmail,
        /// <summary>
        /// The certificate can be used for signing end-to-end Internet Protocol Security (IPSEC) communication.
        /// </summary>
        IpsecEndSystem,
        /// <summary>
        /// The certificate can be used for singing IPSEC communication in tunnel mode.
        /// </summary>
        IpsecTunnel,
        /// <summary>
        /// The certificate can be used for an IPSEC user.
        /// </summary>
        IpsecUser,
        /// <summary>
        /// The certificate can be used for Online Certificate Status Protocol (OCSP) signing.
        /// </summary>
        OcspSigning,
        /// <summary>
        /// The certificate can be used as an SSL server certificate. 
        /// </summary>
        ServerAuthentication,
        /// <summary>
        /// The certificate enables an individual to log on to a computer by using a smart card.
        /// </summary>
        SmartcardLogon,
        /// <summary>
        /// The certificate can be used for signing public key infrastructure timestamps according to RFC 3161.
        /// </summary>
        TimeStamping,
        /// <summary>
        /// The certificate can be used for signing documents.
        /// </summary>
        DocumentSigning,
        /// <summary>
        /// Netscape Server Gated Crypto (nsSGC). An encryption step up for old Browsers so users get strong crypto.
        /// </summary>
        NetscapeServerGatedCrypto,
        /// <summary>
        /// Microsoft Server Gated Crypto (msSGC). An encryption step up for old Browsers so users get strong crypto.
        /// </summary>
        MicrosoftServerGatedCrypto
    }
}
