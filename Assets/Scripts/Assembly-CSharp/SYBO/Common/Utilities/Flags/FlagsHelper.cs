using System;

namespace SYBO.Common.Utilities.Flags
{
	public static class FlagsHelper
	{
		public static bool IsSet<T>(T flags, T flag) where T : struct
		{
			int num = (int)(object)flags;
			int num2 = (int)(object)flag;
			return (num & num2) != 0;
		}

		public static void Set<T>(ref T flags, T flag) where T : struct
		{
			int num = (int)(object)flags;
			int num2 = (int)(object)flag;
			flags = (T)(object)(num | num2);
		}

		public static void SetEverything<T>(ref T flags) where T : struct
		{
			foreach (T value in Enum.GetValues(typeof(T)))
			{
				Set(ref flags, value);
			}
		}

		public static void SetNothing<T>(ref T flags) where T : struct
		{
			foreach (T value in Enum.GetValues(typeof(T)))
			{
				Unset(ref flags, value);
			}
		}

		public static void Unset<T>(ref T flags, T flag) where T : struct
		{
			int num = (int)(object)flags;
			int num2 = (int)(object)flag;
			flags = (T)(object)(num & ~num2);
		}
	}
}
