using System;
using System.Collections.Generic;
using System.Text;
using Data.Managers;
using Info;
using SYBO.Subway.Info;
using UI.Generic;
using UI.Popups.LimitedCharacterPopupInternal;
using UI.Screens;
using UnityEngine;

namespace UI.Popups
{
	public class LimitedCharacterPopup : UIPopup
	{
		[SerializeField]
		private GameObject _characterAnchor;

		[SerializeField]
		private UILabel _nameTitle;

		[SerializeField]
		private UILabel _nameSubTitle;

		[SerializeField]
		private UICharacterThemeShifter _characterThemeShifter;

		[SerializeField]
		private TimerHelper _timerPrefab;

		[SerializeField]
		private UIWidget _timerParent;

		private TimerHelper _timer;

		[SerializeField]
		private CharacterSelectButton _characterSelectButtonPrefab;

		[SerializeField]
		private UIWidget _characterSelectButtonParent;

		protected CharacterSelectButton _characterSelectButton;

		private List<string> _boughtThemeNames = new List<string>();

		private bool _characterWasBought;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			if (UIModelController.Instance.limitedCharacterPopupAnchor == null)
			{
				UIModelController.Instance.limitedCharacterPopupAnchor = _characterAnchor;
			}
			InitializeTimer();
			InitializeSelectButton();
		}

		protected override void Show()
		{
			LimitedCharacterPopupManager.Instance.currentThemeIndex = 0;
			UIModelController.Instance.ShowMenuModel(LimitedCharacterPopupManager.Instance.currentCharacter, LimitedCharacterPopupManager.Instance.currentThemeIndex, UIModelController.ModelScreen.LimitedCharacterPopup);
			UpdateUIElements();
			_characterThemeShifter.OnThemeButtonPressed += OnThemeButtonClicked;
			_characterThemeShifter.gameObject.SetActive(Outfits.HasCharacter(LimitedCharacterPopupManager.Instance.currentCharacter));
			CharacterScreenManager.onCharacterSelected += UpdateSelectedCharacter;
			CharacterScreenManager.characterUnlocked += OnCharacterUnlocked;
			UIScreenController.OnApplicationResumed += HidePopupIfNeeded;
			if (!CharacterStickerManager.HasCharacterBeenSeen(LimitedCharacterPopupManager.Instance.currentCharacter))
			{
				CharacterManager.MarkCharacterAsSeen(LimitedCharacterPopupManager.Instance.currentCharacter);
			}
			_characterWasBought = false;
			_boughtThemeNames.Clear();
		}

