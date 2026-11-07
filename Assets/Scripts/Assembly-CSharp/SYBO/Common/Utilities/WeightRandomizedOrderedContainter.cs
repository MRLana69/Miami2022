using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Common.Utilities
{
	[Serializable]
	public class WeightRandomizedOrderedContainter<TWeighted, T> : IEnumerable<TWeighted>, IEnumerable where TWeighted : WeightedObject<T> where T : class
	{
		[SerializeField]
		private List<TWeighted> _weightedObjects = new List<TWeighted>();

		private WeightRandomizedContainer<T> _weightedRandomizedObjects = new WeightRandomizedContainer<T>();

		private SemiRandomlyOrderedList<T> _randomlyOrderedObjects = new SemiRandomlyOrderedList<T>();

		private Func<T, bool> _pickEvaluator = (T p) => true;

		public bool IsEmpty
		{
			get
			{
				for (int i = 0; i < _weightedObjects.Count; i++)
				{
					if (_weightedObjects[i] != null)
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
				return _weightedObjects.Count;
			}
		}

		public TWeighted this[int index]
		{
			get
			{
				return _weightedObjects[index];
			}
			set
			{
				_weightedObjects[index] = value;
			}
		}

		public IEnumerator<TWeighted> GetEnumerator()
		{
			return Enumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Enumerator();
		}

		private IEnumerator<TWeighted> Enumerator()
		{
			for (int i = 0; i < _weightedObjects.Count; i++)
			{
				if (_weightedObjects[i] != null)
				{
					yield return _weightedObjects[i];
				}
			}
		}

		public void Add(TWeighted item)
		{
			_weightedObjects.Add(item);
		}

		public void Clear()
		{
			_weightedObjects.Clear();
			_weightedRandomizedObjects.Clear();
			_randomlyOrderedObjects.Clear();
		}

		public TWeighted Find(Predicate<TWeighted> predicate)
		{
			return _weightedObjects.Find(predicate);
		}

		public void SetPickEvaluator(Func<T, bool> evaluator)
		{
			_pickEvaluator = evaluator;
		}

		private void BuildRandomlyOrderedList(SyboRandom random)
		{
			using (IEnumerator<TWeighted> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TWeighted current = enumerator.Current;
					for (int i = 0; i < current.Weight; i++)
					{
						if (current.Object == null)
						{
							break;
						}
						_randomlyOrderedObjects.Add(current.Object);
					}
				}
			}
			_randomlyOrderedObjects.Reset(random);
		}

		public T PickSemiRandomlyOrdered(SyboRandom random)
		{
			if (_randomlyOrderedObjects.IsEmpty && !IsEmpty)
			{
				BuildRandomlyOrderedList(random);
			}
			T nextMatching = _randomlyOrderedObjects.GetNextMatching(random, _pickEvaluator);
			if (nextMatching != null)
			{
				return nextMatching;
			}
			List<T> usedEntries = _randomlyOrderedObjects.UsedEntries;
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
			_weightedRandomizedObjects.Clear();
			using (IEnumerator<TWeighted> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TWeighted current = enumerator.Current;
					if (_pickEvaluator(current.Object))
					{
						_weightedRandomizedObjects.Add(current.Object, current.Weight);
					}
				}
			}
			return _weightedRandomizedObjects.GetRandom(random);
		}

		public override string ToString()
		{
			return string.Format("[WeightRandomizedContainter: Name={1}, Empty={0}]", IsEmpty, Name);
		}
	}
}
