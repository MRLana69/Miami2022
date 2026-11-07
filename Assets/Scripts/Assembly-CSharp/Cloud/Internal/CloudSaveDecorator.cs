using System;
using Kiloo.Common.Extensions;

namespace Cloud.Internal
{
	public static class CloudSaveDecorator
	{
		public static string GetName(CloudSaveComparisonElement id)
		{
			switch (id)
			{
			case CloudSaveComparisonElement.Multiplier:
				return Strings.Get("CLOUDSAVE_ADVANCED_MULTIPLIER");
			case CloudSaveComparisonElement.Highscore:
				return Strings.Get("CLOUDSAVE_ADVANCED_HIGHSCORE");
			case CloudSaveComparisonElement.Keys:
				return Strings.Get("CLOUDSAVE_ADVANCED_KEYS");
			case CloudSaveComparisonElement.Upgrades:
				return Strings.Get("CLOUDSAVE_ADVANCED_UPGRADES");
			case CloudSaveComparisonElement.Awards:
				return Strings.Get("CLOUDSAVE_ADVANCED_AWARDS");
			case CloudSaveComparisonElement.Characters:
				return Strings.Get("CLOUDSAVE_ADVANCED_CHARACTERS");
			case CloudSaveComparisonElement.Boards:
				return Strings.Get("CLOUDSAVE_ADVANCED_BOARDS");
			case CloudSaveComparisonElement.Outfits:
				return Strings.Get("CLOUDSAVE_ADVANCED_OUTFITS");
			case CloudSaveComparisonElement.BoardThemes:
				return Strings.Get("CLOUDSAVE_ADVANCED_BOARD_VARIATIONS");
			case CloudSaveComparisonElement.LastModifiedTime:
				return Strings.Get("CLOUDSAVE_ADVANCED_SAVED_LAST");
			case CloudSaveComparisonElement.DeviceSource:
				return Strings.Get("CLOUDSAVE_ADVANCED_SAVED_DEVICE");
			default:
				return string.Empty;
			}
		}

		public static string GetIconName(CloudSaveComparisonElement id)
		{
			switch (id)
			{
			case CloudSaveComparisonElement.Multiplier:
				return "ic_multiplier";
			case CloudSaveComparisonElement.Highscore:
				return "ic_highscore";
			case CloudSaveComparisonElement.Keys:
				return "ic_keys";
			case CloudSaveComparisonElement.Upgrades:
				return "ic_upgrades";
			case CloudSaveComparisonElement.Awards:
				return "ic_awards";
			case CloudSaveComparisonElement.Characters:
				return "ic_character";
			case CloudSaveComparisonElement.Boards:
				return "ic_boards";
			case CloudSaveComparisonElement.Outfits:
				return "ic_character_skins";
			case CloudSaveComparisonElement.BoardThemes:
				return "ic_boards_flavour";
			case CloudSaveComparisonElement.LastModifiedTime:
				return "ic_saved";
			default:
				return string.Empty;
			}
		}

		public static string FormatDateTime(DateTime dt)
		{
			if (dt == DateTime.MinValue)
			{
				return "-";
			}
			return string.Format(Strings.Get("CLOUDSAVE_STATS_SAVED_DAYS_AGO"), (DateTime.UtcNow - dt).NicifyTimeString());
		}

		public static string FormatValue(CloudSaveComparisonElement id, float value)
		{
			switch (id)
			{
			case CloudSaveComparisonElement.Multiplier:
				return "x{0:N0}".Format(value);
			case CloudSaveComparisonElement.Highscore:
				return "{0:N0}".Format(value);
			case CloudSaveComparisonElement.Keys:
				if (Math.Abs(value) < 0.1f)
				{
					return "-";
				}
				return "{0:N0}".Format(value);
			case CloudSaveComparisonElement.Upgrades:
				if (Math.Abs(value) < 0.1f)
				{
					return "-";
				}
				return "{0:N0}".Format(value);
			case CloudSaveComparisonElement.Awards:
				if (Math.Abs(value) < 0.1f)
				{
					return "-";
				}
				return "{0:N0} %".Format(value);
			case CloudSaveComparisonElement.Characters:
				return "{0:N0}".Format(value);
			case CloudSaveComparisonElement.Boards:
				if (Math.Abs(value) < 0.1f)
				{
					return "-";
				}
				return "{0:N0}".Format(value);
			case CloudSaveComparisonElement.Outfits:
				if (Math.Abs(value) < 0.1f)
				{
					return "-";
				}
				return "{0:N0}".Format(value);
			case CloudSaveComparisonElement.BoardThemes:
				if (Math.Abs(value) < 0.1f)
				{
					return "-";
				}
				return "{0:N0}".Format(value);
			default:
				return string.Empty;
			}
		}

		public static string FormatValueWithTitle(CloudSaveComparisonElement id, float value)
		{
			switch (id)
			{
			case CloudSaveComparisonElement.Multiplier:
				return "x{0:N0} {1}".Format(value, GetName(id));
			case CloudSaveComparisonElement.Highscore:
				return "{0:N0}".Format(value);
			case CloudSaveComparisonElement.Keys:
				return "{0:N0} {1}".Format(value, GetName(id));
			case CloudSaveComparisonElement.Upgrades:
				return "{0:N0} {1}".Format(value, GetName(id));
			case CloudSaveComparisonElement.Awards:
				return "{0:N0} % {1}".Format(value, GetName(id));
			case CloudSaveComparisonElement.Characters:
				return "{0:N0} {1}".Format(value, GetName(id));
			case CloudSaveComparisonElement.Boards:
				return "{0:N0} {1}".Format(value, GetName(id));
			case CloudSaveComparisonElement.Outfits:
				return "{0:N0} {1}".Format(value, GetName(id));
			case CloudSaveComparisonElement.BoardThemes:
				return "{0:N0} {1}".Format(value, GetName(id));
			default:
				return string.Empty;
			}
		}
	}
}
