using Krypton.Pdf;
using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Krypton
{
    internal class Licensing
    {
        private const string _PRODUCT_NAME_LIC = "KryptoSigner";

        internal static bool demoVersion;

        static Licensing()
        {
            Licensing.demoVersion = false;
        }

        /// <summary>
        /// Initializes ExceptionsStrings class.
        /// </summary>
        public Licensing()
        {
        }

        internal static void CheckLicense(string librarySerialNumberLicense)
        {
            try
            {
                if (librarySerialNumberLicense != null)
                {
                    int num = 0;
                    while (num <= 5000)
                    {
                        if (librarySerialNumberLicense.Trim().ToLower() != Licensing.GetLicProc(string.Concat(num.ToString(), "KryptoSigner".ToLower())).Substring(0, 20).ToLower())
                        {
                            num++;
                        }
                        else
                        {
                            Licensing.demoVersion = false;
                            return;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new ArgumentException(string.Concat("Licensing error: ", exception.Message));
            }
        }

        public static CustomText GetDemoMessageText()
        {
            CustomText customText = new CustomText()
            {
                FontSize = 14,
                Align = TextAlign.Left,
                PageNumber = 1,
                StartingPointPosition = new Point(50, 50),
                Text = "KryptoSigner DEMO VERSION"
            };
            return customText;
        }

        internal static string GetLicProc(string source)
        {
            return BitConverter.ToString((new SHA1CryptoServiceProvider()).ComputeHash(Encoding.Default.GetBytes(source))).Replace("-", string.Empty);
        }

        internal static void ShowDemoMessage()
        {
            try
            {
                if (Licensing.demoVersion)
                {
                    ConsoleColor foregroundColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("This is a demonstration of the digital signature software. Wait 10 seconds.");
                    Console.WriteLine("www.kryptosigner.com");
                    Thread.Sleep(10000);
                    Console.ForegroundColor = foregroundColor;
                }
            }
            catch
            {
            }
        }

        internal static void ShowDemoMessagePdf()
        {
            try
            {
                if (Licensing.demoVersion)
                {
                    ConsoleColor foregroundColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("This is a demo version of Krypton.");
                    Console.WriteLine("www.kryptosigner.com");
                    Console.ForegroundColor = foregroundColor;
                }
            }
            catch
            {
            }
        }
    }
}
