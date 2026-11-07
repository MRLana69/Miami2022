using System.Collections;
using System.Collections.Generic;
using Data.Save;
using Info;
using Info.VideoRewardCooldowns;
using JetBrains.Annotations;
using Kiloo;
using Kiloo.Common.JsonUtil;
using Kiloo.Common.Utility;
using NaughtyAttributes;
using SYBO.Components.Utilities;
using SYBO.Subway;
using SYBO.Subway.Inputs;
using UI.Generic;
using UI.Generic.Layout;
using UI.Popups.ConsumablesPopupInternal;
using UI.SlideIns;
using UnityEngine;

namespace UI.Popups
{
	public class ConsumablesPopup : UIPopup
	{
		private enum VideoButtonTrackingState
		{
			NotSet = 0,
			NoFill = 1,
			Fill = 2,
			Cooldown = 3
		}

		[Header("UI References:")]
		[SerializeField]
		private UIWidget _bgFill;

		[SerializeField]
		private UILabel _consumableTitle;

		[SerializeField]
		private UILabel _consumableCount;

		[SerializeField]
		private UILabel _consumablePrice;

		[SerializeField]
		private UILabel _consumableDescription;

		[SerializeField]
		private UISprite _glowSprite;

		[SerializeField]
		private UIRect _contentPanel;

		[SerializeField]
		private UIWidget _modelGuideWidget;

		[SerializeField]
		private UISprite _effectBG;

		[Header("Prefab References:")]
		[SerializeField]
		private GameObject _hoverboardPrefab;

		[SerializeField]
		private GameObject _headstartPrefab;

		[SerializeField]
		private GameObject _scoreboosterPrefab;

		[Header("Animation Settings:")]
		[Tooltip("Curve used to animate the glow sprite's alpha and the 3D icon scale. Should be constrained to 0-1 in time.")]
		[SerializeField]
		private AnimationCurve _animCurve;

		[Tooltip("This is the duration of the glow + 3D icon animation. The entire curve above will be traversed in this duration (in seconds).")]
		[SerializeField]
		private float _animationTime;

		[Range(1f, 1.5f)]
		[Tooltip("How much larger the 3D icon should be when animated, at the peak of the animation curve.")]
		[SerializeField]
		private float _modelAnimScaling;

		[Range(1f, 1.5f)]
		[Tooltip("How much larger the glow sprite should be when animated, at the peak of the animation curve.")]
		[SerializeField]
		private float _glowAnimScaling;

		[SerializeField]
		private HorizontalLayout _buttonLayout;

		[SerializeField]
		private GameObject _watchVideoButton;

		[SerializeField]
		private UIButtonOverlayOff _videoButtonOverlay;

		[SerializeField]
		private GameObject _videoButtonNormalState;

		[SerializeField]
		private UILabel _videoButtonFreeLabel;

		[SerializeField]
		private UIWidget _videoTvIconContainer;

		[SerializeField]
		private GameObject _videoTvIconPrefab;

		private GameObject _videoTvIcon;

		private GameObject _hoverboardUIModel;

		private GameObject _headstartUIModel;

		private GameObject _scoreboosterUIModel;

		private GameObject _coinboxFundsDisabled;

		private static readonly Vector3 HOVERBOARD_POSITION_OFFSET = new Vector3(0f, 2f, 0f);

		private static readonly Vector3 HOVERBOARD_ROTATION = new Vector3(-35f, -44f, 26f);

		private static readonly Vector3 HOVERBOARD_SCALE = new Vector3(10.5f, 10.5f, 10.5f);

		private static readonly Vector3 HEADSTART_POSITION_OFFSET = new Vector3(0f, -2f, 0f);

		private static readonly Vector3 HEADSTART_ROTATION = new Vector3(0f, -10f, -51f);

		private static readonly Vector3 HEADSTART_SCALE = new Vector3(8.5f, 8.5f, 8.5f);

		private static readonly Vector3 SCOREBOOSTER_POSITION_OFFSET = new Vector3(0f, 3f, 0f);

		private static readonly Vector3 SCOREBOOSTER_ROTATION = new Vector3(11f, 8f, -72f);

