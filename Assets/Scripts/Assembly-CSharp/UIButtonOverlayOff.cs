using Kiloo.Common.Extensions;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class UIButtonOverlayOff : MonoBehaviour
{
	public enum ButtonType
	{
		Custom = 0,
		Primary = 1,
		Secondary = 2,
		Tertiary = 3,
		Facebook = 4,
		Footer_back = 8,
		Footer_me = 9,
		Footer_friends = 10,
		Footer_shop = 11,
		Seasonal = 12,
		SocialGrey = 13,
		Exit = 17
	}

	public GameObject fillWithPrimeColor;

	private Color _originalFillColor;

	private UIWidget _fillSprite;

	[SerializeField]
	[FormerlySerializedAs("overlay")]
	private UISprite _overlay;

	private bool _initDone;

	private bool _buttonStateLocked;

	public ButtonType buttonType;

	private ButtonColorScheme _buttonColorScheme;

	private void Awake()
	{
		if (!_initDone)
		{
			Init();
		}
	}

	public void SetButtonType(ButtonType type)
	{
		buttonType = type;
		Init();
		ButtonPressed(false);
	}

	private void Init()
	{
		_initDone = true;
		ApplyDefaultColors();
	}

	[Button(null)]
	private void ApplyDefaultColors()
	{
		if (_fillSprite == null)
		{
			_fillSprite = fillWithPrimeColor.GetComponent<UIWidget>();
		}
		_buttonColorScheme = GlobalColors.GetButtonColorScheme(buttonType, base.gameObject);
		Color32? original = _buttonColorScheme.original;
		if (original.HasValue)
		{
			_fillSprite.color = _buttonColorScheme.original.Value;
		}
		_originalFillColor = _fillSprite.color;
		if (_overlay != null)
		{
			_overlay.enabled = true;
			Color32? light = _buttonColorScheme.light;
			if (light.HasValue)
			{
				_overlay.color = _buttonColorScheme.light.Value;
			}
		}
		base.gameObject.Blink();
	}

	[Button(null)]
	public void GoToNormalState()
	{
		if (_buttonStateLocked)
		{
			return;
		}
		if ((bool)GetComponent<BoxCollider>())
		{
			GetComponent<BoxCollider>().enabled = true;
		}
		_fillSprite.color = _originalFillColor;
		if (_overlay != null)
		{
			Color32? light = _buttonColorScheme.light;
			if (light.HasValue)
			{
				_overlay.color = _buttonColorScheme.light.Value;
			}
			_overlay.enabled = true;
		}
	}

	[Button(null)]
	public void GoToNotAvailableState()
	{
		if (!_buttonStateLocked)
		{
			if ((bool)GetComponent<BoxCollider>())
			{
				GetComponent<BoxCollider>().enabled = false;
			}
			if (_overlay != null)
			{
				_overlay.enabled = false;
			}
			Color32? unavailable = _buttonColorScheme.unavailable;
			if (unavailable.HasValue)
			{
				_fillSprite.color = _buttonColorScheme.unavailable.Value;
			}
		}
	}

	[Button(null)]
	public void GoToSelectedState()
	{
		if (!_buttonStateLocked)
		{
			if ((bool)GetComponent<BoxCollider>())
			{
				GetComponent<BoxCollider>().enabled = false;
			}
			if (_overlay != null)
			{
				_overlay.enabled = false;
			}
			Color32? selected = _buttonColorScheme.selected;
			if (selected.HasValue)
			{
				_fillSprite.color = _buttonColorScheme.selected.Value;
			}
		}
	}

	public void LockState(bool locked)
	{
		_buttonStateLocked = locked;
	}

	protected virtual void OnPress(bool isPressed)
	{
		ButtonPressed(isPressed);
	}

	public void ButtonPressed(bool isPressed)
	{
		if (base.enabled)
		{
			if (_overlay != null)
			{
				_overlay.gameObject.SetActive(!isPressed);
			}
			float num = 30f / 255f;
			Color originalFillColor = _originalFillColor;
			originalFillColor.r = Mathf.Max(0f, originalFillColor.r - num);
			originalFillColor.g = Mathf.Max(0f, originalFillColor.g - num);
			originalFillColor.b = Mathf.Max(0f, originalFillColor.b - num);
			_fillSprite.color = ((!isPressed) ? _originalFillColor : originalFillColor);
		}
	}
}
