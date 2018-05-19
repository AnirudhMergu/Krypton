using System;

namespace Krypton.Pdf
{
    /// <summary>
    /// Encryption Algorithm.
    /// </summary>
    public enum PdfEncryptionAlgorithm
    {
        /// <summary>
        /// 40 bit RC4 encryption - weak.
        /// </summary>
        StandardEncryption40BitRC4,
        /// <summary>
        /// 128 bit RC4 encryption - medium.
        /// </summary>
        StandardEncryption128BitRC4,
        /// <summary>
        /// 128 bit AES encryption - high.
        /// </summary>
        EnhancedEncryption128BitAES
    }
}