using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Data.Managers;
using Data.Save;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using SYBO.Subway.Info;
using UI.Generic;
using UI.Screens.CharacterScreenInternal;
using UnityEngine;
using Utilities;

namespace UI.Screens
{
	public class CharacterScreen : UIScreen
	{
		[SerializeField]
		private UIGrid _characterGrid;

		[SerializeField]
		private UIPanel _scrollPanel;

		[SerializeField]
		private GameObject _scrollPlaceholderPrefab;

		[SerializeField]
		private UILabel _nameTitle;

		[SerializeField]
		private UILabel _nameSubtitle;

		[SerializeField]
		private UIWidget _ribbonsContainer;

		[SerializeField]
		private RibbonHelper _comebackRibbon;

		[SerializeField]
		private RibbonHelper _multiplierRibbon;

		[SerializeField]
		private AudioClip _selectSound;

		[SerializeField]
		private UICharacterThemeShifter _characterThemeShifter;

		[SerializeField]
		private GameObject _characterDayParticleSystem;

		[SerializeField]
		private UISprite _characterDayMultiplierSprite;

		[SerializeField]
		private GameObject _characterSelectButtonPrefab;

		[SerializeField]
		private GameObject _centerAnchor;

		[SerializeField]
		private TimerHelper _timerPrefab;

		[SerializeField]
		private UIWidget _timerContainer;

		[SerializeField]
		private GameObject _scrollListParent;

		public static bool forceRefreshOnCharacterScreen = false;

		public static bool forceCenterOnEventCharacter = false;

		public static bool forceCenterOnCurrentlySelectedCharacter = false;

		public static bool externalForceCenterOnCharacter = false;

		public static Characters.CharacterId externalSelectedCharacter = Characters.CharacterId.slick;

		public readonly Vector3 scrollCharacterPosition = new Vector3(0f, -20f, 0f);

		public readonly Vector3 scrollCharacterRotation = new Vector3(53f, 183f, 360f);

		private Characters.CharacterId _initiallyCenteredCharacterId;

		public const float DEFAULT_SCALE_FACTOR = 7f;

		public const float FOCUSED_SCALE_FACTOR = 10f;

		private const int SCROLL_CHARACTER_VISIBILITY_COUNT = 4;

		private const int SCROLL_CHARACTER_UI_VISIBILITY_COUNT = 7;

		private const int SCALE_MODEL_COUNT = 4;

		private const int SYNCHRONOUSLY_LOADED_MODELS = 5;

		private const float SCALE_MODEL_MAX_DISTANCE_FACTOR = 1.5f;

		private static readonly Vector3 SELECT_BUTTON_POSITION = new Vector3(62f, 0f, 0f);

		private const string CHARACTER_SCREEN_DEBUG_LOG_TAG = "Character Screen: ";

		private const Characters.CharacterId DEFAULT_CHARACTER_TO_CENTER = Characters.CharacterId.slick;

		private List<KeyValuePair<Characters.CharacterId, Characters.Model>> _characterList = new List<KeyValuePair<Characters.CharacterId, Characters.Model>>();

		private CharacterListManager _characterListManager;

		private CharacterSelectButton _characterSelectButton;

		private CharacterScreenManager _screenManagerInstance;

		private float _cellWidth = 1f;

		private bool _hasShownModel;

		private SSEventType _cachedActiveEventType;

		private long _cachedActiveEventID = -1L;

		private bool _inappOverlayActive;

		private bool _popupActive;

		private bool _charactersEnabled;

		private bool _firstShow = true;

		private bool _hasInited;

		private TimerHelper _timer;

		private CenterOnChild _centerer;

		private bool _scrollListInitialized;

		private bool _initialCenteredCharacterSelected;

		private int _cachedCenteredIndex = -1;

		public Characters.CharacterId initiallyCenteredCharacter
		{
			get
			{
				return _initiallyCenteredCharacterId;
			}
			private set
			{
				_initiallyCenteredCharacterId = value;
			}
		}

		public int cachedCenteredIndex
		{
			get
			{
				return _cachedCenteredIndex;
			}
		}

