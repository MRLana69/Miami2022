using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Subway.Level
{
	public class RandomizerTarget : Selector
	{
		[SerializeField]
		private List<GameObject> Targets;

		public override void PerformSelection(List<GameObject> objectsToVisit)
		{
			base.PerformSelection(objectsToVisit);
			int num = Random.Range(0, Targets.Count);
			for (int i = 0; i < Targets.Count; i++)
			{
				GameObject gameObject = Targets[i];
				if (i == num)
				{
					objectsToVisit.Add(gameObject);
				}
				else
				{
					gameObject.SetActive(false);
				}
			}
		}
	}
}
