namespace SYBO.Common.Math
{
	public static class MathUtil
	{
		public static float SnapValueStep(float value, float step)
		{
			if (step == 0f)
			{
				return value;
			}
			int num = (int)(value / step);
			return (float)num * step;
		}

		public static float ClampMaxAboveChangedMin(float max, float min, float minPrev)
		{
			if (min != minPrev && max < min)
			{
				return min;
			}
			return max;
		}

		public static float ClampMinBelowChangedMax(float min, float max, float maxPrev)
		{
			if (max != maxPrev && max < min)
			{
				return max;
			}
			return min;
		}

		public static bool IsValueInRange(float value, float min, float max, bool minInclusive = true, bool maxInclusive = true)
		{
			return ((!minInclusive) ? (min < value) : (min <= value)) && ((!maxInclusive) ? (value < max) : (value <= max));
		}

		public static bool AreRangesOverlapping(float range1Start, float range1End, float range2Start, float range2End)
		{
			return IsValueInRange(range1Start, range2Start, range2End) || IsValueInRange(range1End, range2Start, range2End) || IsValueInRange(range2Start, range1Start, range1End) || IsValueInRange(range2End, range1Start, range1End);
		}
	}
}
