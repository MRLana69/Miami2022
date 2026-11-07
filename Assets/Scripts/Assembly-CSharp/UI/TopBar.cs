using System;
using System.Collections.Generic;
using System.Linq;
using Data.Save;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using Kiloo.Common.Utility;
using NaughtyAttributes;
using SYBO.Subway;
using UI.Generic.Layout;
using UI.Screens.FrontScreenInternal;
using UnityEngine;
using Utilities;

namespace UI
{
	public class TopBar : MonoBehaviour
	{
		private enum AnimationDirection
		{
			None = 0,
			In = 1,
			Out = 2
		}

		public class TopBarPayload
		{
			public int width = 320;

			public int height = 26;
		}

		[SerializeField]
		private UISprite _background;

		[SerializeField]
		private UISprite _settingsIcon;

		[SerializeField]
		private UIWidget _coinboxFunds;

		[SerializeField]
		private UIWidget _coinboxFundsBackground;

		[SerializeField]
		private List<UISprite> _buttonBackgrounds;

		[SerializeField]
		private List<UISprite> _buttonShadows;

		[SerializeField]
		private UILabel _scoreLabel;

		[SerializeField]
		private UILabel _hoverboardsLabel;

		[SerializeField]
		private UILabel _keysLabel;

		[SerializeField]
		private UILabel _coinsLabel;

		[SerializeField]
		private Layout _leftLayout;

		[SerializeField]
		private Layout _centerLayout;

		[SerializeField]
		private Layout _rightLayout;

		[SerializeField]
		private TutorialIconAnimation _hoverboardTutorial;

		[SerializeField]
		private Collider[] _clickAreasLockedDuringTutorial;

		private static int _topBarOffset;

		private AnimationDirection _currentAnimationDirection;

		private TopBarPayload _payload;

		private const float SHOWN_POSITION = 0f;

		private float _hiddenPosition;

		private const int FRAME_DELAYS_BEFORE_ANIMATION = 2;

		private int _currentFrameDelays;

		private float _currentFactor = 1f;

		[SerializeField]
		private float _animationDuration = 0.25f;

		private bool _interactionAllowed = true;

		[Space(20f)]
		[Header("Extended Stat Bar")]
		[SerializeField]
		private UIWidget _currencyBackground;

		[SerializeField]
		private UIWidget _currencyShadow;

		[SerializeField]
		private UIWidget _scoreBackground;

		[SerializeField]
		private UIWidget _scoreShadow;

		[SerializeField]
		private UIWidget _scoreContainer;

		[SerializeField]
		private UIWidget _currencyTextWidget;

		[SerializeField]
		private UIWidget _currencyContainer;

		[SerializeField]
		private float _startDelay = 0.2f;

		[SerializeField]
		private float _fadeDuration = 0.1f;

		[SerializeField]
		private float _extendDuration = 0.3f;

		[SerializeField]
		private int _labelExtention = 20;

		[SerializeField]
		private int _shadowOffset = 14;

		[SerializeField]
		private float _widthThreshold = 0.05f;

		[SerializeField]
		private string[] _screensToDisplayFullStats;

		[SerializeField]
		private string[] _popupsToDisplayFullStats;

		[SerializeField]
		private UILabel _dummyCurrency;

		private bool _isExtended;

		private bool _useTweens;

		private TweenWidth _currencyTween;

		private bool _highScoreLocked;

		private bool _coinsLocked;

		private bool _keysLocked;

		private bool _hoverboardsLocked;

		private static bool showHoverboardTutorial
		{
			get
			{
				return OnboardingSave.shouldShowHoverboardPopup && !OnboardingSave.hasShownHoverboardPopup && (!Game.IsInstantiated || !Game.Instance.IsPaused);
			}
		}

		private float deltaFactor
		{
			get
			{
				return Mathf.Min(Time.unscaledDeltaTime, 0.075f) * (1f / _animationDuration);
			}
		}

		public static int GetTopBarOffset()
		{
			return _topBarOffset;
		}

