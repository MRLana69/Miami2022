using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Data.Save;
using Info;
using Kiloo.Common.Extensions;
using Missions;
using SYBO.Components.Utilities;
using SYBO.Subway;
using SYBO.Subway.Pickups;
using UnityEngine;

public class GameStats
{
	private const int SCORE_FOR_PICKUPS = 30;

	private const int AMOUNT_BY_WHICH_EXTRA_SCORE_IS_CONSUMED = 4;

	private const float HOVERBOARD_COOLDOWN = 5f;

	public bool BragNotifyDone;

	public bool BragFacebookDone;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnCoinsChanged__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<int> OnCoinsIncreased__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnScoreChanged__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnJumpsChanged__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnRollsChanged__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<UpgradeType> PowerupTriggered__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnMysteryBoxPickup__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnSuperMysteryBoxPickup__BackingField;

	private int _coinsSpawned;

	private int _coinsSpawnedRun;

	private int _coinsCollectedRaw;

	private int _coinsCollectedRawRun;

	private int _xoredNumberOfCoinsCollected;

	private int _xoredNumberOfCoinsRun;

	private int _coinMultiplier;

	private int _coinsWithJetpack;

	private int _coinsWithPogoStick;

	private int _coinsInAir;

	private int _coinCollectedOnLeftTrack;

	private int _coinsCollectedOnCenterTrack;

	private int _coinsCollectedOnRightTrack;

	private int _coinsWithCoinMagnetCount;

	private bool _scoreBoosterActivated;

	private int _scoreBoosterTier;

	private int _scoreBoosterTierRun;

	private bool _characterDayEventActivated;

	private int _score;

	private float _metersLastUsedForScore;

	private float _meterScore;

	private int _amountLeftToConsume;

	private bool _isPausedPowerups;

	private Dictionary<int, int> _spawnedPickupsLastRun = new Dictionary<int, int>();

	private Dictionary<int, int> _powerupActivationCountsLastRun = new Dictionary<int, int>();

	private Dictionary<int, int> _mysteryPowerupActivationCountsLastRun = new Dictionary<int, int>();

	private List<ActivePowerup> _listOfActivePowerups = new List<ActivePowerup>();

	private Dictionary<UpgradeType, bool> _missionEachPowerupPickupStatus = new Dictionary<UpgradeType, bool>();

	private float _metersInCurrentGame;

	private int _stumbles;

	private int _stumblesRun;

	private int _jumps;

	private int _jumpsOverTrainsCount;

	private int _rollCount;

	private int _rollsLeftTrackCount;

	private int _rollsCenterTrackCount;

	private int _rollsRightTrackCount;

	private int _dodgeBarrierCount;

	private int _jumpBarrierCount;

	private int _jumpHighBarrierCount;

	private int _trainHitCount;

	private int _movingTrainHit;

	private int _barrierHitCount;

	private int _jetpackPickupsCollected;

	private int _jetpackPickupsCollectedRun;

	private int _superSneakerPickupsCollected;

	private int _superSneakerPickupsCollectedRun;

	private int _pogoStickPickupsCollected;

	private int _pogoStickPickupsCollectedRun;

	private int _dailyLetterPickupsCollected;

	private int _dailyLetterPickupsCollectedRun;

	private int _characterLetterPickupsCollected;

	private int _characterLetterPickupsCollectedRun;

	private int _coinMagnetPickupsCollected;

	private int _coinMagnetPickupsCollectedRun;

	private int _mysteryBoxPickupsCollected;

	private int _mysteryBoxPickupsCollectedRun;

	private int _superMysteryBoxPickupsCollected;

	private int _superMysteryBoxPickupsCollectedRun;

	private int _doubleMultiplierPickupsCollected;

	private int _doubleMultiplierPickupsCollectedRun;

	private int _huntTokensCollected;

	private int _huntTokensCollectedRun;

	private int _pickedUpPowerups;

	private int _keyPickupsCollected;

	private int _keyPickupsCollectedRun;

	private static GameStats instance;

	public int CoinsSpawned
	{
		get
		{
			return _coinsSpawned;
		}
	}

	public int CoinsSpawnedRun
	{
		get
		{
			return _coinsSpawnedRun;
		}
	}

	public int CoinsCollectedRaw
	{
		get
		{
			return _coinsCollectedRaw;
		}
	}

	public int CoinsCollectedRawRun
	{
		get
		{
			return _coinsCollectedRawRun;
		}
	}

	public int CoinsCollected
	{
		get
		{
			return _xoredNumberOfCoinsCollected.EncryptDecryptXORValue();
		}
		private set
		{
			int num = _xoredNumberOfCoinsCollected.EncryptDecryptXORValue();
			_xoredNumberOfCoinsCollected = value.EncryptDecryptXORValue();
			if (num != value && OnCoinsChanged__BackingField != null)
			{
				OnCoinsChanged__BackingField();
			}
		}
	}

