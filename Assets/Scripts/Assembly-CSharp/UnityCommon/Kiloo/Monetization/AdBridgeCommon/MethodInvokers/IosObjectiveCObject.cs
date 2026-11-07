using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Kiloo.Common.Serialization;
using Kiloo.Common.Utility;

namespace UnityCommon.Kiloo.Monetization.AdBridgeCommon.MethodInvokers
{
	public class IosObjectiveCObject
	{
		private KilooSerializer _kilooSerializer;

		private string _classPath;

		private Stopwatch _stopwatch;

		public IosObjectiveCObject(string classPath)
		{
			_classPath = classPath;
			_kilooSerializer = new KilooSerializer();
			adBridgeMethodInvokerCreateAdBridge(classPath);
			_stopwatch = new Stopwatch();
		}

		[DllImport("__Internal")]
		private static extern void adBridgeMethodInvokerCreateAdBridge(string className);

		[DllImport("__Internal")]
		private static extern void adBridgeMethodInvokerCallMethodOnAdBridge(string className, string methodName);

		[DllImport("__Internal")]
		private static extern void adBridgeMethodInvokerCallMethodOnAdBridgeWithParams(string className, string methodName, string serializedParameters);

		[DllImport("__Internal")]
		private static extern IntPtr adBridgeMethodInvokerCallMethodOnAdBridgeWithReturnValue(string className, string methodName);

		[DllImport("__Internal")]
		private static extern IntPtr adBridgeMethodInvokerCallMethodOnAdBridgeWithReturnValueAndParams(string className, string methodName, string serializedParameters);

		public void Call(string methodName, params object[] parameters)
		{
			if (parameters.Length <= 0)
			{
				adBridgeMethodInvokerCallMethodOnAdBridge(_classPath, methodName);
				return;
			}
			_stopwatch.Reset();
			_stopwatch.Start();
			string serializedParameters = _kilooSerializer.ToJson(parameters);
			_stopwatch.Stop();
			adBridgeMethodInvokerCallMethodOnAdBridgeWithParams(_classPath, methodName, serializedParameters);
		}

		public T Call<T>(string methodName, params object[] parameters)
		{
			string empty = string.Empty;
			if (parameters.Length <= 0)
			{
				empty = Marshal.PtrToStringAnsi(adBridgeMethodInvokerCallMethodOnAdBridgeWithReturnValue(_classPath, methodName));
			}
			else
			{
				_stopwatch.Reset();
				_stopwatch.Start();
				string serializedParameters = _kilooSerializer.ToJson(parameters);
				_stopwatch.Stop();
				empty = Marshal.PtrToStringAnsi(adBridgeMethodInvokerCallMethodOnAdBridgeWithReturnValueAndParams(_classPath, methodName, serializedParameters));
			}
			object[] array = _kilooSerializer.FromJson<object[]>(empty);
			return (T)array[0];
		}
	}
}
