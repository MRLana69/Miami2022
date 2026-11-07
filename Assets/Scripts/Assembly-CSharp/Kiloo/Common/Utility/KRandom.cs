using System;

namespace Kiloo.Common.Utility
{
	public static class KRandom
	{
		private static Random _internalRandom;

		private static Random random
		{
			get
			{
				return _internalRandom ?? (_internalRandom = new Random());
			}
		}

		public static float Range(float min, float max)
		{
			float num = (float)random.NextDouble();
			return num * (max - min) + min;
		}

		public static int Range(int min, int max)
		{
			return (min <= max) ? random.Next(min, max) : random.Next(max, min);
		}

		private static double NextTriangularDouble(double min, double max, double mean)
		{
			double num = random.NextDouble();
			return (!(num < (mean - min) / (max - min))) ? (max - Math.Sqrt((1.0 - num) * (max - min) * (max - mean))) : (min + Math.Sqrt(num * (max - min) * (mean - min)));
		}

		public static int RangeTriangle(int min, int max, int mean)
		{
			return (int)Math.Floor(NextTriangularDouble(min, max, mean));
		}

		public static float RangeTriangle(float min, float max, float mean)
		{
			return (float)NextTriangularDouble(min, max, mean);
		}
	}
}
