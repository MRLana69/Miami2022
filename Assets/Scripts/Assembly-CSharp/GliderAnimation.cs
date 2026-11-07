using System;
using SYBO.Subway.Characters;
using UnityEngine;

public class GliderAnimation : MonoBehaviour
{
	private Animation _animation;

	[SerializeField]
	private AnimationClip _clipClose;

	[SerializeField]
	private AnimationClip _clipOpen;

	private AnimationState _stateClose;

	private AnimationState _stateOpen;

	private Character _character;

	private HoverboardModelPreviewFactory _factory;

	private bool _open;

	private void Awake()
	{
		_character = Character.Instance;
		_animation = GetComponent<Animation>();
		_stateClose = GetComponent<Animation>()[_clipClose.name];
		_stateClose.weight = 1f;
		_stateClose.time = 1f;
		_stateClose.speed = 0f;
		_stateClose.enabled = true;
		_stateOpen = GetComponent<Animation>()[_clipOpen.name];
	}

	private void HandleOnSwitchToHoverboard(HoverboardBundle hoverbooardBundle, Hoverboard.BoardAbility[] abilities)
	{
		if (!_character.IsGrounded.Value)
		{
			HandleOnHangtime();
		}
	}

	public void AddEvents()
	{
		_character.OnHangtime += HandleOnHangtime;
		_character.OnLanding += HandleOnLanding;
		Hoverboard.Instance.OnSwitchToHoverboard += HandleOnSwitchToHoverboard;
	}

	public void AddEvents(HoverboardModelPreviewFactory factory)
	{
		_factory = factory;
		HoverboardModelPreviewFactory factory2 = _factory;
		factory2.OnOpenGlider = (Action)Delegate.Combine(factory2.OnOpenGlider, new Action(HandleOnOpenMenu));
		HoverboardModelPreviewFactory factory3 = _factory;
		factory3.OnCloseGlider = (Action)Delegate.Combine(factory3.OnCloseGlider, new Action(HandleOnCloseMenu));
	}

	private void OnDestroy()
	{
		_character.OnHangtime -= HandleOnHangtime;
		_character.OnLanding -= HandleOnLanding;
		if (_factory != null)
		{
			HoverboardModelPreviewFactory factory = _factory;
			factory.OnOpenGlider = (Action)Delegate.Remove(factory.OnOpenGlider, new Action(HandleOnOpenMenu));
			HoverboardModelPreviewFactory factory2 = _factory;
			factory2.OnCloseGlider = (Action)Delegate.Remove(factory2.OnCloseGlider, new Action(HandleOnCloseMenu));
		}
		Hoverboard.Instance.OnSwitchToHoverboard -= HandleOnSwitchToHoverboard;
	}

	private void HandleOnLanding(Character character)
	{
		HandleOnClose();
	}

	private void HandleOnClose()
	{
		if (_open)
		{
			_stateClose.speed = 1f;
			if (_animation != null)
			{
				_animation.Play(_clipClose.name);
			}
			_open = false;
		}
	}

	private void HandleOnHangtime()
	{
		if (!_open && !_character.IsGrounded.Value)
		{
			_open = true;
			if (_animation != null)
			{
				_animation.Play(_clipOpen.name);
			}
		}
	}

	private void HandleOnCloseMenu()
	{
		_stateClose.speed = 1f;
		_stateClose.normalizedTime = 0f;
		_stateClose.weight = 1f;
		if (_animation != null)
		{
			_animation.Play(_clipClose.name);
		}
		_open = false;
	}

	private void HandleOnOpenMenu()
	{
		if (_animation == null)
		{
			if (_factory != null)
			{
				HoverboardModelPreviewFactory factory = _factory;
				factory.OnOpenGlider = (Action)Delegate.Remove(factory.OnOpenGlider, new Action(HandleOnOpenMenu));
				HoverboardModelPreviewFactory factory2 = _factory;
				factory2.OnCloseGlider = (Action)Delegate.Remove(factory2.OnCloseGlider, new Action(HandleOnCloseMenu));
			}
		}
		else
		{
			_stateOpen.normalizedTime = 0f;
			_stateClose.weight = 0f;
			_stateClose.normalizedTime = 0f;
			if (_animation != null)
			{
				_animation.Play(_clipOpen.name);
			}
		}
	}

	public void RemoveEventHandling()
	{
		_character.OnHangtime -= HandleOnHangtime;
		_character.OnLanding -= HandleOnLanding;
	}
}
