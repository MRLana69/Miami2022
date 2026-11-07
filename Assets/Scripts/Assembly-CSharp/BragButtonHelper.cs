using System;
using UnityEngine;

public class BragButtonHelper : MonoBehaviour
{
	private UIButtonOverlayOff uiButtonOverlayOff;

	[NonSerialized]
	public bool buttonEnabled;

	public void EnableButton()
	{
		if (uiButtonOverlayOff == null)
		{
			uiButtonOverlayOff = GetComponent<UIButtonOverlayOff>();
		}
		NGUITools.AddWidgetCollider(base.gameObject);
		uiButtonOverlayOff.GoToNormalState();
		buttonEnabled = true;
	}

	public void DisableButton()
	{
		if (uiButtonOverlayOff == null)
		{
			uiButtonOverlayOff = GetComponent<UIButtonOverlayOff>();
		}
		if (base.gameObject.GetComponent<Collider>() != null)
		{
			UnityEngine.Object.Destroy(base.gameObject.GetComponent<Collider>());
		}
		uiButtonOverlayOff.GoToNotAvailableState();
		buttonEnabled = false;
	}
}
