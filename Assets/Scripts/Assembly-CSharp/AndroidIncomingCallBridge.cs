using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class AndroidIncomingCallBridge : MonoBehaviour
{
	public delegate void IncomingCallEvent(string state);

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private IncomingCallEvent onPhoneStateChanged__BackingField;

	private const string CALLBACK_RECEIVER_NAME = "IncomingCallReceiverBridge";

	private AndroidJavaObject _androidBridge;

	public event IncomingCallEvent onPhoneStateChanged
	{
		add
		{
			IncomingCallEvent incomingCallEvent = onPhoneStateChanged__BackingField;
			IncomingCallEvent incomingCallEvent2;
			do
			{
				incomingCallEvent2 = incomingCallEvent;
				incomingCallEvent = Interlocked.CompareExchange(ref onPhoneStateChanged__BackingField, (IncomingCallEvent)Delegate.Combine(incomingCallEvent2, value), incomingCallEvent);
			}
			while ((object)incomingCallEvent != incomingCallEvent2);
		}
		remove
		{
			IncomingCallEvent incomingCallEvent = onPhoneStateChanged__BackingField;
			IncomingCallEvent incomingCallEvent2;
			do
			{
				incomingCallEvent2 = incomingCallEvent;
				incomingCallEvent = Interlocked.CompareExchange(ref onPhoneStateChanged__BackingField, (IncomingCallEvent)Delegate.Remove(incomingCallEvent2, value), incomingCallEvent);
			}
			while ((object)incomingCallEvent != incomingCallEvent2);
		}
	}

	public static AndroidIncomingCallBridge CreatePlugin()
	{
		GameObject gameObject = new GameObject("IncomingCallReceiverBridge");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		return gameObject.AddComponent<AndroidIncomingCallBridge>();
	}

	public void Init()
	{
		if (_androidBridge == null && Application.platform == RuntimePlatform.Android)
		{
			_androidBridge = new AndroidJavaObject("com.kiloo.unityplugins.IncomingCallReceiver");
		}
	}

	private void _callback_onAudioStateChanged(string isModeNormal)
	{
		if (onPhoneStateChanged__BackingField != null)
		{
			onPhoneStateChanged__BackingField(isModeNormal);
		}
	}

	[Conditional("ENABLE_INCOMINGCALL_LOGS")]
	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		SDebug.LogError("AndroidIncomingCallBridge: " + msg, context);
	}

	[Conditional("ENABLE_INCOMINGCALL_LOGS")]
	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
	}

	[Conditional("ENABLE_INCOMINGCALL_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
	}
}
