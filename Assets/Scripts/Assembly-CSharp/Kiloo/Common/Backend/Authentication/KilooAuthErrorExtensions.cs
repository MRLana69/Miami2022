using System;

namespace Kiloo.Common.Backend.Authentication
{
	public static class KilooAuthErrorExtensions
	{
		public static KilooAuthError GetKilooAuthError(this Error error)
		{
			if (error.GetErrorCategory() != ErrorCategory.KilooAuth)
			{
				return KilooAuthError.Unknown;
			}
			return Enum.IsDefined(typeof(KilooAuthError), error.code) ? ((KilooAuthError)error.code) : KilooAuthError.Unknown;
		}

		public static int GetErrorCode(this KilooAuthError error)
		{
			return (int)error;
		}
	}
}
