using System.Collections.Generic;
using UnityEngine;

public class LetterPickup : MonoBehaviour
{
	public MeshFilter LetterMesh;

	public List<Mesh> Letters;

	public bool shouldSpawnParticles;

	protected char _letterChar;

	public const char NO_LETTER = '\0';

	private void Awake()
	{
		TrackObject trackObject = GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>();
		trackObject.OnActivate += OnActivate;
		trackObject.OnDeactivate += OnDeactivate;
	}

	protected virtual void OnActivate()
	{
	}

	protected virtual void OnDeactivate()
	{
	}
}
