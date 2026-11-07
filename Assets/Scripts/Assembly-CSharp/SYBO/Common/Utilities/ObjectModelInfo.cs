using UnityEngine;

namespace SYBO.Common.Utilities
{
	public class ObjectModelInfo
	{
		public MeshRenderer MeshRenderer;

		public MeshFilter MeshFilter;

		public ObjectModelInfo(MeshRenderer meshRenderer, MeshFilter meshFilter)
		{
			MeshRenderer = meshRenderer;
			MeshFilter = meshFilter;
		}
	}
}