		protected override void Init()
		{
			_screenManagerInstance = CharacterScreenManager.Instance;
			_cellWidth = _characterGrid.cellWidth;
			_centerer = _characterGrid.GetComponent<CenterOnChild>();
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			_cachedActiveEventType = ((activeEventManager != null) ? activeEventManager.GetEventType() : SSEventType.None);
			InitializeScrollList();
			InitializeSelectButton();
			InitializeTimer();
			HandleSafeArea();
			VerifyCharacterAmount();
			_hasInited = true;
		}

		private void HandleSafeArea()
		{
			if (SafeArea.IsRestricted)
			{
				NGUIMath.MoveWidget(_ribbonsContainer, 0f, -SafeArea.OFFSET_TOP);
				_ribbonsContainer.ResetAndUpdateAnchors();
				_scrollListParent.transform.OffsetLocalPosY(SafeArea.OFFSET_BOTTOM);
			}
		}

		public override void GainFocus()
		{
			AssetBundleManager.Instance.PerformAvailabilityCheck();
			_characterDayMultiplierSprite.gameObject.SetActive(ShouldEnableCharacterDayParticleSystem());
		}

		public override void LoseFocus()
		{
			_characterDayMultiplierSprite.gameObject.SetActive(false);
		}

		private void SelectInitialCenteredCharacter()
		{
			if (_initialCenteredCharacterSelected)
			{
				return;
			}
			_initialCenteredCharacterSelected = true;
			if (externalForceCenterOnCharacter)
			{
				initiallyCenteredCharacter = externalSelectedCharacter;
				return;
			}
			Characters.CharacterId? characterTypeActiveInEvent = CharacterScreenManager.Instance.GetCharacterTypeActiveInEvent();
			initiallyCenteredCharacter = PlayerSave.currentCharacterId;
			if (characterTypeActiveInEvent.HasValue)
			{
				IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
				if (activeEventManager != null && (activeEventManager.GetEventType() == SSEventType.CharacterDay || activeEventManager.GetEventType() == SSEventType.ExpiredLimitedCharacter))
				{
					bool found;
					Characters.CharacterId eventValue = activeEventManager.GetEventValue<Characters.CharacterId>(out found);
					if (found)
					{
						initiallyCenteredCharacter = eventValue;
						forceCenterOnEventCharacter = true;
					}
				}
			}
			else
			{
				forceCenterOnCurrentlySelectedCharacter = true;
			}
		}

		private void InitializeCentererIfNeeded()
		{
			SelectInitialCenteredCharacter();
			if (externalForceCenterOnCharacter)
			{
				forceCenterOnEventCharacter = false;
				externalForceCenterOnCharacter = false;
				forceCenterOnCurrentlySelectedCharacter = false;
				CenterScrollOnCharacterType(externalSelectedCharacter);
				ChangeCurrentlyShownCharacter(externalSelectedCharacter, CharacterManager.GetOutfitIndex(externalSelectedCharacter));
			}
			else if (forceCenterOnEventCharacter)
			{
				forceCenterOnEventCharacter = false;
				externalForceCenterOnCharacter = false;
				forceCenterOnCurrentlySelectedCharacter = false;
				CenterScrollOnCharacterType(initiallyCenteredCharacter);
				ChangeCurrentlyShownCharacter(initiallyCenteredCharacter, CharacterManager.GetOutfitIndex(initiallyCenteredCharacter));
			}
			else if (forceCenterOnCurrentlySelectedCharacter)
			{
				forceCenterOnEventCharacter = false;
				externalForceCenterOnCharacter = false;
				forceCenterOnCurrentlySelectedCharacter = false;
				CenterScrollOnCharacterType(PlayerSave.currentCharacterId);
				ChangeCurrentlyShownCharacter(PlayerSave.currentCharacterId, PlayerSave.currentOutfitIndex);
			}
			else if (_hasShownModel)
			{
				ChangeCurrentlyShownCharacter(_screenManagerInstance.currentlyShownCharacter, _screenManagerInstance.currentlyShownThemeIndex);
			}
			else
			{
				Update();
			}
		}

