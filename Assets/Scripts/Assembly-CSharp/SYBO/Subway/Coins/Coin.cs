using System;
using System.Diagnostics;
using System.Threading;
using SYBO.Components.Utilities;
using SYBO.Subway.Characters;
using SYBO.Subway.Level;
using SYBO.Subway.Pickups;
using UnityEngine;

namespace SYBO.Subway.Coins
{
	[RequireComponent(typeof(TrackObject))]
	[RequireComponent(typeof(PickupRotate))]
	[RequireComponent(typeof(CoinRenderer))]
	public class Coin : MonoBehaviour
	{
		private class CoinMagnetMovement
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private Action OnComplete__BackingField;

			private Game _game;

			private Character _character;

			private Transform _coinTransform;

			private bool _usedForSuperSneakers;

			private bool _active;

			private Vector3 _startPosition;

			private Vector3 _endPositionOffset;

			private float _startTime;

			private float _duration;

			public event Action OnComplete
			{
				add
				{
					Action action = OnComplete__BackingField;
					Action action2;
					do
					{
						action2 = action;
						action = Interlocked.CompareExchange(ref OnComplete__BackingField, (Action)Delegate.Combine(action2, value), action);
					}
					while ((object)action != action2);
				}
				remove
				{
					Action action = OnComplete__BackingField;
					Action action2;
					do
					{
						action2 = action;
						action = Interlocked.CompareExchange(ref OnComplete__BackingField, (Action)Delegate.Remove(action2, value), action);
					}
					while ((object)action != action2);
				}
			}

			public CoinMagnetMovement(Transform coinTransform, Vector3 endPositionOffset, bool usedForSuperSneakers)
			{
				_coinTransform = coinTransform;
				_endPositionOffset = endPositionOffset;
				_game = Game.Instance;
				_character = Character.Instance;
				_usedForSuperSneakers = usedForSuperSneakers;
			}

			public void Activate(float pullSpeed)
			{
				_startPosition = _coinTransform.position;
				_startTime = Time.time;
				float magnitude = (_startPosition - _character.TransformPosition).magnitude;
				_duration = magnitude / (pullSpeed * _game.CurrentSpeedNormalized);
				_active = true;
			}

			public void Update()
			{
				if (!_active)
				{
					return;
				}
				float num = (Time.time - _startTime) / _duration;
				Transform transform = null;
				transform = (_usedForSuperSneakers ? _character.CharacterRendering.CharacterModel.MeshSuperSneaker.transform : _character.CharacterRendering.CharacterModel.MeshCoinMagnet.transform);
				_coinTransform.position = Vector3.Lerp(_startPosition, transform.position + _endPositionOffset, num * num);
				if (num >= 1f)
				{
					if (OnComplete__BackingField != null)
					{
						OnComplete__BackingField();
					}
					_active = false;
				}
			}

			public void Reset()
			{
				_active = false;
			}
		}

		public const float QA_X_OFFSET = 5f;

		private const float DEACTIVATION_Z_DISTANCE = 10f;

		[SerializeField]
		private PickupRotate _pickupRotate;

		[SerializeField]
		private TrackObject _trackObject;

		[SerializeField]
		private PickupDefault _pickupDefault;

		[SerializeField]
		private CoinRenderer _coinRenderer;

		[SerializeField]
		private Collider _collider;

		private bool _active;

		private Character _character;

		private Transform _transform;

		private CoinCurve _curve;

		private CoinMagnetMovement _coinMagnetMovement;

		private CoinMagnetMovement _superSneakerMagnetMovement;

		private bool _isInitialized;

		protected void Awake()
		{
			Initialize();
		}

		public void UpdateCoin(float time, float characterZ, float characterGamePositionZ, bool lowPerformanceLevel)
		{
			_coinMagnetMovement.Update();
			_superSneakerMagnetMovement.Update();
			if (GamePositionRoot.Instance.GetGamePosition(_transform).z <= characterGamePositionZ - 10f)
			{
				Deactivate();
			}
			_coinRenderer.UpdateCoinRenderer(time, characterZ, lowPerformanceLevel);
		}

