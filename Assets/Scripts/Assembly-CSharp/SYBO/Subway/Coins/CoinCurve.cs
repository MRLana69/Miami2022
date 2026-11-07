using System.Collections.Generic;
using SYBO.Components.Utilities;
using SYBO.Subway.Characters;
using SYBO.Subway.Level;
using SYBO.Subway.StateSystem;
using SYBO.Subway.StateSystem.States;
using UnityEngine;

namespace SYBO.Subway.Coins
{
	[RequireComponent(typeof(TrackObject))]
	public class CoinCurve : MonoBehaviour
	{
		public enum CoinSpaceMode
		{
			NonStraight = 0,
			Straight = 1
		}

		public enum ScaleMode
		{
			NoScale = 0,
			PreserveSpacing = 1,
			ScaleSpacing = 2
		}

		public const float TOP_LEVEL_HEIGHT_THRESHOLD = 70f;

		public const float StraightSpacing = 30f;

		public const float NonStraightSpacing = 15f;

		[SerializeField]
		private CurveParent _curveParent;

		[SerializeField]
		[Tooltip("Build the curve for the lowest speed (110)\nFor a level jump, this is a length of 98")]
		private ScaleMode _scaleMode;

		[SerializeField]
		[Tooltip("30 units for straight lines, 15 for jump curves and lane changes")]
		private CoinSpaceMode _spacingMode;

		[SerializeField]
		[Tooltip("The time value the curve centers around")]
		private float _curveOffset;

		[SerializeField]
		[Tooltip("Setting this to an index that would contain a coin, will prevent the spawn of that coin.\nIndex starts from zero")]
		private int _coinIndexToSkip = -1;

		[SerializeField]
		[Tooltip("If this is true, when the speed changes and the curve is scaled, the amount of coins will remain the same and simply  be redistributed along the length of the curve.")]
		private bool _redistributeAlongCurveOnSpeedChange = true;

		[SerializeField]
		private bool _debugIsMovingTrain;

		private CoinManager _coinManager;

		private Character _character;

		private TrackObject _trackObject;

		private List<Coin> _placedCoins;

		private Game _game;

		private bool _allCoinsActive;

		private int _coinsAtSpawn;

		private bool _activating;

		private int lastSpawnedCoinIndex;

		private float SpeedForCurve
		{
			get
			{
				if (_game != null)
				{
					return (!Application.isPlaying) ? _game.speed.min : _game.CurrentLevelSpeed;
				}
				return 110f;
			}
		}

		private float UnscaledCoinSpacing
		{
			get
			{
				return (_spacingMode != CoinSpaceMode.Straight) ? 15f : 30f;
			}
		}

		private bool IsOnTopLevel
		{
			get
			{
				return base.gameObject != null && base.transform.position.y > 70f;
			}
		}

		protected void Awake()
		{
			_game = Game.Instance;
			_curveParent.ForceLinearZ(true);
			_placedCoins = new List<Coin>(NumberOfCoins(_game.speed.max));
			_coinManager = SingletonMonoBehaviour<CoinManager>.Instance;
			_trackObject = GetComponent<TrackObject>();
			_trackObject.OnActivate += TrackObject_OnActivate;
			_trackObject.OnDeactivate += TrackObject_OnDeactivate;
			_trackObject.PropagateActivationStateToChildren = true;
			_character = Character.Instance;
			_game.RegisterSpeedChangeHandler(SpeedChanged);
			GameStateMachine.Instance.GetState<PogostickState>(5).RegisterStartHandler(PogostickStart);
			GameStateMachine.Instance.GetState<HeadstartState>(3).RegisterStartHandler(JetpackStart);
			GameStateMachine.Instance.GetState<JetpackState>(4).RegisterStartHandler(JetpackStart);
		}

		protected void OnValidate()
		{
			_curveParent.ForceLinearZ(true);
		}

		private void SpeedChanged(float newSpeed)
		{
			if (_trackObject.Active && _scaleMode != ScaleMode.NoScale)
			{
				Game_OnSpeedChanged(newSpeed);
			}
		}

		private void PogostickStart()
		{
			if (_trackObject.Active)
			{
				PogoStick_OnStart();
			}
		}

		private void JetpackStart()
		{
			if (_trackObject.Active)
			{
				Jetpack_OnStart();
			}
		}

