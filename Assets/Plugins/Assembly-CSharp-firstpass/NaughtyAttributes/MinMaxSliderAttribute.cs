using System;

namespace NaughtyAttributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class MinMaxSliderAttribute : DrawerAttribute
	{
		private float minValue;

		private float maxValue;

		public float MinValue
		{
			get
			{
				return minValue;
			}
		}

		public float MaxValue
		{
			get
			{
				return maxValue;
			}
		}

		public MinMaxSliderAttribute(float minValue, float maxValue)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
		}
	}
}
