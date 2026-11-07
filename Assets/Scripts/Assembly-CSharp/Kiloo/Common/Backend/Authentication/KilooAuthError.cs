namespace Kiloo.Common.Backend.Authentication
{
	public enum KilooAuthError
	{
		Unknown = 0,
		NoPlatformsLoggedIn = 1,
		UserNotFound = 1000,
		InvalidFacebookToken = 1001,
		InvalidAppIdentifier = 1003,
		FacebookAccountMismatch = 1005,
		InvalidKilooId = 1007,
		InvalidRequest = 1008,
		FacebookAccountAlreadyUsed = 1009,
		InvalidGoogleIdToken = 1010,
		GameCenterPublicKeyError = 1011,
		InvalidGameCenterSignature = 1012,
		GameCenterAccountAlreadyUsed = 1013,
		GameCenterAccountMismatch = 1014,
		GoogleAccountAlreadyUsed = 1015,
		GoogleAccountMismatch = 1016,
		AmbiguousCredentials = 1017,
		InternalError = 1018,
		Timeout = 1019,
		InvalidTemporaryCode = 1020,
		InvalidKilooAuthToken = 1100,
		AppIdentifierMismatch = 1103,
		AccessTokenExpired = 1110
	}
}
