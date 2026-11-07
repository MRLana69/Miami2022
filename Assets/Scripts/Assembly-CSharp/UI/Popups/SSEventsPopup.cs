using System;
using System.Collections;
using System.Collections.Generic;
using Data.Managers;
using Data.Save;
using Info;
using JetBrains.Annotations;
using SYBO.Subway.Info;
using UI.Generic;
using UI.Generic.Layout;
using UI.Screens;
using UnityEngine;

namespace UI.Popups
{
	public class SSEventsPopup : UIPopup
	{
		[Serializable]
		private class EventTypeGameObjectPair
		{
			public SSEventType eventType;

			public GameObject gameObject;
		}

		[SerializeField]
		private List<EventTypeGameObjectPair> _stateGameObjects = new List<EventTypeGameObjectPair>();

		private const string ONLINE_SETTING_EVENT_ELEMENT_NEW_POSITION_OFFSET = "event_popup_element_new_position_offset";

		private const string ONLINE_SETTING_EVENT_ELEMENT_NEW_ROTATION = "event_popup_element_new_rotation";

		private const string ONLINE_SETTING_EVENT_ELEMENT_NEW_SCALE = "event_popup_element_new_scale";

		private Color32 _timerBackgroundColor = GlobalColors.EVENTS_POPUP_CATEGORY_DEFAULT_TIMER_COLOR;

		private Color32 _timerContentColor = GlobalColors.EVENTS_POPUP_CATEGORY_DEFAULT_TIMER_CONTENT_COLOR;

		private static Vector3 SPECIAL_POWER_BOARD_POSITION_OFFSET = new Vector3(-3f, 5f, 0f);

		private static Vector3 SPECIAL_POWER_BOARD_ROTATION = new Vector3(17.5f, 124f, -52.6f);

		private static Vector3 SPECIAL_POWER_BOARD_SCALE = new Vector3(8f, 8f, 8f);

		private static Vector3 LIMITED_CHAR_POSITION_OFFSET = new Vector3(0f, -37f, 0f);

		private static Vector3 LIMITED_CHAR_ROTATION = new Vector3(13f, -157f, 3f);

		private static Vector3 LIMITED_CHAR_SCALE = new Vector3(8f, 8f, 8f);

		private static Vector3 CHARACTER_DAY_MODEL_POSITION_OFFSET = new Vector3(-2f, -37.5f, 0f);

		private static Vector3 CHARACTER_DAY_MODEL_ROTATION = new Vector3(13f, -157f, 3f);

		private static Vector3 CHARACTER_DAY_MODEL_SCALE = new Vector3(8f, 8f, 8f);

		private static Vector3 MYSTERY_POWERUP_POSITION_OFFSET = new Vector3(0f, -35f, 0f);

		private static Vector3 MYSTERY_POWERUP_ROTATION = new Vector3(10f, 186f, 0f);

		private static Vector3 MYSTERY_POWERUP_SCALE = new Vector3(4.5f, 4.5f, 4.5f);

		private GameObject _specialPowerBoard;

		private GameObject _limitedCharacter;

		private GameObject _mysteryPowerup;

		[SerializeField]
		private UISprite _bgFill;

		[SerializeField]
		private UISprite _bgOutline;

		[SerializeField]
		private UISprite _closeButton;

		[SerializeField]
		private UIWidget _topAlign;

		[SerializeField]
		private UIWidget _bottomAlign;

		[SerializeField]
		private UILabel _eventTitleLine1;

		[SerializeField]
		private UILabel _eventTitleLine2;

		[SerializeField]
		private UILabel _checkItOutLabel;

		[SerializeField]
		private UILabel _messageTitleLabel;

		[SerializeField]
		private UILabel _messageSecondTitleLabel;

		[SerializeField]
		private GameObject _twoLinesDescription;

		[SerializeField]
		private Layout _twoLinesLayout;

		[SerializeField]
		private GameObject _jackpotBottomElements;

		[SerializeField]
		private GameObject _jackpotTopElements;

		[SerializeField]
		private GameObject _jackpotTopElementsHearts;

		[SerializeField]
		private GameObject _jackpotTopElementsNormal;

		[SerializeField]
		private GameObject _goToItemButton;

		[SerializeField]
		private UILabel _jackpotsDescriptionLabel;

		[SerializeField]
		private UILabel _MBJackpotLabel;

		[SerializeField]
		private UILabel _SMBJackpotLabel;

