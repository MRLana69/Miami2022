using Data.Save;
using NaughtyAttributes;
using UnityEngine;

public class CoinBoxSizer : MonoBehaviour
{
	[SerializeField]
	private UISprite _addFundsSprite;

	[SerializeField]
	private UISprite _grayFGCoins;

	[SerializeField]
	private UISprite _coinIcon;

	[SerializeField]
	private UISprite _grayFGKeys;

	[SerializeField]
	private UISprite _keyIcon;

	[SerializeField]
	private UISprite _shadow;

	[SerializeField]
	private GameObject _fundsParent;

	[SerializeField]
	private GameObject _coinsParent;

	[SerializeField]
	private GameObject _keysParent;

	[SerializeField]
	private BoxCollider _boxCollider;

	public UILabel coinAmountLabel;

	public UILabel keyAmountLabel;

	private UIAnchor parentAnchor;

	private bool _updateAutomatically = true;

	private bool _shadowEnabled;

	private float _cachedCoinsFGWidth;

	private float _cachedCoinIconWidth;

	private float _cachedCoinIconOffset;

	private float _cachedKeysFGWidth;

	private float _cachedKeysIconWidth;

	private float _depthOffset_default = -1f;

	private float _shadowWidthOffset = 2f;

	private float COINS_FG_SCALE_OFFSET = 25f;

	private float KEYS_FG_SCALE_OFFSET = 13f;

	private const float BOX_COLLIDER_DEPTH = -15f;

	private bool isInitialized;

	private UIPanel _panel;

	public bool updateAutomatically
	{
		get
		{
			return _updateAutomatically;
		}
		set
		{
			_updateAutomatically = value;
			if (value)
			{
				OnCoinsChanged();
			}
		}
	}

	public void Init(UIWidget widgetAnchor, bool fundsEnabled, bool coinsEnabled, bool keysEnabled, float xOffset_custom = 0f, float yOffset_custom = 0f, float zOffset_custom = 0f)
	{
		EnableElements(fundsEnabled, coinsEnabled, keysEnabled);
		parentAnchor = base.gameObject.transform.parent.GetComponent<UIAnchor>();
		if (widgetAnchor != null)
		{
			parentAnchor.side = UIAnchor.Side.Center;
			base.transform.localPosition = CalculateCoinboxPosition(widgetAnchor, UIScreenController.popupDefaultPosition, xOffset_custom, yOffset_custom);
			parentAnchor.container = widgetAnchor.gameObject;
		}
		else
		{
			if (parentAnchor != null)
			{
				parentAnchor.side = UIAnchor.Side.Top;
			}
			base.transform.localPosition = CalculateCoinboxPosition(widgetAnchor, UIScreenController.screenDefaultPosition, xOffset_custom, yOffset_custom);
			_depthOffset_default += zOffset_custom;
		}
		if (!isInitialized)
		{
			OnCoinsChanged();
			OnKeysChanged();
			isInitialized = true;
		}
	}

	public void UpdatePosition(UIWidget widgetAnchor, float xOffset, float yOffset, float zOffset)
	{
		parentAnchor = base.gameObject.transform.parent.GetComponent<UIAnchor>();
		if (widgetAnchor != null)
		{
			parentAnchor.side = UIAnchor.Side.Center;
			base.transform.localPosition = CalculateCoinboxPosition(widgetAnchor, UIScreenController.popupDefaultPosition, xOffset, yOffset);
			parentAnchor.container = widgetAnchor.gameObject;
			return;
		}
		if (parentAnchor != null)
		{
			parentAnchor.side = UIAnchor.Side.Top;
		}
		base.transform.localPosition = CalculateCoinboxPosition(null, UIScreenController.screenDefaultPosition, xOffset, yOffset);
		_depthOffset_default += zOffset;
	}

	[Button(null)]
	private void SetShadow()
	{
		Start();
		Init(null, true, true, true);
	}

	private void CacheCoinsSegmentElements()
	{
		if (_coinsParent.activeSelf)
		{
			_cachedCoinIconOffset = (float)_coinIcon.width * 2f / 3f;
			_cachedCoinIconWidth = _coinIcon.width;
			_cachedCoinsFGWidth = _grayFGCoins.width;
		}
		else
		{
			_cachedCoinIconOffset = (float)_coinIcon.width * 1f / 3f;
			_cachedCoinIconWidth = 0f;
			_cachedCoinsFGWidth = 0f;
		}
	}

	private void CacheKeysSegmentElements()
	{
		_cachedKeysIconWidth = _keyIcon.width;
		if (_keysParent.activeSelf)
		{
			_cachedKeysFGWidth = _grayFGKeys.width;
		}
		else
		{
			_cachedKeysFGWidth = 0f;
		}
	}

	private void EnableElements(bool fundsEnabled, bool coinsEnabled, bool keysEnabled)
	{
		_shadowEnabled = fundsEnabled;
		_shadow.enabled = _shadowEnabled;
		base.gameObject.GetComponent<BoxCollider>().enabled = fundsEnabled;
		_fundsParent.SetActive(fundsEnabled);
		_coinsParent.SetActive(coinsEnabled);
		_keysParent.SetActive(keysEnabled);
	}

