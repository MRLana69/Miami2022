public class DailyLetterPickup : LetterPickup
{
	public char Letter
	{
		set
		{
			_letterChar = value;
			int num = _letterChar - 65;
			if (num < Letters.Count && num >= 0)
			{
				LetterMesh.mesh = Letters[num];
			}
		}
	}

	private void Awake()
	{
		DailyLetterPickupManager.Instance.InitializePickup(this);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		Letter = DailyLetterPickupManager.Instance.Letter;
	}
}
