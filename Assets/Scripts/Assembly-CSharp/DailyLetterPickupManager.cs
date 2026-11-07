using System.Collections.Generic;
using SYBO.Common.Utilities;
using UnityEngine;
using WordHunt;

public class DailyLetterPickupManager : MonoBehaviour
{
	public const char NO_LETTER = '\0';

	private char letter;

	private HashSet<DailyLetterPickup> pickups = new HashSet<DailyLetterPickup>();

	private static DailyLetterPickupManager instance;

	public char Letter
	{
		get
		{
			return letter;
		}
	}

	public static DailyLetterPickupManager Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject gameObject = new GameObject("DailyLetterPickupManager");
				instance = gameObject.AddComponent<DailyLetterPickupManager>();
				instance.letter = WordHuntManager.GetNewDailyLetter();
				HierarchyUtility.ReparentByHierarchyPathType(gameObject.transform, HierarchyPathType.Dynamic);
				HierarchyUtility.SetAsLastSibling(gameObject.transform);
			}
			return instance;
		}
	}

	private void NotifyPickups()
	{
		foreach (DailyLetterPickup pickup in pickups)
		{
			if (pickup != null)
			{
				pickup.Letter = letter;
			}
		}
	}

	public void InitializePickup(DailyLetterPickup pickup)
	{
		pickups.Add(pickup);
		pickup.Letter = letter;
	}

	public void UpdateLetter()
	{
		letter = WordHuntManager.GetNewDailyLetter();
		NotifyPickups();
	}
}
