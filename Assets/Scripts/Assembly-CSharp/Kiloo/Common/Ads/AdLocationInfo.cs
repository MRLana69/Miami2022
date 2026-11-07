namespace Kiloo.Common.Ads
{
	public class AdLocationInfo
	{
		public readonly string id;

		public readonly AdType adType;

		public AdLocationInfo(string id, AdType adType)
		{
			this.id = id;
			this.adType = adType;
		}
	}
}
