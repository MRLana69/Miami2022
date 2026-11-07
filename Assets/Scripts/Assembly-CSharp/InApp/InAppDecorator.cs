using Kiloo.Common.Extensions;

namespace InApp
{
	public static class InAppDecorator
	{
		public static string GetIconName(string itemId)
		{
			if (itemId.IsOneOf("com.kiloo.subways.coinstier1", "com.kiloo.subways.coinstier1discount", "com.kiloo.subwaysurfers.coinstier1specialdiscount"))
			{
				return "coin_pack_1";
			}
			if (itemId.IsOneOf("com.kiloo.subways.coinstier2", "com.kiloo.subways.coinstier2_discount", "com.kiloo.subwaysurfers.coinstier2specialdiscount"))
			{
				return "coin_pack_2";
			}
			if (itemId.IsOneOf("com.kiloo.subways.coinstier2.5", "com.kiloo.subways.coinstier2.5discount", "com.kiloo.subwaysurfers.coinstier2.5specialdiscount"))
			{
				return "coin_pack_3";
			}
			if (itemId.IsOneOf("com.kiloo.subways.coinstier3", "com.kiloo.subways.coinstier3discount", "com.kiloo.subwaysurfers.coinstier3specialdiscount"))
			{
				return "coin_pack_4";
			}
			if (itemId.IsOneOf("com.kiloo.subways.coinstier4", "com.kiloo.subways.coinstier4discount", "com.kiloo.subwaysurfers.coinstier4specialdiscount"))
			{
				return "coin_pack_5";
			}
			if (itemId.IsOneOf("com.kiloo.subways.coinstier5", "com.kiloo.subways.coinstier5discount", "com.kiloo.subwaysurfers.coinstier5specialdiscount"))
			{
				return "coin_pack_6";
			}
			if (itemId.IsOneOf("com.kiloo.subways.keystier1", "com.kiloo.subways.keystier1discount", "com.kiloo.subwaysurfers.keystier1specialdiscount"))
			{
				return "key_pack_1";
			}
			if (itemId.IsOneOf("com.kiloo.subways.keystier1.5", "com.kiloo.subways.keystier1.5discount", "com.kiloo.subwaysurfers.keystier1.5specialdiscount"))
			{
				return "key_pack_2";
			}
			if (itemId.IsOneOf("com.kiloo.subways.keystier2", "com.kiloo.subways.keystier2discount", "com.kiloo.subwaysurfers.keystier2specialdiscount"))
			{
				return "key_pack_3";
			}
			if (itemId.IsOneOf("com.kiloo.subways.keystier3", "com.kiloo.subways.keystier3discount", "com.kiloo.subwaysurfers.keystier3specialdiscount"))
			{
				return "key_pack_4";
			}
			if (itemId.IsOneOf("com.kiloo.subways.keystier4", "com.kiloo.subways.keystier4discount", "com.kiloo.subwaysurfers.keystier4specialdiscount"))
			{
				return "key_pack_5";
			}
			if (itemId.IsOneOf("com.kiloo.subways.doublecoins", "com.kiloo.subways.doublecoinsdiscount"))
			{
				return "coin_double";
			}
			if (itemId.IsOneOf("com.kiloo.subways.adremovaltier1", "com.kiloo.subways.adremovaltier2", "com.kiloo.subways.adremovaltier3", "com.kiloo.subways.adremovaltier4"))
			{
				return "Icon_Ad_Removal_Stroke";
			}
			if (itemId.IsOneOf("com.kiloo.subwayboard.rioboard03.bundletier1", "com.kiloo.subwayboard.rioboard03.bundletier2"))
			{
				return "icon_chroma";
			}
			if (itemId.IsOneOf("com.kiloo.subways.bundletier1"))
			{
				return "surfer_pack";
			}
			if (itemId.IsOneOf("com.kiloo.subways.bundletier2"))
			{
				return "pro_pack";
			}
			if (itemId.IsOneOf("com.kiloo.subways.bundletier3"))
			{
				return "champion_pack";
			}
			if (itemId.IsOneOf("com.kiloo.subways.bundletier4"))
			{
				return "master_pack";
			}
			if (itemId.IsOneOf("com.kiloo.subways.bundletier5"))
			{
				return "legend_pack";
			}
			return string.Empty;
		}
	}
}
