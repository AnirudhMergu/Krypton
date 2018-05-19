using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krypton.Certificates
{
    public enum SubjectType
    {
        /// <summary>
        /// country code - StringType(SIZE(2)).
        /// </summary>
        C,
        /// <summary>
        /// Email address.
        /// </summary>
        E,
        /// <summary>
        /// locality name - StringType(SIZE(1..64)).
        /// </summary>
        L,
        /// <summary>
        /// organization - StringType(SIZE(1..64)).
        /// </summary>
        O,
        /// <summary>
        /// Title.
        /// </summary>
        T,
        /// <summary>
        /// common name - StringType(SIZE(1..64)).
        /// </summary>
        CN,
        /// <summary>
        /// Others.
        /// </summary>
        DC,
        /// <summary>
        /// organizational unit name - StringType(SIZE(1..64)).
        /// </summary>
        OU,
        /// <summary>
        /// state, or province name - StringType(SIZE(1..64)).
        /// </summary>
        ST,
        /// <summary>
        /// LDAP User id.
        /// </summary>
        UID,
        /// <summary>
        /// street - StringType(SIZE(1..64)).
        /// </summary>
        STREET,
        /// <summary>
        /// RFC 3039 Pseudonym - DirectoryString(SIZE(1..64).
        /// </summary>
        PSEUDONYM,
        /// <summary>
        /// Name.
        /// </summary>
        NAME,
        /// <summary>
        /// Email address (RSA PKCS#9 extension) - IA5String.
        /// </summary>
        EMAILADDRESS,
        /// <summary>
        /// serial number - StringType(SIZE(1..64)).
        /// </summary>
        SERIALNUMBER,
        /// <summary>
        /// Surname.
        /// </summary>
        SURNAME,
        /// <summary>
        /// Given name.
        /// </summary>
        GIVENNAME,
        /// <summary>
        /// Initials.
        /// </summary>
        INITIALS
    }
}
