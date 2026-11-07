using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using AdHandling.AdConstants;
using Data.Save;
using Info;
using Kiloo;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Serialization;
using Kiloo.Common.Utility;
using Kiloo.Monetization.SupersonicPlugin;
using UI.SlideIns;
using UnityEngine;

public class OfferWallManager
{
	private enum RequestStates
	{
		NotStarted = 0,
		InProgress = 1,
		Succeeded = 2,
		Failed = 3
	}

	private const string OFFERWALL_DISABLED_ONLINE_SETTING = "offerwall_disabled";

	private const bool DEFAULT_DISABLED_ONLINE_SETTING = false;

	private const int DEFAULT_REWARDS_REFRESH_INTERVAL_AFTER_FAIL = 30;

	private const int DEFAULT_REWARDS_REFRESH_INTERVAL_AFTER_SUCCESS = 120;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action onOfferWallReceived__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<int> onPendingRewardsReceived__BackingField;

	private static OfferWallManager _instance;

	private static bool _isOfferwallActive;

	private RequestStates _requestCreditsState;

	private DateTime _lastTimeRewardsWereRequested = DateTime.MinValue;

	private string _lastOriginForShow = string.Empty;

	private bool _listenersSet;

	private int MINIMUM_SUPPORTED_ANDROID_API = 17;

	private string CURRENT_OFFERWALL_PROVIDER = "supersonic";

	public static bool isOfferwallActive
	{
		get
		{
			if (_isOfferwallActive)
			{
				_isOfferwallActive = false;
				return true;
			}
			return false;
		}
		private set
		{
			_isOfferwallActive = value;
		}
	}

	public static OfferWallManager instance
	{
		get
		{
			return _instance ?? (_instance = new OfferWallManager());
		}
	}

	private bool isOfferWallDisabled
	{
		get
		{
			return OnlineSettings.instance.GetValue("offerwall_disabled", false);
		}
	}