		private static readonly Vector3 SCOREBOOSTER_SCALE = new Vector3(8f, 8f, 8f);

		private SellConsumablesHelper _consumablesHelper;

		private float _originalGlowAlpha;

		private Vector3 _originalGlowScale;

		private VideoButtonTrackingState _videoButtonTrackingState;

		private bool isVideoPlaying { get; set; }

		private bool isInRun
		{
			get
			{
				return Game.Instance.IsInGame.Value && _consumablesHelper != null && UIScreenController.Instance.GetTopScreenName() == "IngameUI";
			}
		}

		protected override float screenOffset
		{
			get
			{
				if (!ShouldShowTopBar())
				{
					return 0f;
				}
				return -13f;
			}
		}

		protected override void Init()
		{
			_consumablesHelper = new SellConsumablesHelper();
			_originalGlowAlpha = _glowSprite.alpha;
			_originalGlowScale = _glowSprite.transform.localScale;
			Setup3DModels();
			SetupCoinbox();
		}

		public override void LoseFocus()
		{
			Hide3DModels();
		}

		public override void GainFocus()
		{
			if (isInRun)
			{
				Show3DModel(SellConsumable.activeConsumable.upgradeType);
				return;
			}
			UIModelController.Instance.ActivateModelScreen(UIModelController.ModelScreen.TutorialPopup);
			_hoverboardUIModel.SetActive(true);
		}

		public override bool ShouldShowTopBar()
		{
			return !isInRun;
		}

		protected override void Show()
		{
			Setup((!isInRun) ? SellableConsumable.hoverboard : SellConsumable.activeConsumable);
			AdjustSizeForCoinbox(isInRun);
		}

		protected override void Hide()
		{
			if (isInRun)
			{
				_consumablesHelper.DidHideConsumablesPopup();
				UIScreenController.Instance.Camera3D.enabled = false;
				SingletonMonoBehaviour<InputManager>.Instance.WasButtonClicked = true;
			}
			Hide3DModels();
			UIScreenController.Instance.OnOverlayManuallyClosed -= OnVideoOverlayClosed;
			UIScreenController.OnApplicationResumed -= OnApplicationResumed;
			InventorySave.powerupsChanged -= OnPowerupAmountChanged;
		}

		[Button(null)]
		private void SetSizeToNoCoinBox()
		{
			AdjustSizeForCoinbox(false);
		}

		[Button(null)]
		private void SetSizeToAddedCoinBox()
		{
			AdjustSizeForCoinbox(true);
		}

		private void AdjustSizeForCoinbox(bool coinboxEnabled)
		{
			_bgFill.height = ((!coinboxEnabled) ? 283 : 320);
			_contentPanel.topAnchor.absolute = (coinboxEnabled ? (-52) : 0);
			UIRect[] componentsInChildren = _bgFill.GetComponentsInChildren<UIRect>();
			foreach (UIRect uIRect in componentsInChildren)
			{
				uIRect.ResetAndUpdateAnchors();
			}
			if (_coinboxFundsDisabled != null)
			{
				_coinboxFundsDisabled.SetActive(coinboxEnabled);
			}
		}

		private void Setup(SellableConsumable consumable)
		{
			if (consumable != null)
			{
				Setup3DModels();
				Reposition3DModels();
				if (isInRun)
				{
					UIScreenController.Instance.Camera3D.enabled = true;
					_consumablesHelper.DidShowConsumablesPopup();
				}
				ColorData instance = ColorData.instance;
				_consumableTitle.text = Strings.Get(consumable.header);
				_consumableCount.text = string.Format(Strings.Get("INVENTORY_ITEM_COUNTER"), InventorySave.GetAmount(consumable.upgradeType));
				_consumablePrice.text = consumable.price.ToString();
				_consumableDescription.text = Strings.Get(consumable.description);
				UILabel consumableTitle = _consumableTitle;
				Color white = instance.white;
				_consumableDescription.color = white;
				white = white;
				_consumablePrice.color = white;
				white = white;
				_consumableCount.color = white;
				consumableTitle.color = white;
				UILabel consumableTitle2 = _consumableTitle;
				white = instance.shadow;
				_consumableDescription.effectColor = white;
				white = white;
				_consumablePrice.effectColor = white;
				white = white;
				_consumableCount.effectColor = white;
				consumableTitle2.effectColor = white;
				_effectBG.gradientTop = NGUIMath.HexToColor(42258943u);
				_effectBG.gradientBottom = NGUIMath.HexToColor(15073279u);
				Show3DModel(consumable.upgradeType);
				UpdateVideoButtonState();
				UIScreenController.Instance.OnOverlayManuallyClosed += OnVideoOverlayClosed;
				UIScreenController.OnApplicationResumed += OnApplicationResumed;
				InventorySave.powerupsChanged += OnPowerupAmountChanged;
				_buttonLayout.Align();
			}
		}

