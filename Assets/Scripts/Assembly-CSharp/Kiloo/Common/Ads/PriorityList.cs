using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace Kiloo.Common.Ads
{
	public class PriorityList
	{
		private int _numberOfFails;

		private int _currentPriority = -1;

		private DateTime _timeOfProviderChange;

		private readonly List<string> _exhaustedProviders = new List<string>();

		private IList<AdProviderInfo> _priorityList;

		private IAdSettingsProvider _settings;

		private AdType _adType;

		private string _waterfallID;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action changedProvider__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action resetToTop__BackingField;

		public AdProviderInfo currentProvider { get; private set; }

		public string currentProviderID
		{
			get
			{
				return (currentProvider == null) ? string.Empty : currentProvider.providerID;
			}
		}

		public string currentProviderTier
		{
			get
			{
				return (currentProvider == null) ? string.Empty : currentProvider.providerTier;
			}
		}

		public bool wrapped { get; set; }

		public bool hasProviders
		{
			get
			{
				return _priorityList != null && _priorityList.Count > 0;
			}
		}

		private int maxPriorityInList
		{
			get
			{
				int num = int.MinValue;
				foreach (AdProviderInfo priority in _priorityList)
				{
					if (priority.priority > num)
					{
						num = priority.priority;
					}
				}
				return num;
			}
		}

		public bool reachedLastProvider
		{
			get
			{
				int maxPriority = maxPriorityInList;
				if (_currentPriority < maxPriority)
				{
					return false;
				}
				int num = _priorityList.Count((AdProviderInfo p) => p.priority == maxPriority && !_exhaustedProviders.Contains(p.providerTier) && p.providerTier != currentProviderTier);
				return num == 0;
			}
		}

		public event Action changedProvider
		{
			add
			{
				Action action = changedProvider__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref changedProvider__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = changedProvider__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref changedProvider__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action resetToTop
		{
			add
			{
				Action action = resetToTop__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref resetToTop__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = resetToTop__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref resetToTop__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public PriorityList(string waterfallID, AdType adType, IAdSettingsProvider settings, AdCacheSlotInfo adCacheSlotInfo)
		{
			_waterfallID = waterfallID;
			_adType = adType;
			_settings = settings;
			_priorityList = settings.GetPriorityList(waterfallID, adType, adCacheSlotInfo);
		}

		public bool IncrementNumberOfFails()
		{
			_numberOfFails++;
			if (_numberOfFails >= _settings.GetFailCountTolerance(currentProviderID, _adType))
			{
				ChangeToNextProvider();
				return true;
			}
			return false;
		}

		public void CheckIfProviderExpired()
		{
			TimeSpan? timeSpan = ((!string.IsNullOrEmpty(currentProviderID)) ? _settings.GetTimeLimitForAdProvider(currentProviderID, _adType) : new TimeSpan?(TimeSpan.Zero));
			if (timeSpan.HasValue && DateTime.UtcNow > _timeOfProviderChange.Add(timeSpan.Value))
			{
				if (!string.IsNullOrEmpty(currentProviderID))
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("Provider", currentProviderID);
					dictionary.Add("Location", _waterfallID);
					dictionary.Add("Type", _adType.ToString());
					Dictionary<string, string> parameters = dictionary;
					Flurry.LogEventWithSeveralParameters("Provider Time Limit Reached", parameters);
				}
				ResetToTop();
			}
		}

		private void MarkCurrentProviderAsExhausted()
		{
			_exhaustedProviders.Add(currentProviderTier);
		}

		private void ClearExhaustedProviders()
		{
			_exhaustedProviders.Clear();
		}

		public void ChangeToNextProvider()
		{
			if (!hasProviders)
			{
				return;
			}
			MarkCurrentProviderAsExhausted();
			AdProviderInfo[] array = _priorityList.Where((AdProviderInfo p) => p.priority == _currentPriority && !_exhaustedProviders.Contains(p.providerTier)).ToArray();
			if (array.Length == 0)
			{
				int num = int.MaxValue;
				foreach (AdProviderInfo priority in _priorityList)
				{
					if (priority.priority > _currentPriority && priority.priority < num)
					{
						num = priority.priority;
					}
				}
				if (num == int.MaxValue)
				{
					ResetToTop();
					return;
				}
				_currentPriority = num;
				array = _priorityList.Where((AdProviderInfo p) => p.priority == _currentPriority).ToArray();
				ClearExhaustedProviders();
			}
			ChooseProvider(array);
		}

		public void ResetToTop()
		{
			bool flag = _currentPriority == -1;
			ClearExhaustedProviders();
			_currentPriority = int.MaxValue;
			foreach (AdProviderInfo priority in _priorityList)
			{
				if (priority.priority < _currentPriority)
				{
					_currentPriority = priority.priority;
				}
			}
			ChooseProvider(_priorityList.Where((AdProviderInfo p) => p.priority == _currentPriority).ToArray());
			if (!flag)
			{
				wrapped = true;
				resetToTop__BackingField.SafeInvoke();
			}
		}

		public List<AdProviderInfo> GetProviderList()
		{
			return _priorityList.ToList();
		}

		private void ChooseProvider(IList<AdProviderInfo> possibleProviders)
		{
			string text = currentProviderTier;
			if (possibleProviders.Count == 1)
			{
				currentProvider = possibleProviders[0];
			}
			else
			{
				float num = 0f;
				foreach (AdProviderInfo possibleProvider in possibleProviders)
				{
					num += possibleProvider.weight;
				}
				float num2 = UnityEngine.Random.value * num;
				float num3 = 0f;
				foreach (AdProviderInfo possibleProvider2 in possibleProviders)
				{
					num3 += possibleProvider2.weight;
					if (num2 < num3)
					{
						currentProvider = possibleProvider2;
						break;
					}
				}
			}
			_timeOfProviderChange = DateTime.UtcNow;
			_numberOfFails = 0;
			changedProvider__BackingField.SafeInvoke();
		}
	}
}
