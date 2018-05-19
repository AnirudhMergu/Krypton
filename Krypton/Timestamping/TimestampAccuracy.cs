using System;
using System.Runtime.CompilerServices;

namespace Krypton.Timestamping
{
    public class TimestampAccuracy
    {
        public string Microseconds
        {
            get;
            internal set;
        }

        public string Milliseconds
        {
            get;
            internal set;
        }

        public string Seconds
        {
            get;
            internal set;
        }

        public TimestampAccuracy()
        {
        }
    }
}