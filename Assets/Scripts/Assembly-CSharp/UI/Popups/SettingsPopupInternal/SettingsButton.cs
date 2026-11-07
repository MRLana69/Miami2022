using System;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using UI.Breadcrumbs;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.Popups.SettingsPopupInternal
{
	public class SettingsButton : MonoBehaviour
	{
		public class SettingsButtonPayload
		{
			public Action<UILabel> setTitle;

			public Action<UILabel> setDescription;

			public string descriptionID = string.Empty;

			public Action onClick;

			public Action<GameObject> setOfflineIndicator;

			public string iconName;

			public Vector3 iconOffset = Vector2.zero;

			public Breadcrumb.BreadcrumbEvent[] breadcrumbEvents;

			public Func<bool> shouldDisable = () => false;

			public Action<UISprite> setIcon;
		}

		[SerializeField]
		private UISprite _icon;

		[SerializeField]
		private UISprite _offIndicatorIcon;

		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _description;

		[SerializeField]
		private Layout _layout;

		[SerializeField]
		private Breadcrumb _breadcrumb;

		[SerializeField]
		private UISprite _disableOverlay;

		[SerializeField]
		private Collider _buttonCollider;

		private SettingsButtonPayload _payload;

		private bool buttonDisabled
		{
			get
			{
				return _disableOverlay.gameObject.activeSelf;
			}
			set
			{
				_disableOverlay.gameObject.SetActive(value);
				_buttonCollider.enabled = !value;
			}
		}

		public void InitButton(SettingsButtonPayload payload)
		{
			_payload = payload;
			_icon.spriteName = _payload.iconName;
			_icon.MakePixelPerfect();
			_icon.transform.localPosition += _payload.iconOffset;
			_description.text = Strings.Get(_payload.descriptionID);
			_breadcrumb.EnableListeners(_payload.breadcrumbEvents);
			ColorData instance = ColorData.instance;
			_title.color = instance.royale;
			_description.color = instance.push;
			SetupButton();
		}

		[UsedImplicitly]
		public void OnClick()
		{
			if (!buttonDisabled)
			{
				_payload.onClick.SafeInvoke();
				SetupButton();
			}
		}

		private void SetupButton()
		{
			_payload.setOfflineIndicator.SafeInvoke(_offIndicatorIcon.gameObject);
			_payload.setTitle.SafeInvoke(_title);
			_payload.setDescription.SafeInvoke(_description);
			_payload.setIcon.SafeInvoke(_icon);
			_layout.Align();
			buttonDisabled = _payload.shouldDisable.SafeInvoke();
		}

		public void Refresh()
		{
			if (_icon != null)
			{
				_payload.setIcon.SafeInvoke(_icon);
			}
		}

		// Atualiza título/descrição/ícone (ex.: ao voltar do menu de escolha do treino)
		public void RefreshAll()
		{
			if (_payload != null)
			{
				SetupButton();
			}
		}

		private void OnEnable()
		{
			if (_payload != null)
			{
				SetupButton();
			}
		}
	}
}
