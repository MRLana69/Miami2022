using System;
using System.Collections.Generic;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using Kiloo.Common.Utility;
using UI.Breadcrumbs;
using UI.Footer;
using UI.Generic;
using UI.Pages.WeeklyHuntPageInternal;
using UnityEngine;
using WeeklyHunt;

namespace UI.Pages
{
	public class WeeklyHuntPage : Page
	{
		[SerializeField]
		private List<WeekCell> _weekCellTabs;

		[Header("Prefab refs")]
		[SerializeField]
		private PageTop _topPrefab;

		[SerializeField]
		private GameObject _weeklyHuntIconPrefab;

		[SerializeField]
		private TimerHelper _timerPrefab;

		[SerializeField]
		private GameObject _questionmarkReward;

		[Header("Labels")]
		[SerializeField]
		private UILabel _activeWeekGoalLabel;

		[SerializeField]
		private UILabel _rewardHeader;

		[SerializeField]
		private UILabel _prizeLabel;

		[Header("Sprites")]
		[SerializeField]
		private UISprite _activeWeekBgElement;

		[SerializeField]
		private UISprite _rewardBgElement;

		[SerializeField]
		private UISprite _completedSprite;

		[SerializeField]
		private UISprite _missedUiSprite;

		[Header("Progress slider")]
		[SerializeField]
		private GameObject _activeProgressSlider;

		[SerializeField]
		private UISprite _activeProgressBg;

		[SerializeField]
		private UISprite _activeProgressFg;

		[SerializeField]
		private UILabel _activeProgressText;

		[Header("Guide Widgets")]
		[SerializeField]
		private UIWidget _rewardSectionWidget;

		[SerializeField]
		private UIWidget _rewardObjectGuide;

		[Header("Effects")]
		[SerializeField]
		private UISprite[] _beams;

		[SerializeField]
		private UISprite _glow;

		[SerializeField]
		private GameObject _presentPrefabBlue;

		[SerializeField]
		private GameObject _presentPrefabGreen;

		[SerializeField]
		private GameObject _presentPrefabRed;

		private PageTop _top;

		private TimerHelper _timer;

		private WeeklyHuntData.PrizePeriodId _currentPeriod;

		private GameObject _weeklyHuntRewardPrefab;

		private GameObject _weeklyHuntRewardGameObject;

		private Vector3 _rewardBaseRoationEuler = default(Vector3);

		private Vector3 _animatedRewardRotationEuler = default(Vector3);

		private float _currentRewardRotationDuration;

		private int _rewardRotationDirection = 1;

		private const int DEFAULT_CHECKMARK_SIZE = 40;

		private const int MAX_CHECKMARK_SIZE = 100;

		private const int MIN_CHECKMARK_SIZE = 34;

		private const float DEFAULT_ASPECT = 1.5f;

		public override void Init(UIWidget pageGuideWidget)
		{
			TopAlign(pageGuideWidget);
			_top = UnityEngine.Object.Instantiate(_topPrefab, topContainer.transform);
			_top.transform.localPosition = Vector3.zero;
			_top.Setup(new PageTop.EventsPageTopPayload
			{
				mainTitle = Strings.Get("WEEKLY_HUNT_TITLE"),
				subTitle = Strings.Get("WEEKLY_HUNT_SUBTITLE").ToUpper(),
				mainColor = ColorData.instance.midnight,
				subTitleColor = GlobalColorsGenerated.WEEKLYHUNT_CITY_NAME,
				splatColor = GlobalColorsGenerated.WEEKLYHUNT_SPLASH,
				splatShadowColor = GlobalColorsGenerated.WEEKLYHUNT_SPLASH_SHADOW
			});
			_top.MakeIcon(new PageTop.TopIconPayload
			{
				iconPrefab = _weeklyHuntIconPrefab,
				localPositionOffset = WeeklyHuntIconPositioning.MODEL_ICON_POSITION_OFFSET,
				localScaleOffset = WeeklyHuntIconPositioning.MODEL_ICON_SCALE_OFFSET,
				localRotationOffsetEuler = WeeklyHuntIconPositioning.MODEL_ICON_ROTATION
			});
			_timer = _top.MakeSlotItem((UIWidget w) =>
			{
				TimerHelper timerHelper = UnityEngine.Object.Instantiate(_timerPrefab, w.transform);
				return timerHelper.gameObject;
			}).GetComponent<TimerHelper>();
			_currentPeriod = WeeklyHuntData.GetActivePeriod();
			for (int num = 0; num < _weekCellTabs.Count; num++)
			{
				_weekCellTabs[num].Init(new WeekCell.WeekCellPayload
				{
					clicked = TabClicked,
					periodId = (WeeklyHuntData.PrizePeriodId)(num + 1),
					label = WeeklyHuntEditorData.Instance.periodData[num].GetPeriodTitle()
				});
			}
			_rewardHeader.text = Strings.Get("WEEKLY_HUNT_REWARD_TEXT");
			_activeWeekBgElement.gradientTop = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_BG_GRADIENT_TOP;
			_activeWeekBgElement.gradientBottom = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_BG_GRADIENT_BOTTOM;
			_activeWeekGoalLabel.color = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_GOAL_TEXT;
			_activeWeekGoalLabel.effectColor = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_GOAL_TEXT_SHADOW;
			_rewardHeader.color = GlobalColorsGenerated.WEEKLYHUNT_REWARD_HEADLINE;
			_rewardHeader.effectColor = GlobalColorsGenerated.WEEKLYHUNT_REWARD_HEADLINE_SHADOW;
			_prizeLabel.color = GlobalColorsGenerated.WEEKLYHUNT_REWARD_AND_AMOUNT_TEXT;
			_prizeLabel.effectColor = GlobalColorsGenerated.WEEKLYHUNT_REWARD_AND_AMOUNT_TEXT_SHADOW;
			_glow.color = ColorData.instance.glowColor;
			_timer.SetTimerColors(GlobalColorsGenerated.WEEKLYHUNT_TIMER_TEXT, GlobalColorsGenerated.WEEKLYHUNT_TIMER_BG);
			_rewardBgElement.color = GlobalColorsGenerated.WEEKLYHUNT_REWARD_BG;
			UISprite[] beams = _beams;
			foreach (UISprite uISprite in beams)
			{
				uISprite.color = GlobalColors.WEEKLYHUNT_STRIPE_COLOR;
			}
		}

