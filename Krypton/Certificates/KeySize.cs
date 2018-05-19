using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krypton.Certificates
{
    /// <summary>
    /// Key pair length.
    /// </summary>
    public enum KeySize
    {
        /// <summary>
        /// 512 key kength.
        /// </summary>
        KeySize512Bit = 512,
        /// <summary>
        /// Recommended for user certificates.
        /// </summary>
        KeySize1024Bit = 1024,
        /// <summary>
        /// Recommended for CA certificates.
        /// </summary>
        KeySize2048Bit = 2048,
        /// <summary>
        /// 4096 key length.
        /// </summary>
        KeySize4096Bit = 4096
    }
}
