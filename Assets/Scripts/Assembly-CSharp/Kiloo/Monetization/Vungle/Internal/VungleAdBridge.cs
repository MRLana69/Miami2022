using System.Collections.Generic;
using Kiloo.Common.Ads;
using Kiloo.Monetization.AdBridgeCommon;
using Kiloo.Monetization.SuperAwesome.Internal;
using UnityEngine;

namespace Kiloo.Monetization.Vungle.Internal
{
	public class VungleAdBridge : AdBridgeBase<VungleCallbackReceiver, AdBridgeMethodInvoker>
	{
		private Dictionary<RuntimePlatform, AdBridgePlatformData> _adBridgePlatformDataEntries;

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
					classPath = "com.kiloo.vungleplugin.VungleAdBridgeReceiver",
					callbackReceiverName = "VungleAndroidReceiver"
				});
				dictionary.Add(RuntimePlatform.IPhonePlayer, new AdBridgePlatformData
				{
					classPath = "VungleAdBridgeReceiver",
					callbackReceiverName = "VungleIOSReceiver"
				});
				return dictionary;
			}
		}

		public VungleAdBridge(AbstractAdProvider adProvider)
			: base(adProvider)
		{
		}
	}
}