		public override void Show()
		{
			base.gameObject.SetActive(true);
			_currentPeriod = WeeklyHuntData.GetActivePeriod();
			if (_currentPeriod == WeeklyHuntData.PrizePeriodId.None || _currentPeriod == WeeklyHuntData.PrizePeriodId.PeriodFour)
			{
				if (WeeklyHuntData.hideLastPeriod)
				{
					_currentPeriod = WeeklyHuntData.PrizePeriodId.PeriodThree;
				}
				else
				{
					_currentPeriod = WeeklyHuntData.PrizePeriodId.PeriodFour;
				}
			}
			if (WeeklyHuntManager.IsHuntStillActive())
			{
				_timer.onTimerEnded += RefreshTimerState;
				_timer.onTimerEnded += RefreshPage;
			}
			RefreshPage();
		}

		public override void Hide()
		{
			if (_weeklyHuntRewardGameObject != null)
			{
				UnityEngine.Object.Destroy(_weeklyHuntRewardGameObject);
			}
			_timer.onTimerEnded -= RefreshTimerState;
			_timer.onTimerEnded -= RefreshPage;
			base.gameObject.SetActive(false);
		}

		public override void GainFocus()
		{
			SetObjectVisibility(true);
		}

		public override void LoseFocus()
		{
			SetObjectVisibility(false);
		}

		private void RefreshPage()
		{
			Breadcrumb.Consume(Breadcrumb.BreadcrumbEvent.WeeklyHunt);
			RefreshTimerState();
			SetupRewardObject();
			UpdateTabs();
			SetupDynamicTexts();
			SetupDynamicColors();
			SetupDynamicSprites();
			RefreshProgressBar(WeeklyHuntManager.GetPrizeDataForPeriod(_currentPeriod).itemsCollected, WeeklyHuntManager.GetPrizeDataForPeriod(_currentPeriod).itemsToCollect);
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
			footerPayload.iconName = "icon_weekly_hunt";
			footerPayload.iconOffset = new Vector3(1f, 0f, 0f);
			footerPayload.title = Strings.Get("WEEKLY_HUNT_TITLE");
			footerPayload.breadcrumbEvents = new Breadcrumb.BreadcrumbEvent[1] { Breadcrumb.BreadcrumbEvent.WeeklyHunt };
			return footerPayload;
		}

		private void Update()
		{
			if (_weeklyHuntRewardGameObject != null)
			{
				RotateReward(_weeklyHuntRewardGameObject.transform);
			}
		}