		private void UpdateVideoButtonState()
		{
			ColorData instance = ColorData.instance;
			_watchVideoButton.SetActive(false);
			if (ShouldShowVideoButton())
			{
				_watchVideoButton.SetActive(true);
				_videoButtonFreeLabel.text = string.Format(Strings.Get("CONSUMABLES_POPUP_FREE_TEXT"), _consumablesHelper.GetConsumableRewardForVideo(isInRun));
				_videoButtonFreeLabel.color = instance.white;
				_videoButtonFreeLabel.effectColor = instance.shadow;
				_videoButtonNormalState.GetComponent<HorizontalLayout>().Align();
				if (_videoTvIcon == null)
				{
					_videoTvIcon = _videoTvIconContainer.gameObject.AddChild(_videoTvIconPrefab);
					_videoTvIcon.GetComponent<FitSpriteToWidget>().widgetContainer = _videoTvIconContainer;
					_videoTvIcon.GetComponent<FitSpriteToWidget>().FitNow();
				}
				if (_consumablesHelper.IsConsumableVideoCached(isInRun))
				{
					SwitchVideoButtonToNormalState();
				}
			}
			if (_videoButtonTrackingState == VideoButtonTrackingState.Fill || _videoButtonTrackingState == VideoButtonTrackingState.NoFill)
			{
				AdTrackingSystemLogger.LogAttemptToShowButtonFlow(_consumablesHelper.GetAdLocationInfo(isInRun));
			}
		}

		private void OnPowerupAmountChanged(UpgradeType upgradeType, int amount)
		{
			_consumableCount.text = string.Format(Strings.Get("INVENTORY_ITEM_COUNTER"), amount);
		}

		private void TrackConsumablePurchase(bool playedVideo)
		{
			UpgradeType consumableUpgradeType = _consumablesHelper.GetConsumableUpgradeType(isInRun);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Bought in run", isInRun.ToString());
			string eventName = string.Empty;
			switch (consumableUpgradeType)
			{
			case UpgradeType.hoverboard:
				eventName = "Purchase hoverboard from popup";
				break;
			case UpgradeType.scorebooster:
				eventName = "Purchase scorebooster from popup";
				break;
			case UpgradeType.headstart2000:
				eventName = "Purchase headstart from popup";
				break;
			}
			if (_videoButtonTrackingState != VideoButtonTrackingState.NotSet)
			{
				string empty = string.Empty;
				empty = (playedVideo ? "Used" : ((_videoButtonTrackingState != VideoButtonTrackingState.Fill) ? _videoButtonTrackingState.ToString() : "AvailableNotUsed"));
				dictionary.Add("video status", empty);
			}
			Flurry.LogEventWithSeveralParameters(eventName, dictionary);
		}

		[UsedImplicitly]
		public void OnCoinsButtonClicked()
		{
			_consumablesHelper.MakePurchase(isInRun, () =>
			{
				StopAllCoroutines();
				_glowSprite.alpha = _originalGlowAlpha;
				_glowSprite.transform.localScale = _originalGlowScale;
				StartCoroutine(PerformAnimation());
				TrackConsumablePurchase(false);
			});
			if (isInRun)
			{
				SingletonMonoBehaviour<InputManager>.Instance.WasButtonClicked = true;
			}
		}

