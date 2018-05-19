extern alias itext;

using itext.iTextSharp.text;
using itext.iTextSharp.text.pdf;
using Krypton;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace Krypton.Pdf
{
    /// <summary>
    /// Class for adding custom images, watermarks and texts to a PDF document.
    /// </summary>
    public class PdfInsertObject
    {
        private PdfReader reader;

        private List<CustomImage> customImageList = new List<CustomImage>();

        private List<CustomText> customTextList = new List<CustomText>();

        /// <summary>
        /// PDF document information (number of pages, number of digital signatures).
        /// </summary>
        public PdfDocumentProperties DocumentProperties
        {
            get;
            set;
        }

        /// <summary>
        /// PdfInsertObject constructor.
        /// </summary>
        public PdfInsertObject()
        {
            this.DocumentProperties = new PdfDocumentProperties();
        }

        /// <summary>
        /// Add a custom image to the PDF document.
        /// </summary>
        /// <param name="image">The byte array of the image.</param>
        /// <param name="imageLocation">The image signature location as System.Drawing.Rectangle.</param>
        /// <param name="pageNumber">The page where the image will be inserted. Use 0 to insert image on all pages.</param>
        /// <param name="imagePostition">The image position. This could be over the content or under the content (as background).</param>
        public void AddImage(byte[] image, System.Drawing.Rectangle imageLocation, int pageNumber, ImagePosition imagePostition)
        {
            try
            {
                if (image == null)
                {
                    throw new ArgumentNullException(CustomExceptions.CustomImageIsNull);
                }
                if (pageNumber < 0 || pageNumber > this.DocumentProperties.NumberOfPages)
                {
                    throw new ArgumentOutOfRangeException(CustomExceptions.InvalidPageNumber);
                }
                CustomImage customImage = new CustomImage()
                {
                    Image = image,
                    RectangePosition = imageLocation,
                    PageNumber = pageNumber,
                    AddImageAsWatermark = false,
                    ImagePosition = imagePostition
                };
                this.customImageList.Add(customImage);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Add a custom image to the PDF document.
        /// </summary>
        /// <param name="image">The byte array of the image.</param>
        /// <param name="imageLocation">The image signature starting position as System.Drawing.Point.</param>
        /// <param name="pageNumber">The page where the image will be inserted. Use 0 to insert image on all pages.</param>
        /// <param name="imagePostition">The image position. This could be over the content or under the content (as background).</param>
        public void AddImage(byte[] image, Point imageLocation, int pageNumber, ImagePosition imagePostition)
        {
            try
            {
                if (image == null)
                {
                    throw new ArgumentNullException(CustomExceptions.CustomImageIsNull);
                }
                if (pageNumber < 0 || pageNumber > this.DocumentProperties.NumberOfPages)
                {
                    throw new ArgumentOutOfRangeException(CustomExceptions.InvalidPageNumber);
                }
                CustomImage customImage = new CustomImage()
                {
                    Image = image,
                    StartingPointPosition = imageLocation,
                    PageNumber = pageNumber,
                    AddImageAsWatermark = false,
                    ImagePosition = imagePostition
                };
                this.customImageList.Add(customImage);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Add a custom image to the PDF document that will cover all the document page.
        /// </summary>
        /// <param name="image">The byte array of the image.</param>
        /// <param name="pageNumber">The page where the image will be inserted. Use 0 to insert image on all pages.</param>
        /// <param name="imagePostition">The image position. This could be over the content or under the content (as background).</param>
        public void AddImage(byte[] image, int pageNumber, ImagePosition imagePostition)
        {
            try
            {
                if (image == null)
                {
                    throw new ArgumentNullException(CustomExceptions.CustomImageIsNull);
                }
                if (pageNumber < 0 || pageNumber > this.DocumentProperties.NumberOfPages)
                {
                    throw new ArgumentOutOfRangeException(CustomExceptions.InvalidPageNumber);
                }
                CustomImage customImage = new CustomImage()
                {
                    Image = image,
                    PageNumber = pageNumber,
                    AddImageAsWatermark = true,
                    ImagePosition = imagePostition
                };
                this.customImageList.Add(customImage);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Insert a custom text on the PDF document.
        /// </summary>
        /// <param name="customText">The CostomText object</param>
        public void AddText(CustomText customText)
        {
            try
            {
                if (customText.Text == null)
                {
                    throw new ArgumentNullException();
                }
                if (customText.PageNumber < 0 || customText.PageNumber > this.DocumentProperties.NumberOfPages)
                {
                    throw new ArgumentOutOfRangeException(CustomExceptions.InvalidPageNumber);
                }
                this.customTextList.Add(customText);
            }
            catch
            {
                throw;
            }
        }

        private byte[] DownloadBytesFromURL(Uri url)
        {
            byte[] numArray;
            try
            {
                WebRequest webRequest = WebRequest.Create(url);
                webRequest.Timeout = 15000;
                Stream responseStream = webRequest.GetResponse().GetResponseStream();
                byte[] byteArray = this.ToByteArray(responseStream);
                responseStream.Close();
                numArray = byteArray;
            }
            catch
            {
                numArray = null;
            }
            return numArray;
        }

        /// <summary>
        /// Adds the images and texts to the PDF document.
        /// </summary>
        /// <returns>Returns the PDF document with the inserted images and texts.</returns>
        public byte[] InsertObjects()
        {
            itext.iTextSharp.text.Rectangle pageSizeWithRotation;
            BaseFont baseFont;
            byte[] array;
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                try
                {
                    if (this.reader == null)
                    {
                        throw new NullReferenceException(CustomExceptions.DocumentIsNotLoaded);
                    }
                    PdfStamper pdfStamper = new PdfStamper(this.reader, memoryStream);
                    if (this.customImageList.Count > 0)
                    {
                        foreach (CustomImage customImage in this.customImageList)
                        {
                            PdfContentByte underContent = null;
                            itext.iTextSharp.text.Image instance = itext.iTextSharp.text.Image.GetInstance(customImage.Image);
                            if (customImage.PageNumber != 0)
                            {
                                pageSizeWithRotation = this.reader.GetPageSizeWithRotation(customImage.PageNumber);
                                if (customImage.AddImageAsWatermark)
                                {
                                    instance.ScaleAbsolute(pageSizeWithRotation.Width, pageSizeWithRotation.Height);
                                    instance.SetAbsolutePosition(0f, 0f);
                                }
                                if (customImage.RectangePosition != System.Drawing.Rectangle.Empty)
                                {
                                    instance.ScaleAbsolute((float)customImage.RectangePosition.Width, (float)customImage.RectangePosition.Height);
                                    instance.SetAbsolutePosition((float)customImage.RectangePosition.X, (float)customImage.RectangePosition.Y);
                                }
                                if (customImage.StartingPointPosition != Point.Empty)
                                {
                                    instance.SetAbsolutePosition((float)customImage.StartingPointPosition.X, (float)customImage.StartingPointPosition.Y);
                                }
                                if (customImage.ImagePosition == ImagePosition.ImageUnderContent)
                                {
                                    underContent = pdfStamper.GetUnderContent(customImage.PageNumber);
                                }
                                if (customImage.ImagePosition == ImagePosition.ImageOverContent)
                                {
                                    underContent = pdfStamper.GetOverContent(customImage.PageNumber);
                                }
                                underContent.AddImage(instance);
                            }
                            else
                            {
                                for (int i = 1; i <= this.reader.NumberOfPages; i++)
                                {
                                    pageSizeWithRotation = this.reader.GetPageSizeWithRotation(i);
                                    if (customImage.AddImageAsWatermark)
                                    {
                                        instance.ScaleAbsolute(pageSizeWithRotation.Width, pageSizeWithRotation.Height);
                                        instance.SetAbsolutePosition(0f, 0f);
                                    }
                                    if (customImage.RectangePosition != System.Drawing.Rectangle.Empty)
                                    {
                                        instance.ScaleAbsolute((float)customImage.RectangePosition.Width, (float)customImage.RectangePosition.Height);
                                        instance.SetAbsolutePosition((float)customImage.RectangePosition.X, (float)customImage.RectangePosition.Y);
                                    }
                                    if (customImage.StartingPointPosition != Point.Empty)
                                    {
                                        instance.SetAbsolutePosition((float)customImage.StartingPointPosition.X, (float)customImage.StartingPointPosition.Y);
                                    }
                                    if (customImage.ImagePosition == ImagePosition.ImageUnderContent)
                                    {
                                        underContent = pdfStamper.GetUnderContent(i);
                                    }
                                    if (customImage.ImagePosition == ImagePosition.ImageOverContent)
                                    {
                                        underContent = pdfStamper.GetOverContent(i);
                                    }
                                    underContent.AddImage(instance);
                                }
                            }
                        }
                    }
                    if (this.customTextList.Count > 0)
                    {
                        foreach (CustomText customText in this.customTextList)
                        {
                            PdfContentByte overContent = null;
                            overContent = pdfStamper.GetOverContent(customText.PageNumber);
                            baseFont = (string.IsNullOrEmpty(customText.FontFile) ? BaseFont.CreateFont("Helvetica-Bold", "Cp1250", true) : BaseFont.CreateFont(customText.FontFile, "Identity-H", true));
                            overContent.BeginText();
                            overContent.SetFontAndSize(baseFont, (float)customText.FontSize);
                            if (customText.TextDirection == TextDirection.RightToLeft)
                            {
                                customText.Text = this.ReverseString(customText.Text);
                            }
                            if (customText.Align == TextAlign.Center)
                            {
                                string text = customText.Text;
                                float x = (float)customText.StartingPointPosition.X;
                                Point startingPointPosition = customText.StartingPointPosition;
                                overContent.ShowTextAligned(1, text, x, (float)startingPointPosition.Y, 0f);
                            }
                            if (customText.Align == TextAlign.Left)
                            {
                                string str = customText.Text;
                                float single = (float)customText.StartingPointPosition.X;
                                Point point = customText.StartingPointPosition;
                                overContent.ShowTextAligned(0, str, single, (float)point.Y, 0f);
                            }
                            if (customText.Align == TextAlign.Right)
                            {
                                string text1 = customText.Text;
                                float x1 = (float)customText.StartingPointPosition.X;
                                Point startingPointPosition1 = customText.StartingPointPosition;
                                overContent.ShowTextAligned(2, text1, x1, (float)startingPointPosition1.Y, 0f);
                            }
                            if (customText.TextColor != null)
                            {
                                overContent.SetColorFill(customText.TextColor);
                            }
                            overContent.EndText();
                        }
                    }
                    pdfStamper.Close();
                    array = memoryStream.ToArray();
                }
                catch
                {
                    throw;
                }
            }
            finally
            {
                memoryStream.Close();
            }
            return array;
        }

        /// <summary>
        /// Loads the PDF document from a byte array.
        /// </summary>
        /// <param name="pdfArray">Document byte array.</param>
        /// <exception cref="T:System.Exception">Throws an exception if an error occured.</exception>
        public void LoadPdfDocument(byte[] pdfArray)
        {
            try
            {
                if (this.DocumentProperties.Password != null)
                {
                    ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
                    this.reader = new PdfReader(pdfArray, aSCIIEncoding.GetBytes(this.DocumentProperties.Password));
                }
                else
                {
                    this.reader = new PdfReader(pdfArray);
                }
                this.DocumentProperties.LoadPdfDocumentProperties(this.reader);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Loads the PDF document from a file.
        /// </summary>
        /// <param name="PdfFile">Full path to the PDF document.</param>
        /// <exception cref="T:System.Exception">Throws an exception if an error occured.</exception>
        public void LoadPdfDocument(string PdfFile)
        {
            try
            {
                if (!File.Exists(PdfFile))
                {
                    throw new FileNotFoundException();
                }
                if (this.DocumentProperties.Password != null)
                {
                    ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
                    this.reader = new PdfReader(PdfFile, aSCIIEncoding.GetBytes(this.DocumentProperties.Password));
                }
                else
                {
                    this.reader = new PdfReader(PdfFile);
                }
                this.DocumentProperties.LoadPdfDocumentProperties(this.reader);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Loads the PDF document from an URL.
        /// </summary>
        /// <param name="PdfUrl">URL to the PDF file.</param>
        /// <exception cref="T:System.Exception">Throws an exception if an error occured.</exception>
        public void LoadPdfDocument(Uri PdfUrl)
        {
            try
            {
                if (this.DocumentProperties.Password != null)
                {
                    ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
                    this.reader = new PdfReader(this.DownloadBytesFromURL(PdfUrl), aSCIIEncoding.GetBytes(this.DocumentProperties.Password));
                }
                else
                {
                    this.reader = new PdfReader(this.DownloadBytesFromURL(PdfUrl));
                }
                this.DocumentProperties.LoadPdfDocumentProperties(this.reader);
            }
            catch
            {
                throw;
            }
        }

        private string ReverseString(string textToReverse)
        {
            string str;
            try
            {
                char[] charArray = textToReverse.ToCharArray();
                Array.Reverse(charArray);
                str = new string(charArray);
            }
            catch
            {
                str = textToReverse;
            }
            return str;
        }

        private byte[] ToByteArray(Stream stream)
        {
            byte[] array;
            try
            {
                byte[] numArray = new byte[4096];
                MemoryStream memoryStream = new MemoryStream();
                int num = 0;
                while (true)
                {
                    int num1 = stream.Read(numArray, 0, (int)numArray.Length);
                    num = num1;
                    if (num1 <= 0)
                    {
                        break;
                    }
                    memoryStream.Write(numArray, 0, num);
                }
                array = memoryStream.ToArray();
            }
            catch
            {
                array = null;
            }
            return array;
        }
    }
}