using SYBO.Subway;
using SYBO.Subway.Cameras;
using UnityEngine;

public class Tunnel : MonoBehaviour
{
	[SerializeField]
	private float _tunnelLength;

	private Game _game;

	public AudioStateLoop audioStateLoop;

	private void Awake()
	{
		Game.Instance.OnStageMenuSequence += () =>
		{
			CharacterCamera.Instance.ExitCameraModifierCollider();
		};
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.CompareTag("Player"))
		{
			CharacterCamera.Instance.EnterCameraModifierCollider(_tunnelLength);
		}
	}
}
