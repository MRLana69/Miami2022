using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Kiloo.Common.Extensions;
using SYBO.Subway.Info;
using UnityEngine;

namespace UI.Screens.CharacterScreenInternal
{
	public class CharacterListManager
	{
		private const string CHARACTER_LIST_MANAGER_DEBUG_LOG_TAG = "Character List manager: ";

		private readonly CharacterScrollListSlot[] _characterSlots;

		private readonly CharacterScreenManager _screenManager;

		private readonly CharacterScreen _characterScreen;

		public int size
		{
			get
			{
				return _characterSlots.Length;
			}
		}

		public bool allModelsLoaded { get; private set; }

		public CharacterScrollListSlot this[int index]
		{
			get
			{
				if (!_characterSlots.ValidIndex(index))
				{
					LogError("Character Slot index invalid. Index requested: " + index + ". Array size: " + _characterSlots.Length);
					return null;
				}
				return _characterSlots[index];
			}
			set
			{
				if (_characterSlots.ValidIndex(index))
				{
					_characterSlots[index] = value;
					return;
				}
				LogError("Character Slot index invalid. Index requested: " + index + ". Array size: " + _characterSlots.Length);
			}
		}

		public CharacterScrollListSlot this[Characters.CharacterId characterId]
		{
			get
			{
				return _characterSlots.FirstOrDefault((CharacterScrollListSlot slot) => slot.characterId == characterId);
			}
		}

		public CharacterListManager(int size, CharacterScreen characterScreen)
		{
			allModelsLoaded = false;
			_characterSlots = new CharacterScrollListSlot[size];
			_screenManager = CharacterScreenManager.Instance;
			_characterScreen = characterScreen;
		}

		public IEnumerator GetEnumerator()
		{
			return _characterSlots.GetEnumerator();
		}

		public void DisableAllSlots()
		{
			CharacterScrollListSlot[] characterSlots = _characterSlots;
			foreach (CharacterScrollListSlot characterScrollListSlot in characterSlots)
			{
				characterScrollListSlot.SetEnabledState(false, false);
			}
		}

		public void EnableVisibleSlots(int centerIndex)
		{
			CharacterScrollListSlot[] characterSlots = _characterSlots;
			foreach (CharacterScrollListSlot characterScrollListSlot in characterSlots)
			{
				bool isShowingPopup = UIScreenController.Instance.isShowingPopup;
				bool flag = IsWithinRange(centerIndex, characterScrollListSlot.index, _characterScreen.GetCharacterVisibilityCount());
				bool flag2 = IsWithinRange(centerIndex, characterScrollListSlot.index, _characterScreen.GetCharacterUIVisibilityCount());
				characterScrollListSlot.SetEnabledState(flag && !isShowingPopup, flag2 && !isShowingPopup);
			}
		}

		public void SetLoadedState(int slotIndex, bool shouldBeLoaded, int priority)
		{
			CharacterScrollListSlot characterScrollListSlot = _characterSlots[slotIndex];
			if (shouldBeLoaded)
			{
				if (characterScrollListSlot.model == null)
				{
					if (characterScrollListSlot.request == null)
					{
						characterScrollListSlot.request = new CharacterModelRequest(characterScrollListSlot.characterId, characterScrollListSlot.themeIndex, slotIndex, this);
					}
					characterScrollListSlot.request.SetPriority(priority);
				}
			}
			else
			{
				if (characterScrollListSlot.request != null)
				{
					characterScrollListSlot.request.CancelRequest();
					characterScrollListSlot.request = null;
				}
				characterScrollListSlot.DestroyModel();
			}
		}

		public void RefreshCharacter(Characters.CharacterId characterId, int themeIndex)
		{
			CharacterScrollListSlot characterScrollListSlot = this[characterId];
			int index = characterScrollListSlot.index;
			if (characterId != characterScrollListSlot.characterId || themeIndex != characterScrollListSlot.themeIndex)
			{
				if (characterScrollListSlot.request != null)
				{
					characterScrollListSlot.request.CancelRequest();
				}
				characterScrollListSlot.DestroyModel();
				characterScrollListSlot.request = new CharacterModelRequest(characterId, themeIndex, index, this);
			}
		}

		public void LoadAInitialModels()
		{
			CharacterScrollListSlot characterScrollListSlot = this[_characterScreen.initiallyCenteredCharacter];
			int centerIndex = _characterSlots.Length / 2;
			if (characterScrollListSlot != null)
			{
				centerIndex = characterScrollListSlot.index;
			}
			List<CharacterScrollListSlot> prioritizedPendingSlots = GetPrioritizedPendingSlots(centerIndex);
			LoadModelsSynchronously(prioritizedPendingSlots);
		}

