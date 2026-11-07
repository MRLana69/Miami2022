namespace Kiloo.Google
{
	public interface IGoogleProfile
	{
		string displayName { get; }

		string photoUrl { get; }

		string serverAuthCode { get; }

		string userId { get; }
	}
}
