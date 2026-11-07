using System;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace UI.Popups.WelcomeBackRewardPopupInternal
{
	public class WelcomeBackRewardButtonManager : MonoBehaviour
	{
		private Color _normalColor;

		private Color _pressedColor;

		[SerializeField]
		private UISprite _buttonFill;

		[SerializeField]
		private UISprite _buttonHighlightFill;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action OnClicked__BackingField;

		public event Action OnClicked
		{
			add
			{
				Action action = OnClicked__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnClicked__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnClicked__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnClicked__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public void SetButtonColors(Color normalColor, Color pressedColor)
		{
			_normalColor = normalColor;
			_pressedColor = pressedColor;
		}

		[UsedImplicitly]
		private void OnPress(bool pressed)
		{
			ChangeColor(GetButtonColor(pressed));
		}

		private Color GetButtonColor(bool pressed)
		{
			if (pressed)
			{
				return _pressedColor;
			}
			return _normalColor;
		}

		private void ChangeColor(Color newColor)
		{
			_buttonFill.color = newColor;
			_buttonHighlightFill.color = newColor;
		}

		[UsedImplicitly]
		private void OnClick()
		{
			OnClicked__BackingField.SafeInvoke();
		}
	}
}
