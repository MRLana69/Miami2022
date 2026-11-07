using System;
using SYBO.Common.Components;
using SYBO.Common.Data;
using SYBO.Common.Exceptions;
using SYBO.Common.Utilities;
using UnityEngine;

namespace SYBO.Common.Builders
{
	[Serializable]
	public class StackablePrefabBuilder
	{
		private static string _STACKABLE_PREFAB_BUILDER_NAME = "StackablePrefabBuilder";

		[SerializeField]
		private StackablePrefabData _stackablePrefabData;

		public StackablePrefabData StackablePrefabData
		{
			get
			{
				return _stackablePrefabData;
			}
		}

		public StackablePrefabBuilder(StackablePrefabData stackablePrefabData)
		{
			_stackablePrefabData = stackablePrefabData;
		}

		public GameObject BuildAndAttach(GameObject parentGameObject, Vector3 startWorldPosition, Vector3 endWorldPosition)
		{
			GameObject gameObject = new GameObject(_STACKABLE_PREFAB_BUILDER_NAME);
			DeselectMeComponent deselectMeComponent = gameObject.AddComponent<DeselectMeComponent>();
			deselectMeComponent.DeselectMeMode = DeselectMeMode.EnabledRedirectToParent;
			gameObject.transform.position = parentGameObject.transform.position;
			HierarchyUtility.Reparent(gameObject.transform, parentGameObject.transform, true);
			if (_stackablePrefabData.Prefabs.Count != 3)
			{
				SDebug.LogError("Stackabel system CURRENTLY requires 3 Prefabs. Has : " + _stackablePrefabData.Prefabs.Count);
			}
			GameObject gameObject2 = _stackablePrefabData.Prefabs[0];
			GameObject midPrefab = _stackablePrefabData.Prefabs[1];
			GameObject gameObject3 = _stackablePrefabData.Prefabs[2];
			if (gameObject2 != null && gameObject3 != null)
			{
				switch (_stackablePrefabData.StackableBuildDirection)
				{
				case StackableBuildDirection.ZOnly:
					BuildZDirection(gameObject, startWorldPosition, endWorldPosition, gameObject2, midPrefab, gameObject3);
					break;
				case StackableBuildDirection.YDownToFloor:
					BuildYDirection(gameObject, startWorldPosition, endWorldPosition, gameObject2, midPrefab, gameObject3);
					break;
				default:
					SwitchDefaultException.Throw(_stackablePrefabData.StackableBuildDirection.ToString());
					break;
				}
			}
			return gameObject;
		}

		private void BuildZDirection(GameObject stackablePrefabBuilder, Vector3 startWorldPosition, Vector3 endWorldPosition, GameObject startPrefab, GameObject midPrefab, GameObject endPrefab)
		{
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			float num = endWorldPosition.z - startWorldPosition.z;
			gameObject = UnityEngine.Object.Instantiate(startPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
			gameObject.transform.position = startWorldPosition - new Vector3(0f, 0f, 0f);
			HierarchyUtility.Reparent(gameObject.transform, stackablePrefabBuilder.transform, true);
			float num2 = gameObject.GetComponentInChildren<Renderer>().bounds.size.z * 2f;
			if (num > num2)
			{
				gameObject2 = UnityEngine.Object.Instantiate(endPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
				gameObject2.transform.position = endWorldPosition + new Vector3(0f, 0f, 0f - gameObject2.GetComponentInChildren<Renderer>().bounds.size.z);
				HierarchyUtility.Reparent(gameObject2.transform, stackablePrefabBuilder.transform, true);
			}
			if (midPrefab != null && gameObject2 != null)
			{
				float num3 = gameObject2.transform.position.z - gameObject.transform.position.z;
				float num4 = num3;
				float z = midPrefab.GetComponent<Renderer>().bounds.size.z;
				int num5 = 0;
				while (num4 > z)
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate(midPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
					num5++;
					gameObject3.transform.position = gameObject.transform.position + num5 * new Vector3(0f, 0f, gameObject3.GetComponent<Renderer>().bounds.size.z);
					HierarchyUtility.Reparent(gameObject3.transform, stackablePrefabBuilder.transform, true);
					num4 -= z;
				}
			}
		}

		private void BuildYDirection(GameObject stackablePrefabBuilder, Vector3 startWorldPosition, Vector3 endWorldPosition, GameObject startPrefab, GameObject midPrefab, GameObject endPrefab)
		{
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			gameObject = UnityEngine.Object.Instantiate(startPrefab, startWorldPosition, Quaternion.identity);
			HierarchyUtility.Reparent(gameObject.transform, stackablePrefabBuilder.transform, true);
			if (endPrefab != null)
			{
				gameObject2 = UnityEngine.Object.Instantiate(endPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
				gameObject2.transform.position = endWorldPosition;
				HierarchyUtility.Reparent(gameObject2.transform, stackablePrefabBuilder.transform, true);
			}
			if (midPrefab != null && gameObject2 != null)
			{
				float num = Mathf.Abs(startWorldPosition.y - endWorldPosition.y);
				float num2 = num;
				float y = midPrefab.GetComponent<Renderer>().bounds.size.y;
				GameObject gameObject3 = null;
				int num3 = 0;
				while (num2 > 0f)
				{
					gameObject3 = UnityEngine.Object.Instantiate(midPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
					num3++;
					gameObject3.transform.position = gameObject.transform.position + num3 * new Vector3(0f, 0f - gameObject3.GetComponent<Renderer>().bounds.size.y, 0f);
					HierarchyUtility.Reparent(gameObject3.transform, stackablePrefabBuilder.transform, true);
					num2 -= y;
				}
			}
		}
	}
}
