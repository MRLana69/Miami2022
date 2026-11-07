using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data.Save;
using JetBrains.Annotations;
using Kiloo;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using Kiloo.Common.Utility;
using Missions;
using NaughtyAttributes;
using SYBO.Subway;
using TopRun;
using UI.Breadcrumbs;
using UI.Generic;
using UI.Generic.Layout;
using UI.Pages.MissionsPageInternal;
using UI.Popups;
using UI.Screens.PauseScreenInternal;
using UnityEngine;
using Utilities;
using WordHunt;

namespace UI.Screens
{
	public class PauseScreen : UIScreen
	{
		[SerializeField]
		private UIWidget _guideWidget;

		[Header("Daily Challenge")]
		[SerializeField]
		private GameObject _dailyChallenge;

		[SerializeField]
		private UILabel _dailyChallengeMainTitle;

		[SerializeField]
		private UILabel _dailyChallengeSubTitle;

		[SerializeField]
		private UIWidget _slotForDailyChallengeItemLoad;

		[SerializeField]
		private DailyChallengeRewardHelper _dailyChallengeRewardHelper;

		[SerializeField]
		private TimerHelper _timerPrefab;

		private TimerHelper _timer;

		[Header("Top Run")]
		[SerializeField]
		private GameObject _topRun;

		[SerializeField]
		private UILabel _topRunMainTitle;

		[SerializeField]
		private UILabel _topRunSubTitle;

		[SerializeField]
		private UILabel _topRunWeekLabel;

		[SerializeField]
		private UILabel _topRunDaysLeftLabel;

		[SerializeField]
		private UISprite _countryFlag;

		[SerializeField]
		private Layout _topRunLayot;

		private bool _topRunReady;

		[SerializeField]
		private GameObject _topRunMedalGuideSprite;

		[SerializeField]
		private GameObject _diamondMedalPrefab;

		[SerializeField]
		private GameObject _goldMedalPrefab;

		[SerializeField]
		private GameObject _silverMedalPrefab;

		[SerializeField]
		private GameObject _bronzeMedalPrefab;

		private readonly GameObject[] _topRunMedals = new GameObject[4];

		private const float MODEL_SCALE = 0.75f;

		private static readonly Vector3 DIAMOND_POSITION = new Vector3(-50f, 30f, 0f) * 0.75f;

		private static readonly Vector3 DIAMOND_ROTATION = new Vector3(0f, -180f, -15f);

		private static readonly Vector3 DIAMOND_SCALE = new Vector3(13.8f, 13.8f, 13.8f) * 0.75f;

		private static readonly Vector3 GOLD_POSITION = new Vector3(2f, 32f, 12f) * 0.75f;

		private static readonly Vector3 GOLD_ROTATION = new Vector3(0f, 180f, 13f);

		private static readonly Vector3 GOLD_SCALE = new Vector3(11f, 11f, 11f) * 0.75f;

		private static readonly Vector3 SILVER_POSITION = new Vector3(44f, 20f, 24f) * 0.75f;

		private static readonly Vector3 SILVER_ROTATION = new Vector3(0f, 180f, 22f);

		private static readonly Vector3 SILVER_SCALE = new Vector3(9.5f, 9.5f, 9.5f) * 0.75f;

		private static readonly Vector3 BRONZE_POSITION = new Vector3(72f, -5f, 36f) * 0.75f;

		private static readonly Vector3 BRONZE_ROTATION = new Vector3(0f, 180f, 35f);

		private static readonly Vector3 BRONZE_SCALE = new Vector3(7.6f, 7.6f, 7.6f) * 0.75f;

		[Header("Missions")]
		[SerializeField]
		private UILabel _missionsMainTitle;

		[SerializeField]
		private UILabel _missionSubTitle;

		[SerializeField]
		private UIWidget _slotForMissionItemLoad;

		[SerializeField]
		private GameObject _missionCellPrefab;

		[SerializeField]
		private MissionSetProgress _missionSetProgressPrefab;

