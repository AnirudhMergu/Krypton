using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krypton.Certificates
{
    /// <summary>
    /// Certificate Selection Type. for example, CommonNameCN means CN = from certificate Subject field.
    /// </summary>
    public enum DigitalCertificateSearchCriteria
    {
        /// <summary>
        /// "CN=" field extracted from certificate subject.
        /// </summary>
        CommonNameCN,
        /// <summary>
        /// "O=" field extracted from certificate subject.
        /// </summary>
        OrganizationO,
        /// <summary>
        /// "O=" field extracted from certificate subject.
        /// </summary>
        OrganizationUnitOU,
        /// <summary>
        /// "E=" field extracted from certificate subject.
        /// </summary>
        EmailE,
        /// <summary>
        /// "L=" field extracted from certificate subject.
        /// </summary>
        LocalityL,
        /// <summary>
        /// "C=" field extracted from certificate subject.
        /// </summary>
        CountryC,
        /// <summary>
        /// "S=" field extracted from certificate subject.
        /// </summary>
        StateS,
        /// <summary>
        /// "T=" field extracted from certificate subject.
        /// </summary>
        TitleT,
        /// <summary>
        /// Certificate thumbprint.
        /// </summary>
        Thumbprint,
        /// <summary>
        /// Certificate serial number.
        /// </summary>
        SerialNumber
    }
}
