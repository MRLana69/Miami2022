using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Data.Save;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using SYBO.Subway;
using SYBO.Subway.Info;
using SYBO.Subway.Promotion;
using UI.ModelCamera;
using UI.Screens;
using UnityEngine;
using Utilities;
using Debug = UnityEngine.Debug;

public class UIModelController : MonoBehaviour
{
	public enum ModelScreen
	{
		Character = 0,
		GameOver = 1,
		TutorialPopup = 2,
		Popup = 3,
		Boards = 4,
		CelebrationCharacterUnlock = 5,
		CelebrationBoardUnlock = 6,
		CelebrationHighScore = 7,
		DuelResultPopup = 8,
		CelebrationTopRun = 9,
		LimitedCharacterPopup = 10,
		LimitedBoardPopup = 11,
		BoardPromotionPopup = 12,
		FirstPurchaseBonusBigCharacter = 13,
		CloudSaveReminderPopup = 14,
		CharacterPromotionPopup = 15,
		ShopScreenPromotion = 16,
		BirthdayBoardPopup = 17
	}

	public GameObject CharacterAnchor;

	public GameObject GameOverAnchor;

	public GameObject CelebrationPopupAnchor;

	public GameObject TutorialPopupAnchor;

	public GameObject HuntPrizeAnchor;

	public GameObject PauseScreenAnchor;

	public GameObject WeeklyHuntRewardAnchor;

	public GameObject DailyChallengeRewardAnchor;

	public GameObject FirstPurchaseBonusAnchor;

	public GameObject CharacterPromotionAnchor;

	[SerializeField]
	private GameObject _maskedCharacterAnchor;

	public GameObject ModelPrefab;

	public CelebrationBackgroundHelper celebrationBackgroundHelper;

	[SerializeField]
	private AnimationClip _firstPurchaseBonusIdleAnimation;

	[SerializeField]
	private AnimationClip _firstPurchaseBonusBreakDanceAnimation;

	[SerializeField]
	private AnimationClip _firstPurchaseBonusBreakDanceAnimation2;

	private Dictionary<ModelScreen, ModelScreenData> _cachedModelScreenDataEntries;

	private const string CELEBRATION_STRIPE_COROUTINE_NAME = "AnimateUnlockBackground";

	private const string BRAG_CELEBRATION_ANIMATION_COROUTINE = "AnimateRunningCharacterForBragCelebration";

	private bool celebrationShouldAnimateStripes = true;

	private bool _isCelebrationCharacterScreen;

	private bool _isCelebrationBackgroundRotating;

	private Vector3 _discoBgRotation = new Vector3(35f, 180f, 0f);

	private Vector3 _tunnerBgRotationNormal = new Vector3(0f, 0f, 180f);

	private ModelScreen _currentActivatedScreenModel;

	private Boards.BoardId _currentMysteryBoxBoardId;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnChangedCurrentlyShownCharacter__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnChangedCurrentlyShownHoverboard__BackingField;

	private int _currentlyShownThemeIndex;

	private Characters.CharacterId _currentlyShownModel;

	private Vector3 CHAR_TO_POS = new Vector3(-60f, -150f, -100f);

	private Vector3 CHAR_FROM_POS = new Vector3(382f, -125f, 750f);

	private const float RUNNING_SPEED_HIGH = 3f;

	private const float RUNNING_SPEED_LOW = 1f;

	private const float DURATION = 3.5f;

	private const float PARTICLE_ANIMATION_SPEED_FACTOR_LOW = 0.5f;

	private const float PARTICLE_ANIMATION_SPEED_FACTOR_HIGH = 3.5f;

	private static UIModelController _instance;

	public GameObject limitedCharacterPopupAnchor { get; set; }

	public GameObject limitedBoardPopupAnchor { get; set; }

	public GameObject BoardPromotionPopupAnchor { get; set; }

	public GameObject birthdayBoardPopupAnchor { get; set; }

	public int currentlyShownThemeIndex
	{
		get
		{
			return _currentlyShownThemeIndex;
		}
	}

