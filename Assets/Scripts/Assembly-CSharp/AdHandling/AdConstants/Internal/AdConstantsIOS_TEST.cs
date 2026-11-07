using System.Collections.Generic;
using Kiloo.Common.Ads;

namespace AdHandling.AdConstants.Internal
{
	public class AdConstantsIOS_TEST : IAdConstants
	{
		private readonly Dictionary<AdCacheSlotInfo, string> _adColonyCacheIDs = new Dictionary<AdCacheSlotInfo, string>
		{
			{
				AdCacheSlots.POOL1_VIDEOS,
				"vz1b7fa5d4092a4f59a4"
			},
			{
				AdCacheSlots.POOL2_VIDEOS,
				"vz0a1d7438d43348b297"
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
				"1536452057849"
			},
			{
				AdCacheSlots.POOL2_VIDEOS,
				"1536708164374"
			},
			{
				AdCacheSlots.POOL1_INTERSTITIALS,
				"1483524202742"
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

		private readonly Dictionary<AdCacheSlotInfo, string> _facebookCacheIDs = new Dictionary<AdCacheSlotInfo, string>();

		private readonly Dictionary<AdCacheSlotInfo, string> _mobvistaCacheIDs = new Dictionary<AdCacheSlotInfo, string>
		{
			{
				AdCacheSlots.POOL1_VIDEOS,
				"53598"
			},
			{
				AdCacheSlots.POOL2_VIDEOS,
				"53599"
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
				return "appe16ddc2239b94013abd6b2";
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
				return "feb4000a-c9ee-4388-941f-e0391e462ec5";
			}
		}

		public string TapJoyAppKey
		{
			get
			{
				return "_rQACsnuQ4iUH-A5HkYuxQEBElEQtlcZKs1W2D9sa6g1grIoAafYJ9EJffsN";
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
				return "5024e3582284bb9a5a00003d";
			}
		}

		public string ChartboostAppSignature
		{
			get
			{
				return "73a9d4bbe6eee00ee592f4aa73604c78fddd717e";
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
				return "586f90feee0dcc35280004c3";
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
				return "4cbeec05";
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
				return "92763";
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