		public void Init(TopBarPayload payload)
		{
			_payload = payload;
			_topBarOffset = -(payload.height + 2);
			_hiddenPosition = _payload.height + 20;
			base.transform.SetLocalPosY(_hiddenPosition);
			SetupBasic();
			_hoverboardTutorial.InitAnimation(GuidelineButton.GuidelineButtonType.Hoverboard.ToString());
			SetHighScore(PlayerSave.highScore, false);
			SetHoverboards(InventorySave.GetAmount(UpgradeType.hoverboard), false);
			SetCoins(InventorySave.coins, false);
			SetKeys(InventorySave.keys, false);
			InventorySave.coinsChanged += CoinsChanged;
			InventorySave.keysChanged += KeysChanged;
			PlayerSave.highScoreChanged += HighScoreChanged;
			InventorySave.powerupsChanged += PowerupsChanged;
			UIWidget currencyContainer = _currencyContainer;
			currencyContainer.onChange = (UIWidget.OnDimensionsChanged)Delegate.Combine(currencyContainer.onChange, new UIWidget.OnDimensionsChanged(OnContainerChange));
			UIScreenController.OnPopupShown += OnPopupShown;
			UIScreenController.OnChangedScreen += ChangedScreen;
			DetermineHoverboardTutorialVisibility();
			Align();
		}

		public void OnDestroy()
		{
			InventorySave.coinsChanged -= CoinsChanged;
			InventorySave.keysChanged -= KeysChanged;
			PlayerSave.highScoreChanged -= HighScoreChanged;
			InventorySave.powerupsChanged -= PowerupsChanged;
			UIScreenController.OnChangedScreen -= ChangedScreen;
			UIScreenController.OnPopupShown -= OnPopupShown;
			UIWidget currencyContainer = _currencyContainer;
			currencyContainer.onChange = (UIWidget.OnDimensionsChanged)Delegate.Remove(currencyContainer.onChange, new UIWidget.OnDimensionsChanged(OnContainerChange));
		}

		private void HighScoreChanged()
		{
			if (!_highScoreLocked)
			{
				SetHighScore(PlayerSave.highScore);
			}
		}

		private void CoinsChanged()
		{
			if (!_coinsLocked)
			{
				SetCoins(InventorySave.coins);
			}
		}

		private void KeysChanged()
		{
			if (!_keysLocked)
			{
				SetKeys(InventorySave.keys);
			}
		}

		private void PowerupsChanged(UpgradeType powerup, int amount)
		{
			if (powerup == UpgradeType.hoverboard)
			{
				HoverboardsChanged();
			}
		}

		private void HoverboardsChanged()
		{
			if (!_hoverboardsLocked)
			{
				SetHoverboards(InventorySave.GetAmount(UpgradeType.hoverboard));
			}
		}

		public void SetHighScoreLock(bool value)
		{
			_highScoreLocked = value;
			if (!_highScoreLocked)
			{
				SetHighScore(PlayerSave.highScore);
			}
		}

		public void SetCoinsLock(bool value)
		{
			_coinsLocked = value;
			if (!_coinsLocked)
			{
				SetCoins(InventorySave.coins);
			}
		}

		public void SetKeysLock(bool value)
		{
			_keysLocked = value;
			if (!_keysLocked)
			{
				SetKeys(InventorySave.keys);
			}
		}

		public void SetHoverboardLock(bool value)
		{
			_hoverboardsLocked = value;
			if (!_hoverboardsLocked)
			{
				SetHoverboards(InventorySave.GetAmount(UpgradeType.hoverboard));
			}
		}

		public void SetHighScore(int newScore, bool align = true)
		{
			_scoreLabel.text = newScore.FormatSI();
			if (align)
			{
				Align();
			}
		}

