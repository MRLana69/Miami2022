using System;
using System.Collections;
using System.Collections.Generic;
using Data.Save;
using Info;
using Kiloo.Common.Utility;
using SYBO.Common.Exceptions;
using SYBO.Subway.Characters;
using SYBO.Subway.Pickups.Picker.Pickers;
using SYBO.Subway.Pickups.Pickers;
using UI.Screens.RewardManagement;
using UnityEngine;
using WeeklyHunt;
using WordHunt;

namespace SYBO.Subway.Pickups
{
	public class PickupManagerController
	{
		private PickupManager _pickupManager;

		private List<Action> _refreshEventSpecificPickupsFunctions = new List<Action>();

		public PickupManagerController(PickupManager pickupManager)
		{
			_pickupManager = pickupManager;
			_pickupManager.OnPickerRemoved += PickupManager_OnPickerRemoved;
			_pickupManager.OnPickupCollected += PickupManager_OnPickupCollected;
			SetupEventSpecificPickups();
			RefreshEventSpecificPickups();
		}

		public void Destroy()
		{
			if (_pickupManager != null)
			{
				_pickupManager.OnPickerRemoved -= PickupManager_OnPickerRemoved;
				_pickupManager.OnPickupCollected -= PickupManager_OnPickupCollected;
			}
		}

		public void Update()
		{
			RefreshEventSpecificPickups();
		}

		private void SetupEventSpecificPickups()
		{
			_refreshEventSpecificPickupsFunctions = new List<Action>();
			PickupData pickupDataForPickupType = _pickupManager.GetPickupDataForPickupType(PickupType.LetterHuntDaily);
			PickupData pickupDataForPickupType2 = _pickupManager.GetPickupDataForPickupType(PickupType.LetterHuntCharacter);
			PickupData pickupDataForPickupType3 = _pickupManager.GetPickupDataForPickupType(PickupType.MysteryPowerup);
			PickupData pickupDataForPickupType4 = _pickupManager.GetPickupDataForPickupType(PickupType.SuperMysteryBox);
			PickupData pickupDataForPickupType5 = _pickupManager.GetPickupDataForPickupType(PickupType.TokenHunt);
			if (pickupDataForPickupType != null && pickupDataForPickupType.PickupEnabledState == PickupEnabledState.SetProgrammatically)
			{
				_refreshEventSpecificPickupsFunctions.Add(RefreshEventSpecificPickups_LetterHuntDaily);
			}
			if (pickupDataForPickupType2 != null && pickupDataForPickupType2.PickupEnabledState == PickupEnabledState.SetProgrammatically)
			{
				_refreshEventSpecificPickupsFunctions.Add(RefreshEventSpecificPickups_LetterHuntCharacter);
			}
			if (pickupDataForPickupType3 != null && pickupDataForPickupType3.PickupEnabledState == PickupEnabledState.SetProgrammatically)
			{
				_refreshEventSpecificPickupsFunctions.Add(RefreshEventSpecificPickups_MysteryPowerup);
			}
			if (pickupDataForPickupType4 != null && pickupDataForPickupType4.PickupEnabledState == PickupEnabledState.SetProgrammatically)
			{
				_refreshEventSpecificPickupsFunctions.Add(RefreshEventSpecificPickups_SuperMysteryBox);
			}
			if (pickupDataForPickupType5 != null && pickupDataForPickupType5.PickupEnabledState == PickupEnabledState.SetProgrammatically)
			{
				_refreshEventSpecificPickupsFunctions.Add(RefreshEventSpecificPickups_TokenHunt);
			}
		}

