namespace Kiloo.Common.Utility
{
	public static class ArrayUtil
	{
		public static bool ArraysAreEqual<T>(T[] a, T[] b)
		{
			if (a == null)
			{
				return b == null;
			}
			if (b == null)
			{
				return false;
			}
			if (a.Length != b.Length)
			{
				return false;
			}
			for (int i = 0; i < a.Length; i++)
			{
				if (!object.Equals(a[i], b[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
