using System;
using UnityEngine;

namespace SYBO.Subway.Routes
{
	[Serializable]
	public class WeightedPrefab<T> where T : Component
	{
		[HideInInspector]
		[SerializeField]
		private string _name;

		public int Weight = 1;

		public T Prefab;

		public WeightedPrefab()
		{
			Weight = 1;
		}

		public WeightedPrefab(T prefab, int weight)
		{
			_name = prefab.name;
			Prefab = prefab;
			Weight = weight;
		}
	}
}
