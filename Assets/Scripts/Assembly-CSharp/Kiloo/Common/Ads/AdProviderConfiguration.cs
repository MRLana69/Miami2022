namespace Kiloo.Common.Ads
{
	public class AdProviderConfiguration
	{
		public bool isRestricted { get; set; }

		public string appID { get; set; }

		public string appSDKKey { get; set; }

		public string appSignature { get; set; }

		public string[] zoneIDs { get; set; }
	}
}
