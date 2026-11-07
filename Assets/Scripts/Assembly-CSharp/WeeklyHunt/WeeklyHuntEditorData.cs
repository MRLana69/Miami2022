using System;
using System.Collections.Generic;
using Kiloo.Common.Utility;
using SYBO.Components.Utilities;
using UnityEngine;

namespace WeeklyHunt
{
	public class WeeklyHuntEditorData : ScriptableObject, ISerializationCallbackReceiver
	{
		[Serializable]
		public class PeriodData
		{
			public bool twoWeekPeriod;

			public int itemsToCollect;

			[NonSerialized]
			public int itemsCollected;

			public bool hiddenUntilUnlocked;

			public string periodTitleSuffix;

			public DateTime periodStartDate;

			public DateTime periodEndDate;

			public long serializedPeriodStartDate;

			public long serializedPeriodEndDate;

			public WeeklyHuntData.PrizeData mainPrize;

			public WeeklyHuntData.PrizeData alternatePrize;

			public bool hasAlternatePrize;

			public string GetPeriodTitle()
			{
				return string.Format(Strings.Get("WEEKLY_HUNT_PERIOD_TITLE"), periodTitleSuffix);
			}
		}

		private static WeeklyHuntEditorData _instance;

		public PeriodData[] periodData;

		public static WeeklyHuntEditorData Instance
		{
			get
			{
				if (!_instance)
				{
					_instance = (WeeklyHuntEditorData)Resources.Load("WeeklyHuntEditorData");
				}
				return _instance;
			}
		}

		public Dictionary<int, WeeklyHuntData.WeeklyHuntPeriodData> GetPeriodData()
		{
			SerializableDictionary<int, WeeklyHuntData.WeeklyHuntPeriodData> serializableDictionary = new SerializableDictionary<int, WeeklyHuntData.WeeklyHuntPeriodData>();
			serializableDictionary.Add(1, new WeeklyHuntData.WeeklyHuntPeriodData
			{
				itemsToCollect = periodData[0].itemsToCollect,
				itemsCollected = periodData[0].itemsCollected,
				hiddenUntilUnlocked = periodData[0].hiddenUntilUnlocked,
				periodTitle = periodData[0].GetPeriodTitle(),
				periodStartDate = periodData[0].periodStartDate,
				periodEndDate = periodData[0].periodEndDate,
				mainPrize = periodData[0].mainPrize,
				hasAlternativePrize = periodData[0].hasAlternatePrize,
				alternativePrize = periodData[0].alternatePrize
			});
			serializableDictionary.Add(2, new WeeklyHuntData.WeeklyHuntPeriodData
			{
				itemsToCollect = periodData[1].itemsToCollect,
				itemsCollected = periodData[1].itemsCollected,
				hiddenUntilUnlocked = periodData[1].hiddenUntilUnlocked,
				periodTitle = periodData[1].GetPeriodTitle(),
				periodStartDate = periodData[1].periodStartDate,
				periodEndDate = periodData[1].periodEndDate,
				mainPrize = periodData[1].mainPrize,
				hasAlternativePrize = periodData[1].hasAlternatePrize,
				alternativePrize = periodData[1].alternatePrize
			});
			serializableDictionary.Add(3, new WeeklyHuntData.WeeklyHuntPeriodData
			{
				itemsToCollect = periodData[2].itemsToCollect,
				itemsCollected = periodData[2].itemsCollected,
				hiddenUntilUnlocked = periodData[2].hiddenUntilUnlocked,
				periodTitle = periodData[2].GetPeriodTitle(),
				periodStartDate = periodData[2].periodStartDate,
				periodEndDate = periodData[2].periodEndDate,
				mainPrize = periodData[2].mainPrize,
				hasAlternativePrize = periodData[2].hasAlternatePrize,
				alternativePrize = periodData[2].alternatePrize
			});
			return serializableDictionary;
		}

		public void OnBeforeSerialize()
		{
			PeriodData[] array = this.periodData;
			foreach (PeriodData periodData in array)
			{
				periodData.serializedPeriodStartDate = periodData.periodStartDate.ToUnixTime();
				periodData.serializedPeriodEndDate = periodData.periodEndDate.ToUnixTime();
			}
		}

		public void OnAfterDeserialize()
		{
			PeriodData[] array = this.periodData;
			foreach (PeriodData periodData in array)
			{
				periodData.periodStartDate = TimeUtility.FromUnixTime(periodData.serializedPeriodStartDate);
				periodData.periodEndDate = TimeUtility.FromUnixTime(periodData.serializedPeriodEndDate);
			}
		}
	}
}
