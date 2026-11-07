using System;
using System.Collections.Generic;
using Data.Save;
using Info;
using Kiloo.Common.Extensions;
using UI.Generic.Layout;
using UI.UpgradesInternal;
using UnityEngine;

namespace UI.Screens.UpgradeScreenInternal
{
	public class UpgradeContainer : MonoBehaviour
	{
		public class UpgradePayload
		{
			public UpgradeType upgradeType;

			public Func<float> getAnimationTime;
		}

		[SerializeField]
		private UISprite _upgradeIcon;

		[SerializeField]
		private UISprite _background;

		[SerializeField]
		private UILabel _iconValue;

		[SerializeField]
		private UILabel _header;

		[SerializeField]
		private UILabel _description;

		[SerializeField]
		private UpgradeButtonHelper _buttonHelper;

		[SerializeField]
		private Layout _tierLayout;

		[SerializeField]
		private UpgradeTierHelper _originalTierHelper;

		[SerializeField]
		private AnimationCurve _buyableTierAnimationCurve;

		private List<UpgradeTierHelper> _tierHelpers;

		private UpgradePayload _payload;

		private int _price = -1;

		private const string TIER_COMPLETE_SPRITE_NAME = "upgrade_tier_completed";

		private const string TIER_BUYABLE_SPRITE_NAME = "upgrade_tier_active";

		private const string TIER_LOCKED_SPRITE_NAME = "upgrade_tier_slot";

		private const string TIER_BOUGHT_SPRITE_NAME = "upgrade_tier";

		private bool _purchaseInProgress;

		private void SetupColors()
		{
			ColorData instance = ColorData.instance;
			_header.color = instance.sun;
			_header.effectColor = instance.shadow;
			_description.color = instance.white;
			_description.effectColor = instance.shadow;
			_background.color = instance.iron;
		}

		public void Setup(UpgradePayload payload)
		{
			_payload = payload;
			UpgradeType upgradeType = payload.upgradeType;
			SetupColors();
			_upgradeIcon.spriteName = Upgrades.upgrades[upgradeType].iconNameMenu;
			_upgradeIcon.MakePixelPerfect();
			_header.text = Strings.Get(Upgrades.upgrades[upgradeType].name);
			_description.text = Strings.Get(Upgrades.upgrades[upgradeType].description);
			string buttonText = string.Empty;
			bool flag = false;
			if (Upgrades.IsConsumable(upgradeType))
			{
				_tierLayout.gameObject.SetActive(false);
				_iconValue.text = string.Empty + InventorySave.GetAmount(upgradeType);
				buttonText = string.Empty + Upgrades.upgrades[upgradeType].GetPriceToIncreaseTier(0);
			}
			else if (Upgrades.IsPermanent(upgradeType))
			{
				_tierLayout.gameObject.SetActive(true);
				_iconValue.text = string.Empty;
				LoadTierSection();
				flag = Upgrades.IsCompleted(upgradeType);
				if (flag)
				{
					buttonText = Strings.Get("UPGRADES_UPGRADE_COMPLETED");
					SetupCompleteTierBoxes(upgradeType);
				}
				else
				{
					_price = Upgrades.upgrades[upgradeType].GetPriceToIncreaseTier(InventorySave.GetTier(upgradeType));
					buttonText = _price + string.Empty;
					SetupNormalTierBoxes(upgradeType);
				}
			}
			_buttonHelper.Setup(new UpgradeButtonHelper.UpgradeButtonPayload
			{
				buttonText = buttonText,
				isComplete = flag
			});
		}

		private void SetupNormalTierBoxes(UpgradeType type)
		{
			int tier = InventorySave.GetTier(type);
			for (int i = 0; i < _tierHelpers.Count; i++)
			{
				string tierSpriteName = "upgrade_tier_active";
				string tierText = string.Empty;
				Color white = ColorData.instance.white;
				Color shadow = ColorData.instance.shadow;
				if (i == tier)
				{
					tierSpriteName = "upgrade_tier";
					tierText = GetUpgradeDurationString(type);
				}
				else if (i > tier)
				{
					tierSpriteName = "upgrade_tier_slot";
				}
				else if (i == tier - 1)
				{
					tierText = GetUpgradeDurationString(type, false);
					white.a = 0.5f;
					shadow.a = 0.5f;
				}
				_tierHelpers[i].Setup(new UpgradeTierHelper.UpgradeTierPayload
				{
					tierSpriteName = tierSpriteName,
					tierText = tierText,
					_labelColor = white,
					_labelEffectColor = shadow
				});
			}
		}

