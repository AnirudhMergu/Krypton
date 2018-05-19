using Krypton;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Krypton.Timestamping
{
    /// <summary>
    /// Time Stamping options.
    /// </summary>
    public class TimestampSettings
    {
        /// <summary>
        /// Gets or sets the certificate used for TSA Server authentication.
        /// </summary>
        public X509Certificate2 AuthenticationCertificate
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the hash algorithm used for the time stamping request. The default value is SHA1.
        /// </summary>
        public HashAlgorithm HashAlgorithm
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Time Stamping Server password. Used for server authentication.
        /// </summary>
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the TSA Policy OID. Set it only when the server requires this parameter.
        /// </summary>
        public Oid PolicyOid
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the timeout for the time stamping server response in milliseconds. The default value is 20000 milliseconds (20 seconds).
        /// </summary>
        public int ServerTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time stamping server URL. For example: https://ca.signfiles.com/tsa/get.aspx.
        /// </summary>
        public Uri ServerUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Using nonce for TSA Request. Default is true.
        /// </summary>
        public bool UseNonce
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Time Stamping Server username. Used for server authentication.
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes TSAConfiguration class.
        /// </summary>
        public TimestampSettings()
        {
            this.UseNonce = true;
            this.HashAlgorithm = HashAlgorithm.SHA1;
            this.ServerTimeout = 20000;
        }
    }
}