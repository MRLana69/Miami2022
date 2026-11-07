using UnityEngine;

public class EnemiesRig : MonoBehaviour
{
	[SerializeField]
	private Animation _guardAnimation;

	[SerializeField]
	private Animation _dogAnimation;

	public Animation GuardAnimation
	{
		get
		{
			return _guardAnimation;
		}
	}

	public Animation DogAnimation
	{
		get
		{
			return _dogAnimation;
		}
	}
}
