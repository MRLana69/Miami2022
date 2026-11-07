using System.Collections;
using System.Collections.Generic;
using Info;
using Kiloo.Common.Utility;
using SYBO.Common.StateSystem;
using SYBO.Components.Utilities;
using SYBO.Subway.Coins;
using SYBO.Subway.Level;
using SYBO.Subway.Routes;
using SYBO.Subway.StateSystem.States.Settings;
using UnityEngine;

namespace SYBO.Subway.StateSystem.States
{
	public class JetpackState : FlyingState
	{
		private const float CoinStayInTrackDistance = 60f;

		private const float CoinSpacingInZ = 30f;

		private JetpackStateSettings _settings;

		private List<Coin> _dynamicSpawnedCoins = new List<Coin>();

		protected override UpgradeType UpgradeType
		{
			get
			{
				return UpgradeType.jetpack;
			}
		}

		public JetpackState(int id)
			: base(id)
		{
			_settings = ScriptableSingleton<JetpackStateSettings>.instance;
		}

		public override void Enter(State fromState)
		{
			base.Enter(fromState);
			GameStats.Instance.PickedUpPowerupsIncrement();
			float startZ = _character.TransformPosition.z + _flyAheadDistance + _settings.CoinOffset;
			float worldPositionZ = GamePositionRoot.Instance.GetWorldPositionZ(_targetGamePositionZ);
			SpawnCoins(startZ, worldPositionZ, base.FlyingStateSettings.FlyHeight);
		}

		public override void Leave(State toState)
		{
			base.Leave(toState);
			if (toState is MenuState)
			{
				DespawnCoins();
			}
		}

		private void SpawnCoins(float startZ, float endZ, float height)
		{
			AnimationCurve animationCurve = new AnimationCurve();
			int num = 1;
			for (float num2 = startZ; num2 < endZ; num2 += base.FlyingStateSettings.CharacterChangeTrackLength + 60f)
			{
				animationCurve.AddKey(new Keyframe(num2, Route.GetTrackX(num)));
				animationCurve.AddKey(new Keyframe(num2 + 60f, Route.GetTrackX(num)));
				num = Mathf.Clamp(num + Random.Range(-1, 2), 0, 2);
				animationCurve.AddKey(new Keyframe(num2 + 60f + base.FlyingStateSettings.CharacterChangeTrackLength, Route.GetTrackX(num)));
			}
			CoroutineUtil.StartCoroutine(SpawnCoinCurve(animationCurve, startZ, endZ, height));
		}

		private IEnumerator SpawnCoinCurve(AnimationCurve curve, float startZ, float endZ, float height)
		{
			CoinManager coinManager = SingletonMonoBehaviour<CoinManager>.Instance;
			float z = startZ;
			while (z < endZ)
			{
				Vector3 p = Vector3.up * height + Route.GetPosition(curve.Evaluate(z), z);
				Coin coin = coinManager.SpawnCoin(p);
				_dynamicSpawnedCoins.Add(coin);
				z += 30f;
				yield return null;
			}
		}

		private void DespawnCoins()
		{
			for (int i = 0; i < _dynamicSpawnedCoins.Count; i++)
			{
				if (SingletonMonoBehaviour<CoinManager>.Instance.IsSpawned(_dynamicSpawnedCoins[i]))
				{
					_dynamicSpawnedCoins[i].Deactivate();
				}
			}
			_dynamicSpawnedCoins.Clear();
		}

		protected override float GetFlyingSpeed()
		{
			return _game.CurrentLevelSpeed * _settings.Speedup;
		}
	}
}
