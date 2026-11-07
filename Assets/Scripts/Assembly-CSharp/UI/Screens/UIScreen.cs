using System;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using NaughtyAttributes;
using Onboarding;
using UnityEngine;
using Utilities;

namespace UI.Screens
{
	public abstract class UIScreen : MonoBehaviour
	{
		[UsedImplicitly]
		[SerializeField]
		public bool showOldFooterSection;

		[ShowIf("showOldFooterSection")]
		[SerializeField]
		protected GameObject _footerPrefab;

		[ShowIf("showOldFooterSection")]
		[SerializeField]
		protected int _selectedFooterButton;

		protected UIFooterHandler _footerHandler;

		private bool _footerInitialized;

		private bool _isActive;

		private string _screenShowFlurryEventName = string.Empty;

		[NonSerialized]
		public string screenName = string.Empty;

		protected const float DEFAULT_OFFSET_POPUP = -13f;

		public bool isActive
		{
			get
			{
				return _isActive;
			}
		}

		public string parentScreen { protected get; set; }

		protected virtual float screenOffset
		{
			get
			{
				return 0f;
			}
		}

		public void InitializeScreen()
		{
			PreInit();
			Init();
		}

		protected virtual void Init()
		{
		}

		public void ShowIfInactive()
		{
			if (!_isActive)
			{
				PreShow();
				Show();
			}
		}

		public void HideIfActive()
		{
			if (_isActive)
			{
				Hide();
				PostHide();
			}
		}

		private void PreInit()
		{
			AdjustToResolution();
		}

		private void PreShow()
		{
			base.gameObject.SetActive(true);
			_isActive = true;
			if (!base.gameObject.name.Contains("NotebookScreen"))
			{
				string flurryName = GetFlurryName();
				if (string.IsNullOrEmpty(flurryName))
				{
					flurryName = base.gameObject.name;
				}
				_screenShowFlurryEventName = "UI Screen " + flurryName;
				if (_screenShowFlurryEventName != string.Empty)
				{
					Flurry.LogTimedEvent(_screenShowFlurryEventName);
					OnboardingTracking.LogScreenFlow(base.gameObject.name);
				}
			}
			if (!_footerInitialized)
			{
				InitFooter();
				_footerInitialized = true;
			}
			base.transform.SetLocalPosY(screenOffset);
		}

		private void PostHide()
		{
			if (_screenShowFlurryEventName != string.Empty)
			{
				Flurry.EndTimedEvent(_screenShowFlurryEventName);
			}
			base.gameObject.SetActive(false);
			_isActive = false;
		}

		protected virtual void Show()
		{
		}

		protected virtual void Hide()
		{
		}

		public virtual string GetFlurryName()
		{
			return string.Empty;
		}

		public virtual void LoseFocus()
		{
		}

		public virtual void GainFocus()
		{
		}

		public virtual void AdjustToResolution()
		{
		}

		public virtual bool BlocksCelebrationPopup()
		{
			return false;
		}

		public virtual bool ShouldShowTopBar()
		{
			return true;
		}

		public virtual void BackButtonPressed()
		{
			UIScreenController.Instance.PushScreen("FrontScreen");
		}

		protected float CalculateCenterOffsetFromTopOffset(float desiredTopOffset)
		{
			int manualHeight = UIScreenController.Instance.root.manualHeight;
			float num = (float)(manualHeight - 480) * 0.5f + desiredTopOffset;
			if (SafeArea.IsRestricted)
			{
				num -= (float)SafeArea.OFFSET_TOP;
			}
			return num;
		}

		private void InitFooter()
		{
			if (_footerPrefab != null)
			{
				GameObject gameObject = base.gameObject.AddChild(_footerPrefab);
				_footerHandler = gameObject.GetComponent<UIFooterHandler>();
				switch (_selectedFooterButton)
				{
				case 1:
					_footerHandler.button1.GoToSelectedState();
					_footerHandler.button1.LockState(true);
					break;
				case 2:
					_footerHandler.button2.GoToSelectedState();
					_footerHandler.button2.LockState(true);
					break;
				case 3:
					_footerHandler.button3.GoToSelectedState();
					_footerHandler.button3.LockState(true);
					break;
				}
			}
		}

		public static bool IsScreenHeightOutOfProportion()
		{
			return (float)Screen.height * 1f / ((float)Screen.width * 1f) > 1.5f;
		}

		protected GameObject InitializeCoinbox(UIWidget widgetAnchor, bool fundsEnabled, bool coinsEnabled, bool keysEnabled, float xOffset = 0f, float yOffset = 0f, float zOffset = 0f)
		{
			GameObject prefab = Resources.Load("Prefabs/DynamicLoad/CoinboxQuick") as GameObject;
			GameObject gameObject = base.gameObject.AddChild(prefab);
			CoinBoxSizer componentInChildren = gameObject.GetComponentInChildren<CoinBoxSizer>();
			componentInChildren.Init(widgetAnchor, fundsEnabled, coinsEnabled, keysEnabled, xOffset, yOffset, zOffset);
			return gameObject;
		}

		protected void UpdateCoinboxPosition(GameObject coinbox, UIWidget widgetAnchor, float xOffset = 0f, float yOffset = 0f, float zOffset = 0f)
		{
			CoinBoxSizer componentInChildren = coinbox.GetComponentInChildren<CoinBoxSizer>();
			componentInChildren.UpdatePosition(widgetAnchor, xOffset, yOffset, zOffset);
		}
	}
}