		[SerializeField]
		private UISprite _coinLeft;

		[SerializeField]
		private UISprite _coinRight;

		[SerializeField]
		private UISprite _bottomDescriptionBoxSprite;

		[SerializeField]
		private UISprite _topSplatSprite;

		[SerializeField]
		private UISprite _glowEffectSprite;

		[SerializeField]
		private List<UISprite> _listBgStripes = new List<UISprite>();

		[SerializeField]
		private GameObject _mysteryPowerupPrefab;

		[SerializeField]
		private TimerHelper _timerPrefab;

		[SerializeField]
		private UIWidget _timerContainer;

		private TimerHelper _timer;

		private const float COIN_SPRITE_OFFSET = 2f;

		private IEventManager _activeEventManager;

		private const int DIMENSIONS_BG_FILL_NORMAL_EVENT = 213;

		private const int DIMENSIONS_BG_FILL_CHECK_OUT_ITEM_EVENT = 280;

		private const int DIMENSIONS_BG_FILL_EXTENDED_CHECK_OUT_ITEM_EVENT = 305;

		private const int DESCRIPTION_BOX_NORMAL_HEIGHT = 80;

		private const int DESCRIPTION_BOX_EXTENDED_HEIGHT = 105;

		private Characters.CharacterId? _cachedCharacterID;

		private int _cachedCharacterOutfitIndex = -1;

		private Boards.BoardId? _cachedBoardID;

		private List<SSEventCategory> _supportedCategories = new List<SSEventCategory>
		{
			SSEventCategory.None,
			SSEventCategory.Cat1
		};

		private List<Characters.CharacterId> _specialItemUnsupportedCharacters = new List<Characters.CharacterId>
		{
			Characters.CharacterId.monacoCharacter01,
			Characters.CharacterId.transylvaniaCharacter01
		};

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		private bool IsSpecialItemAllowed(Characters.CharacterId characterID)
		{
			return !_specialItemUnsupportedCharacters.Contains(characterID);
		}

		protected override void Init()
		{
			InitializeTimer();
		}

		protected override void Show()
		{
			UIModelController.Instance.ActivateModelScreen(UIModelController.ModelScreen.TutorialPopup);
			_activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			if (_activeEventManager != null)
			{
				SSEventType eventType = _activeEventManager.GetEventType();
				if (_specialPowerBoard != null)
				{
					_specialPowerBoard.SetActive(eventType == SSEventType.SpecialPowerBoard || eventType == SSEventType.ExpiredLimitedBoard);
				}
				if (_limitedCharacter != null)
				{
					_limitedCharacter.SetActive(eventType == SSEventType.ExpiredLimitedCharacter || eventType == SSEventType.CharacterDay);
				}
				if (_mysteryPowerup != null)
				{
					_mysteryPowerup.SetActive(eventType == SSEventType.MysteryPowerup);
				}
			}
			SSEventsManagerController.OnSSEventFinishedHandler += HidePopupIfNeeded;
			UIScreenController.OnApplicationResumed += HidePopupIfNeeded;
			OnlineSettings.instance.AddOnFinishedDownloadingOnlineSettingsHandler(HidePopupIfNeeded);
			UISprite topSplatSprite = _topSplatSprite;
			topSplatSprite.onChange = (UIWidget.OnDimensionsChanged)Delegate.Combine(topSplatSprite.onChange, new UIWidget.OnDimensionsChanged(HandleSplatPositionChange));
			UpdateUIElements();
			SetLabelsTexts();
			PresentCurrentEventAvailable();
			StartTimer();
		}

		protected override void Hide()
		{
			UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.TutorialPopup);
			if (_specialPowerBoard != null)
			{
				_specialPowerBoard.SetActive(false);
			}
			if (_limitedCharacter != null)
			{
				_limitedCharacter.SetActive(false);
			}
			if (_mysteryPowerup != null)
			{
				_mysteryPowerup.SetActive(false);
			}
			SSEventsManagerController.OnSSEventFinishedHandler -= HidePopupIfNeeded;
			UIScreenController.OnApplicationResumed -= HidePopupIfNeeded;
			OnlineSettings.instance.RemoveOnFinishedDownloadingOnlineSettingsHandler(HidePopupIfNeeded);
			UISprite topSplatSprite = _topSplatSprite;
			topSplatSprite.onChange = (UIWidget.OnDimensionsChanged)Delegate.Remove(topSplatSprite.onChange, new UIWidget.OnDimensionsChanged(HandleSplatPositionChange));
			StopTimer();
		}

