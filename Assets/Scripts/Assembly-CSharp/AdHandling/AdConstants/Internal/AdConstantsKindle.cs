using System.Collections.Generic;
using Kiloo.Common.Ads;

namespace AdHandling.AdConstants.Internal
{
	public class AdConstantsKindle : IAdConstants
	{
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
				return "appdcc8fa2442444dfbad";
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
				return "5b1a3203bd37ad0aaef02d69";
			}
		}

		public string ChartboostAppSignature
		{
			get
			{
				return "245cb01954dbefe6b7f160ae14a948188bbb38fa";
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
				return "5b07dbce3b5f67265c7e5f76";
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
