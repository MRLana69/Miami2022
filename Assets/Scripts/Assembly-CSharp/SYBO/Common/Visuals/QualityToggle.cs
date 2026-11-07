using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Common.Visuals
{
	public class QualityToggle : MonoBehaviour
	{
		[SerializeField]
		private QualityFeature _feature;

		[SerializeField]
		private GameObject _unsupported;

		[SerializeField]
		private GameObject _supported;

		private void Awake()
		{
			bool flag = SingletonMonoBehaviour<QualityManager>.Instance.IsSupported(_feature);
			_unsupported.SetActive(!flag);
			_supported.SetActive(flag);
		}
	}
}
