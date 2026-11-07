using System;
using System.Collections.Generic;
using System.Linq;
using Kiloo.Common.Utility;

namespace Kiloo.Common.Extensions
{
	public static class CollectionExtensions
	{
		public static void AddSorted<T>(this List<T> list, T item) where T : IComparable<T>
		{
			if (list.Count == 0)
			{
				list.Add(item);
				return;
			}
			if (list[list.Count - 1].CompareTo(item) <= 0)
			{
				list.Add(item);
				return;
			}
			if (list[0].CompareTo(item) >= 0)
			{
				list.Insert(0, item);
				return;
			}
			int num = list.BinarySearch(item);
			if (num < 0)
			{
				num = ~num;
			}
			list.Insert(num, item);
		}

		public static T GetRandomElement<T>(this IEnumerable<T> sequence)
		{
			T[] array = (sequence as T[]) ?? sequence.ToArray();
			return array.ElementAt(KRandom.Range(0, array.Length));
		}

		public static T GetRandomElement<T>(this IList<T> list)
		{
			return list[KRandom.Range(0, list.Count)];
		}

		public static void Shuffle<T>(this IList<T> list)
		{
			for (int num = list.Count; num > 1; num--)
			{
				int j = KRandom.Range(0, num);
				list.SwitchPlaces(num, j);
			}
		}

		public static void Shuffle<T>(this IList<T> list, int seed)
		{
			Random random = new Random(seed);
			for (int num = list.Count; num > 0; num--)
			{
				int j = random.Next(num);
				list.SwitchPlaces(num, j);
			}
		}

		public static void SwitchPlaces<T>(this IList<T> arr, int i, int j)
		{
			if (i < arr.Count && j < arr.Count)
			{
				T value = arr[i];
				arr[i] = arr[j];
				arr[j] = value;
			}
		}

		public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
		{
			return listToClone.Select((T item) => (T)item.Clone()).ToList();
		}

		public static Dictionary<T, U> ToDictionary<T, U>(this List<KeyValuePair<T, U>> keyvalueList)
		{
			Dictionary<T, U> dictionary = new Dictionary<T, U>(keyvalueList.Count);
			foreach (KeyValuePair<T, U> keyvalue in keyvalueList)
			{
				dictionary.Add(keyvalue.Key, keyvalue.Value);
			}
			return dictionary;
		}

		public static List<KeyValuePair<T, U>> ToKeyValueList<T, U>(this Dictionary<T, U> dictionary)
		{
			List<KeyValuePair<T, U>> list = new List<KeyValuePair<T, U>>(dictionary.Count);
			foreach (KeyValuePair<T, U> item in dictionary)
			{
				list.Add(item);
			}
			return list;
		}

		public static bool ValidIndex<T>(this IList<T> l, int index)
		{
			return index >= 0 && index < l.Count;
		}

		public static bool AddDistinct<T>(this IList<T> list, T value)
		{
			return list.AddDistinct(value, EqualityComparer<T>.Default);
		}

		private static bool AddDistinct<T>(this IList<T> list, T value, IEqualityComparer<T> comparer)
		{
			if (list.ContainsValue(value, comparer))
			{
				return false;
			}
			list.Add(value);
			return true;
		}

		private static bool ContainsValue<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
		{
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			foreach (TSource item in source)
			{
				if (comparer.Equals(item, value))
				{
					return true;
				}
			}
			return false;
		}

		public static bool AddRangeDistinct<T>(this IList<T> list, IEnumerable<T> values)
		{
			return list.AddRangeDistinct(values, EqualityComparer<T>.Default);
		}

		public static bool AddRangeDistinct<T>(this IList<T> list, IEnumerable<T> values, IEqualityComparer<T> comparer)
		{
			bool result = true;
			foreach (T value in values)
			{
				if (!list.AddDistinct(value, comparer))
				{
					result = false;
				}
			}
			return result;
		}

		public static bool TryGetValueNotNull<T, U>(this IDictionary<T, U> dict, T key, out U obj)
		{
			return dict.TryGetValue(key, out obj) && obj != null;
		}

		public static bool TryGetValueNotNullOrEmpty<T>(this IDictionary<T, string> dict, T key, out string obj)
		{
			return dict.TryGetValue(key, out obj) && !string.IsNullOrEmpty(obj);
		}

		public static void AddOrSet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			if (dict.ContainsKey(key))
			{
				dict[key] = value;
			}
			else
			{
				dict.Add(key, value);
			}
		}

		public static void RemoveWhere<TKey, TValue>(this IDictionary<TKey, TValue> dict, Func<KeyValuePair<TKey, TValue>, bool> predicate)
		{
			List<KeyValuePair<TKey, TValue>> list = dict.Where(predicate).ToList();
			foreach (KeyValuePair<TKey, TValue> item in list)
			{
				dict.Remove(item.Key);
			}
		}
	}
}