	public int CoinsCollectedRun
	{
		get
		{
			return _xoredNumberOfCoinsRun.EncryptDecryptXORValue();
		}
		private set
		{
			_xoredNumberOfCoinsRun = value.EncryptDecryptXORValue();
		}
	}

	public int CoinMultiplier
	{
		get
		{
			if (UserSave.hasDoubleCoinsUpgrade)
			{
				if (DoubleCoins.HasDoubleCoinsModifier)
				{
					_coinMultiplier = 4;
				}
				else
				{
					_coinMultiplier = 2;
				}
			}
			else if (DoubleCoins.HasDoubleCoinsModifier)
			{
				_coinMultiplier = 2;
			}
			else
			{
				_coinMultiplier = 1;
			}
			return _coinMultiplier;
		}
	}

	public int CoinsWithJetpack
	{
		get
		{
			return _coinsWithJetpack;
		}
	}

	public bool ScoreBoosterActivated
	{
		get
		{
			return _scoreBoosterActivated;
		}
		set
		{
			_scoreBoosterActivated = value;
		}
	}

	public int ScoreBoosterTier
	{
		get
		{
			return _scoreBoosterTier;
		}
		private set
		{
			_scoreBoosterTier = value;
			ScoreMultiplierInfo.TriggerOnScoreMultiplierChanged();
		}
	}

	public int ScoreBoosterTierRun
	{
		get
		{
			return _scoreBoosterTierRun;
		}
	}

	public bool CharacterDayEventActivated
	{
		get
		{
			return _characterDayEventActivated;
		}
		set
		{
			_characterDayEventActivated = value;
			ScoreMultiplierInfo.TriggerOnScoreMultiplierChanged();
		}
	}

	public bool IsPausedPowerups
	{
		get
		{
			return _isPausedPowerups;
		}
		set
		{
			_isPausedPowerups = value;
		}
	}

	public int Score
	{
		get
		{
			return _score;
		}
	}

	public Dictionary<UpgradeType, bool> MissionEachPowerupPickupStatus
	{
		get
		{
			return _missionEachPowerupPickupStatus;
		}
	}

	public float MetersInCurrentGame
	{
		get
		{
			return _metersInCurrentGame;
		}
		set
		{
			_metersInCurrentGame = value;
		}
	}

	public int Stumbles
	{
		get
		{
			return _stumbles;
		}
	}

	public int StumblesRun
	{
		get
		{
			return _stumblesRun;
		}
	}

	public int JumpsOverTrainsCount
	{
		get
		{
			return _jumpsOverTrainsCount;
		}
	}

	public int RollCount
	{
		get
		{
			return _rollCount;
		}
		private set
		{
			_rollCount = value;
			if (OnRollsChanged__BackingField != null)
			{
				OnRollsChanged__BackingField();
			}
		}
	}

	public int JumpHighBarrierCount
	{
		get
		{
			return _jumpHighBarrierCount;
		}
	}

