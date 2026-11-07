using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Data.Save;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using SYBO.Subway.Characters;
using SYBO.Subway.Placeholders.Lifecycles;
using UnityEngine;

namespace SYBO.Subway.Coins
{
	[PrefabPath("Prefabs/Managers/CoinManager")]
	public class CoinManager : SingletonMonoBehaviour<CoinManager>
	{
		[SerializeField]
		private GameObject _coinPrefab;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler OnCoinCollected__BackingField;

		private HashSet<Coin> _activeCoins = new HashSet<Coin>();

		private List<Coin> _coinsCopy = new List<Coin>(400);

		private GameStats _gameStats;

		private Character _character;

		public event EventHandler OnCoinCollected
		{
			add
			{
				EventHandler eventHandler = OnCoinCollected__BackingField;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCoinCollected__BackingField, (EventHandler)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler eventHandler = OnCoinCollected__BackingField;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCoinCollected__BackingField, (EventHandler)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		protected void Start()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
			_character = Character.Instance;
			_gameStats = GameStats.Instance;
		}

		public bool IsSpawned(Coin coin)
		{
			return _activeCoins.Contains(coin);
		}

		public void NotifyCoinActivated(Coin coin)
		{
			if (!_activeCoins.Contains(coin))
			{
				_activeCoins.Add(coin);
			}
		}

		public void NotifyCoinDeactivated(Coin coin)
		{
			_activeCoins.Remove(coin);
			PoolManager.Despawn(coin.transform.parent);
		}

		public void NotifyCoinDestroyed(Coin coin)
		{
			_activeCoins.Remove(coin);
		}

		public Coin SpawnCoin(Vector3 position, Transform parentTransform = null)
		{
			Coin coin = null;
			Transform transform = PoolManager.Spawn(_coinPrefab.gameObject.transform);
			PlaceholderLifecycle component = transform.GetComponent<PlaceholderLifecycle>();
			if (!component.Placeholder.HasSpawnedInstances)
			{
				component.Spawn();
			}
			_gameStats.CoinsSpawnedIncrement();
			coin = transform.GetComponentInChildren<Coin>();
			transform.position = position;
			if (parentTransform != null)
			{
				transform.parent = parentTransform;
			}
			coin.Initialize();
			coin.Activate();
			return coin;
		}

		private void RewardCoin()
		{
			if (UserSave.hasDoubleCoinsUpgrade)
			{
				if (DoubleCoins.HasDoubleCoinsModifier)
				{
					_gameStats.CoinsCollectedIncrement(4);
				}
				else
				{
					_gameStats.CoinsCollectedIncrement(2);
				}
			}
			else if (DoubleCoins.HasDoubleCoinsModifier)
			{
				_gameStats.CoinsCollectedIncrement(2);
			}
			else
			{
				_gameStats.CoinsCollectedIncrement(1);
			}
			if (_character.IsAboveGroundOfCharacterRoute)
			{
				_gameStats.CoinsInAirIncrement();
			}
			if (Game.Instance.IsInJetpackMode)
			{
				_gameStats.CoinsWithJetpackIncrement();
			}
			if (Game.Instance.IsInPogostickMode)
			{
				_gameStats.CoinsWithPogoStickIncrement();
			}
			if (_character.TrackIndexCurrent == 0)
			{
				_gameStats.CoinsCollectedOnLeftTrackIncrement();
			}
			else if (_character.TrackIndexCurrent == 1)
			{
				_gameStats.CoinsCollectedOnCenterTrackIncrement();
			}
			else if (_character.TrackIndexCurrent == 2)
			{
				_gameStats.CoinsCollectedOnRightTrackIncrement();
			}
			_gameStats.CoinsCollectedRawIncrement();
		}

		private void Update()
		{
			float time = Time.time;
			bool lowPerformanceLevel = DeviceInfo.Instance.performanceLevel == DeviceInfo.PerformanceLevel.Low;
			float z = Character.Instance.TransformPosition.z;
			float z2 = Character.Instance.GamePosition.z;
			_coinsCopy.Clear();
			_coinsCopy.AddRange(_activeCoins);
			foreach (Coin item in _coinsCopy)
			{
				item.UpdateCoin(time, z, z2, lowPerformanceLevel);
			}
		}

		public void PickupCoin(Coin coin)
		{
			RewardCoin();
			RaiseOnCoinCollected();
		}

		private void RaiseOnCoinCollected()
		{
			if (OnCoinCollected__BackingField != null)
			{
				OnCoinCollected__BackingField(this, EventArgs.Empty);
			}
		}

		public void ResetCoins()
		{
			List<Coin> list = new List<Coin>(_activeCoins);
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Deactivate();
			}
		}
	}
}
