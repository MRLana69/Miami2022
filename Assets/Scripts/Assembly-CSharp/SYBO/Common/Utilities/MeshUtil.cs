using System;
using System.Collections;
using System.Collections.Generic;
using SYBO.Common.Visuals;
using UnityEngine;

namespace SYBO.Common.Utilities
{
	public static class MeshUtil
	{
		public static IList<ObjectModelInfo> GetMeshInfos(IList<CombineableMesh> combineableMeshes)
		{
			List<ObjectModelInfo> list = new List<ObjectModelInfo>(combineableMeshes.Count);
			for (int i = 0; i < combineableMeshes.Count; i++)
			{
				CombineableMesh combineableMesh = combineableMeshes[i];
				list.Add(combineableMesh.MeshInfo);
			}
			return list;
		}

		public static IList<ObjectModelInfo> GetMeshInfos(IList<MeshRenderer> meshRenderers)
		{
			List<ObjectModelInfo> list = new List<ObjectModelInfo>(meshRenderers.Count);
			for (int i = 0; i < meshRenderers.Count; i++)
			{
				MeshRenderer meshRenderer = meshRenderers[i];
				MeshFilter meshFilter = meshRenderer.GetComponent(typeof(MeshFilter)) as MeshFilter;
				list.Add(new ObjectModelInfo(meshRenderer, meshFilter));
			}
			return list;
		}

		public static ModelInfo CombineMeshes(GameObject parentGO, IList<MeshRenderer> meshRenderers)
		{
			IList<ObjectModelInfo> meshInfos = GetMeshInfos(meshRenderers);
			return CombineMeshes(parentGO, meshInfos);
		}

		public static ModelInfo CombineMeshes(GameObject parentGO, IList<CombineableMesh> combineableMeshes)
		{
			IList<ObjectModelInfo> meshInfos = GetMeshInfos(combineableMeshes);
			return CombineMeshes(parentGO, meshInfos);
		}

		public static ModelInfo CombineMeshes(GameObject parentGO, IList<ObjectModelInfo> meshInfos)
		{
			ModelInfo cobinedMesh = null;
			IEnumerator enumerator = CombineMeshesAsync(parentGO, meshInfos, (ModelInfo cm) =>
			{
				cobinedMesh = cm;
			});
			while (enumerator.MoveNext())
			{
			}
			return cobinedMesh;
		}

		public static ModelInfo CombineMeshes(GameObject parentGO, bool combineInactive)
		{
			CombineableMesh[] componentsInChildren = parentGO.GetComponentsInChildren<CombineableMesh>(combineInactive);
			return CombineMeshes(parentGO, componentsInChildren);
		}

		public static IEnumerator CombineMeshesAsync(GameObject parentGO, IList<ObjectModelInfo> meshInfos, Action<ModelInfo> meshReceiveCallback)
		{
			if (meshReceiveCallback == null)
			{
				SDebug.LogError("No callback specified to receive combined mesh", parentGO);
			}
			else
			{
				if (meshInfos.Count == 0)
				{
					yield break;
				}
				Dictionary<MeshFilter, Matrix4x4> localPosMapping = new Dictionary<MeshFilter, Matrix4x4>();
				int totalVertexCount = 0;
				int totalMeshCount = 0;
				for (int i = 0; i < meshInfos.Count; i++)
				{
					MeshFilter meshFilter = meshInfos[i].MeshFilter;
					if (meshFilter != null && meshFilter.sharedMesh != null)
					{
						localPosMapping.Add(meshFilter, meshFilter.transform.localToWorldMatrix);
						totalVertexCount += meshFilter.sharedMesh.vertexCount;
						totalMeshCount++;
					}
				}
				if (totalMeshCount == 0 || totalVertexCount > 65534)
				{
					yield break;
				}
				Matrix4x4 goWorldToLocalMatrix = parentGO.transform.worldToLocalMatrix;
				List<Vector3> cVertices = new List<Vector3>(totalVertexCount);
				List<Vector3> cNormals = new List<Vector3>(totalVertexCount);
				List<Color32> cColors = new List<Color32>(totalVertexCount);
				List<Vector2> cUV1s = new List<Vector2>(totalVertexCount);
				List<Vector2> cUV2s = new List<Vector2>(totalVertexCount);
				Dictionary<Material, List<int>> cSubMeshes = new Dictionary<Material, List<int>>();
				for (int j = 0; j < meshInfos.Count; j++)
				{
					MeshRenderer meshRenderer = meshInfos[j].MeshRenderer;
					MeshFilter meshFilter2 = meshInfos[j].MeshFilter;
					if (meshFilter2 != null && meshFilter2.sharedMesh != null)
					{
						MergeMeshInto(meshFilter2.sharedMesh, meshRenderer.sharedMaterials, goWorldToLocalMatrix * localPosMapping[meshFilter2], cVertices, cNormals, cColors, cUV1s, cUV2s, cSubMeshes);
						yield return null;
					}
				}
				ModelInfo combinedMesh = new ModelInfo(new Mesh(), new Material[cSubMeshes.Count])
				{
					Mesh = 
					{
						vertices = cVertices.ToArray()
					}
				};
				if (0 < cNormals.Count)
				{
					combinedMesh.Mesh.normals = cNormals.ToArray();
				}
				if (0 < cUV1s.Count)
				{
					combinedMesh.Mesh.uv = cUV1s.ToArray();
				}
				if (0 < cUV2s.Count)
				{
					combinedMesh.Mesh.uv2 = cUV2s.ToArray();
				}
				combinedMesh.Mesh.subMeshCount = cSubMeshes.Count;
				combinedMesh.Mesh.colors32 = cColors.ToArray();
				int mi = 0;
				foreach (Material key in cSubMeshes.Keys)
				{
					combinedMesh.Materials[mi] = key;
					combinedMesh.Mesh.SetTriangles(cSubMeshes[key].ToArray(), mi++);
				}
				meshReceiveCallback(combinedMesh);
			}
		}

