using SYBO.Common.Builders;
using SYBO.Common.Data;
using SYBO.Common.Exceptions;
using UnityEngine;

namespace SYBO.Common.Components
{
	public class StackablePrefabBuilderComponent : MonoBehaviour
	{
		[SerializeField]
		private StackablePrefabData _stackablePrefabData;

		protected void Start()
		{
			float y = Mathf.Floor(base.transform.parent.position.y / 90f) * 90f;
			if (_stackablePrefabData.Prefabs.Count > 0)
			{
				Vector3 position = base.transform.position;
				Vector3 endWorldPosition = default(Vector3);
				StackableBuildDirection stackableBuildDirection = _stackablePrefabData.StackableBuildDirection;
				if (stackableBuildDirection == StackableBuildDirection.YDownToFloor)
				{
					endWorldPosition = new Vector3(position.x, y, position.z);
				}
				else
				{
					SwitchDefaultException.Throw(_stackablePrefabData.StackableBuildDirection.ToString());
				}
				StackablePrefabBuilder stackablePrefabBuilder = new StackablePrefabBuilder(_stackablePrefabData);
				stackablePrefabBuilder.BuildAndAttach(base.gameObject, position, endWorldPosition);
			}
		}
	}
}
