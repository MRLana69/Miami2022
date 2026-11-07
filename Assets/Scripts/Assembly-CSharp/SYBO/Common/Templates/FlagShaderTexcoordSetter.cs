using SYBO.Common.Utilities;
using UnityEngine;

namespace SYBO.Common.Templates
{
	[RequireComponent(typeof(MeshFilter))]
	public class FlagShaderTexcoordSetter : MonoBehaviour
	{
		private void Start()
		{
			MeshFilter component = GetComponent<MeshFilter>();
			if (component == null)
			{
				SDebug.LogError(string.Format("FlagShaderTexcoordSetter missing MeshFilter on {0}", base.transform.GetLongName()), this);
				return;
			}
			Mesh sharedMesh = component.sharedMesh;
			if (sharedMesh == null)
			{
				SDebug.LogError(string.Format("FlagShaderTexcoordSetter missing a mesh on {0}", base.transform.GetLongName()), this);
			}
			else if (sharedMesh != null)
			{
				Vector3[] vertices = sharedMesh.vertices;
				Vector2[] array = new Vector2[vertices.Length];
				for (int i = 0; i < vertices.Length; i++)
				{
					array[i] = new Vector2(vertices[i].y, vertices[i].z);
				}
				sharedMesh.uv2 = array;
			}
		}
	}
}
