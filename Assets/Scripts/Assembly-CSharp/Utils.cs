using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class Utils
{
	private enum LocaleComponents
	{
		language = 0,
		countryCode = 1
	}

	private static int randomNumber = -1;

	public static string stackTrace
	{
		get
		{
			return new StackTrace().ToString();
		}
	}

	public static StringComparison stringComparison
	{
		get
		{
			return StringComparison.InvariantCulture;
		}
	}

	public static StringComparison stringComparisonIgnoreCase
	{
		get
		{
			return StringComparison.InvariantCultureIgnoreCase;
		}
	}

	public static StringComparer stringComparer
	{
		get
		{
			return StringComparer.InvariantCulture;
		}
	}

	public static StringComparer stringComparerIgnoreCase
	{
		get
		{
			return StringComparer.InvariantCultureIgnoreCase;
		}
	}

	public static T FindObject<T>(this MonoBehaviour obj) where T : class
	{
		T val = UnityEngine.Object.FindObjectOfType(typeof(T)) as T;
		if (val == null)
		{
		}
		return val;
	}

	public static T FindObject<T>(bool warnIfNotFound = true) where T : class
	{
		T[] array = UnityEngine.Object.FindObjectsOfType(typeof(T)) as T[];
		if (array == null || array.Length == 0)
		{
			if (warnIfNotFound)
			{
			}
			return (T)null;
		}
		if (array.Length > 1)
		{
		}
		return array[0];
	}

	public static T FindInThisOrParent<T>(Transform t, Func<T, bool> predicate) where T : Component
	{
		Transform transform = t;
		while (transform != null)
		{
			T component = transform.GetComponent<T>();
			if (component != null && (predicate == null || predicate(component)))
			{
				return component;
			}
			transform = transform.parent;
		}
		return (T)null;
	}

	public static T FindInParents<T>(this MonoBehaviour obj, Func<T, bool> predicate) where T : Component
	{
		return FindInThisOrParent(obj.transform.parent, predicate);
	}

	public static T FindComponentInThisOrParents<T>(this MonoBehaviour obj) where T : Component
	{
		return FindComponentInThisOrParents<T>(obj.transform.parent);
	}

	public static T FindComponentInThisOrParents<T>(Transform t) where T : Component
	{
		return FindInThisOrParent<T>(t, null);
	}

	public static string GetLongName(Transform transform)
	{
		return (!(transform == null)) ? (GetLongName(transform.parent) + "/" + transform.name) : string.Empty;
	}

	public static string GetLongNameList(Component[] components)
	{
		string[] array = new string[components.Length];
		for (int i = 0; i < components.Length; i++)
		{
			array[i] = GetLongName(components[i].transform);
		}
		return string.Join(", ", array);
	}

	public static void Bar(string text, float ratio, int offset, Color color)
	{
		float num = 10f;
		float num2 = 20f;
		GUI.color = color;
		GUI.Button(new Rect(num, (float)Screen.height - num2 - num - (float)offset * num2, ((float)Screen.width - 2f * num) * ratio, num2), text);
	}

	public static DateTime StringToDateTime(string value, DateTime defaultValue)
	{
		if (!string.IsNullOrEmpty(value))
		{
			DateTime result;
			if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
			{
				return result;
			}
			if (DateTime.TryParse(value, out result))
			{
				return result;
			}
		}
		return defaultValue;
	}

	private static int GetXorRandomValueForThisSession()
	{
		if (randomNumber == -1)
		{
			randomNumber = UnityEngine.Random.Range(1, 10000);
		}
		return randomNumber;
	}

	public static int EncryptDecryptXORValue(this int value)
	{
		return value ^ GetXorRandomValueForThisSession();
	}

	private static string GetIntBinaryString(int n)
	{
		char[] array = new char[32];
		int num = 31;
		for (int i = 0; i < 32; i++)
		{
			if ((n & (1 << i)) != 0)
			{
				array[num] = '1';
			}
			else
			{
				array[num] = '0';
			}
			num--;
		}
		return new string(array);
	}

	public static void DebugStar(Vector3 position, float radius, Color color, float duration)
	{
		List<Vector3> list = new List<Vector3>(27);
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				for (int k = -1; k <= 1; k++)
				{
					list.Add(new Vector3(i, j, k));
				}
			}
		}
		foreach (Vector3 item in list)
		{
			Debug.DrawRay(position, item.normalized * radius, color, duration);
		}
	}

	public static string GetContryCodeFromLocale(string deviceLocale)
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

	public static bool IsEnum(this Type type)
	{
		return type.IsEnum;
	}

	public static Attribute GetCustomAttribute(Type type, Type attributeType)
	{
		return Attribute.GetCustomAttribute(type, attributeType);
	}

	public static bool IsInterface(this Type type)
	{
		return type.IsInterface;
	}

	public static Type BaseType(this Type type)
	{
		return type.BaseType;
	}

	public static TOutput[] ConvertAll<TInput, TOutput>(TInput[] array, Converter<TInput, TOutput> converter)
	{
		return Array.ConvertAll(array, converter);
	}

	public static void ThreadSleep(TimeSpan timeSpan)
	{
		Thread.Sleep(timeSpan);
	}

	public static float GetAspectRatio()
	{
		if (Screen.width > Screen.height)
		{
			return Mathf.Abs((float)Screen.width / (float)Screen.height);
		}
		return Mathf.Abs((float)Screen.height / (float)Screen.width);
	}
}