		private bool ShouldShowVideoButton()
		{
			_videoButtonTrackingState = VideoButtonTrackingState.NotSet;
			if (!_consumablesHelper.IsConsumableVideoLocationAvailable(isInRun))
			{
				return false;
			}
			if (!CooldownManager.HasCharge(GetCooldownId()))
			{
				_videoButtonTrackingState = VideoButtonTrackingState.Cooldown;
				return false;
			}
			if (!_consumablesHelper.IsConsumableVideoCached(isInRun))
			{
				_videoButtonTrackingState = VideoButtonTrackingState.NoFill;
			}
			else
			{
				_videoButtonTrackingState = VideoButtonTrackingState.Fill;
			}
			string empty = string.Empty;
			string text = _consumablesHelper.GetConsumableUpgradeType(isInRun).ToString();
			empty = ((_videoButtonTrackingState != VideoButtonTrackingState.Fill) ? string.Format("NoFill-{0}", text) : string.Format("Fill-{0}", text));
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("consumable", text);
			dictionary.Add("video status", empty);
			Dictionary<string, string> parameters = dictionary;
			Flurry.LogEventWithSeveralParameters("Consumable video fill attempt", parameters);
			return _videoButtonTrackingState == VideoButtonTrackingState.Fill;
		}

		[NotNull]
		private string GetCooldownId()
		{
			switch (_consumablesHelper.GetConsumableUpgradeType(isInRun))
			{
			case UpgradeType.hoverboard:
				return "hoverboard";
			case UpgradeType.headstart2000:
				return "headstart";
			case UpgradeType.scorebooster:
				return "score_booster";
			default:
				return string.Empty;
			}
		}

		[UsedImplicitly]
		public void OnVideoButtonClicked()
		{
			CoroutineUtil.StopCoroutine(PerformAnimation());
			if (_consumablesHelper.IsConsumableVideoCached(isInRun))
			{
				AdTrackingSystemLogger.LogVideoAdButtonClick(_consumablesHelper.GetAdLocationInfo(isInRun));
				isVideoPlaying = true;
				_consumablesHelper.ShowVideoForConsumable(isInRun, OnFinishedWatchingVideo);
			}
			if (isInRun)
			{
				SingletonMonoBehaviour<InputManager>.Instance.WasButtonClicked = true;
			}
		}

