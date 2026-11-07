using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SYBO.Components.Utilities
{
	[Serializable]
	[ExecuteInEditMode]
	public class NetworkReachabilityMonitor : SingletonMonoBehaviour<NetworkReachabilityMonitor>
	{
		public UnityEvent OnNetworkReachabilityChanged = new UnityEvent();

		private const int TimeDelayBetweenPolling = 1;

		private NetworkReachability _previousNetworkReachability;

		private Coroutine _checkNetworkReachability;

		public bool IsNetworkReachable
		{
			get
			{
				return NetworkReachability != NetworkReachability.NotReachable;
			}
		}

		public NetworkReachability NetworkReachability
		{
			get
			{
				return Application.internetReachability;
			}
		}

		protected override void SubclassInitialize()
		{
			base.SubclassInitialize();
			_previousNetworkReachability = NetworkReachability;
			_checkNetworkReachability = StartCoroutine(PollToCheckNetworkReachability());
		}

		protected override void OnDestroy()
		{
			if (_checkNetworkReachability != null)
			{
				StopCoroutine(_checkNetworkReachability);
			}
			base.OnDestroy();
		}

		private IEnumerator PollToCheckNetworkReachability()
		{
			while (true)
			{
				if (_previousNetworkReachability != NetworkReachability)
				{
					Debug.Log("CheckNetworkReachability() Ping!");
					OnNetworkReachabilityChanged.Invoke();
				}
				_previousNetworkReachability = NetworkReachability;
				yield return new WaitForSecondsRealtime(1f);
			}
		}
	}
}