		public void SetCoins(int coins, bool align = true)
		{
			if (_isExtended)
			{
				if (UIScreenController.Instance.GetTopScreenName() == "GameOverScreen" && !UIScreenController.Instance.isShowingPopup)
				{
					_dummyCurrency.text = coins.ToString();
				}
				else
				{
					_coinsLabel.text = coins.ToString();
					_dummyCurrency.text = coins.ToString();
				}
				if (align)
				{
					Align();
				}
			}
			else
			{
				_coinsLabel.text = coins.FormatSI();
				if (align)
				{
					Align();
				}
			}
		}

		public void UpdateCoinBoxLayout()
		{
			Layout[] componentsInChildren = _rightLayout.GetComponentsInChildren<Layout>();
			foreach (Layout layout in componentsInChildren)
			{
				layout.Align(true);
			}
			_coinboxFunds.ResetAndUpdateAnchors();
			_coinboxFundsBackground.ResetAndUpdateAnchors();
		}

		public void SetKeys(int keys, bool align = true)
		{
			if (_isExtended)
			{
				_keysLabel.text = keys.ToString();
				Align();
				return;
			}
			_keysLabel.text = keys.FormatSI();
			if (align)
			{
				Align();
			}
		}

		public void SetHoverboards(int hoverboards, bool align = true)
		{
			_hoverboardsLabel.text = hoverboards.FormatSI();
			if (align)
			{
				Align();
			}
		}

		public void ShowTopBar(bool show)
		{
			_rightLayout.gameObject.SetActive(OnboardingSave.isTutorialCompleted);
			_centerLayout.gameObject.SetActive(OnboardingSave.isTutorialCompleted);
			Collider[] clickAreasLockedDuringTutorial = _clickAreasLockedDuringTutorial;
			foreach (Collider collider in clickAreasLockedDuringTutorial)
			{
				collider.enabled = OnboardingSave.isTutorialCompleted;
			}
			_currentAnimationDirection = (show ? AnimationDirection.In : AnimationDirection.Out);
			Align();
			if (SafeArea.IsRestricted && SafeArea.deviceUsesStatusBar)
			{
				IOSViewController.ShowStatusBar(show);
			}
			_currentFrameDelays = 2;
		}

		private void Update()
		{
			if (_currentFrameDelays > 0)
			{
				_currentFrameDelays--;
			}
			else
			{
				UpdateAnimation();
			}
			_interactionAllowed = _currentAnimationDirection == AnimationDirection.None;
		}

		private void UpdateAnimation()
		{
			switch (_currentAnimationDirection)
			{
			case AnimationDirection.In:
			{
				_currentFactor = Mathf.Clamp01(_currentFactor - deltaFactor);
				float t = Easings.CubicEaseIn(_currentFactor);
				float posY = Mathf.Lerp(0f, _hiddenPosition, t);
				base.transform.SetLocalPosY(posY);
				if (_currentFactor <= 0f)
				{
					_currentAnimationDirection = AnimationDirection.None;
				}
				break;
			}
			case AnimationDirection.Out:
			{
				_currentFactor = Mathf.Clamp01(_currentFactor + deltaFactor);
				float t = Easings.CubicEaseIn(_currentFactor);
				float posY = Mathf.Lerp(0f, _hiddenPosition, t);
				base.transform.SetLocalPosY(posY);
				if (_currentFactor >= 1f)
				{
					_currentAnimationDirection = AnimationDirection.None;
				}
				break;
			}
			}
		}

		[UsedImplicitly]
		public void SettingsClicked()
		{
			if (_interactionAllowed)
			{
				CloseOtherPopups("SettingsPopup");
				ShowOrClosePopup("SettingsPopup");
			}
		}

		[UsedImplicitly]
		public void ScoreClicked()
		{
			if (_interactionAllowed)
			{
				CloseOtherPopups("LeaderboardPopup");
				ShowOrClosePopup("LeaderboardPopup");
			}
		}

