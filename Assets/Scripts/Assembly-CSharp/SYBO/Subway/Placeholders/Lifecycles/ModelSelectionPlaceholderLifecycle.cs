using System.Collections;
using System.Collections.Generic;
using SYBO.Common;
using SYBO.Common.Utilities;
using SYBO.Common.Visuals;
using SYBO.Components.Utilities;
using SYBO.Subway.Level;
using SYBO.Subway.Placeholders.PrefabProviders;
using UnityEngine;

namespace SYBO.Subway.Placeholders.Lifecycles
{
	public class ModelSelectionPlaceholderLifecycle : SelectionPlaceholderLifecycle
	{
		[SerializeField]
		private bool _combineModelsInstantly;

		private MeshContainer _meshContainer;

		private MeshFilter _meshFilter;

		private MeshRenderer _meshRenderer;

		private bool _usingCombinedMesh;

		private CombineableMeshGroup _spawnedCombineableMeshGroup;

		private bool _isCombiningMeshes;

		private IEnumerator _meshCombiningRoutine;

		private bool _spawnedOnceOnStart;

		protected override void Initialize()
		{
			base.Initialize();
			_meshContainer = SingletonMonoBehaviour<MeshContainer>.Instance;
			if (Application.isPlaying)
			{
				_meshFilter = (MeshFilter)this.AddComponentIfMissing(typeof(MeshFilter));
				_meshRenderer = (MeshRenderer)this.AddComponentIfMissing(typeof(MeshRenderer));
			}
		}

		protected override void SpawnInstances(PlaceholderPrefabProviderBase prefabProvider, bool spawnInChildrenLifecycles = false, bool spawningOnceOnStart = false, bool locatedInIntro = false)
		{
			List<Transform> prefabsToSpawn = prefabProvider.PrefabsToSpawn;
			if (1 < prefabsToSpawn.Count && Application.isEditor)
			{
				SDebug.LogError("ModelSelectionPlaceholderLifecycle: Multiple group combining not implemented");
			}
			Transform transform = prefabsToSpawn[0];
			_spawnedCombineableMeshGroup = transform.GetComponent(typeof(CombineableMeshGroup)) as CombineableMeshGroup;
			if (_spawnedCombineableMeshGroup == null)
			{
				if (Application.isEditor)
				{
					SDebug.LogError("ModelSelectionPlaceholderLifecycle: spawned object without CombineableMeshGroup");
				}
				base.SpawnInstances(prefabProvider, spawnInChildrenLifecycles, spawningOnceOnStart, locatedInIntro);
				return;
			}
			if (_spawnedCombineableMeshGroup != null)
			{
				ModelInfo combinedMeshInfo;
				if (_meshContainer.TryGetGroupMesh(_spawnedCombineableMeshGroup, out combinedMeshInfo))
				{
					_meshFilter.mesh = combinedMeshInfo.Mesh;
					_meshRenderer.materials = combinedMeshInfo.Materials;
					_meshRenderer.enabled = true;
					_usingCombinedMesh = true;
				}
				else if (_meshContainer.IsMeshGroupCombineInProgress(_spawnedCombineableMeshGroup))
				{
					_meshContainer.AddMeshGroupCombinedCallback(_spawnedCombineableMeshGroup, UpdateToCombinedMesh);
				}
				else
				{
					base.SpawnInstances(prefabProvider, true, spawningOnceOnStart, locatedInIntro);
					CombineableMesh[] componentsInChildren = GetComponentsInChildren<CombineableMesh>(true);
					if (0 < componentsInChildren.Length)
					{
						if (spawningOnceOnStart || _combineModelsInstantly)
						{
							CombineMeshes(componentsInChildren);
						}
						else
						{
							_meshContainer.NotifyMeshGroupCombineInProgress(_spawnedCombineableMeshGroup);
							_meshCombiningRoutine = CombineMeshesAsync(componentsInChildren);
							SingletonMonoBehaviour<WorkLoadQueue>.Instance.EnqueueWork(_meshCombiningRoutine);
						}
					}
				}
			}
			_spawnedOnceOnStart = spawningOnceOnStart;
		}

