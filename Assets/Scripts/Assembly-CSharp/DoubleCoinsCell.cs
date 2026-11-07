using System.Collections;
using System.Collections.Generic;
using Data.Save;
using InApp;
using Kiloo.Common.Backend.PurchaseValidation;
using UI;
using UI.Generic.Layout;
using UnityEngine;

public class DoubleCoinsCell : MonoBehaviour
{
	[SerializeField]
	private UILabel _titleLabel;

	[SerializeField]
	private UILabel _subTitleLabel;

	[SerializeField]
	private UIButtonOverlayOff _priceButton;

	[SerializeField]
	private Layout _priceButtonLayout;

	[SerializeField]
	private UILabel _priceButtonLabel;

	[SerializeField]
	private UISprite _priceButtonBoughtCheckmark;

	[SerializeField]
	private float _starAnimationPause;

	[SerializeField]
	private float _starCountDown;

	[SerializeField]
	private List<GameObject> _starPosition;

	[SerializeField]
	private GameObject _starPrefab;

	private List<GameObject> _stars = new List<GameObject>();

	private IEnumerator _starAnimation;

	private string _doubleCoinsInAppId = "com.kiloo.subways.doublecoins";

	public void Init()
	{
		if (_starPosition.Count > 0 && _stars.Count <= 0)
		{
			foreach (GameObject item in _starPosition)
			{
				_stars.Add(Object.Instantiate(_starPrefab, item.transform.position, Quaternion.Euler(0f, 0f, 0f), item.transform));
			}
			_starAnimation = ActivateStars();
			StartCoroutine(_starAnimation);
		}
		Setup();
		UpdatePriceButton();
	}

	private void Setup()
	{
		SetColors();
		_titleLabel.text = Strings.Get("GAMEOVER_DOUBLE_COINS_BANNER_TITLE");
		_subTitleLabel.text = Strings.Get("DOUBLE_COINS_BANNER_SUBTITLE");
	}

	private void SetColors()
	{
		_titleLabel.color = ColorData.instance.sun;
		_titleLabel.effectColor = ColorData.instance.shadow;
		_subTitleLabel.color = ColorData.instance.royale;
		_subTitleLabel.effectColor = ColorData.instance.lightShadow;
		_priceButtonLabel.color = Color.white;
		_priceButtonLabel.effectColor = ColorData.instance.shadow;
	}

	private void UpdatePriceButton()
	{
		if (UserSave.hasDoubleCoinsUpgrade)
		{
			_priceButtonBoughtCheckmark.enabled = true;
			_priceButtonLabel.text = Strings.Get("STORE_CELL_PURCHASED");
			_priceButton.GoToNotAvailableState();
		}
		else
		{
			_priceButtonBoughtCheckmark.enabled = false;
			_priceButtonLabel.text = InAppData.inAppData[_doubleCoinsInAppId].localizedPrice;
			_priceButton.GoToNormalState();
		}
		_priceButtonLayout.Align(true);
	}

	private void OnEnable()
	{
		SetCallbacks();
		if (_starAnimation != null)
		{
			StartCoroutine(_starAnimation);
		}
	}

	private void OnDisable()
	{
		RemoveCallbacks();
		if (_starAnimation != null)
		{
			StopCoroutine(_starAnimation);
		}
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
		if (productId == _doubleCoinsInAppId)
		{
			UpdatePriceButton();
		}
	}

	private IEnumerator ActivateStars()
	{
		while (true)
		{
			foreach (GameObject star in _stars)
			{
				int children = star.transform.childCount;
				for (int i = 0; i < children; i++)
				{
					star.transform.GetChild(i).GetComponent<Animation>().Play();
				}
				yield return new WaitForSeconds(_starCountDown);
			}
			yield return new WaitForSeconds(_starAnimationPause);
		}
	}

	public void OnPurchaseClicked()
	{
		if (!UserSave.hasDoubleCoinsUpgrade)
		{
			InAppManager.instance.StartPurchase(_doubleCoinsInAppId);
		}
	}
}
