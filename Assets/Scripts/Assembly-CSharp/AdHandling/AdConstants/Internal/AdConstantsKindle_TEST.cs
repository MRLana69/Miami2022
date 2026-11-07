using System.Collections.Generic;
using Kiloo.Common.Ads;

namespace AdHandling.AdConstants.Internal
{
	public class AdConstantsKindle_TEST : IAdConstants
	{
		private readonly Dictionary<AdCacheSlotInfo, string> _adColonyCacheIDs = new Dictionary<AdCacheSlotInfo, string>();

		private readonly Dictionary<AdCacheSlotInfo, string> _tapjoyCacheIDs = new Dictionary<AdCacheSlotInfo, string>();

		private readonly Dictionary<AdCacheSlotInfo, string> _chartboostCacheIDs = new Dictionary<AdCacheSlotInfo, string>
		{
			{
				AdCacheSlots.POOL1_VIDEOS,
				"pool1_videos"
			},
			{
				AdCacheSlots.POOL2_VIDEOS,
				"pool2_videos"
			},
			{
				AdCacheSlots.POOL1_INTERSTITIALS,
				"home_interstitials"
			}
		};

		private readonly Dictionary<AdCacheSlotInfo, string> _inmobiCacheIDs = new Dictionary<AdCacheSlotInfo, string>();

		private readonly Dictionary<AdCacheSlotInfo, string> _vungleCacheIDs = new Dictionary<AdCacheSlotInfo, string>();

		private readonly Dictionary<AdCacheSlotInfo, string> _supersonicCacheIDs = new Dictionary<AdCacheSlotInfo, string>();

		private readonly Dictionary<AdCacheSlotInfo, string> _facebookCacheIDs = new Dictionary<AdCacheSlotInfo, string>();

		public bool isRestricted
		{
			get
			{
				return false;
			}
		}

		public string AdColonyAppId
		{
			get
			{
				return string.Empty;
			}
		}

		public Dictionary<AdCacheSlotInfo, string> AdColonyCacheIDs
		{
			get
			{
				return _adColonyCacheIDs;
			}
		}

		public string TapJoyAppId
		{
			get
			{
				return string.Empty;
			}
		}

		public string TapJoyAppKey
		{
			get
			{
				return string.Empty;
			}
		}

		public Dictionary<AdCacheSlotInfo, string> TapJoyCacheIDs
		{
			get
			{
				return _tapjoyCacheIDs;
			}
		}

		public string ChartboostAppId
		{
			get
			{
				return "541c0fa51873da1a46201811";
			}
		}

		public string ChartboostAppSignature
		{
			get
			{
				return "f7422a8be58ef109f22c0413dfffb6df6fa905dc";
			}
		}

		public Dictionary<AdCacheSlotInfo, string> ChartboostCacheIDs
		{
			get
			{
				return _chartboostCacheIDs;
			}
		}

		public string InmobiAccountId
		{
			get
			{
				return string.Empty;
			}
		}

		public Dictionary<AdCacheSlotInfo, string> InmobiCacheIDs
		{
			get
			{
				return _inmobiCacheIDs;
			}
		}

		public string VungleAppId
		{
			get
			{
				return string.Empty;
			}
		}

		public Dictionary<AdCacheSlotInfo, string> VungleCacheIDs
		{
			get
			{
				return _vungleCacheIDs;
			}
		}

		public string SupersonicAppId
		{
			get
			{
				return string.Empty;
			}
		}

		public Dictionary<AdCacheSlotInfo, string> SupersonicCacheIDs
		{
			get
			{
				return _supersonicCacheIDs;
			}
		}

		public Dictionary<AdCacheSlotInfo, string> FacebookCacheIDs
		{
			get
			{
				return _facebookCacheIDs;
			}
		}

		public Dictionary<AdCacheSlotInfo, string> MobVistaCacheIDs
		{
			get
			{
				return new Dictionary<AdCacheSlotInfo, string>();
			}
		}

		public string MobVistaAppId
		{
			get
			{
				return string.Empty;
			}
		}

		public string MobVistaAppSdkKey
		{
			get
			{
				return string.Empty;
			}
		}

		public Dictionary<AdLocationInfo, string> SuperAwesomeCacheIDs
		{
			get
			{
				return new Dictionary<AdLocationInfo, string>();
			}
		}
	}
}