		private MissionSetProgress _missionSetProgress;

		[SerializeField]
		private Layout[] _missionCellLayouts;

		private int _missionCellHeight;

		private float _cellAnimationDuration = 1f;

		private float _cellAnimationDelay = 0.1f;

		private int _currentSet;

		private const int MAX_MULTIPLIER = 30;

		[Header("Footer")]
		[SerializeField]
		private UIWidget _footerGuide;

		[SerializeField]
		private ResumeButtonHelper _resumeButtonHelper;

		[SerializeField]
		private UILabel _homeButtonLabel;

		[SerializeField]
		private UILabel _settingsButtonLabel;

		[SerializeField]
		private UILabel _resumeButtonLabel;

		public static bool pauseScreenIsShown = false;

		private const int NUMBER_OF_MISSIONS_IN_SET = 3;

		private MissionInfo[][] _missionInfos = new MissionInfo[2][];

		private List<MissionCell>[] _missionCells = new List<MissionCell>[2];

		private int GetCurrentSetIndex()
		{
			return _currentSet % 2;
		}

		private int GetOtherSetIndex()
		{
			return (_currentSet + 1) % 2;
		}

		protected override void Init()
		{
			base.Init();
			_dailyChallengeMainTitle.text = Strings.Get("DAILY_CHALLENGE_WORD_HUNT");
			_dailyChallengeSubTitle.text = Strings.Get("DAILY_CHALLENGE_COLLECT").ToUpper();
			_dailyChallengeMainTitle.color = ColorData.instance.midnight;
			_dailyChallengeSubTitle.color = ColorData.instance.peacock;
			_missionsMainTitle.text = Strings.Get("MISSIONS_TITLE");
			_missionsMainTitle.color = ColorData.instance.midnight;
			_missionSubTitle.color = ColorData.instance.peacock;
			_homeButtonLabel.text = Strings.Get("FOOTER_HOME_BUTTON_TITLE");
			_settingsButtonLabel.text = Strings.Get("SETTINGS_TITLE");
			_resumeButtonLabel.text = Strings.Get("PAUSE_UI_RESUME");
			_topRunMainTitle.text = Strings.Get("TOPRUN_TITLE");
			_topRunMainTitle.color = ColorData.instance.midnight;
			_topRunSubTitle.color = ColorData.instance.peacock;
			TimerHelper timerHelper = UnityEngine.Object.Instantiate(_timerPrefab, _slotForDailyChallengeItemLoad.transform);
			timerHelper.StartTimer(EventsSave.wordHuntExpireTime);
			timerHelper.SetTimerColors(Color.white, ColorData.instance.lime);
			timerHelper.SetAlignment(HorizontalAlignment.Right);
			timerHelper.transform.localPosition = Vector3.zero;
			_timer = timerHelper;
			if (SafeArea.IsRestricted)
			{
				_guideWidget.topAnchor.absolute = -SafeArea.OFFSET_TOP;
				_guideWidget.bottomAnchor.absolute = SafeArea.OFFSET_BOTTOM;
				_guideWidget.ResetAndUpdateAnchors();
			}
			_missionSetProgress = UnityEngine.Object.Instantiate(_missionSetProgressPrefab, _slotForMissionItemLoad.transform);
			UIWidget component = _missionCellLayouts[GetCurrentSetIndex()].GetComponent<UIWidget>();
			component.ResetAndUpdateAnchors();
			_missionCellHeight = CalculateCellHeight(component);
			RefreshMissionInfos();
			CreateNewMissionCells();
		}