		private void InitializeSelectButton()
		{
			GameObject gameObject = _centerAnchor.AddChild(_characterSelectButtonPrefab);
			_characterSelectButton = gameObject.GetComponent<CharacterSelectButton>();
			gameObject.transform.localPosition = SELECT_BUTTON_POSITION;
			UpdateThemeShifter(_screenManagerInstance.currentlyShownCharacter, _screenManagerInstance.currentlyShownThemeIndex);
			_characterSelectButton.InitButton(CharacterSelectButton.ButtonContext.CharacterScreen);
		}

		private void InitializeTimer()
		{
			_timer = UnityEngine.Object.Instantiate(_timerPrefab, _timerContainer.transform);
		}

		protected override void Show()
		{
			AssetBundleManager.Instance.PerformAvailabilityCheck();
			if (_firstShow)
			{
				UIScreenController.Instance.Camera3D.enabled = false;
				UIScreenController.Instance.Camera3DClip.enabled = false;
				forceRefreshOnCharacterScreen = false;
			}
			UIScreenController.OnPopupShown += OnPopupShown;
			UIScreenController.OnLastPopupClosed += OnLastPopupClosed;
			SSEventsManagerController.OnSSEventStartedHandler += CharacterEventStarted;
			SSEventsManagerController.OnSSEventFinishedHandler += CharacterEventFinished;
			UIScreenController.OnApplicationResumed += RefreshCharacterScreen;
			RefreshCharacterListIfNeeded();
			_characterThemeShifter.OnThemeButtonPressed += OnThemeButtonClicked;
			CharacterScreenManager.characterUnlocked += OnCharacterUnlocked;
			CharacterScreenManager.onCharacterSelected += UpdateSelectedCharacter;
			CharacterScreenManager.onFacebookStatusChanged += RefreshCharacterScreen;
			InitializeCentererIfNeeded();
			bool active = Outfits.HasCharacter(_screenManagerInstance.currentlyShownCharacter);
			_characterThemeShifter.gameObject.SetActive(active);
			if (_firstShow)
			{
				CoroutineUtil.StartCoroutine(EnableCamerasDelayed());
			}
			UpdateUIElements();
			RefreshScrollListUiOverlay(GetCenterIndex());
		}

		private IEnumerator EnableCamerasDelayed()
		{
			_firstShow = false;
			yield return new WaitForEndOfFrame();
			UIScreenController.Instance.Camera3D.enabled = true;
			UIScreenController.Instance.Camera3DClip.enabled = true;
		}

		protected override void Hide()
		{
			UIScreenController.OnPopupShown -= OnPopupShown;
			UIScreenController.OnLastPopupClosed -= OnLastPopupClosed;
			CharacterScreenManager.Instance.OnCharacterScreenHide();
			_characterThemeShifter.OnThemeButtonPressed -= OnThemeButtonClicked;
			UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.Character);
			CharacterScreenManager.characterUnlocked -= OnCharacterUnlocked;
			CharacterScreenManager.onCharacterSelected -= UpdateSelectedCharacter;
			CharacterScreenManager.onFacebookStatusChanged -= RefreshCharacterScreen;
			SSEventsManagerController.OnSSEventStartedHandler -= CharacterEventStarted;
			SSEventsManagerController.OnSSEventFinishedHandler -= CharacterEventFinished;
			UIScreenController.OnApplicationResumed -= RefreshCharacterScreen;
		}

		private void InitializeScrollList()
		{
			_initialCenteredCharacterSelected = false;
			SelectInitialCenteredCharacter();
			ClearScrollList();
			CacheActiveEventType();
			_characterList = CharacterManager.GetAvailableCharacters(true);
			_scrollListInitialized = false;
			_cachedCenteredIndex = -1;
			_characterListManager = new CharacterListManager(_characterList.Count, this);
			int num = 0;
			foreach (KeyValuePair<Characters.CharacterId, Characters.Model> character in _characterList)
			{
				Characters.CharacterId key = character.Key;
				GameObject gameObject = _characterGrid.gameObject.AddChild(_scrollPlaceholderPrefab);
				gameObject.name = key.ToString();
				CharacterScrollListSlot component = gameObject.GetComponent<CharacterScrollListSlot>();
				component.index = num;
				component.characterId = key;
				component.themeIndex = CharacterManager.GetOutfitIndex(key);
				component.ui.Init2D(key, _screenManagerInstance.currentlyShownCharacter == key);
				_characterListManager[num] = component;
				num++;
			}
			_characterListManager.LoadAInitialModels();
			_characterGrid.SendMessage("Start");
			_characterGrid.Reposition();
			StartCoroutine(DelayedInitCenterChar());
		}