	public int MovingTrainHitCount
	{
		get
		{
			return _movingTrainHit;
		}
		set
		{
			_movingTrainHit = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.DieToTrain);
			}
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.CrashTrains);
			}
		}
	}

	public int JetpackPickupsCollected
	{
		get
		{
			return _jetpackPickupsCollected;
		}
		private set
		{
			_jetpackPickupsCollected = value;
			if (value == 0)
			{
				if (_missionEachPowerupPickupStatus.ContainsKey(UpgradeType.jetpack))
				{
					_missionEachPowerupPickupStatus[UpgradeType.jetpack] = false;
				}
				else
				{
					_missionEachPowerupPickupStatus.Add(UpgradeType.jetpack, true);
				}
			}
		}
	}

	public int JetpackPickupsCollectedRun
	{
		get
		{
			return _jetpackPickupsCollectedRun;
		}
	}

	public int SuperSneakerPickupsCollected
	{
		get
		{
			return _superSneakerPickupsCollected;
		}
		private set
		{
			_superSneakerPickupsCollected = value;
			if (value == 0)
			{
				if (_missionEachPowerupPickupStatus.ContainsKey(UpgradeType.supersneakers))
				{
					_missionEachPowerupPickupStatus[UpgradeType.supersneakers] = false;
				}
				else
				{
					_missionEachPowerupPickupStatus.Add(UpgradeType.supersneakers, true);
				}
			}
		}
	}

	public int SuperSneakerPickupsCollectedRun
	{
		get
		{
			return _superSneakerPickupsCollectedRun;
		}
	}

	public int PogoStickPickupsCollected
	{
		get
		{
			return _pogoStickPickupsCollected;
		}
		private set
		{
			_pogoStickPickupsCollected = value;
			if (value == 0)
			{
				if (_missionEachPowerupPickupStatus.ContainsKey(UpgradeType.pogostick))
				{
					_missionEachPowerupPickupStatus[UpgradeType.pogostick] = false;
				}
				else
				{
					_missionEachPowerupPickupStatus.Add(UpgradeType.pogostick, true);
				}
			}
		}
	}

	public int PogoStickPickupsCollectedRun
	{
		get
		{
			return _pogoStickPickupsCollectedRun;
		}
	}

	public int DailyLetterPickupsCollected
	{
		get
		{
			return _dailyLetterPickupsCollected;
		}
	}

	public int DailyLetterPickupsCollectedRun
	{
		get
		{
			return _dailyLetterPickupsCollectedRun;
		}
	}

	public int CharacterLetterPickupsCollected
	{
		get
		{
			return _characterLetterPickupsCollected;
		}
	}

	public int CharacterLetterPickupsCollectedRun
	{
		get
		{
			return _characterLetterPickupsCollectedRun;
		}
	}

	public int CoinMagnetPickupsCollected
	{
		get
		{
			return _coinMagnetPickupsCollected;
		}
		private set
		{
			_coinMagnetPickupsCollected = value;
			if (value == 0)
			{
				if (_missionEachPowerupPickupStatus.ContainsKey(UpgradeType.coinmagnet))
				{
					_missionEachPowerupPickupStatus[UpgradeType.coinmagnet] = false;
				}
				else
				{
					_missionEachPowerupPickupStatus.Add(UpgradeType.coinmagnet, true);
				}
			}
		}
	}

	public int CoinMagnetPickupsCollectedRun
	{
		get
		{
			return _coinMagnetPickupsCollectedRun;
		}
	}

	public int MysteryBoxPickupsCollected
	{
		get
		{
			return _mysteryBoxPickupsCollected;
		}
	}

	public int MysteryBoxPickupsCollectedRun
	{
		get
		{
			return _mysteryBoxPickupsCollectedRun;
		}
	}

	public int SuperMysteryBoxPickupsCollected
	{
		get
		{
			return _superMysteryBoxPickupsCollected;
		}
	}

	public int SuperMysteryBoxPickupsCollectedRun
	{
		get
		{
			return _superMysteryBoxPickupsCollectedRun;
		}
	}

	public int DoubleMultiplierPickupsCollected
	{
		get
		{
			return _doubleMultiplierPickupsCollected;
		}
		private set
		{
			_doubleMultiplierPickupsCollected = value;
			if (value == 0)
			{
				if (_missionEachPowerupPickupStatus.ContainsKey(UpgradeType.doubleMultiplier))
				{
					_missionEachPowerupPickupStatus[UpgradeType.doubleMultiplier] = false;
				}
				else
				{
					_missionEachPowerupPickupStatus.Add(UpgradeType.doubleMultiplier, true);
				}
			}
		}
	}

	public int DoubleMultiplierPickupsCollectedRun
	{
		get
		{
			return _doubleMultiplierPickupsCollectedRun;
		}
	}

	public int HuntTokensCollected
	{
		get
		{
			return _huntTokensCollected;
		}
	}

	public int HuntTokensCollectedRun
	{
		get
		{
			return _huntTokensCollectedRun;
		}
	}

	public int KeyPickupsCollected
	{
		get
		{
			return _keyPickupsCollected;
		}
	}

	public int KeyPickupsCollectedRun
	{
		get
		{
			return _keyPickupsCollectedRun;
		}
	}

	public static GameStats Instance
	{
		get
		{
			return instance ?? (instance = new GameStats());
		}
	}

	public event Action OnCoinsChanged
	{
		add
		{
			Action action = OnCoinsChanged__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnCoinsChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnCoinsChanged__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnCoinsChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action<int> OnCoinsIncreased
	{
		add
		{
			Action<int> action = OnCoinsIncreased__BackingField;
			Action<int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnCoinsIncreased__BackingField, (Action<int>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<int> action = OnCoinsIncreased__BackingField;
			Action<int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnCoinsIncreased__BackingField, (Action<int>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnScoreChanged
	{
		add
		{
			Action action = OnScoreChanged__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnScoreChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnScoreChanged__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnScoreChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnJumpsChanged
	{
		add
		{
			Action action = OnJumpsChanged__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnJumpsChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnJumpsChanged__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnJumpsChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnRollsChanged
	{
		add
		{
			Action action = OnRollsChanged__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnRollsChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnRollsChanged__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnRollsChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action<UpgradeType> PowerupTriggered
	{
		add
		{
			Action<UpgradeType> action = PowerupTriggered__BackingField;
			Action<UpgradeType> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref PowerupTriggered__BackingField, (Action<UpgradeType>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<UpgradeType> action = PowerupTriggered__BackingField;
			Action<UpgradeType> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref PowerupTriggered__BackingField, (Action<UpgradeType>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnMysteryBoxPickup
	{
		add
		{
			Action action = OnMysteryBoxPickup__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnMysteryBoxPickup__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnMysteryBoxPickup__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnMysteryBoxPickup__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnSuperMysteryBoxPickup
	{
		add
		{
			Action action = OnSuperMysteryBoxPickup__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnSuperMysteryBoxPickup__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnSuperMysteryBoxPickup__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnSuperMysteryBoxPickup__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private GameStats()
	{
		if (Application.isPlaying)
		{
			SingletonMonoBehaviour<PickupManager>.Instance.OnPickupSpawned += PickupManager_OnPickupSpawned;
		}
	}

	public void CoinsSpawnedIncrement()
	{
		_coinsSpawned++;
		_coinsSpawnedRun++;
	}

	public void CoinsCollectedRawIncrement()
	{
		_coinsCollectedRaw++;
		_coinsCollectedRawRun++;
	}

	public void CoinsCollectedIncrement(int count)
	{
		if (count > 0)
		{
			CoinsCollected += count;
			CoinsCollectedRun += count;
			if (OnCoinsIncreased__BackingField != null)
			{
				OnCoinsIncreased__BackingField(count);
			}
			InventorySave.unrewardedCoins += count;
			MissionsManager.Instance.PlayerDidThis(MissionTarget.EarnCoin, count);
			MissionsManager.Instance.RemoveProgressForThis(MissionTarget.NoCoinsWithoutScore);
		}
	}

	public void CoinsWithJetpackIncrement()
	{
		_coinsWithJetpack++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.CoinsWithJetpack);
	}

	public void CoinsWithPogoStickIncrement()
	{
		_coinsWithPogoStick++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.CollectCoinsWithPowerJumper);
	}

	public void CoinsInAirIncrement()
	{
		_coinsInAir++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.EarnCoinWithoutTouchingGround);
	}

	public void CoinsCollectedOnLeftTrackIncrement()
	{
		_coinCollectedOnLeftTrack++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.CollectCoinsLeftLane);
	}

	public void CoinsCollectedOnCenterTrackIncrement()
	{
		_coinsCollectedOnCenterTrack++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.CollectCoinsCenterLane);
	}

	public void CoinsCollectedOnRightTrackIncrement()
	{
		_coinsCollectedOnRightTrack++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.CollectCoinsRightLane);
	}

	public void CoinMagnetCountIncrement()
	{
		_coinsWithCoinMagnetCount++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.CoinsWithMagnet);
	}

	public void ScoreBoosterTierIncrement()
	{
		ScoreBoosterTier++;
		_scoreBoosterTierRun++;
	}

	public void CalculateScore()
	{
		if (_metersLastUsedForScore < MetersInCurrentGame)
		{
			_meterScore = MetersInCurrentGame - _metersLastUsedForScore;
			_metersLastUsedForScore = MetersInCurrentGame;
			int scoreMultiplier = ScoreMultiplierInfo.scoreMultiplier;
			int num = (int)(_meterScore * (float)scoreMultiplier);
			num += ConsumeAmountNeededToAdjustScoreBy(4) * scoreMultiplier;
			_score += num;
			MissionsManager.Instance.PlayerDidThis(MissionTarget.Score, num);
			MissionsManager.Instance.PlayerDidThis(MissionTarget.NoCoinsWithoutScore, num);
			MissionsManager.Instance.PlayerDidThis(MissionTarget.NoRollsWithoutScore, num);
			MissionsManager.Instance.PlayerDidThis(MissionTarget.NoJumpsWithoutScore, num);
			if (_listOfActivePowerups.Count == 0 || (_listOfActivePowerups.Count == 1 && _listOfActivePowerups[0].type == UpgradeType.hoverboard))
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.NoPowerUpsWithoutScore, num);
			}
			if (ScoreBoosterActivated)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.ScoreWithScorebooster, num);
			}
			OnScoreChanged__BackingField.SafeInvoke();
		}
	}

	public void ResetAmountNeededToAdjustScoreBy()
	{
		_amountLeftToConsume = 0;
	}

	public void IncreaseAmountNeededToAdjustScoreBy(int amount)
	{
		_amountLeftToConsume += amount;
	}

	private int ConsumeAmountNeededToAdjustScoreBy(int amount)
	{
		if (amount > 0)
		{
			if (amount > _amountLeftToConsume)
			{
				amount = _amountLeftToConsume;
			}
			if (_amountLeftToConsume > 0)
			{
				_amountLeftToConsume -= amount;
				return amount;
			}
		}
		return 0;
	}

	public void AddScoreForPickup(UpgradeType type)
	{
		switch (type)
		{
		case UpgradeType.jetpack:
		case UpgradeType.supersneakers:
		case UpgradeType.coinmagnet:
		case UpgradeType.doubleMultiplier:
			AddScoreForPickup(true);
			break;
		case UpgradeType.mysterybox:
		case UpgradeType.letters:
			AddScoreForPickup(false);
			break;
		case UpgradeType.tokenmysterybox:
			break;
		}
	}

	private void AddScoreForPickup(bool wasPowerup)
	{
		int num = ScoreMultiplierInfo.scoreMultiplier * 30;
		_score += num;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.Score, num);
		MissionsManager.Instance.PlayerDidThis(MissionTarget.NoCoinsWithoutScore, num);
		MissionsManager.Instance.PlayerDidThis(MissionTarget.NoRollsWithoutScore, num);
		MissionsManager.Instance.PlayerDidThis(MissionTarget.NoJumpsWithoutScore, num);
		if (!wasPowerup)
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.NoPowerUpsWithoutScore, num);
		}
	}

	private void ResetScore()
	{
		_score = 0;
		_metersLastUsedForScore = 0f;
		_meterScore = 0f;
	}

	public void ResetGame()
	{
		BragNotifyDone = false;
		BragFacebookDone = false;
		ResetScore();
		ResetAmountNeededToAdjustScoreBy();
		_coinsSpawned = 0;
		_coinsCollectedRaw = 0;
		CoinsCollected = 0;
		_coinsWithCoinMagnetCount = 0;
		_coinsWithJetpack = 0;
		ScoreBoosterActivated = false;
		ScoreBoosterTier = 0;
		MetersInCurrentGame = 0f;
		_stumbles = 0;
		_jumps = 0;
		_jumpsOverTrainsCount = 0;
		RollCount = 0;
		_rollsLeftTrackCount = 0;
		_rollsCenterTrackCount = 0;
		_rollsRightTrackCount = 0;
		_dodgeBarrierCount = 0;
		_jumpBarrierCount = 0;
		_jumpHighBarrierCount = 0;
		_trainHitCount = 0;
		_barrierHitCount = 0;
		JetpackPickupsCollected = 0;
		SuperSneakerPickupsCollected = 0;
		PogoStickPickupsCollected = 0;
		_dailyLetterPickupsCollected = 0;
		_characterLetterPickupsCollected = 0;
		CoinMagnetPickupsCollected = 0;
		_mysteryBoxPickupsCollected = 0;
		_superMysteryBoxPickupsCollected = 0;
		_superSneakerPickupsCollected = 0;
		_pickedUpPowerups = 0;
		DoubleMultiplierPickupsCollected = 0;
		_keyPickupsCollected = 0;
		_huntTokensCollected = 0;
		ResetRun();
	}

	public void ResetRun()
	{
		_coinsSpawnedRun = 0;
		_coinsCollectedRawRun = 0;
		CoinsCollectedRun = 0;
		_scoreBoosterTierRun = 0;
		_stumblesRun = 0;
		_jetpackPickupsCollectedRun = 0;
		_superSneakerPickupsCollectedRun = 0;
		_pogoStickPickupsCollectedRun = 0;
		_dailyLetterPickupsCollectedRun = 0;
		_characterLetterPickupsCollectedRun = 0;
		_coinMagnetPickupsCollectedRun = 0;
		_mysteryBoxPickupsCollectedRun = 0;
		_superMysteryBoxPickupsCollectedRun = 0;
		_doubleMultiplierPickupsCollectedRun = 0;
		_keyPickupsCollectedRun = 0;
		_huntTokensCollectedRun = 0;
		_powerupActivationCountsLastRun.Clear();
		_mysteryPowerupActivationCountsLastRun.Clear();
		_spawnedPickupsLastRun.Clear();
		_isPausedPowerups = false;
	}

	private void PickupManager_OnPickupSpawned(object sender, PickupManager.PickupEventArgs args)
	{
		int value = 0;
		_spawnedPickupsLastRun.TryGetValue((int)args.PickupData.PickupType, out value);
		value++;
		_spawnedPickupsLastRun[(int)args.PickupData.PickupType] = value;
	}

	public int GetPickupCountSpawnedLastRun(PickupType pickup)
	{
		int value = 0;
		_spawnedPickupsLastRun.TryGetValue((int)pickup, out value);
		return value;
	}

	public ActivePowerup TriggerPowerup(UpgradeType type, bool triggeredViaMysteryPowerup = false)
	{
		int value = 0;
		_powerupActivationCountsLastRun.TryGetValue((int)type, out value);
		value++;
		_powerupActivationCountsLastRun[(int)type] = value;
		if (triggeredViaMysteryPowerup)
		{
			_mysteryPowerupActivationCountsLastRun.TryGetValue((int)type, out value);
			value++;
			_mysteryPowerupActivationCountsLastRun[(int)type] = value;
		}
		ActivePowerup activePowerup = new ActivePowerup();
		activePowerup.type = type;
		activePowerup.timeActivated = Time.time;
		activePowerup.timeLeft = Upgrades.GetUpgradeDuration(type);
		ActivePowerup activePowerup2 = activePowerup;
		for (int num = _listOfActivePowerups.Count - 1; num >= 0; num--)
		{
			if (_listOfActivePowerups[num].type == activePowerup2.type)
			{
				_listOfActivePowerups.RemoveAt(num);
			}
		}
		AddScoreForPickup(type);
		_listOfActivePowerups.Add(activePowerup2);
		_listOfActivePowerups.Sort((ActivePowerup a, ActivePowerup b) => b.timeLeft.CompareTo(a.timeLeft));
		MissionsManager.Instance.RemoveProgressForThis(MissionTarget.ActivePowerups);
		int num2 = _listOfActivePowerups.Count;
		for (int num3 = 0; num3 < _listOfActivePowerups.Count; num3++)
		{
			if (_listOfActivePowerups[num3].type == UpgradeType.hoverboard)
			{
				num2--;
				break;
			}
		}
		MissionsManager.Instance.PlayerDidThis(MissionTarget.ActivePowerups, num2);
		if (PowerupTriggered__BackingField != null)
		{
			PowerupTriggered__BackingField(activePowerup2.type);
		}
		return activePowerup2;
	}

	public int GetPowerupActivationCountLastRun(UpgradeType upgrade)
	{
		int value = 0;
		_powerupActivationCountsLastRun.TryGetValue((int)upgrade, out value);
		return value;
	}

	public int GetMysteryPowerupActivationCountLastRun(UpgradeType upgrade)
	{
		int value = 0;
		_mysteryPowerupActivationCountsLastRun.TryGetValue((int)upgrade, out value);
		return value;
	}

	public List<ActivePowerup> GetActivePowerups()
	{
		return _listOfActivePowerups;
	}

	public bool IsPowerupActive(UpgradeType type)
	{
		return _listOfActivePowerups.Find((ActivePowerup p) => p.type == type) != null;
	}

	public void UpdatePowerupTimes(float deltaTime)
	{
		if (_isPausedPowerups)
		{
			return;
		}
		for (int num = _listOfActivePowerups.Count - 1; num >= 0; num--)
		{
			ActivePowerup activePowerup = _listOfActivePowerups[num];
			if (((Game.Instance.IsInJetpackMode || Game.Instance.IsInPogostickMode) && (activePowerup.type == UpgradeType.hoverboard || activePowerup.type == UpgradeType.supersneakers || activePowerup.type == UpgradeType.doubleJump)) || (activePowerup.type == UpgradeType.supersneakers && Game.Instance.Modifiers.Hoverboard.isActive && Hoverboard.Instance.IsJumper) || (activePowerup.type == UpgradeType.doubleJump && Game.Instance.Modifiers.Hoverboard.isActive && Hoverboard.Instance.IsDoubleJump))
			{
				continue;
			}
			float num2 = deltaTime;
			if (activePowerup.Paused && (activePowerup.type != UpgradeType.hoverboard || activePowerup.timeLeft > 0f))
			{
				num2 = 0f;
			}
			if (activePowerup.type == UpgradeType.jetpack || activePowerup.type == UpgradeType.headstart2000)
			{
				if (Game.Instance.IsInJetpackMode)
				{
					activePowerup.timeLeft = 1f;
				}
				else
				{
					activePowerup.timeLeft = -1f;
				}
			}
			else
			{
				activePowerup.timeLeft -= num2;
			}
			if (!(activePowerup.timeLeft < 0f) || (Game.Instance.IsInJetpackMode && activePowerup.type == UpgradeType.jetpack))
			{
				continue;
			}
			if (activePowerup.type == UpgradeType.hoverboard)
			{
				float num3 = Hoverboard.Instance.WaitForParticlesDelay + 5f;
				if (activePowerup.timeLeft > 0f - num3)
				{
					continue;
				}
				Hoverboard.Instance.HardReset();
			}
			_listOfActivePowerups.RemoveAt(num);
			_listOfActivePowerups.Sort((ActivePowerup a, ActivePowerup b) => b.timeLeft.CompareTo(a.timeLeft));
			MissionsManager.Instance.RemoveProgressForThis(MissionTarget.ActivePowerups);
			int num4 = _listOfActivePowerups.Count;
			for (int num5 = 0; num5 < _listOfActivePowerups.Count; num5++)
			{
				if (_listOfActivePowerups[num5].type == UpgradeType.hoverboard)
				{
					num4--;
					break;
				}
			}
			MissionsManager.Instance.PlayerDidThis(MissionTarget.ActivePowerups, num4);
		}
	}

	public void ClearPowerups()
	{
		_listOfActivePowerups.Clear();
		MissionsManager.Instance.RemoveProgressForThis(MissionTarget.ActivePowerups);
	}

	public void RemoveHoverBoardPowerup()
	{
		for (int num = _listOfActivePowerups.Count - 1; num >= 0; num--)
		{
			if (_listOfActivePowerups[num].type == UpgradeType.hoverboard)
			{
				_listOfActivePowerups[num].timeLeft = 0f;
			}
		}
	}

	public void StumbleCountIncrement()
	{
		_stumbles++;
		_stumblesRun++;
	}

	public void JumpCountIncrement()
	{
		_jumps++;
		if (OnJumpsChanged__BackingField != null)
		{
			OnJumpsChanged__BackingField();
		}
		MissionsManager.Instance.PlayerDidThis(MissionTarget.Jump);
		MissionsManager.Instance.RemoveProgressForThis(MissionTarget.NoJumpsWithoutScore);
	}

	public void JumpsOverTrainsCountIncrement()
	{
		_jumpsOverTrainsCount++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.JumpTrain);
	}

	public void RollCountIncrement()
	{
		RollCount++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.Roll);
		MissionsManager.Instance.RemoveProgressForThis(MissionTarget.NoRollsWithoutScore);
	}

	public void RollsLeftTrackCountIncrement()
	{
		_rollsLeftTrackCount++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.RollLeft);
	}

	public void RollsCenterTrackCountIncrement()
	{
		_rollsCenterTrackCount++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.RollCenter);
	}

	public void RollsRightTrackCountIncrement()
	{
		_rollsRightTrackCount++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.RollRight);
	}

	public void DodgeBarrierCountIncrement()
	{
		if (!Game.Instance.isDead && !Game.Instance.isInStumbleDeathSequence)
		{
			_dodgeBarrierCount++;
			MissionsManager.Instance.PlayerDidThis(MissionTarget.RollUnderBarriers);
			MissionsManager.Instance.PlayerDidThis(MissionTarget.DodgeBarriers);
		}
	}

	public void JumpBarrierCountIncrement()
	{
		if (!Game.Instance.isDead && !Game.Instance.isInStumbleDeathSequence)
		{
			_jumpBarrierCount++;
			MissionsManager.Instance.PlayerDidThis(MissionTarget.JumpBarriers);
			MissionsManager.Instance.PlayerDidThis(MissionTarget.DodgeBarriers);
		}
	}

	public void JumpHighBarrierCountIncrement()
	{
		_jumpHighBarrierCount++;
	}

	public void TrainHitCountIncrement()
	{
		_trainHitCount++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.CrashTrains);
	}

	public void BarrierHitCountIncrement()
	{
		_barrierHitCount++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.CrashBarriers);
	}

	public void JetpackPickupsCollectedIncrement()
	{
		JetpackPickupsCollected++;
		_jetpackPickupsCollectedRun++;
		ReportOneOfEachPowerupIfApplicable(UpgradeType.jetpack);
		MissionsManager.Instance.PlayerDidThis(MissionTarget.Jetpack);
	}

	public void SuperSneakerPickupsCollectedIncrement()
	{
		SuperSneakerPickupsCollected++;
		_superSneakerPickupsCollectedRun++;
		ReportOneOfEachPowerupIfApplicable(UpgradeType.supersneakers);
		MissionsManager.Instance.PlayerDidThis(MissionTarget.SuperSneakers);
	}

	public void PogoStickPickupsCollectedIncrement()
	{
		PogoStickPickupsCollected++;
		_pogoStickPickupsCollectedRun++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.PickUpPowerJumpers);
		MissionsManager.Instance.PlayerDidThis(MissionTarget.PickUpPowerJumpersSingleRun);
		ReportOneOfEachPowerupIfApplicable(UpgradeType.pogostick);
	}

	public void DailyLetterPickupsCollectedIncrement()
	{
		_dailyLetterPickupsCollected++;
		_dailyLetterPickupsCollectedRun++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.Letters);
	}

	public void CharacterLetterPickupsCollectedIncrement()
	{
		_characterLetterPickupsCollected++;
		_characterLetterPickupsCollectedRun++;
	}

	public void CoinMagnetPickupsCollectedIncrement()
	{
		CoinMagnetPickupsCollected++;
		_coinMagnetPickupsCollectedRun++;
		ReportOneOfEachPowerupIfApplicable(UpgradeType.coinmagnet);
		MissionsManager.Instance.PlayerDidThis(MissionTarget.Magnets);
	}

	public void MysteryBoxPickupsCollectedInrement()
	{
		_mysteryBoxPickupsCollected++;
		_mysteryBoxPickupsCollectedRun++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.MysteryBoxes);
		if (OnMysteryBoxPickup__BackingField != null)
		{
			OnMysteryBoxPickup__BackingField();
		}
	}

	public void SuperMysteryBoxPickupsCollectedIncrement()
	{
		_superMysteryBoxPickupsCollected++;
		_superMysteryBoxPickupsCollectedRun++;
		if (OnSuperMysteryBoxPickup__BackingField != null)
		{
			OnSuperMysteryBoxPickup__BackingField();
		}
	}

	public void DoubleMultiplierPickupsCollectedIncrement()
	{
		DoubleMultiplierPickupsCollected++;
		_doubleMultiplierPickupsCollectedRun++;
		ReportOneOfEachPowerupIfApplicable(UpgradeType.doubleMultiplier);
		MissionsManager.Instance.PlayerDidThis(MissionTarget.DoubleMultiplier);
	}

	public void HuntTokensCollectedIncrement()
	{
		_huntTokensCollected++;
		_huntTokensCollectedRun++;
	}

	private void ReportOneOfEachPowerupIfApplicable(UpgradeType upgradeType)
	{
		if (MissionsManager.Instance.IsMissionTargetActive(MissionTarget.OneOfEachPowerup) && _missionEachPowerupPickupStatus.ContainsKey(upgradeType) && !_missionEachPowerupPickupStatus[upgradeType])
		{
			_missionEachPowerupPickupStatus[upgradeType] = true;
			MissionsManager.Instance.PlayerDidThis(MissionTarget.OneOfEachPowerup);
		}
	}

	public void PickedUpPowerupsIncrement()
	{
		_pickedUpPowerups++;
		Stat.PickupPowerup.IncrementValue();
		MissionsManager.Instance.PlayerDidThis(MissionTarget.Powerups);
		MissionsManager.Instance.RemoveProgressForThis(MissionTarget.NoPowerUpsWithoutScore);
	}

	public void KeyPickupsCollectedIncrement()
	{
		_keyPickupsCollected++;
		_keyPickupsCollectedRun++;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.PickupKeys);
	}

	public void RunCompleted()
	{
		OnboardingSave.numberOfRunsSinceLastGuideline++;
		PlayerSave.completedRunCount++;
		if (PlayerSave.runsCompletedInCurrentMissionsSet != -1)
		{
			PlayerSave.runsCompletedInCurrentMissionsSet++;
		}
		if (OnboardingSave.numberOfRunsSinceLastGuideline <= 4)
		{
			return;
		}
		OnboardingSave.numberOfRunsSinceLastGuideline = 0;
		if (!OnboardingSave.hasShownHoverboardPopup && OnboardingSave.shouldShowHoverboardPopup)
		{
			return;
		}
		if (!OnboardingSave.hasShownHoverboardPopup)
		{
			if (Achievements.Instance.GetNumberOfHoverboardsUsed() == 0 || InventorySave.GetAmount(UpgradeType.hoverboard) == 3 - Achievements.Instance.GetNumberOfHoverboardsUsed())
			{
				OnboardingSave.shouldShowHoverboardPopup = true;
				OnboardingSave.shouldShowFacebookPopup = false;
				OnboardingSave.shouldShowCollectPopup = false;
				OnboardingSave.shouldShowMissionIntroPopup = false;
				OnboardingSave.shouldShowEndGameMissionPopup = false;
				return;
			}
			OnboardingSave.hasShownHoverboardPopup = true;
			OnboardingSave.shouldShowHoverboardPopup = false;
		}
		if (!OnboardingSave.hasShownFacebookPopup && !SocialManager.instance.facebookIsLoggedIn && !UserSave.hasPaidOutFacebookReward)
		{
			OnboardingSave.shouldShowFacebookPopup = true;
			return;
		}
		if (!OnboardingSave.hasShownMissionIntroPopup)
		{
			if (ScoreMultiplierInfo.rawMultiplier < 5)
			{
				OnboardingSave.shouldShowMissionIntroPopup = true;
				return;
			}
			OnboardingSave.hasShownMissionIntroPopup = true;
			OnboardingSave.shouldShowMissionIntroPopup = false;
		}
		if (!OnboardingSave.hasShownCollectPopup)
		{
			OnboardingSave.shouldShowCollectPopup = !OnboardingSave.hasCollectedFromFriends && (SocialManager.instance.facebookIsLoggedIn || SocialManager.instance.gamecenterIsLoggedIn);
			if (!OnboardingSave.shouldShowCollectPopup && OnboardingSave.hasCollectedFromFriends)
			{
				OnboardingSave.hasShownCollectPopup = true;
				OnboardingSave.shouldShowCollectPopup = false;
			}
		}
	}

	public static int CoinToScoreConversion(int coins)
	{
		return coins * 2 * ScoreMultiplierInfo.rawMultiplier;
	}
}
