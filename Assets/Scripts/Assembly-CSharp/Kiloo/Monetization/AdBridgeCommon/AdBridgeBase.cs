using System;
using System.Collections.Generic;
using Kiloo.Common.Ads;
using UnityEngine;

namespace Kiloo.Monetization.AdBridgeCommon
{
	public abstract class AdBridgeBase<TAdBridgeCallbackReceiver, TAdBridgeMethodInvoker> : IAdBridge where TAdBridgeCallbackReceiver : AdBridgeCallbackReceiver where TAdBridgeMethodInvoker : AdBridgeMethodInvoker
	{
		private bool _hasWatchedVideo;

		private AdBridgeLogger _adBridgeLogger;

		protected AbstractAdProvider _adProvider;

		protected AdBridgePlatformData _adBridgePlatformData;

		protected abstract bool isLoggingEnabled { get; }

		protected abstract Dictionary<RuntimePlatform, AdBridgePlatformData> adBridgePlatformDataEntries { get; }

		public TAdBridgeCallbackReceiver adBridgeCallbackReceiver { get; private set; }

		public TAdBridgeMethodInvoker adBridgeMethodInvoker { get; private set; }

		protected AdBridgeBase(AbstractAdProvider adProvider)
		{
			_adProvider = adProvider;
			CreateAdBridgeComponents();
		}

		public void Initialize(params object[] initializeParams)
		{
			if (!_adProvider.isInitialized)
			{
				Log("Initialize");
				_adBridgeLogger.UpdateDebugLogsState();
				TAdBridgeMethodInvoker val = adBridgeMethodInvoker;
				val.SetCallbackReceiverName(_adBridgePlatformData.callbackReceiverName);
				TAdBridgeMethodInvoker val2 = adBridgeMethodInvoker;
				val2.OnInitialize(initializeParams);
			}
		}

		private void CreateAdBridgeComponents()
		{
			_adBridgePlatformData = GetAdBridgePlatformDataForCurrentPlatform();
			adBridgeMethodInvoker = CreateAdBridgeMethodInvoker();
			_adBridgeLogger = new AdBridgeLogger(_adProvider.id, isLoggingEnabled, adBridgeMethodInvoker);
			adBridgeCallbackReceiver = CreateAdBridgeCallbackReceiver();
			TAdBridgeMethodInvoker val = adBridgeMethodInvoker;
			val.adBridgeLogger = _adBridgeLogger;
		}

		private TAdBridgeCallbackReceiver CreateAdBridgeCallbackReceiver()
		{
			return AdBridgeCallbackReceiver.CreateAdBridgeCallbackReceiver<TAdBridgeCallbackReceiver>(_adBridgePlatformData.callbackReceiverName, _adBridgeLogger, _adProvider);
		}

		private TAdBridgeMethodInvoker CreateAdBridgeMethodInvoker()
		{
			TAdBridgeMethodInvoker result = Activator.CreateInstance<TAdBridgeMethodInvoker>();
			result.Initialize(_adBridgePlatformData.classPath, _adProvider);
			return result;
		}

		private AdBridgePlatformData GetAdBridgePlatformDataForCurrentPlatform()
		{
			RuntimePlatform platform = Application.platform;
			if (adBridgePlatformDataEntries.ContainsKey(platform))
			{
				return adBridgePlatformDataEntries[platform];
			}
			_adBridgeLogger.LogError("Can't find ad bridge platform data for the current platform: " + platform);
			return null;
		}

		public void SubscribeAdListenerToAdBridgeEvents(IAdEventListener eventListener, string providerTier, Func<AdCacheSlotInfo> getAdCacheSlot, Func<AdLocationInfo> getAdLocation)
		{
			TAdBridgeCallbackReceiver val = adBridgeCallbackReceiver;
			val.SubscribeAdListenerToAdBridgeEvents(eventListener, providerTier, getAdCacheSlot, getAdLocation);
		}

		protected void Log(string message)
		{
			_adBridgeLogger.Log(message);
		}
	}
}
