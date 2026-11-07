namespace Kiloo.Monetization.TapjoyPlugin.Internal
{
	public interface ITapjoyBridge
	{
		event TapjoyConnectEvent onConnect;

		event TapjoyGetFullScreenAdSuccess onGetFullScreenAdSuccess;

		event TapjoyGetFullScreenAdFail onGetFullScreenAdFail;

		event TapjoyGetOfferWallResponseEvent onGetOfferWallResponse;

		event TapjoyViewDidAppearEvent onViewDidAppear;

		event TapjoyViewDidDisappearEvent onViewDidDisappear;

		event TapjoyViewWillAppearEvent onViewWillAppear;

		event TapjoyViewWillDisappearEvent onViewWillDisappear;

		event TapjoyVideoStartEvent onVideoStart;

		event TapjoyVideoErrorEvent onVideoError;

		event TapjoyVideoCompleteEvent onVideoComplete;

		event TapjoyEarnedPointsEvent onEarnedPoints;

		event TapjoyGetPointsEvent onGetPoints;

		event TapjoyGetPointsErrorEvent onGetPointsError;

		event TapjoySpendPointsEvent onSpendPoints;

		event TapjoySpendPointsErrorEvent onSpendPointsError;

		void Initialize(string appID, string appKey);

		void SetGdprConsent(bool consent);

		void GetFullScreenAd(string placement);

		void ShowFullScreenAd(string placement);

		void GetOfferWall(string eventname);

		void ShowOfferWall();

		bool IsOfferWallReady();

		void GetPoints();

		void SpendPoints(int amount);
	}
}
