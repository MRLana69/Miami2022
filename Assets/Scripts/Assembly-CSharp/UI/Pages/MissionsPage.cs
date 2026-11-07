using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppRefresh;
using Data.Save;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using Kiloo.Common.Utility;
using Missions;
using NaughtyAttributes;
using UI.Breadcrumbs;
using UI.Footer;
using UI.Generic.Layout;
using UI.Pages.MissionsPageInternal;
using UnityEngine;

namespace UI.Pages
{
	public class MissionsPage : Page
	{
		public const int PAGE_INDEX = 1;

		[SerializeField]
		private PageTop _topPrefab;

		private PageTop _top;

		[SerializeField]
		private GameObject _missionCellPrefab;

		[SerializeField]
		private Layout[] _missionCellLayouts;

		[SerializeField]
		private float _cellAnimationDuration = 1f;

		[SerializeField]
		private float _cellAnimationDelay = 0.1f;

		private int _currentSet;

		[SerializeField]
		private MissionSetProgress _missionSetProgressPrefab;

		private MissionSetProgress _missionSetProgress;

		private int _cellHeight;

		private MissionInfo[][] _missionInfos = new MissionInfo[2][];

		private List<MissionCell>[] _missionCells = new List<MissionCell>[2];

		private const int MAX_MULTIPLIER = 30;

		private const int MISSION_COUNT_IN_SET = 3;

		private int _currentMissionSetCompletedSum;

		private bool _missionCompleteAnimationInProgress;

		private PageTop.EventsPageTopPayload _topPayload;

		private int GetCurrentSetIndex()
		{
			return _currentSet % 2;
		}

		private int GetOtherSetIndex()
		{
			return (_currentSet + 1) % 2;
		}

		public override FooterButton.FooterPayload GetButtonPayload()
		{
			ColorData instance = ColorData.instance;
			FooterButton.FooterPayload footerPayload = new FooterButton.FooterPayload();
			footerPayload.width = 72;
			footerPayload.colorScheme = new ButtonColorScheme
			{
				original = instance.myTourFooterColorMain,
				selected = instance.myTourFooterColorSelected,
				light = instance.myTourFooterColorLight
			};
			footerPayload.iconName = "icon_missions";
			footerPayload.title = Strings.Get("MISSIONS_TITLE");
			footerPayload.breadcrumbEvents = new Breadcrumb.BreadcrumbEvent[1] { Breadcrumb.BreadcrumbEvent.MissionSet };
			return footerPayload;
		}

		public override void Init(UIWidget pageGuideWidget)
		{
			_top = UnityEngine.Object.Instantiate(_topPrefab, topContainer.transform);
			_top.transform.localPosition = Vector3.zero;
			ColorData instance = ColorData.instance;
			_topPayload = new PageTop.EventsPageTopPayload
			{
				mainTitle = Strings.Get("MISSIONS_TITLE"),
				subTitle = string.Format(Strings.Get("MISSION_SET"), PlayerSave.missionSetCompletedCount),
				mainColor = instance.midnight,
				subTitleColor = instance.boston,
				splatColor = instance.lime,
				splatShadowColor = instance.christi,
				multiplierText = "x" + Mathf.Min(PlayerSave.missionSetCompletedCount, 30),
				topSplatType = PageTop.TopSplatType.Star
			};
			_top.Setup(_topPayload);
			_missionSetProgress = _top.MakeSlotItem((UIWidget w) =>
			{
				MissionSetProgress missionSetProgress = UnityEngine.Object.Instantiate(_missionSetProgressPrefab, w.transform);
				return missionSetProgress.gameObject;
			}).GetComponent<MissionSetProgress>();
			pageGuideWidget.ResetAndUpdateAnchors();
			TopAlign(pageGuideWidget);
			_cellHeight = CalculateCellHeight(GetComponent<UIWidget>());
			RefreshMissionInfos();
			CreateNewMissionCells();
		}

