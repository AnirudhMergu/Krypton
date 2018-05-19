using System;

namespace Krypton.Timestamping
{
    /// <summary>
    /// Timestamp format.
    /// </summary>
    public enum TimestampFormat
    {
        /// <summary>
        /// The timestamp information will be saved on a separate file in Time Stamp Response format (.TSR file).
        /// </summary>
        DetachedTimestamp,
        /// <summary>
        /// The timestamp information will be saved with the original file like on RFC 5544 in Time Stamp Data format (.TSD file).
        /// </summary>
        EmbeddedTimestamp
    }
}