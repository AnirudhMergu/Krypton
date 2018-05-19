extern alias itext;
using System.Drawing;


namespace Krypton.Pdf
{
    /// <summary>
    /// Custom text class.
    /// </summary>
    public class CustomText
    {
        /// <summary>
        /// Sets the text align.
        /// </summary>
        public TextAlign Align
        {
            get;
            set;
        }

        /// <summary>
        /// Set the font file.
        /// </summary>
        public string FontFile
        {
            get;
            set;
        }

        /// <summary>
        /// Set the font size.
        /// </summary>
        public int FontSize
        {
            get;
            set;
        }

        /// <summary>
        /// Set the page number wher the text will be inserted.
        /// </summary>
        public int PageNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text starting position as System.Drawing.Point.
        /// </summary>
        public System.Drawing.Point StartingPointPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the text.
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        public itext.iTextSharp.text.BaseColor TextColor
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the signature text direction. The default value is Normal.
        /// </summary>
        public TextDirection TextDirection
        {
            get;
            set;
        }

        /// <summary>
        /// CustomText constructor.
        /// </summary>
        public CustomText()
        {
            this.TextDirection = TextDirection.Normal;
            this.FontFile = null;
        }
    }
}