	private void PlaceSegments()
	{
		CacheCoinsSegmentElements();
		CacheKeysSegmentElements();
		_coinIcon.transform.localPosition = new Vector3(_grayFGCoins.transform.localPosition.x - _cachedCoinsFGWidth - _cachedCoinIconWidth / 2f, _grayFGCoins.transform.localPosition.y, _grayFGCoins.transform.localPosition.z);
		_grayFGKeys.transform.localPosition = new Vector3(_grayFGCoins.transform.localPosition.x - _cachedCoinsFGWidth, _grayFGKeys.transform.localPosition.y, _grayFGKeys.transform.localPosition.z);
		keyAmountLabel.transform.localPosition = new Vector3(_grayFGKeys.transform.localPosition.x - _cachedCoinIconOffset, keyAmountLabel.transform.localPosition.y, keyAmountLabel.transform.localPosition.z);
		_keyIcon.transform.localPosition = new Vector3(_grayFGKeys.transform.localPosition.x - _cachedKeysFGWidth - _cachedKeysIconWidth / 2f, _keyIcon.transform.localPosition.y, _keyIcon.transform.localPosition.z);
		_shadow.transform.localPosition = _grayFGCoins.transform.localPosition;
		ScaleElementsThatStretchTheBox();
	}

	private void ScaleElementsThatStretchTheBox()
	{
		float num = _cachedCoinsFGWidth + _cachedKeysFGWidth + _shadowWidthOffset;
		float f = _addFundsSprite.height;
		_shadow.width = Mathf.RoundToInt(num);
		_shadow.height = Mathf.RoundToInt(f);
		_addFundsSprite.width = Mathf.RoundToInt(num + _cachedKeysIconWidth / 2f);
		_addFundsSprite.height = Mathf.RoundToInt(f);
		_boxCollider.size = new Vector3(_addFundsSprite.width, _addFundsSprite.height * 2, _boxCollider.bounds.size.z);
		_boxCollider.center = new Vector3(_addFundsSprite.transform.localPosition.x - _boxCollider.size.x / 2f, 0f, -15f);
	}

	private Vector3 CalculateCoinboxPosition(UIWidget widgetAnchor, Vector3 defaultPosition, float xOffset_custom, float yOffset_custom)
	{
		float num = defaultPosition.x + xOffset_custom;
		float num2 = defaultPosition.y + yOffset_custom;
		float z = defaultPosition.z;
		Vector2 vector = Vector2.zero;
		if (widgetAnchor != null)
		{
			vector = CalculateDistanceToBounds(widgetAnchor);
		}
		return new Vector3(vector.x + num, vector.y + num2, z);
	}

	private Vector2 CalculateDistanceToBounds(UIWidget widget)
	{
		float x = (float)widget.width / 2f;
		float y = (float)widget.height / 2f;
		return new Vector2(x, y);
	}

	private void OnEnable()
	{
		InventorySave.coinsChanged += OnCoinsChanged;
		InventorySave.keysChanged += OnKeysChanged;
		if (isInitialized)
		{
			OnCoinsChanged();
			OnKeysChanged();
		}
	}

	private void OnDisable()
	{
		InventorySave.coinsChanged -= OnCoinsChanged;
		InventorySave.keysChanged -= OnKeysChanged;
	}

	private void Start()
	{
		AdjustCoinsSegmentSize();
		_AdjustKeysSegmentSize();
		_panel = NGUITools.FindInParents<UIPanel>(base.gameObject);
	}

	private void OnCoinsChanged()
	{
		if (updateAutomatically && _coinsParent.activeSelf)
		{
			coinAmountLabel.text = InventorySave.coins.ToString();
			AdjustCoinsSegmentSize();
			if (coinAmountLabel.gameObject.activeInHierarchy && _panel != null && _panel.widgetsAreStatic)
			{
				_panel.widgetsAreStatic = false;
				_panel.Refresh();
				_panel.widgetsAreStatic = true;
			}
		}
	}

	private void OnKeysChanged()
	{
		if (_keysParent.activeSelf)
		{
			keyAmountLabel.text = InventorySave.keys.ToString();
			_AdjustKeysSegmentSize();
			if (keyAmountLabel.gameObject.activeInHierarchy && _panel != null && _panel.widgetsAreStatic)
			{
				_panel.widgetsAreStatic = false;
				_panel.Refresh();
				_panel.widgetsAreStatic = true;
			}
		}
	}

	public void AdjustCoinsSegmentSize()
	{
		if (_coinsParent.activeSelf)
		{
			float num = coinAmountLabel.width;
			_grayFGCoins.width = Mathf.RoundToInt(num + COINS_FG_SCALE_OFFSET);
		}
		PlaceSegments();
	}

	private void _AdjustKeysSegmentSize()
	{
		float num = 0f;
		if (_keysParent.activeSelf)
		{
			num = KEYS_FG_SCALE_OFFSET + _cachedCoinIconOffset;
			float num2 = keyAmountLabel.width;
			_grayFGKeys.width = Mathf.RoundToInt(num2 + num);
		}
		PlaceSegments();
	}
}
