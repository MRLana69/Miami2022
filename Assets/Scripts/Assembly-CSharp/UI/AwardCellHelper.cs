using System;
using System.Collections;
using Info;
using JetBrains.Annotations;
using Missions;
using UI.Generic.Layout;
using UI.Screens;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace UI
{
	public class AwardCellHelper : MonoBehaviour
	{
		private enum AwardCellState
		{
			NotSet = 0,
			SocialNetworksLocked = 1,
			PreviewInProgress = 2,
			InProgress = 3,
			Collectible = 4,
			PreviewFinished = 5,
			Finished = 6
		}

		private const string AWARD_IN_PROGRESS_DESCRIPTION_HEX_COLOR = "[2486BB]";

		private const string AWARD_IN_PROGRESS_VALUE_HEX_COLOR = "[004A7D]";

		private const string AWARD_COMPLETE_DESCRIPTION_HEX_COLOR = "[549133]";

		private const string AWARD_COMPLETE_VALUE_HEX_COLOR = "[015A01]";

		private const string END_COLOR_SYMBOL = "[-]";

		private static readonly Color32 COMPLETED_TITLE_TEXT_COLOR = new Color32(1, 90, 1, byte.MaxValue);

		private static readonly Color32 ACTIVE_TITLE_TEXT_COLOR = new Color32(0, 74, 125, byte.MaxValue);

		private static readonly Color32 ACTIVE_TITLE_TEXT_SHADOW_COLOR = new Color32(229, 234, 238, byte.MaxValue);

		private static readonly Color32 COMPLETED_TITLE_TEXT_SHADOW_COLOR = new Color32(232, 248, 228, byte.MaxValue);

		private static readonly Color32 COLLECTIBLE_SEGMENT_BACKGROUND_COLOR = new Color32(142, 206, 242, 180);

		private static readonly Color32 PROGRESS_SEGMENT_BACKGROUND_COLOR = new Color32(170, 190, 204, 180);

		private static readonly Color32 COMPLETE_SEGMENT_BACKGROUND_COLOR = new Color32(146, 231, 127, 180);

		private static readonly Color32 REWARD_BIG_BACKGROUND_FINISHED_COLOR = new Color32(90, 124, 90, byte.MaxValue);

		private static readonly Color32 REWARD_BIG_BACKGROUND_IN_PROGRESS_COLOR = new Color32(99, 116, 128, byte.MaxValue);

		private static readonly Color32 REWARD_BIG_BACKGROUND_COLLECTIBLE_COLOR = new Color32(91, 116, 134, byte.MaxValue);

		private static readonly Color32 EFFECT_INITIAL_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);

		private static readonly Color32 MYSTERYBOX_BACKGROUND_PROGRESS_COLOR = new Color32(36, 134, 187, byte.MaxValue);

		private static readonly Color32 MYSTERYBOX_BACKGROUND_FINISHED_COLOR = new Color32(84, 145, 51, byte.MaxValue);

		[SerializeField]
		private AudioClip _awardTierUnlockSound;

		[SerializeField]
		private AwardCollectibleHelper _mainGoalElementsHelper;

		[SerializeField]
		private GameObject _expandedParent;

		[SerializeField]
		private UILabel _missionTitle;

		[SerializeField]
		private UILabel _missionTextDescription;

		[SerializeField]
		private GameObject _missionIconsDescriptionParent;

		[SerializeField]
		private UISprite _cellBackground;

		[SerializeField]
		private GameObject _collectButtonParent;

		[SerializeField]
		private UILabel _collectButtonLabel;

		[SerializeField]
		private UISprite _progressIconsBG;

		[SerializeField]
		private GameObject _progressBarParent;

		[SerializeField]
		private UISlider _progressBarSlider;

		[SerializeField]
		private GameObject _progressBarFG;

		[SerializeField]
		private GameObject _tieredParent;

		[SerializeField]
		private AwardTierHelper _notTieredParent;

		[SerializeField]
		private UISprite _rewardBigBG;

		[SerializeField]
		private UILabel _previewCompleteLabel;

		[SerializeField]
		private GameObject _loginFBButton;

		[SerializeField]
		private GameObject _loginButtonContents;

		[SerializeField]
		private GameObject _connectingButtonContents;

		[SerializeField]
		private Layout _connectingContentsLayout;

		[SerializeField]
		private Layout _loginContentLayout;

		[SerializeField]
		private UISprite _loginFBButtonFill;

		[SerializeField]
		private UILabel _loginFBLabel;

		[SerializeField]
		private UILabel _connectingLabel;

		[SerializeField]
		private AwardTierHelper[] _tierIconHelpers = new AwardTierHelper[4];

		[SerializeField]
		private UISprite _tierEffectSprite;

		[SerializeField]
		private UISprite _noTierEffectSprite;

		[SerializeField]
		private AwardCollectibleHelper[] _goalIcons = new AwardCollectibleHelper[4];

		[SerializeField]
		private UISprite _mysteryBoxIcon;

		[SerializeField]
		private UISprite _mysteryBoxArrowBackground;

		[SerializeField]
		private UILabel _progressAmount;

		[SerializeField]
		private GameObject _rewardContentParent;

		[SerializeField]
		private UILabel _rewardAmount;

		[SerializeField]
		private HorizontalLayout _rewardContentLayout;

		[SerializeField]
		private GameObject _stickerParent;

		[SerializeField]
		private UISprite _stickerIconSprite;

		[SerializeField]
		private UISprite _stickerIconNew;

		[SerializeField]
		private UISprite _stickerSprite;

		[SerializeField]
		private Animation wiggleAnimation;

		[SerializeField]
		private BoxCollider _loginCollider;

		[SerializeField]
		private UISprite _socialLock;

		private string[] _mainTierGoalSpriteNames = new string[4] { "icon_bronze_can", "icon_silver_can", "icon_gold_can", "icon_diamond_can" };

		private string[] _mysteryBoxIconNames = new string[2] { "icon_small_mysteryBox", "icon_small_superMysteryBox" };

		private const string DIAMOND_TIER_ITEM_BACKGROUND_SPRITE_NAME = "icon_diamond_award_bg";

		private const string COLLECTIBLE_DIAMOND_TIER_ITEM_BACKGROUND_SPRITE_NAME = "icon_diamond_award_collect_bg";

		private const string COMPLETED_GOAL_ICON_BACKGROUND_SPRITE_NAME = "icon_award_bg_completed";

		private const string COLLECTIBLE_GOAL_ICON_BACKGROUND_SPRITE_NAME = "icon_award_collect_bg";

		private const string IN_PROGRESS_GOAL_ICON_BACKGROUND_SPRITE_NAME = "icon_award_bg";

		private float soundVolume = 0.1f;

		private float soundPitch = 1f;

		private AwardCellState _currentCellState;

		private AwardsScreen _awardsScreen;

		private bool _hasManyTiers;

		private Award _currentAward;

		private bool _enablePreview;

		private float _animationLerpFactor;

		private bool _isConnectedToFacebook;

		private void OnDisable()
		{
			if (_currentAward != null)
			{
				_currentAward.isNew = false;
			}
		}

		public void InitCell(Award award, AwardsScreen awardsScreen)
		{
			if (award != null)
			{
				_currentAward = award;
				_enablePreview = award.isInPreview;
				_isConnectedToFacebook = SocialManager.instance.facebookIsLoggedIn;
				_currentCellState = GetCellState();
				_hasManyTiers = award.awardsData.missionTiers.Count > 1;
				_awardsScreen = awardsScreen;
			}
			SetupCellStructure();
			SetupCellContent();
			SetupCellColors();
			if (_enablePreview)
			{
				if (_hasManyTiers)
				{
					if (award.tierAnimationPlayed != (AwardTierType)(GetNbToAnimate() + 1))
					{
						StartCoroutine(AnimateChangingOfTierSprite(_tierEffectSprite, _tierIconHelpers[GetNbToAnimate()]));
						award.tierAnimationPlayed = (AwardTierType)(GetNbToAnimate() + 1);
					}
					else
					{
						_tierEffectSprite.gameObject.SetActive(false);
					}
				}
				else
				{
					StartCoroutine(AnimateChangingOfTierSprite(_noTierEffectSprite, _notTieredParent));
				}
			}
			if (_currentCellState == AwardCellState.Collectible)
			{
				wiggleAnimation.wrapMode = WrapMode.Loop;
				wiggleAnimation.Play();
			}
			else
			{
				wiggleAnimation.Stop();
			}
		}

		private int GetNbToAnimate()
		{
			int result = 0;
			switch (_currentAward.activeTierType)
			{
			case AwardTierType.Silver:
				result = 0;
				break;
			case AwardTierType.Gold:
				result = 1;
				break;
			case AwardTierType.Diamond:
				result = ((_currentAward.awardState != AwardState.Collectible && _currentAward.awardState != AwardState.InProgress) ? 3 : 2);
				break;
			}
			return result;
		}

		private AwardCellState GetCellState()
		{
			AwardCellState awardCellState = AwardCellState.NotSet;
			bool flag = _currentAward.awardsData.useFacebook && !_isConnectedToFacebook;
			switch (_currentAward.awardState)
			{
			case AwardState.InProgress:
				awardCellState = ((!_enablePreview) ? AwardCellState.InProgress : AwardCellState.PreviewInProgress);
				break;
			case AwardState.Collectible:
				awardCellState = AwardCellState.Collectible;
				break;
			case AwardState.Finished:
				awardCellState = ((!_enablePreview) ? AwardCellState.Finished : AwardCellState.PreviewFinished);
				break;
			default:
				awardCellState = AwardCellState.NotSet;
				break;
			}
			if (flag && (awardCellState == AwardCellState.InProgress || awardCellState == AwardCellState.PreviewInProgress))
			{
				awardCellState = AwardCellState.SocialNetworksLocked;
			}
			return awardCellState;
		}

		private void SetupCellStructure()
		{
			_expandedParent.SetActive(_currentCellState != AwardCellState.Finished);
			_missionTextDescription.gameObject.SetActive(_currentAward.awardsData.hasDescription);
			_missionIconsDescriptionParent.SetActive(!_currentAward.awardsData.hasDescription);
			_socialLock.gameObject.SetActive(_currentCellState == AwardCellState.SocialNetworksLocked);
			if (_expandedParent.activeSelf)
			{
				_tierEffectSprite.gameObject.SetActive(_enablePreview);
				_collectButtonParent.SetActive(_currentCellState == AwardCellState.Collectible);
				_progressBarParent.SetActive(_currentCellState == AwardCellState.InProgress || _currentCellState == AwardCellState.PreviewInProgress);
				_previewCompleteLabel.gameObject.SetActive(_currentCellState == AwardCellState.PreviewFinished);
				_notTieredParent.gameObject.SetActive(!_hasManyTiers);
				_tieredParent.SetActive(_hasManyTiers);
				_stickerParent.SetActive(_currentAward.isNew || _currentCellState == AwardCellState.Collectible);
				_stickerIconNew.gameObject.SetActive(_currentAward.isNew && _currentCellState != AwardCellState.Collectible);
				_stickerIconSprite.gameObject.SetActive(_currentCellState == AwardCellState.Collectible);
				_rewardContentParent.SetActive(_currentCellState != AwardCellState.PreviewFinished && _currentCellState != AwardCellState.SocialNetworksLocked);
				_loginFBButton.gameObject.SetActive(_currentCellState == AwardCellState.SocialNetworksLocked);
				if (_currentCellState == AwardCellState.SocialNetworksLocked)
				{
					SetFBLoginButtonToNormalState();
				}
			}
		}

		private void SetupCellContent()
		{
			AwardGoal[] goals = _currentAward.goals;
			int progressAsPercentage = _currentAward.GetProgressAsPercentage();
			int num = (int)(_currentAward.activeTierType - 1);
			_missionTitle.text = Strings.Get(_currentAward.awardsData.title);
			bool flag = _currentCellState == AwardCellState.Finished || _currentCellState == AwardCellState.PreviewFinished;
			bool useGlitter = flag || _currentCellState == AwardCellState.Collectible || _currentAward.activeTierType == AwardTierType.Diamond;
			_mainGoalElementsHelper.AddSocialLockCanOverlay(_currentCellState == AwardCellState.SocialNetworksLocked);
			bool animateStars = _currentAward.activeTierType == AwardTierType.Diamond && _currentCellState != AwardCellState.Finished && _currentCellState != AwardCellState.PreviewFinished;
			_mainGoalElementsHelper.SetupAwardCollectible(FindBackgroundToUse(true), _mainTierGoalSpriteNames[num], useGlitter, flag, animateStars);
			_mainGoalElementsHelper.ActivateCanGlow(_currentAward.activeTierType == AwardTierType.Diamond && _currentCellState == AwardCellState.Collectible);
			if (_currentCellState == AwardCellState.PreviewFinished)
			{
				_previewCompleteLabel.text = Strings.Get("AWARD_PREVIEW_COMPLETED_TEXT");
			}
			if (_currentAward.awardsData.hasDescription)
			{
				string key = ((!_currentAward.awardsData.hasAnotherDescriptionForOneItemGoal || goals[0].goal != 1) ? _currentAward.awardsData.description : _currentAward.awardsData.descriptionForOneItemGoal);
				if (_currentCellState != AwardCellState.PreviewFinished && _currentCellState != AwardCellState.Finished)
				{
					_missionTextDescription.text = string.Format(Strings.Get(key), "[2486BB]", "[-]", "[004A7D]", goals[0].goal, "[-]", "[2486BB]", "[-]");
				}
				else
				{
					_missionTextDescription.text = string.Format(Strings.Get(key), "[549133]", "[-]", "[015A01]", goals[0].goal, "[-]", "[549133]", "[-]");
				}
			}
			else
			{
				for (int i = 0; i < goals.Length; i++)
				{
					AwardGoal awardGoal = goals[i];
					if (_currentCellState == AwardCellState.Collectible)
					{
						_goalIcons[i].SetupAwardCollectible(FindBackgroundToUse(false), awardGoal.iconName, false, true, animateStars);
					}
					else
					{
						_goalIcons[i].SetupAwardCollectible(FindBackgroundToUse(false), awardGoal.iconName, false, _currentAward.IsGoalAccomplished(awardGoal), animateStars);
					}
				}
				if (_currentAward.awardsData.id == AwardId.COLLECT_OLD_TROPHY_ITEMS_MB)
				{
					_mysteryBoxIcon.spriteName = _mysteryBoxIconNames[0];
				}
				else if (_currentAward.awardsData.id == AwardId.COLLECT_OLD_TROPHY_ITEMS_SMB)
				{
					_mysteryBoxIcon.spriteName = _mysteryBoxIconNames[1];
				}
				if (_currentCellState == AwardCellState.PreviewFinished)
				{
					_mysteryBoxArrowBackground.color = MYSTERYBOX_BACKGROUND_FINISHED_COLOR;
				}
				else
				{
					_mysteryBoxArrowBackground.color = MYSTERYBOX_BACKGROUND_PROGRESS_COLOR;
				}
			}
			if (!_expandedParent.activeSelf)
			{
				return;
			}
			_rewardAmount.text = _currentAward.reward.ToString();
			if (_progressBarParent.activeSelf)
			{
				float num2 = (float)progressAsPercentage / 100f;
				if (num2 > 0f)
				{
					_progressBarFG.SetActive(true);
					_progressBarSlider.value = num2;
				}
				else
				{
					_progressBarFG.SetActive(false);
				}
				_progressAmount.text = string.Format(Strings.Get("AWARD_PROGRESS_AMOUNT_FORMAT"), progressAsPercentage);
			}
			if (_collectButtonParent.activeSelf)
			{
				_collectButtonLabel.text = Strings.Get("AWARDS_COLLECT_BUTTON_TEXT");
			}
			if (_loginFBButton.gameObject.activeSelf)
			{
				_loginFBLabel.text = Strings.Get("AWARD_SOCIAL_NETWORK_LOCKED_TEXT");
				_connectingLabel.text = Strings.Get("SOCIAL_BUTTON_FEEDBACK_CONNECTING");
			}
			if (_rewardContentLayout != null && _rewardContentLayout.gameObject.activeInHierarchy)
			{
				_rewardContentLayout.Align();
			}
			for (int j = 0; j < _tierIconHelpers.Length; j++)
			{
				if (j == GetTierNumberToEnable())
				{
					_tierIconHelpers[j].TierNextComplete();
				}
				else
				{
					_tierIconHelpers[j].TierComplete(j < GetTierNumberToEnable());
				}
			}
		}

		private void SetupCellColors()
		{
			_missionTextDescription.color = Color.white;
			switch (_currentCellState)
			{
			case AwardCellState.PreviewInProgress:
			case AwardCellState.Collectible:
				_cellBackground.color = COLLECTIBLE_SEGMENT_BACKGROUND_COLOR;
				_missionTitle.color = ACTIVE_TITLE_TEXT_COLOR;
				_missionTitle.effectColor = ACTIVE_TITLE_TEXT_SHADOW_COLOR;
				break;
			case AwardCellState.SocialNetworksLocked:
			case AwardCellState.InProgress:
				_cellBackground.color = PROGRESS_SEGMENT_BACKGROUND_COLOR;
				_missionTitle.color = ACTIVE_TITLE_TEXT_COLOR;
				_missionTitle.effectColor = ACTIVE_TITLE_TEXT_SHADOW_COLOR;
				break;
			case AwardCellState.Finished:
				_cellBackground.color = COMPLETE_SEGMENT_BACKGROUND_COLOR;
				_missionTitle.color = COMPLETED_TITLE_TEXT_COLOR;
				_missionTitle.effectColor = COMPLETED_TITLE_TEXT_SHADOW_COLOR;
				break;
			case AwardCellState.PreviewFinished:
				_cellBackground.color = COMPLETE_SEGMENT_BACKGROUND_COLOR;
				_missionTitle.color = COMPLETED_TITLE_TEXT_COLOR;
				_missionTitle.effectColor = COMPLETED_TITLE_TEXT_SHADOW_COLOR;
				break;
			}
			if (_currentCellState == AwardCellState.Collectible)
			{
				_stickerSprite.color = GlobalColors.AWARDS_COLLECTIBLE_STICKER_COLOR;
			}
			else if (_currentAward.isNew)
			{
				_stickerSprite.color = GlobalColors.AWARDS_NEW_STICKER_COLOR;
			}
			if (_currentCellState == AwardCellState.PreviewFinished)
			{
				_rewardBigBG.color = REWARD_BIG_BACKGROUND_FINISHED_COLOR;
				_progressIconsBG.color = REWARD_BIG_BACKGROUND_FINISHED_COLOR;
			}
			else if (_currentCellState == AwardCellState.Collectible)
			{
				_rewardBigBG.color = REWARD_BIG_BACKGROUND_COLLECTIBLE_COLOR;
				_progressIconsBG.color = REWARD_BIG_BACKGROUND_COLLECTIBLE_COLOR;
			}
			else
			{
				_rewardBigBG.color = REWARD_BIG_BACKGROUND_IN_PROGRESS_COLOR;
				_progressIconsBG.color = REWARD_BIG_BACKGROUND_IN_PROGRESS_COLOR;
			}
		}

		private string FindBackgroundToUse(bool mainGoalIcon)
		{
			switch (_currentCellState)
			{
			case AwardCellState.Collectible:
				return (!mainGoalIcon) ? "icon_award_collect_bg" : ((_currentAward.activeTierType != AwardTierType.Diamond) ? "icon_award_collect_bg" : "icon_diamond_award_collect_bg");
			case AwardCellState.PreviewFinished:
			case AwardCellState.Finished:
				return (!mainGoalIcon) ? "icon_award_bg_completed" : "icon_award_bg_completed";
			default:
				return (!mainGoalIcon) ? "icon_award_bg" : ((_currentAward.activeTierType != AwardTierType.Diamond) ? "icon_award_bg" : "icon_diamond_award_bg");
			}
		}

		private int GetTierNumberToEnable()
		{
			int num = -1;
			if (_currentAward.HasCompletedTierExact(AwardTierType.Diamond) && _currentAward.awardState == AwardState.Finished)
			{
				return (int)_currentAward.activeTierType;
			}
			return (int)(_currentAward.activeTierType - 1);
		}

		[UsedImplicitly]
		public void CollectRewardClicked()
		{
			Flurry.LogEventWithSeveralParameters("Award tier collected time", "Award;Tier;Time", _currentAward.awardsData.id.ToString() + ";" + (int)_currentAward.activeTierType + ";" + DateTime.Now);
			CelebrationReward celebrationReward = new CelebrationReward();
			celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.Award;
			celebrationReward.rewardType = CelebrationRewardType.keys;
			celebrationReward.amount = _currentAward.reward;
			celebrationReward.awardTierType = _currentAward.activeTierType;
			celebrationReward.awardTitle = Strings.Get(_currentAward.awardsData.title);
			celebrationReward.syboAnalyticsSource = ObtainableItemSource.Award;
			celebrationReward.syboAnalyticsSourceId = -1L;
			celebrationReward.syboAnalyticsSourceMetaOrNull = string.Format("{0}_{1}", celebrationReward.awardTierType.ToString(), _currentAward.awardsData.id.ToString()).ToLowerInvariant();
			_currentAward.isInPreview = true;
			_currentAward.RewardPayedOut();
			RewardManager.AddRewardToUnlock(celebrationReward);
			UIScreenController.Instance.QueuePopup("CelebrationPopup");
		}

		[UsedImplicitly]
		public void FacebookLoginClicked()
		{
			_awardsScreen.FacebookLoginClicked();
			SetFBLoginButtonToConnectingState();
		}

		private IEnumerator AnimateChangingOfTierSprite(UISprite effectSprite, AwardTierHelper tierIconToChange)
		{
			effectSprite.transform.localPosition = tierIconToChange.transform.localPosition;
			effectSprite.color = EFFECT_INITIAL_COLOR;
			NGUITools.PlaySound(_awardTierUnlockSound, soundVolume, soundPitch);
			yield return StartCoroutine(AnimateEffectColor(effectSprite, effectSprite.color, Color.white, 3f));
			tierIconToChange.TierComplete(true);
			yield return new WaitForSeconds(0.5f);
			yield return StartCoroutine(AnimateEffectColor(effectSprite, effectSprite.color, EFFECT_INITIAL_COLOR, 0.8f));
		}

		private IEnumerator AnimateEffectColor(UISprite spriteToAnimate, Color color1, Color color2, float speedFactor = 1f)
		{
			_animationLerpFactor = 0f;
			while (_animationLerpFactor < 1f)
			{
				_animationLerpFactor += Time.deltaTime * speedFactor;
				spriteToAnimate.color = Color.Lerp(color1, color2, _animationLerpFactor);
				yield return null;
			}
		}

		private void SetFBLoginButtonToNormalState()
		{
			_loginButtonContents.SetActive(true);
			_connectingButtonContents.SetActive(false);
			_loginFBButtonFill.color = GlobalColors.PRIMARY_ACTION_COLOR;
			_loginCollider.enabled = true;
			_loginContentLayout.Align();
		}

		private void SetFBLoginButtonToConnectingState()
		{
			_loginButtonContents.SetActive(false);
			_connectingButtonContents.SetActive(true);
			_loginFBButtonFill.color = GlobalColors.SOCIAL_GREY;
			_loginCollider.enabled = false;
			_connectingContentsLayout.Align();
		}
	}
}
