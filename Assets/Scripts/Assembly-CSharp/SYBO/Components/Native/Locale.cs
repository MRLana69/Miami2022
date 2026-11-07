using UnityEngine;

namespace SYBO.Components.Native
{
	public class Locale
	{
		private static AndroidJavaClass _nativePlugin;

		private static AndroidJavaClass NativePlugin
		{
			get
			{
				if (_nativePlugin == null)
				{
					_nativePlugin = new AndroidJavaClass("com.SYBO.nativeplugin.LocaleAPI");
				}
				return _nativePlugin;
			}
		}

		public static string GetISOThreeLetterLanguageCode()
		{
			return _GetISOThreeLetterLanguageCode();
		}

		private static string _GetISOThreeLetterLanguageCode()
		{
			return NativePlugin.CallStatic<string>("getISO3Language", new object[0]);
		}
	}
}
