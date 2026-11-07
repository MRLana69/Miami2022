using System;
using System.Collections.Generic;
using SYBO.Common.Utilities;
using UnityEngine;

namespace SYBO.Common.Visuals
{
	public class MeshCombiner : MonoBehaviour
	{
		private const int MAX_MESH_VERTEX_COUNT = 65534;

		private const int MAX_BATCH_VERTEX_COUNT = 65534;

		private bool _initialized;

		private CombineableMesh[] combineableMeshes;

		private Transform _combinedMeshRootTransform;

		private int _combinedMeshesCount;

		private void Awake()
		{
			InitializeCheck();
		}

		private void InitializeCheck()
		{
			if (!_initialized)
			{
				GameObject gameObject = new GameObject("combinedMeshes");
				_combinedMeshRootTransform = gameObject.transform;
				_combinedMeshRootTransform.parent = base.transform;
				_combinedMeshRootTransform.ResetLocalPosition();
				_initialized = true;
			}
		}

		public void CombineInactive()
		{
			InitializeCheck();
			Combine(true);
		}

		public void Combine()
		{
			InitializeCheck();
			Combine(false);
		}

		public void DestroyCombined()
		{
			if (!_initialized)
			{
				return;
			}
			if (combineableMeshes != null)
			{
				for (int i = 0; i < combineableMeshes.Length; i++)
				{
					combineableMeshes[i].MeshRenderer.enabled = true;
				}
				combineableMeshes = null;
			}
			for (int j = 0; j < _combinedMeshRootTransform.childCount; j++)
			{
				Transform child = _combinedMeshRootTransform.GetChild(j);
				MeshFilter meshFilter = child.GetComponent(typeof(MeshFilter)) as MeshFilter;
				if (meshFilter != null)
				{
					UnityEngine.Object.Destroy(meshFilter.mesh);
					meshFilter.mesh = null;
				}
			}
		}

		private void Combine(bool combineInactive)
		{
			InitializeCheck();
			combineableMeshes = GetComponentsInChildren<CombineableMesh>(combineInactive);
			int num = 0;
			for (int i = 0; i < combineableMeshes.Length; i++)
			{
				num += combineableMeshes[i].MeshFilter.sharedMesh.vertexCount;
			}
			if (65534 < num)
			{
				SortCombineableMeshes();
			}
			int num2 = 1 + num / 65534;
			int num3 = num / num2;
			int num4 = 0;
			List<ObjectModelInfo> list = new List<ObjectModelInfo>(Mathf.Min(num, 65534));
			int num5 = combineableMeshes.Length - 1;
			for (int j = 0; j < combineableMeshes.Length; j++)
			{
				CombineableMesh combineableMesh = combineableMeshes[j];
				list.Add(new ObjectModelInfo(combineableMesh.MeshRenderer, combineableMesh.MeshFilter));
				num4 += combineableMesh.MeshFilter.mesh.vertexCount;
				if (num3 < num4 || j == num5)
				{
					GameObject combinedMeshGO = GetCombinedMeshGO();
					Combine(combinedMeshGO, list);
					_combinedMeshesCount++;
					list.Clear();
					num4 = 0;
				}
			}
		}

		private void SortCombineableMeshes()
		{
			ComponentPositionWrapper<CombineableMesh>[] array = new ComponentPositionWrapper<CombineableMesh>[combineableMeshes.Length];
			for (int i = 0; i < combineableMeshes.Length; i++)
			{
				array[i] = new ComponentPositionWrapper<CombineableMesh>(combineableMeshes[i]);
			}
			Array.Sort(array, (ComponentPositionWrapper<CombineableMesh> x, ComponentPositionWrapper<CombineableMesh> y) => x.Position.z.CompareTo(y.Position.z));
			for (int num = 0; num < array.Length; num++)
			{
				combineableMeshes[num] = array[num].Component;
			}
		}

		private GameObject GetCombinedMeshGO()
		{
			if (_combinedMeshRootTransform.childCount <= _combinedMeshesCount)
			{
				GameObject gameObject = new GameObject("combinedMeshes_" + (_combinedMeshRootTransform.childCount + 1));
				Transform transform = gameObject.transform;
				transform.parent = _combinedMeshRootTransform;
				transform.ResetLocalPosition();
				return transform.gameObject;
			}
			return _combinedMeshRootTransform.GetChild(_combinedMeshesCount - 1).gameObject;
		}

		private void Combine(GameObject combinedMeshAssignGO, IList<ObjectModelInfo> meshInfos)
		{
			ModelInfo modelInfo = MeshUtil.CombineMeshes(base.gameObject, meshInfos);
			if (modelInfo != null)
			{
				ModelInfo modelInfo2 = modelInfo;
				modelInfo2.Mesh.name = base.name + "_combinedMesh";
				for (int i = 0; i < meshInfos.Count; i++)
				{
					meshInfos[i].MeshRenderer.enabled = false;
				}
				MeshRenderer meshRenderer = combinedMeshAssignGO.AddComponentIfMissing(typeof(MeshRenderer)) as MeshRenderer;
				MeshFilter meshFilter = combinedMeshAssignGO.AddComponentIfMissing(typeof(MeshFilter)) as MeshFilter;
				meshRenderer.materials = modelInfo2.Materials;
				meshFilter.mesh = modelInfo2.Mesh;
			}
		}
	}
}