		private void InitializeTimer()
		{
			_timer = UnityEngine.Object.Instantiate(_timerPrefab, _timerContainer.transform);
		}

		private void UpdateUIElements()
		{
			if (_activeEventManager == null)
			{
				return;
			}
			SSEventType eventType = _activeEventManager.GetEventType();
			_bottomDescriptionBoxSprite.color = GlobalColors.EVENTS_POPUP_DESCRIPTION_BOX_NORMAL_COLOR;
			_topSplatSprite.color = GlobalColors.EVENTS_POPUP_SPLASH_COLOR;
			_glowEffectSprite.color = GlobalColors.EVENTS_POPUP_GLOW_DEFAULT_COLOR;
			switch (eventType)
			{
			case SSEventType.BiggerJackpot:
				_goToItemButton.SetActive(true);
				_bgFill.height = 305;
				_bottomDescriptionBoxSprite.height = 105;
				_bottomDescriptionBoxSprite.color = GlobalColors.EVENTS_POPUP_DESCRIPTION_BOX_JACKPOT_EVENT_COLOR;
				_topSplatSprite.color = GlobalColors.EVENTS_POPUP_DESCRIPTION_BOX_NORMAL_COLOR;
				_glowEffectSprite.color = GlobalColors.EVENTS_POPUP_GLOW_JACKPOT_COLOR;
				break;
			case SSEventType.SpecialPowerBoard:
			case SSEventType.ExpiredLimitedCharacter:
			case SSEventType.ExpiredLimitedBoard:
			case SSEventType.CharacterDay:
				_goToItemButton.SetActive(true);
				_bgFill.height = 280;
				_bottomDescriptionBoxSprite.height = 80;
				break;
			default:
				_goToItemButton.SetActive(false);
				_bgFill.height = 213;
				_bottomDescriptionBoxSprite.height = 80;
				break;
			}
			UISprite[] componentsInChildren = _bottomDescriptionBoxSprite.GetComponentsInChildren<UISprite>();
			UISprite[] array = componentsInChildren;
			foreach (UISprite uISprite in array)
			{
				uISprite.UpdateAnchors();
			}
			_bgFill.UpdateAnchors();
			_bgOutline.UpdateAnchors();
			_topAlign.UpdateAnchors();
			_bottomAlign.UpdateAnchors();
			_closeButton.UpdateAnchors();
			_timerContainer.UpdateAnchors();
			_eventTitleLine1.color = ColorData.instance.midnight;
			_eventTitleLine2.color = GlobalColors.EVENTS_POPUP_CATEGORY_DEFAULT_TITLE_LINE_2_COLOR;
			_timerBackgroundColor = GlobalColors.EVENTS_POPUP_CATEGORY_DEFAULT_TIMER_COLOR;
			foreach (UISprite listBgStripe in _listBgStripes)
			{
				listBgStripe.color = ((eventType != SSEventType.BiggerJackpot) ? GlobalColors.EVENTS_POPUP_STRIPES_NORMAL : GlobalColors.EVENTS_POPUP_STRIPES_JACKPOT);
			}
		}

		private void SetCategorySpecificElements()
		{
			SSEventCategory eventCategory = GetEventCategory();
			if (eventCategory == SSEventCategory.Cat1)
			{
				_eventTitleLine1.color = GlobalColors.EVENTS_POPUP_CATEGORY_VALENTINE_TITLE_LINE_1_COLOR;
				_eventTitleLine2.color = GlobalColors.EVENTS_POPUP_CATEGORY_VALENTINE_TITLE_LINE_2_COLOR;
				_timerBackgroundColor = GlobalColors.EVENTS_POPUP_CATEGORY_VALENTINE_TIMER_COLOR;
				_topSplatSprite.color = GlobalColors.EVENTS_POPUP_CATEGORY_VALENTINE_SPLASH_COLOR;
				_eventTitleLine1.text = Strings.Get("EVENTS_POPUP_CATEGORY_VALENTINE_TITLE");
			}
			else
			{
				_eventTitleLine1.color = ColorData.instance.midnight;
				_eventTitleLine2.color = GlobalColors.EVENTS_POPUP_CATEGORY_DEFAULT_TITLE_LINE_2_COLOR;
				_timerBackgroundColor = GlobalColors.EVENTS_POPUP_CATEGORY_DEFAULT_TIMER_COLOR;
			}
			_jackpotTopElementsNormal.SetActive(eventCategory == SSEventCategory.None);
			_jackpotTopElementsHearts.SetActive(eventCategory == SSEventCategory.Cat1);
		}

