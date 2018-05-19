using System;

namespace Krypton.Pdf
{
    /// <summary>
    /// Image can be added on the signature rectangle on different ways.
    /// </summary>
    public enum SignatureImageType
    {
        /// <summary>
        /// Image will displayed on the background and the text will cover the image.
        /// </summary>
        ImageAsBackground,
        /// <summary>
        /// Only the image will be displayed on the signature rectangle.
        /// </summary>
        ImageWithNoText,
        /// <summary>
        /// Image will be displayed on the left and the signature text on the right.
        /// </summary>
        ImageAndText
    }
}