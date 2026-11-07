using System;

namespace SYBO.Common
{
	[Serializable]
	public struct SerializableDate
	{
		public const int DefaultYear = 2019;

		public const int DefaultMonth = 1;

		public const int DefaultDay = 1;

		public int Day;

		public int Month;

		public int Year;

		public static SerializableDate DefaultDate
		{
			get
			{
				return new SerializableDate(1, 1, 2019);
			}
		}

		public SerializableDate(int day, int month, int year)
		{
			Day = day;
			Month = month;
			Year = year;
		}
	}
}
