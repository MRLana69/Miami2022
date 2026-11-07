using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using UI;
using UnityEngine;

[ExecuteInEditMode]
public class DoubleCoinsButton : MonoBehaviour
{
	private bool _owned;

	[SerializeField]
	private UILabel _doubleCoinsLabel;

	[SerializeField]
	private Color _notOwnedLabelColor;

	[SerializeField]
	private bool _switch;

	[SerializeField]
	private UISprite _overlaySprite;

	[SerializeField]
	private bool _notOwnedOverlayActive;

	[SerializeField]
	private bool _ownedOverlayActive;

	[SerializeField]
	private Color _notOwnedOverlayColor;

	[SerializeField]
	private Color _ownedOverlayColor;

	[SerializeField]
	private UISprite _backgroundSprite;

	[SerializeField]
	private Color _notOwnedBackgroundColor;

	[SerializeField]
	private Color _ownedBackgroundColor;

	[SerializeField]
	private Animation _animation;

	[SerializeField]
	private GameObject _animationSprite;

	[SerializeField]
	private bool _notOwnedAnimationActive;

	[SerializeField]
	private bool _ownedAnimationActive;

	[SerializeField]
	private GameObject _ownedCheck;

	[SerializeField]
	private Collider _collider;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action onClicked__BackingField;

	public event Action onClicked
	{
		add
		{
			Action action = onClicked__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onClicked__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onClicked__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onClicked__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private void Awake()
	{
		_doubleCoinsLabel.text = Strings.Get("DOUBLE_COINS_TITLE");
	}

	public void ChangeState(bool owned)
	{
		_ownedCheck.SetActive(owned);
		if (owned)
		{
			if ((bool)_doubleCoinsLabel)
			{
				_doubleCoinsLabel.color = ColorData.instance.sun;
			}
			if ((bool)_overlaySprite)
			{
				_overlaySprite.enabled = _ownedOverlayActive;
				_overlaySprite.color = _ownedOverlayColor;
				_overlaySprite.enabled = false;
			}
			if ((bool)_backgroundSprite)
			{
				_backgroundSprite.color = _ownedBackgroundColor;
				_backgroundSprite.enabled = false;
			}
			if ((bool)_animation)
			{
				_animation.enabled = _ownedAnimationActive;
			}
			if ((bool)_animationSprite)
			{
				_animationSprite.SetActive(_ownedAnimationActive && !owned);
			}
			if ((bool)_collider)
			{
				_collider.enabled = false;
			}
		}
		else
		{
			if ((bool)_doubleCoinsLabel)
			{
				_doubleCoinsLabel.color = _notOwnedLabelColor;
			}
			if ((bool)_overlaySprite)
			{
				_overlaySprite.enabled = _notOwnedOverlayActive;
				_overlaySprite.color = _notOwnedOverlayColor;
			}
			if ((bool)_backgroundSprite)
			{
				_backgroundSprite.color = _notOwnedBackgroundColor;
			}
			if ((bool)_animation)
			{
				_animation.enabled = _notOwnedAnimationActive;
			}
			if ((bool)_animationSprite)
			{
				_animationSprite.SetActive(_notOwnedAnimationActive);
			}
		}
	}

	public void OnClick()
	{
		onClicked__BackingField.SafeInvoke();
	}

	private void Update()
	{
		if (_switch)
		{
			_switch = !_switch;
			_owned = !_owned;
			ChangeState(_owned);
		}
	}
}
