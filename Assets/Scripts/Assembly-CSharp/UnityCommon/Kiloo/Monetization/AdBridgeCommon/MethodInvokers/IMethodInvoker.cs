namespace UnityCommon.Kiloo.Monetization.AdBridgeCommon.MethodInvokers
{
	public interface IMethodInvoker
	{
		void Call(string methodName, params object[] parameters);

		T Call<T>(string methodName, params object[] parameters);
	}
}