	public Characters.CharacterId currentlyShownModel
	{
		get
		{
			return _currentlyShownModel;
		}
	}

	public static UIModelController Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = UnityEngine.Object.FindObjectOfType<UIModelController>();
				_instance.Init();
			}
			return _instance;
		}
	}

	public static event Action OnChangedCurrentlyShownCharacter
	{
		add
		{
			Action action = OnChangedCurrentlyShownCharacter__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnChangedCurrentlyShownCharacter__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnChangedCurrentlyShownCharacter__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnChangedCurrentlyShownCharacter__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnChangedCurrentlyShownHoverboard
	{
		add
		{
			Action action = OnChangedCurrentlyShownHoverboard__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnChangedCurrentlyShownHoverboard__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnChangedCurrentlyShownHoverboard__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnChangedCurrentlyShownHoverboard__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public void ActivateGameOverModel()
	{
		_SetupModelAndActivateIt(PlayerSave.currentCharacterId, PlayerSave.currentOutfitIndex, ModelScreen.GameOver);
	}

	public GameObject ShowCharacterInCelebration(Characters.CharacterId charId, int themeIndex, ModelScreen screen, bool cache)
	{
		return _SetupModelAndActivateIt(charId, themeIndex, screen, cache);
	}

	public void DeactivateCelebrationPopupModels()
	{
		StopCoroutine("AnimateUnlockBackground");
		StopCoroutine("AnimateRunningCharacterForBragCelebration");
		DeactivateModelScreen(ModelScreen.CelebrationBoardUnlock);
		celebrationBackgroundHelper.Hide();
	}

	public void ActivateCelebrationBoardWithStripes(Boards.BoardId boardId, ModelScreen screen)
	{
		_currentMysteryBoxBoardId = boardId;
		if (screen == ModelScreen.CelebrationHighScore)
		{
			_SetupModelAndActivateIt(PlayerSave.currentCharacterId, PlayerSave.currentOutfitIndex, screen);
		}
		else
		{
			_SetupModelAndActivateIt(GetActiveCharacterId(ModelScreen.Boards), PlayerSave.currentOutfitIndex, screen);
		}
	}

	public void SetDiscoLightBackground()
	{
		celebrationBackgroundHelper.Show(Quaternion.Euler(_discoBgRotation), false, true);
	}

	public void ActivateCelebrationTunnel()
	{
		celebrationBackgroundHelper.Show(Quaternion.Euler(_tunnerBgRotationNormal), false, false);
	}

	public void DeactivateCelebrationTunnel()
	{
		celebrationBackgroundHelper.Hide();
	}

	public void SelectThemeForCurrentModel(int themeVersion)
	{
		_currentlyShownThemeIndex = themeVersion;
		SelectCurrentModelForPlay();
	}

	public void SelectCurrentModelForPlay()
	{
		SelectCharacterForPlay(currentlyShownModel, currentlyShownThemeIndex);
	}

	public void SelectCharacterForPlay(Characters.CharacterId characterId, int themeIndex)
	{
		PlayerSave.currentCharacterId = characterId;
		PlayerSave.currentOutfitIndex = themeIndex;
		if (ShouldCreateABrandNewModel())
		{
			Game.Instance.RecreateCharacterModel();
		}
		else if (Game.Instance != null)
		{
			string modelName = Characters.characterData[PlayerSave.currentCharacterId].modelName;
			Game.Instance.Character.CharacterModel.ChangeCharacterModel(modelName, PlayerSave.currentOutfitIndex, false, true);
		}
		OnChangedCurrentlyShownCharacter__BackingField.SafeInvoke();
	}

	public bool ExistsTransformModelInModelScreen(ModelScreen screen, Transform transformModel)
	{
		foreach (KeyValuePair<ModelScreen, ModelScreenData> cachedModelScreenDataEntry in _cachedModelScreenDataEntries)
		{
			ModelScreenData value = cachedModelScreenDataEntry.Value;
			if (value.modelTranform == transformModel && cachedModelScreenDataEntry.Key == screen)
			{
				return true;
			}
		}
		return false;
	}

	public void ActivateHoverboardModel(ModelScreen screen = ModelScreen.Boards)
	{
		_currentlyShownModel = PlayerSave.currentCharacterId;
		_currentlyShownThemeIndex = PlayerSave.currentOutfitIndex;
		_SetupModelAndActivateIt(_currentlyShownModel, _currentlyShownThemeIndex, screen);
		OnChangedCurrentlyShownHoverboard__BackingField.SafeInvoke();
	}

	public void ShowHoverboardMenuModel(Boards.BoardId currentlyShownBoard, ModelScreen screen, bool willUpdateAnimation, bool willAllowSignatureAnimation, Hoverboard.BoardAbility boardAbility)
	{
		CharacterModel characterModel = _cachedModelScreenDataEntries[screen].characterModel;
		GameObject hoverboardGameObject = characterModel.GetHoverboardGameObject();
		HoverboardModelPreviewFactory.Instance.ChangeHoverboard(currentlyShownBoard, hoverboardGameObject, characterModel.CharacterAnimation, willUpdateAnimation, willAllowSignatureAnimation, boardAbility);
		OnChangedCurrentlyShownHoverboard__BackingField.SafeInvoke();
	}

	public void SelectCurrentlyShownBoard(Boards.BoardId boardId)
	{
		PlayerSave.currentBoardId = boardId;
		OnChangedCurrentlyShownHoverboard__BackingField.SafeInvoke();
	}

	public CharacterModel GetActiveCharacterModel(ModelScreen screen)
	{
		return _cachedModelScreenDataEntries[screen].characterModel;
	}

	public Characters.CharacterId GetActiveCharacterId(ModelScreen screen)
	{
		Characters.CharacterId result = PlayerSave.currentCharacterId;
		if (_cachedModelScreenDataEntries.ContainsKey(screen))
		{
			result = _cachedModelScreenDataEntries[screen].characterId;
		}
		return result;
	}

	public void ShowMenuModel(Characters.CharacterId charId, int themeVersion, ModelScreen screen = ModelScreen.Character)
	{
		if (!IsModelCurrentlyShown(charId, themeVersion, screen))
		{
			_currentlyShownModel = charId;
			_currentlyShownThemeIndex = themeVersion;
			_SetupModelAndActivateIt(charId, themeVersion, screen);
		}
	}

	private bool IsModelCurrentlyShown(Characters.CharacterId charId, int themeVersion, ModelScreen screen)
	{
		foreach (KeyValuePair<ModelScreen, ModelScreenData> cachedModelScreenDataEntry in _cachedModelScreenDataEntries)
		{
			if (cachedModelScreenDataEntry.Key == screen)
			{
				ModelScreenData value = cachedModelScreenDataEntry.Value;
				if (value.characterId == charId && value.themeVersion == themeVersion && _cachedModelScreenDataEntries[screen].isModelActive)
				{
					return true;
				}
			}
		}
		return false;
	}

	private GameObject _SetupModelAndActivateIt(Characters.CharacterId charId, int themeVersion, ModelScreen screen, bool cache = true)
	{
		if (ShouldCreateABrandNewModel())
		{
			ClearScreenAnchor(ModelScreen.Character);
			ClearScreenAnchor(ModelScreen.Boards);
			ClearScreenAnchor(ModelScreen.FirstPurchaseBonusBigCharacter);
			if (screen != ModelScreen.Character && screen != ModelScreen.Boards && screen != ModelScreen.FirstPurchaseBonusBigCharacter)
			{
				ClearScreenAnchor(screen);
			}
		}
		ModelScreenData modelScreenData = null;
		ModelScreenSetup screenSetup = ModelScreenSetups.GetScreenSetup(screen);
		if (_cachedModelScreenDataEntries.ContainsKey(screen) && _cachedModelScreenDataEntries[screen] != null)
		{
			modelScreenData = _cachedModelScreenDataEntries[screen];
			modelScreenData.UpdateCharacterDataIfNeeded(charId, themeVersion);
		}
		else
		{
			modelScreenData = new ModelScreenData(UnityEngine.Object.Instantiate(ModelPrefab), UIScreenController.Instance.Camera3D, charId, themeVersion);
		}
		SetupModelScreen(screen, screenSetup, modelScreenData);
		return _ActivateModel(modelScreenData, charId, themeVersion, screen, cache);
	}

	private void SetupModelScreen(ModelScreen screen, ModelScreenSetup screenSetup, ModelScreenData modelScreenData)
	{
		GameObject screenAnchor = GetScreenAnchor(screen);
		Transform modelTranform = modelScreenData.modelTranform;
		modelTranform.parent = screenAnchor.transform;
		screenSetup.SetupScreen(modelTranform, modelScreenData.cameraTransform, modelScreenData.cameraLayer);
	}

	private GameObject _ActivateModel(ModelScreenData modelScreenData, Characters.CharacterId characterId, int modelVersion, ModelScreen screen, bool cache, Characters.CharacterId? friendsCharacterName = null, int friendsModelVersion = -1)
	{
		ActivateModelScreen(screen);
		string modelName = Characters.characterData[characterId].modelName;
		GameObject modelGo = modelScreenData.modelGo;
		CharacterModel characterModel = modelScreenData.characterModel;
		_currentActivatedScreenModel = screen;
		switch (screen)
		{
		case ModelScreen.Character:
			characterModel.ChangeCharacterModel(modelName, modelVersion, true, true);
			characterModel.HideAllPowerups();
			characterModel.StartIdleAnimations(screen);
			break;
		case ModelScreen.GameOver:
			if (SafeArea.IsRestricted)
			{
				modelGo.transform.localPosition = new Vector3(0f, -12f, 0f);
				modelGo.transform.localRotation = Quaternion.Euler(new Vector3(-6f, 180f, 0f));
			}
			characterModel.ChangeCharacterModel(modelName, modelVersion, true, true);
			characterModel.HideAllPowerups();
			characterModel.StartIdleAnimations(screen);
			break;
		case ModelScreen.Boards:
			characterModel.ChangeCharacterModel(modelName, modelVersion, false, true);
			characterModel.HideAllPowerups();
			characterModel.StartEyeAnimations(screen);
			break;
		case ModelScreen.CelebrationCharacterUnlock:
		case ModelScreen.CelebrationTopRun:
			if (characterId == Characters.CharacterId.halloweenCharacter01)
			{
				modelGo.transform.localScale = new Vector3(20f, 20f, 20f);
			}
			characterModel.ChangeCharacterModel(modelName, modelVersion, screen != ModelScreen.CelebrationTopRun, true);
			characterModel.HideAllPowerups();
			characterModel.StartEyeAnimations(ModelScreen.CelebrationTopRun);
			switch (screen)
			{
			case ModelScreen.CelebrationCharacterUnlock:
				characterModel.StartIdleAnimations(screen);
				celebrationBackgroundHelper.Show(Quaternion.Euler(_discoBgRotation), false, true);
				celebrationShouldAnimateStripes = false;
				_isCelebrationBackgroundRotating = true;
				break;
			case ModelScreen.CelebrationTopRun:
			{
				TopRunIdleAnimation topRunIdleAnimation = characterModel.TopRunIdleAnimation;
				if (topRunIdleAnimation != null)
				{
					topRunIdleAnimation.TR_ActivateIdle();
				}
				else
				{
					characterModel.StartIdleAnimations(screen);
				}
				break;
			}
			}
			_isCelebrationCharacterScreen = true;
			break;
		case ModelScreen.CelebrationBoardUnlock:
		case ModelScreen.CelebrationHighScore:
			characterModel.ChangeCharacterModel(modelName, modelVersion, false, false);
			characterModel.HideAllPowerups();
			if (screen == ModelScreen.CelebrationBoardUnlock)
			{
				GameObject hoverboardGameObject = characterModel.GetHoverboardGameObject();
				bool willAllowSignatureAnimation = false;
				HoverboardModelPreviewFactory.Instance.ChangeHoverboard(_currentMysteryBoxBoardId, hoverboardGameObject, characterModel.CharacterAnimation, true, willAllowSignatureAnimation, Hoverboard.BoardAbility.None);
			}
			Utility.SetLayerRecursively(modelGo.transform, TutorialPopupAnchor.layer);
			celebrationShouldAnimateStripes = true;
			_isCelebrationCharacterScreen = false;
			_isCelebrationBackgroundRotating = false;
			StartCoroutine("AnimateUnlockBackground", modelGo);
			if (screen == ModelScreen.CelebrationHighScore)
			{
				StartCoroutine("AnimateRunningCharacterForBragCelebration", modelGo);
			}
			break;
		case ModelScreen.LimitedCharacterPopup:
			characterModel.ChangeCharacterModel(modelName, modelVersion, true, true);
			characterModel.HideAllPowerups();
			characterModel.StartIdleAnimations(screen);
			break;
		case ModelScreen.LimitedBoardPopup:
			characterModel.ChangeCharacterModel(modelName, modelVersion, false, true);
			characterModel.HideAllPowerups();
			characterModel.StartEyeAnimations(screen);
			break;
		case ModelScreen.BoardPromotionPopup:
			characterModel.ChangeCharacterModel(modelName, modelVersion, false, true);
			characterModel.HideAllPowerups();
			characterModel.StartEyeAnimations(screen);
			break;
		case ModelScreen.FirstPurchaseBonusBigCharacter:
			characterModel.ChangeCharacterModel(modelName, modelVersion, true, true);
			characterModel.HideAllPowerups();
			characterModel.StartCustomAnimation(screen, false, _firstPurchaseBonusBreakDanceAnimation, _firstPurchaseBonusBreakDanceAnimation2);
			break;
		case ModelScreen.CloudSaveReminderPopup:
			characterModel.ChangeCharacterModel(modelName, modelVersion, false, true);
			characterModel.HideAllPowerups();
			characterModel.StartIdleAnimations(ModelScreen.CloudSaveReminderPopup);
			break;
		case ModelScreen.CharacterPromotionPopup:
		{
			CharacterPromotion characterPromotion = PromotionManager.Instance.TryGetCurrentPromotion<CharacterPromotion>();
			if (characterPromotion == null)
			{
				Debug.LogError("Current promotion isn't a character promotion");
				return modelGo;
			}
			characterModel.ChangeCharacterModel(modelName, modelVersion, characterPromotion.UseSpecialAssets, true);
			characterModel.HideAllPowerups();
			characterModel.StartEyeAnimations(screen);
			characterModel.StartCustomAnimation(screen, false, characterPromotion.CharacterAnimations);
			break;
		}
		case ModelScreen.BirthdayBoardPopup:
			characterModel.ChangeCharacterModel(modelName, modelVersion, false, true);
			characterModel.HideAllPowerups();
			characterModel.StartEyeAnimations(screen);
			break;
		}
		if (!_cachedModelScreenDataEntries.ContainsKey(screen) && cache)
		{
			_cachedModelScreenDataEntries.Add(screen, modelScreenData);
		}
		return modelGo;
	}

	[UsedImplicitly]
	private IEnumerator AnimateRunningCharacterForBragCelebration(GameObject model)
	{
		CharacterAnimationControl _charAnimCtrl = model.GetComponentInChildren<CharacterAnimationControl>();
		model.transform.localPosition = CHAR_FROM_POS;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / 3.5f;
			factor = Mathf.Clamp01(factor);
			float animCurveFactor = celebrationBackgroundHelper.GetBragCelebrationAnimationCurveValue(factor);
			float inverseFactor = 1f - factor;
			celebrationBackgroundHelper.SetParticleAnimationSpeed(Mathf.Lerp(0.5f, 3.5f, inverseFactor));
			float runFactor = Mathf.Lerp(3f, 1f, factor * 3.5f / 2f);
			_charAnimCtrl.Play();
			_charAnimCtrl.Speed(runFactor);
			model.transform.localPosition = Vector3.Lerp(CHAR_FROM_POS, CHAR_TO_POS, animCurveFactor);
			yield return null;
		}
	}

	[UsedImplicitly]
	private IEnumerator AnimateUnlockBackground(GameObject go)
	{
		Vector3 charWithBoardPosition = new Vector3(-80f, -100f, 0f);
		Vector3 rotation1CharWithBoard = new Vector3(27f, 205f, 16f);
		Vector3 rotation2CharWithBoard = new Vector3(350f, 140f, 10f);
		Vector3 rotation3CharWithBoard = new Vector3(5f, 210f, 0f);
		Vector3 offset1PosBoard = new Vector3(0f, 0f, -30f);
		Vector3 offset2PosBoard = new Vector3(0f, 0f, -170f);
		Vector3 offset3PosBoard = new Vector3(0f, 0f, -100f);
		Vector3 charPosition = new Vector3(-85f, -110f, 0f);
		Vector3 rotation1character = new Vector3(0f, 130f, 0f);
		Vector3 rotation2character = new Vector3(0f, 180f, 0f);
		Vector3 rotation3character = new Vector3(0f, 240f, 0f);
		Vector3 offset1PosCharacter = new Vector3(-10f, 0f, -180f);
		Vector3 offset2PosCharacter = new Vector3(0f, 0f, -200f);
		Vector3 offset3PosCharacter = new Vector3(5f, 0f, -200f);
		Vector3 offset1BgRotationCharScreen = new Vector3(35f, 20f, 0f);
		Vector3 offset2BgRotationCharScreen = new Vector3(35f, 0f, 0f);
		Vector3 offset3BgRotationCharScreen = new Vector3(35f, -30f, 0f);
		int index = 0;
		Vector3 currentRotation = new Vector3(0f, 0f, 0f);
		Vector3 currentOffset = new Vector3(0f, 0f, 0f);
		Vector3 currentBackgroundRotationOffset = new Vector3(0f, 0f, 0f);
		Vector3 currentCharPosition = new Vector3(0f, 0f, 0f);
		if (_currentActivatedScreenModel == ModelScreen.CelebrationHighScore)
		{
			currentBackgroundRotationOffset = new Vector3(0f, 0f, 350f);
			if ((float)Screen.height / (float)Screen.width > 2f)
			{
				currentBackgroundRotationOffset = new Vector3(0f, -5f, 350f);
			}
			UpdateCelebrationRotation(go, rotation3CharWithBoard, charWithBoardPosition, offset3PosBoard, currentBackgroundRotationOffset, celebrationShouldAnimateStripes, _isCelebrationBackgroundRotating);
			yield break;
		}
		while (true)
		{
			switch (index % 3)
			{
			case 0:
				if (_isCelebrationCharacterScreen)
				{
					currentRotation = rotation1character;
					currentOffset = offset1PosCharacter;
					currentBackgroundRotationOffset = offset1BgRotationCharScreen;
					currentCharPosition = charPosition;
				}
				else
				{
					currentRotation = rotation1CharWithBoard;
					currentOffset = offset1PosBoard;
					currentCharPosition = charWithBoardPosition;
				}
				break;
			case 1:
				if (_isCelebrationCharacterScreen)
				{
					currentRotation = rotation2character;
					currentOffset = offset2PosCharacter;
					currentBackgroundRotationOffset = offset2BgRotationCharScreen;
					currentCharPosition = charPosition;
				}
				else
				{
					currentRotation = rotation2CharWithBoard;
					currentOffset = offset2PosBoard;
					currentCharPosition = charWithBoardPosition;
				}
				break;
			default:
				if (_isCelebrationCharacterScreen)
				{
					currentRotation = rotation3character;
					currentOffset = offset3PosCharacter;
					currentBackgroundRotationOffset = offset3BgRotationCharScreen;
					currentCharPosition = charPosition;
				}
				else
				{
					currentRotation = rotation3CharWithBoard;
					currentOffset = offset3PosBoard;
					currentCharPosition = charWithBoardPosition;
				}
				break;
			}
			UpdateCelebrationRotation(go, currentRotation, currentCharPosition, currentOffset, currentBackgroundRotationOffset, celebrationShouldAnimateStripes, _isCelebrationBackgroundRotating);
			index++;
			yield return new WaitForSeconds(2.5f);
		}
	}

	private void UpdateCelebrationRotation(GameObject go, Vector3 charRotation, Vector3 charPosition, Vector3 charPositionOffset, Vector3 backgroundRotationOffset, bool shouldAnimateStripes, bool rotateBackground)
	{
		if (go != null)
		{
			Vector3 euler = charRotation + backgroundRotationOffset;
			Quaternion rotation = Quaternion.Euler(euler);
			Quaternion localRotation = Quaternion.Euler(charRotation);
			go.transform.localRotation = localRotation;
			celebrationBackgroundHelper.Show(rotation, shouldAnimateStripes, rotateBackground);
			Vector3 localPosition = charPosition;
			localPosition += charPositionOffset;
			go.transform.localPosition = localPosition;
		}
	}

	private bool ShouldCreateABrandNewModel()
	{
		bool result = false;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			bool flag = SystemInfo.operatingSystem.Contains("iPhone OS 7") || SystemInfo.operatingSystem.Contains("iOS 7");
			bool flag2 = SystemInfo.graphicsDeviceName.Contains("Apple A7");
			result = flag && flag2;
		}
		return result;
	}

	public void ActivateModelScreen(ModelScreen screen)
	{
		if (_cachedModelScreenDataEntries.ContainsKey(screen))
		{
			_cachedModelScreenDataEntries[screen].Activate();
		}
		GameObject screenAnchor = GetScreenAnchor(screen);
		screenAnchor.SetActive(true);
	}

	public void DeactivateModelScreen(ModelScreen screen)
	{
		if (_cachedModelScreenDataEntries.ContainsKey(screen))
		{
			_cachedModelScreenDataEntries[screen].Deactivate();
		}
		GameObject screenAnchor = GetScreenAnchor(screen);
		screenAnchor.SetActive(false);
	}

	public void ClearScreenAnchor(ModelScreen screen)
	{
		if (_cachedModelScreenDataEntries.ContainsKey(screen))
		{
			_cachedModelScreenDataEntries.Remove(screen);
		}
		GameObject screenAnchor = GetScreenAnchor(screen);
		if (!screenAnchor)
		{
			return;
		}
		Transform transform = screenAnchor.transform;
		foreach (Transform item in transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}

	private GameObject GetScreenAnchor(ModelScreen screen)
	{
		GameObject gameObject = null;
		switch (screen)
		{
		case ModelScreen.Character:
		case ModelScreen.Boards:
		case ModelScreen.FirstPurchaseBonusBigCharacter:
			return CharacterAnchor;
		case ModelScreen.GameOver:
			return GameOverAnchor;
		case ModelScreen.TutorialPopup:
			return TutorialPopupAnchor;
		case ModelScreen.LimitedCharacterPopup:
			return limitedCharacterPopupAnchor;
		case ModelScreen.LimitedBoardPopup:
			return limitedBoardPopupAnchor;
		case ModelScreen.BoardPromotionPopup:
			return BoardPromotionPopupAnchor;
		case ModelScreen.BirthdayBoardPopup:
			return birthdayBoardPopupAnchor;
		case ModelScreen.CharacterPromotionPopup:
			return CharacterPromotionAnchor;
		case ModelScreen.CelebrationCharacterUnlock:
		case ModelScreen.CelebrationBoardUnlock:
		case ModelScreen.CelebrationHighScore:
		case ModelScreen.CelebrationTopRun:
			return TutorialPopupAnchor;
		default:
			return CharacterAnchor;
		}
	}

	private void Init()
	{
		_cachedModelScreenDataEntries = new Dictionary<ModelScreen, ModelScreenData>();
	}
}
