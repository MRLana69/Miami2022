using System;
using System.Collections.Generic;

namespace SYBO.Common.Utilities
{
	public class SyboRandom
	{
		private IRandomGen _randomGen;

		public SyboRandom()
		{
			_randomGen = MakeRandomGen();
		}

		public SyboRandom(IRandomGen randomGen)
		{
			_randomGen = randomGen;
		}

		public static IRandomGen MakeRandomGen()
		{
			return new MTRandomGen((uint)(DateTime.Now.Ticks % uint.MaxValue), 0);
		}

		public void SetGenerator(IRandomGen randomGen)
		{
			_randomGen = randomGen;
		}

		public uint NextUint()
		{
			return _randomGen.NextUint();
		}

		public float NextFloat()
		{
			return (float)NextUint() / 4.2949673E+09f;
		}

		public float Value()
		{
			return NextFloat();
		}

		public bool Bool()
		{
			return NextFloat() > 0.5f;
		}

		public int Range(int min, int max)
		{
			if (max <= min)
			{
				return min;
			}
			uint num = uint.MaxValue - uint.MaxValue % (uint)(max - min);
			uint num2;
			do
			{
				num2 = NextUint();
			}
			while (num2 > num);
			return (int)(num2 % (max - min)) + min;
		}

		public float Range(float min, float max)
		{
			if (max <= min)
			{
				return min;
			}
			return NextFloat() * (max - min) + min;
		}

		public T RandomElement<T>(T[] array)
		{
			return array[Range(0, array.Length)];
		}

		public T RandomElementNoRepeats<T>(T[] array, T lastPickedElement) where T : class
		{
			if (array.Length == 1)
			{
				return array[0];
			}
			int num = 0;
			int num2 = 2 * array.Length;
			T val;
			do
			{
				val = array[Range(0, array.Length)];
				num++;
			}
			while (num2 > num && val == lastPickedElement);
			return val;
		}

		public T RandomElementNoRepeats<T>(IList<T> array, T lastPickedElement) where T : class
		{
			if (array.Count == 1)
			{
				return array[0];
			}
			T val;
			do
			{
				val = array[Range(0, array.Count)];
			}
			while (val == lastPickedElement);
			return val;
		}

		public T RandomElement<T>(IList<T> array)
		{
			return array[Range(0, array.Count)];
		}

		public int RandomElementIndex<T>(ICollection<T> array)
		{
			return Range(0, array.Count);
		}

		public void Shuffle<T>(IList<T> list)
		{
			for (int num = list.Count; num > 1; num--)
			{
				int index = Range(0, num);
				T value = list[index];
				list[index] = list[num - 1];
				list[num - 1] = value;
			}
		}
	}
}
