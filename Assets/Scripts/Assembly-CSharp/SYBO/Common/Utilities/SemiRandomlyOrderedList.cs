using System;
using System.Collections.Generic;

namespace SYBO.Common.Utilities
{
	public class SemiRandomlyOrderedList<T>
	{
		private List<T> _readyEntries = new List<T>();

		private List<T> _usedEntries = new List<T>();

		private List<T> _allEntries = new List<T>();

		private HashSet<T> _evaluatedEntries = new HashSet<T>();

		public bool IsEmpty
		{
			get
			{
				return _readyEntries.Count == 0 && _usedEntries.Count == 0;
			}
		}

		public int TotalEntries
		{
			get
			{
				return _readyEntries.Count + _usedEntries.Count;
			}
		}

		public List<T> ReadyEntries
		{
			get
			{
				return _readyEntries;
			}
		}

		public List<T> UsedEntries
		{
			get
			{
				return _usedEntries;
			}
		}

		public List<T> AllEntries
		{
			get
			{
				return _allEntries;
			}
		}

		public void Build(IEnumerable<T> sourceList, SyboRandom random)
		{
			_readyEntries.AddRange(sourceList);
			_allEntries.AddRange(sourceList);
			Reset(random);
		}

		public void Add(T entry)
		{
			_readyEntries.Add(entry);
			_allEntries.Add(entry);
		}

		public void Clear()
		{
			_readyEntries.Clear();
			_usedEntries.Clear();
			_allEntries.Clear();
		}

		public void Reset(SyboRandom random)
		{
			Regenerate(random);
			random.Shuffle(_readyEntries);
		}

		public T GetNext(SyboRandom random)
		{
			if (_readyEntries.Count == 0)
			{
				return default(T);
			}
			T val = _readyEntries[0];
			Consume(val, random);
			return val;
		}

		public T GetNextMatching(SyboRandom random, Func<T, bool> matchSelector)
		{
			_evaluatedEntries.Clear();
			int num = 0;
			while (num < _readyEntries.Count)
			{
				T val = _readyEntries[num++];
				if (!_evaluatedEntries.Contains(val))
				{
					if (matchSelector(val))
					{
						Consume(val, random);
						return val;
					}
					_evaluatedEntries.Add(val);
				}
			}
			return default(T);
		}

		public void Consume(T entry, SyboRandom random)
		{
			_readyEntries.Remove(entry);
			_usedEntries.Add(entry);
			RegenerateIfNeeded(random);
		}

		private void RegenerateIfNeeded(SyboRandom random)
		{
			if (_readyEntries.Count <= 3 && _usedEntries.Count >= 4)
			{
				Regenerate(random);
			}
			else if (_readyEntries.Count <= 2 && _usedEntries.Count >= 3)
			{
				Regenerate(random);
			}
			else if (_readyEntries.Count <= 1)
			{
				Regenerate(random);
			}
		}

		private void Regenerate(SyboRandom random)
		{
			if (_usedEntries.Count != 0)
			{
				random.Shuffle(_usedEntries);
				_readyEntries.AddRange(_usedEntries);
				_usedEntries.Clear();
			}
		}

		public static void TestUniformity()
		{
			SyboRandom random = new SyboRandom(new MTRandomGen(1337u, 0));
			int num = 4;
			List<int> list = new List<int>();
			SemiRandomlyOrderedList<int> semiRandomlyOrderedList = new SemiRandomlyOrderedList<int>();
			for (int i = 0; i < num; i++)
			{
				semiRandomlyOrderedList.Add(i);
				list.Add(0);
			}
			semiRandomlyOrderedList.Reset(random);
			for (int j = 0; j < 10000; j++)
			{
				list[semiRandomlyOrderedList.GetNext(random)]++;
			}
			for (int k = 0; k < num; k++)
			{
			}
		}
	}
}
