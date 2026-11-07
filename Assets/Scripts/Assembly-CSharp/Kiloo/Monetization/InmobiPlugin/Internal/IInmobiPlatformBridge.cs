namespace Kiloo.Monetization.InmobiPlugin.Internal
{
	public interface IInmobiPlatformBridge
	{
		event InmobiCallback didCacheAd;

		event InmobiCallback didFailToLoadAd;

		event InmobiCallback didDismissAd;

		event InmobiCallback willShowAd;

		event InmobiCallback didClickAd;

		event InmobiCallback didCompleteIncentive;

		event InmobiCallback didInitSuccessfully;

		event InmobiCallback didFailToDisplayAd;

		void Init(string accountId);

		void CacheAd(string placementId);

		void ShowAd(string placementId);

		bool HasCachedAd(string placementId);
	}
}
