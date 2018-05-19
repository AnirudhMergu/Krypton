extern alias itext;
using itext.iTextSharp.text;
using itext.iTextSharp.text.pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Krypton.Pdf
{
    /// <summary>
    /// Class for merging multiple PDF files.
    /// </summary>
    public class PdfMerge
    {
        public PdfMerge()
        {
        }

        /// <summary>
        /// Merging multiple PDF files in a single one. 
        /// </summary>
        /// <param name="sourceFiles">List of the source files as byte[]. The first file from the List is the first part form the merged file.</param>
        /// <returns>Returns the merged file as byte[].</returns>
        public static byte[] MergePdfFiles(List<byte[]> sourceFiles)
        {
            byte[] array;
            try
            {
                int num = 0;
                PdfReader pdfReader = new PdfReader(sourceFiles[num]);
                int numberOfPages = pdfReader.NumberOfPages;
                Document document = new Document(pdfReader.GetPageSizeWithRotation(1));
                MemoryStream memoryStream = new MemoryStream();
                PdfWriter instance = PdfWriter.GetInstance(document, memoryStream);
                document.Open();
                PdfContentByte directContent = instance.DirectContent;
                while (num < sourceFiles.Count)
                {
                    int num1 = 0;
                    while (num1 < numberOfPages)
                    {
                        num1++;
                        document.SetPageSize(pdfReader.GetPageSizeWithRotation(num1));
                        document.NewPage();
                        PdfImportedPage importedPage = instance.GetImportedPage(pdfReader, num1);
                        int pageRotation = pdfReader.GetPageRotation(num1);
                        if (pageRotation == 90 || pageRotation == 270)
                        {
                            directContent.AddTemplate(importedPage, 0f, -1f, 1f, 0f, 0f, pdfReader.GetPageSizeWithRotation(num1).Height);
                        }
                        else
                        {
                            directContent.AddTemplate(importedPage, 1f, 0f, 0f, 1f, 0f, 0f);
                        }
                    }
                    num++;
                    if (num >= sourceFiles.Count)
                    {
                        continue;
                    }
                    pdfReader = new PdfReader(sourceFiles[num]);
                    numberOfPages = pdfReader.NumberOfPages;
                }
                pdfReader = new PdfReader(sourceFiles[0]);
                try
                {
                    document.AddTitle(pdfReader.Info["Title"].ToString());
                }
                catch
                {
                }
                try
                {
                    document.AddAuthor(pdfReader.Info["Author"].ToString());
                }
                catch
                {
                }
                try
                {
                    document.AddSubject(pdfReader.Info["Subject"].ToString());
                }
                catch
                {
                }
                try
                {
                    document.AddKeywords(pdfReader.Info["Keywords"].ToString());
                }
                catch
                {
                }
                try
                {
                    document.AddCreator(pdfReader.Info["Creator"].ToString());
                }
                catch
                {
                }
                document.Close();
                array = memoryStream.ToArray();
            }
            catch
            {
                throw;
            }
            return array;
        }
    }
}