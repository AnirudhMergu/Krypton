using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

namespace Krypton.Timestamping
{
    public class TimestampInfo
    {
        /// <summary>
        /// The TSA Server accuracy.
        /// </summary>
        public TimestampAccuracy Accuracy
        {
            get;
            internal set;
        }

        /// <summary>
        /// Get the TSA hash algorithm (SHA1, SHA256, etc.).
        /// </summary>
        public Oid HashAlgorithm
        {
            get;
            internal set;
        }

        /// <summary>
        /// Check if the timestamp signature is altered.
        /// </summary>
        public bool IsTimestampAltered
        {
            get;
            internal set;
        }

        /// <summary>
        /// Get the Nonce.
        /// </summary>
        public string Nonce
        {
            get;
            internal set;
        }

        /// <summary>
        /// The hash data sent to the TSA Server.
        /// </summary>
        public byte[] OriginalDataHash
        {
            get;
            internal set;
        }

        /// <summary>
        /// Get the TSA Policy (e.g. 1.3.6.1.4.1.13762.3)
        /// </summary>
        public Oid Policy
        {
            get;
            internal set;
        }

        /// <summary>
        /// Get the TSA Response serial number.
        /// </summary>
        public string SerialNumber
        {
            get;
            internal set;
        }

        /// <summary>
        /// Get the signature time. The signing date is taken from the time stamping response and it is in UTC.
        /// </summary>
        public DateTime SignatureTime
        {
            get;
            internal set;
        }

        /// <summary>
        /// Get the timestamp token.
        /// </summary>
        public byte[] TimestampToken
        {
            get;
            internal set;
        }

        /// <summary>
        /// Get the time stamping certificate.
        /// </summary>
        public X509Certificate2 TsaCertificate
        {
            get;
            internal set;
        }

        /// <summary>
        /// Get the TSA Server Name.
        /// </summary>
        public string TsaServerName
        {
            get;
            internal set;
        }

        public TimestampInfo(byte[] timestampToken)
        {
            try
            {
                TimeStampToken timeStampToken = new TimeStampToken(new CmsSignedData(timestampToken));
                SignedCms signedCm = new SignedCms();
                signedCm.Decode(timestampToken);
                try
                {
                    signedCm.CheckSignature(true);
                    this.IsTimestampAltered = false;
                }
                catch
                {
                    this.IsTimestampAltered = true;
                }
                this.TimestampToken = timestampToken;
                this.SignatureTime = timeStampToken.TimeStampInfo.GenTime;
                try
                {
                    this.TsaCertificate = signedCm.Certificates[signedCm.Certificates.Count - 1];
                }
                catch
                {
                }
                try
                {
                    this.OriginalDataHash = timeStampToken.TimeStampInfo.GetMessageImprintDigest();
                }
                catch
                {
                }
                try
                {
                    this.HashAlgorithm = new Oid(timeStampToken.TimeStampInfo.MessageImprintAlgOid);
                }
                catch
                {
                }
                try
                {
                    this.Policy = new Oid(timeStampToken.TimeStampInfo.Policy);
                }
                catch
                {
                }
                try
                {
                    this.Nonce = timeStampToken.TimeStampInfo.Nonce.ToString(10);
                }
                catch
                {
                }
                try
                {
                    this.SerialNumber = timeStampToken.TimeStampInfo.SerialNumber.ToString(10);
                }
                catch
                {
                }
                try
                {
                    this.TsaServerName = timeStampToken.TimeStampInfo.Tsa.Name.ToString();
                }
                catch
                {
                }
                this.Accuracy = new TimestampAccuracy();
                try
                {
                    this.Accuracy.Seconds = timeStampToken.TimeStampInfo.Accuracy.Seconds.ToString();
                }
                catch
                {
                }
                try
                {
                    this.Accuracy.Milliseconds = timeStampToken.TimeStampInfo.Accuracy.Millis.ToString();
                }
                catch
                {
                }
                try
                {
                    this.Accuracy.Microseconds = timeStampToken.TimeStampInfo.Accuracy.Micros.ToString();
                }
                catch
                {
                }
            }
            catch
            {
            }
        }

        private static bool CompareHashes(byte[] a1, byte[] a2)
        {
            if ((int)a1.Length != (int)a2.Length)
            {
                return false;
            }
            for (int i = 0; i < (int)a1.Length; i++)
            {
                if (a1[i] != a2[i])
                {
                    return false;
                }
            }
            return true;
        }

        private static byte[] ComputeHash(byte[] msg, Oid algorithm)
        {
            byte[] numArray;
            try
            {
                System.Security.Cryptography.HashAlgorithm mD5CryptoServiceProvider = null;
                if (algorithm.FriendlyName.ToUpper() == "MD5")
                {
                    mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
                }
                if (algorithm.FriendlyName.ToUpper() == "SHA1")
                {
                    mD5CryptoServiceProvider = new SHA1CryptoServiceProvider();
                }
                if (algorithm.FriendlyName.ToUpper() == "SHA256")
                {
                    mD5CryptoServiceProvider = new SHA256Managed();
                }
                if (algorithm.FriendlyName.ToUpper() == "SHA384")
                {
                    mD5CryptoServiceProvider = new SHA384Managed();
                }
                if (algorithm.FriendlyName.ToUpper() == "SHA512")
                {
                    mD5CryptoServiceProvider = new SHA512Managed();
                }
                numArray = mD5CryptoServiceProvider.ComputeHash(msg);
            }
            catch
            {
                throw;
            }
            return numArray;
        }

        public static TimestampInfo GetInfoFromTsaResponse(byte[] timestampToken)
        {
            TimestampInfo timestampInfo;
            try
            {
                TimeStampResponse timeStampResponse = new TimeStampResponse(timestampToken);
                timestampInfo = new TimestampInfo(timeStampResponse.TimeStampToken.GetEncoded());
            }
            catch
            {
                try
                {
                    timestampInfo = new TimestampInfo(timestampToken);
                }
                catch
                {
                    timestampInfo = null;
                }
            }
            return timestampInfo;
        }

        /// <summary>
        /// In the file was changed or the timestamp signature is altered, an exception will be thrown.
        /// </summary>
        /// <param name="originalFile"></param>
        /// <param name="timestampToken"></param>
        /// <returns></returns>
        public static bool IsTsaReponseFileValid(byte[] originalFile, byte[] timestampToken)
        {
            bool flag;
            try
            {
                TimestampInfo infoFromTsaResponse = TimestampInfo.GetInfoFromTsaResponse(timestampToken);
                if (infoFromTsaResponse == null || infoFromTsaResponse.HashAlgorithm == null)
                {
                    throw new Exception("The timestamp token is not a valid timestamp response.");
                }
                if (infoFromTsaResponse.IsTimestampAltered)
                {
                    throw new Exception("The timestamp response is altered.");
                }
                byte[] numArray = TimestampInfo.ComputeHash(originalFile, infoFromTsaResponse.HashAlgorithm);
                if (!TimestampInfo.CompareHashes(infoFromTsaResponse.OriginalDataHash, numArray))
                {
                    throw new Exception("The hash that appear on the timestamp response is not identical with the calculated file hash.");
                }
                flag = true;
            }
            catch
            {
                throw;
            }
            return flag;
        }
    }
}