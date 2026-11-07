using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Data.Managers;
using Data.Save;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using Kiloo.Common.Utility;
using Missions;
using SYBO.Common.Managers;
using SYBO.Components.Utilities;
using SYBO.Subway.Info;
using UI.Generic;
using UI.Generic.Layout;
using UI.Screens.RewardManagement;
using UnityEngine;
using Utilities;
using WeeklyHunt;

namespace UI.Popups
{
	public class CelebrationPopup : UIPopup
	{
		private const float MOVE_IN_NEXT_CELEBRATION_DURATION = 0.35f;

		private const float REMOVE_CELEBRATION_POPUP_AFTER_OPEN = 0.7f;

		private const float SHOW_PRIZE_BUTTON_POSITION = -46f;

		public GameObject boxParent;

		public GameObject rewardLabelTemplate;

		private Vector3 _outOfScreenPosition = new Vector3(0f, -500f, 0f);

		[SerializeField]
		private GameObject _prizeButtonNormal;

		[SerializeField]
		private GameObject _prizeButtonSuper;

		[SerializeField]
		private GameObject _prizeButtonMini;

		[SerializeField]
		private GameObject _prizeButtonToken;

		[SerializeField]
		private UILabel _prizeButtonNormalLabel;

		[SerializeField]
		private UILabel _prizeButtonSuperLabel;

		[SerializeField]
		private UILabel _prizeButtonMiniLabel;

		[SerializeField]
		private UILabel _prizeButtonTokenLabel;

		[SerializeField]
		private Layout _prizeButtonNormalLayout;

		[SerializeField]
		private Layout _prizeButtonSuperLayout;

		[SerializeField]
		private Layout _prizeButtonMiniLayout;

		[SerializeField]
		private Layout _prizeButtonTokenLayout;

		[SerializeField]
		private Collider openButtonCollider;

		[SerializeField]
		private Collider skipButtonCollider;

		[SerializeField]
		private Collider continueButtonCollider;

		[SerializeField]
		private GameObject tapToStartLabelPrefab;

		private GameObject _coinbox;

		private UILabel _coinboxLabel;

		public UILabel newTrophyLabel;

		private static readonly Vector3 FIRST_SLOT_POSITION = new Vector3(0f, 15f, -50f);

		private static readonly Vector3 OTHER_SLOT_OFFSET = new Vector3(0f, 0f, 9370f);

		private static readonly Vector3 SUPERBOX_EFFECT_POSITION = new Vector3(0f, 0f, 180f);

		private GameObject[] _slots = new GameObject[0];

		[SerializeField]
		private GameObject achievementBagPrefab;

		[SerializeField]
		private GameObject mysteryEggPrefab;

		[SerializeField]
		private GameObject mysteryBoxPrefab;

		[SerializeField]
		private GameObject superMysteryBoxPrefab;

		[SerializeField]
		private GameObject miniMysteryBoxPrefab;

		[SerializeField]
		private GameObject hiddenPrizePrefab_blue;

		[SerializeField]
		private GameObject hiddenPrizePrefab_green;

		[SerializeField]
		private GameObject hiddenPrizePrefab_red;

		[SerializeField]
		private GameObject tokenMysteryBoxPrefab;

		[SerializeField]
		private GameObject superMysteryBoxEffectPrefab;

		[SerializeField]
		private GameObject roundGlowEffectPrefab;

		public GameObject testRewardPrefab;

		public GameObject rewardCoins;

		public GameObject rewardCoinsJackpot;

		[SerializeField]
		private GameObject rewardTokenTricky;

		[SerializeField]
		private GameObject rewardTokenSlick;

		[SerializeField]
		private GameObject rewardTokenFresh;

		[SerializeField]
		private GameObject rewardTokenSpike;

		[SerializeField]
		private GameObject rewardTokenYutani;

		[SerializeField]
		private GameObject trophyDiamond;

		[SerializeField]
		private GameObject trophyGoldBar;

		[SerializeField]
		private GameObject trophyChainClock;

		[SerializeField]
		private GameObject trophyChainDollar;

		[SerializeField]
		private GameObject trophyGoldSkull;

		[SerializeField]
		private GameObject trophyHeadphones;

		[SerializeField]
		private GameObject trophyLPBlack;

		[SerializeField]
		private GameObject trophyTapeBlack;

		[SerializeField]
		private GameObject rewardPowerupHoverboard;

		[SerializeField]
		private GameObject rewardPowerupHeadstart500;

		[SerializeField]
		private GameObject rewardPowerupHeadstart2000;

		[SerializeField]
		private GameObject rewardPowerupScoreBooster;

		[SerializeField]
		private GameObject rewardSpecialBoard;

		[SerializeField]
		private GameObject rewardWelcomePackBoard;

		[SerializeField]
		private GameObject rewardKey;

		[SerializeField]
		private GameObject rewardBronzeCan;

		[SerializeField]
		private GameObject rewardSilverCan;

		[SerializeField]
		private GameObject rewardGoldCan;

		[SerializeField]
		private GameObject rewardDiamondCan;

		[SerializeField]
		private GameObject[] rewardsTopRun;

		[SerializeField]
		private GameObject _weeklyHuntTopTitle;

		[SerializeField]
		private UILabel _weeklyHuntTopTitleLabel;

		[SerializeField]
		private Layout _weeklyHuntTopLayout;

		[SerializeField]
		private GameObject fleshEffectPrefab;

		[SerializeField]
		private GameObject preOpenLabel;

		[SerializeField]
		private UILabel _bonusTextLabel;

		[SerializeField]
		private GameObject _tokenCounter;

		[SerializeField]
		private UILabel _tokenCounterLabel;

		[SerializeField]
		private UISprite _tokenCounterSprite;

		private GameObject _flashEffectGO;

		private UISprite _flashEffectSprite;

		private UILabel _tapToStartLabel;

		public AudioStateLoop audioStateLoop;

		private int _currentReward;

		private GameObject[] _celebrationRewardContainer;

		private bool _anotherReward = true;

		private Vector3 _boxScale = new Vector3(1150f, 1150f, 1150f);

		private Vector3 _boxRotation = new Vector3(0f, 250f, 20f);

		private Vector3 _boxScaleAward = new Vector3(10f, 10f, 10f);

		private Vector3 _boxRotationAward = new Vector3(20f, 220f, 12f);

		private Vector3 _boxPositionAward = new Vector3(0f, 0f, 0f);

		private Vector3 _boxScaleTopRun = new Vector3(19f, 19f, 19f);

		private Vector3 _miniboxScale = new Vector3(900f, 900f, 900f);

		private Vector3 _boxRotationTopRun = new Vector3(0f, -180f, 0f);

		private Vector3 _boxPositionTopRun = new Vector3(5f, -97f, 0f);

		private Vector3 _rewardStartRotation = new Vector3(15f, -10.5f, 0f);

		private Vector3 _rewardStartScale = new Vector3(4f, 4f, 4f);

		private Vector3 _rewardStartScaleCans = new Vector3(7f, 7f, 7f);

		private Vector3 _rewardStartRotationJackpot = new Vector3(0f, 180f, 0f);

		private Vector3 _rewardStartRotationCans = new Vector3(15f, 180f, 0f);

		private Vector3 _rewardStartRotationTopRunRank = new Vector3(-10f, -180f, 0f);

		private Vector3 _rewardStartRotationTopRunMedal = new Vector3(-10f, -180f, 0f);

		private Vector3 _rewardStartPositionCans = new Vector3(0f, 0f, 0f);

		private Vector3 _rewardStartScaleJackpot = new Vector3(900f, 900f, 900f);

		private Vector3 _rewardEndScaleCoin = new Vector3(10f, 10f, 10f);

		private Vector3 _rewardEndScaleCoinSafeAreaSpecific = new Vector3(9f, 9f, 9f);

		private Vector3 _rewardEndScaleCoinJackpot = new Vector3(1050f, 1050f, 1050f);

		private Vector3 _rewardEndPositionJackpot = new Vector3(0f, -95f, 0f);

		private Vector3 _rewardEndScale = new Vector3(18f, 18f, 18f);

		private Vector3 _rewardEndScaleToken = new Vector3(16f, 16f, 16f);

		private Vector3 _rewardEndScaleCan = new Vector3(35f, 35f, 35f);

		private Vector3 _rewardEndScaleTopRunRank = new Vector3(14f, 14f, 14f);

		private Vector3 _rewardEndPositionCan = new Vector3(0f, 0f, 0f);

		private Vector3 _rewardEndPositionTopRunRank = new Vector3(0f, -25f, 0f);

		private Vector3 _rewardEndPositionTopRunMedal = new Vector3(0f, 15f, 0f);

		private GameObject _rewardMain;

		private UILabel _labelSingle;

		private UILabel _labelDouble1;

		private Vector3 _labelPosition = new Vector3(0f, -110f, -5f);

		private Vector3 _labelPositionCelebration = new Vector3(0f, -160f, -5f);

		private Vector3 _labelPositionBoardBundleCelebration = new Vector3(0f, -140f, -5f);

		private Vector3 _continueLabelPosition = new Vector3(0f, -223f, -5f);

		private readonly Vector3 _coinBoxPosition = new Vector3(20f, 16f);

		private bool _maySetTimeScale;

		public GameObject GlowEffect;

		private Color32 EFFECT_INITIAL_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);

		private float TAP_TO_START_ALPHA = 0.7f;

		private const float BOX_IDLE_ANIM_SPEED = 2f;

		private const float BOX_IDLE_ANIM_AMOUNT = 10f;

		private const float BOX_IDLE_ANIM_END_SPEED = 20f;

		private bool _stopIdleAnim;

		private int _numberOfRewards;

		private CoinBoxSizer _coinboxSizer;

		private CelebrationReward[] _rewardsToUnlock;

		private bool _celebrationHasStarted;

		private bool _isFingerPressed;

		private SuperMysteryBoxEffect _superMysteryBoxEffect;

		private SuperMysteryBoxEffect _roundGlowEffect;

		private float _timeScaleBeforeCelebrationPopup = 1f;

		[SerializeField]
		private Texture2D awardsBackground;

		[SerializeField]
		private Texture2D normalBackground;

		[SerializeField]
		private Texture2D _weeklyHuntBackground;

		[SerializeField]
		private Texture2D _weeklyHuntStripe;

		[SerializeField]
		private GameObject stripePanel;

		[SerializeField]
		private Texture2D characterEventBackgroundTexture;

		[SerializeField]
		private UITexture backgroundTexture;

		private UITexture[] _stripeTextures;

		private IEnumerator _stripeCoroutine;

		private bool _stripeCoroutineRunning;

		private GameObject _tapToStartGo;

		private bool _isRewardingJackpot;

		[SerializeField]
		private BragCelebrationHandler _bragCelebrationHandlerPrefab;

		private BragCelebrationHandler _currentBragCelebrationHandler;

		[SerializeField]
		private MysteryBoxMultiplyButton _mysteryBoxMultiplyButton;

		[SerializeField]
		private Vector3 _scaleTargetFactor = new Vector3(1.1f, 1.25f, 1.25f);