		protected override void Hide()
		{
			_characterThemeShifter.OnThemeButtonPressed -= OnThemeButtonClicked;
			CharacterScreenManager.onCharacterSelected -= UpdateSelectedCharacter;
			CharacterScreenManager.characterUnlocked -= OnCharacterUnlocked;
			UIScreenController.OnApplicationResumed -= HidePopupIfNeeded;
			UIModelController.Instance.ClearScreenAnchor(UIModelController.ModelScreen.LimitedCharacterPopup);
			UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.LimitedCharacterPopup);
			LogFlurryEvent();
		}

		public override void GainFocus()
		{
			UIModelController.Instance.ShowMenuModel(LimitedCharacterPopupManager.Instance.currentCharacter, LimitedCharacterPopupManager.Instance.currentThemeIndex, UIModelController.ModelScreen.LimitedCharacterPopup);
		}

		public override void LoseFocus()
		{
			UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.LimitedCharacterPopup);
		}

		private void LogFlurryEvent()
		{
			Characters.CharacterId currentCharacter = LimitedCharacterPopupManager.Instance.currentCharacter;
			string arg = Strings.GetDefault(Characters.characterData[currentCharacter].name).ToLower();
			StringBuilder stringBuilder = new StringBuilder(string.Format("Character {0} bought", arg));
			stringBuilder.Append(";");
			StringBuilder stringBuilder2 = new StringBuilder(GetStateString(_characterWasBought));
			stringBuilder2.Append(";");
			List<Outfit> list = Outfits.TryGetOutfitsForCharacter(currentCharacter);
			if (list != null)
			{
				string[] array = new string[list.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = GetThemeNameForAnalytics(list[i]);
					bool state = _boughtThemeNames.Contains(array[i]);
					stringBuilder.AppendFormat("{0}_{1} bought", arg, array[i]);
					stringBuilder.Append(";");
					stringBuilder2.AppendFormat("{0};", GetStateString(state));
				}
			}
			Flurry.LogEventWithSeveralParameters("Limited character popup closed", stringBuilder.ToString(), stringBuilder2.ToString());
		}

		private string GetThemeNameForAnalytics(Outfit theme)
		{
			return Strings.GetDefault(theme.title);
		}

		private string GetStateString(bool state)
		{
			return (!state) ? "no" : "yes";
		}

		private void OnThemeButtonClicked(int index)
		{
			if (LimitedCharacterPopupManager.Instance.currentThemeIndex != index)
			{
				LimitedCharacterPopupManager.Instance.currentThemeIndex = index;
				UIModelController.Instance.ShowMenuModel(LimitedCharacterPopupManager.Instance.currentCharacter, index, UIModelController.ModelScreen.LimitedCharacterPopup);
				UpdateUIElements();
			}
		}

		private void HidePopupIfNeeded()
		{
			if (!LimitedCharacterPopupManager.Instance.IsCharacterAvailable())
			{
				UIScreenController.Instance.CloseActivePopups("LimitedCharacterPopup");
			}
		}

		private void InitializeTimer()
		{
			_timer = UnityEngine.Object.Instantiate(_timerPrefab, _timerParent.transform);
			_timer.SetTimerColors(GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR, GlobalColorsGenerated.DISCOUNT_STICKER_COLOR);
		}

		private void InitializeSelectButton()
		{
			_characterSelectButton = UnityEngine.Object.Instantiate(_characterSelectButtonPrefab, _characterSelectButtonParent.transform);
			_characterSelectButton.transform.localPosition = Vector3.zero;
			_characterSelectButton.GetComponent<UIWidget>().width = _characterSelectButtonParent.width;
			_characterThemeShifter.InitValues(LimitedCharacterPopupManager.Instance.currentCharacter, LimitedCharacterPopupManager.Instance.currentThemeIndex);
			_characterSelectButton.SetSticker(CharacterSelectButton.StickerType.Limited);
			_characterSelectButton.InitButton(CharacterSelectButton.ButtonContext.LimitedCharacterPopup);
		}

		private void UpdateThemeShifter(Characters.CharacterId charId, int index)
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

		private void UpdateUIElements()
		{
			Characters.CharacterId currentlyShownModel = UIModelController.Instance.currentlyShownModel;
			Characters.Model model = Characters.characterData[currentlyShownModel];
			bool flag = CharacterManager.IsCharacterOwned(currentlyShownModel);
			Outfit outfitForCharacter = Outfits.GetOutfitForCharacter(currentlyShownModel, LimitedCharacterPopupManager.Instance.currentThemeIndex);
			if (model.hasOnlineSettings)
			{
				if (!flag)
				{
					_characterSelectButton.SetSticker(CharacterSelectButton.StickerType.Limited);
					TimeSpan timeSpan = CharacterManager.CharacterExpirationTimespan(currentlyShownModel);
					_timer.gameObject.SetActive(true);
					_timer.StartTimer(timeSpan);
				}
				else
				{
					_timer.StopTimer();
					_timer.gameObject.SetActive(false);
					_characterSelectButton.SetSticker(CharacterSelectButton.StickerType.None);
				}
			}
			_nameTitle.text = Strings.Get(model.name);
			_nameTitle.color = ColorData.instance.midnight;
			_nameSubTitle.text = (string.IsNullOrEmpty(Strings.Get(model.characterLimitedDescription)) ? string.Empty : Strings.Get(model.characterLimitedDescription));
			_nameSubTitle.color = ColorData.instance.peacock;
			if (outfitForCharacter != null)
			{
				Outfit outfit = outfitForCharacter;
				string text = Strings.Get(outfit.title);
				if (!string.IsNullOrEmpty(text))
				{
					_nameSubTitle.text = text;
				}
			}
			UpdateThemeShifter(LimitedCharacterPopupManager.Instance.currentCharacter, LimitedCharacterPopupManager.Instance.currentThemeIndex);
			UpdateCharacterSticker(LimitedCharacterPopupManager.Instance.currentCharacter, LimitedCharacterPopupManager.Instance.currentThemeIndex);
			_characterSelectButton.ReloadButton();
		}

		private void UpdateSelectedCharacter()
		{
			Characters.CharacterId currentlyShownModel = UIModelController.Instance.currentlyShownModel;
			int currentlyShownThemeIndex = UIModelController.Instance.currentlyShownThemeIndex;
			bool flag = CharacterManager.IsCharacterOwned(currentlyShownModel);
			bool flag2 = CharacterManager.IsOutfitUnlocked(currentlyShownModel, currentlyShownThemeIndex);
			_characterSelectButton.ReloadButton();
			if (flag && flag2)
			{
				CharacterManager.SetOutfitSelectionIndex(currentlyShownModel, currentlyShownThemeIndex);
				_characterThemeShifter.UpdateUIForCharacter(currentlyShownModel, currentlyShownThemeIndex);
			}
		}

		private void UpdateCharacterSticker(Characters.CharacterId charId, int index)
		{
			Characters.Model model = Characters.characterData[charId];
			Outfit outfitForCharacter = Outfits.GetOutfitForCharacter(charId, index);
			bool flag = CharacterManager.IsCharacterOwned(charId);
			bool flag2 = outfitForCharacter != null;
			bool flag3 = flag2 && outfitForCharacter.price == 0 && !CharacterStickerManager.IsThemeUnlockedForCharacter(charId, index);
			bool flag4 = !flag2 && model.hasOnlineSettings && !flag;
			bool flag5 = flag3 || flag4;
			bool flag6 = LimitedCharacterPopupManager.Instance.CurrentlyShownCharacterMatchesEventItem();
			if (flag5)
			{
				if (flag3)
				{
					_characterSelectButton.SetSticker(CharacterSelectButton.StickerType.Gift);
				}
				else if (flag6)
				{
					_characterSelectButton.SetSticker(CharacterSelectButton.StickerType.Event);
				}
			}
			else
			{
				_characterSelectButton.SetSticker(CharacterSelectButton.StickerType.None);
			}
		}

		private void OnCharacterUnlocked(Characters.CharacterId charId, int version)
		{
			LimitedCharacterPopupManager.Instance.currentThemeIndex = version;
			UpdateSelectedCharacter();
			UpdateUIElements();
			_characterWasBought = true;
			if (version > 0)
			{
				Outfit outfitForCharacter = Outfits.GetOutfitForCharacter(charId, version);
				if (outfitForCharacter != null)
				{
					_boughtThemeNames.Add(GetThemeNameForAnalytics(outfitForCharacter));
				}
			}
		}
	}
}