		private void RotateReward(Transform transformToRotate)
		{
			float t = Easings.SineEaseInOut(_currentRewardRotationDuration / PrizePositionData.rewardRotationDuration);
			if (_rewardRotationDirection > 0)
			{
				_animatedRewardRotationEuler = Vector3.Lerp(PrizePositionData.rewardRotationOffsetFrom, PrizePositionData.rewardRotationOffsetTo, t);
			}
			else
			{
				_animatedRewardRotationEuler = Vector3.Lerp(PrizePositionData.rewardRotationOffsetTo, PrizePositionData.rewardRotationOffsetFrom, t);
			}
			transformToRotate.rotation = Quaternion.Euler(_rewardBaseRoationEuler + _animatedRewardRotationEuler);
			_currentRewardRotationDuration += Time.unscaledDeltaTime;
			if (_currentRewardRotationDuration >= PrizePositionData.rewardRotationDuration)
			{
				_rewardRotationDirection *= -1;
				_currentRewardRotationDuration -= PrizePositionData.rewardRotationDuration;
			}
		}

		private void SetupRewardObject()
		{
			_rewardObjectGuide.ResetAndUpdateAnchors();
			WeeklyHuntData.PrizeData prizeForPeriod = WeeklyHuntManager.GetPrizeForPeriod(_currentPeriod);
			bool hiddenUntilUnlockedForPeriod = WeeklyHuntManager.GetHiddenUntilUnlockedForPeriod(_currentPeriod);
			bool showPresentIcon = hiddenUntilUnlockedForPeriod && WeeklyHuntManager.GetPrizeDataForPeriod(_currentPeriod).itemsCollected != WeeklyHuntManager.GetPrizeDataForPeriod(_currentPeriod).itemsToCollect;
			PrizePositionData.TransformValues? posData = null;
			Transform transform = null;
			if (WeeklyHuntManager.GetPeriodStatus(_currentPeriod) == WeeklyHuntData.PeriodStatus.Unstarted && WeeklyHuntManager.GetPrizeDataForPeriod(_currentPeriod).itemsCollected != WeeklyHuntManager.GetPrizeDataForPeriod(_currentPeriod).itemsToCollect)
			{
				_weeklyHuntRewardPrefab = _questionmarkReward;
				transform = UIModelController.Instance.WeeklyHuntRewardAnchor.transform;
				posData = PrizePositionData.GetPositioningDataForUnknownReward(showPresentIcon);
			}
			else
			{
				_weeklyHuntRewardPrefab = PrizeIconPrefabData.instance.GetPrefab(prizeForPeriod);
				transform = ((!showPresentIcon) ? UIModelController.Instance.WeeklyHuntRewardAnchor.transform : UIModelController.Instance.HuntPrizeAnchor.transform);
				posData = PrizePositionData.GetPrizePositioningData(prizeForPeriod, showPresentIcon);
			}
			if (showPresentIcon)
			{
				switch (_currentPeriod)
				{
				case WeeklyHuntData.PrizePeriodId.PeriodOne:
					_weeklyHuntRewardPrefab = _presentPrefabBlue;
					break;
				case WeeklyHuntData.PrizePeriodId.PeriodTwo:
					_weeklyHuntRewardPrefab = _presentPrefabGreen;
					break;
				case WeeklyHuntData.PrizePeriodId.PeriodThree:
					_weeklyHuntRewardPrefab = _presentPrefabRed;
					break;
				default:
					SDebug.LogError("Hunt Period not available for present!");
					_weeklyHuntRewardPrefab = _presentPrefabGreen;
					break;
				}
			}
			UnityEngine.Object.Destroy(_weeklyHuntRewardGameObject);
			_weeklyHuntRewardGameObject = UnityEngine.Object.Instantiate(_weeklyHuntRewardPrefab, transform);
			if (UIScreenController.Instance.isShowingPopup)
			{
				_weeklyHuntRewardGameObject.SetActive(false);
			}
			_weeklyHuntRewardGameObject.SetLayerRecursively((!showPresentIcon) ? 31 : 29);
			Timer.ExecuteNextFrame(() =>
			{
				PrizePositionData.PositionModel(posData, _weeklyHuntRewardGameObject.transform, _rewardObjectGuide, showPresentIcon);
				if (posData.HasValue)
				{
					_rewardBaseRoationEuler = _weeklyHuntRewardGameObject.transform.rotation.eulerAngles;
				}
				RotateReward(_weeklyHuntRewardGameObject.transform);
			});
		}

		private void SetupDynamicTexts()
		{
			_activeWeekGoalLabel.text = string.Format(Strings.Get("WEEKLY_HUNT_COLLECT_TEXT"), WeeklyHuntManager.GetPrizeDataForPeriod(_currentPeriod).itemsToCollect);
			_activeProgressText.text = WeeklyHuntManager.GetPrizeDataForPeriod(_currentPeriod).itemsCollected + "/" + WeeklyHuntManager.GetPrizeDataForPeriod(_currentPeriod).itemsToCollect;
			_prizeLabel.text = string.Format(WeeklyHuntManager.GetPrizeTitle(WeeklyHuntManager.GetPrizeForPeriod(_currentPeriod)), WeeklyHuntManager.GetPrizeForPeriod(_currentPeriod).amount);
			_prizeLabel.enabled = WeeklyHuntManager.GetPeriodStatus(_currentPeriod) != WeeklyHuntData.PeriodStatus.Unstarted && !WeeklyHuntManager.GetHiddenUntilUnlockedForPeriod(_currentPeriod);
		}

