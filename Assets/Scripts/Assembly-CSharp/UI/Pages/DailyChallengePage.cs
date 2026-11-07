using System.Collections.Generic;
using Data.Save;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using Kiloo.Common.Utility;
using UI.Breadcrumbs;
using UI.Footer;
using UI.Generic;
using UI.Generic.Layout;
using UI.Pages.DailyChallengePageInternal;
using UnityEngine;

namespace UI.Pages
{
	public class DailyChallengePage : Page
	{
		[SerializeField]
		private PageTop _topPrefab;

		[SerializeField]
		private GameObject _iconPrefab;

		[SerializeField]
		private UIWidget _scalingElement;

		private PageTop _top;

		[SerializeField]
		private TimerHelper _timerPrefab;

		[SerializeField]
		private UIWidget _rewardGuide;

		[SerializeField]
		private GameObject _superMysteryBoxPrefab;

		[SerializeField]
		private Layout _dayParent;

		[SerializeField]
		private RegularDayHelper _dayPrefab;

		[SerializeField]
		private FinalDayHelper _finalDay;

		private List<UI.Pages.DailyChallengePageInternal.DayHelper> _dayHelpers = new List<UI.Pages.DailyChallengePageInternal.DayHelper>();

		private TimerHelper _timer;

		private GameObject _superMysteryBoxObject;

		private Vector3 _rotationFrom = default(Vector3);

		private Vector3 _rotationTo = default(Vector3);

		private float _currentRewardRotationDuration;

		private int _rewardRotationDirection = 1;

		public override void Init(UIWidget pageGuideWidget)
		{
			_top = Object.Instantiate(_topPrefab, topContainer.transform);
			_top.transform.localPosition = Vector3.zero;
			_top.Setup(new PageTop.EventsPageTopPayload
			{
				mainTitle = Strings.Get("DAILY_CHALLENGE_WORD_HUNT"),
				subTitle = Strings.Get("DAILY_CHALLENGE_COLLECT").ToUpper(),
				mainColor = ColorData.instance.midnight,
				subTitleColor = ColorData.instance.lime,
				splatColor = ColorData.instance.wordHuntBubbleColor,
				splatShadowColor = ColorData.instance.wordHuntBubbleShadowColor
			});
			_timer = _top.MakeSlotItem((UIWidget w) =>
			{
				TimerHelper timerHelper = Object.Instantiate(_timerPrefab, w.transform);
				timerHelper.StartTimer(EventsSave.wordHuntExpireTime);
				timerHelper.SetTimerColors(Color.white, ColorData.instance.lime);
				return timerHelper.gameObject;
			}).GetComponent<TimerHelper>();
			_top.MakeIcon(new PageTop.TopIconPayload
			{
				iconPrefab = _iconPrefab
			});
			TopAlign(pageGuideWidget);
			for (int num = 0; num < 4; num++)
			{
				RegularDayHelper regularDayHelper = Object.Instantiate(_dayPrefab, _dayParent.transform);
				regularDayHelper.SetIndex(num);
				regularDayHelper.Init();
				_dayHelpers.Add(regularDayHelper);
			}
			_finalDay.Init();
			_dayHelpers.Add(_finalDay);
			_dayParent.Align();
			if (_superMysteryBoxObject == null)
			{
				Transform parent = UIModelController.Instance.DailyChallengeRewardAnchor.transform;
				_superMysteryBoxObject = Object.Instantiate(_superMysteryBoxPrefab, parent);
				_superMysteryBoxObject.SetLayerRecursively(31);
				PrizePositionData.TransformValues? posData = new PrizePositionData.TransformValues(PrizePositionData.SUPER_MYSTERY_BOX_POSITION_OFFSET, PrizePositionData.SUPER_MYSTERY_BOX_ROTATION_OFFSET, PrizePositionData.SUPER_MYSTERY_BOX_SCALE_OFFSET);
				base.gameObject.BroadcastMessage("ResetAndUpdateAnchors");
				Timer.ExecuteNextFrame(() =>
				{
					PrizePositionData.PositionModel(posData, _superMysteryBoxObject.transform, _rewardGuide);
					_rotationFrom = _superMysteryBoxObject.transform.rotation.eulerAngles + PrizePositionData.rewardRotationOffsetFrom;
					_rotationTo = _superMysteryBoxObject.transform.rotation.eulerAngles + PrizePositionData.rewardRotationOffsetTo;
				});
			}
		}

		public override void Show()
		{
			base.gameObject.SetActive(true);
			foreach (UI.Pages.DailyChallengePageInternal.DayHelper dayHelper in _dayHelpers)
			{
				dayHelper.RefreshDay();
			}
			RefreshDailyChallenge();
			Breadcrumb.Consume(Breadcrumb.BreadcrumbEvent.WordHunt);
		}

		public override void Hide()
		{
			base.gameObject.SetActive(false);
			if (_superMysteryBoxObject != null)
			{
				_superMysteryBoxObject.SetActive(false);
			}
		}

		public override void GainFocus()
		{
			RefreshDailyChallenge();
			if (_finalDay != null)
			{
				_finalDay.RefreshDay();
			}
		}

		public override void LoseFocus()
		{
			if (_superMysteryBoxObject != null)
			{
				_superMysteryBoxObject.SetActive(false);
			}
			if (_finalDay != null)
			{
				_finalDay.Hide2DOverlaySprites();
			}
		}

		private void Update()
		{
			if (_superMysteryBoxObject != null)
			{
				RotateReward(_superMysteryBoxObject.transform);
			}
		}

		private void RotateReward(Transform transformToRotate)
		{
			if (_currentRewardRotationDuration <= PrizePositionData.rewardRotationDuration)
			{
				Vector3 euler = default(Vector3);
				float t = Easings.SineEaseInOut(_currentRewardRotationDuration / PrizePositionData.rewardRotationDuration);
				if (_rewardRotationDirection > 0)
				{
					euler = Vector3.Lerp(_rotationFrom, _rotationTo, t);
				}
				if (_rewardRotationDirection < 0)
				{
					euler = Vector3.Lerp(_rotationTo, _rotationFrom, t);
				}
				transformToRotate.rotation = Quaternion.Euler(euler);
				_currentRewardRotationDuration += Time.unscaledDeltaTime;
				if (_currentRewardRotationDuration >= PrizePositionData.rewardRotationDuration)
				{
					_rewardRotationDirection *= -1;
					_currentRewardRotationDuration -= PrizePositionData.rewardRotationDuration;
				}
			}
		}

		private void RefreshDailyChallenge()
		{
			_timer.StartTimer(EventsSave.wordHuntTimeLeft);
			if (_superMysteryBoxObject != null)
			{
				_superMysteryBoxObject.SetActive(true);
			}
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
			footerPayload.iconName = "icon_daily_challenge";
			footerPayload.title = Strings.Get("DAILY_CHALLENGE_WORD_HUNT");
			footerPayload.breadcrumbEvents = new Breadcrumb.BreadcrumbEvent[1];
			return footerPayload;
		}
	}
}
