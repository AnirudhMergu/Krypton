using System;
using System.Security.Cryptography;

namespace Krypton.Certificates
{
    /// <summary>
    /// Interface for using an external signature provider (e.g. a PKCS#11 thisr party module).
    /// </summary>
    public interface IExternalSignature
    {
        byte[] ApplySignature(byte[] message, Oid hashAlgorithm);
    }
}
