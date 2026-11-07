using UI;
using UI.Screens;
using UnityEngine;

public class MeButtonNotificationHelper : MonoBehaviour
{
	[SerializeField]
	private BreadCrumbHelper breadCrumbHelper;

	[SerializeField]
	private UISprite notificationIcon;

	[SerializeField]
	private UILabel notificationLabel;

	[SerializeField]
	private UISprite exclamationMark;

	private void OnEnable()
	{
		RefreshNotification();
	}

	private void RefreshNotification()
	{
		int num = CharacterStickerManager.GetNumberOfNewCharactersOrThemes() + HoverboardStickerManager.GetNumberOfNewHoverboardsAndThemes();
		if (FirstPurchaseBonusManager.isCollectingFirstPurchaseBonus)
		{
			num++;
		}
		if (num > 0 && breadCrumbHelper != null && !breadCrumbHelper.IsAnyBreadCrumbActive && CharacterStickerManager.ShouldShowCharacterThemesStickerFlow() && HoverboardStickerManager.ShouldShowHoverboardStickerFlow())
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
		}
	}
}
