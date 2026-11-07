using System;
using Kiloo.Common.Ads;

namespace Kiloo.Monetization.AdBridgeCommon
{
	public interface IAdBridge
	{
		void Initialize(params object[] initializeParams);

		void SubscribeAdListenerToAdBridgeEvents(IAdEventListener eventListener, string providerTier, Func<AdCacheSlotInfo> getAdCacheSlot, Func<AdLocationInfo> getAdLocation);
	}
}
