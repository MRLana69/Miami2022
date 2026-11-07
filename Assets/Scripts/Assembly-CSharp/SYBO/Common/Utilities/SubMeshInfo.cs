using UnityEngine;

namespace SYBO.Common.Utilities
{
	public class SubMeshInfo
	{
		public MeshFilter MeshFilter;

		public int SubmeshIndex;

		public Material Material;

		public SubMeshInfo(MeshFilter meshFilter, int submeshIndex, Material material)
		{
			MeshFilter = meshFilter;
			SubmeshIndex = submeshIndex;
			Material = material;
		}
	}
}