		private Vector3 GetCoinPosition(int index)
		{
			return GetCoinPosition(index, SpeedForCurve);
		}

		private Vector3 GetCoinPosition(int index, float speed, bool speedChanged = false)
		{
			if (_curveParent == null || NumberOfCoinsIgnoringSkip(speed) < 2)
			{
				return base.transform.position;
			}
			float t = ((!_redistributeAlongCurveOnSpeedChange || !speedChanged) ? ((float)index * CoinSpacing(speed) / LengthOfCurve(speed)) : ((float)index / (float)(_coinsAtSpawn - 1)));
			return EvaluateCurve(t, speed);
		}

		private Vector3 EvaluateCurve(float t)
		{
			return EvaluateCurve(t, SpeedForCurve);
		}

		private Vector3 EvaluateCurve(float t, float speed)
		{
			float z = _curveParent.GetWorldPositionForTime(_curveParent.MinTime).z;
			float z2 = _curveParent.GetWorldPositionForTime(_curveParent.MaxTime).z;
			float z3 = z + (z2 - z) * t;
			float timeForWorldZ = _curveParent.GetTimeForWorldZ(z3, true);
			Vector3 worldPositionForTime = _curveParent.GetWorldPositionForTime(timeForWorldZ, true);
			float num = ScaleZ(speed, (z2 - z) * (t - _curveOffset));
			worldPositionForTime.z = z + num;
			return worldPositionForTime;
		}

		private void RepositionCoins(float speed)
		{
			if (!_redistributeAlongCurveOnSpeedChange)
			{
				ResizeIfNeeded(speed);
			}
			int num = _coinsAtSpawn - _placedCoins.Count;
			for (int i = 0; i < _placedCoins.Count; i++)
			{
				int num2 = i + num;
				int index = ((!IsSkippingCoin(speed) || i <= _coinIndexToSkip) ? i : (i - 1));
				if (num2 != _coinIndexToSkip)
				{
					_placedCoins[index].transform.position = GetCoinPosition(num2, speed, true);
				}
			}
		}

		private void ResizeIfNeeded(float speed)
		{
			if (!_allCoinsActive)
			{
				return;
			}
			int num = NumberOfCoins(speed) - _placedCoins.Count;
			for (int i = 0; i < Mathf.Abs(num); i++)
			{
				if (num < 0)
				{
					_placedCoins[0].Deactivate();
					CoinStats.New.NotifySpawnPointDeactivated(num);
				}
				else
				{
					Vector3 coinPosition = GetCoinPosition(_placedCoins.Count + i);
					SpawnCoin(coinPosition);
					CoinStats.New.NotifySpawnPointActivated(num);
				}
			}
			_coinsAtSpawn += num;
		}

		private Coin SpawnCoin(Vector3 spawnPos)
		{
			Coin coin = _coinManager.SpawnCoin(spawnPos, base.transform);
			coin.SetCurve(this);
			_placedCoins.Add(coin);
			return coin;
		}

		private float ScaleZ(float speed, float z)
		{
			float num = ((_scaleMode != ScaleMode.NoScale) ? (speed / 110f) : 1f);
			return num * z;
		}

		private float JumpLength(float speed)
		{
			if (_character == null)
			{
				_character = Character.Instance;
			}
			return _character.JumpLength(speed, _character.jumpHeightNormal);
		}

		private float LengthOfCurve(float speed)
		{
			float z = _curveParent.GetLocalPositionForTime(1f).z - _curveParent.GetLocalPositionForTime(0f).z;
			return ScaleZ(speed, z);
		}

		private float CoinSpacing(float speed)
		{
			return (_scaleMode != ScaleMode.ScaleSpacing) ? UnscaledCoinSpacing : ScaleZ(speed, UnscaledCoinSpacing);
		}

		private int NumberOfCoinsIgnoringSkip(float speed)
		{
			float f = LengthOfCurve(speed);
			float num = CoinSpacing(speed);
			return 1 + Mathf.FloorToInt(Mathf.Abs(f) / num);
		}

		private int NumberOfCoins(float speed)
		{
			return NumberOfCoinsIgnoringSkip(speed) - (IsSkippingCoin(speed) ? 1 : 0);
		}

