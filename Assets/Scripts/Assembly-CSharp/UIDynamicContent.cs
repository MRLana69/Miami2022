using UnityEngine;

public class UIDynamicContent : MonoBehaviour
{
	public GameObject[] PanelElements;

	private void Start()
	{
		InitElements();
	}

	public void InitElements()
	{
		for (int i = 0; i < PanelElements.Length; i++)
		{
			base.gameObject.AddChild(PanelElements[i]);
		}
	}
}