		private void AddAnyEventSpecificPickers()
		{
			PickupData pickupDataForPickupType = _pickupManager.GetPickupDataForPickupType(PickupType.LetterHuntDaily);
			PickupData pickupDataForPickupType2 = _pickupManager.GetPickupDataForPickupType(PickupType.TokenHunt);
			PickupData pickupDataForPickupType3 = _pickupManager.GetPickupDataForPickupType(PickupType.SuperMysteryBox);
			PickupData pickupDataForPickupType4 = _pickupManager.GetPickupDataForPickupType(PickupType.MysteryPowerup);
			PickupData pickupDataForPickupType5 = _pickupManager.GetPickupDataForPickupType(PickupType.LetterHuntCharacter);
			PickerData pickerDataForPickerType = _pickupManager.GetPickerDataForPickerType(PickerType.LetterHuntDaily);
			PickerData pickerDataForPickerType2 = _pickupManager.GetPickerDataForPickerType(PickerType.TokenHunt);
			PickerData pickerDataForPickerType3 = _pickupManager.GetPickerDataForPickerType(PickerType.SuperMysteryBox);
			PickerData pickerDataForPickerType4 = _pickupManager.GetPickerDataForPickerType(PickerType.MysteryPowerUp);
			PickerData pickerDataForPickerType5 = _pickupManager.GetPickerDataForPickerType(PickerType.LetterHuntCharacter);
			if (pickupDataForPickupType != null && pickupDataForPickupType.PickupEnabledState == PickupEnabledState.Enabled && !_pickupManager._pickerCollection.HasPickerOfType<LetterHuntDailyPicker>() && pickerDataForPickerType.PickerEnabledState != PickerEnabledState.NotEnabled)
			{
				_pickupManager._pickerCollection.Add(new LetterHuntDailyPicker());
			}
			if (pickupDataForPickupType2 != null && pickupDataForPickupType2.PickupEnabledState == PickupEnabledState.Enabled && !_pickupManager._pickerCollection.HasPickerOfType<TokenHuntPicker>() && pickerDataForPickerType2.PickerEnabledState != PickerEnabledState.NotEnabled)
			{
				_pickupManager._pickerCollection.Add(new TokenHuntPicker());
			}
			if (pickupDataForPickupType3 != null && pickupDataForPickupType3.PickupEnabledState == PickupEnabledState.Enabled && !_pickupManager._pickerCollection.HasPickerOfType<SuperMysteryBoxPicker>() && pickerDataForPickerType3.PickerEnabledState != PickerEnabledState.NotEnabled)
			{
				_pickupManager._pickerCollection.Add(new SuperMysteryBoxPicker());
			}
			if (pickupDataForPickupType4 != null && pickupDataForPickupType4.PickupEnabledState == PickupEnabledState.Enabled && !_pickupManager._pickerCollection.HasPickerOfType<MysteryPowerupPicker>() && pickerDataForPickerType4.PickerEnabledState != PickerEnabledState.NotEnabled)
			{
				_pickupManager._pickerCollection.Add(new MysteryPowerupPicker());
			}
			if (pickupDataForPickupType5 != null && pickupDataForPickupType5.PickupEnabledState == PickupEnabledState.Enabled && !_pickupManager._pickerCollection.HasPickerOfType<LetterHuntCharacterPicker>() && pickerDataForPickerType5.PickerEnabledState != PickerEnabledState.NotEnabled)
			{
				_pickupManager._pickerCollection.Add(new LetterHuntCharacterPicker());
			}
		}

		private void RefreshEventSpecificPickups()
		{
			AddAnyEventSpecificPickers();
			for (int i = 0; i < _refreshEventSpecificPickupsFunctions.Count; i++)
			{
				_refreshEventSpecificPickupsFunctions[i]();
			}
		}

		private void RefreshEventSpecificPickups_SuperMysteryBox()
		{
			PickupData pickupDataForPickupType = _pickupManager.GetPickupDataForPickupType(PickupType.SuperMysteryBox);
			if (pickupDataForPickupType != null)
			{
				IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.SuperMysteryBox);
				if (activeEventManager != null || Globals.DEBUG_USE_ENABLE_SS_EVENT_TYPE___SUPER_MYSTERY_BOX)
				{
					pickupDataForPickupType.PickupEnabledState = PickupEnabledState.Enabled;
				}
				else
				{
					pickupDataForPickupType.PickupEnabledState = PickupEnabledState.NotEnabled;
				}
			}
		}

