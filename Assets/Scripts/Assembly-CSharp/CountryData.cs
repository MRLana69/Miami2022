using System.Collections.Generic;

public static class CountryData
{
	private static List<Country> _listOfCountries;

	public static Country GetMotherCountryForCountryID(string countryID)
	{
		switch (countryID)
		{
		case "as":
			return new Country("COUNTRY_NAME_US", "United-States", "us");
		case "ai":
			return new Country("COUNTRY_NAME_GB", "United-Kingdom", "gb");
		case "aw":
			return new Country("COUNTRY_NAME_NL", "Netherlands", "nl");
		case "bm":
			return new Country("COUNTRY_NAME_GB", "United-Kingdom", "gb");
		case "ky":
			return new Country("COUNTRY_NAME_GB", "United-Kingdom", "gb");
		case "ck":
			return new Country("COUNTRY_NAME_NZ", "New-Zealand", "nz");
		case "fk":
			return new Country("COUNTRY_NAME_GB", "United-Kingdom", "gb");
		case "fo":
			return new Country("COUNTRY_NAME_DK", "Denmark", "dk");
		case "gf":
			return new Country("COUNTRY_NAME_FR", "France", "fr");
		case "pf":
			return new Country("COUNTRY_NAME_FR", "France", "fr");
		case "gi":
			return new Country("COUNTRY_NAME_GB", "United-Kingdom", "gb");
		case "gl":
			return new Country("COUNTRY_NAME_DK", "Denmark", "dk");
		case "gp":
			return new Country("COUNTRY_NAME_FR", "France", "fr");
		case "gu":
			return new Country("COUNTRY_NAME_US", "United-States", "us");
		case "va":
			return new Country("COUNTRY_NAME_IT", "Italy", "it");
		case "mo":
			return new Country("COUNTRY_NAME_CN", "China", "cn");
		case "mq":
			return new Country("COUNTRY_NAME_FR", "France", "fr");
		case "yt":
			return new Country("COUNTRY_NAME_FR", "France", "fr");
		case "mc":
			return new Country("COUNTRY_NAME_FR", "France", "fr");
		case "ms":
			return new Country("COUNTRY_NAME_GB", "United-Kingdom", "gb");
		case "an":
			return new Country("COUNTRY_NAME_NL", "Netherlands", "nl");
		case "nc":
			return new Country("COUNTRY_NAME_FR", "France", "fr");
		case "nu":
			return new Country("COUNTRY_NAME_NZ", "New-Zealand", "nz");
		case "mp":
			return new Country("COUNTRY_NAME_US", "United-States", "us");
		case "pr":
			return new Country("COUNTRY_NAME_US", "United-States", "us");
		case "re":
			return new Country("COUNTRY_NAME_FR", "France", "fr");
		case "sh":
			return new Country("COUNTRY_NAME_GB", "United-Kingdom", "gb");
		case "pm":
			return new Country("COUNTRY_NAME_FR", "France", "fr");
		case "sm":
			return new Country("COUNTRY_NAME_IT", "Italy", "it");
		case "sx":
			return new Country("COUNTRY_NAME_NL", "Netherlands", "nl");
		case "tk":
			return new Country("COUNTRY_NAME_NZ", "New-Zealand", "nz");
		case "tc":
			return new Country("COUNTRY_NAME_GB", "United-Kingdom", "gb");
		case "vg":
			return new Country("COUNTRY_NAME_GB", "United-Kingdom", "gb");
		case "vi":
			return new Country("COUNTRY_NAME_US", "United-States", "us");
		case "wf":
			return new Country("COUNTRY_NAME_FR", "France", "fr");
		default:
			return null;
		}
	}

