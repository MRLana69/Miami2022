namespace AdHandling.AdConstants.Internal
{
	public interface IAdConstants
	{
		bool isRestricted { get; }

		string AdColonyAppId { get; }

		string TapJoyAppId { get; }

		string TapJoyAppKey { get; }

		string ChartboostAppId { get; }

		string ChartboostAppSignature { get; }

		string InmobiAccountId { get; }

		string VungleAppId { get; }

		string SupersonicAppId { get; }

		string MobVistaAppId { get; }

		string MobVistaAppSdkKey { get; }
	}
}
