using System.Collections.Generic;
using Kiloo.Common.Ads;

namespace AdHandling.AdConstants.Internal
{
	public class AdConstantsAndroid_TEST : IAdConstants
	{
		private readonly Dictionary<AdCacheSlotInfo, string> _adColonyCacheIDs = new Dictionary<AdCacheSlotInfo, string>
		{
			{
				AdCacheSlots.POOL1_VIDEOS,
				"pool1_videos"
			},
			{
				AdCacheSlots.POOL2_VIDEOS,
				"pool2_videos"
			}
		};

		private readonly Dictionary<AdCacheSlotInfo, string> _tapjoyCacheIDs = new Dictionary<AdCacheSlotInfo, string>
		{
			{
				AdCacheSlots.POOL1_VIDEOS,
				"pool1_videos"
			},
			{
				AdCacheSlots.POOL2_VIDEOS,
				"pool2_videos"
			}
		};

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

		private readonly Dictionary<AdCacheSlotInfo, string> _inmobiCacheIDs = new Dictionary<AdCacheSlotInfo, string>
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
				"1484122908377"
			}
		};

		private readonly Dictionary<AdCacheSlotInfo, string> _vungleCacheIDs = new Dictionary<AdCacheSlotInfo, string>
		{
			{
				AdCacheSlots.POOL1_VIDEOS,
				"pool1_videos"
			},
			{
				AdCacheSlots.POOL2_VIDEOS,
				"pool2_videos"
			}
		};

		private readonly Dictionary<AdCacheSlotInfo, string> _supersonicCacheIDs = new Dictionary<AdCacheSlotInfo, string>
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

		private readonly Dictionary<AdCacheSlotInfo, string> _facebookCacheIDs = new Dictionary<AdCacheSlotInfo, string> { 
		{
			AdCacheSlots.POOL1_INTERSTITIALS,
			"254616967963463_1278936912198125"
		} };

		private readonly Dictionary<AdCacheSlotInfo, string> _mobvistaCacheIDs = new Dictionary<AdCacheSlotInfo, string>
		{
			{
				AdCacheSlots.POOL1_VIDEOS,
				"pool1_videos"
			},
			{
				AdCacheSlots.POOL2_VIDEOS,
				"pool2_videos"
			}
		};

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
				return "app6caa49a0fc224e4e81";
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
				return "79933fc1-a870-4194-9cca-68e4f571c2a6";
			}
		}

		public string TapJoyAppKey
		{
			get
			{
				return "DyK3KNZ-RJKC-lSjbZzkowEC1FeZOeZEJGzfQxpf4kT3jgxfDKQb3LDSPfJW";
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
				return "583413c2f6cd450e7cddae1b";
			}
		}

		public string ChartboostAppSignature
		{
			get
			{
				return "cb1e3f6960cf0070e868c049e903232d72b83b83";
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
				return "65f49e71333d405686b652d839b89af9";
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
				return "586f5f39ee4ada033e00025b";
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
				return "4cbeb27d";
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
				return _mobvistaCacheIDs;
			}
		}

		public string MobVistaAppId
		{
			get
			{
				return "92762";
			}
		}

		public string MobVistaAppSdkKey
		{
			get
			{
				return "936dcbdd57fe235fd7cf61c2e93da3c4";
			}
		}
	}
}