		public static ModelInfo CombineMeshes(this GameObject go)
		{
			MeshRenderer[] array = go.GetComponentsInChildren(typeof(MeshRenderer), true) as MeshRenderer[];
			ModelInfo result = CombineMeshes(go, array);
			foreach (MeshRenderer meshRenderer in array)
			{
				if (meshRenderer.gameObject != go)
				{
					meshRenderer.enabled = false;
				}
			}
			return result;
		}

		private static void MergeMeshInto(Mesh mesh, Material[] materials, Matrix4x4 transformMatrix, List<Vector3> cVertices, List<Vector3> cNormals, List<Color32> cColors, List<Vector2> cUV1s, List<Vector2> cUV2s, Dictionary<Material, List<int>> cSubMeshes)
		{
			if (mesh == null)
			{
				return;
			}
			int count = cVertices.Count;
			Vector3[] vertices = mesh.vertices;
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = transformMatrix.MultiplyPoint3x4(vertices[i]);
			}
			cVertices.AddRange(vertices);
			Color32[] colors = mesh.colors32;
			if (colors != null && 0 < colors.Length)
			{
				for (int j = 0; j < colors.Length; j++)
				{
					cColors.Add(colors[j]);
				}
			}
			Quaternion quaternion = Quaternion.LookRotation(transformMatrix.GetColumn(2), transformMatrix.GetColumn(1));
			Vector3[] normals = mesh.normals;
			if (normals != null && 0 < normals.Length)
			{
				for (int k = 0; k < normals.Length; k++)
				{
					normals[k] = quaternion * normals[k];
				}
				cNormals.AddRange(normals);
			}
			Vector2[] uv = mesh.uv;
			if (uv != null && 0 < uv.Length)
			{
				cUV1s.AddRange(uv);
			}
			uv = mesh.uv2;
			if (uv != null && 0 < uv.Length)
			{
				cUV2s.AddRange(uv);
			}
			for (int l = 0; l < materials.Length; l++)
			{
				Material material = materials[l];
				if (material == null || l >= mesh.subMeshCount)
				{
					continue;
				}
				int[] triangles = mesh.GetTriangles(l);
				if (0 < triangles.Length)
				{
					List<int> value = null;
					cSubMeshes.TryGetValue(material, out value);
					if (value == null)
					{
						value = (cSubMeshes[material] = new List<int>());
					}
					for (int m = 0; m < triangles.Length; m++)
					{
						triangles[m] += count;
					}
					value.AddRange(triangles);
				}
			}
		}

