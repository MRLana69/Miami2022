using System;
using System.Diagnostics;
using System.Threading;
using Data.Managers;
using Info;
using Kiloo.Common.Extensions;
using SYBO.Subway.Info;
using UI.SlideIns;

namespace UI.Screens
{
	public class CharacterScreenManager
	{
		public enum CharacterPurchaseInitiator
		{
			none = 0,
			characterScreen = 1,
			characterNameEvent = 2,
			limitedCharacterPopup = 3,
			BirthdayCharacterComeback = 4
		}

		private static CharacterScreenManager _instance;

		private bool _purchaseInProgress;

		private bool _facebookLoginRunning;

		private CharacterPurchaseInitiator _characterPurchaseInitiator;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<Characters.CharacterId, int> characterUnlocked__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action characterUnlockedFailure__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onCharacterSelected__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onFacebookStatusChanged__BackingField;

		public Characters.CharacterId currentlyShownCharacter { get; set; }

		public int currentlyShownThemeIndex { get; set; }

		public static CharacterScreenManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CharacterScreenManager();
				}
				return _instance;
			}
		}

		public static event Action<Characters.CharacterId, int> characterUnlocked
		{
			add
			{
				Action<Characters.CharacterId, int> action = characterUnlocked__BackingField;
				Action<Characters.CharacterId, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref characterUnlocked__BackingField, (Action<Characters.CharacterId, int>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<Characters.CharacterId, int> action = characterUnlocked__BackingField;
				Action<Characters.CharacterId, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref characterUnlocked__BackingField, (Action<Characters.CharacterId, int>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action characterUnlockedFailure
		{
			add
			{
				Action action = characterUnlockedFailure__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref characterUnlockedFailure__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = characterUnlockedFailure__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref characterUnlockedFailure__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onCharacterSelected
		{
			add
			{
				Action action = onCharacterSelected__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onCharacterSelected__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onCharacterSelected__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onCharacterSelected__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onFacebookStatusChanged
		{
			add
			{
				Action action = onFacebookStatusChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onFacebookStatusChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onFacebookStatusChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onFacebookStatusChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		private CharacterScreenManager()
		{
		}

		public void PurchaseCharacter(CharacterPurchaseInitiator initiator, Characters.CharacterId characterId, int themeIndex, int priceOverride = -1)
		{
			if (!_purchaseInProgress)
			{
				_purchaseInProgress = true;
				_characterPurchaseInitiator = initiator;
				PurchaseHandler.Instance.PurchaseCharacter(characterId, themeIndex, priceOverride);
			}
		}

		public void PurchaseCharacterWithInapp(CharacterPurchaseInitiator initiator, Characters.CharacterId characterId)
		{
			if (!_purchaseInProgress)
			{
				_purchaseInProgress = true;
				_characterPurchaseInitiator = initiator;
				PurchaseHandler.Instance.StartPurchaseCharacterWithInapp(characterId);
			}
		}

		public void CharacterPurchaseSuccessful(Characters.CharacterId purchasedCharacter, int themeIndex)
		{
			if (_purchaseInProgress)
			{
				if (_characterPurchaseInitiator == CharacterPurchaseInitiator.characterNameEvent && themeIndex == 0 && Characters.characterData[purchasedCharacter].unlockType == PurchaseData.UnlockType.Coins)
				{
					Flurry.LogEventWithSeveralParameters("Unlocked character for Wordy Weekend", "Character;Price", string.Concat(purchasedCharacter, ";", Characters.characterData[purchasedCharacter].Price));
				}
				_purchaseInProgress = false;
				_characterPurchaseInitiator = CharacterPurchaseInitiator.none;
				OnCharacterUnlocked(purchasedCharacter, themeIndex);
			}
		}

		public void CharacterPurchaseFailure()
		{
			if (_purchaseInProgress)
			{
				_characterPurchaseInitiator = CharacterPurchaseInitiator.none;
				_purchaseInProgress = false;
			}
			if (characterUnlockedFailure__BackingField != null)
			{
				characterUnlockedFailure__BackingField();
			}
		}

		private void OnCharacterUnlocked(Characters.CharacterId character, int outfit)
		{
			if (characterUnlocked__BackingField != null)
			{
				SelectCharacter(character, outfit);
				characterUnlocked__BackingField(character, outfit);
			}
		}

		public void SelectCharacter(Characters.CharacterId charId, int themeIndex)
		{
			bool flag = CharacterManager.IsCharacterOwned(charId);
			bool flag2 = CharacterManager.IsOutfitUnlocked(charId, themeIndex);
			if (flag && flag2)
			{
				UIModelController.Instance.SelectCharacterForPlay(charId, themeIndex);
				onCharacterSelected__BackingField.SafeInvoke();
			}
		}

		public void StartLoginToFacebook()
		{
			HouseKeeper.SetFacebookPayoutAction(FacebookStatusChanged);
			_facebookLoginRunning = true;
			if (SocialManager.instance.FacebookLogin(OnFacebookLoginResult))
			{
				UIScreenController.Instance.FacebookLogIn(true);
			}
		}

		public void OnFacebookLoginResult(bool loggedIn)
		{
			if (!loggedIn)
			{
				QueueFacebookFailSlideIn(OnFacebookLoginStopped);
			}
			else
			{
				_facebookLoginRunning = false;
			}
		}

		private void OnFacebookLoginStopped()
		{
			_facebookLoginRunning = false;
			onFacebookStatusChanged__BackingField.SafeInvoke();
		}

		private void QueueFacebookFailSlideIn(Action onFinishCallback)
		{
			UIScreenController.Instance.QueueSlideIn(new SlideIn
			{
				type = SlideInData.SlideInType.Facebook,
				facebookState = UISlideInFacebook.FacebookSlideInState.ConnectFailed,
				onDidShowCallback = onFinishCallback
			});
		}

		private void FacebookStatusChanged(bool success)
		{
			HouseKeeper.SetFacebookPayoutActionToDefault();
			if (success)
			{
				_facebookLoginRunning = false;
				if (!FacebookRewardsInfo.PayoutRewardIfPossible())
				{
					OnFacebookCharacterUnlocked();
					UIScreenController.Instance.QueueSlideIn(new SlideIn
					{
						type = SlideInData.SlideInType.Facebook,
						facebookState = UISlideInFacebook.FacebookSlideInState.ConnectSucceeded
					});
				}
			}
		}

		public void OnFacebookCharacterUnlocked()
		{
			OnCharacterUnlocked(Characters.CharacterId.facebookRewardCharacter01, 0);
		}

		public void OnFirstPurchaseCharacterUnlocked()
		{
			OnCharacterUnlocked(Characters.CharacterId.firstPurchaseBonusCharacter01, 0);
		}

		public bool IsFacebookLoginRunning()
		{
			return _facebookLoginRunning;
		}

		public void OnFacebookLogout()
		{
			onFacebookStatusChanged__BackingField.SafeInvoke();
		}

		public void OnCharacterScreenHide()
		{
			if (_facebookLoginRunning)
			{
				HouseKeeper.SetFacebookPayoutActionToDefault();
			}
		}

		public Characters.CharacterId? GetCharacterTypeActiveInEvent()
		{
			Characters.CharacterId? result = null;
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			if (activeEventManager != null && (activeEventManager.GetEventType() == SSEventType.CharacterDay || activeEventManager.GetEventType() == SSEventType.ExpiredLimitedCharacter))
			{
				bool found = false;
				result = activeEventManager.GetEventValue<Characters.CharacterId>(out found);
			}
			return result;
		}

		public int GetLastSelectedThemeForCharacterType(Characters.CharacterId charId)
		{
			int result = 0;
			if (Outfits.GetOutfitForCharacter(charId, CharacterManager.GetOutfitIndex(charId)) != null)
			{
				result = CharacterManager.GetOutfitIndex(charId);
			}
			return result;
		}
	}
}
