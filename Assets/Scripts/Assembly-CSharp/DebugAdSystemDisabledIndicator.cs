using Data.Save;
using UnityEngine;

public class DebugAdSystemDisabledIndicator : MonoBehaviour
{
	[SerializeField]
	private GameObject _indicatorObject;

	private void Awake()
	{
		if (_indicatorObject != null)
		{
			_indicatorObject.SetActive(Globals.DEBUG && DebugSave.disableAdSystem);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}
}
