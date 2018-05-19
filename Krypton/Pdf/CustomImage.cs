extern alias itext;

using itext::iTextSharp.awt.geom;
using itext::iTextSharp.text;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Krypton.Pdf
{
    /// <summary>
    /// Custom image class.
    /// </summary>
    public class CustomImage
    {
        /// <summary>
        /// The image will cover all the document page.
        /// </summary>
        public bool AddImageAsWatermark
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the byte array of the image.
        /// </summary>
        public byte[] Image
        {
            get;
            set;
        }

        /// <summary>
        /// The image position. This could be over the content or under the content (as background).
        /// </summary>
        public ImagePosition ImagePosition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the page where the image will be inserted. Use 0 to insert image on all pages.
        /// </summary>
        public int PageNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the image signature location as System.Drawing.Rectangle.
        /// </summary>
        public System.Drawing.Rectangle RectangePosition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the image signature starting position as System.Drawing.Point.
        /// </summary>
        public System.Drawing.Point StartingPointPosition
        {
            get;
            set;
        }

        /// <summary>
        /// CustomImage constructor.
        /// </summary>
        public CustomImage()
        {
        }
    }
}