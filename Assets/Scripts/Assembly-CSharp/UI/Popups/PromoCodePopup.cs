using System;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Timer;
using UI.Generic;
using UnityEngine;

namespace UI.Popups
{
	public class PromoCodePopup : UIPopup
	{
		private enum PromoCodeButtonState
		{
			Active = 0,
			Inactive = 1,
			Loading = 2
		}

		[SerializeField]
		private ButtonPressedColorChange _buttonColorChanger;

		[SerializeField]
		private UILabel _titleLabel;

		[SerializeField]
		private UIInput _input;

		[SerializeField]
		private UILabel _codeInputLabel;

		[SerializeField]
		private UISprite _codeInputLabelIndicator;

		[SerializeField]
		private GameObject _errorBoxObject;

		[SerializeField]
		private UISprite _errorBoxSprite;

		[SerializeField]
		private UILabel _errorBoxLabel;

		[SerializeField]
		private UISprite _okButtonFillSprite;

		[SerializeField]
		private UISprite _okButtonOverlaySprite;

		[SerializeField]
		private GameObject _okButtonCheckmark;

		[SerializeField]
		private GameObject _okButtonSpinner;

		[SerializeField]
		private UITexture _charactersTexture;

		[SerializeField]
		private UIWidget _charactersGuide;

		private bool _textfieldClicked;

		private float _caretBlinkDuration;

		private const float COUNTDOWN_DURATION = 0.5f;

		private float _cachedOffset = float.MinValue;

		protected override float screenOffset
		{
			get
			{
				if (Math.Abs(_cachedOffset - float.MinValue) > 0.01f)
				{
					return _cachedOffset;
				}
				float aspectRatio = Utils.GetAspectRatio();
				if (aspectRatio.IsBetweenInclusive(1.5f, 1.6f))
				{
					_cachedOffset = CalculateCenterOffsetFromTopOffset(13f);
				}
				else
				{
					_cachedOffset = CalculateCenterOffsetFromTopOffset(-18f);
				}
				return _cachedOffset;
			}
		}

		protected override void Init()
		{
			BaseUISetup();
			FitCharactersTextureToGuide();
		}

		private void BaseUISetup()
		{
			_titleLabel.color = ColorData.instance.peacock;
			_codeInputLabelIndicator.color = ColorData.instance.pigeon;
			_input.caretColor = ColorData.instance.pigeon;
			_input.caretColor.a = 0f;
			Color32 color = ColorData.instance.peach;
			color.a = 102;
			_errorBoxSprite.color = color;
			_errorBoxLabel.color = ColorData.instance.crimson;
			_buttonColorChanger.normalColor = ColorData.instance.grass;
			_buttonColorChanger.pressedColor = ColorData.instance.avocado;
		}

		protected override void Hide()
		{
			PromoCodeManager.instance.StopGetRoutine();
			if (!Application.isEditor)
			{
				DeviceUtility.DismissKeyboard();
			}
		}

		protected override void Show()
		{
			_titleLabel.text = Strings.Get("PROMO_CODES_POPUP_DESCRIPTION");
			SetButtonState(PromoCodeButtonState.Active);
			_input.activeTextColor = ColorData.instance.midnight;
			_errorBoxObject.gameObject.SetActive(false);
			_input.transform.parent.gameObject.SetActive(false);
			_input.transform.parent.gameObject.SetActive(true);
			_input.value = string.Empty;
			_textfieldClicked = false;
			_codeInputLabelIndicator.enabled = false;
		}

		public void OnFindPromoCodeClicked()
		{
			SetButtonState(PromoCodeButtonState.Loading);
			_textfieldClicked = false;
			_codeInputLabelIndicator.enabled = false;
			string code = _codeInputLabel.text.ToLower();
			PromoCodeManager.instance.ActivateCode(code, HandleError);
		}

		public void CheckInputField()
		{
			_codeInputLabelIndicator.enabled = true;
			_caretBlinkDuration = 0.5f;
			_codeInputLabelIndicator.transform.localPosition = new Vector3(_codeInputLabel.transform.localPosition.x + _codeInputLabel.printedSize.x + 2f, _codeInputLabelIndicator.transform.position.y);
		}

