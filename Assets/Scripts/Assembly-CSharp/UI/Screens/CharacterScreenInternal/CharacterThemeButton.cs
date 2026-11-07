using System;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace UI.Screens.CharacterScreenInternal
{
	public class CharacterThemeButton : MonoBehaviour
	{
		[SerializeField]
		private UISprite _background;

		[SerializeField]
		private UISprite _icon;

		[SerializeField]
		private GameObject _giftBubble;

		[SerializeField]
		private GameObject _newBubble;

		[SerializeField]
		private GameObject _lockIcon;

		[SerializeField]
		private UISprite _limitedGiftBubble;

		[SerializeField]
		private UISprite _limitedGiftIcon;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<int> OnPress__BackingField;

		public int themeIndex { get; private set; }

		public event Action<int> OnPress
		{
			add
			{
				Action<int> action = OnPress__BackingField;
				Action<int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPress__BackingField, (Action<int>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<int> action = OnPress__BackingField;
				Action<int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPress__BackingField, (Action<int>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public void SetOutfitColors(Color32 bgColor, Color32 iconColor, int index)
		{
			_background.color = bgColor;
			_icon.color = iconColor;
			themeIndex = index;
		}

		public void SetActiveBubbles(bool enableGiftBubble, bool enableNewBubble, bool enableLimitedGiftBubble)
		{
			if (_giftBubble != null)
			{
				_giftBubble.SetActive(enableGiftBubble);
			}
			if (_newBubble != null)
			{
				_newBubble.SetActive(enableNewBubble);
			}
			if (_limitedGiftBubble != null)
			{
				_limitedGiftBubble.enabled = enableLimitedGiftBubble;
				_limitedGiftBubble.color = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;
			}
			if (_limitedGiftIcon != null)
			{
				_limitedGiftIcon.enabled = enableLimitedGiftBubble;
				_limitedGiftIcon.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
			}
		}

		public void SetLockIconEnabled(bool state)
		{
			if (_lockIcon != null)
			{
				_lockIcon.SetActive(state);
			}
		}

		[UsedImplicitly]
		public void ButtonPressed()
		{
			if (_newBubble != null && _newBubble.activeSelf)
			{
				_newBubble.SetActive(false);
			}
			OnPress__BackingField.SafeInvoke(themeIndex);
		}
	}
}
