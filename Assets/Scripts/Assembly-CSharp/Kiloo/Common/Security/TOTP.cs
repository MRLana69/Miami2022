using System;
using System.Text;
using UnityEngine;

namespace Kiloo.Common.Security
{
	public class TOTP
	{
		private static string _appString;

		public static string NewTOTP()
		{
			string s = Eius();
			uint num = 762522550u;
			num ^= num << 21;
			num ^= num >> 3;
			num ^= num << 4;
			num ^= 0x5C90F340;
			TimeSpan timeSpan = TimeSpan.FromSeconds(num);
			long ticks = DateTime.UtcNow.Ticks;
			long ticks2 = timeSpan.Ticks;
			DateTime dateTime = new DateTime(ticks / ticks2 * ticks2, DateTimeKind.Utc);
			DateTime dateTime2 = dateTime.AddTicks(ticks2);
			uint num2 = 94103444u;
			num2 ^= num2 << 21;
			num2 ^= num2 >> 3;
			num2 ^= num2 << 4;
			num2 ^= 0x76326C4D;
			byte[] bytes = BitConverter.GetBytes(num2);
			byte[] bytes2 = BitConverter.GetBytes(dateTime.ToBinary());
			byte[] bytes3 = BitConverter.GetBytes(dateTime2.ToBinary());
			byte[] value = Amare(Encoding.UTF8.GetBytes(s));
			uint[] array = new uint[6]
			{
				BitConverter.ToUInt32(bytes, 0),
				BitConverter.ToUInt32(bytes2, 0),
				BitConverter.ToUInt32(bytes2, 4),
				BitConverter.ToUInt32(bytes3, 0),
				BitConverter.ToUInt32(bytes3, 4),
				BitConverter.ToUInt32(value, 0)
			};
			Quia(array);
			byte[] array2 = new byte[24];
			Buffer.BlockCopy(BitConverter.GetBytes(array[0]), 0, array2, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(array[1]), 0, array2, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(array[2]), 0, array2, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(array[3]), 0, array2, 12, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(array[4]), 0, array2, 16, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(array[5]), 0, array2, 20, 4);
			return Convert.ToBase64String(array2);
		}

		private static string Eius()
		{
			if (_appString == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getPackageManager", new object[0]);
				string text = androidJavaObject.Call<string>("getPackageName", new object[0]);
				stringBuilder.AppendLine(text);
				AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getPackageInfo", new object[2] { text, 64 });
				AndroidJavaObject[] array = androidJavaObject3.Get<AndroidJavaObject[]>("signatures");
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.AppendLine(array[i].Call<string>("toCharsString", new object[0]));
					stringBuilder.AppendLine(array[i].Call<int>("hashCode", new object[0]).ToString("X"));
				}
				_appString = stringBuilder.ToString();
			}
			return _appString;
		}

		private static byte[] Amare(byte[] input)
		{
			uint num = 2054879403u;
			num ^= num << 21;
			num ^= num >> 3;
			num ^= num << 4;
			num ^= 0x4D6323AF;
			uint num2 = 1537734556u;
			num2 ^= num2 << 21;
			num2 ^= num2 >> 3;
			num2 ^= num2 << 4;
			num2 ^= 0x6F13606E;
			uint num3 = 1u;
			uint num4 = 0u;
			for (int i = 0; i < input.Length; i++)
			{
				num3 = (num3 + input[i]) % num;
				num4 = (num4 + num3) % num2;
			}
			uint num5 = num3 | (num4 << 16);
			num5 ^= num5 << 21;
			num5 ^= num5 >> 3;
			num5 ^= num5 << 4;
			return BitConverter.GetBytes(num5);
		}

		private static void Quia(uint[] words)
		{
			uint num = 11428847u;
			num ^= num << 21;
			num ^= num >> 3;
			num ^= num << 4;
			num ^= 0xE02999C2u;
			uint num2 = 1279794293u;
			num2 ^= num2 << 21;
			num2 ^= num2 >> 3;
			num2 ^= num2 << 4;
			num2 ^= 0x85D060CCu;
			uint num3 = 2235988143u;
			num3 ^= num3 << 21;
			num3 ^= num3 >> 3;
			num3 ^= num3 << 4;
			num3 ^= 0x85585C1Du;
			uint num4 = 467813562u;
			num4 ^= num4 << 21;
			num4 ^= num4 >> 3;
			num4 ^= num4 << 4;
			num4 ^= 0xFE68256Du;
			uint[] array = new uint[4] { num, num2, num3, num4 };
			uint num5 = 2051834548u;
			num5 ^= num5 << 21;
			num5 ^= num5 >> 3;
			num5 ^= num5 << 4;
			num5 ^= 0x598D1945;
			uint num6 = 3840752286u;
			num6 ^= num6 << 21;
			num6 ^= num6 >> 3;
			num6 ^= num6 << 4;
			num6 ^= 0xAB69A144u;
			uint[] array2 = new uint[2] { num5, num6 };
			uint num7 = 1004814659u;
			num7 ^= num7 << 21;
			num7 ^= num7 >> 3;
			num7 ^= num7 << 4;
			num7 ^= 0x908F2662u;
			uint num8 = num7;
			for (int i = 0; i < words.Length; i += 2)
			{
				uint num9 = 0u;
				uint num10 = 32u;
				words[i] ^= array2[0];
				words[i + 1] ^= array2[1];
				while (num10-- != 0)
				{
					words[i] += (((words[i + 1] << 4) ^ (words[i + 1] >> 5)) + words[i + 1]) ^ (num9 + array[num9 & 3]);
					num9 += num8;
					words[i + 1] += (((words[i] << 4) ^ (words[i] >> 5)) + words[i]) ^ (num9 + array[(num9 >> 11) & 3]);
				}
				array2[0] = words[i];
				array2[1] = words[i + 1];
			}
		}
	}
}