		protected override void Show()
		{
			base.Show();
			if (Game.Instance != null)
			{
				Game.Instance.TriggerPause(true);
			}
			WordHuntManager.CheckIfWeShouldRemoveProgressForDailyQuestInRow();
			RefreshDailyChallenge();
			RefreshTopRun();
			SelectTopSection();
			RefreshMissionInfos();
			RefreshCurrentCells();
			ResetCellPositions();
			DestroyOtherMissionCells();
			RefreshMissionSubtitle();
			_missionSetProgress.SetMissionSetProgress(_missionInfos[GetCurrentSetIndex()].Count((MissionInfo m) => m.complete));
			if (MissionsManager.Instance.currentMissionSet >= 30)
			{
				_missionSetProgress.ChangeToMysteryBoxReward();
			}
			if (_resumeButtonHelper != null)
			{
				_resumeButtonHelper.EnableButton();
			}
			pauseScreenIsShown = true;
			Breadcrumb.Consume(Breadcrumb.BreadcrumbEvent.WordHunt);
			Breadcrumb.Consume(Breadcrumb.BreadcrumbEvent.MissionSet);
			SaveManager.Save();
		}

		protected override void Hide()
		{
			base.Hide();
			pauseScreenIsShown = false;
			Destroy3DMedals();
			_timer.StopTimer();
		}

		public override void BackButtonPressed()
		{
			UIScreenController.Instance.PushScreen("IngameUI");
		}

		private void SelectTopSection()
		{
			bool flag = !WordHuntManager.IsDailyWordComplete() || !_topRunReady;
			_dailyChallenge.SetActive(flag);
			_topRun.SetActive(!flag);
			if (!flag)
			{
				Add3DModels();
			}
			else
			{
				Destroy3DMedals();
			}
		}

		private void RefreshDailyChallenge()
		{
			_dailyChallengeRewardHelper.Refresh();
			_timer.StartTimer(EventsSave.wordHuntTimeLeft);
		}

		private void RefreshTopRun()
		{
			IManager manager = TopRunRoot.manager;
			if (manager != null && manager.tournament != null)
			{
				ITournament tournament = manager.tournament;
				TimeSpan ts = tournament.officialEndTimeUTC - CDateTime.UtcNow;
				_topRunDaysLeftLabel.text = FormatTopRunTimeLeft(ts);
				_topRunWeekLabel.text = string.Format(Strings.Get("TOPRUN_WEEK_TITLE"), tournament.sequenceId);
				_topRunReady = true;
			}
			else
			{
				_topRunReady = false;
			}
			_topRunLayot.Align();
			Country currentTournamentCountry = CountryManager.instance.GetCurrentTournamentCountry();
			_countryFlag.spriteName = currentTournamentCountry.flag;
			_topRunSubTitle.text = Strings.Get(currentTournamentCountry.displayNameID).ToUpper();
		}

		private static string FormatTopRunTimeLeft(TimeSpan ts)
		{
			if (ts.Days > 0)
			{
				return "{0} {1}".Populate(ts.Days, Strings.Get((ts.Days <= 1) ? "TOPRUN_STATE_DAYS_LEFT_SINGLE" : "TOPRUN_STATE_DAYS_LEFT"));
			}
			if (ts.TotalSeconds < 0.0)
			{
				return Strings.Get("TOPRUN_STATE_NEXT_SOON");
			}
			return ts.NicifyTimeString();
		}

		private void Add3DModels()
		{
			_topRunMedalGuideSprite.GetComponent<UISprite>().ResetAndUpdateAnchors();
			Add3DModel(0, _diamondMedalPrefab, DIAMOND_POSITION, DIAMOND_ROTATION, DIAMOND_SCALE);
			Add3DModel(1, _goldMedalPrefab, GOLD_POSITION, GOLD_ROTATION, GOLD_SCALE);
			Add3DModel(2, _silverMedalPrefab, SILVER_POSITION, SILVER_ROTATION, SILVER_SCALE);
			Add3DModel(3, _bronzeMedalPrefab, BRONZE_POSITION, BRONZE_ROTATION, BRONZE_SCALE);
		}

		private void Add3DModel(int index, GameObject prefab, Vector3 position, Vector3 rotation, Vector3 scale)
		{
			if (_topRunMedals[index] == null)
			{
				_topRunMedals[index] = UIModelController.Instance.PauseScreenAnchor.AddChild(prefab);
			}
			_topRunMedals[index].SetActive(true);
			MedalIconPositioning(_topRunMedals[index], position, rotation, scale);
		}

