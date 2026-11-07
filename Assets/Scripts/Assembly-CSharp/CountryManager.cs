using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Common.Plugins.DeviceUtility;
using TopRun;
using UnityEngine;

public class CountryManager
{
	private enum LocaleComponents
	{
		language = 0,
		countryCode = 1
	}

	private static CountryManager _instance;

	public static CountryManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new CountryManager();
			}
			return _instance;
		}
	}

	private CountryManager()
	{
	}

	public string GetDefaultCountryCodeFromDevice()
	{
		if (Application.isEditor)
		{
			return "DK";
		}
		Country country = null;
		string countryCodeFromLocale = GetCountryCodeFromLocale(DeviceUtility.GetLocale());
		country = CountryData.GetCountryForID(countryCodeFromLocale);
		if (country == null)
		{
			country = CountryData.GetMotherCountryForCountryID(countryCodeFromLocale);
		}
		if (country == null)
		{
			return "US";
		}
		return country.countryCode.ToUpper();
	}

	public List<Country> GetListOfCountries()
	{
		return CountryData.GetListOfCountries();
	}

	public int GetCurrentCountryIndex()
	{
		IManager manager = TopRunRoot.manager;
		if (manager != null)
		{
			string text = ((manager.tournament == null) ? manager.country.ToLower() : manager.tournament.country.ToLower());
			for (int i = 0; i < CountryData.GetListOfCountries().Count; i++)
			{
				if (CountryData.GetListOfCountries()[i].countryCode == text)
				{
					return i;
				}
			}
		}
		return 0;
	}

	private static string GetCountryCodeFromLocale(string deviceLocale)
	{
		string result = string.Empty;
		if (!string.IsNullOrEmpty(deviceLocale))
		{
			deviceLocale = deviceLocale.Trim().ToLower();
			deviceLocale = deviceLocale.Replace('-', '_');
			if (deviceLocale.Contains("_"))
			{
				string[] array = deviceLocale.Split('_');
				if (array.Length == 2)
				{
					result = array[1];
				}
			}
		}
		return result;
	}

	public Country GetCurrentTournamentCountry()
	{
		Country country = null;
		IManager manager = TopRunRoot.manager;
		if (manager != null)
		{
			if (manager.tournament != null)
			{
				string country2 = manager.tournament.country;
				country = CountryData.GetCountryForID(country2);
			}
			else
			{
				string country3 = manager.country;
				country = CountryData.GetCountryForID(country3);
			}
		}
		if (country == null)
		{
			country = new Country(string.Empty, string.Empty, string.Empty);
		}
		return country;
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogError(string msg, Object context = null)
	{
		SDebug.LogError("[CountryManager] " + msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogWarning(string msg, Object context = null)
	{
	}

	[Conditional("ENABLE_COUNTRYMANAGER_LOGS")]
	public static void Log(string msg, Object context = null)
	{
	}
}