		private void ClearScrollList()
		{
			if (_characterListManager != null)
			{
				foreach (CharacterScrollListSlot item in _characterListManager)
				{
					item.gameObject.SetActive(false);
					item.transform.parent = null;
					UnityEngine.Object.Destroy(item.gameObject);
				}
				_characterListManager = null;
			}
			if (!_characterGrid.gameObject.activeSelf)
			{
				_characterGrid.gameObject.SetActive(true);
			}
			_characterGrid.transform.localPosition = new Vector3(0f, _characterGrid.transform.localPosition.y, _characterGrid.transform.localPosition.z);
			_scrollPanel.cachedTransform.localPosition = Vector3.zero;
			_scrollPanel.clipOffset = new Vector2(0f, 0f);
			_centerer.ClearCenterObject();
			SpringPanel component = _scrollPanel.GetComponent<SpringPanel>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
		}

		private IEnumerator DelayedInitCenterChar()
		{
			yield return new WaitForEndOfFrame();
			CenterScrollOnCharacterType(_screenManagerInstance.currentlyShownCharacter);
		}

		private void RefreshCharacterListIfNeeded()
		{
			if (_scrollListInitialized && _charactersEnabled && DetectChangeInCharacterList())
			{
				InitializeScrollList();
			}
		}

		public void OnInitialModelLoadingComplete()
		{
			_scrollListInitialized = true;
		}

		public void OnSingleModelLoaded(Characters.CharacterId characterId)
		{
			if (base.gameObject.activeInHierarchy && _screenManagerInstance.currentlyShownCharacter == characterId)
			{
				ChangeCurrentlyShownCharacter(_screenManagerInstance.currentlyShownCharacter, _screenManagerInstance.currentlyShownThemeIndex);
			}
		}

		private bool CacheActiveEventType()
		{
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			SSEventType sSEventType = ((activeEventManager != null) ? activeEventManager.GetEventType() : SSEventType.None);
			long num = ((activeEventManager == null) ? (-1) : activeEventManager.GetEventID());
			if (_cachedActiveEventType != sSEventType || _cachedActiveEventID != num)
			{
				_cachedActiveEventType = sSEventType;
				_cachedActiveEventID = num;
				return true;
			}
			return false;
		}

		private bool DetectChangeInCharacterList()
		{
			bool flag = CacheActiveEventType();
			if (forceRefreshOnCharacterScreen)
			{
				forceRefreshOnCharacterScreen = false;
				return true;
			}
			if (!flag)
			{
				List<KeyValuePair<Characters.CharacterId, Characters.Model>> availableCharacters = CharacterManager.GetAvailableCharacters();
				int count = availableCharacters.Count;
				if (count != _characterList.Count)
				{
					return true;
				}
				for (int i = 0; i < count; i++)
				{
					if (!_characterList.ValidIndex(i))
					{
						return true;
					}
					if (availableCharacters[i].Key != _characterList[i].Key)
					{
						return true;
					}
				}
			}
			return flag;
		}

