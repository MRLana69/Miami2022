using System;
using Data.Save;
using InApp;
using Kiloo.Common.Backend.PurchaseValidation;
using Kiloo.Common.Extensions;
using UI;
using UI.Generic;
using UI.Generic.Layout;
using UI.Screens;
using UnityEngine;

public class StarterPack : MonoBehaviour
{
	[Serializable]
	private class CellElements
	{
		public GameObject _gameObject;

		public UISprite _cellSprite;

		public UILabel _cellLabel;
	}

	[SerializeField]
	private CellElements[] _cells;

	[SerializeField]
	private TableLayout _cellLayout;

	[SerializeField]
	private UILabel _titelLabel;

	[SerializeField]
	private UISprite _mainIcon;

	[SerializeField]
	private FitSpriteToWidget _mainIconFitter;

	[SerializeField]
	private UIButtonOverlayOff _priceButton;

	[SerializeField]
	private UILabel _priceLabel;

	private InAppData.BundleTier _lastPurchasedBundle;

	private InAppItemModel _currentBundleItemModel;

	public void Init()
	{
		_lastPurchasedBundle = UserSave.lastPurchasedBundle;
		Setup();
	}

	private void OnEnable()
	{
		SetCallbacks();
	}

	private void OnDisable()
	{
		RemoveCallbacks();
	}

	private void SetCallbacks()
	{
		if (InAppManager.instance != null)
		{
			InAppManager.instance.onPurchaseSuccess += OnPurchaseSuccess;
		}
	}

	private void RemoveCallbacks()
	{
		if (InAppManager.instance != null)
		{
			InAppManager.instance.onPurchaseSuccess -= OnPurchaseSuccess;
		}
	}

	private void OnPurchaseSuccess(string productId, bool wasRestored, IPurchaseValidationResult validationResult)
	{
		if (productId.IsOneOf("com.kiloo.subways.bundletier1", "com.kiloo.subways.bundletier2", "com.kiloo.subways.bundletier3", "com.kiloo.subways.bundletier4", "com.kiloo.subways.bundletier5"))
		{
			_lastPurchasedBundle = UserSave.lastPurchasedBundle;
			if (_lastPurchasedBundle == InAppData.BundleTier.BundleTier5)
			{
				StoreScreen.RefreshOnNextShow();
				_priceButton.GoToNotAvailableState();
				_priceLabel.text = Strings.Get("STORE_CELL_PURCHASED");
			}
			else
			{
				Setup();
			}
			if (!wasRestored)
			{
				UIScreenController.Instance.ShowRewardAnimationForBundlePurchase(InAppData.inAppData[productId].bundleContents, productId);
			}
		}
	}

	private void Setup()
	{
		_currentBundleItemModel = InAppData.inAppData[InAppData.GetBundleId(StoreABManager.GetBundleTier(_lastPurchasedBundle))];
		_priceLabel.text = _currentBundleItemModel.localizedPrice;
		_titelLabel.text = StoreABManager.GetBundleLabel(_currentBundleItemModel);
		_titelLabel.color = ColorData.instance.midnight;
		_titelLabel.effectColor = ColorData.instance.lightShadow;
		_mainIcon.spriteName = _currentBundleItemModel.GetIconName();
		_mainIconFitter.FitNow();
		CellElements[] cells = _cells;
		foreach (CellElements cellElements in cells)
		{
			cellElements._gameObject.SetActive(false);
		}
		for (int j = 0; j < _currentBundleItemModel.bundleContents.Count; j++)
		{
			if (_cells[j] != null)
			{
				_cells[j]._gameObject.GetComponent<UIWidget>().width = (int)GetCellSize(j, _currentBundleItemModel.bundleContents.Count).x;
				_cells[j]._gameObject.GetComponent<UIWidget>().height = (int)GetCellSize(j, _currentBundleItemModel.bundleContents.Count).y;
				_cells[j]._gameObject.SetActive(true);
				_cells[j]._cellLabel.text = GetFormattedAmount(_currentBundleItemModel.bundleContents[j].amount);
				_cells[j]._cellSprite.spriteName = GetSpriteName(_currentBundleItemModel.bundleContents[j].type);
				_cells[j]._cellSprite.transform.rotation = Quaternion.Euler(GetSpriteRotation(_currentBundleItemModel.bundleContents[j].type));
				_cells[j]._cellSprite.GetComponent<FitSpriteToWidget>().FitNow();
			}
		}
		_cellLayout.Align(true);
	}

	private Vector3 GetSpriteRotation(InAppItemModel.ItemType type)
	{
		switch (type)
		{
		case InAppItemModel.ItemType.Scoreboosters:
			return Vector3.forward * -25f;
		case InAppItemModel.ItemType.Keys:
			return Vector3.forward * 40f;
		default:
			return Vector3.zero;
		}
	}

	private string GetSpriteName(InAppItemModel.ItemType type)
	{
		switch (type)
		{
		case InAppItemModel.ItemType.Coins:
			return "coin_pack_4";
		case InAppItemModel.ItemType.Keys:
			return "key_pack_1";
		case InAppItemModel.ItemType.Hoverboards:
			return "hoverboard_icon";
		case InAppItemModel.ItemType.Headstarts:
			return "icon_upgrade_headstart";
		case InAppItemModel.ItemType.Scoreboosters:
			return "score_booster_icon";
		default:
			SDebug.LogError(string.Concat("Type ", type, " is not defined"));
			return string.Empty;
		}
	}

	private Vector2 GetCellSize(int index, int amount)
	{
		switch (amount)
		{
		case 4:
			return new Vector2(72f, 47f);
		case 5:
			if (index == 4)
			{
				return new Vector2(47f, 97f);
			}
			return new Vector2(47f, 47f);
		default:
			return new Vector2(47f, 97f);
		}
	}

	private InAppData.BundleTier GetBundleToSell()
	{
		switch (_lastPurchasedBundle)
		{
		case InAppData.BundleTier.None:
			return InAppData.BundleTier.BundleTier1;
		case InAppData.BundleTier.BundleTier1:
			return InAppData.BundleTier.BundleTier2;
		case InAppData.BundleTier.BundleTier2:
			return InAppData.BundleTier.BundleTier3;
		case InAppData.BundleTier.BundleTier3:
			return InAppData.BundleTier.BundleTier4;
		case InAppData.BundleTier.BundleTier4:
			return InAppData.BundleTier.BundleTier5;
		case InAppData.BundleTier.BundleTier5:
			SDebug.LogError("There are no more bundles to sell");
			return InAppData.BundleTier.None;
		default:
			SDebug.LogError("Unidetified bundle tier: " + _lastPurchasedBundle);
			return InAppData.BundleTier.None;
		}
	}

	private string GetFormattedAmount(int amount)
	{
		if (amount >= 10000)
		{
			return amount / 1000 + "K";
		}
		return amount.ToString();
	}

	public void OnBuyClicked()
	{
		if (_currentBundleItemModel != null)
		{
			InAppManager.instance.StartPurchase(_currentBundleItemModel.itemId);
		}
	}
}