		private void MedalIconPositioning(GameObject medalGameObject, Vector3 positionOffset, Vector3 rotation, Vector3 scale)
		{
			if (medalGameObject != null)
			{
				Vector3 position = _topRunMedalGuideSprite.transform.position;
				Vector3 vector = UIScreenController.Instance.Camera2DNormal.WorldToScreenPoint(position);
				vector = new Vector3(vector.x, vector.y, 3f);
				Vector3 newPosition = UIScreenController.Instance.Camera3D.ScreenToWorldPoint(vector);
				ResetTransform(medalGameObject.transform, newPosition, positionOffset, rotation, scale);
			}
		}

		private void Destroy3DMedals()
		{
			GameObject[] topRunMedals = _topRunMedals;
			foreach (GameObject gameObject in topRunMedals)
			{
				if (gameObject != null)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
		}

		private void ResetTransform(Transform trans, Vector3 newPosition, Vector3 positionOffset, Vector3 newRotation, Vector3 newScale)
		{
			trans.position = newPosition;
			trans.localPosition += positionOffset;
			trans.localRotation = Quaternion.Euler(newRotation);
			trans.localScale = newScale;
		}

		private void RefreshMissionInfos()
		{
			_missionInfos[GetCurrentSetIndex()] = MissionsManager.Instance.GetMissionInfo();
			_missionInfos[GetOtherSetIndex()] = MissionsManager.Instance.GetNextMissionInfos();
		}

		private void CreateNewMissionCells()
		{
			int currentSetIndex = GetCurrentSetIndex();
			MissionInfo[] array = _missionInfos[currentSetIndex];
			_missionCells[currentSetIndex] = new List<MissionCell>();
			for (int i = 0; i < 3; i++)
			{
				MissionCell component = _missionCellLayouts[currentSetIndex].gameObject.AddChild(_missionCellPrefab).GetComponent<MissionCell>();
				component.Init(new MissionCell.MissionCellPayload
				{
					headerHeight = 50,
					cellHeight = _missionCellHeight,
					missionIndex = i,
					missionInfo = array[i],
					source = MissionCell.PayloadSource.PauseMenu,
					onMissionCellAnimationComplete = OnMissionCellAnimationComplete
				});
				_missionCells[currentSetIndex].Add(component);
			}
			_missionCellLayouts[currentSetIndex].Align();
		}

		private int CalculateCellHeight(UIWidget guideWidget)
		{
			int num = 2 * _missionCellLayouts[GetCurrentSetIndex()].padding;
			int height = guideWidget.height;
			float num2 = height - num;
			return (num2 / 3f).RoundToInt();
		}

		private void RefreshCurrentCells()
		{
			for (int i = 0; i < 3; i++)
			{
				_missionCells[GetCurrentSetIndex()][i].RefreshIfNotNewlyCreated(_missionInfos[GetCurrentSetIndex()][i]);
			}
		}

		private void ResetCellPositions()
		{
			_missionCells[GetCurrentSetIndex()].ForEach((MissionCell c) =>
			{
				c.GetComponent<Transform>().SetLocalPosX(0f);
			});
		}

		private void DestroyOtherMissionCells()
		{
			int otherSetIndex = GetOtherSetIndex();
			if (_missionCells[otherSetIndex] == null)
			{
				return;
			}
			_missionCells[otherSetIndex].ForEach((MissionCell cell) =>
			{
				if ((bool)cell)
				{
					UnityEngine.Object.Destroy(cell.gameObject);
				}
			});
		}

		private void RefreshMissionSubtitle()
		{
			_missionSubTitle.text = string.Format(Strings.Get("MISSION_SET"), PlayerSave.missionSetCompletedCount);
		}

		private void OnMissionCellAnimationComplete(int completedMissionsCount)
		{
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(MissionCompleteAnimation(completedMissionsCount));
			}
		}

