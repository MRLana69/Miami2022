using System.Collections;
using UnityEngine;

namespace SYBO.Subway.Visuals
{
	[RequireComponent(typeof(ParticleSystem))]
	public abstract class HuntTokenCollectVFX : MonoBehaviour
	{
		protected ParticleSystem _particleSystem;

		private void Awake()
		{
			_particleSystem = GetComponent<ParticleSystem>();
			Setup();
		}

		private void Update()
		{
		}

		public virtual void Setup()
		{
		}

		public abstract IEnumerator PlayEffect();
	}
}