		private void Update()
		{
			if (!_hasInited)
			{
				return;
			}
			UpdateCharacterModelsState();
			if (_centerer.selectedObject != null)
			{
				CharacterScrollListSlot component = _centerer.selectedObject.GetComponent<CharacterScrollListSlot>();
				int index = component.index;
				Characters.CharacterId characterId = component.characterId;
				if (!_hasShownModel)
				{
					_hasShownModel = true;
					ChangeCurrentlyShownCharacter(characterId, CharacterManager.GetOutfitIndex(characterId));
					component.ui.CenteredInMenu();
				}
				Characters.CharacterId currentlyShownCharacter = _screenManagerInstance.currentlyShownCharacter;
				if (currentlyShownCharacter != characterId)
				{
					RefreshScrollListUiOverlay(index);
					int index2 = CharacterManager.GetOutfitIndex(characterId);
					if (characterId == PlayerSave.currentCharacterId)
					{
						index2 = PlayerSave.currentOutfitIndex;
					}
					ChangeCurrentlyShownCharacter(characterId, index2);
					component.ui.CenteredInMenu();
				}
			}
			ScaleScrollModels();
			if (UIScreenController.Instance.isShowingPopup)
			{
				if (!_popupActive || !_inappOverlayActive)
				{
					_popupActive = true;
				}
			}
			else if (_popupActive && !_inappOverlayActive)
			{
				_popupActive = false;
			}
			if (UIScreenController.Instance.inAppPurchaseOverlay.activeInHierarchy)
			{
				if (!_inappOverlayActive)
				{
					_inappOverlayActive = true;
				}
			}
			else if (_inappOverlayActive)
			{
				_inappOverlayActive = false;
			}
			if (_inappOverlayActive || _popupActive)
			{
				if (_charactersEnabled)
				{
					_characterListManager.DisableAllSlots();
					_charactersEnabled = false;
				}
			}
			else if (!_charactersEnabled)
			{
				_characterListManager.EnableVisibleSlots(_cachedCenteredIndex);
				_charactersEnabled = true;
				RefreshCharacterScreen();
			}
		}

		private int GetCenterIndex()
		{
			if (_centerer.selectedObject != null)
			{
				CharacterScrollListSlot component = _centerer.selectedObject.GetComponent<CharacterScrollListSlot>();
				return component.index;
			}
			return 0;
		}

		private void RefreshScrollListUiOverlay(int centerIndex)
		{
			foreach (CharacterScrollListSlot item in _characterListManager)
			{
				if (item.index != centerIndex && IsWithinRange(centerIndex, item.index, GetCharacterUIVisibilityCount()))
				{
					item.ui.RefreshUI(false);
				}
			}
		}

		private bool IsWithinRange(int center, int position, int threshold)
		{
			return Math.Abs(position - center) <= threshold;
		}

		private void UpdateCharacterModelsState()
		{
			if (!_scrollListInitialized)
			{
				return;
			}
			int num = GetCenteredSlotIndex();
			if (num < 0)
			{
				num = 0;
			}
			if (_cachedCenteredIndex != num)
			{
				_cachedCenteredIndex = num;
				if (_characterListManager != null)
				{
					_characterListManager.UpdateSlotsState(num);
				}
			}
		}

		private void ChangeCurrentlyShownCharacter(Characters.CharacterId charId, int index)
		{
			_screenManagerInstance.currentlyShownCharacter = charId;
			_screenManagerInstance.currentlyShownThemeIndex = index;
			if (!UIScreenController.Instance.isShowingPopup)
			{
				UIModelController.Instance.ShowMenuModel(charId, index);
			}
			UpdateUIElements();
			UpdateThemeShifter(charId, index);
		}

		public void UpdateThemeShifter(Characters.CharacterId charId, int index)
		{
			bool flag = Outfits.HasCharacter(charId);
			_characterThemeShifter.gameObject.SetActive(flag);
			if (flag)
			{
				List<Outfit> themes = Outfits.TryGetOutfitsForCharacter(charId);
				_characterThemeShifter.MakeCustomThemesAvailable(themes);
				_characterThemeShifter.UpdateUIForCharacter(charId, index);
			}
		}