	public event Action onOfferWallReceived
	{
		add
		{
			Action action = onOfferWallReceived__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onOfferWallReceived__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onOfferWallReceived__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onOfferWallReceived__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action<int> onPendingRewardsReceived
	{
		add
		{
			Action<int> action = onPendingRewardsReceived__BackingField;
			Action<int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onPendingRewardsReceived__BackingField, (Action<int>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<int> action = onPendingRewardsReceived__BackingField;
			Action<int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onPendingRewardsReceived__BackingField, (Action<int>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private string GetSDKAppID()
	{
		return AdConstants.instance.SupersonicAppId;
	}

	private bool HasTimeIntervalPassedSinceDate(DateTime lastRequestDateTime, int timeInterval)
	{
		double totalSeconds = CDateTime.Now.Subtract(lastRequestDateTime).TotalSeconds;
		return (double)timeInterval < totalSeconds;
	}

	public void Initialize()
	{
		if (AgeRestriction.waitForAgeRestriction)
		{
			AgeRestriction.restrictionChanged += OnRestrictionChanged;
		}
		else if (IsOfferWallAllowed() && !Supersonic.IsInitialized())
		{
			SetListeners();
			Supersonic.Init(GetSDKAppID(), DeviceUtility.GetDeviceIdentifier());
		}
	}

	private void OnRestrictionChanged()
	{
		if (!AgeRestriction.waitForAgeRestriction)
		{
			AgeRestriction.restrictionChanged -= OnRestrictionChanged;
			Initialize();
		}
	}

	private void SetListeners()
	{
		if (!_listenersSet)
		{
			Supersonic.onOfferwallAvailable += OnOfferWallAvailable;
			Supersonic.onOfferwallOpened += OnViewDidAppear;
			Supersonic.onOfferwallShowFailed += OnViewShowFailed;
			Supersonic.onOfferwallAdCredited += OnOfferWallAdCredited;
			Supersonic.onGetOfferwallCreditsFailed += OnGetOfferWallCreditsFailed;
			Supersonic.onOfferwallClosed += OnViewDidDisappear;
			_listenersSet = true;
		}
	}

	public bool IsOfferWallAllowed()
	{
		return Application.platform == RuntimePlatform.Android && !DeviceUtility.isKindleDevice && DeviceUtility.GetAndroidApiLevel() >= MINIMUM_SUPPORTED_ANDROID_API && !isOfferWallDisabled;
	}

	public void ShowOfferWall(string origin)
	{
		if (IsOfferWallAvailable())
		{
			_lastOriginForShow = origin;
			Supersonic.ShowOfferwall();
			isOfferwallActive = true;
		}
	}

	public bool IsOfferWallAvailable()
	{
		if (!Supersonic.IsInitialized())
		{
			return false;
		}
		bool result = Supersonic.IsOfferwallAvailable();
		Flurry.LogEventWithAParameter("OfferWall Available", "Result", result.ToString());
		return result;
	}

	public void GetPendingRewards()
	{
		if (Supersonic.IsInitialized())
		{
			int timeInterval = ((_requestCreditsState != RequestStates.Succeeded) ? 30 : 120);
			if (HasTimeIntervalPassedSinceDate(_lastTimeRewardsWereRequested, timeInterval) && _requestCreditsState != RequestStates.InProgress)
			{
				_lastTimeRewardsWereRequested = DateTime.Now;
				_requestCreditsState = RequestStates.InProgress;
				Supersonic.GetOfferwallCredits();
			}
		}
	}

	private void PayoutRewardToUser(int rewardAmount)
	{
		if (rewardAmount > 0)
		{
			onPendingRewardsReceived__BackingField.SafeInvoke(rewardAmount);
			InventorySave.IncreaseCoins(rewardAmount, "Offerwall");
			UserSave.IncrementOfferWallCoinsPaidOut(CURRENT_OFFERWALL_PROVIDER, rewardAmount);
			if (UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.OfferWall,
					stringValue = rewardAmount.ToString(),
					purchase = false
				});
			}
			SaveManager.Save();
		}
	}

	private void OnOfferWallAvailable(string message)
	{
		bool result;
		bool.TryParse(message, out result);
		Flurry.LogEventWithAParameter("OfferWall Available", "Result", result.ToString());
		if (result)
		{
			onOfferWallReceived__BackingField.SafeInvoke();
		}
	}

	private void OnViewDidAppear()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Origin", _lastOriginForShow);
		dictionary.Add("Result", "Shown");
		Dictionary<string, string> parameters = dictionary;
		Flurry.LogEventWithSeveralParameters("OfferWall Shown", parameters);
		isOfferwallActive = true;
	}

	private void OnViewShowFailed(string error)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Origin", _lastOriginForShow);
		dictionary.Add("Result", "Not shown");
		Dictionary<string, string> parameters = dictionary;
		Flurry.LogEventWithSeveralParameters("OfferWall Shown", parameters);
		isOfferwallActive = false;
	}

	private void OnOfferWallAdCredited(string message)
	{
		int num = 0;
		try
		{
			KilooSerializer serializer = new KilooSerializer();
			Dictionary<string, object> dictionary = serializer.FromJson<Dictionary<string, object>>(message);
			foreach (string key in dictionary.Keys)
			{
			}
			if (!dictionary.ContainsKey("totalCreditsFlag") || !dictionary.ContainsKey("credits") || !dictionary.ContainsKey("totalCredits"))
			{
				throw new Exception("Missing parameters in payload");
			}
			bool flag = Convert.ToBoolean(dictionary["totalCreditsFlag"]);
			int num2 = Convert.ToInt32(dictionary["credits"]);
			int num3 = Convert.ToInt32(dictionary["totalCredits"]);
			int offerWallCoinsPaidOut = UserSave.GetOfferWallCoinsPaidOut(CURRENT_OFFERWALL_PROVIDER);
			if (num2 > 0 && !flag)
			{
				num = num2;
			}
			if (num == 0 && offerWallCoinsPaidOut < num3)
			{
				num = num3 - offerWallCoinsPaidOut;
			}
		}
		catch (Exception ex)
		{
			LogError(ex.Message);
		}
		if (num > 0)
		{
			Flurry.LogEventWithAParameter("OfferWall Payout", "Result", num.ToString());
			PayoutRewardToUser(num);
		}
		_requestCreditsState = RequestStates.Succeeded;
	}

	private void OnViewDidDisappear()
	{
		Flurry.LogEventWithAParameter("OfferWall Closed", "Origin", _lastOriginForShow);
		GetPendingRewards();
		isOfferwallActive = false;
	}

	private void OnGetOfferWallCreditsFailed(string error)
	{
		_requestCreditsState = RequestStates.Failed;
	}

	[Conditional("ENABLE_OFFERWALL_LOGS")]
	private void Log(string message)
	{
	}

	private void LogError(string message)
	{
		SDebug.LogError("[OfferWallManager] " + message + " [FrameCount] " + Time.frameCount);
	}
}
