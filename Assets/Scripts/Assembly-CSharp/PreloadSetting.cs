using UnityEngine;

public class PreloadSetting : MonoBehaviour
{
	[SerializeField]
	private int _preloadAmount = 1;

	public int PreloadAmount
	{
		get
		{
			return _preloadAmount;
		}
	}
}
