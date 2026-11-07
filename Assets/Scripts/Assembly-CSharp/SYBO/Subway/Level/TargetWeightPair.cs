using System;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[Serializable]
	public class TargetWeightPair
	{
		[SerializeField]
		private GameObject _target;

		[SerializeField]
		private float _weight;

		public GameObject Target
		{
			get
			{
				return _target;
			}
		}

		public float Weight
		{
			get
			{
				return _weight;
			}
		}
	}
}