		private SSEventCategory GetEventCategory()
		{
			SSEventCategory sSEventCategory = SSEventCategory.None;
			string valueString;
			if (Globals.DEBUG_USE_EVENT_TESTER)
			{
				sSEventCategory = EventTesterManager.Instance.GetEventCategory();
			}
			else if (OnlineSettings.instance.TryGetValue("event_category", out valueString))
			{
				try
				{
					SSEventCategory sSEventCategory2 = (SSEventCategory)Enum.Parse(typeof(SSEventCategory), valueString);
					sSEventCategory = sSEventCategory2;
				}
				catch (Exception)
				{
				}
			}
			if (!_supportedCategories.Contains(sSEventCategory))
			{
				sSEventCategory = SSEventCategory.None;
			}
			return sSEventCategory;
		}

		private void SetLabelsTexts()
		{
			if (_activeEventManager != null)
			{
				string firstLine = string.Empty;
				string secondLine = string.Empty;
				string secondTitle = string.Empty;
				string firstTitle = string.Empty;
				if (_activeEventManager != null)
				{
					_activeEventManager.GetPopupTitle(out firstLine, out secondLine);
					_activeEventManager.GetPopupDescription(out firstTitle, out secondTitle);
				}
				_eventTitleLine1.text = firstLine;
				_eventTitleLine2.text = secondLine;
				if (_activeEventManager.GetEventType() == SSEventType.BiggerJackpot)
				{
					_twoLinesDescription.SetActive(false);
					_jackpotBottomElements.SetActive(true);
					_jackpotTopElements.SetActive(true);
					_jackpotsDescriptionLabel.text = firstTitle;
					_MBJackpotLabel.text = string.Format("{0:#,###0}", MysteryBox.GetMegaJackpotAmount(MysteryBox.Type.Normal));
					_SMBJackpotLabel.text = string.Format("{0:#,###0}", MysteryBox.GetMegaJackpotAmount(MysteryBox.Type.Super));
				}
				else
				{
					_jackpotBottomElements.SetActive(false);
					_jackpotTopElements.SetActive(false);
					_twoLinesDescription.SetActive(true);
					_messageTitleLabel.text = firstTitle;
					_messageSecondTitleLabel.text = secondTitle;
					_twoLinesLayout.Align();
				}
				_checkItOutLabel.text = Strings.Get("EVENTS_POPUP_CHECKOUT_BUTTON_TITLE");
			}
		}