		private void CombineMeshes(IList<CombineableMesh> combineableMeshes)
		{
			IList<ObjectModelInfo> meshInfos = MeshUtil.GetMeshInfos(combineableMeshes);
			ModelInfo combinedMeshes = MeshUtil.CombineMeshes(base.gameObject, meshInfos);
			ReceiveCombinedMeshAndDespawn(combinedMeshes);
		}

		private IEnumerator CombineMeshesAsync(IList<CombineableMesh> combineableMeshes)
		{
			_isCombiningMeshes = true;
			yield return MeshUtil.CombineMeshesAsync(meshInfos: MeshUtil.GetMeshInfos(combineableMeshes), parentGO: base.gameObject, meshReceiveCallback: ReceiveCombinedMeshAndDespawn);
		}

		private void UpdateToCombinedMesh(ModelInfo combinedMeshes)
		{
			if (combinedMeshes != null)
			{
				combinedMeshes.Mesh.name = base.name + "_combined";
				_meshFilter.mesh = combinedMeshes.Mesh;
				_meshRenderer.materials = combinedMeshes.Materials;
				_meshRenderer.enabled = true;
				_usingCombinedMesh = true;
			}
			else
			{
				SDebug.LogError("Did not receive a combined mesh. No child meshes to combine", this);
			}
		}

		protected override void GetSpawnedObjects(List<Selector> selectors)
		{
		}

		private void ReceiveCombinedMeshAndDespawn(ModelInfo combinedMeshes)
		{
			UpdateToCombinedMesh(combinedMeshes);
			if (combinedMeshes != null)
			{
				_meshContainer.AddGroup(_spawnedCombineableMeshGroup, combinedMeshes);
				TrackObject[] componentsInChildrenBFS = this.GetComponentsInChildrenBFS<TrackObject>(true, false);
				for (int num = componentsInChildrenBFS.Length - 1; num >= 0; num--)
				{
					if (componentsInChildrenBFS[num].Active)
					{
						componentsInChildrenBFS[num].Deactivate();
					}
					else
					{
						Placeholder component = componentsInChildrenBFS[num].GetComponent<Placeholder>();
						component.DespawnInstances();
					}
				}
				base.DespawnInstances();
			}
			_isCombiningMeshes = false;
		}

		protected override void DespawnInstances()
		{
			if (_isCombiningMeshes)
			{
				_meshContainer.NotifyMeshGroupCombineCanceled(_spawnedCombineableMeshGroup);
				_isCombiningMeshes = false;
				_spawnedCombineableMeshGroup = null;
				SingletonMonoBehaviour<WorkLoadQueue>.Instance.CancelWork(_meshCombiningRoutine);
			}
			if (_usingCombinedMesh && !_spawnedOnceOnStart)
			{
				_meshFilter.mesh = null;
				_meshRenderer.enabled = false;
				_usingCombinedMesh = false;
			}
			else
			{
				_placeholder.DespawnInstances();
			}
		}

		private void OnValidate()
		{
			PlaceholderPrefabProviderBase component = GetComponent<PlaceholderPrefabProviderBase>();
			if (component == null)
			{
				SDebug.LogError("ModelSelectionPlaceholderLifecycle: Missing a PlaceholderPrefabProvider at " + Utils.GetLongName(base.transform), this);
				return;
			}
			for (int i = 0; i < component.PrefabCount; i++)
			{
				Transform prefab = component.GetPrefab(i);
				if (prefab == null)
				{
					SDebug.LogError("ModelSelectionPlaceholderLifecycle: NULL prefabs at index " + i + " in PlaceholderPrefabProvider at " + Utils.GetLongName(prefab.transform), component);
				}
				else
				{
					CombineableMeshGroup component2 = prefab.GetComponent<CombineableMeshGroup>();
					if (component2 == null)
					{
						SDebug.LogError("ModelSelectionPlaceholderLifecycle: Model prefab missing CombineableMeshGroup component " + Utils.GetLongName(prefab.transform), component);
					}
				}
			}
		}
	}
}
