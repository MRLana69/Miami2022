using System.Collections.Generic;
using Kiloo.Common.Ads;
using Kiloo.Monetization.AdBridgeCommon;
using Kiloo.Monetization.SuperAwesome.Internal;
using UnityEngine;

namespace Kiloo.Monetization.SuperAwesome
{
	public class SuperAwesomeAdBridge : AdBridgeBase<AdBridgeCallbackReceiver, SuperAwesomeMethodInvoker>
	{
		protected override bool isLoggingEnabled
		{
			get
			{
				return false;
			}
		}

		protected override Dictionary<RuntimePlatform, AdBridgePlatformData> adBridgePlatformDataEntries
		{
			get
			{
				Dictionary<RuntimePlatform, AdBridgePlatformData> dictionary = new Dictionary<RuntimePlatform, AdBridgePlatformData>();
				dictionary.Add(RuntimePlatform.Android, new AdBridgePlatformData
				{
					classPath = "com.kiloo.superawesome.SuperAwesomeAdBridgeReceiver",
					callbackReceiverName = "SuperAwesomeAdBridgeAndroidReceiver"
				});
				dictionary.Add(RuntimePlatform.IPhonePlayer, new AdBridgePlatformData
				{
					classPath = "SuperAwesomeAdBridgeReceiver",
					callbackReceiverName = "SuperAwesomeIosReceiver"
				});
				return dictionary;
			}
		}

		public SuperAwesomeAdBridge(AbstractAdProvider adProvider)
			: base(adProvider)
		{
		}
	}
}
