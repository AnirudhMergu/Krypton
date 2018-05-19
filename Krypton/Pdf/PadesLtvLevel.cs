using System;

namespace Krypton.Pdf
{
    public enum PadesLtvLevel
    {
        /// <summary>
        /// The signature will not contain any revocation data.
        /// </summary>
        None,
        /// <summary>
        /// The signature will contain the CRL of the signing certificate, if available.
        /// </summary>
        IncludeCrl,
        /// <summary>
        /// The signature will contain the CRL of the signing certificate and the OCSP, if available. 
        /// </summary>
        IncludeCrlAndOcsp
    }
}