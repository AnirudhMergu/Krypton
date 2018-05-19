using System;

namespace Krypton.Pdf
{
    /// <summary>
    /// Signature location for basic signature.
    /// </summary>
    public enum SignaturePosition
    {
        /// <summary>
        /// The signature rectangle will be placed on top right.
        /// </summary>
        TopRight,
        /// <summary>
        /// The signature rectangle will be placed on top middle.
        /// </summary>
        TopMiddle,
        /// <summary>
        /// The signature rectangle will be placed on top left.
        /// </summary>
        TopLeft,
        /// <summary>
        /// The signature rectangle will be placed on bottom right.
        /// </summary>
        BottomRight,
        /// <summary>
        /// The signature rectangle will be placed on bottom middle.
        /// </summary>
        BottomMiddle,
        /// <summary>
        /// The signature rectangle will be placed on bottom left.
        /// </summary>
        BottomLeft
    }
}