		public void UpdateUIElements()
		{
			Characters.CharacterId activeCharacterId = UIModelController.Instance.GetActiveCharacterId(UIModelController.ModelScreen.Character);
			Characters.Model model = Characters.characterData[activeCharacterId];
			Outfit outfitForCharacter = Outfits.GetOutfitForCharacter(activeCharacterId, _screenManagerInstance.currentlyShownThemeIndex);
			bool flag = CharacterManager.IsCharacterOwned(activeCharacterId);
			TimeSpan timeLeft = default(TimeSpan);
			bool showTimer = false;
			if (!flag && model.hasOnlineSettings)
			{
				showTimer = true;
				timeLeft = CharacterManager.CharacterExpirationTimespan(activeCharacterId);
				_timer.SetTimerColors(GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR, GlobalColorsGenerated.DISCOUNT_STICKER_COLOR);
			}
			bool flag2 = _characterSelectButton.CurrentlyShownCharacterMatchesEventItem();
			bool flag3 = flag2 && (_cachedActiveEventType == SSEventType.CharacterDay || _cachedActiveEventType == SSEventType.ExpiredLimitedCharacter);
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			if (activeEventManager != null && flag3 && (!flag || _cachedActiveEventType == SSEventType.CharacterDay))
			{
				showTimer = true;
				timeLeft = TimeSpan.FromSeconds(SSEventsManagerController.instance.TimeLeftInSeconds());
				_timer.SetTimerColors(GlobalColors.GIFT_LABEL_COLOR, GlobalColors.GIFT_COLOR);
			}
			ShowTimer(showTimer, timeLeft);
			SetEventRibbonActive(flag2);
			_nameTitle.text = Strings.Get(model.name);
			_nameTitle.color = ColorData.instance.midnight;
			_nameSubtitle.text = (string.IsNullOrEmpty(Strings.Get(model.characterLimitedDescription)) ? string.Empty : Strings.Get(model.characterLimitedDescription));
			if (outfitForCharacter != null)
			{
				Outfit outfit = outfitForCharacter;
				string text = Strings.Get(outfit.title);
				if (!string.IsNullOrEmpty(text))
				{
					_nameSubtitle.text = text;
				}
			}
			_nameSubtitle.color = ColorData.instance.peacock;
			_characterDayParticleSystem.SetActive(ShouldEnableCharacterDayParticleSystem());
			_characterDayMultiplierSprite.gameObject.SetActive(ShouldEnableCharacterDayParticleSystem());
			UpdateThemeShifter(_screenManagerInstance.currentlyShownCharacter, _screenManagerInstance.currentlyShownThemeIndex);
			UpdateCharacterSticker(_screenManagerInstance.currentlyShownCharacter, _screenManagerInstance.currentlyShownThemeIndex);
			_characterSelectButton.ReloadButton();
		}

		private void ShowTimer(bool showTimer, TimeSpan timeLeft)
		{
			_timer.gameObject.SetActive(showTimer);
			if (showTimer)
			{
				_timer.StartTimer(timeLeft);
			}
			else
			{
				_timer.StopTimer();
			}
		}

		private void UpdateSelectedCharacter()
		{
			Characters.CharacterId currentlyShownModel = UIModelController.Instance.currentlyShownModel;
			int currentlyShownThemeIndex = UIModelController.Instance.currentlyShownThemeIndex;
			bool flag = CharacterManager.IsCharacterOwned(currentlyShownModel);
			bool flag2 = CharacterManager.IsOutfitUnlocked(currentlyShownModel, currentlyShownThemeIndex);
			if (flag && flag2)
			{
				CharacterManager.SetOutfitSelectionIndex(currentlyShownModel, currentlyShownThemeIndex);
				_characterThemeShifter.UpdateUIForCharacter(currentlyShownModel, currentlyShownThemeIndex);
				_characterSelectButton.ReloadButton();
				RefreshCharacterModelInScrollList(currentlyShownModel, currentlyShownThemeIndex);
			}
		}

		private void RefreshCharacterModelInScrollList(Characters.CharacterId charId, int themeIndex)
		{
			if (_scrollListInitialized)
			{
				_characterListManager.RefreshCharacter(charId, themeIndex);
			}
		}

		private void CharacterEventStarted()
		{
			Characters.CharacterId? characterId = null;
			bool found = false;
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			if (activeEventManager != null && (activeEventManager.GetEventType() == SSEventType.CharacterDay || activeEventManager.GetEventType() == SSEventType.ExpiredLimitedCharacter))
			{
				characterId = activeEventManager.GetEventValue<Characters.CharacterId>(out found);
			}
			if (found)
			{
				_screenManagerInstance.currentlyShownCharacter = characterId.Value;
				_screenManagerInstance.currentlyShownThemeIndex = CharacterManager.GetOutfitIndex(characterId.Value);
				RefreshCharacterListIfNeeded();
				ChangeCurrentlyShownCharacter(_screenManagerInstance.currentlyShownCharacter, _screenManagerInstance.currentlyShownThemeIndex);
			}
		}

