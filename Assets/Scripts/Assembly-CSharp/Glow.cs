using System.Collections.Generic;
using System.Linq;
using SYBO.Subway.Coins;
using UnityEngine;

public class Glow : MonoBehaviour
{
	private MeshRenderer meshRenderer;

	private bool _isDebugGlowEnabled;

	public bool IsDebugGlowEnabled
	{
		get
		{
			return _isDebugGlowEnabled;
		}
		internal set
		{
			_isDebugGlowEnabled = value;
			List<Transform> list = GetComponentsInChildren<Transform>().ToList();
			foreach (Transform item in list)
			{
				if (_isDebugGlowEnabled)
				{
					item.localScale = new Vector3(1f, 1f, 1f);
				}
				else
				{
					item.localScale = new Vector3(0f, 0f, 0f);
				}
			}
		}
	}

	public void Awake()
	{
		if (DeviceInfo.Instance.performanceLevel == DeviceInfo.PerformanceLevel.Low && base.transform.GetComponentInParent<Coin>() != null)
		{
			foreach (Transform item in base.transform)
			{
				Object.Destroy(item.gameObject);
			}
			meshRenderer = GetComponentInChildren<MeshRenderer>();
			if (meshRenderer != null)
			{
				meshRenderer.enabled = false;
				base.enabled = false;
				Object.Destroy(meshRenderer);
				meshRenderer = null;
			}
		}
		else
		{
			meshRenderer = GetComponentInChildren<MeshRenderer>();
		}
	}

	public void SetVisible(bool visible)
	{
		if (meshRenderer != null)
		{
			meshRenderer.enabled = visible;
			base.enabled = visible;
		}
	}
}
