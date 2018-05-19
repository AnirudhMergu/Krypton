using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krypton.Certificates
{
    /// <summary>
    /// Signature hash algorithm used for signing the certificate.
    /// </summary>
    public enum SignatureAlgorithm
    {
        /// <summary>
        /// Recommended for user certificates.
        /// </summary>
        SHA1WithRSA,
        /// <summary>
        /// Advanced hash algorithm.
        /// </summary>
        SHA256WithRSA,
        /// <summary>
        /// Advanced hash algorithm.
        /// </summary>
        SHA512WithRSA,
        /// <summary>
        /// Old hash algorithm.
        /// </summary>
        MD5WithRSA
    }
}