		private void CharacterEventFinished()
		{
			_screenManagerInstance.currentlyShownCharacter = PlayerSave.currentCharacterId;
			_screenManagerInstance.currentlyShownThemeIndex = CharacterManager.GetOutfitIndex(_screenManagerInstance.currentlyShownCharacter);
			RefreshCharacterListIfNeeded();
			ChangeCurrentlyShownCharacter(_screenManagerInstance.currentlyShownCharacter, _screenManagerInstance.currentlyShownThemeIndex);
		}

		private void RefreshCharacterScreen()
		{
			RefreshCharacterListIfNeeded();
			UpdateUIElements();
		}

		private void CenterScrollOnCharacterType(Characters.CharacterId charId)
		{
			Characters.CharacterId characterId = charId;
			int index;
			if (_characterListManager[characterId] == null)
			{
				characterId = PlayerSave.currentCharacterId;
				if (_characterListManager[characterId] == null)
				{
					characterId = Characters.CharacterId.slick;
					index = _characterListManager[characterId].index;
				}
				else
				{
					index = _characterListManager[characterId].index;
				}
			}
			else
			{
				index = _characterListManager[charId].index;
			}
			if (_characterListManager[index] == null)
			{
				LogError("Could not find character to center on. Aborting centering.");
				return;
			}
			_centerer.CenterOnTransform(_characterListManager[index].transform, true);
			_centerer.Recenter();
		}

		private void OnCharacterUnlocked(Characters.CharacterId charId, int version)
		{
			ChangeCurrentlyShownCharacter(charId, version);
			if (version > 0)
			{
				Outfit outfitForCharacter = Outfits.GetOutfitForCharacter(charId, version);
				if (outfitForCharacter != null)
				{
					Flurry.LogEventWithAParameter("Outfit bought", "Id", Strings.Get(outfitForCharacter.title));
				}
			}
		}

		[UsedImplicitly]
		public void ScrollClicked(Vector2 pos)
		{
			Characters.CharacterId currentlyShownCharacter = _screenManagerInstance.currentlyShownCharacter;
			Characters.Model model = Characters.characterData[currentlyShownCharacter];
			bool flag = _centerer.CenterOnClosestChildAtPosition(pos);
			bool flag2 = CharacterManager.IsCharacterOwned(currentlyShownCharacter);
			int outfitIndex = CharacterManager.GetOutfitIndex(currentlyShownCharacter);
			bool flag3 = outfitIndex == _screenManagerInstance.currentlyShownThemeIndex;
			bool flag4 = true;
			if (model.unlockType == PurchaseData.UnlockType.Facebook && (!SocialManager.instance.facebookIsLoggedIn || CharacterScreenManager.Instance.IsFacebookLoginRunning()))
			{
				flag4 = false;
			}
			if (flag && flag2 && flag3 && flag4)
			{
				UIModelController.Instance.SelectThemeForCurrentModel(outfitIndex);
				UpdateSelectedCharacter();
				NGUITools.PlaySound(_selectSound);
			}
		}

		private void OnThemeButtonClicked(int index)
		{
			_screenManagerInstance.currentlyShownThemeIndex = index;
			UIModelController.Instance.ShowMenuModel(_screenManagerInstance.currentlyShownCharacter, index);
			UpdateUIElements();
		}

		private void UpdateCharacterSticker(Characters.CharacterId charId, int index)
		{
			Characters.Model model = Characters.characterData[charId];
			Outfit outfitForCharacter = Outfits.GetOutfitForCharacter(charId, index);
			bool flag = CharacterManager.IsCharacterOwned(charId);
			bool flag2 = outfitForCharacter != null && outfitForCharacter.price == 0 && !CharacterStickerManager.IsThemeUnlockedForCharacter(charId, index);
			bool flag3 = model.hasOnlineSettings && !flag && index == 0;
			bool flag4 = _characterSelectButton.CurrentlyShownCharacterMatchesEventItem() && (_cachedActiveEventType == SSEventType.CharacterDay || _cachedActiveEventType == SSEventType.ExpiredLimitedCharacter) && index == 0;
			if (flag2)
			{
				_characterSelectButton.SetSticker(CharacterSelectButton.StickerType.Gift);
			}
			else if (flag4)
			{
				_characterSelectButton.SetSticker(CharacterSelectButton.StickerType.Event);
			}
			else if (flag3)
			{
				_characterSelectButton.SetSticker(CharacterSelectButton.StickerType.Limited);
			}
			else
			{
				_characterSelectButton.SetSticker(CharacterSelectButton.StickerType.None);
			}
		}

