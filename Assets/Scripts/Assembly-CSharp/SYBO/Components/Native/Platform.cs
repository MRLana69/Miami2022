using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SYBO.Components.Native
{
	public class Platform
	{
		private static AndroidJavaClass _nativePlugin;

		private static Dictionary<int, string> _APIToPlatform;

		private static AndroidJavaClass NativePlugin
		{
			get
			{
				if (_nativePlugin == null)
				{
					_nativePlugin = new AndroidJavaClass("com.SYBO.nativeplugin.PlatformAPI");
				}
				return _nativePlugin;
			}
		}

		private static Dictionary<int, string> APIToPlatform
		{
			get
			{
				if (_APIToPlatform == null)
				{
					Dictionary<int, string> dictionary = new Dictionary<int, string>();
					dictionary.Add(1, "1.0");
					dictionary.Add(2, "1.1");
					dictionary.Add(3, "1.5");
					dictionary.Add(4, "1.6");
					dictionary.Add(5, "2.0");
					dictionary.Add(6, "2.0");
					dictionary.Add(7, "2.1");
					dictionary.Add(8, "2.2.3");
					dictionary.Add(9, "2.3");
					dictionary.Add(10, "2.3.7");
					dictionary.Add(11, "3.0");
					dictionary.Add(12, "3.1");
					dictionary.Add(13, "3.2.6");
					dictionary.Add(14, "4.0");
					dictionary.Add(15, "4.0.4");
					dictionary.Add(16, "4.1");
					dictionary.Add(17, "4.2");
					dictionary.Add(18, "4.3.1");
					dictionary.Add(19, "4.4");
					dictionary.Add(20, "4.4.4");
					dictionary.Add(21, "5.0");
					dictionary.Add(22, "5.1.1");
					dictionary.Add(23, "6.0.1");
					dictionary.Add(24, "7.0");
					dictionary.Add(25, "7.1.2");
					dictionary.Add(26, "8.0");
					dictionary.Add(27, "8.0+");
					dictionary.Add(28, "8.0+");
					dictionary.Add(29, "8.0+");
					_APIToPlatform = dictionary;
				}
				return _APIToPlatform;
			}
		}

		public static string GetPlatformVersion()
		{
			return _GetPlatformVersion();
		}

		private static string _GetPlatformVersion()
		{
			string text = StringCleaner(NativePlugin.CallStatic<string>("GetPlatformVersion", new object[0]));
			int num = NativePlugin.CallStatic<int>("GetAPILevel", new object[0]);
			if (text == null || text.Length < 3)
			{
				string value;
				text = ((!APIToPlatform.TryGetValue(num, out value)) ? "N/A" : value);
			}
			return string.Format("Android OS {0}/ API {1}", text, num);
		}

		private static string StringCleaner(string stringToClean)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < stringToClean.Length; i++)
			{
				if (char.IsDigit(stringToClean[i]) || stringToClean[i] == '.')
				{
					stringBuilder.Append(stringToClean[i]);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
