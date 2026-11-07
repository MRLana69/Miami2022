using UnityEngine;

public class DebugBreakOnDisable : MonoBehaviour
{
	private void OnDisable()
	{
		Debug.Break();
	}
}