		private void SetEventRibbonActive(bool characterMatchesEvent)
		{
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			if (activeEventManager != null)
			{
				SSEventType eventType = activeEventManager.GetEventType();
				bool flag = eventType == SSEventType.ExpiredLimitedCharacter && characterMatchesEvent;
				bool flag2 = eventType == SSEventType.CharacterDay && characterMatchesEvent;
				string firstLine = string.Empty;
				string secondLine = string.Empty;
				string thirdLine = string.Empty;
				if (flag || flag2)
				{
					activeEventManager.GetRibbonDescription(out firstLine, out secondLine, out thirdLine);
				}
				_comebackRibbon.Setup(new RibbonHelper.RibbonPayload
				{
					shouldShow = flag,
					line1 = firstLine,
					line2 = secondLine,
					line3 = thirdLine
				});
				_multiplierRibbon.Setup(new RibbonHelper.RibbonPayload
				{
					shouldShow = flag2,
					line1 = firstLine,
					line2 = secondLine,
					line3 = thirdLine
				});
			}
			else
			{
				_comebackRibbon.Setup(new RibbonHelper.RibbonPayload());
				_multiplierRibbon.Setup(new RibbonHelper.RibbonPayload());
			}
		}

		private void OnPopupShown(string popupName)
		{
			if (_characterDayParticleSystem.activeSelf)
			{
				_characterDayParticleSystem.SetActive(false);
			}
		}

		private void OnLastPopupClosed()
		{
			_characterDayParticleSystem.SetActive(ShouldEnableCharacterDayParticleSystem());
			if (_hasShownModel)
			{
				ChangeCurrentlyShownCharacter(_screenManagerInstance.currentlyShownCharacter, _screenManagerInstance.currentlyShownThemeIndex);
			}
		}

		private bool ShouldEnableCharacterDayParticleSystem()
		{
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.CharacterDay);
			return activeEventManager != null && !UIScreenController.Instance.isShowingPopup && _characterSelectButton.CurrentlyShownCharacterMatchesEventItem();
		}

		public void ScaleScrollModels(bool forceScaleSingleCharacter = false, int scrollIndex = 0)
		{
			float num = Mathf.Abs(_scrollPanel.transform.localPosition.x);
			int centeredSlotIndex = GetCenteredSlotIndex();
			if (centeredSlotIndex < 0)
			{
				centeredSlotIndex = _cachedCenteredIndex;
			}
			int num2 = Mathf.Max(0, centeredSlotIndex - 4);
			int num3 = Mathf.Min(_characterListManager.size, centeredSlotIndex + 4);
			float num4 = 1.5f * _cellWidth;
			for (int i = num2; i < num3; i++)
			{
				if (!forceScaleSingleCharacter || i == scrollIndex)
				{
					float num5 = Mathf.Abs(num - (float)i * _cellWidth);
					float num6 = Mathf.SmoothStep(10f, 7f, num5 / num4);
					CharacterScrollListSlot characterScrollListSlot = _characterListManager[i];
					if (characterScrollListSlot != null && characterScrollListSlot.model != null)
					{
						_characterListManager[i].model.transform.localScale = Vector3.one * num6;
					}
				}
			}
		}

		private int GetCenteredSlotIndex()
		{
			int result = -1;
			if (_centerer != null && _centerer.selectedObject != null)
			{
				CharacterScrollListSlot component = _centerer.selectedObject.GetComponent<CharacterScrollListSlot>();
				if (component != null)
				{
					result = component.index;
				}
			}
			return result;
		}

		public int GetCharacterVisibilityCount()
		{
			return 4;
		}

		public int GetCharacterUIVisibilityCount()
		{
			return 7;
		}

		public bool AreCharactersEnabled()
		{
			return _charactersEnabled;
		}

		public int GetAmountOfSyncModelsLoaded()
		{
			return 5;
		}

		private void VerifyCharacterAmount()
		{
		}

		[Conditional("ENABLE_CHARACTER_SCREEN_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_CHARACTER_SCREEN_LOGS")]
		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("Character Screen: " + msg, context);
		}
	}
}
