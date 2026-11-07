using System;
using System.Diagnostics;
using System.Threading;
using Adverty.AdUnit;
using Adverty.DebugMode;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using NaughtyAttributes;
using UnityEngine;

[ExecuteInEditMode]
public class BillboardAdPlacement : MonoBehaviour
{
	private const float BOX_RATIO = 1.2f;

	private const float PORTRAIT_RATIO = 0.5f;

	private const float LANDSCAPE_RATIO = 1.7777778f;

	private const float WIDE_LANDSCAPE_RATIO = 3.88f;

	[SerializeField]
	[ShowIf("ShowBillboardSettings")]
	private Transform _billboardAdParent;

	[SerializeField]
	[ShowIf("ShowBillboardSettings")]
	private MeshRenderer _billboardAdMeshRenderer;

	[SerializeField]
	[ShowIf("ShowBillboardSettings")]
	private Transform _billboardAdTransform;

	[SerializeField]
	private MeshData.UnitRatio _ratio;

	[SerializeField]
	private float _size;

	[SerializeField]
	private MeshData.WidthHeight _match;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnBillboardAdUnitSpawned__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnBillboardAdUnitActivated__BackingField;

	public static event Action OnBillboardAdUnitSpawned
	{
		add
		{
			Action action = OnBillboardAdUnitSpawned__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnBillboardAdUnitSpawned__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnBillboardAdUnitSpawned__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnBillboardAdUnitSpawned__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnBillboardAdUnitActivated
	{
		add
		{
			Action action = OnBillboardAdUnitActivated__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnBillboardAdUnitActivated__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnBillboardAdUnitActivated__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnBillboardAdUnitActivated__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	[UsedImplicitly]
	private bool ShowBillboardSettings()
	{
		return false;
	}

	private void Awake()
	{
		if (Application.isPlaying)
		{
			if (BillboardAdsManager.IsBillboardAdsEnabled)
			{
				SpawnUnit();
				OnBillboardAdUnitSpawned__BackingField.SafeInvoke();
			}
			_billboardAdMeshRenderer.enabled = false;
		}
	}

	private void OnEnable()
	{
		OnBillboardAdUnitActivated__BackingField.SafeInvoke();
	}

	private void SpawnUnit()
	{
		Configuration config = CreateUnitConfiguration();
		GameObject gameObject = UnitFactory.Create(config);
		SetSpawnPosition(gameObject);
		DisableMeshRenderer(gameObject);
		AddImpVizIfNeeded(gameObject);
	}

	private void SetSpawnPosition(GameObject spawnedUnitGo)
	{
		spawnedUnitGo.transform.parent = _billboardAdParent;
		spawnedUnitGo.transform.localPosition = Vector3.zero;
		spawnedUnitGo.transform.localRotation = Quaternion.Euler(Vector3.zero);
	}

	public void DisableMeshRenderer(GameObject spawnedUnitGo)
	{
		MeshRenderer component = spawnedUnitGo.GetComponent<MeshRenderer>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	private void AddImpVizIfNeeded(GameObject spawnedUnitGo)
	{
		if (Globals.DEBUG_BILLBOARDS_IMPVIZ_ENABLED)
		{
			spawnedUnitGo.AddComponent<ImpVizComponent>();
		}
	}

	private Configuration CreateUnitConfiguration()
	{
		return new Configuration
		{
			MeshData = new MeshData
			{
				Size = _size,
				MatchWidthOrHeight = _match,
				Ratio = _ratio
			},
			Behavior = new UnitBehavior
			{
				ImmediateActivation = true
			},
			SceneContext = new SceneContext
			{
				Lighting = SceneContext.SceneLighting.Light,
				Locations = SceneContext.Location.City,
				Period = SceneContext.TimePeriod.Present,
				Setting = SceneContext.AdSetting.Outdoor,
				Size = SceneContext.UnitSize.L,
				Time = SceneContext.SceneTime.Day
			}
		};
	}
}
