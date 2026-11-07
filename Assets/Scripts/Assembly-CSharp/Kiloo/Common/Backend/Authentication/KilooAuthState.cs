namespace Kiloo.Common.Backend.Authentication
{
	public enum KilooAuthState
	{
		None = 0,
		Authenticating = 1,
		Authorized = 2,
		Error = 3,
		Invalid = 4,
		Expired = 5
	}
}
