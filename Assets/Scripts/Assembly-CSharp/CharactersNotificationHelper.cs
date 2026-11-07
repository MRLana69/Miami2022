using Data.Managers;
using SYBO.Subway.Info;
using UI.Screens;
using UnityEngine;

public class CharactersNotificationHelper : MonoBehaviour
{
	[SerializeField]
	private UISprite notificationIcon;

	[SerializeField]
	private UILabel notificationLabel;

	[SerializeField]
	private UISprite exclamationMark;

	private bool _hasGoneToZero;

	private const float UPDATE_INTERVAL = 0.1f;

	private float _currentInterval;

	private void OnEnable()
	{
		RefreshNotification();
		FirstPurchaseBonusManager.onFirstPurchaseBonusBecameInactive += RefreshNotification;
		CharacterManager.characterUnlocked += OnCharacterUnlocked;
	}

	private void OnDisable()
	{
		FirstPurchaseBonusManager.onFirstPurchaseBonusBecameInactive -= RefreshNotification;
		CharacterManager.characterUnlocked -= OnCharacterUnlocked;
	}

	private void OnCharacterUnlocked(Characters.CharacterId characterID, bool wasRestored)
	{
		if (characterID == Characters.CharacterId.firstPurchaseBonusCharacter01)
		{
			RefreshNotification();
		}
	}

	private void Update()
	{
		if (!_hasGoneToZero && !(UIScreenController.Instance.GetTopScreenName() != "CharacterScreen"))
		{
			if (_currentInterval <= 0f)
			{
				_currentInterval += 0.1f;
				RefreshNotification();
			}
			else
			{
				_currentInterval -= Time.deltaTime;
			}
		}
	}

	private void RefreshNotification()
	{
		int num = CharacterStickerManager.GetNumberOfNewCharactersOrThemes();
		if (FirstPurchaseBonusManager.isCollectingFirstPurchaseBonus)
		{
			num++;
		}
		if (num > 0)
		{
			notificationIcon.enabled = true;
			notificationLabel.enabled = true;
			notificationLabel.text = num.ToString();
			exclamationMark.enabled = num == 1;
			notificationLabel.enabled = num > 1;
		}
		else
		{
			notificationIcon.enabled = false;
			notificationLabel.enabled = false;
			exclamationMark.enabled = false;
			_hasGoneToZero = true;
		}
	}
}
