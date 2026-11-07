using SYBO.Subway;
using SYBO.Subway.Pickups;
using UnityEngine;

public class PickupDefault : MonoBehaviour
{
	public delegate void ActivateDelegate();

	public MeshRenderer meshRenderer;

	public Glow glow;

	public MeshRenderer[] rings;

	public bool ShouldSpawnParticles;

	private Collider parentCollider;

	public ActivateDelegate ActivatePickup;

	public bool IsVisible
	{
		get
		{
			if (meshRenderer != null)
			{
				return meshRenderer.enabled;
			}
			return false;
		}
	}

	public bool IsActive { get; private set; }

	private void Awake()
	{
		TrackObject trackObject = GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>();
		trackObject.OnActivate += OnActivate;
		trackObject.OnDeactivate += OnDeactivate;
		parentCollider = FindParentCollider(base.transform);
		if (!(parentCollider == null) || Game.IsInstantiated)
		{
		}
		SetVisible(false);
	}

	public static PickupType ConvertSubclassInstanceToPickupType(PickupDefault pickupDefault)
	{
		string text = pickupDefault.name;
		if (text.Contains("jetpackPickup"))
		{
			return PickupType.JetPack;
		}
		if (text.Contains("jumpBooster"))
		{
			return PickupType.SuperSneakers;
		}
		if (text.Contains("doubleScoreMultiplier"))
		{
			return PickupType.MultiplierScore2x;
		}
		if (text.Contains("magnetBooster"))
		{
			return PickupType.CoinMagnet;
		}
		SDebug.LogError("Unexpected value here: " + text + ". Fix immediately");
		return PickupType.JetPack;
	}

	private Collider FindParentCollider(Transform current)
	{
		if (current.GetComponent<Collider>() != null)
		{
			return current.GetComponent<Collider>();
		}
		if (current.parent != null)
		{
			return FindParentCollider(current.parent);
		}
		return null;
	}

	private void SetVisible(bool isVisible)
	{
		if (this.meshRenderer != null)
		{
			this.meshRenderer.enabled = isVisible;
		}
		if (glow != null)
		{
			glow.SetVisible(isVisible);
		}
		if (rings != null)
		{
			MeshRenderer[] array = rings;
			foreach (MeshRenderer meshRenderer in array)
			{
				meshRenderer.enabled = isVisible;
			}
		}
		IsActive = isVisible;
	}

	private void OnActivate()
	{
		parentCollider.enabled = true;
		if (ActivatePickup != null)
		{
			ActivatePickup();
		}
		SetVisible(true);
	}

	private void OnDeactivate()
	{
		SetVisible(false);
	}

	private void OnValidate()
	{
		MeshFilter[] componentsInChildren = GetComponentsInChildren<MeshFilter>(true);
		MeshFilter[] array = componentsInChildren;
		foreach (MeshFilter meshFilter in array)
		{
			if (meshFilter.sharedMesh == null)
			{
			}
		}
	}

	public void Activate()
	{
		base.gameObject.SetActive(true);
		if (ActivatePickup != null)
		{
			ActivatePickup();
		}
		if (parentCollider != null)
		{
			parentCollider.enabled = true;
		}
		SetVisible(true);
	}

	public void Deactivate()
	{
		base.gameObject.SetActive(false);
		SetVisible(false);
	}
}
