using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Subway.Level
{
	public class Randomizer : Selector
	{
		public override void PerformSelection(List<GameObject> objectsToVisit)
		{
			base.PerformSelection(objectsToVisit);
			int num = Random.Range(0, _transform.childCount);
			for (int i = 0; i < _transform.childCount; i++)
			{
				GameObject gameObject = _transform.GetChild(i).gameObject;
				if (i == num)
				{
					objectsToVisit.Add(gameObject);
				}
				else if (gameObject.activeSelf)
				{
					gameObject.SetActive(false);
				}
			}
		}
	}
}
