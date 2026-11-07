using System.Collections.Generic;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using SYBO.Subway.Level;
using SYBO.Subway.Themes;
using UnityEngine;

namespace SYBO.Subway.Managers
{
	public class AssetManager : SingletonMonoBehaviour<AssetManager>
	{
		private Dictionary<EnvironmentBaseInfo, Transform> _combinedEnvBaseModels = new Dictionary<EnvironmentBaseInfo, Transform>();

		protected override void SubclassInitialize()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
			CombineGroundModels();
		}

		public Transform GetEnvironmentBaseModel(EnvironmentBaseInfo envBaseInfo)
		{
			Transform value = null;
			_combinedEnvBaseModels.TryGetValue(envBaseInfo, out value);
			return value;
		}

		private void CreateModelInstances(out GroundAssetGroup instances)
		{
			GroundAssetGroup groundAssets = ThemeManager.Instance.Theme.GroundAssets;
			instances.Ground = SYBO.Common.Utilities.GameObjectUtil.InstantiateTransform(groundAssets.Ground);
			instances.GroundShadowMid = SYBO.Common.Utilities.GameObjectUtil.InstantiateTransform(groundAssets.GroundShadowMid);
			instances.GroundShortShadowStart = SYBO.Common.Utilities.GameObjectUtil.InstantiateTransform(groundAssets.GroundShortShadowStart);
			instances.GroundShortShadowEnd = SYBO.Common.Utilities.GameObjectUtil.InstantiateTransform(groundAssets.GroundShortShadowEnd);
			instances.Track = SYBO.Common.Utilities.GameObjectUtil.InstantiateTransform(groundAssets.Track);
			instances.TrackShadowStart = SYBO.Common.Utilities.GameObjectUtil.InstantiateTransform(groundAssets.TrackShadowStart);
			instances.TrackShadowMid = SYBO.Common.Utilities.GameObjectUtil.InstantiateTransform(groundAssets.TrackShadowMid);
			instances.TrackShadowEnd = SYBO.Common.Utilities.GameObjectUtil.InstantiateTransform(groundAssets.TrackShadowEnd);
			instances.TrackShortShadowStart = SYBO.Common.Utilities.GameObjectUtil.InstantiateTransform(groundAssets.TrackShortShadowStart);
			instances.TrackShortShadowEnd = SYBO.Common.Utilities.GameObjectUtil.InstantiateTransform(groundAssets.TrackShortShadowEnd);
			instances.TracksGates = SYBO.Common.Utilities.GameObjectUtil.InstantiateTransform(groundAssets.TracksGates);
			instances.TracksGatesShadow = SYBO.Common.Utilities.GameObjectUtil.InstantiateTransform(groundAssets.TracksGatesShadow);
			instances.TracksSpecial = SYBO.Common.Utilities.GameObjectUtil.InstantiateTransform(groundAssets.TracksSpecial);
		}

		private void DestroyModelInstances(ref GroundAssetGroup ga)
		{
			Object.Destroy(ga.Ground.gameObject);
			Object.Destroy(ga.GroundShadowMid.gameObject);
			Object.Destroy(ga.GroundShortShadowStart.gameObject);
			Object.Destroy(ga.GroundShortShadowEnd.gameObject);
			Object.Destroy(ga.Track.gameObject);
			Object.Destroy(ga.TrackShadowStart.gameObject);
			Object.Destroy(ga.TrackShadowMid.gameObject);
			Object.Destroy(ga.TrackShadowEnd.gameObject);
			Object.Destroy(ga.TrackShortShadowStart.gameObject);
			Object.Destroy(ga.TrackShortShadowEnd.gameObject);
			Object.Destroy(ga.TracksGates.gameObject);
			Object.Destroy(ga.TracksGatesShadow.gameObject);
			Object.Destroy(ga.TracksSpecial.gameObject);
		}

