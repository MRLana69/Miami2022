using System;
using UnityEngine;

namespace SYBO.Common.Utilities
{
	[Serializable]
	public class ModelInfo
	{
		public Mesh Mesh;

		public Material[] Materials;

		public ModelInfo(Mesh mesh, Material[] materials)
		{
			Mesh = mesh;
			Materials = materials;
		}
	}
}
