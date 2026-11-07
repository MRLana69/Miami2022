using UnityEngine;

public class TrainRendererActivator : MonoBehaviour
{
	private Renderer[] renderers;

	public void Awake()
	{
		TrackObject trackObject = GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>();
		trackObject.OnActivate += OnActivate;
		trackObject.OnDeactivate += OnDeactivate;
		renderers = GetComponentsInChildren<Renderer>();
		OnDeactivate();
	}

	public void OnActivate()
	{
		Renderer[] array = renderers;
		foreach (Renderer renderer in array)
		{
			if (renderer != null)
			{
				renderer.enabled = true;
			}
		}
	}

	public void OnDeactivate()
	{
		Renderer[] array = renderers;
		foreach (Renderer renderer in array)
		{
			if (renderer != null)
			{
				renderer.enabled = false;
			}
		}
	}
}
