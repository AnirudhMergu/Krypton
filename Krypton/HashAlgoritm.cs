using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krypton
{
    /// <summary>
    /// Hash algorithms used for signatures.
    /// </summary>
    public enum HashAlgorithm
    {
        /// <summary>
        /// SHA-1 hash algorithm (normal).
        /// </summary>
        SHA1,
        /// <summary>
        /// SHA-256 hash algorithm (strong).
        /// </summary>
        SHA256,
        /// <summary>
        /// SHA-384 hash algorithm (strong).
        /// </summary>
        SHA384,
        /// <summary>
        /// SHA-512 hash algorithm (very strong).
        /// </summary>
        SHA512
    }
}
