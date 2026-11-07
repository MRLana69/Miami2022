using System;
using Info;
using Kiloo.Common.Extensions;
using UI.Screens.RewardManagement;
using UnityEngine;

[Serializable]
public class ItemIconSetup
{
	private const string ICON_NAME_COINS = "icon_coin_large";

	private const string ICON_NAME_KEYS = "icon_key";

	private const string ICON_NAME_HOVERBOARD = "icon_hoverboard";

	private const string ICON_NAME_SCOREBOOSTER = "icon_upgrade_score_booster";

	private const string ICON_NAME_HEADSTART = "icon_upgrades_headstart2000_large";

	[SerializeField]
	private CelebrationRewardType _itemType;

	[SerializeField]
	private UpgradeType _upgradeType;

	[SerializeField]
	private int _iconHeight;

	[SerializeField]
	private float _iconYPosition;

	[SerializeField]
	private Vector3 _iconRotation;

	public CelebrationRewardType itemType
	{
		get
		{
			return _itemType;
		}
	}

	public UpgradeType upgradeType
	{
		get
		{
			return _upgradeType;
		}
	}

	public void Activate(UISprite iconSprite)
	{
		iconSprite.spriteName = GetIconSpriteName();
		iconSprite.transform.SetLocalPosY(_iconYPosition);
		iconSprite.transform.localEulerAngles = _iconRotation;
		MakeSpritePixelPerfect(iconSprite);
		iconSprite.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
		iconSprite.Update();
		iconSprite.height = _iconHeight;
	}

	private void MakeSpritePixelPerfect(UISprite iconSprite)
	{
		iconSprite.keepAspectRatio = UIWidget.AspectRatioSource.Free;
		iconSprite.MakePixelPerfect();
	}

	private string GetIconSpriteName()
	{
		switch (_itemType)
		{
		case CelebrationRewardType.coins:
			return "icon_coin_large";
		case CelebrationRewardType.keys:
			return "icon_key";
		case CelebrationRewardType.powerup:
			return GetPrizeIconSpriteNameForPowerUp(_upgradeType);
		default:
			return string.Empty;
		}
	}

	private string GetPrizeIconSpriteNameForPowerUp(UpgradeType upgradeType)
	{
		switch (upgradeType)
		{
		case UpgradeType.hoverboard:
			return "icon_hoverboard";
		case UpgradeType.headstart2000:
			return "icon_upgrades_headstart2000_large";
		case UpgradeType.scorebooster:
			return "icon_upgrade_score_booster";
		default:
			return string.Empty;
		}
	}
}