		private void SetupDynamicColors()
		{
			if (WeeklyHuntManager.GetPeriodStatus(_currentPeriod) == WeeklyHuntData.PeriodStatus.Expired || WeeklyHuntManager.GetPeriodStatus(_currentPeriod) == WeeklyHuntData.PeriodStatus.Unstarted)
			{
				_activeProgressBg.color = GlobalColorsGenerated.WEEKLYHUNT_INACTIVE_PROGRESSBAR_BG;
				_activeProgressFg.color = GlobalColorsGenerated.WEEKLYHUNT_INACTIVE_PROGRESSBAR_FG;
				_activeProgressText.color = GlobalColorsGenerated.WEEKLYHUNT_INACTIVE_PROGRESSBAR_TEXT;
			}
			else if (WeeklyHuntManager.GetPeriodStatus(_currentPeriod) == WeeklyHuntData.PeriodStatus.Current)
			{
				_activeProgressBg.color = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_PROGRESSBAR_BG;
				_activeProgressFg.color = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_PROGRESSBAR_FG;
				_activeProgressText.color = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_PROGRESSBAR_TEXT;
			}
		}

		private void SetupDynamicSprites()
		{
			_missedUiSprite.gameObject.SetActive(false);
			_completedSprite.gameObject.SetActive(false);
			if (WeeklyHuntManager.GetPeriodStatus(_currentPeriod) == WeeklyHuntData.PeriodStatus.Expired && WeeklyHuntManager.GetPrizeDataForPeriod(_currentPeriod).itemsCollected != WeeklyHuntManager.GetPrizeDataForPeriod(_currentPeriod).itemsToCollect)
			{
				_missedUiSprite.gameObject.SetActive(true);
				_missedUiSprite.height = Mathf.Clamp((40f * (1.5f - NGUITools.screenSize.x / NGUITools.screenSize.y)).RoundToInt(), 34, 100);
			}
			if (WeeklyHuntManager.GetPrizeDataForPeriod(_currentPeriod).itemsCollected == WeeklyHuntManager.GetPrizeDataForPeriod(_currentPeriod).itemsToCollect)
			{
				_completedSprite.gameObject.SetActive(true);
				_completedSprite.height = Mathf.Clamp((40f * (1.5f - NGUITools.screenSize.x / NGUITools.screenSize.y)).RoundToInt(), 34, 100);
			}
		}

		public void RefreshProgressBar(int currentItemAmount, int totalItemAmount)
		{
			UISlider component = _activeProgressSlider.GetComponent<UISlider>();
			if (totalItemAmount != 0)
			{
				component.value = (float)currentItemAmount / (float)totalItemAmount;
			}
			else
			{
				component.value = 1f;
			}
			_activeProgressText.text = string.Format("{0} / {1}", currentItemAmount, totalItemAmount);
		}

		private void SetObjectVisibility(bool visible)
		{
			if (!visible)
			{
				_missedUiSprite.gameObject.SetActive(false);
				_completedSprite.gameObject.SetActive(false);
			}
			else
			{
				SetupDynamicSprites();
			}
			if (_weeklyHuntRewardGameObject != null)
			{
				_weeklyHuntRewardGameObject.SetActive(visible);
			}
		}

		private void RefreshTimerState()
		{
			if (WeeklyHuntManager.IsHuntStillActive())
			{
				TimeSpan timeLeftForActivePeriod = WeeklyHuntManager.GetTimeLeftForActivePeriod();
				_timer.StartTimer(timeLeftForActivePeriod);
			}
			else
			{
				_timer.StopTimer();
				_timer.HideIcon();
				_timer.SetTimerText(Strings.Get("WEEKLY_HUNT_ENDED_TIMER"));
			}
		}

		private void UpdateTabs()
		{
			for (int i = 0; i < _weekCellTabs.Count; i++)
			{
				_weekCellTabs[i].SetCellActive(_weekCellTabs[i].periodId == _currentPeriod);
				_weekCellTabs[i].UpdateStatus();
			}
			if (_currentPeriod == WeeklyHuntData.PrizePeriodId.None)
			{
				_weekCellTabs[2].SetCellActive(true);
			}
		}

		private void TabClicked(WeeklyHuntData.PrizePeriodId tabId)
		{
			_currentPeriod = tabId;
			RefreshPage();
		}
	}
}
