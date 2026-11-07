using System;
using SYBO.Subway.Characters;
using UnityEngine;

public class playThruster : MonoBehaviour
{
	public enum JumpStyle
	{
		Super = 0,
		Double = 1
	}

	private Hoverboard _hoverboard;

	[SerializeField]
	private Renderer[] _renderes;

	[SerializeField]
	private AnimationClip _clip;

	[SerializeField]
	private ParticleSystem _particles;

	[SerializeField]
	private float thrusterPlayTime;

	[SerializeField]
	private JumpStyle _jumpStyle;

	private AnimationState[] _states;

	private Character _character;

	private HoverboardModelPreviewFactory _factory;

	private bool addedEvent;

	private bool addedMenuEvent;

	public JumpStyle jumpStyle
	{
		get
		{
			return _jumpStyle;
		}
	}

	public void Initialize()
	{
		_hoverboard = Hoverboard.Instance;
		if (_renderes != null)
		{
			_states = new AnimationState[_renderes.Length];
			for (int i = 0; i < _renderes.Length; i++)
			{
				_states[i] = _renderes[i].GetComponent<Animation>()[_clip.name];
				_states[i].weight = 1f;
				_states[i].enabled = true;
				_renderes[i].enabled = false;
			}
		}
		_character = Character.Instance;
		if (_jumpStyle == JumpStyle.Super)
		{
			addedEvent = true;
			_character.OnJump += Character_OnJump;
		}
		if (_jumpStyle == JumpStyle.Double)
		{
			addedEvent = true;
			_character.OnDoubleJump += Character_OnDoubleJump;
		}
		if (_particles != null)
		{
			_particles.transform.parent = null;
		}
	}

	public void InitializeMenu(HoverboardModelPreviewFactory factory)
	{
		if (!addedMenuEvent)
		{
			Initialize();
			addedMenuEvent = true;
			_factory = factory;
			HoverboardModelPreviewFactory factory2 = _factory;
			factory2.OnJump = (Action)Delegate.Combine(factory2.OnJump, new Action(Factory_OnJump));
		}
	}

	private void OnEnable()
	{
		if (_character == null)
		{
			return;
		}
		if (!addedEvent)
		{
			if (_jumpStyle == JumpStyle.Super)
			{
				_character.OnJump += Character_OnJump;
			}
			if (_jumpStyle == JumpStyle.Double)
			{
				_character.OnDoubleJump += Character_OnDoubleJump;
			}
			addedEvent = true;
		}
		if (!addedMenuEvent)
		{
			HoverboardModelPreviewFactory factory = _factory;
			factory.OnJump = (Action)Delegate.Combine(factory.OnJump, new Action(Factory_OnJump));
			addedMenuEvent = true;
		}
	}

	private void OnDisable()
	{
		if (addedEvent)
		{
			if (_jumpStyle == JumpStyle.Super)
			{
				_character.OnJump -= Character_OnJump;
			}
			if (_jumpStyle == JumpStyle.Double)
			{
				_character.OnDoubleJump -= Character_OnDoubleJump;
			}
			addedEvent = false;
		}
		if (addedMenuEvent)
		{
			HoverboardModelPreviewFactory factory = _factory;
			factory.OnJump = (Action)Delegate.Remove(factory.OnJump, new Action(Factory_OnJump));
			addedMenuEvent = false;
		}
	}

	private void OnDestroy()
	{
		if (_particles != null)
		{
			UnityEngine.Object.Destroy(_particles.gameObject);
		}
	}

	private void Factory_OnJump()
	{
		PlayEffects(false);
	}

	private void Character_OnDoubleJump()
	{
		Character_OnJump(0f, false);
	}

	private void Character_OnJump(float jumpHeight, bool willPlaySound)
	{
		if (_hoverboard.isActive)
		{
			PlayEffects();
		}
	}

	private void PlayEffects(bool playParticleEffect = true)
	{
		if (_renderes != null)
		{
			Renderer[] renderes = _renderes;
			foreach (Renderer renderer in renderes)
			{
				renderer.enabled = true;
			}
			for (int j = 0; j < _states.Length; j++)
			{
				_states[j].weight = 1f;
				_states[j].enabled = true;
			}
			StartCoroutine(pTween.To(thrusterPlayTime, (float t) =>
			{
				AnimationState[] states = _states;
				foreach (AnimationState animationState in states)
				{
					animationState.normalizedTime = t;
				}
				if (t == 1f)
				{
					Renderer[] renderes2 = _renderes;
					foreach (Renderer renderer2 in renderes2)
					{
						renderer2.enabled = false;
					}
				}
			}));
		}
		if (_particles != null && playParticleEffect && base.enabled)
		{
			_particles.transform.localRotation = Quaternion.Euler(new Vector3(270f, 0f, 0f));
			_particles.transform.position = base.transform.position;
			float offset = 0f;
			StartCoroutine(pTween.To(thrusterPlayTime, (float t) =>
			{
				offset += Time.deltaTime * 20f;
				Vector3 position = new Vector3(_particles.transform.position.x, _particles.transform.position.y, base.transform.position.z - offset);
				_particles.transform.position = position;
			}));
			_particles.Play();
		}
	}
}
