using System;
using System.Linq;
using Cloud;
using Cloud.Internal;
using Data.Save;
using NaughtyAttributes;
using UI.Generic;
using UnityEngine;
using WeeklyHunt;

namespace UI.Breadcrumbs
{
	public class Breadcrumb : MonoBehaviour
	{
		public enum BreadcrumbEvent
		{
			WordHunt = 0,
			WeeklyHunt = 1,
			MissionSet = 2,
			CloudSaveError = 3,
			CloudSaveConflict = 4,
			CloudSaveOk = 5
		}

		[Serializable]
		public class BreadcrumbSave
		{
			public string lastDailyWord = string.Empty;

			public long weeklyHuntPeriodExpireDateTicks = DateTime.MinValue.Ticks;

			public int lastMissionSet = -1;
		}

		[Header("Events to listen to")]
		[SerializeField]
		private bool _newMissionSet;

		[SerializeField]
		private bool _newWeeklyHunt;

		[SerializeField]
		private bool _newWordHunt;

		[SerializeField]
		private bool _cloudSaveError;

		[SerializeField]
		private bool _cloudSaveConflict;

		[SerializeField]
		private bool _cloudSaveOk;

		[SerializeField]
		private UISprite _normalBackground;

		[SerializeField]
		private UISprite _exclamationMark;

		[SerializeField]
		private UILabel _number;

		private void Start()
		{
			CheckIfListening();
			_normalBackground.spriteName = string.Empty;
		}

		public void EnableListeners(BreadcrumbEvent[] events)
		{
			if (events == null)
			{
				base.gameObject.SetActive(false);
				return;
			}
			_newWordHunt = events.Contains(BreadcrumbEvent.WordHunt);
			_newWeeklyHunt = events.Contains(BreadcrumbEvent.WeeklyHunt);
			_newMissionSet = events.Contains(BreadcrumbEvent.MissionSet);
			_cloudSaveConflict = events.Contains(BreadcrumbEvent.CloudSaveConflict);
			_cloudSaveError = events.Contains(BreadcrumbEvent.CloudSaveError);
			_cloudSaveOk = events.Contains(BreadcrumbEvent.CloudSaveOk);
			CheckIfListening();
		}

		private void SetupNormalBreadcrumb(int breadcrumbNumber)
		{
			_normalBackground.spriteName = "notification_counter_sticker";
			_normalBackground.gameObject.SetActive(breadcrumbNumber > 0);
			_exclamationMark.gameObject.SetActive(breadcrumbNumber == 1);
			_number.gameObject.SetActive(breadcrumbNumber > 1);
			_number.text = breadcrumbNumber.ToString();
			_normalBackground.MakePixelPerfect();
		}

		private void SetupSpecialBreadcrumb(string spriteName)
		{
			_normalBackground.spriteName = spriteName;
			_normalBackground.gameObject.SetActive(true);
			_exclamationMark.gameObject.SetActive(false);
			_number.gameObject.SetActive(false);
			GetComponent<FitSpriteToWidget>().FitNow();
		}

		private void CheckIfListening()
		{
			bool active = _newWordHunt || _newWeeklyHunt || _newMissionSet || _cloudSaveConflict || _cloudSaveError || _cloudSaveOk;
			base.gameObject.SetActive(active);
		}

		private void Update()
		{
			CheckListeners();
		}

		private void CheckListeners()
		{
			if (_cloudSaveConflict && CheckCloudSaveConflict())
			{
				SetupSpecialBreadcrumb("ic_brdrcmb_conflict");
				return;
			}
			if (_cloudSaveError && CheckCloudSaveError())
			{
				SetupSpecialBreadcrumb("ic_brdrcmb_error");
				return;
			}
			if (_cloudSaveOk && CheckCloudSaveActive())
			{
				SetupSpecialBreadcrumb("ic_check");
				return;
			}
			int num = 0;
			if (_newMissionSet && CheckNewMissionSet())
			{
				num++;
			}
			if (_newWeeklyHunt && CheckNewWeeklyHunt())
			{
				num++;
			}
			if (_newWordHunt && CheckNewWordhunt())
			{
				num++;
			}
			SetupNormalBreadcrumb(num);
		}

		private bool CheckCloudSaveActive()
		{
			return CloudSave.state == CloudSaveState.Active;
		}

		private bool CheckCloudSaveConflict()
		{
			return CloudSave.state == CloudSaveState.Conflict;
		}

		private bool CheckCloudSaveError()
		{
			return CloudSave.state == CloudSaveState.Error;
		}

		private bool CheckNewMissionSet()
		{
			return PlayerSave.missionSetCompletedCount != UserSave.breadcrumbs.lastMissionSet;
		}

		private static bool CheckNewWeeklyHunt()
		{
			if (WeeklyHuntManager.IsHuntStillActive())
			{
				long ticks = WeeklyHuntManager.GetEndTimeForActivePeriod().Ticks;
				return ticks != UserSave.breadcrumbs.weeklyHuntPeriodExpireDateTicks;
			}
			return false;
		}

		private static bool CheckNewWordhunt()
		{
			string wordHuntDailyWord = EventsSave.wordHuntDailyWord;
			return wordHuntDailyWord != UserSave.breadcrumbs.lastDailyWord;
		}

		[Button(null)]
		private void FakeSetupConflictBreadcrumb()
		{
			base.enabled = false;
			SetupSpecialBreadcrumb("ic_brdrcmb_conflict");
		}

		public static void Consume(BreadcrumbEvent e)
		{
			switch (e)
			{
			case BreadcrumbEvent.WordHunt:
				UserSave.breadcrumbs.lastDailyWord = EventsSave.wordHuntDailyWord;
				break;
			case BreadcrumbEvent.WeeklyHunt:
				if (WeeklyHuntManager.IsHuntStillActive())
				{
					UserSave.breadcrumbs.weeklyHuntPeriodExpireDateTicks = WeeklyHuntManager.GetEndTimeForActivePeriod().Ticks;
				}
				break;
			case BreadcrumbEvent.MissionSet:
				UserSave.breadcrumbs.lastMissionSet = PlayerSave.missionSetCompletedCount;
				break;
			}
		}
	}
}