		public void OnDestroy()
		{
			if (SingletonMonoBehaviour<CoinManager>.IsInstantiated() && !SingletonMonoBehaviour.IsShuttingDown)
			{
				SingletonMonoBehaviour<CoinManager>.Instance.NotifyCoinDestroyed(this);
			}
			CoinStats.New.NotifyCoinDestroyed();
		}

		public void ToggleRenderer(bool enabled)
		{
			_coinRenderer.ToggleRenderers(enabled);
		}

		public void SetCurve(CoinCurve curve)
		{
			_curve = curve;
		}

		public void Deactivate(bool trackObjectDeactivated = false)
		{
			if (_active)
			{
				_active = false;
				if (!trackObjectDeactivated)
				{
					_trackObject.Deactivate();
				}
				SingletonMonoBehaviour<CoinManager>.Instance.NotifyCoinDeactivated(this);
				_coinMagnetMovement.Reset();
				_superSneakerMagnetMovement.Reset();
				if (_curve != null)
				{
					_curve.DeactivateCoin(this);
				}
				_curve = null;
				CoinStats.New.NotifyCoinDeactivated();
			}
		}

		public void Activate(bool trackObjectActivated = false)
		{
			if (!_active)
			{
				_active = true;
				if (!trackObjectActivated)
				{
					_trackObject.Activate();
				}
				base.transform.localPosition = Vector3.zero;
				_collider.enabled = true;
				ToggleRenderer(true);
				SingletonMonoBehaviour<CoinManager>.Instance.NotifyCoinActivated(this);
				CoinStats.New.NotifyCoinActivated();
			}
		}

		public void MoveToMagnet(float pullSpeed)
		{
			_collider.enabled = false;
			_coinMagnetMovement.Activate(pullSpeed);
		}

		public void MoveToSuperSneakers(float pullSpeed)
		{
			_collider.enabled = false;
			_superSneakerMagnetMovement.Activate(pullSpeed);
		}

		public void Initialize()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;
				if (_pickupRotate == null)
				{
					_pickupRotate = GetComponent<PickupRotate>();
				}
				if (_pickupDefault == null)
				{
					_pickupDefault = GetComponent<PickupDefault>();
				}
				if (_coinRenderer == null)
				{
					_coinRenderer = GetComponent<CoinRenderer>();
				}
				if (_collider == null)
				{
					_collider = GetComponent<Collider>();
				}
				if (_trackObject == null)
				{
					_trackObject = GetComponent<TrackObject>();
				}
				_trackObject.OnActivate += TrackObject_OnActivate;
				_trackObject.OnDeactivate += TrackObject_OnDeactivate;
				_character = Character.Instance;
				_transform = base.transform;
				_coinMagnetMovement = new CoinMagnetMovement(base.transform, Vector3.down * 6f, false);
				_superSneakerMagnetMovement = new CoinMagnetMovement(base.transform, Vector3.down * 6f, true);
				_coinMagnetMovement.OnComplete += MagnetMovement_OnComplete;
				_superSneakerMagnetMovement.OnComplete += SuperSneakersMagnetMovement_OnComplete;
				CoinStats.New.NotifyCoinCreated();
			}
		}

		private void TrackObject_OnActivate()
		{
			Activate(true);
		}

		private void TrackObject_OnDeactivate()
		{
			Deactivate(true);
		}

		public void Pickup(CharacterPickupParticles pickupParticles)
		{
			if (_active)
			{
				SingletonMonoBehaviour<CoinManager>.Instance.PickupCoin(this);
			}
			pickupParticles.PickedUpCoin(base.transform);
			Deactivate();
		}

		private void MagnetMovement_OnComplete()
		{
			Pickup(_character.CharacterPickupParticleSystem);
			_collider.enabled = true;
			GameStats.Instance.CoinMagnetCountIncrement();
		}

		private void SuperSneakersMagnetMovement_OnComplete()
		{
			Pickup(_character.CharacterPickupParticleSystem);
			_collider.enabled = true;
		}
	}
}