		private void StartTimer()
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(SSEventsManagerController.instance.TimeLeftInSeconds());
			_timer.StartTimer(timeSpan);
			_timer.SetTimerColors(_timerContentColor, _timerBackgroundColor);
		}

		private void StopTimer()
		{
			_timer.StopTimer();
		}

		private void PresentCurrentEventAvailable()
		{
			if (_activeEventManager == null)
			{
				return;
			}
			SSEventType activeEventType = _activeEventManager.GetEventType();
			DisableAllStates();
			if (activeEventType != SSEventType.None)
			{
				EventTypeGameObjectPair eventTypeGameObjectPair = _stateGameObjects.Find((EventTypeGameObjectPair pair) => pair.eventType == activeEventType);
				if (eventTypeGameObjectPair != null)
				{
					eventTypeGameObjectPair.gameObject.SetActive(true);
				}
			}
			else
			{
				_messageTitleLabel.text = Strings.Get("EVENT_POPUP_NOT_AVAILABLE_DESCRIPTION");
				_messageSecondTitleLabel.text = string.Empty;
			}
			SetExtraSettingsForState(activeEventType);
		}

		private void HandleSplatPositionChange()
		{
			PresentCurrentEventAvailable();
		}

		private void SetExtraSettingsForState(SSEventType eventType)
		{
			switch (eventType)
			{
			case SSEventType.SpecialPowerBoard:
				AddBoard3DElement(eventType);
				break;
			case SSEventType.ExpiredLimitedCharacter:
			case SSEventType.CharacterDay:
				AddLimitedCharacter3DElements(eventType);
				break;
			case SSEventType.ExpiredLimitedBoard:
				AddBoard3DElement(eventType);
				break;
			case SSEventType.BiggerJackpot:
				PositionCoinIcons();
				SetCategorySpecificElements();
				break;
			case SSEventType.MysteryPowerup:
				PositionMysteryPowerup3DElements();
				break;
			case SSEventType.SuperMysteryBox:
				break;
			}
		}

		private void PositionMysteryPowerup3DElements()
		{
			if (_mysteryPowerup == null)
			{
				_mysteryPowerup = PlacePrefabUnderAnchor(_mysteryPowerupPrefab, UIModelController.Instance.TutorialPopupAnchor.transform, 31);
			}
			if (_mysteryPowerup != null)
			{
				Vector3 positionOffset = VerifySettingOnline("event_popup_element_new_position_offset", MYSTERY_POWERUP_POSITION_OFFSET);
				Vector3 newRotation = VerifySettingOnline("event_popup_element_new_rotation", MYSTERY_POWERUP_ROTATION);
				Vector3 newScale = VerifySettingOnline("event_popup_element_new_scale", MYSTERY_POWERUP_SCALE);
				PositionModelOnSprite(_topSplatSprite, _mysteryPowerup, positionOffset, newRotation, newScale);
			}
		}

		private void PositionCoinIcons()
		{
			_coinLeft.transform.localPosition = new Vector3(_MBJackpotLabel.transform.localPosition.x + 2f, _MBJackpotLabel.transform.localPosition.y, 0f);
			_coinRight.transform.localPosition = new Vector3(_SMBJackpotLabel.transform.localPosition.x + 2f, _SMBJackpotLabel.transform.localPosition.y, 0f);
		}

		private void AddBoard3DElement(SSEventType eventType)
		{
			bool found = false;
			Boards.BoardId? cachedBoardID = null;
			bool flag = false;
			if (eventType == SSEventType.ExpiredLimitedBoard || eventType == SSEventType.SpecialPowerBoard)
			{
				cachedBoardID = _activeEventManager.GetEventValue<Boards.BoardId>(out found);
				Boards.BoardId? cachedBoardID2 = _cachedBoardID;
				flag = cachedBoardID2.GetValueOrDefault() != cachedBoardID.GetValueOrDefault() || (cachedBoardID2.HasValue ^ cachedBoardID.HasValue);
			}
			if (!found || (!(_specialPowerBoard == null) && !flag))
			{
				return;
			}
			if (flag && _specialPowerBoard != null)
			{
				UnityEngine.Object.Destroy(_specialPowerBoard);
			}
			Boards.Board board = Boards.boardData[cachedBoardID.Value];
			_specialPowerBoard = HoverboardModelPreviewFactory.Instance.GetHoverboardModelPreview(board.boardModelName, cachedBoardID.Value);
			if (!(_specialPowerBoard == null))
			{
				GameObject gameObject = PlaceModelUnderAnchor(_specialPowerBoard, UIModelController.Instance.TutorialPopupAnchor.transform, 31).transform.GetChild(0).gameObject;
				if (gameObject != null)
				{
					Vector3 positionOffset = VerifySettingOnline("event_popup_element_new_position_offset", SPECIAL_POWER_BOARD_POSITION_OFFSET);
					Vector3 newRotation = VerifySettingOnline("event_popup_element_new_rotation", SPECIAL_POWER_BOARD_ROTATION);
					Vector3 newScale = VerifySettingOnline("event_popup_element_new_scale", SPECIAL_POWER_BOARD_SCALE);
					PositionModelOnSprite(_topSplatSprite, gameObject, positionOffset, newRotation, newScale);
				}
				_cachedBoardID = cachedBoardID;
			}
		}

		private void AddLimitedCharacter3DElements(SSEventType eventType)
		{
			bool found = false;
			Characters.CharacterId? eventCharacterId = null;
			if (_activeEventManager != null)
			{
				eventCharacterId = _activeEventManager.GetEventValue<Characters.CharacterId>(out found);
			}
			if (!found)
			{
				return;
			}
			try
			{
				Characters.CharacterId? cachedCharacterID = _cachedCharacterID;
				bool flag = cachedCharacterID.GetValueOrDefault() != eventCharacterId.GetValueOrDefault() || (cachedCharacterID.HasValue ^ eventCharacterId.HasValue);
				bool flag2 = !flag && _cachedCharacterOutfitIndex != CharacterManager.GetOutfitIndex(eventCharacterId.Value) && !OnlineSettings.instance.HasValue("event_theme");
				if (!(_limitedCharacter == null) && !flag && !flag2)
				{
					return;
				}
				if ((flag || flag2) && _limitedCharacter != null)
				{
					UnityEngine.Object.Destroy(_limitedCharacter);
				}
				Characters.Model model = Characters.characterData[eventCharacterId.Value];
				int outfitIndex = 0;
				if (OnlineSettings.instance.HasValue("event_theme"))
				{
					outfitIndex = OnlineSettings.instance.GetValue("event_theme", 0);
				}
				else if (CharacterManager.IsCharacterOwned(eventCharacterId.Value))
				{
					outfitIndex = CharacterManager.GetOutfitIndex(eventCharacterId.Value);
				}
				if (Outfits.GetThemeCount(eventCharacterId.Value) < outfitIndex)
				{
					outfitIndex = 0;
					SDebug.LogError("SSEventsPopupError: Attempted to use a non-existing theme for character!");
				}
				CharacterModelPreviewFactory.Instance.GetEventPopupModel(model.modelName, outfitIndex, IsSpecialItemAllowed(eventCharacterId.Value), (GameObject lC) =>
				{
					_limitedCharacter = lC;
					if (!(_limitedCharacter == null))
					{
						_cachedCharacterID = eventCharacterId;
						_cachedCharacterOutfitIndex = outfitIndex;
						GameObject gameObject = PlaceModelUnderAnchor(_limitedCharacter, UIModelController.Instance.TutorialPopupAnchor.transform, 31);
						if (gameObject != null)
						{
							Vector3 positionOffset = Vector3.zero;
							Vector3 newRotation = Vector3.zero;
							Vector3 newScale = Vector3.zero;
							if (eventType == SSEventType.ExpiredLimitedCharacter)
							{
								positionOffset = VerifySettingOnline("event_popup_element_new_position_offset", LIMITED_CHAR_POSITION_OFFSET);
								newRotation = VerifySettingOnline("event_popup_element_new_rotation", LIMITED_CHAR_ROTATION);
								newScale = VerifySettingOnline("event_popup_element_new_scale", LIMITED_CHAR_SCALE);
							}
							else if (eventType == SSEventType.CharacterDay)
							{
								positionOffset = VerifySettingOnline("event_popup_element_new_position_offset", CHARACTER_DAY_MODEL_POSITION_OFFSET);
								newRotation = VerifySettingOnline("event_popup_element_new_rotation", CHARACTER_DAY_MODEL_ROTATION);
								newScale = VerifySettingOnline("event_popup_element_new_scale", CHARACTER_DAY_MODEL_SCALE);
							}
							PositionModelOnSprite(_topSplatSprite, gameObject, positionOffset, newRotation, newScale);
						}
					}
				});
			}
			catch (ArgumentException ex)
			{
				SDebug.LogError("Limited character name is not accurate from Online Setting! : " + ex.StackTrace);
			}
		}

		private GameObject PlaceModelUnderAnchor(GameObject model, Transform anchor, int layer)
		{
			NGUITools.SetLayer(model, layer);
			model.transform.parent = anchor;
			model.transform.localPosition = Vector3.zero;
			model.transform.localEulerAngles = Vector3.zero;
			model.transform.localScale = Vector3.one;
			return model;
		}

		private GameObject PlacePrefabUnderAnchor(GameObject prefab, Transform anchor, int layer)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
			NGUITools.SetLayer(gameObject, layer);
			gameObject.transform.parent = anchor;
			gameObject.transform.localScale = Vector3.one;
			return gameObject;
		}

		private Vector3 VerifySettingOnline(string key, Vector3 defaultVal)
		{
			string valueString;
			if (OnlineSettings.instance.TryGetValue(key, out valueString))
			{
				string[] array = valueString.Split(',');
				if (array.Length == 3)
				{
					try
					{
						return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
					}
					catch (Exception)
					{
					}
				}
			}
			return defaultVal;
		}

		private void DisableAllStates()
		{
			foreach (EventTypeGameObjectPair stateGameObject in _stateGameObjects)
			{
				stateGameObject.gameObject.SetActive(false);
			}
		}

		public static void ShowIfNeeded()
		{
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			if (activeEventManager == null || !OnboardingSave.isTutorialCompleted)
			{
				return;
			}
			long lastTimeEventPopupWasForceShown = EventsSave.lastTimeEventPopupWasForceShown;
			long eventID = activeEventManager.GetEventID();
			if (eventID > 0 && (lastTimeEventPopupWasForceShown == 0 || eventID != lastTimeEventPopupWasForceShown))
			{
				if (activeEventManager.GetEventType() == SSEventType.CharacterName)
				{
					UIScreenController.Instance.QueuePopup("CharacterNameEventPopup");
				}
				else
				{
					UIScreenController.Instance.QueuePopup("EventsPopup");
				}
				EventsSave.lastTimeEventPopupWasForceShown = eventID;
				Flurry.LogEvent("Event Popup was shown");
			}
		}

		private void HidePopupIfNeeded()
		{
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			if (_activeEventManager == null || activeEventManager == null || activeEventManager.GetEventID() != _activeEventManager.GetEventID())
			{
				if (_limitedCharacter != null)
				{
					_limitedCharacter.SetActive(false);
				}
				if (_specialPowerBoard != null)
				{
					_specialPowerBoard.SetActive(false);
				}
				UIScreenController.Instance.CloseActivePopups(screenName);
			}
		}

		[UsedImplicitly]
		public void GoToEventItem()
		{
			if (_activeEventManager == null)
			{
				return;
			}
			switch (_activeEventManager.GetEventType())
			{
			case SSEventType.ExpiredLimitedCharacter:
			case SSEventType.CharacterDay:
				if (UIScreenController.IsInstanced)
				{
					if (_limitedCharacter != null)
					{
						_limitedCharacter.SetActive(false);
					}
					UIScreenController.Instance.ignoreLastPopupClosed = true;
					UIScreenController.Instance.doSaveAndDelayRemainingPopups = true;
					UIScreenController.Instance.CloseActivePopups(screenName);
					CharacterScreen.forceCenterOnEventCharacter = true;
					UIScreenController.Instance.PushScreen("CharacterScreen");
				}
				break;
			case SSEventType.SpecialPowerBoard:
			case SSEventType.ExpiredLimitedBoard:
				if (UIScreenController.IsInstanced)
				{
					if (_specialPowerBoard != null)
					{
						_specialPowerBoard.SetActive(false);
					}
					UIScreenController.Instance.ignoreLastPopupClosed = true;
					UIScreenController.Instance.doSaveAndDelayRemainingPopups = true;
					UIScreenController.Instance.CloseActivePopups(screenName);
					BoardScreenManager.Instance.forceCenterOnSpecificBoard = true;
					Boards.BoardId? eventBoardType = BoardManager.GetEventBoardType();
					if (eventBoardType.HasValue)
					{
						BoardScreenManager.Instance.forceSpecificBoard = eventBoardType.Value;
					}
					UIScreenController.Instance.PushScreen("BoardScreen");
				}
				break;
			case SSEventType.BiggerJackpot:
				if (UIScreenController.IsInstanced)
				{
					UIScreenController.Instance.ignoreLastPopupClosed = true;
					UIScreenController.Instance.doSaveAndDelayRemainingPopups = true;
					StoreScreen.ScrollToInterestPointOnNextShow(StoreInterestPoint.MysteryBox);
					UIScreenController.Instance.CloseActivePopups(screenName);
					UIScreenController.Instance.PushScreen("StoreScreen");
				}
				break;
			}
		}

		private void PositionModelOnSprite(UISprite tracker, GameObject objectToPosition, Vector3 positionOffset, Vector3 newRotation, Vector3 newScale)
		{
			StartCoroutine(DelayPositioning(tracker, objectToPosition, positionOffset, newRotation, newScale));
		}

		private IEnumerator DelayPositioning(UISprite tracker, GameObject objectToPosition, Vector3 positionOffset, Vector3 newRotation, Vector3 newScale)
		{
			yield return new WaitForEndOfFrame();
			Vector3 trackerPos = tracker.transform.position;
			Vector3 pixelPos = UIScreenController.Instance.Camera2DNormal.WorldToScreenPoint(trackerPos);
			pixelPos = new Vector3(pixelPos.x, pixelPos.y, 1f);
			Vector3 worldPos = UIScreenController.Instance.Camera3D.ScreenToWorldPoint(pixelPos);
			if (objectToPosition != null)
			{
				objectToPosition.transform.position = worldPos;
				objectToPosition.transform.localPosition = objectToPosition.transform.localPosition + positionOffset;
				objectToPosition.transform.localRotation = Quaternion.Euler(newRotation);
				objectToPosition.transform.localScale = newScale;
			}
		}
	}
}
