using UnityEngine;

namespace UI.Popups.WelcomeBackRewardPopupInternal
{
	public class WelcomeBackBox : WelcomeBackRewardPopupBox
	{
		private WelcomeBackRewardItemButton _spawnedWelcomeBackButton;

		private WelcomeBackRewardPopup _welcomeBackRewardPopup;

		[SerializeField]
		private UILabel _welcomeBackTitleLabel;

		[SerializeField]
		private Transform _buttonParent;

		[SerializeField]
		private GameObject _welcomeBackBoxGo;

		private void Awake()
		{
			_spawnedWelcomeBackButton = SpawnButton(_welcomeBackRewardItemButtonPrefab, _buttonParent);
			InitButton();
		}

		private void InitButton()
		{
			_spawnedWelcomeBackButton.Setup(new WelcomeBackRewardItemPayload
			{
				itemType = WelcomeBackRewardManager.welcomeBackRewardType,
				itemAmount = WelcomeBackRewardManager.welcomeBackRewardAmount,
				requiresWatchingAdForItem = true,
				isOffer = false,
				offerMultiplier = 1,
				OnButtonClicked = base.InvokeOnButtonClicked,
				OnItemGiven = base.InvokeOnItemGiven
			});
		}

		public void Show()
		{
			_welcomeBackBoxGo.SetActive(true);
			UpdateLabels();
		}

		public void Hide()
		{
			_welcomeBackBoxGo.SetActive(false);
		}

		private void UpdateLabels()
		{
			_welcomeBackTitleLabel.text = Strings.Get("WELCOME_BACK_REWARD_TITLE");
		}

		public string GetWelcomeBackItemDescription()
		{
			return GetDescriptionOfItem(WelcomeBackRewardManager.welcomeBackRewardType, WelcomeBackRewardManager.welcomeBackRewardAmount);
		}
	}
}
