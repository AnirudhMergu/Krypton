using System;

namespace Krypton.Certificates
{
	public enum CertificateStatus
	{
		Valid,
		Revoked,
		Expired,
		Unknown,
		NotPresent
	}
}