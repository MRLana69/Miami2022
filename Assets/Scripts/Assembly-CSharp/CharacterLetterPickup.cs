using System.Collections;
using Kiloo.Common.Utility;

public class CharacterLetterPickup : LetterPickup
{
	public char Letter
	{
		set
		{
			_letterChar = value;
			if (HasDailyLetter)
			{
				int num = _letterChar - 65;
				if (num < Letters.Count && num >= 0)
				{
					LetterMesh.mesh = Letters[num];
				}
			}
		}
	}

	private bool HasDailyLetter
	{
		get
		{
			return _letterChar != '\0';
		}
	}

	public static bool IsCharacterNameEventActive()
	{
		return SSEventsManagerController.instance.GetActiveEventManager(SSEventType.CharacterName) != null;
	}

	public static bool IsCharacterLetterPickupSpawnable()
	{
		bool result = false;
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

	private void ReportLetterWasPickedup()
	{
		if (IsCharacterNameEventActive())
		{
			CharacterNameEventManager typeOfActiveEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
			if (typeOfActiveEventManager != null)
			{
				typeOfActiveEventManager.NotifyLetterCollected(_letterChar);
			}
		}
	}

	private char? GetNextCharacterLetterToSpawn()
	{
		char? result = null;
		if (IsCharacterLetterPickupSpawnable())
		{
			CharacterNameEventManager typeOfActiveEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
			if (typeOfActiveEventManager != null)
			{
				result = typeOfActiveEventManager.NextLetterToSpawn();
			}
		}
		return result;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		if (IsCharacterLetterPickupSpawnable())
		{
			char? nextCharacterLetterToSpawn = GetNextCharacterLetterToSpawn();
			if (nextCharacterLetterToSpawn.HasValue)
			{
				Letter = nextCharacterLetterToSpawn.Value;
			}
			CharacterNameEventManager.OnLetterCollected += CharacterNameEventManager_OnLetterCollected;
			SSEventsManagerController.OnSSEventChangedHandler += SSEventsManagerController_OnSSEventChangedHandler;
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		CharacterNameEventManager.OnLetterCollected -= CharacterNameEventManager_OnLetterCollected;
		SSEventsManagerController.OnSSEventChangedHandler -= SSEventsManagerController_OnSSEventChangedHandler;
	}

	private void DeactivatePickupIfEventHasCompleted()
	{
		CharacterNameEventManager typeOfActiveEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
		if (typeOfActiveEventManager == null || typeOfActiveEventManager.HasCompletedCollectionForSelectedCharacter())
		{
			OnDeactivate();
		}
	}

	private void CharacterNameEventManager_OnLetterCollected(object sender, CharacterNameEventManager.LetterCollectedEventArgs letterCollectedEventArgs)
	{
		DeactivatePickupIfEventHasCompleted();
	}

	private void SSEventsManagerController_OnSSEventChangedHandler()
	{
		DeactivatePickupIfEventHasCompleted();
	}

	public void Reward()
	{
		GameStats.Instance.CharacterLetterPickupsCollectedIncrement();
		CoroutineUtil.StartCoroutine(RewardCoroutine());
	}

	private IEnumerator RewardCoroutine()
	{
		ReportLetterWasPickedup();
		yield return null;
	}
}
