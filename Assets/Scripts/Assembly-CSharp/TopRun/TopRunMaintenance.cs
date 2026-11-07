using System;
using System.Globalization;
using Info;
using Kiloo;

namespace TopRun
{
	public static class TopRunMaintenance
	{
		private static readonly CultureInfo _datetimeCultureinfo = new CultureInfo("da-DK");

		public static bool IsActive()
		{
			string value = OnlineSettings.instance.GetValue("top_run_maintenance_timestamp_end", string.Empty);
			DateTime result;
			if (DateTime.TryParse(value, _datetimeCultureinfo, DateTimeStyles.None, out result))
			{
				return CDateTime.UtcNow < result;
			}
			return false;
		}

		public static void GetMessage(out string line1, out string line2, out string line3)
		{
			string localizedOnlineSettings = Strings.GetLocalizedOnlineSettings("top_run_maintenance_message", string.Empty);
			string[] array = localizedOnlineSettings.Split(';');
			if (array.Length != 3)
			{
				line1 = Strings.Get("TOPRUN_MAINTENANCE_LINE1");
				line2 = Strings.Get("TOPRUN_MAINTENANCE_LINE2");
				line3 = Strings.Get("TOPRUN_MAINTENANCE_LINE3");
			}
			else
			{
				line1 = array[0];
				line2 = array[1];
				line3 = array[2];
			}
		}
	}
}