		private void SetButtonState(PromoCodeButtonState state)
		{
			switch (state)
			{
			case PromoCodeButtonState.Active:
				_okButtonFillSprite.color = ColorData.instance.grass;
				_okButtonOverlaySprite.gameObject.SetActive(true);
				_okButtonOverlaySprite.color = ColorData.instance.spring;
				_okButtonCheckmark.gameObject.SetActive(true);
				_okButtonSpinner.gameObject.SetActive(false);
				_buttonColorChanger.GetComponent<BoxCollider>().enabled = true;
				_input.GetComponent<BoxCollider>().enabled = true;
				_codeInputLabel.color = ColorData.instance.midnight;
				break;
			case PromoCodeButtonState.Inactive:
				_okButtonFillSprite.color = ColorData.instance.slate;
				_okButtonOverlaySprite.gameObject.SetActive(true);
				_okButtonOverlaySprite.color = ColorData.instance.stroking;
				_okButtonCheckmark.gameObject.SetActive(true);
				_okButtonSpinner.gameObject.SetActive(false);
				_buttonColorChanger.GetComponent<BoxCollider>().enabled = false;
				_input.GetComponent<BoxCollider>().enabled = true;
				_codeInputLabel.color = ColorData.instance.crimson;
				break;
			case PromoCodeButtonState.Loading:
				_okButtonFillSprite.color = ColorData.instance.slate;
				_okButtonOverlaySprite.gameObject.SetActive(false);
				_okButtonCheckmark.gameObject.SetActive(false);
				_okButtonSpinner.gameObject.SetActive(true);
				_buttonColorChanger.GetComponent<BoxCollider>().enabled = false;
				_input.GetComponent<BoxCollider>().enabled = false;
				_codeInputLabel.color = ColorData.instance.pigeon;
				break;
			}
		}

		private void HandleError(int code)
		{
			_errorBoxObject.SetActive(true);
			SetButtonState(PromoCodeButtonState.Inactive);
			switch (code)
			{
			case 7000:
			case 7001:
			case 7002:
			case 7006:
				_errorBoxLabel.text = Strings.Get("PROMO_CODES_ERROR_TRY_AGAIN");
				break;
			case 7004:
				_errorBoxLabel.text = Strings.Get("PROMO_CODES_ERROR_CODE_NOT_EXIST");
				break;
			case 7005:
				_errorBoxLabel.text = Strings.Get("PROMO_CODES_ERROR_CODE_EXPIRED");
				break;
			case 7007:
				_errorBoxLabel.text = Strings.Get("PROMO_CODES_ERROR_CODE_BEEN_USED");
				break;
			case 100:
				_errorBoxLabel.text = Strings.Get("PROMO_CODES_ERROR_UPDATE_GAME");
				break;
			case 101:
				_errorBoxLabel.text = Strings.Get("PROMO_CODES_ERROR_CONTACT_SUPPORT");
				break;
			case 102:
				_errorBoxLabel.text = Strings.Get("PROMO_CODES_ERROR_ALREADY_OWNED");
				break;
			case 0:
				ResetPopupState();
				break;
			default:
				_errorBoxLabel.text = "Error Code: " + code;
				break;
			}
		}

		private void ResetPopupState()
		{
			_input.value = string.Empty;
			_errorBoxObject.SetActive(false);
			SetButtonState(PromoCodeButtonState.Active);
		}

		[UsedImplicitly]
		public void EntryFieldClicked()
		{
			ResetPopupState();
			_textfieldClicked = true;
			_codeInputLabelIndicator.enabled = true;
			_caretBlinkDuration = 0.5f;
		}

		private void Update()
		{
			UpdateTextfieldIndicator();
		}

		private void UpdateTextfieldIndicator()
		{
			if (_textfieldClicked)
			{
				if (_caretBlinkDuration <= 0f)
				{
					_codeInputLabelIndicator.enabled = !_codeInputLabelIndicator.enabled;
					_caretBlinkDuration = 0.5f;
				}
				_caretBlinkDuration -= Time.deltaTime;
			}
			if (TouchScreenKeyboard.isSupported && !TouchScreenKeyboard.visible)
			{
				_codeInputLabelIndicator.enabled = false;
			}
		}

		private void FitCharactersTextureToGuide()
		{
			_charactersGuide.topAnchor.absolute = TopBar.GetTopBarOffset();
			Timer.ExecuteNextFrame(() =>
			{
				_charactersGuide.ResetAndUpdateAnchors();
				float num = (float)_charactersTexture.mainTexture.width / (float)_charactersTexture.mainTexture.height;
				float num2 = _charactersTexture.transform.position.y - _charactersGuide.transform.position.y;
				num2 /= (float)_charactersTexture.height;
				if ((float)_charactersGuide.width / num > (float)_charactersGuide.height)
				{
					_charactersTexture.height = _charactersGuide.height;
					_charactersTexture.width = Mathf.CeilToInt((float)_charactersTexture.height * num);
				}
				else
				{
					_charactersTexture.width = _charactersGuide.width;
					_charactersTexture.height = Mathf.CeilToInt((float)_charactersTexture.width / num);
				}
				_charactersTexture.transform.position = _charactersGuide.transform.position + new Vector3(0f, num2 * (float)_charactersTexture.height);
			});
		}
	}
}
