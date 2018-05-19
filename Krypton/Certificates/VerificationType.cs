using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krypton.Certificates
{
    /// <summary>
    /// Verification type.
    /// </summary>
    public enum VerificationType
    {
        LocalTime,
        OCSP,
        CRL,
        LDAP
    }
}