		public override void Show()
		{
			_missionCompleteAnimationInProgress = false;
			base.gameObject.SetActive(true);
			RefreshMissionInfos();
			RefreshCurrentCells();
			ResetCellPositions();
			DestroyOtherMissionCells();
			RefreshPageTitles();
			_missionSetProgress.SetMissionSetProgress(_missionInfos[GetCurrentSetIndex()].Count((MissionInfo m) => m.complete));
			if (MissionsManager.Instance.currentMissionSet >= MissionsManager.Instance.GetNumberOfBasicMission())
			{
				_missionSetProgress.ChangeToMysteryBoxReward();
			}
			Breadcrumb.Consume(Breadcrumb.BreadcrumbEvent.MissionSet);
		}

		public override void Hide()
		{
			base.gameObject.SetActive(false);
			if (!AppRefreshHandler.refreshInProgress)
			{
				DisplayCelebrationPopupIfNeeded();
			}
			_missionCompleteAnimationInProgress = false;
		}

		private void DisplayCelebrationPopupIfNeeded()
		{
			if (PlayerSave.missionSetCompletedCount > MissionsManager.Instance.GetNumberOfBasicMission() + 1 && (_missionCompleteAnimationInProgress || _missionCells[GetCurrentSetIndex()].Count((MissionCell x) => !x.hasSkippedMission) == 0))
			{
				CoroutineUtil.StartCoroutine(DelayedQueueCelebrationPopup());
			}
		}

		private IEnumerator DelayedQueueCelebrationPopup()
		{
			yield return null;
			UIScreenController.Instance.QueuePopup("CelebrationPopup");
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
					cellHeight = _cellHeight,
					missionIndex = i,
					missionInfo = array[i],
					source = MissionCell.PayloadSource.ChallengesScreen,
					onMissionCellAnimationComplete = OnMissionCellAnimationComplete
				});
				_missionCells[currentSetIndex].Add(component);
			}
			_missionCellLayouts[currentSetIndex].Align();
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
				_missionCompleteAnimationInProgress = true;
				int currentMissionsSet = MissionsManager.Instance.currentMissionSet;
				_missionSetProgress.BounceNextProgressDot();
				yield return new WaitForSecondsRealtime(_missionSetProgress.GetBounceDuration());
				_missionSetProgress.BounceReward();
				yield return new WaitForSecondsRealtime(_missionSetProgress.GetBounceDuration());
				if (currentMissionsSet < MissionsManager.Instance.GetNumberOfBasicMission() + 1)
				{
					_top.PlayMultiplierAnimations();
					yield return new WaitForSecondsRealtime(_top.GetMultiplierAnimationDuration());
				}
				AnimateNewMissionSet();
				yield return new WaitForSecondsRealtime(GetTotalMissionCellAnimationTime());
				RefreshMissionInfos();
				RefreshPageTitles();
				_missionSetProgress.SetMissionSetProgress(0);
				if (MissionsManager.Instance.currentMissionSet >= MissionsManager.Instance.GetNumberOfBasicMission())
				{
					_missionSetProgress.ChangeToMysteryBoxReward(true);
				}
				_missionCompleteAnimationInProgress = false;
				if (PlayerSave.missionSetCompletedCount > MissionsManager.Instance.GetNumberOfBasicMission() + 1)
				{
					UIScreenController.Instance.QueuePopup("CelebrationPopup");
				}
			}
			else
			{
				_missionSetProgress.BounceNextProgressDot();
			}
		}

		private int CalculateCellHeight(UIWidget pageGuideWidget)
		{
			int num = 2 * _missionCellLayouts[GetCurrentSetIndex()].padding;
			int height = pageGuideWidget.height;
			float num2 = height - (topContainer.height + num);
			return (num2 / 3f).CeilToInt();
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

		private void RefreshPageTitles(bool useEffect = false)
		{
			_topPayload.subTitle = Strings.Get("MISSION_SET").Populate(PlayerSave.missionSetCompletedCount);
			_topPayload.multiplierText = "x" + Mathf.Min(PlayerSave.missionSetCompletedCount, 30);
			_top.Setup(_topPayload);
			_currentMissionSetCompletedSum = PlayerSave.missionSetCompletedCount;
			if (_currentMissionSetCompletedSum <= 30)
			{
				_top.UpdateMultiplierText("x" + _currentMissionSetCompletedSum, useEffect);
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
			Action onComplete = DestroyOtherMissionCells;
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
				StartCoroutine(AnimateCell(oldMissionsCells[2].transform, mainPosition, outEndPosition, onComplete));
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
	}
}