		[UsedImplicitly]
		public void HoverboardsClicked()
		{
			if (_interactionAllowed)
			{
				SellConsumable.activeConsumable = SellableConsumable.hoverboard;
				CloseOtherPopups("TutorialHoverboardsPopup", "ConsumablesPopup");
				if (showHoverboardTutorial)
				{
					UIScreenController.Instance.QueuePopup("TutorialHoverboardsPopup");
					OnboardingSave.hasShownHoverboardPopup = true;
					OnboardingSave.shouldShowHoverboardPopup = false;
					DetermineHoverboardTutorialVisibility();
				}
				else if (UIScreenController.Instance.IsPopupInQueue("TutorialHoverboardsPopup"))
				{
					UIScreenController.Instance.CloseActivePopups("TutorialHoverboardsPopup");
				}
				else
				{
					ShowOrClosePopup("ConsumablesPopup");
				}
			}
		}

		[UsedImplicitly]
		public void CoinboxClicked()
		{
			if (_interactionAllowed)
			{
				CloseOtherPopups("StorePopup");
				ShowOrClosePopup("StorePopup");
			}
		}

		private void ShowOrClosePopup(string popupName)
		{
			if (!UIScreenController.Instance.IsPopupInQueue(popupName))
			{
				UIScreenController.Instance.QueuePopup(popupName);
			}
			else
			{
				UIScreenController.Instance.CloseAllPopups();
			}
		}

		private void CloseOtherPopups(params string[] excludedPopups)
		{
			if (UIScreenController.Instance.isShowingPopup && !UIScreenController.Instance.IsPopupInQueue(excludedPopups))
			{
				UIScreenController.Instance.ignoreLastPopupClosed = true;
				UIScreenController.Instance.CloseAllPopups();
			}
		}

		private void OnPopupShown(string popup)
		{
			if (popup.IsNullOrEmpty())
			{
				return;
			}
			if (_popupsToDisplayFullStats.Contains(popup))
			{
				if (!_isExtended)
				{
					Expand();
				}
			}
			else if (!_screensToDisplayFullStats.Contains(UIScreenController.Instance.GetTopScreenName()))
			{
				Shrink();
			}
		}

		private void ChangedScreen(string screen)
		{
			if (!UIScreenController.Instance.isShowingPopup)
			{
				DetermineHoverboardTutorialVisibility();
				if (_screensToDisplayFullStats.Contains(screen))
				{
					Expand();
				}
				else
				{
					Shrink();
				}
				if (screen == "GameOverScreen")
				{
					_dummyCurrency.text = InventorySave.coins.ToString();
					_dummyCurrency.transform.position = _coinsLabel.transform.position;
					_dummyCurrency.alpha = 1f;
					_coinsLabel.alpha = 0f;
				}
				else
				{
					_coinsLabel.alpha = 1f;
					_dummyCurrency.alpha = 0f;
				}
			}
		}

		private void DetermineHoverboardTutorialVisibility()
		{
			_hoverboardTutorial.ShowIcon(showHoverboardTutorial);
		}

		[Button(null)]
		private void Align()
		{
			UIWidget component = GetComponent<UIWidget>();
			component.width = ((_payload == null) ? 320 : _payload.width);
			component.height = ((_payload == null) ? 26 : _payload.height);
			List<Layout> list = (from c in GetComponentsInChildren<Layout>()
				orderby c.transform.GetHierarchyDepth(base.transform) descending
				select c).ToList();
			list.ForEach((Layout l) =>
			{
				l.Align(true);
			});
			_coinboxFunds.ResetAndUpdateAnchors();
			_coinboxFundsBackground.ResetAndUpdateAnchors();
			_leftLayout.Align(true);
			_rightLayout.Align(true);
			_centerLayout.Align(true);
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(base.transform, _leftLayout.transform, false);
			Bounds bounds2 = NGUIMath.CalculateRelativeWidgetBounds(base.transform, _rightLayout.transform, false, true, true);
			int num = (component.width - 10) / 2;
			float x = bounds.size.x;
			float x2 = bounds2.size.x;
			float posX = (x - x2) / 2f;
			_leftLayout.transform.SetLocalPosX(-num);
			_rightLayout.transform.SetLocalPosX(num);
			_centerLayout.transform.SetLocalPosX(posX);
			(from r in GetComponentsInChildren<UIRect>().ToList()
				orderby r.transform.GetHierarchyDepth() descending
				select r).ToList().ForEach((UIRect r) =>
			{
				r.ResetAndUpdateAnchors();
			});
		}

