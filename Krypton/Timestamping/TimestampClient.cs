using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using Krypton;
using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Krypton.Timestamping
{
    /// <summary>
    /// Class for creating timestamp responses (TSD or TSR).
    /// </summary>
    public class TimestampClient
    {
        public TimestampFormat TimestampFormat
        {
            get;
            set;
        }

        /// <summary>
        /// Time Stamping options.
        /// </summary>
        public TimestampSettings TimeStamping
        {
            get;
            set;
        }

        public TimestampClient(string librarySerialNumberLicense)
        {
            try
            {
                Licensing.CheckLicense(librarySerialNumberLicense);
            }
            catch (Exception exception)
            {
                throw new ArgumentException(string.Concat("Licensing error: ", exception.Message));
            }
            this.TimeStamping = new TimestampSettings();
            this.TimestampFormat = TimestampFormat.DetachedTimestamp;
        }

        private byte[] ComputeHash(byte[] msg, HashAlgorithm hashAlgorithm)
        {
            byte[] numArray;
            try
            {
                System.Security.Cryptography.HashAlgorithm sHA1CryptoServiceProvider = null;
                if (hashAlgorithm == HashAlgorithm.SHA1)
                {
                    sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
                }
                if (hashAlgorithm == HashAlgorithm.SHA256)
                {
                    sHA1CryptoServiceProvider = new SHA256Managed();
                }
                if (hashAlgorithm == HashAlgorithm.SHA384)
                {
                    sHA1CryptoServiceProvider = new SHA384Managed();
                }
                if (hashAlgorithm == HashAlgorithm.SHA512)
                {
                    sHA1CryptoServiceProvider = new SHA512Managed();
                }
                numArray = sHA1CryptoServiceProvider.ComputeHash(msg);
            }
            catch
            {
                throw;
            }
            return numArray;
        }

        private byte[] GenerateTSRFile(byte[] originalContent, string fileName, byte[] timestampResponse)
        {
            DerIA5String derIA5String = null;
            if (!string.IsNullOrEmpty(fileName))
            {
                derIA5String = new DerIA5String(fileName);
            }
            BerOctetString berOctetStrings = new BerOctetString(originalContent);
            TimeStampResponse timeStampResponse = new TimeStampResponse(timestampResponse);
            TimeStampAndCrl timeStampAndCrl = new TimeStampAndCrl(timeStampResponse.TimeStampToken.ToCmsSignedData().ContentInfo);
            Evidence evidence = new Evidence(new TimeStampTokenEvidence(timeStampAndCrl));
            TimeStampedData timeStampedDatum = new TimeStampedData(derIA5String, null, berOctetStrings, evidence);
            return (new ContentInfo(CmsObjectIdentifiers.timestampedData, timeStampedDatum)).GetEncoded();
        }

        private byte[] GetTimestampToken(byte[] imprint)
        {
            byte[] numArray;
            try
            {
                Licensing.ShowDemoMessage();
                string value = (new Oid(this.TimeStamping.HashAlgorithm.ToString())).Value;
                TimeStampRequestGenerator timeStampRequestGenerator = new TimeStampRequestGenerator();
                timeStampRequestGenerator.SetCertReq(true);
                if (this.TimeStamping.PolicyOid != null)
                {
                    timeStampRequestGenerator.SetReqPolicy(this.TimeStamping.PolicyOid.Value.ToString());
                }
                TimeStampRequest timeStampRequest = null;
                if (!this.TimeStamping.UseNonce)
                {
                    timeStampRequest = timeStampRequestGenerator.Generate(value, imprint);
                }
                else
                {
                    long tickCount = (long)Environment.TickCount;
                    DateTime now = DateTime.Now;
                    BigInteger bigInteger = BigInteger.ValueOf(tickCount + now.Ticks);
                    timeStampRequest = timeStampRequestGenerator.Generate(value, imprint, bigInteger);
                }
                byte[] tSAResponse = this.GetTSAResponse(timeStampRequest.GetEncoded());
                TimeStampResponse timeStampResponse = new TimeStampResponse(tSAResponse);
                timeStampResponse.Validate(timeStampRequest);
                if ((timeStampResponse.GetFailInfo() == null ? 0 : 1) != 0)
                {
                    string[] invalidTimeStampingResponse = new string[] { CustomExceptions.InvalidTimeStampingResponse, "Status: ", null, null, null };
                    invalidTimeStampingResponse[2] = timeStampResponse.Status.ToString();
                    invalidTimeStampingResponse[3] = "; Status information : ";
                    invalidTimeStampingResponse[4] = timeStampResponse.GetStatusString();
                    throw new WebException(string.Concat(invalidTimeStampingResponse));
                }
                if (timeStampResponse.TimeStampToken == null)
                {
                    throw new WebException(CustomExceptions.InvalidTimeStampingResponse);
                }
                numArray = tSAResponse;
            }
            catch
            {
                throw;
            }
            return numArray;
        }

        private byte[] GetTSAResponse(byte[] requestBytes)
        {
            byte[] array;
            byte[] numArray;
            try
            {
                HttpWebRequest serverTimeout = (HttpWebRequest)WebRequest.Create(this.TimeStamping.ServerUrl);
                serverTimeout.Timeout = this.TimeStamping.ServerTimeout;
                serverTimeout.ContentType = "application/timestamp-query";
                serverTimeout.Method = "POST";
                if (!string.IsNullOrEmpty(this.TimeStamping.UserName))
                {
                    serverTimeout.Headers.Add("Authorization", string.Concat("Basic ", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Concat(this.TimeStamping.UserName, ":", this.TimeStamping.Password)))));
                }
                if (this.TimeStamping.AuthenticationCertificate != null)
                {
                    serverTimeout.ClientCertificates.Add(this.TimeStamping.AuthenticationCertificate);
                }
                serverTimeout.ContentLength = (long)((int)requestBytes.Length);
                Stream requestStream = serverTimeout.GetRequestStream();
                requestStream.Write(requestBytes, 0, (int)requestBytes.Length);
                requestStream.Close();
                WebResponse response = serverTimeout.GetResponse();
                Stream responseStream = response.GetResponseStream();
                MemoryStream memoryStream = new MemoryStream();
                byte[] numArray1 = new byte[4096];
                using (MemoryStream memoryStream1 = new MemoryStream())
                {
                    int num = 0;
                    do
                    {
                        num = responseStream.Read(numArray1, 0, (int)numArray1.Length);
                        memoryStream1.Write(numArray1, 0, num);
                    }
                    while (num != 0);
                    array = memoryStream1.ToArray();
                }
                response.Close();
                numArray = array;
            }
            catch
            {
                throw;
            }
            return numArray;
        }

        public byte[] ObtainTimestamp(byte[] dataToBeTimestamped)
        {
            byte[] numArray;
            try
            {
                byte[] timestampToken = this.GetTimestampToken(this.ComputeHash(dataToBeTimestamped, this.TimeStamping.HashAlgorithm));
                numArray = (this.TimestampFormat != TimestampFormat.DetachedTimestamp ? this.GenerateTSRFile(dataToBeTimestamped, null, timestampToken) : timestampToken);
            }
            catch
            {
                throw;
            }
            return numArray;
        }

        public byte[] ObtainTimestamp(string fileToBeTimestamped)
        {
            byte[] numArray;
            try
            {
                byte[] numArray1 = File.ReadAllBytes(fileToBeTimestamped);
                byte[] timestampToken = this.GetTimestampToken(this.ComputeHash(numArray1, this.TimeStamping.HashAlgorithm));
                numArray = (this.TimestampFormat != TimestampFormat.DetachedTimestamp ? this.GenerateTSRFile(numArray1, Path.GetFileName(fileToBeTimestamped), timestampToken) : timestampToken);
            }
            catch
            {
                throw;
            }
            return numArray;
        }
    }
}