		[SerializeField]
		private UITweener.Method _scaleUpMethod = UITweener.Method.EaseOut;

		[SerializeField]
		private UITweener.Method _scaleDownMethod = UITweener.Method.EaseIn;

		[SerializeField]
		private float _scaleUpDuration = 0.0875f;

		[SerializeField]
		private float _scaleDownDurationFactor = 1.5f;

		[SerializeField]
		private AudioClip _multiplyRewardClip;

		private Dictionary<CelebrationRewardType, Action> _multiplyRewardCallbacks;

		private UIPanel _rootPanel;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<CelebrationReward> OnRewardMultiplied__BackingField;

		private float _animationLerpFactor;

		private bool _isWaitingForInput;

		private bool _skipWaitBackButtonPressed;

		private const float MULTIPLY_REWARD_DELAY = 0.5f;

		private const float MULTIPLY_SFX_VOLUME = 0.7f;

		private CelebrationReward _rewardToMultiply;

		private CelebrationPopupLabelTemplate _rewardTemplate;

		public static event Action<CelebrationReward> OnRewardMultiplied
		{
			add
			{
				Action<CelebrationReward> action = OnRewardMultiplied__BackingField;
				Action<CelebrationReward> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnRewardMultiplied__BackingField, (Action<CelebrationReward>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<CelebrationReward> action = OnRewardMultiplied__BackingField;
				Action<CelebrationReward> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnRewardMultiplied__BackingField, (Action<CelebrationReward>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		protected override void Init()
		{
			_rootPanel = GetComponent<UIPanel>();
			audioStateLoop = this.FindObject<AudioStateLoop>();
			Vector3 coinBoxPosition = _coinBoxPosition;
			float x = coinBoxPosition.x;
			Vector3 coinBoxPosition2 = _coinBoxPosition;
			_coinbox = InitializeCoinbox(null, false, true, true, x, coinBoxPosition2.y);
			_coinboxSizer = _coinbox.GetComponentInChildren<CoinBoxSizer>();
			_coinboxLabel = _coinboxSizer.coinAmountLabel;
			_weeklyHuntTopTitleLabel.text = Strings.Get("CELEBRATION_ITEM_HUNT_TOP_TITLE");
			newTrophyLabel.text = Strings.Get("CELEBRATION_POPUP_NEW_TROPHY");
			_multiplyRewardCallbacks = new Dictionary<CelebrationRewardType, Action>
			{
				{
					CelebrationRewardType.coins,
					MultiplyCoins
				},
				{
					CelebrationRewardType.keys,
					MultiplyKeys
				},
				{
					CelebrationRewardType.token,
					MultiplyTokens
				},
				{
					CelebrationRewardType.powerup,
					MultiplyPowerups
				}
			};
			_tapToStartGo = UIScreenController.Instance.CameraOverlay2d.gameObject.AddChild(tapToStartLabelPrefab);
			_tapToStartGo.transform.localPosition = _continueLabelPosition;
			_tapToStartLabel = _tapToStartGo.GetComponentInChildren<UILabel>();
			_tapToStartGo.GetComponent<UIPanel>().depth = _rootPanel.depth + 1;
			_flashEffectGO = UIScreenController.Instance.CameraOverlay2d.gameObject.AddChild(fleshEffectPrefab);
			_flashEffectSprite = _flashEffectGO.GetComponentInChildren<UISprite>();
			_flashEffectGO.GetComponent<UIPanel>().depth = _rootPanel.depth + 1;
			_prizeButtonNormalLabel.text = Strings.Get("PRIZE_POPUP_TITLE");
			_prizeButtonSuperLabel.text = Strings.Get("PRIZE_POPUP_TITLE");
			_prizeButtonMiniLabel.text = Strings.Get("PRIZE_POPUP_TITLE");
			_prizeButtonTokenLabel.text = Strings.Get("PRIZE_POPUP_TITLE");
			_bonusTextLabel.text = Strings.Get("CELEBRATION_POPUP_STICKER_BONUS");
			if (SafeArea.IsRestricted)
			{
				float posY = _tokenCounter.transform.localPosition.y - (float)SafeArea.OFFSET_TOP;
				_tokenCounter.transform.SetLocalPosY(posY);
			}
		}

		protected override void Show()
		{
			UIScreenController.OnPopupClosed += OnPopClosed;
			_timeScaleBeforeCelebrationPopup = SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale;
			SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScaleInMenuDefault;
			GlowEffect.GetComponent<MeshRenderer>().enabled = false;
			SetupCelebrationScreen();
		}

		protected override void Hide()
		{
			UIScreenController.OnPopupClosed -= OnPopClosed;
			SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = _timeScaleBeforeCelebrationPopup;
			_stripeCoroutineRunning = false;
			CoroutineUtil.StopCoroutine(_stripeCoroutine);
			backgroundTexture.mainTexture = normalBackground;
			NGUITools.SetActive(_tapToStartGo, false);
			NGUITools.SetActive(_flashEffectGO, false);
		}

		public override bool ShouldShowTopBar()
		{
			return false;
		}

		public override void BackButtonPressed()
		{
			if (openButtonCollider.enabled)
			{
				FingerPressed();
				FingerReleased();
			}
			if (skipButtonCollider.enabled)
			{
				SkipNow();
			}
			if (continueButtonCollider.enabled && _isWaitingForInput)
			{
				_skipWaitBackButtonPressed = true;
			}
		}

		private void SetupCelebrationScreen()
		{
			boxParent.transform.position = UIModelController.Instance.CelebrationPopupAnchor.transform.position;
			_rewardsToUnlock = RewardManager.GetRewardsToUnlockForCelebration();
			CelebrationReward[] rewardsToUnlock = _rewardsToUnlock;
			foreach (CelebrationReward celebrationReward in rewardsToUnlock)
			{
				switch (celebrationReward.CelebrationRewardOrigin)
				{
				case CelebrationRewardOrigin.MysteryBox:
					Flurry.LogEvent("Mystery Box opened");
					break;
				case CelebrationRewardOrigin.SuperMysteryBox:
				{
					bool flag = false;
					if (EventsSave.willShowSMBAfterDailyChallenge)
					{
						flag = true;
						EventsSave.willShowSMBAfterDailyChallenge = false;
					}
					Flurry.LogEventWithAParameter("Super mystery box opened", "SMB from daily challenge", flag.ToString());
					break;
				}
				case CelebrationRewardOrigin.MysteryBoxMini:
					Flurry.LogEvent("Mini Mystery Box opened");
					break;
				}
			}
			_numberOfRewards = _rewardsToUnlock.Length;
			if (_numberOfRewards <= 0)
			{
				SDebug.LogError("You should not be here when have no celebration items");
				_anotherReward = false;
				openButtonCollider.enabled = false;
				Kiloo.Common.Timer.Timer.ExecuteNextFrame(() =>
				{
					UIScreenController.Instance.ignoreLastPopupClosed = true;
					UIScreenController.Instance.CloseActivePopups(screenName);
				});
				return;
			}
			if (_numberOfRewards == 1)
			{
				_anotherReward = false;
			}
			else
			{
				_anotherReward = true;
			}
			TogglePrizeButtons(_rewardsToUnlock[0].CelebrationRewardOrigin);
			NGUITools.SetActive(_tapToStartGo, true);
			_tapToStartLabel.alpha = 0f;
			NGUITools.SetActive(_flashEffectGO, true);
			_flashEffectSprite.alpha = 0f;
			newTrophyLabel.gameObject.SetActive(false);
			GameObject[] slots = _slots;
			foreach (GameObject obj in slots)
			{
				UnityEngine.Object.Destroy(obj);
			}
			_slots = new GameObject[_numberOfRewards];
			_celebrationRewardContainer = new GameObject[_numberOfRewards];
			for (int num2 = 0; num2 < _numberOfRewards; num2++)
			{
				_slots[num2] = boxParent.AddChild();
				_slots[num2].transform.localPosition = FIRST_SLOT_POSITION + OTHER_SLOT_OFFSET * ((num2 > 0) ? 1 : 0);
			}
			FillOutAllTheSlotsWithRewardContainers();
			_currentReward = 0;
			if (superMysteryBoxEffectPrefab != null && roundGlowEffectPrefab != null)
			{
				GameObject gameObject = ((_rewardsToUnlock[_currentReward].CelebrationRewardOrigin == CelebrationRewardOrigin.Award || _rewardsToUnlock[_currentReward].CelebrationRewardOrigin == CelebrationRewardOrigin.TopRun) ? _slots[_currentReward].AddChild(roundGlowEffectPrefab) : _slots[_currentReward].AddChild(superMysteryBoxEffectPrefab));
				gameObject.transform.transform.localPosition = SUPERBOX_EFFECT_POSITION;
				_superMysteryBoxEffect = gameObject.GetComponent<SuperMysteryBoxEffect>();
				if (_superMysteryBoxEffect == null)
				{
					SDebug.LogError("CelebrationPopup ERROR: unable to find SuperMysteryBoxEffect component on superMysteryBoxEffectPrefab", this);
				}
				else
				{
					ToggleSuperMysteryBoxEffect(_rewardsToUnlock[_currentReward].CelebrationRewardOrigin);
				}
			}
			else
			{
				SDebug.LogError("CelebrationPopup ERROR: superMysteryBoxEffectPrefab is not assigned", this);
			}
			UpdateGui(_rewardsToUnlock[_currentReward]);
			if (IsTwoStepsCelebration(_currentReward))
			{
				StartIdleAnimCoroutine(_celebrationRewardContainer[_currentReward].transform, _rewardsToUnlock[_currentReward]);
			}
			else
			{
				StartOneStepUnlock();
			}
		}

		private void FillOutAllTheSlotsWithRewardContainers()
		{
			for (int i = 0; i < _numberOfRewards; i++)
			{
				GameObject gameObject = null;
				CelebrationRewardOrigin celebrationRewardOrigin = _rewardsToUnlock[i].CelebrationRewardOrigin;
				bool flag = true;
				switch (celebrationRewardOrigin)
				{
				case CelebrationRewardOrigin.MysteryBox:
				case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
					gameObject = _slots[i].AddChild(mysteryBoxPrefab);
					flag = false;
					break;
				case CelebrationRewardOrigin.SuperMysteryBox:
				case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
				case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
					gameObject = _slots[i].AddChild(superMysteryBoxPrefab);
					flag = false;
					break;
				case CelebrationRewardOrigin.MysteryBoxMini:
					gameObject = _slots[i].AddChild(miniMysteryBoxPrefab);
					flag = false;
					break;
				case CelebrationRewardOrigin.TokenMysteryBox:
				case CelebrationRewardOrigin.WeeklyChallengeTokenMysteryBox:
					gameObject = _slots[i].AddChild(tokenMysteryBoxPrefab);
					flag = false;
					break;
				case CelebrationRewardOrigin.WeeklyChallengeHiddenPrize:
				{
					GameObject prefab;
					switch (WeeklyHuntData.GetActivePeriod())
					{
					case WeeklyHuntData.PrizePeriodId.PeriodOne:
						prefab = hiddenPrizePrefab_blue;
						break;
					case WeeklyHuntData.PrizePeriodId.PeriodTwo:
						prefab = hiddenPrizePrefab_green;
						break;
					case WeeklyHuntData.PrizePeriodId.PeriodThree:
						prefab = hiddenPrizePrefab_red;
						break;
					default:
						prefab = hiddenPrizePrefab_blue;
						SDebug.LogError("Weekly Hunt Period not available!");
						break;
					}
					gameObject = _slots[i].AddChild(prefab);
					flag = false;
					break;
				}
				case CelebrationRewardOrigin.Award:
					gameObject = _slots[i].AddChild(achievementBagPrefab);
					flag = false;
					break;
				case CelebrationRewardOrigin.TopRun:
					gameObject = ShowModel(PlayerSave.currentCharacterId, PlayerSave.currentOutfitIndex, UIModelController.ModelScreen.CelebrationTopRun, false);
					gameObject.transform.parent = _slots[i].transform;
					flag = false;
					break;
				}
				if (!flag)
				{
					switch (celebrationRewardOrigin)
					{
					case CelebrationRewardOrigin.Award:
						gameObject.transform.localScale = _boxScaleAward;
						gameObject.transform.localRotation = Quaternion.Euler(_boxRotationAward);
						gameObject.transform.localPosition = _boxPositionAward;
						break;
					case CelebrationRewardOrigin.TopRun:
						gameObject.transform.localScale = _boxScaleTopRun;
						gameObject.transform.localRotation = Quaternion.Euler(_boxRotationTopRun);
						gameObject.transform.localPosition = _boxPositionTopRun;
						break;
					case CelebrationRewardOrigin.MysteryBoxMini:
						gameObject.transform.localScale = _miniboxScale;
						gameObject.transform.localRotation = Quaternion.Euler(_boxRotation);
						break;
					default:
						gameObject.transform.localScale = _boxScale;
						gameObject.transform.localRotation = Quaternion.Euler(_boxRotation);
						break;
					}
					Utility.SetLayerRecursively(gameObject.transform, boxParent.layer);
					_celebrationRewardContainer[i] = gameObject;
				}
				else
				{
					_celebrationRewardContainer[i] = null;
				}
			}
			GlowEffect.GetComponent<MeshRenderer>().material.SetColor("_MainColor", Color.black);
		}

		private void UpdateGui(CelebrationReward reward)
		{
			newTrophyLabel.gameObject.SetActive(false);
			_tokenCounter.SetActive(false);
			_weeklyHuntTopTitle.SetActive(false);
			switch (reward.CelebrationRewardOrigin)
			{
			case CelebrationRewardOrigin.MysteryBox:
			case CelebrationRewardOrigin.SuperMysteryBox:
			case CelebrationRewardOrigin.MysteryBoxMini:
			case CelebrationRewardOrigin.UnlockItem:
			case CelebrationRewardOrigin.TokenMysteryBox:
			case CelebrationRewardOrigin.PromoCode:
				_coinbox.gameObject.SetActive(true);
				ResetBackgroundToNormal();
				openButtonCollider.enabled = true;
				break;
			case CelebrationRewardOrigin.BundleUnlock:
				_coinbox.gameObject.SetActive(true);
				ResetBackgroundToNormal();
				openButtonCollider.enabled = true;
				break;
			case CelebrationRewardOrigin.WeeklyChallengeNormal:
				_coinbox.gameObject.SetActive(false);
				CoroutineUtil.StopCoroutine(_stripeCoroutine);
				_stripeCoroutine = AnimateStripes();
				CoroutineUtil.StartCoroutine(_stripeCoroutine);
				_weeklyHuntTopTitle.SetActive(true);
				openButtonCollider.enabled = false;
				break;
			case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
			case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
			case CelebrationRewardOrigin.WeeklyChallengeHiddenPrize:
			case CelebrationRewardOrigin.WeeklyChallengeTokenMysteryBox:
				_coinbox.gameObject.SetActive(false);
				CoroutineUtil.StopCoroutine(_stripeCoroutine);
				_stripeCoroutine = AnimateStripes();
				CoroutineUtil.StartCoroutine(_stripeCoroutine);
				_weeklyHuntTopTitle.SetActive(true);
				openButtonCollider.enabled = true;
				break;
			case CelebrationRewardOrigin.CharacterUnlock:
			case CelebrationRewardOrigin.BoardUnlock:
			case CelebrationRewardOrigin.BoardBundleUnlock:
				_coinbox.gameObject.SetActive(false);
				ResetBackgroundToNormal();
				openButtonCollider.enabled = false;
				break;
			case CelebrationRewardOrigin.NewHighScore:
				_coinbox.gameObject.SetActive(false);
				ResetBackgroundToNormal();
				openButtonCollider.enabled = false;
				_currentBragCelebrationHandler = UIScreenController.Instance.CameraOverlay2d.gameObject.AddChild(_bragCelebrationHandlerPrefab.gameObject).GetComponent<BragCelebrationHandler>();
				Utility.SetLayerRecursively(_currentBragCelebrationHandler.gameObject.transform, UIScreenController.Instance.CameraOverlay2d.gameObject.layer);
				break;
			case CelebrationRewardOrigin.Award:
				_coinbox.gameObject.SetActive(true);
				ResetBackgroundToNormal();
				openButtonCollider.enabled = true;
				backgroundTexture.mainTexture = awardsBackground;
				break;
			case CelebrationRewardOrigin.TopRun:
				_coinbox.gameObject.SetActive(false);
				ResetBackgroundToNormal();
				openButtonCollider.enabled = true;
				backgroundTexture.mainTexture = awardsBackground;
				break;
			case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
				_coinbox.gameObject.SetActive(true);
				ResetBackgroundToNormal();
				backgroundTexture.mainTexture = characterEventBackgroundTexture;
				openButtonCollider.enabled = true;
				SetPreOpenedCelebrationLabelTemplate(reward);
				break;
			}
			if (_weeklyHuntTopTitleLabel.isActiveAndEnabled)
			{
				_weeklyHuntTopLayout.Align();
			}
		}

		private void SetPreOpenedCelebrationLabelTemplate(CelebrationReward reward)
		{
			if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterEventSuperMysteryBox)
			{
				preOpenLabel = UIScreenController.Instance.CameraOverlay2d.gameObject.AddChild(rewardLabelTemplate);
				preOpenLabel.transform.localPosition = _labelPosition;
				CelebrationPopupLabelTemplate component = preOpenLabel.GetComponent<CelebrationPopupLabelTemplate>();
				component.SetupCharacterWordEvent(reward.characterEventCompleted, reward.description);
			}
		}

		private void ToggleSuperMysteryBoxEffect(CelebrationRewardOrigin origin)
		{
			if (_superMysteryBoxEffect != null)
			{
				switch (origin)
				{
				case CelebrationRewardOrigin.SuperMysteryBox:
				case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
				case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
				case CelebrationRewardOrigin.WeeklyChallengeHiddenPrize:
				case CelebrationRewardOrigin.Award:
				case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
				case CelebrationRewardOrigin.TokenMysteryBox:
				case CelebrationRewardOrigin.WeeklyChallengeTokenMysteryBox:
					_superMysteryBoxEffect.FastForwardEffect(3);
					_superMysteryBoxEffect.StartEffect();
					break;
				default:
					_superMysteryBoxEffect.StopEffect();
					break;
				}
			}
		}

		private bool IsTwoStepsCelebration(int rewardIndex)
		{
			if (_celebrationRewardContainer[rewardIndex] == null)
			{
				return false;
			}
			return true;
		}

		private void StartIdleAnimCoroutine(Transform rewardTrans, CelebrationReward reward)
		{
			if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.TopRun)
			{
				_tapToStartLabel.text = Strings.Get("CELEBRATION_POPUP_CONTINUE");
				_tapToStartLabel.alpha = TAP_TO_START_ALPHA;
			}
			else
			{
				_tapToStartLabel.text = Strings.Get("CELEBRATION_POPUP_OPEN");
				_tapToStartLabel.alpha = TAP_TO_START_ALPHA;
				StartCoroutine(BoxIdleAnimCoroutine(rewardTrans, reward));
			}
		}

		private IEnumerator BoxIdleAnimCoroutine(Transform rewardTrans, CelebrationReward reward)
		{
			if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeTokenMysteryBox)
			{
				audioStateLoop.PlayUnlockSound();
			}
			Vector3 baseLocalPos = rewardTrans.parent.localPosition;
			_stopIdleAnim = false;
			float t = KRandom.Range(0f, 6f);
			Vector3 newLocalPos = baseLocalPos;
			while (!_stopIdleAnim)
			{
				t += Time.deltaTime;
				newLocalPos.y = baseLocalPos.y + Mathf.Sin(t * 2f) * 10f;
				rewardTrans.parent.localPosition = newLocalPos;
				yield return null;
			}
			bool doneResetting = false;
			while (!doneResetting)
			{
				newLocalPos.y = Mathf.MoveTowards(newLocalPos.y, baseLocalPos.y, Time.deltaTime * 20f);
				if (Mathf.Approximately(newLocalPos.y, baseLocalPos.y))
				{
					doneResetting = true;
				}
				rewardTrans.parent.localPosition = newLocalPos;
				yield return null;
			}
		}

		private IEnumerator StartIdleAnimationFor(Transform rewardTrans, Vector3 baseLocalPos)
		{
			if (rewardTrans != null)
			{
				float t = KRandom.Range(0f, 6f);
				Vector3 newLocalPos = baseLocalPos;
				while (rewardTrans != null)
				{
					t += Time.deltaTime;
					newLocalPos.y = baseLocalPos.y + Mathf.Sin(t * 2f) * 10f;
					rewardTrans.localPosition = newLocalPos;
					yield return null;
				}
			}
		}

		private void PlayTopRunAnimation(bool didHighFive)
		{
			if (_celebrationHasStarted)
			{
				return;
			}
			_celebrationHasStarted = true;
			openButtonCollider.enabled = false;
			if (!IsTwoStepsCelebration(_currentReward))
			{
				return;
			}
			CharacterModel component = _celebrationRewardContainer[_currentReward].GetComponent<CharacterModel>();
			if (!(component != null))
			{
				return;
			}
			TopRunIdleAnimation topRunIdleAnimation = component.TopRunIdleAnimation;
			if (!(topRunIdleAnimation != null))
			{
				return;
			}
			topRunIdleAnimation.AnimationFinished += OnTopRunAnimationFinished;
			if (didHighFive)
			{
				if (audioStateLoop != null)
				{
					audioStateLoop.PlayUnlockSound();
				}
				StartCoroutine(AnimateScreenFlash(_flashEffectSprite));
				if (_superMysteryBoxEffect != null)
				{
					_superMysteryBoxEffect.StartEffect();
				}
				topRunIdleAnimation.TR_PlayHit();
			}
			else
			{
				if (audioStateLoop != null)
				{
					audioStateLoop.PlayCelebrationTapSound();
				}
				topRunIdleAnimation.TR_PlayMiss();
			}
		}

		public void OnTopRunAnimationFinished()
		{
			CharacterModel component = _celebrationRewardContainer[_currentReward].GetComponent<CharacterModel>();
			if (component != null)
			{
				TopRunIdleAnimation topRunIdleAnimation = component.TopRunIdleAnimation;
				if (topRunIdleAnimation != null)
				{
					topRunIdleAnimation.AnimationFinished -= OnTopRunAnimationFinished;
				}
			}
			StartCoroutine(_ShowReward(_currentReward));
		}

		private IEnumerator AnimateScreenFlash(UISprite effectSprite)
		{
			effectSprite.color = EFFECT_INITIAL_COLOR;
			yield return StartCoroutine(AnimateEffectColor(effectSprite, effectSprite.color, Color.white, 10f, true));
			StartCoroutine(AnimateEffectColor(effectSprite, effectSprite.color, EFFECT_INITIAL_COLOR, 10f));
		}

		public void FingerPressed()
		{
			if (_isFingerPressed)
			{
				return;
			}
			_isFingerPressed = true;
			_stopIdleAnim = true;
			if (IsTwoStepsCelebration(_currentReward))
			{
				Animation componentInChildren = _celebrationRewardContainer[_currentReward].GetComponentInChildren<Animation>();
				if (componentInChildren != null && (bool)componentInChildren["down"])
				{
					componentInChildren.Play("down");
				}
			}
		}

		private void StartOneStepUnlock()
		{
			FingerReleased();
		}

		public void FingerReleased()
		{
			if (preOpenLabel != null)
			{
				UnityEngine.Object.Destroy(preOpenLabel);
			}
			CelebrationRewardOrigin celebrationRewardOrigin = _rewardsToUnlock[_currentReward].CelebrationRewardOrigin;
			if (celebrationRewardOrigin != CelebrationRewardOrigin.TopRun)
			{
				if (!_celebrationHasStarted)
				{
					openButtonCollider.enabled = false;
					HidePrizeButtons();
					if (_superMysteryBoxEffect != null && celebrationRewardOrigin != CelebrationRewardOrigin.Award)
					{
						_superMysteryBoxEffect.StopEffect();
					}
					StartCoroutine(AnimateAlpha(_tapToStartLabel, 0.5f, 0f));
					StartCoroutine(_ShowReward(_currentReward));
					_celebrationHasStarted = true;
				}
			}
			else
			{
				StartCoroutine(AnimateAlpha(_tapToStartLabel, 0.5f, 0f));
				PlayTopRunAnimation(true);
			}
		}

		private IEnumerator _ShowReward(int currentRewardIndex)
		{
			CelebrationReward reward = _rewardsToUnlock[currentRewardIndex];
			switch (reward.CelebrationRewardOrigin)
			{
			case CelebrationRewardOrigin.MysteryBox:
			case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
				Stat.MysteryBoxesOpened.IncrementValue();
				break;
			case CelebrationRewardOrigin.SuperMysteryBox:
			case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
			case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
				Stat.SuperMysteryBoxesOpened.IncrementValue();
				break;
			}
			GlowEffect.GetComponent<MeshRenderer>().enabled = true;
			if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.TopRun)
			{
				if (audioStateLoop != null)
				{
					audioStateLoop.PlayRewardMysterySound();
				}
				StartCoroutine(AnimateColor(GlowEffect.GetComponent<MeshRenderer>().material, 1.5f, Color.white));
				StartCoroutine(RotateGameObject(GlowEffect.transform, 3000f, new Vector3(0f, 0f, -270000f)));
				yield return new WaitForSeconds(0.4f);
				_flashEffectSprite.color = EFFECT_INITIAL_COLOR;
				yield return StartCoroutine(AnimateEffectColor(_flashEffectSprite, _flashEffectSprite.color, Color.white, 4f));
				_celebrationRewardContainer[currentRewardIndex].SetActive(false);
				if (_superMysteryBoxEffect != null)
				{
					_superMysteryBoxEffect.StartEffect();
				}
				StartCoroutine(AnimateEffectColor(_flashEffectSprite, _flashEffectSprite.color, EFFECT_INITIAL_COLOR, 0.4f));
			}
			GameObject rewardGo = null;
			switch (reward.CelebrationRewardOrigin)
			{
			case CelebrationRewardOrigin.CharacterUnlock:
				ShowModel(reward.characterId, reward.characterOutfitIndex, UIModelController.ModelScreen.CelebrationCharacterUnlock, true);
				if (audioStateLoop != null)
				{
					audioStateLoop.PlayUnlockSound();
				}
				break;
			case CelebrationRewardOrigin.BoardUnlock:
			case CelebrationRewardOrigin.BoardBundleUnlock:
				ShowBackgroundWithStripes(reward.boardId, UIModelController.ModelScreen.CelebrationBoardUnlock);
				if (audioStateLoop != null)
				{
					audioStateLoop.PlayUnlockSound();
				}
				break;
			case CelebrationRewardOrigin.NewHighScore:
				if (_currentBragCelebrationHandler.state == BragCelebrationHandler.State.BeatenOnlySelf)
				{
					ShowBackgroundWithStripes(PlayerSave.currentBoardId, UIModelController.ModelScreen.CelebrationHighScore);
				}
				else
				{
					UIModelController.Instance.SetDiscoLightBackground();
				}
				if (audioStateLoop != null)
				{
					audioStateLoop.PlayUnlockSound();
				}
				break;
			default:
				rewardGo = NGUITools.AddChild(prefab: ChooseRewardPrefab(reward), parent: _slots[0]);
				if (IsJackpot(reward))
				{
					rewardGo.transform.localScale = _rewardStartScaleJackpot;
					rewardGo.transform.localRotation = Quaternion.Euler(_rewardStartRotationJackpot);
				}
				else
				{
					switch (reward.CelebrationRewardOrigin)
					{
					case CelebrationRewardOrigin.Award:
						rewardGo.transform.localScale = _rewardStartScaleCans;
						rewardGo.transform.localPosition = _rewardStartPositionCans;
						rewardGo.transform.localRotation = Quaternion.Euler(_rewardStartRotationCans);
						break;
					case CelebrationRewardOrigin.TopRun:
						rewardGo.transform.localScale = _rewardStartScale;
						rewardGo.transform.localRotation = Quaternion.Euler((reward.rank <= 4) ? _rewardStartRotationTopRunMedal : _rewardStartRotationTopRunRank);
						break;
					default:
						rewardGo.transform.localScale = _rewardStartScale;
						rewardGo.transform.localPosition = new Vector3(0f, Mathf.Clamp((float)Screen.height / (float)Screen.width * 3.5f, 0f, 9f), 0f);
						rewardGo.transform.localRotation = Quaternion.Euler(_rewardStartRotation);
						break;
					}
				}
				Utility.SetLayerRecursively(rewardGo.transform, boxParent.layer);
				if (audioStateLoop != null)
				{
					switch (reward.CelebrationRewardOrigin)
					{
					case CelebrationRewardOrigin.WeeklyChallengeNormal:
						audioStateLoop.PlayUnlockSound();
						break;
					default:
						audioStateLoop.PlayCelebrationOpenSound();
						break;
					case CelebrationRewardOrigin.TopRun:
						break;
					}
				}
				if (IsTwoStepsCelebration(currentRewardIndex))
				{
					Animation anim = _celebrationRewardContainer[currentRewardIndex].GetComponentInChildren<Animation>();
					if (anim != null && (bool)anim["up"])
					{
						anim.Play("up");
						while (anim["up"].normalizedTime < 0.5f)
						{
							yield return null;
						}
					}
				}
				_maySetTimeScale = true;
				if (reward.rewardType == CelebrationRewardType.coins)
				{
					if (IsJackpot(reward))
					{
						StartCoroutine(ScaleGameObject(rewardGo.transform, 2f, _rewardEndScaleCoinJackpot));
						Animation componentInChildren = rewardGo.GetComponentInChildren<Animation>();
						if (componentInChildren != null && (bool)componentInChildren["open_superJackpot"])
						{
							componentInChildren.Play("open_superJackpot");
							StartCoroutine(MoveGameObject(rewardGo.transform, 1f, _rewardEndPositionJackpot));
						}
					}
					else if (SafeArea.IsRestricted)
					{
						StartCoroutine(ScaleGameObject(rewardGo.transform, 2f, _rewardEndScaleCoinSafeAreaSpecific));
					}
					else
					{
						StartCoroutine(ScaleGameObject(rewardGo.transform, 2f, _rewardEndScaleCoin));
					}
				}
				else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.Award)
				{
					StartCoroutine(ScaleGameObject(rewardGo.transform, 2f, _rewardEndScaleCan));
					StartCoroutine(MoveGameObject(rewardGo.transform, 2f, _rewardEndPositionCan));
				}
				else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.TopRun)
				{
					if (reward.rank > 4)
					{
						StartCoroutine(ScaleGameObject(rewardGo.transform, 0.3f, _rewardEndScaleTopRunRank));
						StartCoroutine(MoveGameObject(rewardGo.transform, 0.5f, _rewardEndPositionTopRunRank));
					}
					else
					{
						StartCoroutine(ScaleGameObject(rewardGo.transform, 1.5f, _rewardEndScaleTopRunRank));
						StartCoroutine(MoveGameObject(rewardGo.transform, 1.5f, _rewardEndPositionTopRunMedal));
					}
				}
				else if (reward.rewardType == CelebrationRewardType.token)
				{
					StartCoroutine(ScaleGameObject(rewardGo.transform, 2f, _rewardEndScaleToken));
				}
				else
				{
					StartCoroutine(ScaleGameObject(rewardGo.transform, 2f, _rewardEndScale));
				}
				if (IsTwoStepsCelebration(currentRewardIndex))
				{
					StartCoroutine(MoveGameObject(_celebrationRewardContainer[currentRewardIndex].transform, 0.7f, _outOfScreenPosition));
				}
				if (!IsJackpot(reward))
				{
					switch (reward.CelebrationRewardOrigin)
					{
					case CelebrationRewardOrigin.Award:
						StartCoroutine(RotateGameObject(rewardGo.transform, 2f, new Vector3(0f, 1440f, 0f)));
						break;
					case CelebrationRewardOrigin.TopRun:
						if (reward.rank < 5)
						{
							StartCoroutine(RotateGameObject(rewardGo.transform, 2f, new Vector3(0f, 1440f, 0f)));
							StartCoroutine(StartIdleAnimationFor(rewardGo.transform, _rewardEndPositionTopRunMedal));
						}
						else
						{
							StartCoroutine(StartIdleAnimationFor(rewardGo.transform, _rewardEndPositionTopRunRank));
						}
						break;
					default:
						StartCoroutine(RotateGameObject(rewardGo.transform, 4f, new Vector3(0f, 1500f, 0f)));
						break;
					}
				}
				yield return new WaitForSeconds(0.5f);
				switch (reward.CelebrationRewardOrigin)
				{
				case CelebrationRewardOrigin.Award:
					StartCoroutine(AnimateColor(GlowEffect.GetComponent<MeshRenderer>().material, 1.5f, Color.white));
					StartCoroutine(RotateGameObject(GlowEffect.transform, 3000f, new Vector3(0f, 0f, -270000f)));
					break;
				default:
					StartCoroutine(AnimateColor(GlowEffect.GetComponent<MeshRenderer>().material, 1.5f, Color.white));
					StartCoroutine(RotateGameObject(GlowEffect.transform, 3f, new Vector3(0f, 0f, -270f)));
					break;
				case CelebrationRewardOrigin.TopRun:
					break;
				}
				yield return new WaitForSeconds(1.3f);
				break;
			}
			GameObject labelGo = UIScreenController.Instance.CameraOverlay2d.gameObject.AddChild(rewardLabelTemplate);
			if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.BoardBundleUnlock && reward.bundledThemes.Count > 0)
			{
				labelGo.transform.localPosition = _labelPositionBoardBundleCelebration;
			}
			else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.BoardUnlock || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterUnlock || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.BoardBundleUnlock)
			{
				labelGo.transform.localPosition = _labelPositionCelebration;
			}
			else
			{
				labelGo.transform.localPosition = _labelPosition;
			}
			CelebrationPopupLabelTemplate template = labelGo.GetComponent<CelebrationPopupLabelTemplate>();
			template.Init(_rootPanel.depth);
			if (_stripeCoroutineRunning)
			{
				template.bigLabel.color = GlobalColors.WEEKLYHUNT_CELEBRATION_TEXT_COLOR_STRIPED_BACKGROUND;
				template.subLabel.color = GlobalColors.WEEKLYHUNT_CELEBRATION_TEXT_COLOR_STRIPED_BACKGROUND;
				template.bigLabel.effectColor = GlobalColorsGenerated.WEEKLYHUNT_CELEBRATION_TEXT_SHADOW_COLOR_STRIPED_BACKGROUND;
				template.subLabel.effectColor = GlobalColorsGenerated.WEEKLYHUNT_CELEBRATION_TEXT_SHADOW_COLOR_STRIPED_BACKGROUND;
			}
			else
			{
				template.bigLabel.color = GlobalColors.WEEKLYHUNT_CELEBRATION_TEXT_COLOR;
				template.subLabel.color = GlobalColors.WEEKLYHUNT_CELEBRATION_TEXT_COLOR;
				template.bigLabel.effectColor = GlobalColors.WEEKLYHUNT_CELEBRATION_TEXT_SHADOW_COLOR;
				template.subLabel.effectColor = GlobalColors.WEEKLYHUNT_CELEBRATION_TEXT_SHADOW_COLOR;
			}
			bool showNewTrophyLabel = false;
			_tokenCounter.SetActive(false);
			switch (reward.CelebrationRewardOrigin)
			{
			case CelebrationRewardOrigin.Award:
				template.SetupAward(reward.awardTitle, reward.amount, reward.awardTierType);
				break;
			case CelebrationRewardOrigin.TopRun:
				template.SetupTopRun(reward.score, reward.rank, reward.weekNumber);
				break;
			default:
				switch (reward.rewardType)
				{
				case CelebrationRewardType.coins:
					template.SetupCoins();
					break;
				case CelebrationRewardType.powerup:
					template.SetupPowerup(reward.upgradeType, reward.amount);
					break;
				case CelebrationRewardType.token:
				{
					if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeTokenMysteryBox)
					{
						_tokenCounter.gameObject.SetActive(false);
					}
					else
					{
						_tokenCounter.gameObject.SetActive(true);
						_tokenCounterSprite.spriteName = Characters.characterData[reward.characterId].tokenSprite2dName;
						_tokenCounterSprite.GetComponent<FitSpriteToWidget>().FitNow();
						_tokenCounter.GetComponent<TweenAlpha>().ResetToBeginning();
						_tokenCounter.GetComponent<TweenAlpha>().PlayForward();
					}
					int num = Outfits.GetNextTokenUnlockableOutfitIndex(reward.characterId) + 1;
					int num2 = ((num <= 0) ? Characters.characterData[reward.characterId].Price : Outfits.GetOutfitForCharacter(reward.characterId, num).price);
					_tokenCounterLabel.text = string.Format("{0} / {1}", Mathf.Clamp(CharacterManager.GetCollectedTokens(reward.characterId) + reward.amount, 0, num2), num2);
					template.SetupToken(reward.characterId, reward.amount);
					MissionsManager.Instance.PlayerDidThis(MissionTarget.Tokens, reward.amount);
					break;
				}
				case CelebrationRewardType.trophy:
					showNewTrophyLabel = true;
					template.SetupTrophy(reward.trophyType);
					break;
				case CelebrationRewardType.medal:
					showNewTrophyLabel = true;
					template.SetupMedal();
					break;
				case CelebrationRewardType.keys:
					template.SetupKeys(reward.amount);
					break;
				case CelebrationRewardType.character:
				{
					Outfit outfitForCharacter = Outfits.GetOutfitForCharacter(reward.characterId, reward.characterOutfitIndex);
					if (outfitForCharacter != null)
					{
						Outfit outfit = outfitForCharacter;
						template.SetupCharacter(Strings.Get(outfit.title));
					}
					else
					{
						template.SetupCharacter(Strings.Get(Characters.characterData[reward.characterId].name));
					}
					break;
				}
				case CelebrationRewardType.specialBoard:
				case CelebrationRewardType.welcomePackBoard:
				{
					bool found = false;
					Boards.BoardId? boardId = null;
					IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
					if (activeEventManager != null)
					{
						SSEventType eventType = activeEventManager.GetEventType();
						if (eventType == SSEventType.SpecialPowerBoard)
						{
							boardId = activeEventManager.GetEventValue<Boards.BoardId>(out found);
						}
					}
					if (found && boardId.GetValueOrDefault() == reward.boardId && boardId.HasValue)
					{
						template.SetupEventSpecialBoard(Strings.Get(Boards.boardData[reward.boardId].name));
						break;
					}
					if (reward.boardPower == Hoverboard.BoardAbility.None)
					{
						template.SetupSpecialBoard(string.Format(Strings.Get("BOARD_DISPLAY_NAME_WITH_SUFFIX"), Strings.Get(Boards.boardData[reward.boardId].name)));
						break;
					}
					string boardPowerName = HoverboardThemes.GetBoardPowerName(reward.boardId, reward.boardPower);
					template.SetupSpecialBoard(boardPowerName);
					break;
				}
				case CelebrationRewardType.highscore:
					labelGo.SetActive(false);
					break;
				case CelebrationRewardType.boardBundle:
				{
					HoverboardThemes.HoverboardTheme hoverboardTheme = null;
					if (reward.bundledThemes.Count > 0)
					{
						hoverboardTheme = HoverboardThemes.hoverboardCustomThemes[reward.boardId].Find((HoverboardThemes.HoverboardTheme x) => reward.bundledThemes[0] == x.identifier);
					}
					HoverboardThemes.HoverboardTheme hoverboardTheme2 = null;
					if (reward.bundledThemes.Count > 1)
					{
						hoverboardTheme2 = HoverboardThemes.hoverboardCustomThemes[reward.boardId].Find((HoverboardThemes.HoverboardTheme x) => reward.bundledThemes[1] == x.identifier);
					}
					if (hoverboardTheme == null && hoverboardTheme2 != null)
					{
						SDebug.LogError("It shouldn't happen, but theme 2 is set while theme 1 is not");
					}
					template.SetupBoardBundle(reward.boardId, hoverboardTheme, hoverboardTheme2);
					break;
				}
				}
				break;
			}
			newTrophyLabel.gameObject.SetActive(showNewTrophyLabel);
			if (reward.rewardType != CelebrationRewardType.highscore)
			{
				StartCoroutine(AnimateAlpha(template, 0.2f, 1f));
			}
			yield return new WaitForSeconds(0.5f);
			skipButtonCollider.enabled = false;
			int coinboxCountFromAmount = InventorySave.coins;
			PayoutReward(reward, true, false);
			if (reward.rewardType == CelebrationRewardType.coins)
			{
				bool superMysterBoxJackpot = false;
				if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.MysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterEventSuperMysteryBox)
				{
					superMysterBoxJackpot = reward.amount > 300000;
				}
				yield return StartCoroutine(CountUpCoins(coinboxCountFromAmount, reward.amount, template, superMysterBoxJackpot));
			}
			else if (reward.CelebrationRewardOrigin != CelebrationRewardOrigin.CharacterUnlock && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.BoardUnlock && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.BoardBundleUnlock && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.Award && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.TopRun)
			{
				yield return new WaitForSeconds(1f);
			}
			if (_mysteryBoxMultiplyButton.TryEnable(reward, _multiplyRewardCallbacks))
			{
				_rewardToMultiply = reward;
				_rewardTemplate = template;
			}
			if (reward.CelebrationRewardOrigin != CelebrationRewardOrigin.Award && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.TopRun)
			{
				StartCoroutine(AnimateColor(GlowEffect.GetComponent<MeshRenderer>().material, 0.5f, Color.black));
			}
			if (reward.CelebrationRewardOrigin != CelebrationRewardOrigin.CharacterUnlock && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.BoardUnlock && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.BoardBundleUnlock && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.Award && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.TopRun)
			{
				yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.75f));
			}
			_tapToStartLabel.text = Strings.Get("CELEBRATION_POPUP_CONTINUE");
			StartCoroutine(AnimateAlpha(_tapToStartLabel, 0.5f, TAP_TO_START_ALPHA));
			_maySetTimeScale = false;
			SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScaleInMenuDefault;
			_isWaitingForInput = true;
			while (IsWaitingForUserInput())
			{
				yield return null;
			}
			_isWaitingForInput = false;
			_mysteryBoxMultiplyButton.DisableButton();
			switch (reward.CelebrationRewardOrigin)
			{
			case CelebrationRewardOrigin.CharacterUnlock:
				HideModel();
				UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.CelebrationCharacterUnlock);
				break;
			case CelebrationRewardOrigin.BoardUnlock:
			case CelebrationRewardOrigin.BoardBundleUnlock:
				HideSpecialBoard();
				UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.CelebrationBoardUnlock);
				break;
			case CelebrationRewardOrigin.NewHighScore:
				HideModel();
				UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.CelebrationHighScore);
				break;
			case CelebrationRewardOrigin.TopRun:
				UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.CelebrationTopRun);
				break;
			}
			UnityEngine.Object.Destroy(rewardGo);
			UnityEngine.Object.Destroy(labelGo);
			UnityEngine.Object.Destroy(_celebrationRewardContainer[currentRewardIndex]);
			if (_currentBragCelebrationHandler != null)
			{
				UnityEngine.Object.Destroy(_currentBragCelebrationHandler.gameObject);
				_currentBragCelebrationHandler = null;
			}
			_FinishOpening();
		}

		private void PayoutReward(CelebrationReward reward, bool allowRemove, bool isMultipliedReward)
		{
			if (reward.rewardType == CelebrationRewardType.topRun)
			{
				UIScreenController.Instance.justPaidOutTopRun = true;
				UIScreenController.Instance.PayoutCelebrationReward(reward);
			}
			else if (reward.rewardType == CelebrationRewardType.powerup)
			{
				InventorySave.IncreaseAmount(reward.upgradeType, reward.amount);
			}
			else if (reward.rewardType == CelebrationRewardType.token)
			{
				CharacterManager.CollectToken(reward.characterId, reward.amount);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("TokenInfo", string.Concat(reward.characterId, ";", reward.amount));
				dictionary.Add("Source", string.Concat(reward.CelebrationRewardOrigin, ";", reward.amount));
				Flurry.LogEventWithSeveralParameters("Character Token Obtained", dictionary);
			}
			else if (reward.rewardType == CelebrationRewardType.trophy)
			{
				Trophies.OldTrophySpawned(reward.trophyType);
			}
			else if (reward.rewardType == CelebrationRewardType.keys)
			{
				string celebrationRewardSource = GetCelebrationRewardSource(reward);
				InventorySave.IncreaseKeys(reward.amount, celebrationRewardSource);
			}
			else if (reward.rewardType == CelebrationRewardType.coins)
			{
				int amount = reward.amount;
				_coinboxSizer.updateAutomatically = false;
				string celebrationRewardSource2 = GetCelebrationRewardSource(reward);
				InventorySave.IncreaseCoins(reward.amount, celebrationRewardSource2);
				MissionsManager.Instance.PlayerDidThis(MissionTarget.EarnCoin, amount);
				if (IsJackpot(reward) && !isMultipliedReward)
				{
					Stat.TotalJackpotsWon.IncrementValue();
					Stat.JackpotsWon.IncrementValue();
				}
			}
			else if (reward.rewardType == CelebrationRewardType.welcomePackBoard)
			{
				BoardManager.UnlockBoard(reward.boardId);
			}
			if (allowRemove)
			{
				RewardManager.RewardPaidOut(reward);
			}
			SaveManager.Save();
			_isRewardingJackpot = false;
		}

		private string GetCelebrationRewardSource(CelebrationReward reward)
		{
			string text = reward.CelebrationRewardOrigin.ToString();
			if (!string.IsNullOrEmpty(reward.kilooAnalyticsSourceId))
			{
				text = ((reward.CelebrationRewardOrigin != CelebrationRewardOrigin.MysteryBoxMini && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.MysteryBox && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.SuperMysteryBox && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.TokenMysteryBox) ? reward.kilooAnalyticsSourceId : (text + string.Format("From{0}", reward.kilooAnalyticsSourceId)));
			}
			if (reward.unlockItemSource != UnlockItemSource.NotSet)
			{
				text += string.Format("From{0}", reward.unlockItemSource);
			}
			return text;
		}

		private void _FinishOpening()
		{
			_rewardToMultiply = null;
			_rewardTemplate = null;
			_celebrationHasStarted = false;
			_isFingerPressed = false;
			if (_superMysteryBoxEffect != null)
			{
				_superMysteryBoxEffect.StopEffect();
			}
			if (_anotherReward)
			{
				StartCoroutine(MoveNextBoxToFront());
				return;
			}
			GameObject[] slots = _slots;
			foreach (GameObject obj in slots)
			{
				UnityEngine.Object.Destroy(obj);
			}
			ResetBackgroundToNormal();
			skipButtonCollider.enabled = true;
			openButtonCollider.enabled = false;
			NGUITools.SetActive(_tapToStartGo, false);
			NGUITools.SetActive(_flashEffectGO, false);
			if (RewardManager.rewardsToUnlockCount > 0 && RewardManager.canShowMultipleQueuedCelebrations)
			{
				SetupCelebrationScreen();
				return;
			}
			if (RewardManager.canShowMultipleQueuedCelebrations)
			{
				RewardManager.canShowMultipleQueuedCelebrations = false;
			}
			UIScreenController.Instance.ignoreLastPopupClosed = true;
			UIScreenController.Instance.CloseActivePopups(screenName);
		}

		private IEnumerator MoveNextBoxToFront()
		{
			SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScaleInMenuDefault;
			_currentReward++;
			if (_currentReward >= _numberOfRewards - 1)
			{
				_anotherReward = false;
			}
			UpdateGui(_rewardsToUnlock[_currentReward]);
			TogglePrizeButtons(_rewardsToUnlock[_currentReward].CelebrationRewardOrigin);
			GlowEffect.GetComponent<MeshRenderer>().material.SetColor("_MainColor", Color.black);
			ToggleSuperMysteryBoxEffect(_rewardsToUnlock[_currentReward].CelebrationRewardOrigin);
			openButtonCollider.enabled = false;
			if (IsTwoStepsCelebration(_currentReward))
			{
				_celebrationRewardContainer[_currentReward].transform.parent = _slots[0].transform;
				StartCoroutine((_rewardsToUnlock[_currentReward].CelebrationRewardOrigin == CelebrationRewardOrigin.TopRun) ? MoveGameObject(_celebrationRewardContainer[_currentReward].transform, 0.35f, _boxPositionTopRun) : MoveGameObject(_celebrationRewardContainer[_currentReward].transform, 0.35f, Vector3.zero));
				StartIdleAnimCoroutine(_celebrationRewardContainer[_currentReward].transform, _rewardsToUnlock[_currentReward]);
				yield return new WaitForSeconds(0.35f);
				openButtonCollider.enabled = true;
			}
			else
			{
				FingerReleased();
			}
			skipButtonCollider.enabled = true;
		}

		private IEnumerator RotateGameObject(Transform trans, float duration, Vector3 angleToRotate)
		{
			if (trans != null)
			{
				Quaternion fromRotation = trans.localRotation;
				float factor = 0f;
				while (factor < 1f && trans != null)
				{
					factor += Time.deltaTime / duration;
					factor = Mathf.Clamp01(factor);
					float angle = Mathf.Lerp(4.712389f, (float)Math.PI * 2f, factor);
					float cosFactor = Mathf.Cos(angle) * 0.5f + 0.5f;
					trans.localRotation = fromRotation;
					trans.Rotate(angleToRotate * cosFactor, Space.World);
					yield return null;
				}
			}
		}

		private IEnumerator CountUpCoins(int coinboxAmount, int toAmount, CelebrationPopupLabelTemplate rewardTemplate, bool isMegaJackpot = false, int fromAmount = 0, float countTimeMultiplier = 1f, bool multiplied = false)
		{
			_coinboxSizer.updateAutomatically = false;
			float countFactor = 0f;
			float countTime = CalcCoinCountTime(toAmount, fromAmount, countTimeMultiplier);
			int coinboxTo = coinboxAmount + toAmount - fromAmount;
			yield return new WaitForSeconds(0.5f);
			while (countFactor < 1f)
			{
				countFactor += Time.deltaTime / countTime;
				rewardTemplate.UpdateCoins(Mathf.RoundToInt(Mathf.SmoothStep(fromAmount, toAmount, countFactor)), isMegaJackpot, multiplied);
				_coinboxLabel.text = Mathf.RoundToInt(Mathf.SmoothStep(coinboxAmount, coinboxTo, countFactor)).ToString();
				_coinboxSizer.AdjustCoinsSegmentSize();
				yield return null;
			}
			_coinboxSizer.updateAutomatically = true;
		}

		private float CalcCoinCountTime(int toAmount, int fromAmount, float countTimeMultiplier)
		{
			return Mathf.Lerp(1.5f, 4f, (float)(toAmount - fromAmount) / 100000f) * countTimeMultiplier;
		}

		private IEnumerator AnimateAlpha(UILabel label, float duration, float toAlpha)
		{
			float fromAlpha = label.alpha;
			float factor = 0f;
			while (factor < 1f)
			{
				factor += Time.deltaTime / duration;
				factor = Mathf.Clamp01(factor);
				label.alpha = Mathf.Lerp(fromAlpha, toAlpha, factor);
				yield return null;
			}
		}

		private IEnumerator AnimateAlpha(CelebrationPopupLabelTemplate template, float duration, float toAlpha)
		{
			float fromAlpha = template.Alpha;
			float factor = 0f;
			while (factor < 1f)
			{
				factor += Time.deltaTime / duration;
				factor = Mathf.Clamp01(factor);
				template.Alpha = Mathf.Lerp(fromAlpha, toAlpha, factor);
				yield return null;
			}
		}

		private IEnumerator AnimateColor(UIWidget widget, float duration, Color toColor)
		{
			Color fromColor = widget.color;
			float factor = 0f;
			while (factor < 1f)
			{
				factor += Time.deltaTime / duration;
				factor = Mathf.Clamp01(factor);
				widget.color = Color.Lerp(fromColor, toColor, factor);
				yield return null;
			}
		}

		private IEnumerator AnimateColor(Material material, float duration, Color toColor)
		{
			Color fromColor = material.GetColor("_MainColor");
			float factor = 0f;
			while (factor < 1f)
			{
				factor += Time.deltaTime / duration;
				factor = Mathf.Clamp01(factor);
				material.SetColor("_MainColor", Color.Lerp(fromColor, toColor, factor));
				yield return null;
			}
		}

		private IEnumerator AnimateEffectColor(UISprite spriteToAnimate, Color color1, Color color2, float speedFactor = 1f, bool instant = false)
		{
			if (instant)
			{
				spriteToAnimate.color = color2;
				yield break;
			}
			_animationLerpFactor = 0f;
			while (_animationLerpFactor < 1f)
			{
				_animationLerpFactor += Time.deltaTime * speedFactor;
				spriteToAnimate.color = Color.Lerp(color1, color2, _animationLerpFactor);
				yield return null;
			}
		}

		private IEnumerator MoveGameObject(Transform trans, float duration, Vector3 toPos)
		{
			if (trans != null)
			{
				Vector3 fromPos = trans.localPosition;
				float factor = 0f;
				while (factor < 1f && trans != null)
				{
					factor += Time.deltaTime / duration;
					factor = Mathf.Clamp01(factor);
					trans.localPosition = Vector3.Lerp(fromPos, toPos, 0.5f * (Mathf.Sin((factor - 0.5f) * (float)Math.PI) + 1f));
					yield return null;
				}
				if (trans != null)
				{
					trans.localPosition = toPos;
				}
			}
		}

		private IEnumerator ScaleGameObject(Transform trans, float duration, Vector3 toScale)
		{
			if (trans != null)
			{
				float factor = 0f;
				Vector3 fromScale = trans.localScale;
				while (factor < 1f && trans != null)
				{
					factor += Time.deltaTime / duration;
					factor = Mathf.Clamp01(factor);
					float angle = Mathf.Lerp((float)Math.PI, (float)Math.PI * 2f, factor);
					float cosFactor = Mathf.Cos(angle) * 0.5f + 0.5f;
					trans.localScale = Vector3.Lerp(fromScale, toScale, cosFactor);
					yield return null;
				}
			}
		}

		private GameObject ChooseRewardPrefab(CelebrationReward reward)
		{
			GameObject result = rewardCoins;
			if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.Award)
			{
				switch (reward.awardTierType)
				{
				case AwardTierType.Bronze:
					result = rewardBronzeCan;
					break;
				case AwardTierType.Silver:
					result = rewardSilverCan;
					break;
				case AwardTierType.Gold:
					result = rewardGoldCan;
					break;
				case AwardTierType.Diamond:
					result = rewardDiamondCan;
					break;
				}
				return result;
			}
			if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.TopRun)
			{
				int num = reward.rank - 1;
				if (num <= rewardsTopRun.Length)
				{
					return rewardsTopRun[num];
				}
				result = rewardsTopRun[rewardsTopRun.Length - 1];
			}
			switch (reward.rewardType)
			{
			case CelebrationRewardType.coins:
				result = ((!IsJackpot(reward)) ? rewardCoins : rewardCoinsJackpot);
				break;
			case CelebrationRewardType.powerup:
				switch (reward.upgradeType)
				{
				case UpgradeType.hoverboard:
					result = rewardPowerupHoverboard;
					break;
				case UpgradeType.headstart2000:
					result = rewardPowerupHeadstart2000;
					break;
				case UpgradeType.scorebooster:
					result = rewardPowerupScoreBooster;
					break;
				}
				break;
			case CelebrationRewardType.token:
				switch (reward.characterId)
				{
				case Characters.CharacterId.slick:
					result = rewardTokenSlick;
					break;
				case Characters.CharacterId.fresh:
					result = rewardTokenFresh;
					break;
				case Characters.CharacterId.tricky:
					result = rewardTokenTricky;
					break;
				case Characters.CharacterId.spike:
					result = rewardTokenSpike;
					break;
				case Characters.CharacterId.yutani:
					result = rewardTokenYutani;
					MissionsManager.Instance.PlayerDidThis(MissionTarget.HaveYutani);
					break;
				}
				break;
			case CelebrationRewardType.trophy:
				switch (reward.trophyType)
				{
				case Trophies.Trophy.diamond:
					result = trophyDiamond;
					break;
				case Trophies.Trophy.goldbar:
					result = trophyGoldBar;
					break;
				case Trophies.Trophy.goldChainClock:
					result = trophyChainClock;
					break;
				case Trophies.Trophy.goldChainDollar:
					result = trophyChainDollar;
					break;
				case Trophies.Trophy.goldSkull:
					result = trophyGoldSkull;
					break;
				case Trophies.Trophy.headphones:
					result = trophyHeadphones;
					break;
				case Trophies.Trophy.lpBlack:
					result = trophyLPBlack;
					break;
				case Trophies.Trophy.tapeBlack:
					result = trophyTapeBlack;
					break;
				default:
					SDebug.LogError(string.Concat(reward.trophyType, string.Empty));
					break;
				}
				break;
			case CelebrationRewardType.keys:
				result = rewardKey;
				break;
			case CelebrationRewardType.specialBoard:
				result = rewardSpecialBoard;
				break;
			case CelebrationRewardType.welcomePackBoard:
				result = rewardWelcomePackBoard;
				break;
			}
			return result;
		}

		private IEnumerator AnimateStripes()
		{
			_stripeCoroutineRunning = true;
			Color32 colorOfStripe = GlobalColorsGenerated.WEEKLYHUNT_CELEBRATION_STRIPE_COLOR;
			int minSpeed = 600;
			int maxSpeed = 2000;
			float minAlpha = 0.3f;
			float maxAlpha = 0.9f;
			float minScale = 0.5f;
			float maxScale = 1f;
			backgroundTexture.mainTexture = _weeklyHuntBackground;
			int numberOfStripes = 12;
			Vector2 minimumThreshold = new Vector2(-150f, -225f);
			Vector2 maximumThreshold = new Vector2(150f, 700f);
			if (_stripeTextures != null)
			{
				UITexture[] stripeTextures = _stripeTextures;
				foreach (UITexture uITexture in stripeTextures)
				{
					uITexture.gameObject.SetActive(false);
					UnityEngine.Object.Destroy(uITexture.gameObject);
				}
			}
			_stripeTextures = new UITexture[numberOfStripes];
			float[] stripeSpeeds = new float[numberOfStripes];
			for (int j = 0; j < numberOfStripes; j++)
			{
				stripeSpeeds[j] = KRandom.Range(minSpeed, maxSpeed);
				_stripeTextures[j] = stripePanel.AddWidget<UITexture>();
				_stripeTextures[j].mainTexture = _weeklyHuntStripe;
				_stripeTextures[j].shader = Shader.Find("Unlit/Transparent Colored");
				_stripeTextures[j].MakePixelPerfect();
				_stripeTextures[j].color = colorOfStripe;
				_stripeTextures[j].alpha = Mathf.Lerp(minAlpha, maxAlpha, stripeSpeeds[j] / (float)maxSpeed);
				Vector3 localScale = _stripeTextures[j].cachedTransform.localScale;
				_stripeTextures[j].cachedTransform.localScale = Vector3.Lerp(new Vector3(localScale.x * minScale, localScale.y * minScale, localScale.z), new Vector3(localScale.x * maxScale, localScale.y * maxScale, localScale.z), stripeSpeeds[j] / (float)maxSpeed);
				float x = KRandom.Range(minimumThreshold.x, maximumThreshold.x);
				float y = KRandom.Range(minimumThreshold.y, maximumThreshold.y);
				_stripeTextures[j].cachedTransform.localPosition = new Vector3(x, y, 0f);
			}
			yield return null;
			while (true)
			{
				for (int k = 0; k < _stripeTextures.Length; k++)
				{
					Vector3 localPosition = _stripeTextures[k].cachedTransform.localPosition;
					float num = stripeSpeeds[k] * Time.deltaTime;
					if (localPosition.y > maximumThreshold.y)
					{
						localPosition.x = KRandom.Range(minimumThreshold.x, maximumThreshold.x);
						stripeSpeeds[k] = KRandom.Range(minSpeed, maxSpeed);
						num = stripeSpeeds[k] * Time.deltaTime;
						_stripeTextures[k].alpha = Mathf.Lerp(minAlpha, maxAlpha, stripeSpeeds[k] / (float)maxSpeed);
						_stripeTextures[k].MakePixelPerfect();
						Vector3 localScale2 = _stripeTextures[k].cachedTransform.localScale;
						_stripeTextures[k].cachedTransform.localScale = Vector3.Lerp(new Vector3(localScale2.x * minScale, localScale2.y * minScale, localScale2.z), new Vector3(localScale2.x * maxScale, localScale2.y * maxScale, localScale2.z), stripeSpeeds[k] / (float)maxSpeed);
						localPosition.y = minimumThreshold.y - num;
					}
					_stripeTextures[k].cachedTransform.localPosition = new Vector3(localPosition.x, localPosition.y + num, localPosition.z);
				}
				yield return null;
			}
		}

		private void ResetBackgroundToNormal()
		{
			_stripeCoroutineRunning = false;
			CoroutineUtil.StopCoroutine(_stripeCoroutine);
			backgroundTexture.mainTexture = normalBackground;
			if (_stripeTextures != null)
			{
				UITexture[] stripeTextures = _stripeTextures;
				foreach (UITexture uITexture in stripeTextures)
				{
					uITexture.gameObject.SetActive(false);
					UnityEngine.Object.Destroy(uITexture.gameObject);
				}
			}
			_stripeTextures = null;
		}

		private void HidePrizeButtons()
		{
			TogglePrizeButtons(CelebrationRewardOrigin.Notset);
		}

		private void TogglePrizeButtons(CelebrationRewardOrigin origin)
		{
			GameObject gameObject = null;
			Layout layout = null;
			switch (origin)
			{
			case CelebrationRewardOrigin.MysteryBox:
				gameObject = _prizeButtonNormal;
				layout = _prizeButtonNormalLayout;
				break;
			case CelebrationRewardOrigin.MysteryBoxMini:
				gameObject = _prizeButtonMini;
				layout = _prizeButtonMiniLayout;
				break;
			case CelebrationRewardOrigin.SuperMysteryBox:
			case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
				gameObject = _prizeButtonSuper;
				layout = _prizeButtonSuperLayout;
				break;
			case CelebrationRewardOrigin.TokenMysteryBox:
				gameObject = _prizeButtonToken;
				layout = _prizeButtonTokenLayout;
				break;
			}
			_prizeButtonNormal.SetActive(_prizeButtonNormal == gameObject);
			_prizeButtonSuper.SetActive(_prizeButtonSuper == gameObject);
			_prizeButtonMini.SetActive(_prizeButtonMini == gameObject);
			_prizeButtonToken.SetActive(_prizeButtonToken == gameObject);
			if (gameObject != null)
			{
				float num = -46f;
				if (SafeArea.IsRestricted)
				{
					num += (float)(-SafeArea.OFFSET_TOP);
				}
				gameObject.transform.SetLocalPosY(num);
				layout.Align();
			}
		}

		private void ShowPrizeList(MysteryBox.Type type)
		{
			if (preOpenLabel != null)
			{
				preOpenLabel.SetActive(false);
			}
			switch (type)
			{
			case MysteryBox.Type.Normal:
				Flurry.LogEventWithAParameter("Mysterybox view prices", "Screen Name", "ShowMysteryBox");
				UIScreenController.Instance.PushPopup("PrizeMBPopup");
				break;
			case MysteryBox.Type.Super:
				Flurry.LogEventWithAParameter("Mysterybox view prices", "Screen Name", "ShowSuperMysteryBox");
				Flurry.LogEvent("Super Mystery Box prizes");
				UIScreenController.Instance.PushPopup("PrizeSMBPopup");
				break;
			case MysteryBox.Type.Mini:
				Flurry.LogEventWithAParameter("Mysterybox view prices", "Screen Name", "ShowMiniMysteryBox");
				Flurry.LogEvent("Mini Mystery Box prizes");
				UIScreenController.Instance.PushPopup("PrizeMMBPopup");
				break;
			case MysteryBox.Type.Token:
				UIScreenController.Instance.PushPopup("TokenMysteryBoxPrizesPopup");
				break;
			default:
				SDebug.LogError("Unhandeled mysterybox type " + type, this);
				return;
			}
			for (int i = 0; i < _numberOfRewards; i++)
			{
				if (_celebrationRewardContainer[i] != null)
				{
					NGUITools.SetActive(_celebrationRewardContainer[i], false);
				}
			}
			if (_superMysteryBoxEffect != null)
			{
				_superMysteryBoxEffect.StopEffect();
				_superMysteryBoxEffect.SetVisible(false);
			}
			openButtonCollider.enabled = false;
			skipButtonCollider.enabled = false;
			_tapToStartLabel.enabled = false;
		}

		private bool IsWaitingForUserInput()
		{
			if (_skipWaitBackButtonPressed)
			{
				_skipWaitBackButtonPressed = false;
				return false;
			}
			return _isWaitingForInput;
		}

		[UsedImplicitly]
		public void StopWaitingForInput()
		{
			_isWaitingForInput = false;
		}

		[UsedImplicitly]
		public void SkipNow()
		{
			if (_maySetTimeScale && !_isRewardingJackpot)
			{
				SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = 4f;
			}
		}

		private void OnPopClosed(string popupName)
		{
			switch (popupName)
			{
			case "PrizeMBPopup":
			case "PrizeMMBPopup":
			case "PrizeSMBPopup":
			case "TokenMysteryBoxPrizesPopup":
				ClosePrizeList();
				break;
			}
		}

		public void ClosePrizeList()
		{
			for (int i = 0; i < _numberOfRewards; i++)
			{
				if (_celebrationRewardContainer[i] != null)
				{
					NGUITools.SetActive(_celebrationRewardContainer[i], true);
				}
			}
			if (_superMysteryBoxEffect != null)
			{
				_superMysteryBoxEffect.SetVisible(true);
				ToggleSuperMysteryBoxEffect(_rewardsToUnlock[_currentReward].CelebrationRewardOrigin);
			}
			openButtonCollider.enabled = true;
			skipButtonCollider.enabled = true;
			_tapToStartLabel.enabled = true;
			if (preOpenLabel != null)
			{
				preOpenLabel.SetActive(true);
				CelebrationReward celebrationReward = _rewardsToUnlock[_currentReward];
				if (celebrationReward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterEventSuperMysteryBox)
				{
					CelebrationPopupLabelTemplate component = preOpenLabel.GetComponent<CelebrationPopupLabelTemplate>();
					component.SetupCharacterWordEvent(celebrationReward.characterEventCompleted, celebrationReward.description);
				}
			}
		}

		[UsedImplicitly]
		public void PrizeListNormalClicked()
		{
			ShowPrizeList(MysteryBox.Type.Normal);
		}

		[UsedImplicitly]
		public void PrizeListSuperClicked()
		{
			ShowPrizeList(MysteryBox.Type.Super);
		}

		[UsedImplicitly]
		public void PrizeListMiniClicked()
		{
			ShowPrizeList(MysteryBox.Type.Mini);
		}

		[UsedImplicitly]
		public void PrizeListTokenClicked()
		{
			Flurry.LogEventWithAParameter("Character token box view prices", "ScreenName", screenName);
			ShowPrizeList(MysteryBox.Type.Token);
		}

		private GameObject ShowModel(Characters.CharacterId charId, int themeIndex, UIModelController.ModelScreen modelScreen, bool cache)
		{
			return UIModelController.Instance.ShowCharacterInCelebration(charId, themeIndex, modelScreen, cache);
		}

		private void HideModel()
		{
			UIModelController.Instance.DeactivateCelebrationPopupModels();
		}

		private void ShowBackgroundWithStripes(Boards.BoardId boardId, UIModelController.ModelScreen modelScreen)
		{
			UIModelController.Instance.ActivateCelebrationBoardWithStripes(boardId, modelScreen);
		}

		private void HideSpecialBoard()
		{
			UIModelController.Instance.DeactivateCelebrationPopupModels();
		}

		private void HideCelebrationTunnel()
		{
			UIModelController.Instance.DeactivateCelebrationTunnel();
		}

		private bool IsJackpot(CelebrationReward reward)
		{
			if (reward.rewardType == CelebrationRewardType.coins)
			{
				switch (reward.CelebrationRewardOrigin)
				{
				case CelebrationRewardOrigin.MysteryBox:
				case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
					if (reward.amount >= 100000)
					{
						_isRewardingJackpot = true;
						return true;
					}
					break;
				case CelebrationRewardOrigin.SuperMysteryBox:
				case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
				case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
					if (reward.amount >= 300000)
					{
						_isRewardingJackpot = true;
						return true;
					}
					break;
				}
			}
			return false;
		}

		private IEnumerator Initial_Generic_Multiply_Routine(CelebrationReward reward, CelebrationPopupLabelTemplate template)
		{
			_weeklyHuntTopTitle.SetActive(false);
			ResetBackgroundToNormal();
			_flashEffectSprite.color = EFFECT_INITIAL_COLOR;
			yield return StartCoroutine(AnimateEffectColor(_flashEffectSprite, _flashEffectSprite.color, Color.white, 4f));
			continueButtonCollider.enabled = false;
			_tapToStartLabel.alpha = 0f;
			_tapToStartLabel.text = Strings.Get("CELEBRATION_POPUP_CONTINUE");
			template.SetupMultiply(reward);
			backgroundTexture.mainTexture = awardsBackground;
			int multipliedAmount = reward.amount * (int)(MysteryBox.GetMultiplierForReward(reward) - 1);
			if (multipliedAmount > 0)
			{
				int amount = reward.amount;
				reward.amount = multipliedAmount;
				PayoutReward(reward, false, true);
				reward.amount = amount;
			}
			if (OnRewardMultiplied__BackingField != null)
			{
				OnRewardMultiplied__BackingField(reward);
			}
			StartCoroutine(AnimateColor(GlowEffect.GetComponent<MeshRenderer>().material, 1.5f, Color.white));
			StartCoroutine(RotateGameObject(GlowEffect.transform, 3000f, new Vector3(0f, 0f, -270000f)));
			if (_superMysteryBoxEffect != null)
			{
				_superMysteryBoxEffect.StartEffect();
			}
			StartCoroutine(AnimateEffectColor(_flashEffectSprite, _flashEffectSprite.color, EFFECT_INITIAL_COLOR, 0.4f));
		}

		private void MultiplyCoins()
		{
			if (_rewardToMultiply != null && _rewardTemplate != null)
			{
				StartCoroutine(MultiplyCoins_Routine(_rewardToMultiply, _rewardTemplate));
			}
		}

		private IEnumerator MultiplyCoins_Routine(CelebrationReward reward, CelebrationPopupLabelTemplate template)
		{
			int coinboxCountFromAmount = InventorySave.coins;
			yield return StartCoroutine(Initial_Generic_Multiply_Routine(reward, template));
			int multipliedRewardAmount = reward.amount * (int)MysteryBox.GetMultiplierForReward(reward);
			int from = reward.amount;
			int to = multipliedRewardAmount;
			float factor = 0.5f;
			float countTime = CalcCoinCountTime(to, from, factor);
			bool megaJackpot = false;
			if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.MysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterEventSuperMysteryBox)
			{
				megaJackpot = reward.amount > 300000;
			}
			StartCoroutine(PlaySoundDelayed(_multiplyRewardClip, countTime, 0.7f));
			yield return StartCoroutine(CountUpCoins(coinboxCountFromAmount, to, template, megaJackpot, from, factor, true));
			yield return StartCoroutine(TweenScaleUpDown(template.doubleUpItemLabel.gameObject));
			StartCoroutine(AnimateAlpha(_tapToStartLabel, 0.5f, TAP_TO_START_ALPHA));
			continueButtonCollider.enabled = true;
		}

		private void MultiplyKeys()
		{
			if (_rewardToMultiply != null && _rewardTemplate != null)
			{
				StartCoroutine(MultiplyKeys_Routine(_rewardToMultiply, _rewardTemplate));
			}
		}

		private IEnumerator MultiplyKeys_Routine(CelebrationReward reward, CelebrationPopupLabelTemplate template)
		{
			yield return StartCoroutine(Initial_Generic_Multiply_Routine(reward, template));
			NGUITools.PlaySound(_multiplyRewardClip, 0.7f);
			yield return StartCoroutine(TweenScaleUpDown(template.doubleUpItemLabel.gameObject));
			StartCoroutine(AnimateAlpha(_tapToStartLabel, 0.5f, TAP_TO_START_ALPHA));
			continueButtonCollider.enabled = true;
		}

		private void MultiplyTokens()
		{
			if (_rewardToMultiply != null && _rewardTemplate != null)
			{
				StartCoroutine(MultiplyTokens_Routine(_rewardToMultiply, _rewardTemplate));
			}
		}

		private IEnumerator MultiplyTokens_Routine(CelebrationReward reward, CelebrationPopupLabelTemplate template)
		{
			yield return StartCoroutine(Initial_Generic_Multiply_Routine(reward, template));
			MissionsManager.Instance.PlayerDidThis(MissionTarget.Tokens, reward.amount);
			int tokenGoal = -1;
			int nextOutfit = Outfits.GetNextTokenUnlockableOutfitIndex(reward.characterId) + 1;
			tokenGoal = ((nextOutfit <= 0) ? Characters.characterData[reward.characterId].Price : Outfits.GetOutfitForCharacter(reward.characterId, nextOutfit).price);
			_tokenCounterLabel.text = string.Format("{0} / {1}", Mathf.Clamp(CharacterManager.GetCollectedTokens(reward.characterId), 0, tokenGoal), tokenGoal);
			yield return StartCoroutine(TweenScaleUpDown(template.doubleUpItemLabel.gameObject));
			NGUITools.PlaySound(_multiplyRewardClip, 0.7f);
			StartCoroutine(AnimateAlpha(_tapToStartLabel, 0.5f, TAP_TO_START_ALPHA));
			continueButtonCollider.enabled = true;
		}

		private void MultiplyPowerups()
		{
			if (_rewardToMultiply != null && _rewardTemplate != null)
			{
				StartCoroutine(MultiplyPowerups_Routine(_rewardToMultiply, _rewardTemplate));
			}
		}

		private IEnumerator MultiplyPowerups_Routine(CelebrationReward reward, CelebrationPopupLabelTemplate template)
		{
			yield return StartCoroutine(Initial_Generic_Multiply_Routine(reward, template));
			NGUITools.PlaySound(_multiplyRewardClip, 0.7f);
			yield return StartCoroutine(TweenScaleUpDown(template.doubleUpItemLabel.gameObject));
			StartCoroutine(AnimateAlpha(_tapToStartLabel, 0.5f, TAP_TO_START_ALPHA));
			continueButtonCollider.enabled = true;
		}

		private IEnumerator TweenScaleUpDown(GameObject go)
		{
			TweenScale tweenScale = go.GetComponent<TweenScale>();
			if (tweenScale == null)
			{
				tweenScale = go.AddComponent<TweenScale>();
				tweenScale.AddOnFinished(() =>
				{
					tweenScale.duration = _scaleUpDuration * _scaleDownDurationFactor;
					tweenScale.method = _scaleDownMethod;
					tweenScale.PlayReverse();
				});
			}
			tweenScale.from = go.transform.localScale;
			tweenScale.to = Vector3.Scale(base.transform.localScale, _scaleTargetFactor);
			tweenScale.duration = _scaleUpDuration;
			tweenScale.method = _scaleUpMethod;
			tweenScale.PlayForward();
			float totalDuration = _scaleUpDuration + _scaleUpDuration * _scaleDownDurationFactor;
			yield return new WaitForSeconds(totalDuration);
		}

		private IEnumerator PlaySoundDelayed(AudioClip clip, float delay = 0f, float volume = 1f)
		{
			yield return new WaitForSeconds(delay);
			NGUITools.PlaySound(clip, volume);
		}
	}
}
