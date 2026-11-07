using System;
using System.Collections.Generic;
using System.Text;
using Data.Save;
using InApp;
using SYBO.Components.Json;
using SYBO.Components.Logging;
using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class AggregatedDataTracking : SingletonMonoBehaviour<AggregatedDataTracking>
	{
		[Serializable]
		public class DataContainer
		{
			public int KeysReceived;

			public int CoinsReceived;

			public int LastDayPlayed;

			public int UniqueDaysPlayed;

			public int SessionCount;

			public int MissingKeysCount;

			public int MissingCoinsCount;

			[SerializeField]
			private List<UnsampledPurchaseData> _unsampledPurchases;

			[SerializeField]
			private DataCompleteness _dataCompleteness;

			public List<UnsampledPurchaseData> UnsampledPurchases
			{
				get
				{
					if (_unsampledPurchases == null)
					{
						_unsampledPurchases = new List<UnsampledPurchaseData>();
					}
					return _unsampledPurchases;
				}
			}

			public DataCompleteness DataCompleteness
			{
				get
				{
					return _dataCompleteness;
				}
			}

			public DataContainer(DataCompleteness dataCompleteness, int initialKeysAmount = 0)
			{
				_dataCompleteness = dataCompleteness;
				KeysReceived = initialKeysAmount;
			}

			public void AddUnsampledPurchase(string itemTitleId)
			{
				foreach (UnsampledPurchaseData unsampledPurchase in UnsampledPurchases)
				{
					if (unsampledPurchase.ItemTitleId.Equals(itemTitleId))
					{
						unsampledPurchase.Count++;
						return;
					}
				}
				_unsampledPurchases.Add(new UnsampledPurchaseData
				{
					ItemTitleId = itemTitleId,
					Count = 1
				});
			}

			public static string Serialize(DataContainer dataContainer)
			{
				string s = JsonApi.Serialize(dataContainer);
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				return Convert.ToBase64String(bytes);
			}

			public static DataContainer Deserialize(string dataString)
			{
				byte[] bytes = Convert.FromBase64String(dataString);
				string json = Encoding.UTF8.GetString(bytes);
				return JsonApi.Deserialize<DataContainer>(json);
			}
		}

		[Serializable]
		public class UnsampledPurchaseData
		{
			public string ItemTitleId;

			public int Count;
		}

		public enum DataCompleteness
		{
			Undefined = 0,
			Full = 1,
			Partial = 2,
			None = 3
		}

		private const int MaxDaysInYear = 366;

		private bool _initialized;

		private bool _stoppedTracking;

		[SerializeField]
		private DataContainer _dataContainer;

		private int _currentKeysAmount;

		private int _currentCoinsAmount;

		private SessionTracker _sessionTracker;

		public void Initialize(bool hadUserInPlayerInfo, int sessionTimeoutSeconds)
		{
			if (!_initialized)
			{
				_initialized = true;
				_sessionTracker = new SessionTracker(sessionTimeoutSeconds);
				_sessionTracker.RegisterLastActiveMsTicks();
				Load(hadUserInPlayerInfo);
				_currentKeysAmount = InventorySave.keys;
				_currentCoinsAmount = InventorySave.coins;
				InventorySave.keysChanged += PlayerInfo_OnKeysChanged;
				InventorySave.coinsChanged += PlayerInfo_OnCoinsChanged;
				CheckForNewUniqueDay();
				IncrementSession();
				UIScreenController.OnAwake += UIScreenController_OnAwake;
			}
		}

		public void StopAggregatedDataTracking()
		{
			UIScreenController.OnPopupShown -= UIScreenController_OnPopupShown;
			SyboAnalyticsSave.syboAggregatedDataString = string.Empty;
			UnityEngine.Object.Destroy(base.gameObject);
			_stoppedTracking = true;
		}

		private void OnApplicationFocus(bool focusStatus)
		{
			if (focusStatus)
			{
				if (_sessionTracker.ShouldStartNewSession())
				{
					IncrementSession();
				}
			}
			else
			{
				_sessionTracker.RegisterLastActiveMsTicks();
			}
		}

		private void Load(bool hadUserInPlayerInfo)
		{
			string syboAggregatedDataString = SyboAnalyticsSave.syboAggregatedDataString;
			if (!string.IsNullOrEmpty(syboAggregatedDataString))
			{
				try
				{
					_dataContainer = DataContainer.Deserialize(syboAggregatedDataString);
					return;
				}
				catch (Exception arg)
				{
					Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Error parsing persisted aggregated data: {0}. Resetting data.", Time.time, arg);
					_dataContainer = new DataContainer(DataCompleteness.Partial);
					Save();
					return;
				}
			}
			DataCompleteness dataCompleteness = ((!hadUserInPlayerInfo) ? DataCompleteness.Full : DataCompleteness.Partial);
			int initialKeysAmount = ((!hadUserInPlayerInfo) ? 5 : 0);
			_dataContainer = new DataContainer(dataCompleteness, initialKeysAmount);
			Save();
		}

		private void Save()
		{
			SyboAnalyticsSave.syboAggregatedDataString = DataContainer.Serialize(_dataContainer);
		}

		private void PlayerInfo_OnKeysChanged()
		{
			int keys = InventorySave.keys;
			if (keys > _currentKeysAmount)
			{
				_dataContainer.KeysReceived += keys - _currentKeysAmount;
				Save();
			}
			_currentKeysAmount = keys;
		}

		private void PlayerInfo_OnCoinsChanged()
		{
			int coins = InventorySave.coins;
			if (coins > _currentCoinsAmount)
			{
				_dataContainer.CoinsReceived += coins - _currentCoinsAmount;
				Save();
			}
			_currentCoinsAmount = coins;
		}

		private void CheckForNewUniqueDay()
		{
			int num = ConvertDateTimeToDay(DateTime.Now);
			if (num > _dataContainer.LastDayPlayed)
			{
				_dataContainer.UniqueDaysPlayed++;
				_dataContainer.LastDayPlayed = num;
				Save();
			}
		}

		private int ConvertDateTimeToDay(DateTime dateTime)
		{
			return dateTime.Year * 366 + dateTime.DayOfYear;
		}

		private void IncrementSession()
		{
			_dataContainer.SessionCount++;
			Save();
		}

		private void UIScreenController_OnAwake()
		{
			if (!_stoppedTracking)
			{
				UIScreenController.OnPopupShown += UIScreenController_OnPopupShown;
				PurchaseHandler.Instance.OnIngameCurrencyPurchaseCompleted += PurchaseHandler_OnIngameCurrencyPurchaseCompleted;
			}
			UIScreenController.OnAwake -= UIScreenController_OnAwake;
		}

		private void UIScreenController_OnPopupShown(string screenName)
		{
			if (screenName.Equals("NotEnoughCurrencyPopup"))
			{
				if (InAppManager.instance.GetPopupData().isCoinsPack)
				{
					_dataContainer.MissingCoinsCount++;
				}
				else
				{
					_dataContainer.MissingKeysCount++;
				}
				Save();
			}
		}

		private void PurchaseHandler_OnIngameCurrencyPurchaseCompleted(object sender, PurchaseHandler.SyboInGameCurrencyPurchaseEventArgs e)
		{
			_dataContainer.AddUnsampledPurchase(e.ItemTitleId);
			Save();
		}
	}
}
