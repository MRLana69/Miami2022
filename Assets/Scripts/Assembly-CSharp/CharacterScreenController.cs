using System.Collections.Generic;
using UnityEngine;

public class CharacterScreenController : MonoBehaviour
{
	private float _shadowFadeFactor = 0.7f;

	private float _characterFadeFactor = 0.6f;

	private AnimationClip _idleClip;

	public float _freeze = 1f;

	public bool _freezing;

	public bool _defreezing;

	private bool _wasFreezing;

	private bool _wasDefreezing;

	private Animation _animation;

	public AnimationState _idleState;

	private Renderer[] _renderers;

	private List<Material> _materials = new List<Material>();

	private Material _shadowMaterial;

	private AvatarEyeAnimation _avatarEyeAnimation;

	private Color _overlayColor = new Color(1f, 1f, 1f);

	public float Freeze
	{
		set
		{
			SetFreeze(value);
		}
	}

	public void Initialize(AnimationClip idleClip, Shader characterShader, Shader outfitShader, float characterFadeFactor, float shadowFadeFactor)
	{
		_characterFadeFactor = characterFadeFactor;
		_shadowFadeFactor = shadowFadeFactor;
		_avatarEyeAnimation = base.gameObject.GetComponentInChildren<AvatarEyeAnimation>();
		_idleClip = idleClip;
		_animation = base.gameObject.GetComponentInChildren<Animation>();
		_animation.AddClip(_idleClip, _idleClip.name);
		_idleState = _animation[_idleClip.name];
		_idleState.enabled = true;
		_idleState.speed = 1f;
		_idleState.weight = 1f;
		_idleState.layer = 0;
		_idleState.blendMode = AnimationBlendMode.Blend;
		_idleState.wrapMode = WrapMode.Loop;
		_renderers = base.gameObject.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < _renderers.Length; i++)
		{
			Renderer renderer = _renderers[i];
			if (renderer.gameObject.name == "shadowPlane_blob")
			{
				_shadowMaterial = renderer.material;
				continue;
			}
			Material[] materials = renderer.materials;
			foreach (Material material in materials)
			{
				if (material.shader.name == "Custom/Distorted/Character seperated alpha")
				{
					material.shader = outfitShader;
				}
				else
				{
					material.shader = characterShader;
				}
				material.SetColor("_Color", _overlayColor);
				_materials.Add(material);
			}
		}
		SetFreeze(1f);
	}

	private void SetFreeze(float freeze)
	{
		float freeze2 = _freeze;
		_freeze = Mathf.Clamp01(freeze);
		_freezing = freeze2 < _freeze;
		_defreezing = _freeze < freeze2;
		bool flag = _freeze == 1f;
		if (flag && !_wasFreezing)
		{
			_idleState.enabled = true;
			_animation.Sample();
		}
		else if (!flag)
		{
			if (!_wasDefreezing && _defreezing)
			{
				_idleState.enabled = true;
				_idleState.wrapMode = WrapMode.Loop;
				_idleState.normalizedTime = 0f;
				_wasFreezing = false;
				if (_avatarEyeAnimation != null)
				{
					_avatarEyeAnimation.AnimateEyes(UIModelController.ModelScreen.LimitedCharacterPopup);
				}
			}
			else if (!_wasFreezing && _freezing)
			{
				_idleState.wrapMode = WrapMode.ClampForever;
				_idleState.normalizedTime %= 1f;
				_wasFreezing = true;
				if (_avatarEyeAnimation != null)
				{
					_avatarEyeAnimation.StopAnimatingEyes();
				}
			}
		}
		_idleState.enabled = !flag || _wasFreezing;
		_wasFreezing = _freezing;
		_wasDefreezing = _defreezing;
		for (int i = 0; i < _materials.Count; i++)
		{
			_materials[i].SetFloat("_ShiftColor", _freeze * _characterFadeFactor);
		}
		if (_shadowMaterial != null)
		{
			_shadowMaterial.SetFloat("_Fade", _freeze * _shadowFadeFactor);
		}
	}

	private void OnDestroy()
	{
		Object.Destroy(_shadowMaterial);
	}
}
