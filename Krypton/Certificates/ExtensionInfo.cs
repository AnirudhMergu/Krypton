using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krypton.Certificates
{
    internal class ExtensionInfo
    {
        private object extensionType;

        private bool isCritical;

        private bool extendedKeyUsage;

        public bool ExtendedKeyUsage
        {
            get
            {
                return this.extendedKeyUsage;
            }
        }

        public object ExtensionType
        {
            get
            {
                return this.extensionType;
            }
        }

        public ExtensionInfo(object extension, bool critical, bool extKeyUsage)
        {
            this.extensionType = extension;
            this.isCritical = critical;
            this.extendedKeyUsage = extKeyUsage;
        }
    }
}
