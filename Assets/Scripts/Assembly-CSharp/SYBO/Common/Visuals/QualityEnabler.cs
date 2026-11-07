using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Common.Visuals
{
	public class QualityEnabler : MonoBehaviour
	{
		[SerializeField]
		private QualityFeature _feature;

		public QualityFeature Feature
		{
			get
			{
				return _feature;
			}
		}

		private void OnEnable()
		{
			ApplyQuality();
		}

		public void ApplyQuality()
		{
			bool flag = SingletonMonoBehaviour<QualityManager>.Instance.IsSupported(_feature);
			if (flag != base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(flag);
			}
			if (flag && SingletonMonoBehaviour<QualityManager>.Instance.PrewarmParticleSystems(_feature))
			{
				ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>(true);
				foreach (ParticleSystem particleSystem in componentsInChildren)
				{
					particleSystem.Simulate(particleSystem.main.duration);
				}
			}
		}
	}
}
