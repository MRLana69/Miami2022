using System;
using System.Collections;
using System.Collections.Generic;
using SYBO.Common.Utilities;
using UnityEngine;

namespace SYBO.Subway.Routes
{
	[Serializable]
	public class WeightRandomizedPrefabContainter<TWeighted, T> : IEnumerable<TWeighted>, IEnumerable where TWeighted : WeightedPrefab<T> where T : Component
	{
		[SerializeField]
		private List<TWeighted> _weightedPrefabs = new List<TWeighted>();

		private WeightRandomizedContainer<T> _weightedRandomizedPrefabs = new WeightRandomizedContainer<T>();

		private SemiRandomlyOrderedList<T> _randomlyOrderedPrefabs = new SemiRandomlyOrderedList<T>();

		private Func<T, bool> _pickEvaluator = (T p) => true;

		public bool IsEmpty
		{
			get
			{
				for (int i = 0; i < _weightedPrefabs.Count; i++)
				{
					if (_weightedPrefabs[i] != null)
					{
						return false;
					}
				}
				return true;
			}
		}

		public string Name { get; set; }

		public int Count
		{
			get
			{
				return _weightedPrefabs.Count;
			}
		}

		public TWeighted this[int index]
		{
			get
			{
				return _weightedPrefabs[index];
			}
			set
			{
				_weightedPrefabs[index] = value;
			}
		}

		public int GetTotalWeight()
		{
			int num = 0;
			for (int i = 0; i < _weightedPrefabs.Count; i++)
			{
				num += _weightedPrefabs[i].Weight;
			}
			return num;
		}

		public IEnumerator<TWeighted> GetEnumerator()
		{
			return PrefabIterator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return PrefabIterator();
		}

		private IEnumerator<TWeighted> PrefabIterator()
		{
			for (int i = 0; i < _weightedPrefabs.Count; i++)
			{
				if (_weightedPrefabs[i] != null)
				{
					yield return _weightedPrefabs[i];
				}
			}
		}

		public void Add(TWeighted item)
		{
			_weightedPrefabs.Add(item);
		}

		public bool Contains(TWeighted item)
		{
			return _weightedPrefabs.Contains(item);
		}

		public bool Contains(T item)
		{
			return _weightedPrefabs.Find((TWeighted t) => t.Prefab == item) != null;
		}

		public void Clear()
		{
			_weightedPrefabs.Clear();
			_weightedRandomizedPrefabs.Clear();
			_randomlyOrderedPrefabs.Clear();
		}

		public void RemoveEmptyEntries()
		{
			for (int num = _weightedPrefabs.Count - 1; num >= 0; num--)
			{
				if (_weightedPrefabs[num].Prefab == null)
				{
					_weightedPrefabs.RemoveAt(num);
				}
			}
		}

		public void SetPickEvaluator(Func<T, bool> evaluator)
		{
			_pickEvaluator = evaluator;
		}

		private void BuildRandomlyOrderedPrefabList(SyboRandom random)
		{
			using (IEnumerator<TWeighted> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TWeighted current = enumerator.Current;
					for (int i = 0; i < current.Weight; i++)
					{
						if (!(current.Prefab != null))
						{
							break;
						}
						_randomlyOrderedPrefabs.Add(current.Prefab);
					}
				}
			}
			_randomlyOrderedPrefabs.Reset(random);
		}

		public T PickSemiRandomlyOrderedPrefab(SyboRandom random)
		{
			if (_randomlyOrderedPrefabs.IsEmpty && !IsEmpty)
			{
				BuildRandomlyOrderedPrefabList(random);
			}
			T nextMatching = _randomlyOrderedPrefabs.GetNextMatching(random, _pickEvaluator);
			if (nextMatching != null)
			{
				return nextMatching;
			}
			List<T> usedEntries = _randomlyOrderedPrefabs.UsedEntries;
			random.Shuffle(usedEntries);
			for (int i = 0; i < usedEntries.Count; i++)
			{
				T val = usedEntries[i];
				if (_pickEvaluator(val))
				{
					return val;
				}
			}
			return (T)null;
		}

		public T PickRandomPrefab(SyboRandom random)
		{
			if (IsEmpty)
			{
			}
			_weightedRandomizedPrefabs.Clear();
			using (IEnumerator<TWeighted> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TWeighted current = enumerator.Current;
					if (_pickEvaluator(current.Prefab))
					{
						_weightedRandomizedPrefabs.Add(current.Prefab, current.Weight);
					}
				}
			}
			return _weightedRandomizedPrefabs.GetRandom(random);
		}

		public override string ToString()
		{
			return string.Format("[WeightRandomizedPrefabs: Name={1}, Empty={0}]", IsEmpty, Name);
		}
	}
}
