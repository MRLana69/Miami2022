using SYBO.Subway.Characters;
using UnityEngine;

namespace SYBO.Subway.Visuals
{
	public class FallingSnowThemeEffect : ThemeEffect
	{
		[SerializeField]
		private ParticleSystem _snowPrefab;

		private ParticleSystem _snow;

		[SerializeField]
		private float zOffset;

		private Vector3 _snowOriginalPosition = Vector3.zero;

		public override void Initialize(Transform root)
		{
			_snow = Object.Instantiate(_snowPrefab);
			_snow.transform.SetParent(root, true);
			_snow.gameObject.SetActive(true);
			_snowOriginalPosition = _snow.transform.position;
			Game.OnGameStarted += Game_OnGameStarted;
			Game.OnGameEnded += Game_OnGameEnded;
		}

		private void Game_OnGameStarted()
		{
			if (_snow != null)
			{
				_snow.Clear();
				_snow.Play();
			}
		}

		private void Game_OnGameEnded()
		{
			if (_snow != null)
			{
				_snow.Stop();
			}
		}

		public override void UpdateEffect()
		{
			_snow.transform.position = new Vector3(_snowOriginalPosition.x, _snowOriginalPosition.y, Character.Instance.TransformPosition.z + zOffset);
		}
	}
}
