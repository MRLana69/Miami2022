using System;
using UnityEngine;

public class SuperMysteryBoxEffect : MonoBehaviour
{
	private const string GLOW_MATERIAL_COLOR_NAME = "_TintColor";

	private const float GLOW_ANI_SPEED = 0.5f;

	public ParticleSystem[] particleSystems = new ParticleSystem[0];

	public Renderer[] glowRenderers = new Renderer[0];

	private float _glowAniFactor;

	private bool _glowShouldBeOn;

	private Color[] _glowBaseColors;

	private Material[] _glowMaterials;

	public bool isRendering { get; set; }

	private void Awake()
	{
		isRendering = false;
	}

	private void Start()
	{
		ParticleSystem[] array = particleSystems;
		foreach (ParticleSystem particleSystem in array)
		{
			ParticleSystem.MainModule main = particleSystem.main;
			main.playOnAwake = false;
		}
		_glowMaterials = new Material[glowRenderers.Length];
		_glowBaseColors = new Color[glowRenderers.Length];
		for (int j = 0; j < glowRenderers.Length; j++)
		{
			_glowMaterials[j] = glowRenderers[j].material;
			_glowBaseColors[j] = _glowMaterials[j].GetColor("_TintColor");
		}
		ApplyGlowAniFactor();
	}

	private void Update()
	{
		if (_glowShouldBeOn && _glowAniFactor < 1f)
		{
			_glowAniFactor = Mathf.Clamp01(_glowAniFactor + 0.5f * Time.deltaTime);
			ApplyGlowAniFactor();
		}
		else if (!_glowShouldBeOn && _glowAniFactor > 0f)
		{
			_glowAniFactor = Mathf.Clamp01(_glowAniFactor - 0.5f * Time.deltaTime);
			ApplyGlowAniFactor();
		}
	}

	public void StartEffect()
	{
		if (!isRendering)
		{
			isRendering = true;
			_glowShouldBeOn = true;
			ParticleSystem[] array = particleSystems;
			foreach (ParticleSystem particleSystem in array)
			{
				particleSystem.Play();
			}
		}
	}

	public void StopEffect()
	{
		isRendering = false;
		_glowShouldBeOn = false;
		ParticleSystem[] array = particleSystems;
		foreach (ParticleSystem particleSystem in array)
		{
			particleSystem.Stop();
		}
	}

	public void SetVisible(bool visible)
	{
		base.gameObject.SetActive(visible);
	}

	public void FastForwardEffect(int time)
	{
		ParticleSystem[] array = particleSystems;
		foreach (ParticleSystem particleSystem in array)
		{
			particleSystem.Simulate(time);
		}
	}

	private void ApplyGlowAniFactor()
	{
		float num = 0.5f + 0.5f * Mathf.Cos((float)Math.PI + (float)Math.PI * _glowAniFactor);
		for (int i = 0; i < _glowMaterials.Length; i++)
		{
			_glowMaterials[i].SetColor("_TintColor", _glowBaseColors[i] * num);
		}
	}
}
