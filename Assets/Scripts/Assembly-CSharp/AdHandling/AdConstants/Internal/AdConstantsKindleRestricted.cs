using System.Collections.Generic;
using Kiloo.Common.Ads;

namespace AdHandling.AdConstants.Internal
{
	public class AdConstantsKindleRestricted : IAdConstants
	{
		public bool isRestricted
		{
			get
			{
				return true;
			}
		}

		public string AdColonyAppId
		{
			get
			{
				return "app60a5688ed75a4ef595";
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

		public string ChartboostAppId
		{
			get
			{
				return "5922afa143150f6edddd6c23";
			}
		}

		public string ChartboostAppSignature
		{
			get
			{
				return "bbf5160efc2c49b4fc3e7d22fb9272f5eb3768df";
			}
		}

		public string InmobiAccountId
		{
			get
			{
				return "6c7ea8566bfd4b2c82269e3647046624";
			}
		}

		public string VungleAppId
		{
			get
			{
				return "5a3a17ab7f1834bc71006e8a";
			}
		}

		public string SupersonicAppId
		{
			get
			{
				return string.Empty;
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