	public static List<Country> GetListOfCountries()
	{
		List<Country> list = _listOfCountries;
		if (list == null)
		{
			List<Country> list2 = new List<Country>();
			list2.Add(new Country("COUNTRY_NAME_AF", "Afghanistan", "af"));
			list2.Add(new Country("COUNTRY_NAME_AL", "Albania", "al"));
			list2.Add(new Country("COUNTRY_NAME_DZ", "Algeria", "dz"));
			list2.Add(new Country("COUNTRY_NAME_AO", "Angola", "ao"));
			list2.Add(new Country("COUNTRY_NAME_AG", "Antigua-and-Barbuda", "ag"));
			list2.Add(new Country("COUNTRY_NAME_AR", "Argentina", "ar"));
			list2.Add(new Country("COUNTRY_NAME_AM", "Armenia", "am"));
			list2.Add(new Country("COUNTRY_NAME_AU", "Australia", "au"));
			list2.Add(new Country("COUNTRY_NAME_AT", "Austria", "at"));
			list2.Add(new Country("COUNTRY_NAME_AZ", "Azerbaijan", "az"));
			list2.Add(new Country("COUNTRY_NAME_BS", "Bahamas", "bs"));
			list2.Add(new Country("COUNTRY_NAME_BH", "Bahrain", "bh"));
			list2.Add(new Country("COUNTRY_NAME_BD", "Bangladesh", "bd"));
			list2.Add(new Country("COUNTRY_NAME_BB", "Barbados", "bb"));
			list2.Add(new Country("COUNTRY_NAME_BY", "Belarus", "by"));
			list2.Add(new Country("COUNTRY_NAME_BE", "Belgium", "be"));
			list2.Add(new Country("COUNTRY_NAME_BZ", "Belize", "bz"));
			list2.Add(new Country("COUNTRY_NAME_BJ", "Benin", "bj"));
			list2.Add(new Country("COUNTRY_NAME_BO", "Bolivia", "bo"));
			list2.Add(new Country("COUNTRY_NAME_BA", "Bosnia-and-Herzegovina", "ba"));
			list2.Add(new Country("COUNTRY_NAME_BR", "Brazil", "br"));
			list2.Add(new Country("COUNTRY_NAME_BN", "Brunei", "bn"));
			list2.Add(new Country("COUNTRY_NAME_BG", "Bulgaria", "bg"));
			list2.Add(new Country("COUNTRY_NAME_BF", "Burkina-Faso", "bf"));
			list2.Add(new Country("COUNTRY_NAME_KH", "Cambodia", "kh"));
			list2.Add(new Country("COUNTRY_NAME_CA", "Canada", "ca"));
			list2.Add(new Country("COUNTRY_NAME_CF", "Central-African-Republic", "cf"));
			list2.Add(new Country("COUNTRY_NAME_CL", "Chile", "cl"));
			list2.Add(new Country("COUNTRY_NAME_CN", "China", "cn"));
			list2.Add(new Country("COUNTRY_NAME_CO", "Colombia", "co"));
			list2.Add(new Country("COUNTRY_NAME_CR", "Costa-Rica", "cr"));
			list2.Add(new Country("COUNTRY_NAME_CI", "Cote-d'Ivoire", "ci"));
			list2.Add(new Country("COUNTRY_NAME_HR", "Croatia", "hr"));
			list2.Add(new Country("COUNTRY_NAME_CU", "Cuba", "cu"));
			list2.Add(new Country("COUNTRY_NAME_CY", "Cyprus", "cy"));
			list2.Add(new Country("COUNTRY_NAME_CZ", "Czech-Republic", "cz"));
			list2.Add(new Country("COUNTRY_NAME_DK", "Denmark", "dk"));
			list2.Add(new Country("COUNTRY_NAME_DO", "Dominican-Republic", "do"));
			list2.Add(new Country("COUNTRY_NAME_EC", "Ecuador", "ec"));
			list2.Add(new Country("COUNTRY_NAME_EG", "Egypt", "eg"));
			list2.Add(new Country("COUNTRY_NAME_SV", "El-Salvador", "sv"));
			list2.Add(new Country("COUNTRY_NAME_ER", "Eritrea", "er"));
			list2.Add(new Country("COUNTRY_NAME_EE", "Estonia", "ee"));
			list2.Add(new Country("COUNTRY_NAME_ET", "Ethiopia", "et"));
			list2.Add(new Country("COUNTRY_NAME_FJ", "Fiji", "fj"));
			list2.Add(new Country("COUNTRY_NAME_FI", "Finland", "fi"));
			list2.Add(new Country("COUNTRY_NAME_FR", "France", "fr"));
			list2.Add(new Country("COUNTRY_NAME_GE", "Georgia", "ge"));
			list2.Add(new Country("COUNTRY_NAME_DE", "Germany", "de"));
			list2.Add(new Country("COUNTRY_NAME_GH", "Ghana", "gh"));
			list2.Add(new Country("COUNTRY_NAME_GR", "Greece", "gr"));
			list2.Add(new Country("COUNTRY_NAME_GT", "Guatemala", "gt"));
			list2.Add(new Country("COUNTRY_NAME_GY", "Guyana", "gy"));
			list2.Add(new Country("COUNTRY_NAME_HT", "Haiti", "ht"));
			list2.Add(new Country("COUNTRY_NAME_HN", "Honduras", "hn"));
			list2.Add(new Country("COUNTRY_NAME_HK", "Hong-Kong", "hk"));
			list2.Add(new Country("COUNTRY_NAME_HU", "Hungary", "hu"));
			list2.Add(new Country("COUNTRY_NAME_IS", "Iceland", "is"));
			list2.Add(new Country("COUNTRY_NAME_IN", "India", "in"));
			list2.Add(new Country("COUNTRY_NAME_ID", "Indonesia", "id"));
			list2.Add(new Country("COUNTRY_NAME_IR", "Iran", "ir"));
			list2.Add(new Country("COUNTRY_NAME_IQ", "Iraq", "iq"));
			list2.Add(new Country("COUNTRY_NAME_IE", "Ireland", "ie"));
			list2.Add(new Country("COUNTRY_NAME_IL", "Israel", "il"));
			list2.Add(new Country("COUNTRY_NAME_IT", "Italy", "it"));
			list2.Add(new Country("COUNTRY_NAME_JM", "Jamaica", "jm"));
			list2.Add(new Country("COUNTRY_NAME_JP", "Japan", "jp"));
			list2.Add(new Country("COUNTRY_NAME_JO", "Jordan", "jo"));
			list2.Add(new Country("COUNTRY_NAME_KZ", "Kazakhstan", "kz"));
			list2.Add(new Country("COUNTRY_NAME_KE", "Kenya", "ke"));
			list2.Add(new Country("COUNTRY_NAME_KI", "Kiribati", "ki"));
			list2.Add(new Country("COUNTRY_NAME_KW", "Kuwait", "kw"));
			list2.Add(new Country("COUNTRY_NAME_KG", "Kyrgyzstan", "kg"));
			list2.Add(new Country("COUNTRY_NAME_LA", "Laos", "la"));
			list2.Add(new Country("COUNTRY_NAME_LV", "Latvia", "lv"));
			list2.Add(new Country("COUNTRY_NAME_LB", "Lebanon", "lb"));
			list2.Add(new Country("COUNTRY_NAME_LY", "Libya", "ly"));
			list2.Add(new Country("COUNTRY_NAME_LI", "Liechtenstein", "li"));
			list2.Add(new Country("COUNTRY_NAME_LT", "Lithuania", "lt"));
			list2.Add(new Country("COUNTRY_NAME_LU", "Luxembourg", "lu"));
			list2.Add(new Country("COUNTRY_NAME_MK", "Macedonia", "mk"));
			list2.Add(new Country("COUNTRY_NAME_MY", "Malaysia", "my"));
			list2.Add(new Country("COUNTRY_NAME_MV", "Maldives", "mv"));
			list2.Add(new Country("COUNTRY_NAME_ML", "Mali", "ml"));
			list2.Add(new Country("COUNTRY_NAME_MT", "Malta", "mt"));
			list2.Add(new Country("COUNTRY_NAME_MR", "Mauritania", "mr"));
			list2.Add(new Country("COUNTRY_NAME_MU", "Mauritius", "mu"));
			list2.Add(new Country("COUNTRY_NAME_MX", "Mexico", "mx"));
			list2.Add(new Country("COUNTRY_NAME_MD", "Moldova", "md"));
			list2.Add(new Country("COUNTRY_NAME_MN", "Mongolia", "mn"));
			list2.Add(new Country("COUNTRY_NAME_ME", "Montenegro", "me"));
			list2.Add(new Country("COUNTRY_NAME_MA", "Morocco", "ma"));
			list2.Add(new Country("COUNTRY_NAME_MZ", "Mozambique", "mz"));
			list2.Add(new Country("COUNTRY_NAME_MM", "Myanmar", "mm"));
			list2.Add(new Country("COUNTRY_NAME_NA", "Namibia", "na"));
			list2.Add(new Country("COUNTRY_NAME_NR", "Nauru", "nr"));
			list2.Add(new Country("COUNTRY_NAME_NP", "Nepal", "np"));
			list2.Add(new Country("COUNTRY_NAME_NL", "Netherlands", "nl"));
			list2.Add(new Country("COUNTRY_NAME_NZ", "New-Zealand", "nz"));
			list2.Add(new Country("COUNTRY_NAME_NI", "Nicaragua", "ni"));
			list2.Add(new Country("COUNTRY_NAME_NG", "Nigeria", "ng"));
			list2.Add(new Country("COUNTRY_NAME_KP", "North-Korea", "kp"));
			list2.Add(new Country("COUNTRY_NAME_NO", "Norway", "no"));
			list2.Add(new Country("COUNTRY_NAME_OM", "Oman", "om"));
			list2.Add(new Country("COUNTRY_NAME_PK", "Pakistan", "pk"));
			list2.Add(new Country("COUNTRY_NAME_PS", "Palestine", "ps"));
			list2.Add(new Country("COUNTRY_NAME_PA", "Panama", "pa"));
			list2.Add(new Country("COUNTRY_NAME_PY", "Paraguay", "py"));
			list2.Add(new Country("COUNTRY_NAME_PE", "Peru", "pe"));
			list2.Add(new Country("COUNTRY_NAME_PH", "Philippines", "ph"));
			list2.Add(new Country("COUNTRY_NAME_PL", "Poland", "pl"));
			list2.Add(new Country("COUNTRY_NAME_PT", "Portugal", "pt"));
			list2.Add(new Country("COUNTRY_NAME_QA", "Qatar", "qa"));
			list2.Add(new Country("COUNTRY_NAME_RO", "Romania", "ro"));
			list2.Add(new Country("COUNTRY_NAME_RU", "Russia", "ru"));
			list2.Add(new Country("COUNTRY_NAME_KN", "Saint-Kitts-and-Nevis", "kn"));
			list2.Add(new Country("COUNTRY_NAME_LC", "Saint-Lucia", "lc"));
			list2.Add(new Country("COUNTRY_NAME_VC", "Saint-Vincent-and-the-Grenadines", "vc"));
			list2.Add(new Country("COUNTRY_NAME_SA", "Saudi-Arabia", "sa"));
			list2.Add(new Country("COUNTRY_NAME_SN", "Senegal", "sn"));
			list2.Add(new Country("COUNTRY_NAME_RS", "Serbia", "rs"));
			list2.Add(new Country("COUNTRY_NAME_SG", "Singapore", "sg"));
			list2.Add(new Country("COUNTRY_NAME_SK", "Slovakia", "sk"));
			list2.Add(new Country("COUNTRY_NAME_SI", "Slovenia", "si"));
			list2.Add(new Country("COUNTRY_NAME_ZA", "South-Africa", "za"));
			list2.Add(new Country("COUNTRY_NAME_KR", "South-Korea", "kr"));
			list2.Add(new Country("COUNTRY_NAME_SS", "South-Sudan", "ss"));
			list2.Add(new Country("COUNTRY_NAME_ES", "Spain", "es"));
			list2.Add(new Country("COUNTRY_NAME_LK", "Sri-Lanka", "lk"));
			list2.Add(new Country("COUNTRY_NAME_SD", "Sudan", "sd"));
			list2.Add(new Country("COUNTRY_NAME_SR", "Suriname", "sr"));
			list2.Add(new Country("COUNTRY_NAME_SE", "Sweden", "se"));
			list2.Add(new Country("COUNTRY_NAME_CH", "Switzerland", "ch"));
			list2.Add(new Country("COUNTRY_NAME_SY", "Syria", "sy"));
			list2.Add(new Country("COUNTRY_NAME_TW", "Taiwan", "tw"));
			list2.Add(new Country("COUNTRY_NAME_TJ", "Tajikistan", "tj"));
			list2.Add(new Country("COUNTRY_NAME_TZ", "Tanzania", "tz"));
			list2.Add(new Country("COUNTRY_NAME_TH", "Thailand", "th"));
			list2.Add(new Country("COUNTRY_NAME_TT", "Trinidad-and-Tobago", "tt"));
			list2.Add(new Country("COUNTRY_NAME_TN", "Tunisia", "tn"));
			list2.Add(new Country("COUNTRY_NAME_TR", "Turkey", "tr"));
			list2.Add(new Country("COUNTRY_NAME_TM", "Turkmenistan", "tm"));
			list2.Add(new Country("COUNTRY_NAME_TV", "Tuvalu", "tv"));
			list2.Add(new Country("COUNTRY_NAME_UG", "Uganda", "ug"));
			list2.Add(new Country("COUNTRY_NAME_UA", "Ukraine", "ua"));
			list2.Add(new Country("COUNTRY_NAME_AE", "United-Arab-Emirates", "ae"));
			list2.Add(new Country("COUNTRY_NAME_GB", "United-Kingdom", "gb"));
			list2.Add(new Country("COUNTRY_NAME_US", "United-States", "us"));
			list2.Add(new Country("COUNTRY_NAME_UY", "Uruguay", "uy"));
			list2.Add(new Country("COUNTRY_NAME_UZ", "Uzbekistan", "uz"));
			list2.Add(new Country("COUNTRY_NAME_VE", "Venezuela", "ve"));
			list2.Add(new Country("COUNTRY_NAME_VN", "Vietnam", "vn"));
			list2.Add(new Country("COUNTRY_NAME_YE", "Yemen", "ye"));
			list2.Add(new Country("COUNTRY_NAME_ZM", "Zambia", "zm"));
			list2.Add(new Country("COUNTRY_NAME_ZW", "Zimbabwe", "zw"));
			list = (_listOfCountries = list2);
		}
		return list;
	}

	public static Country GetCountryForID(string id)
	{
		string text = id;
		if (!string.IsNullOrEmpty(text))
		{
			text = text.ToLower();
			foreach (Country listOfCountry in GetListOfCountries())
			{
				if (listOfCountry.countryCode == text)
				{
					return listOfCountry;
				}
			}
		}
		return null;
	}
}
