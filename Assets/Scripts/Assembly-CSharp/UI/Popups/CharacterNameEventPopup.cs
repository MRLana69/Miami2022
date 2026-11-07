using System;
using System.Collections;
using System.Collections.Generic;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.JsonUtil;
using SYBO.Subway.Info;
using UI.Generic;
using UI.Screens;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Popups
{
	public class CharacterNameEventPopup : UIPopup
	{
		private enum HorizontalScrollDirections
		{
			Left = -1,
			Right = 1
		}

		[FormerlySerializedAs("characterScrollAnchor")]
		[SerializeField]
		private GameObject _characterScrollAnchor;

		[SerializeField]
		private GameObject _categoryModelAnchor;

		[FormerlySerializedAs("skipWithKeysButtonWidget")]
		[SerializeField]
		private GameObject _skipWithKeysButtonWidget;

		[FormerlySerializedAs("skipWithAdButtonWidget")]
		[SerializeField]
		private GameObject _skipWithAdButtonWidget;

		[FormerlySerializedAs("completeSignWidget")]
		[SerializeField]
		private GameObject _completeSignWidget;

		[FormerlySerializedAs("centerAnchor")]
		[SerializeField]
		private GameObject _centerAnchor;

		[FormerlySerializedAs("overlayClipAnchor")]
		[SerializeField]
		private GameObject _overlayClipAnchor;

		[FormerlySerializedAs("selectButtonPrefab")]
		[SerializeField]
		private GameObject _selectButtonPrefab;

		[FormerlySerializedAs("eventDescriptionBox")]
		[SerializeField]
		private GameObject _eventDescriptionBox;

		[FormerlySerializedAs("eventDescriptionBoxName")]
		[SerializeField]
		private UILabel _eventDescriptionBoxName;

		[FormerlySerializedAs("eventDescriptionBoxActive")]
		[SerializeField]
		private GameObject _eventDescriptionBoxActive;

		[FormerlySerializedAs("eventDescriptionBoxTitleInactive")]
		[SerializeField]
		private UILabel _eventDescriptionBoxTitleInactive;

		[FormerlySerializedAs("eventDescriptionTitleActive")]
		[SerializeField]
		private UILabel _eventDescriptionTitleActive;

		[FormerlySerializedAs("eventDescriptionBoxNameActive")]
		[SerializeField]
		private UILabel _eventDescriptionBoxNameActive;

		[FormerlySerializedAs("eventDescriptionBoxNameActiveShadow")]
		[SerializeField]
		private UILabel _eventDescriptionBoxNameActiveShadow;

		[FormerlySerializedAs("firstTitle")]
		[SerializeField]
		private UILabel _firstTitle;

		[FormerlySerializedAs("secondTitle")]
		[SerializeField]
		private UILabel _secondTitle;

		[FormerlySerializedAs("skipButtonKeyAmount")]
		[SerializeField]
		private UILabel _skipButtonKeyAmount;

		[SerializeField]
		private GameObject _lpModelPrefab;

		[SerializeField]
		private UISprite _topSplatSprite;

		[SerializeField]
		private GameObject _superMysteryBoxIcon;

		[SerializeField]
		private GameObject _movieIcon;

		[SerializeField]
		private GameObject _danceIcon;

		[SerializeField]
		private TimerHelper _timerPrefab;

		[SerializeField]
		private UIWidget _timerContainer;

		private TimerHelper _timer;

		private GameObject _lpModel;

		private List<KeyValuePair<Characters.CharacterId, CharacterScreenController>> _characterControllers = new List<KeyValuePair<Characters.CharacterId, CharacterScreenController>>();

		private int _scrollPopulationStartFromSlot;

		private int _currentlyFocusedIndex;

		private bool _isFirstFocused;

		private bool _isLastFocused;

		private CharacterNameEventManager _characterNameEventManager;

		private CharacterSelectButton _selectButton;

		private BoxCollider _skipWithKeysButtonCollider;

		private BoxCollider _skipWithAdButtonCollider;

		private const float DEFAULT_SCALE_FACTOR = 9f;

		private const float FOCUSED_SCALE_FACTOR = 12f;

		private const float DEFAULT_Y_POSITION_ON_SCROLL = 0f;

		private const float FOCUSED_Y_POSITION_ON_SCROLL = -9f;

		private const float DEFAULT_FREEZE_VALUE = 1f;

		private const float FOCUSED_FREEZE_VALUE = 0f;

		private const float ANIMATION_SPEED_FACTOR = 2f;

		private const float SPACING_BETWEEN_CHARACTERS = 76f;

		private const string ONLINESETTINGS_ALLOW_SKIP_WITH_ADS_KEY = "character_name_event_skip_with_ads";

		private const bool DEFAULT_ALLOW_SKIP_WITH_ADS = true;

		private const string ONLINE_SETTING_EVENT_ELEMENT_NEW_POSITION_OFFSET = "event_popup_element_new_position_offset";

		private const string ONLINE_SETTING_EVENT_ELEMENT_NEW_ROTATION = "event_popup_element_new_rotation";

		private const string ONLINE_SETTING_EVENT_ELEMENT_NEW_SCALE = "event_popup_element_new_scale";

		private readonly Vector3 _lpPositionOffset = new Vector3(0f, 4f, 0f);

		private readonly Vector3 _lpRotation = new Vector3(0f, 210f, 17f);

		private readonly Vector3 _lpScale = new Vector3(4.8f, 4.8f, 4.8f);

		private readonly Color32 _timerBackgroundColor = GlobalColors.EVENTS_POPUP_CATEGORY_DEFAULT_TIMER_COLOR;

		private readonly Color32 _timerContentColor = GlobalColors.EVENTS_POPUP_CATEGORY_DEFAULT_TIMER_CONTENT_COLOR;

		private bool isAllowedToSkipWithAds
		{
			get
			{
				return OnlineSettings.instance.GetValue("character_name_event_skip_with_ads", true);
			}
		}

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			InitializeSelectButton();
			InitializeTimer();
			_skipWithKeysButtonCollider = _skipWithKeysButtonWidget.GetComponent<BoxCollider>();
			_skipWithAdButtonCollider = _skipWithAdButtonWidget.GetComponent<BoxCollider>();
			if (_skipWithAdButtonCollider == null || _skipWithKeysButtonCollider == null)
			{
				SDebug.LogError("Failed to find colliders on skip buttons");
			}
		}

		public override void GainFocus()
		{
			_overlayClipAnchor.SetActive(true);
			_characterScrollAnchor.SetActive(true);
			_categoryModelAnchor.SetActive(true);
			ClearScrollListOfCharacters();
			InitScrollWithCharacterModels();
			EnableUIElementsForList();
			InitCategorySpecificElements();
		}

		public override void LoseFocus()
		{
			_overlayClipAnchor.SetActive(false);
			_characterScrollAnchor.SetActive(false);
			_categoryModelAnchor.SetActive(false);
		}

		private void SetActiveCharacterNameEventManager()
		{
			_characterNameEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
		}

		private void InitializeSelectButton()
		{
			GameObject gameObject = _centerAnchor.AddChild(_selectButtonPrefab);
			_selectButton = gameObject.GetComponent<CharacterSelectButton>();
			if (_selectButton != null)
			{
				Utility.SetLayerRecursively(_selectButton.transform, 30);
				_selectButton.transform.localPosition = new Vector3(0f, -173f, 0f);
				_selectButton.SetSticker(CharacterSelectButton.StickerType.Event);
				_selectButton.InitButton(CharacterSelectButton.ButtonContext.CharacterNameEventPopup);
			}
			else
			{
				SDebug.LogError("Failed to initialized select button");
			}
		}

		private void InitializeTimer()
		{
			_timer = UnityEngine.Object.Instantiate(_timerPrefab, _timerContainer.transform);
		}

		private void ReloadSelectButton(Characters.CharacterId charId, int themeIndex)
		{
			RefreshDescriptionBox();
			if (_characterNameEventManager != null)
			{
				UpdateSkipButtonState();
			}
			_selectButton.ReloadButton();
		}

		private void UpdateSkipButtonState()
		{
			bool flag = AdSystemManager.IsCached(AdLocations.WORDY_WEEKEND_VIDEOS);
			bool flag2 = isAllowedToSkipWithAds && flag;
			bool flag3 = _characterNameEventManager.IsAbleToSkip();
			_skipWithKeysButtonWidget.SetActive(flag3 && !flag2);
			_skipWithAdButtonWidget.SetActive(flag3 && flag2);
			if (flag3 && isAllowedToSkipWithAds)
			{
				AdTrackingSystemLogger.LogAttemptToShowButtonFlow(AdLocations.WORDY_WEEKEND_VIDEOS);
			}
		}

		private void OnSelectCharacter()
		{
			RefreshDescriptionBox();
			_selectButton.ReloadButton();
		}

		protected override void Show()
		{
			SetActiveCharacterNameEventManager();
			SSEventsManagerController.OnSSEventFinishedHandler += HidePopupIfNeeded;
			CharacterScreenManager.characterUnlocked += ReloadSelectButton;
			CharacterScreenManager.onCharacterSelected += OnSelectCharacter;
			UIScreenController.Instance.RecalculateViewportRectForClipCamera(UIScreenController.Instance.Camera3DClip, true);
			UIScreenController.Instance.SetCameraToOrtographic(UIScreenController.Instance.Camera3DClip, false);
			UIScreenController.Instance.CalculateCameraFieldOfView(UIScreenController.Instance.Camera3DClip, 640f, 450f);
			UIScreenController.OnApplicationResumed += HidePopupIfNeeded;
			ClearScrollListOfCharacters();
			InitScrollWithCharacterModels();
			EnableUIElementsForList();
			InitCategorySpecificElements();
			string firstLine = string.Empty;
			string secondLine = string.Empty;
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.CharacterName);
			if (activeEventManager != null)
			{
				activeEventManager.GetPopupTitle(out firstLine, out secondLine);
			}
			_firstTitle.text = firstLine;
			_firstTitle.color = ColorData.instance.midnight;
			_secondTitle.text = secondLine;
			RefreshDescriptionBox();
			SetSkipButtonCollidersEnabled(true);
			_selectButton.ReloadButton(true, _characterControllers[_currentlyFocusedIndex].Key);
			StartTimer();
		}

		private void InitCategorySpecificElements()
		{
			if (_characterNameEventManager != null)
			{
				SSEventCategory sSEventCategory = _characterNameEventManager.GetEventCategory();
				switch (sSEventCategory)
				{
				case SSEventCategory.Cat1:
					CreateAndPositionLP3DElements();
					break;
				default:
					sSEventCategory = SSEventCategory.None;
					break;
				case SSEventCategory.None:
				case SSEventCategory.Cat2:
				case SSEventCategory.Cat3:
					break;
				}
				SetActiveIfExists(_lpModel, sSEventCategory == SSEventCategory.Cat1);
				_movieIcon.SetActive(sSEventCategory == SSEventCategory.Cat2);
				_danceIcon.SetActive(sSEventCategory == SSEventCategory.Cat3);
				_superMysteryBoxIcon.SetActive(sSEventCategory == SSEventCategory.None);
			}
		}

		private void SetActiveIfExists(GameObject go, bool state)
		{
			if (go != null)
			{
				go.SetActive(state);
			}
		}

		private void SetSkipButtonCollidersEnabled(bool state)
		{
			if (_skipWithAdButtonCollider != null)
			{
				_skipWithAdButtonCollider.enabled = state;
			}
			if (_skipWithKeysButtonCollider != null)
			{
				_skipWithKeysButtonCollider.enabled = state;
			}
		}

		protected override void Hide()
		{
			SSEventsManagerController.OnSSEventFinishedHandler -= HidePopupIfNeeded;
			CharacterScreenManager.characterUnlocked -= ReloadSelectButton;
			CharacterScreenManager.onCharacterSelected -= OnSelectCharacter;
			ClearScrollListOfCharacters();
			UIScreenController.Instance.RecalculateViewportRectForClipCamera(UIScreenController.Instance.Camera3DClip, false);
			UIScreenController.Instance.SetCameraToOrtographic(UIScreenController.Instance.Camera3DClip, true);
			UIScreenController.OnApplicationResumed -= HidePopupIfNeeded;
			StopTimer();
		}

		private void ClearScrollListOfCharacters()
		{
			foreach (KeyValuePair<Characters.CharacterId, CharacterScreenController> characterController in _characterControllers)
			{
				if (characterController.Value != null && characterController.Value.gameObject != null)
				{
					UnityEngine.Object.Destroy(characterController.Value.gameObject);
				}
			}
			_characterControllers.Clear();
		}

		private void InitScrollWithCharacterModels()
		{
			_characterScrollAnchor.transform.localPosition = new Vector3(-76f, _characterScrollAnchor.transform.localPosition.y, _characterScrollAnchor.transform.localPosition.z);
			if (_characterNameEventManager != null)
			{
				List<Characters.CharacterId> charactersToDisplay = _characterNameEventManager.GetCharactersToDisplay();
				_isFirstFocused = _characterNameEventManager.IsShowingFirstElement();
				_isLastFocused = _characterNameEventManager.IsShowingLastElement();
				bool flag = _characterNameEventManager.IsFirstCharacterSelected();
				if (_characterNameEventManager.HasCompletedCollectionForSelectedCharacter())
				{
					_currentlyFocusedIndex = ((!flag) ? 1 : 0);
					_scrollPopulationStartFromSlot = (flag ? 1 : 0);
					Invoke("ScrollToNextCharacter", 0.3f);
				}
				else
				{
					_currentlyFocusedIndex = ((!_isFirstFocused) ? 1 : 0);
					_scrollPopulationStartFromSlot = (_isFirstFocused ? 1 : 0);
				}
				for (int i = 0; i < charactersToDisplay.Count; i++)
				{
					InsertCharacterIntoModelsList(charactersToDisplay[i], _currentlyFocusedIndex);
				}
				Utility.SetLayerRecursively(_characterScrollAnchor.transform, 29);
			}
		}

		private void InsertCharacterIntoModelsList(Characters.CharacterId characterId, int focusedIndex)
		{
			if (_characterNameEventManager == null)
			{
				return;
			}
			int lastSelectedThemeForCharacterType = _characterNameEventManager.GetLastSelectedThemeForCharacterType(characterId);
			CharacterModelPreviewFactory.Instance.GetCharacterWordEventScreenModel(characterId.ToString(), lastSelectedThemeForCharacterType, (CharacterScreenController currentCharacterModelController) =>
			{
				if (currentCharacterModelController == null)
				{
					SDebug.LogError(string.Concat("CharacterName Event Popup: GetCharacterEventScreenModel for character ", characterId, " has failed. Creating default model."));
				}
				_characterControllers.Add(new KeyValuePair<Characters.CharacterId, CharacterScreenController>(characterId, currentCharacterModelController));
				if (currentCharacterModelController != null)
				{
					currentCharacterModelController.name = string.Format("{0:000}{1}", _characterControllers.Count, characterId.ToString());
					Transform transform = currentCharacterModelController.transform;
					transform.parent = _characterScrollAnchor.transform;
					transform.localPosition = new Vector3((float)_scrollPopulationStartFromSlot * 76f, 0f, 0f);
					transform.localEulerAngles = new Vector3(0f, 180f, 0f);
					if (_characterControllers.Count - 1 == focusedIndex)
					{
						transform.localScale = Vector3.one * 12f;
						Vector3 localPosition = transform.localPosition;
						localPosition.y = -9f;
						transform.localPosition = localPosition;
						currentCharacterModelController.Freeze = 0f;
					}
					else
					{
						transform.localScale = Vector3.one * 9f;
						Vector3 localPosition2 = transform.localPosition;
						localPosition2.y = 0f;
						transform.localPosition = localPosition2;
						currentCharacterModelController.Freeze = 1f;
					}
					_scrollPopulationStartFromSlot++;
					Utility.SetLayerRecursively(transform, 29);
				}
			});
		}

		private void CreateAndPositionLP3DElements()
		{
			if (_lpModel == null)
			{
				_lpModel = _categoryModelAnchor.AddChild(_lpModelPrefab);
				NGUITools.SetLayer(_lpModel, 31);
			}
			if (_lpModel != null)
			{
				Vector3 positionOffset = VerifySettingOnline("event_popup_element_new_position_offset", _lpPositionOffset);
				Vector3 newRotation = VerifySettingOnline("event_popup_element_new_rotation", _lpRotation);
				Vector3 newScale = VerifySettingOnline("event_popup_element_new_scale", _lpScale);
				PositionModelOnSprite(_topSplatSprite, _lpModel, positionOffset, newRotation, newScale);
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
			objectToPosition.transform.position = worldPos;
			objectToPosition.transform.localPosition = objectToPosition.transform.localPosition + positionOffset;
			objectToPosition.transform.localRotation = Quaternion.Euler(newRotation);
			objectToPosition.transform.localScale = newScale;
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

		private void EnableUIElementsForList()
		{
			if (_characterNameEventManager != null)
			{
				_completeSignWidget.SetActive(_characterNameEventManager.ShouldShowTheCompletedSign());
				UpdateSkipButtonState();
				_skipButtonKeyAmount.text = _characterNameEventManager.GetNumberOfKeysToSkip().ToString();
				_selectButton.ReloadButton(true, _characterControllers[_currentlyFocusedIndex].Key);
			}
		}

		private void RefreshDescriptionBox()
		{
			if (_characterNameEventManager != null)
			{
				string firstTitle;
				string secondTitle;
				_characterNameEventManager.GetPopupDescription(out firstTitle, out secondTitle);
				if (_characterNameEventManager.SelectedCharacterMatchesFocusedCharacter())
				{
					SetActiveCharacterBox(firstTitle, secondTitle);
				}
				else
				{
					SetInactiveCharacterBox(firstTitle, secondTitle);
				}
			}
		}

		private void SetInactiveCharacterBox(string descriptionLine1, string descriptionLine2)
		{
			_eventDescriptionBox.SetActive(true);
			_eventDescriptionBoxActive.SetActive(false);
			_eventDescriptionBoxTitleInactive.text = descriptionLine1;
			_eventDescriptionBoxName.text = descriptionLine2;
		}

		private void SetActiveCharacterBox(string descriptionLine1, string descriptionLine2)
		{
			_eventDescriptionBox.SetActive(false);
			_eventDescriptionBoxActive.SetActive(true);
			string text = NGUIText.EncodeColor(GlobalColors.CHARACTERWORDEVENT_POPUP_NAME_ACTIVE);
			string text2 = NGUIText.EncodeColor(GlobalColors.CHARACTERWORDEVENT_POPUP_NAME_INACTIVE);
			string text3 = NGUIText.EncodeColor(GlobalColors.CHARACTERWORDEVENT_POPUP_NAME_SHADOW_ACTIVE);
			string text4 = NGUIText.EncodeColor(GlobalColors.CHARACTERWORDEVENT_POPUP_NAME_SHADOW_INACTIVE);
			int num = 0;
			CharacterNameEventManager typeOfActiveEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
			if (typeOfActiveEventManager != null)
			{
				num = typeOfActiveEventManager.GetAmountOfCollectedLetters();
			}
			char[] array = descriptionLine2.ToCharArray();
			string text5 = string.Empty;
			string text6 = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				if (IsFilteredCharacter(array[i]))
				{
					num++;
				}
				else if (num > i)
				{
					text5 = text5 + "[" + text + "]";
					text6 = text6 + "[" + text3 + "]";
				}
				else
				{
					text5 = text5 + "[" + text2 + "]";
					text6 = text6 + "[" + text4 + "]";
				}
				text5 = text5 + array[i] + " ";
				text6 = text6 + array[i] + " ";
			}
			if (array.Length > 0)
			{
				_eventDescriptionBoxNameActive.text = text5.Substring(0, text5.Length - 1);
				_eventDescriptionBoxNameActiveShadow.text = text6.Substring(0, text6.Length - 1);
			}
			_eventDescriptionTitleActive.text = descriptionLine1;
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

		[UsedImplicitly]
		public void OnSkipClicked()
		{
			if (_characterNameEventManager != null)
			{
				if (_characterNameEventManager.CanAffordToSkip())
				{
					_characterNameEventManager.UseKeysToSkip();
					ScrollToNextCharacter();
				}
				else
				{
					PurchaseHandler.Instance.PurchaseKeysIfNeeded(_characterNameEventManager.GetNumberOfKeysToSkip());
				}
			}
		}

		[UsedImplicitly]
		public void OnSkipWithAdClicked()
		{
			if (_characterNameEventManager != null)
			{
				AdTrackingSystemLogger.LogVideoAdButtonClick(AdLocations.WORDY_WEEKEND_VIDEOS);
				AdSystemManager.Show(AdLocations.WORDY_WEEKEND_VIDEOS, OnSkipWithAdSuccessful);
			}
		}

		private void OnSkipWithAdSuccessful()
		{
			UIScreenController.Instance.HideVideoAdsOverlay(true);
			_characterNameEventManager.LogWatchedAdToSkip();
			ScrollToNextCharacter();
		}

		private void ScrollToNextCharacter()
		{
			if (_characterNameEventManager != null && !_isLastFocused)
			{
				_isFirstFocused = false;
				_characterNameEventManager.SkipCurrentCharacter();
				Characters.CharacterId? nextCharacterToDisplay = _characterNameEventManager.GetNextCharacterToDisplay();
				_currentlyFocusedIndex++;
				if (nextCharacterToDisplay.HasValue)
				{
					InsertCharacterIntoModelsList(nextCharacterToDisplay.Value, _currentlyFocusedIndex);
				}
				else
				{
					_isLastFocused = true;
				}
				ScrollListTo(_currentlyFocusedIndex);
				RefreshDescriptionBox();
			}
		}

		private void ScrollListTo(int currentCharacterIndex)
		{
			EnableUIElementsForList();
			if (currentCharacterIndex <= _characterControllers.Count)
			{
				StartCoroutine(AnimateScrollBar(HorizontalScrollDirections.Left, 76f, 2f));
				StartCoroutine(AnimateCharacter(_characterControllers[currentCharacterIndex - 1].Value, 12f, 9f, -9f, 0f, 0f, 1f, 2f));
				StartCoroutine(AnimateCharacter(_characterControllers[currentCharacterIndex].Value, 9f, 12f, 0f, -9f, 1f, 0f, 2f));
			}
		}

		private IEnumerator AnimateScrollBar(HorizontalScrollDirections direction, float amount, float speedFactor)
		{
			float animationLerpFactor = 0f;
			Transform characterScrollTrans = _characterScrollAnchor.transform;
			Vector3 characterScrollLocalPosition = characterScrollTrans.localPosition;
			float startValue = characterScrollLocalPosition.x;
			float endValue = startValue + amount * (float)direction;
			SetSkipButtonCollidersEnabled(false);
			while (animationLerpFactor < 1f)
			{
				animationLerpFactor += Time.deltaTime * speedFactor;
				float temporaryX = Mathf.Lerp(startValue, endValue, animationLerpFactor);
				characterScrollTrans.localPosition = new Vector3(temporaryX, characterScrollLocalPosition.y, characterScrollLocalPosition.z);
				yield return null;
			}
			SetSkipButtonCollidersEnabled(true);
		}

		private IEnumerator AnimateCharacter(CharacterScreenController characterController, float scaleStartValue, float scaleEndValue, float positionStartValue, float positionEndValue, float freezeStart, float freezeEnd, float speedFactor)
		{
			float animationLerpFactor = 0f;
			if (characterController != null)
			{
				Transform characterModelTrans = characterController.transform;
				Vector3 characterModelPosition = characterModelTrans.localPosition;
				while (animationLerpFactor < 1f)
				{
					animationLerpFactor += Time.deltaTime * speedFactor;
					characterController.Freeze = Mathf.Lerp(freezeStart, freezeEnd, animationLerpFactor);
					float newScale = Mathf.Lerp(scaleStartValue, scaleEndValue, animationLerpFactor);
					float newYPosition = Mathf.Lerp(positionStartValue, positionEndValue, animationLerpFactor);
					characterModelTrans.localScale = new Vector3(newScale, newScale, newScale);
					characterModelTrans.localPosition = new Vector3(characterModelPosition.x, newYPosition, characterModelPosition.z);
					yield return null;
				}
				characterController.Freeze = freezeEnd;
			}
		}

		private bool IsFilteredCharacter(char newChar)
		{
			char[] fILTERED_CHARACTERS = CharacterNameEventManager.FILTERED_CHARACTERS;
			foreach (char c in fILTERED_CHARACTERS)
			{
				if (newChar == c)
				{
					return true;
				}
			}
			return false;
		}

		public void HidePopupIfNeeded()
		{
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			if (_characterNameEventManager == null || activeEventManager == null || activeEventManager.GetEventID() != _characterNameEventManager.GetEventID())
			{
				UIScreenController.Instance.CloseActivePopups(screenName);
			}
		}
	}
}