		public void CombineGroundModels()
		{
			GroundAssetGroup[] array = new GroundAssetGroup[3];
			CreateModelInstances(out array[0]);
			CreateModelInstances(out array[1]);
			CreateModelInstances(out array[2]);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks0, array[0].Ground, array[1].Ground, array[2].Ground);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks1, array[0].Ground, array[1].Track, array[2].Ground);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks1ShadowMid, array[0].GroundShadowMid, array[1].TrackShadowMid, array[2].GroundShadowMid);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks1ShortShadowStart, array[0].GroundShortShadowStart, array[1].TrackShortShadowStart, array[2].GroundShortShadowStart);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks1ShortShadowEnd, array[0].GroundShortShadowEnd, array[1].TrackShortShadowEnd, array[2].GroundShortShadowEnd);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks2, array[0].Track, array[1].Ground, array[2].Track, 4);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks2ShadowMid, array[0].TrackShadowMid, array[1].GroundShadowMid, array[2].TrackShadowMid, 8);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks2ShortShadowStart, array[0].TrackShortShadowStart, array[1].GroundShortShadowStart, array[2].TrackShortShadowStart, 4);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks2ShortShadowEnd, array[0].TrackShortShadowEnd, array[1].GroundShortShadowEnd, array[2].TrackShortShadowEnd, 4);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks3, array[0].Track, array[1].Track, array[2].Track, 18);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks3ShadowStart, array[0].TrackShadowStart, array[1].TrackShadowStart, array[2].TrackShadowStart, 4);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks3ShadowMid, array[0].TrackShadowMid, array[1].TrackShadowMid, array[2].TrackShadowMid, 15);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks3ShadowEnd, array[0].TrackShadowEnd, array[1].TrackShadowEnd, array[2].TrackShadowEnd);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks3ShortShadowStart, array[0].TrackShortShadowStart, array[1].TrackShortShadowStart, array[2].TrackShortShadowStart);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks3ShortShadowEnd, array[0].TrackShortShadowEnd, array[1].TrackShortShadowEnd, array[2].TrackShortShadowEnd);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks3Gates, null, array[0].TracksGates, null);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks3GatesShadow, null, array[0].TracksGatesShadow, null);
			CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo.Tracks3Special, null, array[0].TracksSpecial, null);
			DestroyModelInstances(ref array[0]);
			DestroyModelInstances(ref array[1]);
			DestroyModelInstances(ref array[2]);
			PoolManager.TriggerPoolBalancing();
		}

		public void CombineEnvironmentBaseModelsAndStore(EnvironmentBaseInfo envBaseInfo, Transform left, Transform mid, Transform right, int preloadAmount = 2)
		{
			EnvironmentBaseKind environmentBaseKind = envBaseInfo.EnvironmentBaseKind;
			if (environmentBaseKind.Type != EnvironmentBaseType.Gates)
			{
				EnvironmentBaseKind environmentBaseKind2 = envBaseInfo.EnvironmentBaseKind;
				if (environmentBaseKind2.Type != EnvironmentBaseType.Special && (left == null || right == null))
				{
					SDebug.LogError(string.Concat("Can't combine regular track models (not Gates/Special), one of the sides is null, left = '", left, "' right = '", right, "'"));
					return;
				}
			}
			string text = EnvironmentBaseInfo.GetName(envBaseInfo);
			GameObject gameObject = new GameObject(text);
			Transform transform = gameObject.transform;
			if (left != null)
			{
				left.parent = transform;
				left.localPosition = new Vector3(-20f, 0f, 0f);
			}
			mid.parent = transform;
			mid.localPosition = Vector3.zero;
			if (right != null)
			{
				right.parent = transform;
				right.localPosition = new Vector3(20f, 0f, 0f);
			}
			ModelInfo modelInfo = MeshUtil.CombineMeshes(gameObject, true);
			if (modelInfo != null)
			{
				MeshFilter meshFilter = gameObject.AddComponentIfMissing<MeshFilter>();
				MeshRenderer meshRenderer = gameObject.AddComponentIfMissing<MeshRenderer>();
				meshFilter.sharedMesh = modelInfo.Mesh;
				meshFilter.sharedMesh.name = text;
				meshRenderer.materials = modelInfo.Materials;
			}
			transform.DetachChildren();
			_combinedEnvBaseModels.Add(envBaseInfo, transform);
			transform.parent = base.transform;
			transform.localPosition = Vector3.down * 500f;
			PoolManager.GetPrefabPool(transform).preloadAmount = preloadAmount;
		}

		public static Transform[] GetEnvironmentBasePrefabs(EnvironmentBaseInfo envBaseInfo)
		{
			GroundAssetGroup groundAssets = ThemeManager.Instance.Theme.GroundAssets;
			return GetPrefabsForBaseInfo(groundAssets, envBaseInfo);
		}

		public static Transform[] GetPrefabsForBaseInfo(GroundAssetGroup ga, EnvironmentBaseInfo envBaseInfo)
		{
			Transform[] array = new Transform[3];
			if (envBaseInfo == EnvironmentBaseInfo.Tracks0)
			{
				array[0] = ga.Ground;
				array[1] = ga.Ground;
				array[2] = ga.Ground;
			}
			else if (envBaseInfo == EnvironmentBaseInfo.Tracks1)
			{
				array[0] = ga.Ground;
				array[1] = ga.Track;
				array[2] = ga.Ground;
			}
			else if (envBaseInfo == EnvironmentBaseInfo.Tracks1ShadowMid)
			{
				array[0] = ga.GroundShadowMid;
				array[1] = ga.TrackShadowMid;
				array[2] = ga.GroundShadowMid;
			}
			else if (envBaseInfo == EnvironmentBaseInfo.Tracks1ShortShadowStart)
			{
				array[0] = ga.GroundShortShadowStart;
				array[1] = ga.TrackShortShadowStart;
				array[2] = ga.GroundShortShadowStart;
			}
			else if (envBaseInfo == EnvironmentBaseInfo.Tracks1ShortShadowEnd)
			{
				array[0] = ga.GroundShortShadowEnd;
				array[1] = ga.TrackShortShadowEnd;
				array[2] = ga.GroundShortShadowEnd;
			}
			else if (envBaseInfo == EnvironmentBaseInfo.Tracks2)
			{
				array[0] = ga.Track;
				array[1] = ga.Ground;
				array[2] = ga.Track;
			}
			else if (envBaseInfo == EnvironmentBaseInfo.Tracks2ShadowMid)
			{
				array[0] = ga.TrackShadowMid;
				array[1] = ga.GroundShadowMid;
				array[2] = ga.TrackShadowMid;
			}
			else if (envBaseInfo == EnvironmentBaseInfo.Tracks2ShortShadowStart)
			{
				array[0] = ga.TrackShortShadowStart;
				array[1] = ga.GroundShortShadowStart;
				array[2] = ga.TrackShortShadowStart;
			}
			else if (envBaseInfo == EnvironmentBaseInfo.Tracks2ShortShadowEnd)
			{
				array[0] = ga.TrackShortShadowEnd;
				array[1] = ga.GroundShortShadowEnd;
				array[2] = ga.TrackShortShadowEnd;
			}
			else if (envBaseInfo == EnvironmentBaseInfo.Tracks3)
			{
				array[0] = ga.Track;
				array[1] = ga.Track;
				array[2] = ga.Track;
			}
			else if (envBaseInfo == EnvironmentBaseInfo.Tracks3ShadowStart)
			{
				array[0] = ga.TrackShadowStart;
				array[1] = ga.TrackShadowStart;
				array[2] = ga.TrackShadowStart;
			}
			else if (envBaseInfo == EnvironmentBaseInfo.Tracks3ShadowMid)
			{
				array[0] = ga.TrackShadowMid;
				array[1] = ga.TrackShadowMid;
				array[2] = ga.TrackShadowMid;
			}
			else if (envBaseInfo == EnvironmentBaseInfo.Tracks3ShadowEnd)
			{
				array[0] = ga.TrackShadowEnd;
				array[1] = ga.TrackShadowEnd;
				array[2] = ga.TrackShadowEnd;
			}
			else if (envBaseInfo == EnvironmentBaseInfo.Tracks3ShortShadowStart)
			{
				array[0] = ga.TrackShortShadowStart;
				array[1] = ga.TrackShortShadowStart;
				array[2] = ga.TrackShortShadowStart;
			}
			else if (envBaseInfo == EnvironmentBaseInfo.Tracks3ShortShadowEnd)
			{
				array[0] = ga.TrackShortShadowEnd;
				array[1] = ga.TrackShortShadowEnd;
				array[2] = ga.TrackShortShadowEnd;
			}
			else if (envBaseInfo == EnvironmentBaseInfo.Tracks3Gates)
			{
				array[1] = ga.TracksGates;
			}
			else if (envBaseInfo == EnvironmentBaseInfo.Tracks3GatesShadow)
			{
				array[1] = ga.TracksGatesShadow;
			}
			else if (envBaseInfo == EnvironmentBaseInfo.Tracks3Special)
			{
				array[1] = ga.TracksSpecial;
			}
			return array;
		}
	}
}
