using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Subway.Visuals
{
	public class HuntTokenCollectVFXColorCycle : HuntTokenCollectVFX
	{
		[Serializable]
		public class ColorEntry
		{
			[SerializeField]
			private Color _minColor = Color.white;

			[SerializeField]
			private Color _maxColor = Color.white;

			public ParticleSystem.MinMaxGradient Gradient
			{
				get
				{
					return new ParticleSystem.MinMaxGradient(_minColor, _maxColor);
				}
			}
		}

		private System.Random _randomGen;

		private ParticleSystem.MainModule _mainParticleModule;

		[SerializeField]
		private List<ColorEntry> _colorEntries = new List<ColorEntry>();

		private int _randomSeed = -1;

		public override void Setup()
		{
			_randomGen = ((_randomSeed == -1) ? new System.Random() : new System.Random(_randomSeed));
			_mainParticleModule = _particleSystem.main;
		}

		public override IEnumerator PlayEffect()
		{
			_mainParticleModule.startColor = PickRandomColorGradient();
			_particleSystem.Play();
			yield break;
		}

		private ParticleSystem.MinMaxGradient PickRandomColorGradient()
		{
			int index = _randomGen.Next(0, _colorEntries.Count - 1);
			return _colorEntries[index].Gradient;
		}
	}
}
