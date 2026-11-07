namespace Kiloo.Common.LocalizationManager
{
	public class Locale
	{
		public enum LocaleType
		{
			Language = 0,
			LanguageAndRegion = 1
		}

		public readonly LocaleType localeType;

		public readonly string language;

		public readonly string region;

		public readonly string[] extraRegions;

		public readonly string combined;

		public readonly string nativeName = "Unknown";

		private Locale(string language, string region, string[] extraRegions, string nativeName)
		{
			localeType = ((!string.IsNullOrEmpty(region)) ? LocaleType.LanguageAndRegion : LocaleType.Language);
			this.language = language.ToLower();
			this.region = ((!string.IsNullOrEmpty(region)) ? region.ToUpper() : string.Empty);
			combined = GetLanguageCode();
			this.extraRegions = extraRegions ?? new string[0];
			if (nativeName == null)
			{
				string value;
				if (LocaleData.instance.nativeNames.TryGetValue(combined, out value))
				{
					this.nativeName = value;
				}
			}
			else
			{
				this.nativeName = nativeName;
			}
		}

		private bool IsValid()
		{
			switch (localeType)
			{
			case LocaleType.Language:
				return language.Length > 0;
			case LocaleType.LanguageAndRegion:
				return language.Length > 0 && region.Length > 0;
			default:
				SDebug.LogError("Unhandled localeType: " + localeType);
				return false;
			}
		}

		public static Locale Create(string language, string region = null, string[] extraRegions = null, string nativeName = null)
		{
			return new Locale(language, region, extraRegions, nativeName);
		}

		private string GetLanguageCode()
		{
			switch (localeType)
			{
			case LocaleType.Language:
				return language;
			case LocaleType.LanguageAndRegion:
				return string.Format("{0}-{1}", language, region);
			default:
				SDebug.LogError("Unhandled localeType: " + localeType);
				return string.Empty;
			}
		}

		public static bool TryParse(string input, out Locale locale)
		{
			try
			{
				locale = Parse(input);
				if (locale != null)
				{
					return true;
				}
			}
			catch
			{
			}
			locale = null;
			return false;
		}

		public static Locale Parse(string input)
		{
			string[] array = input.Split('-', '_');
			Locale locale;
			switch (array.Length)
			{
			case 1:
				locale = Create(array[0]);
				break;
			case 2:
				locale = Create(array[0], array[1]);
				break;
			default:
				locale = null;
				break;
			}
			if (locale != null && locale.IsValid())
			{
				return locale;
			}
			return null;
		}

		public static bool operator ==(Locale loc1, Locale loc2)
		{
			if ((object)loc1 == loc2)
			{
				return true;
			}
			if ((object)loc1 == null || (object)loc2 == null)
			{
				return false;
			}
			return string.Equals(loc1.language, loc2.language) && string.Equals(loc1.region, loc2.region);
		}

		public static bool operator !=(Locale loc1, Locale loc2)
		{
			return !(loc1 == loc2);
		}

		public override bool Equals(object obj)
		{
			return this == (Locale)obj;
		}

		public override int GetHashCode()
		{
			return (((language != null) ? language.GetHashCode() : 0) * 397) ^ ((region != null) ? region.GetHashCode() : 0);
		}
	}
}