		private bool IsSkippingCoin(float speed)
		{
			return 0 <= _coinIndexToSkip && _coinIndexToSkip < NumberOfCoinsIgnoringSkip(speed);
		}

		private void ToggleRenderers(bool doRender)
		{
			foreach (Coin placedCoin in _placedCoins)
			{
				placedCoin.ToggleRenderer(doRender);
			}
		}

		private void TrackObject_OnActivate()
		{
			_coinsAtSpawn = 0;
			if (IsZAvailableToJetpackState())
			{
				_coinsAtSpawn = NumberOfCoinsIgnoringSkip(SpeedForCurve);
				lastSpawnedCoinIndex = -1;
				_activating = true;
			}
		}

		private void ActivateOneCoinUtilDone()
		{
			lastSpawnedCoinIndex++;
			if (lastSpawnedCoinIndex < _coinsAtSpawn)
			{
				if (lastSpawnedCoinIndex == _coinIndexToSkip)
				{
					return;
				}
				Vector3 coinPosition = GetCoinPosition(lastSpawnedCoinIndex);
				SpawnCoin(coinPosition);
			}
			if (_coinsAtSpawn - 1 <= lastSpawnedCoinIndex)
			{
				CoinStats.New.NotifySpawnPointActivated(_coinsAtSpawn);
				if (IsOnTopLevel)
				{
					bool doRender = _character.GamePosition.y > 70f && !_game.IsInJetpackMode;
					ToggleRenderers(doRender);
				}
				_allCoinsActive = true;
				_activating = false;
			}
		}

		private void Update()
		{
			if (_activating)
			{
				ActivateOneCoinUtilDone();
			}
		}

		protected bool IsZAvailableToJetpackState()
		{
			FlyingState flyingState = GameStateMachine.Instance.TryGetCurrentState<FlyingState>();
			return flyingState == null || GamePositionRoot.Instance.GetGamePosition(base.transform).z > (float)flyingState.TargetGamePositionZ;
		}

		private void TrackObject_OnDeactivate()
		{
			CoinStats.New.NotifySpawnPointDeactivated(_coinsAtSpawn);
		}

		public void DeactivateCoin(Coin coin)
		{
			_allCoinsActive = false;
			_activating = false;
			_placedCoins.Remove(coin);
		}

		private void Game_OnSpeedChanged(float newSpeed)
		{
			_redistributeAlongCurveOnSpeedChange = true;
			RepositionCoins(newSpeed);
		}

		private void Jetpack_OnStart()
		{
			if (IsOnTopLevel)
			{
				_trackObject.Deactivate();
			}
		}

		private void PogoStick_OnStart()
		{
			if (IsOnTopLevel)
			{
				ToggleRenderers(true);
			}
		}

		public void OnDrawGizmos()
		{
			if (Application.isPlaying)
			{
				return;
			}
			if (_game == null)
			{
				_game = Game.Instance;
			}
			if (_game == null)
			{
				return;
			}
			Vector3 zero = Vector3.zero;
			Gizmos.color = Color.red;
			for (int i = 0; i < NumberOfCoinsIgnoringSkip(_game.speed.min); i++)
			{
				if (i != _coinIndexToSkip)
				{
					Vector3 coinPosition = GetCoinPosition(i);
					Gizmos.DrawWireSphere(coinPosition + zero, 1.5f);
					Gizmos.DrawRay(coinPosition + zero, Vector3.left * 5f);
				}
			}
			if (_scaleMode != ScaleMode.NoScale)
			{
				DrawCurve(_game.speed.min, Color.white);
				DrawCurve(_game.speed.max, Color.white);
			}
		}

		public void DrawCurve(float speed, Color color)
		{
			Gizmos.color = color;
			float num = _curveParent.CachedCurve.MaxTime - _curveParent.CachedCurve.MinTime;
			Vector3 vector = EvaluateCurve(0f, speed);
			for (int i = 0; i < 13; i++)
			{
				float t = num * (float)i / 12f;
				Vector3 vector2 = EvaluateCurve(t, speed);
				Gizmos.DrawLine(vector, vector2);
				vector = vector2;
			}
		}

		public List<Vector3> GetCoinPositions()
		{
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < NumberOfCoins(SpeedForCurve); i++)
			{
				Vector3 coinPosition = GetCoinPosition(i);
				list.Add(coinPosition);
			}
			return list;
		}
	}
}
