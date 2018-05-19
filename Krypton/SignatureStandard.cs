using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krypton
{
    /// <summary>
    /// The signature standard used for generating digital signatures (PKCS#7 or CAdES).
    /// </summary>
    public enum SignatureStandard
    {
        /// <summary>
        /// PKCS#7 format. This type of signature can be interpreted by the older software.
        /// </summary>
        Pkcs7,
        /// <summary>
        /// PAdES/CAdES. This type of signature uses a newer standard. Pdf signature requires Adobe Reader X to be validated.
        /// </summary>
        Cades
    }
}
