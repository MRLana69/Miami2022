using SYBO.Common.Utilities;
using UnityEngine;

namespace SYBO.Common.Visuals
{
	public class OnStartMeshCombineLauncher : MonoBehaviour
	{
		private MeshCombiner _meshCombiner;

		private void Start()
		{
			_meshCombiner = this.AddComponentIfMissing(typeof(MeshCombiner)) as MeshCombiner;
			_meshCombiner.CombineInactive();
		}
	}
}