		[Button(null)]
		private void SetupBasic()
		{
			ColorData colors = ColorData.instance;
			_settingsIcon.color = colors.stroking;
			_background.gradientTop = "#54666d".ToColor();
			_background.gradientBottom = "#617b85".ToColor();
			_buttonBackgrounds.ForEach((UISprite s) =>
			{
				s.gradientTop = colors.rock;
				s.gradientBottom = colors.iron;
			});
			_buttonShadows.ForEach((UISprite s) =>
			{
				s.color = colors.shadow;
			});
			if (!Application.isPlaying)
			{
				base.gameObject.SetActive(false);
				base.gameObject.SetActive(true);
			}
		}

		[Button("ShowBar")]
		private void Editor_ShowBar()
		{
			ShowTopBar(true);
		}

		[Button("HideBar")]
		private void Editor_HideBar()
		{
			ShowTopBar(false);
		}

		private void OnContainerChange()
		{
			if (_useTweens)
			{
				TweenWidth tweenWidth = ((_currencyContainer.width % 2 != 0) ? TweenWidth.Begin(_currencyBackground, _extendDuration, _currencyContainer.width - 1) : TweenWidth.Begin(_currencyBackground, _extendDuration, _currencyContainer.width));
				tweenWidth.delay = _startDelay;
				tweenWidth.AddOnFinished(OnTweenFinish);
				if ((_currencyContainer.width + _shadowOffset) % 2 == 0)
				{
					TweenWidth.Begin(_currencyShadow, _extendDuration, _currencyContainer.width + _shadowOffset).delay = _startDelay;
				}
				else
				{
					TweenWidth.Begin(_currencyShadow, _extendDuration, _currencyContainer.width + _shadowOffset - 1).delay = _startDelay;
				}
				if (_scoreContainer.width % 2 == 0)
				{
					TweenWidth.Begin(_scoreBackground, _extendDuration, _scoreContainer.width).delay = _startDelay;
					TweenWidth.Begin(_scoreShadow, _extendDuration, _scoreContainer.width).delay = _startDelay;
				}
				else
				{
					TweenWidth.Begin(_scoreBackground, _extendDuration, _scoreContainer.width - 1).delay = _startDelay;
					TweenWidth.Begin(_scoreShadow, _extendDuration, _scoreContainer.width - 1).delay = _startDelay;
				}
				return;
			}
			if (_isExtended)
			{
				ShrinkScoreIfNeeded();
			}
			if (_currencyContainer.width % 2 == 0)
			{
				_currencyBackground.width = _currencyContainer.width;
			}
			else
			{
				_currencyBackground.width = _currencyContainer.width - 1;
			}
			if ((_currencyContainer.width + _shadowOffset) % 2 == 0)
			{
				_currencyShadow.width = _currencyContainer.width + _shadowOffset;
			}
			else
			{
				_currencyShadow.width = _currencyContainer.width + _shadowOffset - 1;
			}
			if (_scoreContainer.width % 2 == 0)
			{
				_scoreBackground.width = _scoreContainer.width;
				_scoreShadow.width = _scoreContainer.width;
			}
			else
			{
				_scoreBackground.width = _scoreContainer.width - 1;
				_scoreShadow.width = _scoreContainer.width - 1;
			}
			Align();
		}

		private void ShrinkScoreIfNeeded()
		{
			_scoreLabel.text = PlayerSave.highScore.FormatSI();
			Align();
			float num = _currencyContainer.width + _hoverboardsLabel.width + _scoreContainer.width;
			if (num > _widthThreshold)
			{
				_scoreLabel.text = "...";
			}
			else
			{
				SetHighScore(PlayerSave.highScore);
			}
			Align();
		}

