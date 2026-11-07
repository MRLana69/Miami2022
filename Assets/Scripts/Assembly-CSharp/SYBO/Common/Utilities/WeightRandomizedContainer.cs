using System.Collections.Generic;

namespace SYBO.Common.Utilities
{
	public class WeightRandomizedContainer<T> where T : class
	{
		private class WeightedEntry
		{
			public int AccumulatedWeight;

			public T WeightedObject;

			public WeightedEntry(T weightedObject, int accumulatedWeight)
			{
				WeightedObject = weightedObject;
				AccumulatedWeight = accumulatedWeight;
			}
		}

		private List<WeightedEntry> _entries = new List<WeightedEntry>();

		private int _accumulatedWeight;

		public bool IsEmpty
		{
			get
			{
				return _entries.Count == 0;
			}
		}

		public int Count
		{
			get
			{
				return _entries.Count;
			}
		}

		public void Clear()
		{
			_accumulatedWeight = 0;
			_entries.Clear();
		}

		public void Add(T prefab, int weight)
		{
			if (weight != 0)
			{
				_accumulatedWeight += weight;
				_entries.Add(new WeightedEntry(prefab, _accumulatedWeight));
			}
		}

		public T GetRandom(SyboRandom random)
		{
			int num = random.Range(0, _accumulatedWeight + 1);
			for (int i = 0; i < _entries.Count; i++)
			{
				if (num <= _entries[i].AccumulatedWeight)
				{
					return _entries[i].WeightedObject;
				}
			}
			return (T)null;
		}
	}
}
