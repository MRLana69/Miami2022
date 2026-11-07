using System.Collections.Generic;
using Kiloo.Common.Ads;

public static class AdCacheSlots
{
	public static readonly AdCacheSlotInfo UNDETERMINED_SLOT = NewAdCacheSlot("undetermined", AdType.Interstitial);

	public static readonly AdCacheSlotInfo POOL1_VIDEOS = NewAdCacheSlot("pool1_videos", AdType.Video);

	public static readonly AdCacheSlotInfo POOL2_VIDEOS = NewAdCacheSlot("pool2_videos", AdType.Video);

	public static readonly AdCacheSlotInfo POOL1_INTERSTITIALS = NewAdCacheSlot("home_interstitials", AdType.Interstitial);

	public static ICollection<AdCacheSlotInfo> adCacheSlots;

	private static AdCacheSlotInfo NewAdCacheSlot(string id, AdType type)
	{
		adCacheSlots = adCacheSlots ?? new List<AdCacheSlotInfo>();
		AdCacheSlotInfo adCacheSlotInfo = new AdCacheSlotInfo(id, type);
		adCacheSlots.Add(adCacheSlotInfo);
		return adCacheSlotInfo;
	}

	public static AdCacheSlotInfo GetCacheSlotForIndex(AdType adType, int index)
	{
		switch (adType)
		{
		case AdType.Video:
			switch (index)
			{
			case 0:
				return POOL1_VIDEOS;
			case 1:
				return POOL2_VIDEOS;
			default:
				return UNDETERMINED_SLOT;
			}
		case AdType.Interstitial:
			if (index == 0)
			{
				return POOL1_INTERSTITIALS;
			}
			break;
		}
		return UNDETERMINED_SLOT;
	}
}
