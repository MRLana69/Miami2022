namespace Kiloo.Common.Ads
{
	public class AdCacheSlotInfo
	{
		public readonly string id;

		public readonly AdType adType;

		public AdCacheSlotInfo(string id, AdType adType)
		{
			this.id = id;
			this.adType = adType;
		}
	}
}
