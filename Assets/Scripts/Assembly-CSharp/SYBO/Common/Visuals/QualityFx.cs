using System;
using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Common.Visuals
{
	[Serializable]
	public class QualityFx
	{
		[SerializeField]
		private QualityFeature _feature;

		[SerializeField]
		private MonoBehaviour _unsupportedPrefab;

		[SerializeField]
		private MonoBehaviour _supportedPrefab;

		public T InstantiateEffect<T>() where T : MonoBehaviour
		{
			return InstantiateEffect<T>(Vector3.zero, Quaternion.identity);
		}

		public T InstantiateEffect<T>(Vector3 position, Quaternion rotation) where T : MonoBehaviour
		{
			bool flag = SingletonMonoBehaviour<QualityManager>.Instance.IsSupported(_feature);
			return (T)InstantiateInternal((!flag) ? _unsupportedPrefab : _supportedPrefab, position, rotation);
		}

		private T InstantiateInternal<T>(T prefab, Vector3 position, Quaternion rotation) where T : MonoBehaviour
		{
			if (prefab == null)
			{
				SDebug.LogError("Could not instantiate effect: " + prefab.name);
			}
			return UnityEngine.Object.Instantiate(prefab, position, rotation);
		}
	}
}