		private void OnTweenFinish()
		{
			Timer.ExecuteNextFrame(FixWidth);
			TweenAlpha.Begin(_currencyTextWidget.gameObject, _fadeDuration, 1f);
			TweenAlpha.Begin(_scoreLabel.gameObject, _fadeDuration, 1f);
			_useTweens = false;
		}

		private void FixWidth()
		{
			if (_currencyContainer.width % 2 == 0)
			{
				_currencyBackground.width = _currencyContainer.width;
			}
			else
			{
				_currencyBackground.width = _currencyContainer.width - 1;
			}
			if ((_currencyContainer.width + _shadowOffset) % 2 == 0)
			{
				_currencyShadow.width = _currencyContainer.width + _shadowOffset;
			}
			else
			{
				_currencyShadow.width = _currencyContainer.width + _shadowOffset - 1;
			}
			if (_scoreContainer.width % 2 == 0)
			{
				_scoreBackground.width = _scoreContainer.width;
				_scoreShadow.width = _scoreContainer.width;
			}
			else
			{
				_scoreBackground.width = _scoreContainer.width - 1;
				_scoreShadow.width = _scoreContainer.width - 1;
			}
			Align();
		}

		private void OnTweenFinishedOut()
		{
			_coinsLabel.width += _labelExtention;
			_keysLabel.width += _labelExtention;
			_coinsLabel.text = InventorySave.coins.ToString();
			_keysLabel.text = InventorySave.keys.ToString();
			ShrinkScoreIfNeeded();
			Align();
			if (_currencyBackground.width == _currencyContainer.width)
			{
				SetHighScore(PlayerSave.highScore);
				OnTweenFinish();
			}
		}

		private void OnTweenFinishedIn()
		{
			_coinsLabel.width -= _labelExtention;
			_keysLabel.width -= _labelExtention;
			SetCoins(InventorySave.coins);
			SetKeys(InventorySave.keys);
			if (_scoreLabel.text == "..." && !_popupsToDisplayFullStats.Contains(UIScreenController.Instance.GetCurrentPopupName()))
			{
				_scoreLabel.alpha = 0f;
				SetHighScore(PlayerSave.highScore);
			}
			if (_currencyBackground.width == _currencyContainer.width)
			{
				OnTweenFinish();
			}
		}

		[Button("Expand")]
		private void Expand()
		{
			if (!_isExtended)
			{
				if (UIScreenController.Instance.GetTopScreenName() != "GameOverScreen")
				{
					_coinsLabel.alpha = 1f;
					_dummyCurrency.alpha = 0f;
				}
				if (IsDigitsOnly(_coinsLabel.text) && IsDigitsOnly(_keysLabel.text))
				{
					_isExtended = true;
					return;
				}
				_useTweens = true;
				_isExtended = true;
				TweenAlpha.Begin(_currencyTextWidget.gameObject, _fadeDuration, 0f).AddOnFinished(OnTweenFinishedOut);
			}
		}

		[Button("Shrink")]
		private void Shrink()
		{
			if (!_isExtended)
			{
				return;
			}
			if (IsDigitsOnly(InventorySave.coins.FormatSI()) && IsDigitsOnly(InventorySave.keys.FormatSI()))
			{
				_isExtended = false;
				return;
			}
			_useTweens = true;
			_isExtended = false;
			if (_currencyTextWidget.alpha == 0f)
			{
				OnTweenFinishedIn();
			}
			else
			{
				TweenAlpha.Begin(_currencyTextWidget.gameObject, _fadeDuration, 0f).AddOnFinished(OnTweenFinishedIn);
			}
		}

		private bool IsDigitsOnly(string str)
		{
			foreach (char c in str)
			{
				if (c < '0' || c > '9')
				{
					return false;
				}
			}
			return true;
		}

		public void BeforeCountUp(int newCoinAmount)
		{
			_coinsLabel.text = newCoinAmount.ToString();
			Align();
		}
	}
}
