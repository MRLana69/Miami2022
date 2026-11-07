using System;

namespace SYBO.Common.Utilities
{
	public class EnumUtility
	{
		public static int[] ConvertEnumToIntArray<T>()
		{
			T[] array = (T[])Enum.GetValues(typeof(T));
			int[] array2 = new int[array.Length];
			int num = 0;
			T[] array3 = array;
			foreach (T val in array3)
			{
				array2[num] = (int)Convert.ChangeType(val, typeof(T));
				num++;
			}
			return array2;
		}

		public static T ConvertIntToEnumValue<T>(int i)
		{
			return (T)Enum.ToObject(typeof(T), i);
		}

		public static T ConvertIntToFlagEnumValue<T>(int i)
		{
			T[] array = Enum.GetValues(typeof(T)) as T[];
			return array[i];
		}

		public static T ConvertIndexToEnumValue<T>(int index)
		{
			T[] array = (T[])Enum.GetValues(typeof(T));
			return array[index];
		}

		public static int ConvertEnumValueToIndex<T>(T enumValue)
		{
			T[] array = (T[])Enum.GetValues(typeof(T));
			int num = 0;
			int num2 = -1;
			T[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				T val = array2[i];
				if (val.Equals(enumValue))
				{
					num2 = num;
					break;
				}
				num++;
			}
			if (num2 == -1)
			{
				SDebug.LogError(string.Concat("Did not find enumvalue: ", enumValue, " in Enum"));
			}
			return num2;
		}
	}
}
