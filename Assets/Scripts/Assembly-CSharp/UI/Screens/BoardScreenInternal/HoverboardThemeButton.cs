using System;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;

namespace UI.Screens.BoardScreenInternal
{
	public class HoverboardThemeButton : MonoBehaviour
	{
		public delegate void OnPressEvent(int index);

		public enum ThemeState
		{
			NotOwned = 0,
			OwnedAndInactive = 1,
			OwnedAndActive = 2,
			NotPurchaseable = 3,
			BundleState = 4
		}

		[SerializeField]
		private UISprite _background;

		[SerializeField]
		private UISprite _icon;

		[SerializeField]
		private GameObject giftBuble;

		[SerializeField]
		private GameObject newBuble;

		[SerializeField]
		private GameObject selectedCircle;

		[SerializeField]
		private GameObject ownedCircle;

		[SerializeField]
		private GameObject _lockIcon;

		private HoverboardThemes.ThemeIdentifier _identifier;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnPressEvent OnPress__BackingField;

		public int themeIndex { get; private set; }

		public event OnPressEvent OnPress
		{
			add
			{
				OnPressEvent onPressEvent = OnPress__BackingField;
				OnPressEvent onPressEvent2;
				do
				{
					onPressEvent2 = onPressEvent;
					onPressEvent = Interlocked.CompareExchange(ref OnPress__BackingField, (OnPressEvent)Delegate.Combine(onPressEvent2, value), onPressEvent);
				}
				while ((object)onPressEvent != onPressEvent2);
			}
			remove
			{
				OnPressEvent onPressEvent = OnPress__BackingField;
				OnPressEvent onPressEvent2;
				do
				{
					onPressEvent2 = onPressEvent;
					onPressEvent = Interlocked.CompareExchange(ref OnPress__BackingField, (OnPressEvent)Delegate.Remove(onPressEvent2, value), onPressEvent);
				}
				while ((object)onPressEvent != onPressEvent2);
			}
		}

		public HoverboardThemeButton()
		{
			themeIndex = 0;
		}

		public void InitButton(string iconName, Color32 bgColor, Color32 iconColor, int index)
		{
			if (!string.IsNullOrEmpty(iconName))
			{
				_icon.spriteName = iconName;
			}
			_icon.MakePixelPerfect();
			_icon.color = iconColor;
			_background.color = bgColor;
			themeIndex = index;
		}

		public void SetActiveBubbles(bool enableGiftBuble, bool enableNewBuble)
		{
			if (giftBuble != null)
			{
				giftBuble.SetActive(enableGiftBuble);
			}
			if (newBuble != null)
			{
				newBuble.SetActive(enableNewBuble);
			}
		}

		public void SetCurrentState(ThemeState newState)
		{
			if (newState == ThemeState.BundleState)
			{
				_lockIcon.SetActive(false);
			}
			else
			{
				_lockIcon.SetActive(newState == ThemeState.NotPurchaseable);
			}
			selectedCircle.SetActive(newState == ThemeState.OwnedAndActive);
			ownedCircle.SetActive(newState == ThemeState.OwnedAndInactive);
		}

		[UsedImplicitly]
		public void ButtonPressed()
		{
			if (newBuble != null && newBuble.activeSelf)
			{
				newBuble.SetActive(false);
			}
			OnPressEvent onPress__BackingField = OnPress__BackingField;
			if (onPress__BackingField != null)
			{
				onPress__BackingField(themeIndex);
			}
		}
	}
}
