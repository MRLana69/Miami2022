using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SYBO.Common.Utilities
{
	[Serializable]
	public class WeightedObject<T> : IWeighted where T : class
	{
		[FormerlySerializedAs("Weight")]
		[SerializeField]
		private int _weight = 1;

		[FormerlySerializedAs("Prefab")]
		[SerializeField]
		private T _object;

		public int Weight
		{
			get
			{
				return _weight;
			}
		}

		public T Object
		{
			get
			{
				return _object;
			}
		}

		public WeightedObject()
		{
			_weight = 1;
		}

		public WeightedObject(T weightedObject, int weight)
		{
			_object = weightedObject;
			_weight = weight;
		}
	}
}