		private void RefreshEventSpecificPickups_TokenHunt()
		{
			PickupData pickupDataForPickupType = _pickupManager.GetPickupDataForPickupType(PickupType.TokenHunt);
			if (pickupDataForPickupType != null)
			{
				if (WeeklyHuntManager.IsAnyPeriodActive() || Globals.DEBUG_USE_ENABLE_SS_EVENT_TYPE___LETTER_HUNT_CHARACTER)
				{
					pickupDataForPickupType.PickupEnabledState = PickupEnabledState.Enabled;
				}
				else
				{
					pickupDataForPickupType.PickupEnabledState = PickupEnabledState.NotEnabled;
				}
			}
		}

		private void RefreshEventSpecificPickups_MysteryPowerup()
		{
			PickupData pickupDataForPickupType = _pickupManager.GetPickupDataForPickupType(PickupType.MysteryPowerup);
			if (pickupDataForPickupType != null)
			{
				IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.MysteryPowerup);
				if (activeEventManager != null || Globals.DEBUG_USE_ENABLE_SS_EVENT_TYPE___MYSTERY_POWER_UP)
				{
					pickupDataForPickupType.PickupEnabledState = PickupEnabledState.Enabled;
				}
				else
				{
					pickupDataForPickupType.PickupEnabledState = PickupEnabledState.NotEnabled;
				}
			}
		}

		private void RefreshEventSpecificPickups_LetterHuntDaily()
		{
			PickupData pickupDataForPickupType = _pickupManager.GetPickupDataForPickupType(PickupType.LetterHuntDaily);
			bool flag = false;
			if (pickupDataForPickupType != null && (WordHuntManager.IsActiveDailyLetterEvent() || Globals.DEBUG_USE_ENABLE_PLAYER_INFO___IS_ACTIVE_DAILY_LETTER_EVENT) && (WordHuntManager.HasTimeLeft() || Globals.DEBUG_USE_ENABLE_PLAYER_INFO___HAS_DAILY_WORD_TIME_LEFT))
			{
				flag = true;
			}
			if (flag)
			{
				pickupDataForPickupType.PickupEnabledState = PickupEnabledState.Enabled;
			}
			else
			{
				pickupDataForPickupType.PickupEnabledState = PickupEnabledState.NotEnabled;
			}
		}

		private bool IsCharacterNameEventActive()
		{
			return SSEventsManagerController.instance.GetActiveEventManager(SSEventType.CharacterName) != null;
		}

		private bool IsCharacterLetterPickupSpawnable()
		{
			bool result = false;
			if (Globals.DEBUG_USE_ENABLE_SS_EVENT_TYPE___LETTER_HUNT_CHARACTER)
			{
				return true;
			}
			if (IsCharacterNameEventActive())
			{
				CharacterNameEventManager typeOfActiveEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
				if (typeOfActiveEventManager != null)
				{
					result = typeOfActiveEventManager.ShouldSpawnLetters();
				}
			}
			return result;
		}

		private void RefreshEventSpecificPickups_LetterHuntCharacter()
		{
			PickupData pickupDataForPickupType = _pickupManager.GetPickupDataForPickupType(PickupType.LetterHuntCharacter);
			if (Globals.DEBUG_USE_ENABLE_SS_EVENT_TYPE___LETTER_HUNT_CHARACTER)
			{
				pickupDataForPickupType.PickupEnabledState = PickupEnabledState.Enabled;
			}
			else if (pickupDataForPickupType != null)
			{
				if (IsCharacterNameEventActive() && IsCharacterLetterPickupSpawnable())
				{
					pickupDataForPickupType.PickupEnabledState = PickupEnabledState.Enabled;
				}
				else
				{
					pickupDataForPickupType.PickupEnabledState = PickupEnabledState.NotEnabled;
				}
			}
		}

		private void PickupManager_OnPickupCollected(object sender, PickupManager.PickupEventArgs args)
		{
			if (Game.Instance.isInStumbleDeathSequence || Game.Instance.isDead)
			{
				return;
			}
			switch (args.PickupData.PickupType)
			{
			case PickupType.CoinMagnet:
				Game.Instance.Modifiers.Add(Game.Instance.Modifiers.CoinMagnet);
				GameStats.Instance.CoinMagnetPickupsCollectedIncrement();
				break;
			case PickupType.JetPack:
				Game.Instance.PickupJetpack();
				break;
			case PickupType.Key:
				InventorySave.IncreaseKeys(1, "InRun");
				GameStats.Instance.KeyPickupsCollectedIncrement();
				GameStats.Instance.AddScoreForPickup(UpgradeType.saveMeToken);
				Stat.KeysCollected.IncrementValue();
				break;
			case PickupType.LetterHuntDaily:
				CoroutineUtil.StartCoroutine(RewardDailyLetterCoroutine());
				break;
			case PickupType.LetterHuntCharacter:
			{
				CharacterLetterPickup component = args.SpawnPoint.SpawnedPickupIcon.GetComponent<CharacterLetterPickup>();
				if (component != null)
				{
					component.Reward();
				}
				break;
			}
			case PickupType.MultiplierScore2x:
				Game.Instance.Modifiers.Add(Game.Instance.Modifiers.DoubleScoreMultiplier);
				GameStats.Instance.DoubleMultiplierPickupsCollectedIncrement();
				break;
			case PickupType.MysteryBox:
				GameStats.Instance.MysteryBoxPickupsCollectedInrement();
				RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.MysteryBox, ObtainableItemSource.InGame);
				GameStats.Instance.AddScoreForPickup(UpgradeType.mysterybox);
				break;
			case PickupType.MysteryPowerup:
			{
				CharacterModifier randomModifier = MysteryPowerup.GetRandomModifier();
				randomModifier.TriggeredByMysteryPowerup = true;
				Game.Instance.Modifiers.Add(randomModifier);
				if (randomModifier is Hoverboard)
				{
					Character.Instance.MysteryPowerupParticleSystem.Play();
				}
				break;
			}
			case PickupType.PogoStick:
				Game.Instance.PickupPogostick();
				break;
			case PickupType.SuperMysteryBox:
				RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.SuperMysteryBox, ObtainableItemSource.InGame);
				GameStats.Instance.SuperMysteryBoxPickupsCollectedIncrement();
				break;
			case PickupType.SuperSneakers:
				Game.Instance.Modifiers.Add(Game.Instance.Modifiers.SuperSneakes);
				GameStats.Instance.SuperSneakerPickupsCollectedIncrement();
				break;
			case PickupType.TokenHunt:
				GameStats.Instance.HuntTokensCollectedIncrement();
				WeeklyHuntManager.ItemPickedUp();
				Character.Instance.CharacterPickupParticleSystem.PickedUpHuntToken();
				break;
			default:
				SwitchDefaultException.Throw(args.PickupData.PickupType.ToString());
				break;
			}
		}

		public static IEnumerator RewardDailyLetterCoroutine()
		{
			GameStats.Instance.AddScoreForPickup(UpgradeType.letters);
			WordHuntManager.PickedupLetter(DailyLetterPickupManager.Instance.Letter);
			GameStats.Instance.DailyLetterPickupsCollectedIncrement();
			yield return new WaitForSeconds(2f);
			DailyLetterPickupManager.Instance.UpdateLetter();
		}

		private void PickupManager_OnPickerRemoved(PickerRemoveRequestEventArgs pickerRemoveEvent)
		{
			IPicker iPicker = pickerRemoveEvent.IPicker;
			Type type = iPicker.GetType();
			if (type == typeof(XTestPicker))
			{
				if (_pickupManager._pickerCollection.HasPickerOfType<DefaultPicker>())
				{
				}
				return;
			}
			throw new UnityException("Type not yet expected. Add a case for " + type);
		}
	}
}