		public static IEnumerator CombineMeshesAsync(GameObject parentGO, IList<ObjectModelInfo> meshInfos, Action<ModelInfo[]> meshReceiveCallback = null)
		{
			if (meshInfos.Count == 0)
			{
				yield break;
			}
			int totalVertexCount = 0;
			int totalMeshCount = 0;
			Dictionary<Material, List<SubMeshInfo>> materialMeshes = new Dictionary<Material, List<SubMeshInfo>>();
			for (int i = 0; i < meshInfos.Count; i++)
			{
				ObjectModelInfo objectModelInfo = meshInfos[i];
				MeshFilter meshFilter = objectModelInfo.MeshFilter;
				if (!(meshFilter != null) || !(meshFilter.sharedMesh != null))
				{
					continue;
				}
				totalVertexCount += meshFilter.sharedMesh.vertexCount;
				totalMeshCount++;
				Material[] sharedMaterials = objectModelInfo.MeshRenderer.sharedMaterials;
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					Material material = sharedMaterials[j];
					SubMeshInfo item = new SubMeshInfo(meshFilter, j, material);
					List<SubMeshInfo> value = null;
					if (!materialMeshes.TryGetValue(material, out value))
					{
						value = (materialMeshes[material] = new List<SubMeshInfo>());
					}
					value.Add(item);
				}
			}
			if (totalMeshCount == 0 || totalVertexCount > 65534)
			{
				yield break;
			}
			Matrix4x4 goWorldToLocalMatrix = parentGO.transform.worldToLocalMatrix;
			List<Vector3> cVertices = new List<Vector3>(totalVertexCount);
			List<Vector3> cNormals = new List<Vector3>(totalVertexCount);
			List<Vector2> cUV1s = new List<Vector2>(totalVertexCount);
			List<Vector2> cUV2s = new List<Vector2>(totalVertexCount);
			List<int> cTriangles = new List<int>(totalVertexCount);
			Dictionary<int, int> oldToNewVertsIDs = new Dictionary<int, int>(totalVertexCount);
			int cmi = 0;
			ModelInfo[] combinedMeshes = new ModelInfo[materialMeshes.Count];
			foreach (KeyValuePair<Material, List<SubMeshInfo>> kvp in materialMeshes)
			{
				cVertices.Clear();
				cNormals.Clear();
				cUV1s.Clear();
				cUV2s.Clear();
				cTriangles.Clear();
				List<SubMeshInfo> subMeshInfos = kvp.Value;
				for (int k = 0; k < subMeshInfos.Count; k++)
				{
					oldToNewVertsIDs.Clear();
					SubMeshInfo subMeshInfo = subMeshInfos[k];
					MeshFilter meshFilter2 = subMeshInfo.MeshFilter;
					if (meshFilter2.sharedMesh.subMeshCount > subMeshInfo.SubmeshIndex)
					{
						MergeSubmeshInto(transformMatrix: goWorldToLocalMatrix * meshFilter2.transform.localToWorldMatrix, mesh: meshFilter2.sharedMesh, submeshIndex: subMeshInfo.SubmeshIndex, cVertices: cVertices, cNormals: cNormals, cUV1s: cUV1s, cUV2s: cUV2s, cTriangles: cTriangles, oldToNewVertIDs: oldToNewVertsIDs);
						if (meshReceiveCallback != null)
						{
							yield return null;
						}
					}
				}
				ModelInfo combinedMeshInfo = new ModelInfo(new Mesh(), new Material[1] { kvp.Key })
				{
					Mesh = 
					{
						vertices = cVertices.ToArray()
					}
				};
				if (0 < cNormals.Count)
				{
					combinedMeshInfo.Mesh.normals = cNormals.ToArray();
				}
				if (0 < cUV1s.Count)
				{
					combinedMeshInfo.Mesh.uv = cUV1s.ToArray();
				}
				if (0 < cUV2s.Count)
				{
					combinedMeshInfo.Mesh.uv2 = cUV2s.ToArray();
				}
				combinedMeshInfo.Mesh.subMeshCount = 1;
				combinedMeshInfo.Mesh.SetTriangles(cTriangles.ToArray(), 0);
				combinedMeshes[cmi++] = combinedMeshInfo;
			}
			if (meshReceiveCallback != null)
			{
				meshReceiveCallback(combinedMeshes);
			}
		}

		private static void MergeSubmeshInto(Mesh mesh, int submeshIndex, Matrix4x4 transformMatrix, List<Vector3> cVertices, List<Vector3> cNormals, List<Vector2> cUV1s, List<Vector2> cUV2s, List<int> cTriangles, Dictionary<int, int> oldToNewVertIDs)
		{
			if (mesh == null)
			{
				return;
			}
			int[] triangles = mesh.GetTriangles(submeshIndex);
			Vector3[] vertices = mesh.vertices;
			Vector3[] normals = mesh.normals;
			bool flag = normals != null && 0 < normals.Length;
			Quaternion quaternion = Quaternion.LookRotation(transformMatrix.GetColumn(2), transformMatrix.GetColumn(1));
			Vector2[] uv = mesh.uv;
			bool flag2 = uv != null && 0 < uv.Length;
			Vector2[] uv2 = mesh.uv2;
			bool flag3 = uv2 != null && 0 < uv2.Length;
			for (int i = 0; i < triangles.Length; i++)
			{
				int num = triangles[i];
				int value;
				if (!oldToNewVertIDs.TryGetValue(num, out value))
				{
					Vector3 point = vertices[num];
					Vector3 item = transformMatrix.MultiplyPoint3x4(point);
					cVertices.Add(item);
					value = (oldToNewVertIDs[num] = cVertices.Count - 1);
					if (flag)
					{
						Vector3 vector = normals[num];
						Vector3 item2 = quaternion * vector;
						cNormals.Add(item2);
					}
					if (flag2)
					{
						cUV1s.Add(uv[num]);
					}
					if (flag3)
					{
						cUV2s.Add(uv2[num]);
					}
				}
				triangles[i] = value;
			}
			cTriangles.AddRange(triangles);
		}
	}
}