		private IEnumerator MissionCompleteAnimation(int completedMissionsCount)
		{
			if (completedMissionsCount == 0)
			{
				_missionSetProgress.BounceNextProgressDot();
				yield return new WaitForSecondsRealtime(_missionSetProgress.GetBounceDuration());
				_missionSetProgress.BounceReward();
				yield return new WaitForSecondsRealtime(_missionSetProgress.GetBounceDuration());
				AnimateNewMissionSet();
				yield return new WaitForSecondsRealtime(GetTotalMissionCellAnimationTime());
				RefreshMissionInfos();
				RefreshCurrentCells();
				RefreshMissionSubtitle();
				_missionSetProgress.SetMissionSetProgress(0);
				if (MissionsManager.Instance.currentMissionSet >= MissionsManager.Instance.GetNumberOfBasicMission())
				{
					_missionSetProgress.ChangeToMysteryBoxReward(true);
				}
			}
			else
			{
				_missionSetProgress.BounceNextProgressDot();
			}
		}

		[Button(null)]
		[UsedImplicitly]
		private void AnimateNewMissionSet()
		{
			float inStartPosition = -350f;
			float mainPosition = 0f;
			float outEndPosition = 350f;
			_currentSet++;
			CreateNewMissionCells();
			List<MissionCell> newMissionCells = _missionCells[GetCurrentSetIndex()];
			foreach (MissionCell item in newMissionCells)
			{
				item.transform.SetLocalPosX(inStartPosition);
			}
			StartCoroutine(AnimateCell(newMissionCells[0].transform, inStartPosition, mainPosition));
			Timer.ExecuteWithDelay(_cellAnimationDelay, () =>
			{
				StartCoroutine(AnimateCell(newMissionCells[1].transform, inStartPosition, mainPosition));
			});
			Timer.ExecuteWithDelay(_cellAnimationDelay * 2f, () =>
			{
				StartCoroutine(AnimateCell(newMissionCells[2].transform, inStartPosition, mainPosition));
			});
			List<MissionCell> oldMissionsCells = _missionCells[GetOtherSetIndex()];
			StartCoroutine(AnimateCell(oldMissionsCells[0].transform, mainPosition, outEndPosition));
			Timer.ExecuteWithDelay(_cellAnimationDelay, () =>
			{
				StartCoroutine(AnimateCell(oldMissionsCells[1].transform, mainPosition, outEndPosition));
			});
			Timer.ExecuteWithDelay(_cellAnimationDelay * 2f, () =>
			{
				StartCoroutine(AnimateCell(oldMissionsCells[2].transform, mainPosition, outEndPosition, DestroyOtherMissionCells));
			});
		}

		private IEnumerator AnimateCell(Transform cellTransform, float startX, float endX, Action onComplete = null)
		{
			float currentDuration = 0f;
			while (currentDuration < _cellAnimationDuration)
			{
				currentDuration += Time.unscaledDeltaTime;
				float p = currentDuration / _cellAnimationDuration;
				float plot = Easings.BounceEaseOut(p);
				if (p > 372f / 1023f)
				{
					plot--;
					plot /= 8f;
					plot++;
				}
				cellTransform.SetLocalPosX(Mathf.LerpUnclamped(startX, endX, plot));
				yield return null;
			}
			cellTransform.SetLocalPosX(endX);
			onComplete.SafeInvoke();
		}

		private float GetTotalMissionCellAnimationTime()
		{
			return _cellAnimationDelay * 2f + _cellAnimationDuration;
		}

		private void OnApplicationPause(bool paused)
		{
			if (!paused)
			{
				RefreshDailyChallenge();
			}
		}

		public bool IsWaiting()
		{
			if (_resumeButtonHelper == null)
			{
				return false;
			}
			return !_resumeButtonHelper.isButtonEnabled;
		}

		[UsedImplicitly]
		public void HomeButtonClicked()
		{
			ConfirmationPopup.variant = ConfirmationPopupVariant.CancelRun;
			UIScreenController.Instance.PushPopup("ConfirmationPopup");
		}
	}
}
