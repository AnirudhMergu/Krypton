using System;

namespace Krypton.Pdf
{
    /// <summary>
    /// Certify method.
    /// </summary>W
    public enum CertifyMethod
    {
        /// <summary>
        /// No changes are permitted.
        /// </summary>
        NoChangesAllowed,
        /// <summary>
        /// Form filling is perimtted.
        /// </summary>
        FormFilling,
        /// <summary>
        /// Adding annotations and form filling are permitted.
        /// </summary>
        AnnotationsAndFormFilling
    }
}