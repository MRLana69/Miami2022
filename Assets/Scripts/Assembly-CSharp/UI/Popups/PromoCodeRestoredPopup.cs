using System.Collections.Generic;
using System.Linq;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using NaughtyAttributes;
using UI.Generic;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.Popups
{
	public class PromoCodeRestoredPopup : UIPopup
	{
		private enum RestoredType
		{
			Character = 0,
			Board = 1,
			Coins = 2,
			Keys = 3
		}

		[SerializeField]
		private GameObject _restoredContentPrefab;

		[SerializeField]
		private GameObject _payloadContentObj;

		[SerializeField]
		private UILabel _headerLabel;

		[SerializeField]
		private UILabel _okButtonLabel;

		[SerializeField]
		private UISprite _blueBGSprite;

		[SerializeField]
		private UISprite[] _raySprites;

		[SerializeField]
		private ButtonColor _buttonColor;

		[SerializeField]
		private UISprite _buttonFillSprite;

		[SerializeField]
		private UISprite _buttonGlintSprite;

		[SerializeField]
		private Layout _contentLayout;

		private List<GameObject> _restoredContents;

		private const string CHARACTER_ICON = "ic_character";

		private const string KEY_ICON = "ic_keys";

		private const string COIN_ICON = "ic_coins";

		private const string BOARD_ICON = "ic_boards";

		private const float RAY_COLOR_ALPHA = 0.2f;

		private const float RESTORED_CONTENT_LOGO_SIZE = 21f;

		private float _restoredContentsMaxWidth;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			BasicSetup();
			_restoredContents = new List<GameObject>();
		}

		[Button(null)]
		private void BasicSetup()
		{
			ColorData instance = ColorData.instance;
			_headerLabel.color = instance.midnight;
			_blueBGSprite.color = instance.smurf;
			UISprite[] raySprites = _raySprites;
			foreach (UISprite uISprite in raySprites)
			{
				uISprite.color = instance.white.WithAlpha(0.2f);
			}
			_buttonFillSprite.color = instance.grass;
			_buttonGlintSprite.color = instance.spring;
			_buttonColor.SetNormalColor = instance.grass;
			_buttonColor.SetPressedColor = instance.avocado;
			base.gameObject.Blink();
		}

		protected override void Show()
		{
			_headerLabel.text = Strings.Get("PROMO_CODES_ITEMS_RESTORED");
			_okButtonLabel.text = Strings.Get("OK");
			ClearContent();
			if (PromoCodeManager.instance.restoredNumberOfCharacters != 0)
			{
				AddRestoredContentToList(RestoredType.Character);
			}
			if (PromoCodeManager.instance.restoredNumberOfBoards != 0)
			{
				AddRestoredContentToList(RestoredType.Board);
			}
			if (PromoCodeManager.instance.restoredNumberOfKeys != 0)
			{
				AddRestoredContentToList(RestoredType.Keys);
			}
			if (PromoCodeManager.instance.restoredNumberOfCoins != 0)
			{
				AddRestoredContentToList(RestoredType.Coins);
			}
			foreach (GameObject restoredContent in _restoredContents)
			{
				restoredContent.transform.localPosition = new Vector3((0f - _restoredContentsMaxWidth) / 2f - 10.5f, restoredContent.transform.localPosition.y, restoredContent.transform.localPosition.z);
			}
			_contentLayout.Align();
			GetComponentsInChildren<UIRect>().ToList().ForEach((UIRect r) =>
			{
				r.ResetAndUpdateAnchors();
			});
		}

		private void ClearContent()
		{
			foreach (GameObject restoredContent in _restoredContents)
			{
				restoredContent.Kill();
			}
			_restoredContents.Clear();
			_restoredContentsMaxWidth = 0f;
		}

		private void AddRestoredContentToList(RestoredType type)
		{
			string spriteName = string.Empty;
			string text = string.Empty;
			switch (type)
			{
			case RestoredType.Character:
				spriteName = "ic_character";
				text = string.Format(Strings.Get("PROMO_CODES_CHARACTERS_RESTORED"), PromoCodeManager.instance.restoredNumberOfCharacters);
				break;
			case RestoredType.Board:
				spriteName = "ic_boards";
				text = string.Format(Strings.Get("PROMO_CODES_BOARDS_RESTORED"), PromoCodeManager.instance.restoredNumberOfBoards);
				break;
			case RestoredType.Keys:
				spriteName = "ic_keys";
				text = string.Format(Strings.Get("PROMO_CODES_KEYS_RESTORED"), PromoCodeManager.instance.restoredNumberOfKeys);
				break;
			case RestoredType.Coins:
				spriteName = "ic_coins";
				text = string.Format(Strings.Get("PROMO_CODES_COINS_RESTORED"), PromoCodeManager.instance.restoredNumberOfCoins);
				break;
			}
			GameObject gameObject = _payloadContentObj.AddChild(_restoredContentPrefab);
			gameObject.GetComponent<RestoredContentHelper>().Setup(spriteName, text);
			float num = 0f;
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				num += (float)gameObject.transform.GetChild(i).GetComponent<UIWidget>().width;
			}
			if (_restoredContentsMaxWidth < num)
			{
				_restoredContentsMaxWidth = num;
			}
			_restoredContents.Add(gameObject);
		}

		[UsedImplicitly]
		public void ClosePopupClicked()
		{
			UIScreenController.Instance.ClosePopup();
		}
	}
}
