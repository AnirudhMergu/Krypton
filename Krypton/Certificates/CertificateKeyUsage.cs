using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krypton.Certificates
{
    /// <summary>
    /// Certificate Key Usage extensions.
    /// </summary>
    public enum CertificateKeyUsage
    {
        /// <summary>
        /// The certificate public key may be used only for enciphering data while performing key agreement, if the keyAgreement bit also is set.
        /// </summary>
        EncipherOnly = 1,
        /// <summary>
        /// The certificate use the public key for verifying a signature on CRLs.
        /// </summary>
        CRLSigning = 2,
        /// <summary>
        /// The certificate use the public key for verifying a signature on certificates.
        /// </summary>
        CertificateSigning = 4,
        /// <summary>
        /// The certificate use the public key for key agreement.
        /// </summary>
        KeyAgreement = 8,
        /// <summary>
        /// The certificate use the public key for enciphering user data, other than cryptographic keys.
        /// </summary>
        DataEncipherment = 16,
        /// <summary>
        /// The certificate use the public key for key transport.
        /// </summary>
        KeyEncipherment = 32,
        /// <summary>
        /// The certificate use the public key for verifying digital signatures used to provide a non-repudiation service which protects against the signing entity falsely denying some action, excluding certificate or CRL signing.
        /// </summary>
        NonRepudiation = 64,
        /// <summary>
        /// The certificate use the public key for verifying digital signatures that have purposes other than non-repudiation, certificate signature, and CRL signature.
        /// </summary>
        DigitalSignature = 128,
        /// <summary>
        /// The certificate public key may be used only for enciphering data while performing key agreement, if the keyAgreement bit also is set.
        /// </summary>
        DecipherOnly = 32768
    }
}
