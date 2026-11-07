using UnityEngine;

public class RendererActivator : MonoBehaviour
{
	public void Awake()
	{
		TrackObject trackObject = GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>();
		trackObject.OnActivate += OnActivate;
		trackObject.OnDeactivate += OnDeactivate;
		GetComponent<Renderer>().enabled = false;
	}

	public void OnActivate()
	{
		GetComponent<Renderer>().enabled = true;
	}

	public void OnDeactivate()
	{
		GetComponent<Renderer>().enabled = false;
	}
}
