using UnityCommon.Kiloo.Monetization.AdBridgeCommon.MethodInvokers;
using UnityEngine;

namespace Kiloo.Monetization.AdBridgeCommon
{
	public class AndroidMethodInvoker : IMethodInvoker
	{
		private AndroidJavaObject _platformBridge;

		public AndroidMethodInvoker(string javaBridgePath)
		{
			_platformBridge = new AndroidJavaObject(javaBridgePath);
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