		private List<CharacterScrollListSlot> GetPrioritizedPendingSlots(int centerIndex, bool fullList = false)
		{
			List<CharacterScrollListSlot> source = ((!fullList) ? _characterSlots.Where((CharacterScrollListSlot slot) => !slot.modelReady && IsWithinRange(centerIndex, slot.index, _characterScreen.GetCharacterVisibilityCount())).ToList() : _characterSlots.ToList());
			return source.OrderBy((CharacterScrollListSlot s) => Mathf.Abs(s.index - centerIndex)).ToList();
		}

		private void LoadModelsSynchronously(List<CharacterScrollListSlot> pendingSlots)
		{
			foreach (CharacterScrollListSlot pendingSlot in pendingSlots)
			{
				Characters.CharacterId characterId = pendingSlot.characterId;
				int lastSelectedThemeForCharacterType = _screenManager.GetLastSelectedThemeForCharacterType(characterId);
				pendingSlot.request = new CharacterModelRequest(pendingSlot.characterId, lastSelectedThemeForCharacterType, pendingSlot.index, this, true);
			}
			OnInitialModelLoadingComplete();
		}

		public void OnModelLoaded(Characters.CharacterId characterId, int themeIndex, int scrollIndex, GameObject characterModel, bool synchronous)
		{
			CharacterScrollListSlot characterScrollListSlot = this[scrollIndex];
			if (characterModel == null)
			{
				LogError(string.Concat("Error loading model. Model for character ", characterId, " is null."));
				characterScrollListSlot.request = null;
				return;
			}
			if (characterScrollListSlot == null)
			{
				LogError("Error loading model. The character slot is  null.");
				UnityEngine.Object.Destroy(characterModel);
				return;
			}
			if (characterScrollListSlot.characterId != characterId)
			{
				LogError(string.Concat("Error loading model. Mismatch of character type. Fetched: ", characterId, ". Slot: ", characterScrollListSlot.characterId));
				UnityEngine.Object.Destroy(characterModel);
				characterScrollListSlot.request = null;
				return;
			}
			characterModel.name = characterId.ToString();
			Transform transform = characterModel.transform;
			characterScrollListSlot.model = characterModel;
			bool flag = !UIScreenController.Instance.isShowingPopup;
			characterScrollListSlot.SetEnabledState(flag, flag);
			characterScrollListSlot.ui.Init(characterId, scrollIndex == _characterScreen.cachedCenteredIndex, transform);
			characterScrollListSlot.themeIndex = themeIndex;
			Utility.SetLayerRecursively(transform, 29);
			transform.parent = characterScrollListSlot.transform;
			float num = ((_screenManager.currentlyShownCharacter != characterId) ? 7f : 10f);
			transform.localScale = Vector3.one * num;
			transform.localPosition = _characterScreen.scrollCharacterPosition;
			transform.localEulerAngles = _characterScreen.scrollCharacterRotation;
			characterScrollListSlot.request = null;
			if (!synchronous)
			{
				_characterScreen.OnSingleModelLoaded(characterId);
			}
			_characterScreen.ScaleScrollModels(true, scrollIndex);
		}

		private void OnInitialModelLoadingComplete()
		{
			allModelsLoaded = true;
			_characterScreen.OnInitialModelLoadingComplete();
			UpdateSlotsState(_characterScreen.cachedCenteredIndex);
		}

		public void UpdateSlotsState(int centerIndex)
		{
			List<CharacterScrollListSlot> prioritizedPendingSlots = GetPrioritizedPendingSlots(centerIndex, true);
			int count = prioritizedPendingSlots.Count;
			for (int i = 0; i < count; i++)
			{
				CharacterScrollListSlot characterScrollListSlot = prioritizedPendingSlots[i];
				bool isShowingPopup = UIScreenController.Instance.isShowingPopup;
				bool flag = IsWithinRange(centerIndex, characterScrollListSlot.index, _characterScreen.GetCharacterVisibilityCount());
				bool flag2 = IsWithinRange(centerIndex, characterScrollListSlot.index, _characterScreen.GetCharacterUIVisibilityCount());
				characterScrollListSlot.SetEnabledState(flag && _characterScreen.AreCharactersEnabled() && !isShowingPopup, flag2 && !isShowingPopup);
				SetLoadedState(characterScrollListSlot.index, IsWithinRange(centerIndex, characterScrollListSlot.index, _characterScreen.GetCharacterVisibilityCount()), i);
			}
		}

		private bool IsWithinRange(int center, int position, int threshold)
		{
			return Math.Abs(position - center) <= threshold;
		}

		[Conditional("ENABLE_CHARACTER_LIST_MANAGER_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}

		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("Character List manager: " + msg, context);
		}
	}
}
