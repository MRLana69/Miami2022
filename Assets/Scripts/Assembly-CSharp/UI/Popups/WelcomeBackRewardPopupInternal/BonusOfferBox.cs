using UnityEngine;

namespace UI.Popups.WelcomeBackRewardPopupInternal
{
	public class BonusOfferBox : WelcomeBackRewardPopupBox
	{
		private const int BONUS_OFFER_AD_ITEM_INDEX = 0;

		[SerializeField]
		private GameObject _bonusOfferGo;

		[SerializeField]
		private UILabel _bonusOfferTitleLabel;

		[SerializeField]
		private UILabel _bonusOfferOrLabel;

		[SerializeField]
		private Transform[] _buttonParents;

		private WelcomeBackRewardItemButton[] _spawnedButtons;

		private WelcomeBackRewardItemPayload[] _buttonPayloads;

		private void Awake()
		{
			_spawnedButtons = new WelcomeBackRewardItemButton[_buttonParents.Length];
			CreateButtonPayloads();
			SpawnButtons();
		}

		private void CreateButtonPayloads()
		{
			WelcomeBackRewardItem randomBonusOfferItem = WelcomeBackRewardManager.GetRandomBonusOfferItem();
			_buttonPayloads = new WelcomeBackRewardItemPayload[2]
			{
				new WelcomeBackRewardItemPayload(randomBonusOfferItem)
				{
					requiresWatchingAdForItem = true,
					isOffer = true,
					offerMultiplier = 4,
					OnButtonClicked = base.InvokeOnButtonClicked,
					OnItemGiven = base.InvokeOnItemGiven
				},
				new WelcomeBackRewardItemPayload(randomBonusOfferItem)
				{
					requiresWatchingAdForItem = false,
					isOffer = false,
					offerMultiplier = 1,
					OnButtonClicked = base.InvokeOnButtonClicked,
					OnItemGiven = base.InvokeOnItemGiven
				}
			};
		}

		private void SpawnButtons()
		{
			for (int i = 0; i < _buttonParents.Length; i++)
			{
				WelcomeBackRewardItemButton welcomeBackRewardItemButton = SpawnButton(_welcomeBackRewardItemButtonPrefab, _buttonParents[i]);
				welcomeBackRewardItemButton.Setup(_buttonPayloads[i]);
				_spawnedButtons[i] = welcomeBackRewardItemButton;
			}
		}

		public void Show()
		{
			_bonusOfferGo.SetActive(true);
			UpdateLabels();
			UpdateButtonsWithNewItem();
		}

		private void UpdateLabels()
		{
			_bonusOfferOrLabel.text = Strings.Get("OR");
			_bonusOfferTitleLabel.text = Strings.Get("WELCOME_BACK_BONUS_OFFER_TITLE");
		}

		private void UpdateButtonsWithNewItem()
		{
			WelcomeBackRewardItem randomBonusOfferItem = WelcomeBackRewardManager.GetRandomBonusOfferItem();
			WelcomeBackRewardItemButton[] spawnedButtons = _spawnedButtons;
			foreach (WelcomeBackRewardItemButton welcomeBackRewardItemButton in spawnedButtons)
			{
				welcomeBackRewardItemButton.UpdateItemValues(randomBonusOfferItem);
			}
		}

		public void Hide()
		{
			_bonusOfferGo.SetActive(false);
		}

		public string GetBonusOfferAdItemDescription()
		{
			return GetDescriptionOfItem(_buttonPayloads[0]);
		}
	}
}
