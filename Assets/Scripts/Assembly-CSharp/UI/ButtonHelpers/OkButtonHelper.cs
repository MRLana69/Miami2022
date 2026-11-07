using System;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;

namespace UI.ButtonHelpers
{
	public class OkButtonHelper : MonoBehaviour
	{
		[SerializeField]
		private UILabel _okButtonLabel;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action OkButtonClicked__BackingField;

		public event Action OkButtonClicked
		{
			add
			{
				Action action = OkButtonClicked__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OkButtonClicked__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OkButtonClicked__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OkButtonClicked__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		private void OnEnable()
		{
			_okButtonLabel.text = Strings.Get("OK");
		}

		[UsedImplicitly]
		private void OnClick()
		{
			if (OkButtonClicked__BackingField != null)
			{
				OkButtonClicked__BackingField();
			}
		}
	}
}
