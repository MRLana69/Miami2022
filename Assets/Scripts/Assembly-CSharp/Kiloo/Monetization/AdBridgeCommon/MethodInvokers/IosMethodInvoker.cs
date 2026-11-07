using UnityCommon.Kiloo.Monetization.AdBridgeCommon.MethodInvokers;

namespace Kiloo.Monetization.AdBridgeCommon.MethodInvokers
{
	public class IosMethodInvoker : IMethodInvoker
	{
		private IosObjectiveCObject _platformBridge;

		public IosMethodInvoker(string classPath)
		{
			_platformBridge = new IosObjectiveCObject(classPath);
		}

		public void Call(string methodName, params object[] parameters)
		{
			_platformBridge.Call(methodName, parameters);
		}

		public T Call<T>(string methodName, params object[] parameters)
		{
			return _platformBridge.Call<T>(methodName, parameters);
		}
	}
}
