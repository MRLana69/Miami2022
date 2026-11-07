using Kiloo.Monetization.AdBridgeCommon;

namespace Kiloo.Monetization.SuperAwesome.Internal
{
	public class SuperAwesomeMethodInvoker : AdBridgeMethodInvoker
	{
		private const string ENABLE_TEST_ADS_METHOD_NAME = "enableTestAds";

		public void EnableTestAds()
		{
			_methodInvoker.Call("enableTestAds");
		}
	}
}
