using System;

namespace Kiloo
{
	public static class CDateTime
	{
		public static DateTime Now
		{
			get
			{
				return DateTime.Now;
			}
		}

		public static DateTime UtcNow
		{
			get
			{
				return DateTime.UtcNow;
			}
		}

		public static DateTime Today
		{
			get
			{
				return DateTime.Today;
			}
		}
	}
}
