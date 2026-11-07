using SYBO.Subway;
using UnityEngine;

public class SetParticleZVelo : MonoBehaviour
{
	[SerializeField]
	private float _speedMultiplier;

	private ParticleSystem _particleSystem;

	private Game _game;

	private void Awake()
	{
		_particleSystem = base.gameObject.GetComponent<ParticleSystem>();
		_game = Game.Instance;
	}

	private void Update()
	{
		base.transform.rotation = Quaternion.identity;
		if (_speedMultiplier > 0f)
		{
			ParticleSystem.MainModule main = _particleSystem.main;
			main.startSpeed = _game.CurrentSpeed * _speedMultiplier;
		}
	}
}
