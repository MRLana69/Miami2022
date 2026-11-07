using System;
using UnityEngine;

public class SpeederFXMenu : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem[] _particleSystems;

	[SerializeField]
	private Renderer[] _renderes;

	[SerializeField]
	private TrailRenderer[] _trailRenderes;

	private void Awake()
	{
		if (_renderes.Length > 0)
		{
			Renderer[] renderes = _renderes;
			foreach (Renderer renderer in renderes)
			{
				renderer.enabled = false;
			}
		}
		if (_trailRenderes.Length > 0)
		{
			TrailRenderer[] trailRenderes = _trailRenderes;
			foreach (TrailRenderer trailRenderer in trailRenderes)
			{
				trailRenderer.enabled = false;
			}
		}
		HoverboardModelPreviewFactory instance = HoverboardModelPreviewFactory.Instance;
		instance.OnSpeederStart = (Action)Delegate.Combine(instance.OnSpeederStart, new Action(HandleOnSpeederStart));
		HoverboardModelPreviewFactory instance2 = HoverboardModelPreviewFactory.Instance;
		instance2.OnSpeederEnd = (Action)Delegate.Combine(instance2.OnSpeederEnd, new Action(HandleOnSpeederEnd));
	}

	private void OnDestroy()
	{
		if (HoverboardModelPreviewFactory.Instance != null)
		{
			HoverboardModelPreviewFactory instance = HoverboardModelPreviewFactory.Instance;
			instance.OnSpeederStart = (Action)Delegate.Remove(instance.OnSpeederStart, new Action(HandleOnSpeederStart));
			HoverboardModelPreviewFactory instance2 = HoverboardModelPreviewFactory.Instance;
			instance2.OnSpeederEnd = (Action)Delegate.Remove(instance2.OnSpeederEnd, new Action(HandleOnSpeederEnd));
		}
	}

	private void HandleOnSpeederStart()
	{
		if (_particleSystems.Length > 0)
		{
			ParticleSystem[] particleSystems = _particleSystems;
			foreach (ParticleSystem particleSystem in particleSystems)
			{
				particleSystem.gameObject.SetActive(false);
			}
		}
		if (_renderes.Length > 0)
		{
			Renderer[] renderes = _renderes;
			foreach (Renderer renderer in renderes)
			{
				renderer.enabled = true;
			}
		}
		if (_trailRenderes.Length > 0)
		{
			TrailRenderer[] trailRenderes = _trailRenderes;
			foreach (TrailRenderer trailRenderer in trailRenderes)
			{
				trailRenderer.enabled = true;
			}
		}
	}

	private void HandleOnSpeederEnd()
	{
		if (_particleSystems.Length > 0)
		{
			ParticleSystem[] particleSystems = _particleSystems;
			foreach (ParticleSystem particleSystem in particleSystems)
			{
				particleSystem.gameObject.SetActive(true);
			}
		}
		if (_renderes.Length > 0)
		{
			Renderer[] renderes = _renderes;
			foreach (Renderer renderer in renderes)
			{
				renderer.enabled = false;
			}
		}
		if (_trailRenderes.Length > 0)
		{
			TrailRenderer[] trailRenderes = _trailRenderes;
			foreach (TrailRenderer trailRenderer in trailRenderes)
			{
				trailRenderer.enabled = false;
			}
		}
	}
}
