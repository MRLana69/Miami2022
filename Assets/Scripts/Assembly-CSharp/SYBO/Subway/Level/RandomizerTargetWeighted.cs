using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Subway.Level
{
	public class RandomizerTargetWeighted : Selector
	{
		[SerializeField]
		private List<TargetWeightPair> _targetWeights = new List<TargetWeightPair>();

		private float _weightSum;

		protected override void Initialize()
		{
			base.Initialize();
			for (int i = 0; i < _targetWeights.Count; i++)
			{
				_weightSum += _targetWeights[i].Weight;
			}
		}

		public override void PerformSelection(List<GameObject> objectsToVisit)
		{
			base.PerformSelection(objectsToVisit);
			if (_targetWeights.Count == 0)
			{
				return;
			}
			if (Application.isEditor)
			{
				_weightSum = 0f;
				for (int i = 0; i < _targetWeights.Count; i++)
				{
					_weightSum += _targetWeights[i].Weight;
				}
			}
			float num = Random.Range(0f, _weightSum);
			float num2 = 0f;
			bool flag = false;
			for (int j = 0; j < _targetWeights.Count; j++)
			{
				TargetWeightPair targetWeightPair = _targetWeights[j];
				if (!flag)
				{
					num2 += targetWeightPair.Weight;
					if (num < num2)
					{
						objectsToVisit.Add(targetWeightPair.Target);
						flag = true;
						continue;
					}
				}
				targetWeightPair.Target.SetActive(false);
			}
		}
	}
}