		private void OnFinishedWatchingVideo()
		{
			if (isVideoPlaying)
			{
				CooldownManager.Consume(GetCooldownId());
				UIScreenController.Instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.VideoPayout,
					consumableReward = _consumablesHelper.GetConsumableReward(isInRun),
					value = _consumablesHelper.GetConsumableRewardForVideo(isInRun)
				});
				if (!ShouldShowVideoButton())
				{
					_watchVideoButton.SetActive(false);
					_buttonLayout.Align();
				}
				TrackConsumablePurchase(true);
				SaveManager.Save();
				isVideoPlaying = false;
			}
		}

		public void OnVideoOverlayClosed()
		{
			if (isVideoPlaying)
			{
				OnFinishedWatchingVideo();
			}
		}

		public void OnApplicationResumed()
		{
			UpdateVideoButtonState();
			_buttonLayout.Align();
		}

		private void SwitchVideoButtonToNormalState()
		{
			_videoButtonNormalState.SetActive(true);
			_videoButtonOverlay.GoToNormalState();
			_videoButtonNormalState.GetComponent<HorizontalLayout>().Align();
		}

		[UsedImplicitly]
		public void BackgroundClicked()
		{
			if (!isInRun)
			{
				UIScreenController.Instance.ClosePopup();
			}
		}

		private void Setup3DModels()
		{
			if (_hoverboardUIModel == null)
			{
				_hoverboardUIModel = UIModelController.Instance.TutorialPopupAnchor.AddChild(_hoverboardPrefab);
			}
			if (_headstartUIModel == null)
			{
				_headstartUIModel = UIModelController.Instance.TutorialPopupAnchor.AddChild(_headstartPrefab);
			}
			if (_scoreboosterUIModel == null)
			{
				_scoreboosterUIModel = UIModelController.Instance.TutorialPopupAnchor.AddChild(_scoreboosterPrefab);
			}
			Hide3DModels();
		}

		private void Reposition3DModels()
		{
			WidgetUtils.PositionModelOnSprite(_modelGuideWidget, _hoverboardUIModel, HOVERBOARD_POSITION_OFFSET, HOVERBOARD_ROTATION, HOVERBOARD_SCALE);
			WidgetUtils.PositionModelOnSprite(_modelGuideWidget, _headstartUIModel, HEADSTART_POSITION_OFFSET, HEADSTART_ROTATION, HEADSTART_SCALE);
			WidgetUtils.PositionModelOnSprite(_modelGuideWidget, _scoreboosterUIModel, SCOREBOOSTER_POSITION_OFFSET, SCOREBOOSTER_ROTATION, SCOREBOOSTER_SCALE);
		}

		private void Reset3DModelScales()
		{
			if (_hoverboardUIModel != null)
			{
				_hoverboardUIModel.transform.localScale = HOVERBOARD_SCALE;
			}
			if (_headstartUIModel != null)
			{
				_headstartUIModel.transform.localScale = HEADSTART_SCALE;
			}
			if (_scoreboosterUIModel != null)
			{
				_scoreboosterUIModel.transform.localScale = SCOREBOOSTER_SCALE;
			}
		}

		private void Show3DModel(UpgradeType type)
		{
			UIModelController.Instance.ActivateModelScreen(UIModelController.ModelScreen.TutorialPopup);
			switch (type)
			{
			case UpgradeType.hoverboard:
				_hoverboardUIModel.SetActive(true);
				break;
			case UpgradeType.headstart2000:
				_headstartUIModel.SetActive(true);
				break;
			case UpgradeType.scorebooster:
				_scoreboosterUIModel.SetActive(true);
				break;
			}
		}

		private void Hide3DModels()
		{
			UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.TutorialPopup);
			if (_hoverboardUIModel != null)
			{
				_hoverboardUIModel.SetActive(false);
			}
			if (_headstartUIModel != null)
			{
				_headstartUIModel.SetActive(false);
			}
			if (_scoreboosterUIModel != null)
			{
				_scoreboosterUIModel.SetActive(false);
			}
		}

		private void SetupCoinbox()
		{
			if (_coinboxFundsDisabled == null)
			{
				_coinboxFundsDisabled = InitializeCoinbox(_bgFill, false, true, true, 10f, -6f);
				_coinboxFundsDisabled.SetActive(false);
			}
		}

		private IEnumerator PerformAnimation(float delay = 0f)
		{
			float startTime = Time.realtimeSinceStartup + delay;
			while (Time.realtimeSinceStartup < startTime)
			{
				yield return null;
			}
			while (Time.realtimeSinceStartup < startTime + _animationTime)
			{
				_glowSprite.alpha = Mathf.Lerp(_originalGlowAlpha, 1f, _animCurve.Evaluate((Time.realtimeSinceStartup - startTime) / _animationTime));
				_glowSprite.transform.localScale = Vector3.Lerp(_originalGlowScale, _originalGlowScale * _glowAnimScaling, _animCurve.Evaluate((Time.realtimeSinceStartup - startTime) / _animationTime));
				_hoverboardUIModel.transform.localScale = Vector3.Lerp(HOVERBOARD_SCALE, HOVERBOARD_SCALE * _modelAnimScaling, _animCurve.Evaluate((Time.realtimeSinceStartup - startTime) / _animationTime));
				_headstartUIModel.transform.localScale = Vector3.Lerp(HEADSTART_SCALE, HEADSTART_SCALE * _modelAnimScaling, _animCurve.Evaluate((Time.realtimeSinceStartup - startTime) / _animationTime));
				_scoreboosterUIModel.transform.localScale = Vector3.Lerp(SCOREBOOSTER_SCALE, SCOREBOOSTER_SCALE * _modelAnimScaling, _animCurve.Evaluate((Time.realtimeSinceStartup - startTime) / _animationTime));
				yield return null;
			}
			_glowSprite.alpha = _originalGlowAlpha;
			_glowSprite.transform.localScale = _originalGlowScale;
			Reset3DModelScales();
		}
	}
}