		private void SetupCompleteTierBoxes(UpgradeType type)
		{
			for (int i = 0; i < _tierHelpers.Count; i++)
			{
				string tierText = string.Empty;
				if (i == _tierHelpers.Count - 1)
				{
					tierText = GetUpgradeDurationString(type);
				}
				_tierHelpers[i].Setup(new UpgradeTierHelper.UpgradeTierPayload
				{
					tierSpriteName = "upgrade_tier_completed",
					tierText = tierText,
					_labelColor = ColorData.instance.mahogany,
					_labelEffectColor = ColorData.instance.shadow
				});
			}
		}

		private void Update()
		{
			if (_payload != null && _payload.getAnimationTime != null && _tierHelpers != null && _tierHelpers.Count > 0)
			{
				if (_price >= 0 && InventorySave.coins >= _price)
				{
					SetAnimationTime(_payload.getAnimationTime());
				}
				else
				{
					ResetAnimation();
				}
			}
		}

		private void ResetAnimation()
		{
			for (int i = 0; i < _tierHelpers.Count; i++)
			{
				_tierHelpers[i].SetScale(1f);
			}
		}

		private void SetAnimationTime(float time)
		{
			int tier = InventorySave.GetTier(_payload.upgradeType);
			float num = 1f;
			float num2 = 0.25f;
			float num3 = time % 2f;
			num += num2 * _buyableTierAnimationCurve.Evaluate(num3 / 2f);
			for (int i = 0; i < _tierHelpers.Count; i++)
			{
				if (i == tier)
				{
					_tierHelpers[i].SetScale(num);
				}
				else
				{
					_tierHelpers[i].SetScale(1f);
				}
			}
		}

		private void LoadTierSection()
		{
			if (_tierHelpers == null)
			{
				_tierHelpers = new List<UpgradeTierHelper>(6);
				_tierHelpers.Add(_originalTierHelper);
				for (int i = 0; i < 5; i++)
				{
					UpgradeTierHelper item = UnityEngine.Object.Instantiate(_originalTierHelper, _tierLayout.transform);
					_tierHelpers.Add(item);
				}
				_tierLayout.Align();
			}
		}

		private string GetUpgradeDurationString(UpgradeType type, bool nextValue = true)
		{
			return ((!nextValue) ? Upgrades.GetUpgradeDuration(type) : Upgrades.GetNextUpgradeDuration(type)).RoundToInt() + Strings.Get("TIME_SHORTFORM_SECONDS");
		}

		public void Refresh()
		{
			Setup(_payload);
		}

		public void ButtonClicked()
		{
			if (_purchaseInProgress)
			{
				return;
			}
			PurchaseHandler.Instance.PurchaseUpgrade(_payload.upgradeType, () =>
			{
				switch (_payload.upgradeType)
				{
				case UpgradeType.scorebooster:
					Flurry.LogEvent("Boost Scorebooster purchased");
					break;
				case UpgradeType.headstart2000:
					Flurry.LogEvent("Boost Headstart2000 purchased");
					break;
				case UpgradeType.hoverboard:
					Flurry.LogEvent("Boost Hoverboard purchased");
					break;
				case UpgradeType.coinmagnet:
					Flurry.LogEventWithAParameter("Boost Coinmagnet purchased", "Tier", InventorySave.GetTier(_payload.upgradeType).ToString());
					break;
				case UpgradeType.doubleMultiplier:
					Flurry.LogEventWithAParameter("Boost 2x multiplier purchased", "Tier", InventorySave.GetTier(_payload.upgradeType).ToString());
					break;
				case UpgradeType.jetpack:
					Flurry.LogEventWithAParameter("Boost jetpack purchased", "Tier", InventorySave.GetTier(_payload.upgradeType).ToString());
					break;
				case UpgradeType.supersneakers:
					Flurry.LogEventWithAParameter("Boost supersneakers purchased", "Tier", InventorySave.GetTier(_payload.upgradeType).ToString());
					break;
				}
				_purchaseInProgress = false;
				Refresh();
			}, () =>
			{
				_purchaseInProgress = false;
			});
		}
	}
}
