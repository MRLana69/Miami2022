using System.Globalization;
using System.Linq;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Kiloo.Common.Extensions
{
	public static class NumberExtensions
	{
		public static float MRound(this float f, float roundFactor)
		{
			return (f / roundFactor).Round() * roundFactor;
		}

		public static int MRoundToInt(this float f, float roundFactor)
		{
			return f.MRound(roundFactor).RoundToInt();
		}

		public static bool IsBetween(this int current, int low, int high)
		{
			return current > low && current < high;
		}

		public static bool Chance(this float percentChance)
		{
			return KRandom.Range(0f, 1f) <= percentChance;
		}

		public static float Roll(this float max)
		{
			return KRandom.Range(0f, max);
		}

		public static int Roll(this int max)
		{
			return KRandom.Range(0, max);
		}

		public static float Lerp(this float t, float a, float b, bool extrapolate = false)
		{
			if (!extrapolate)
			{
				t = Mathf.Clamp01(t);
			}
			return t * b + (1f - t) * a;
		}

		public static float Variance(this float baseValue, float percentVariance)
		{
			return baseValue + KRandom.Range((0f - baseValue) * percentVariance, baseValue * percentVariance);
		}

		public static bool IsBetween(this float f, float smallFloat, float bigFloat)
		{
			return f > smallFloat && f < bigFloat;
		}

		public static bool IsBetweenInclusive(this float f, float small, float big)
		{
			return f >= small && f <= big;
		}

		public static bool IsOneOf(this int needle, params int[] haystack)
		{
			return haystack.Contains(needle);
		}

		public static string ToOrdinal(this int num)
		{
			if (num <= 0)
			{
				return num.ToString(CultureInfo.InvariantCulture);
			}
			int num2 = num % 100;
			if (num2 == 11 || num2 == 12 || num2 == 13)
			{
				return num + "th";
			}
			switch (num % 10)
			{
			case 1:
				return num + "st";
			case 2:
				return num + "nd";
			case 3:
				return num + "rd";
			default:
				return num + "th";
			}
		}

		public static float Round(this float f)
		{
			return Mathf.Round(f);
		}

		public static int RoundToInt(this float f)
		{
			return Mathf.RoundToInt(f);
		}

		public static float Ceil(this float f)
		{
			return Mathf.Ceil(f);
		}

		public static int CeilToInt(this float f)
		{
			return Mathf.CeilToInt(f);
		}

		public static float Floor(this float f)
		{
			return Mathf.Floor(f);
		}

		public static int FloorToInt(this float f)
		{
			return Mathf.FloorToInt(f);
		}

		public static float Pow(this float f, float power)
		{
			return Mathf.Pow(f, power);
		}

		public static float Pow(this int i, float power)
		{
			return Mathf.Pow(i, power);
		}

		public static float Clamp(this float f, float min, float max)
		{
			return Mathf.Clamp(f, min, max);
		}

		public static int Clamp(this int f, int min, int max)
		{
			return Mathf.Clamp(f, min, max);
		}

		public static float Sqr(this float f)
		{
			return Mathf.Sqrt(f);
		}
	}
}
