using System;

namespace Krypton.Pdf
{
    /// <summary>
    /// Document Restrictions
    /// </summary>
    public enum PdfDocumentRestrictions
    {
        /// <summary>
        /// No actions are permitted.
        /// </summary>
        AllowNone = 0,
        /// <summary>
        /// Allows content copying.
        /// </summary>
        AllowContentCopying = 16,
        /// <summary>
        /// Allows content copying for accessibility.
        /// </summary>
        AllowContentCopyingForAccessibility = 512,
        /// <summary>
        /// Allows printing.
        /// </summary>
        AllowPrinting = 2052
    }
}