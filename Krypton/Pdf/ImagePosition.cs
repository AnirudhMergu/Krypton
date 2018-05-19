using System;

namespace Krypton.Pdf
{
    /// <summary>
    /// The image position. This could be over the content or under the content (as background).
    /// </summary>
    public enum ImagePosition
    {
        /// <summary>
        /// The image will be inserted under the document content.
        /// </summary>
        ImageUnderContent,
        /// <summary>
        /// The image will be inserted over the document content.
        /// </summary>
        ImageOverContent
    }
}