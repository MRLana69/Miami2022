using System;
using Data.Save;
using Kiloo;

namespace Info.VideoRewardCooldowns
{
	public class CooldownManager
	{
		private static CooldownManager _instance;

		private const string OS_KEY = "video_reward_cooldowns";

		private const char OS_ENTRY_SEPARATOR = ',';

		private const char OS_COMPONENT_SEPARATOR = ':';

		private const int INFINITE_CHARGES_VALUE = -1;

		private OnlineSettings _onlineSettings;

		private Cooldowns _cooldowns;

		private static CooldownManager instance
		{
			get
			{
				return _instance ?? (_instance = new CooldownManager());
			}
		}

		public CooldownManager()
		{
			_onlineSettings = OnlineSettings.instance;
			_cooldowns = AdsSave.cooldowns;
			_onlineSettings.AddOnFinishedDownloadingOnlineSettingsHandler(LoadAndParseFromOnlineSettings);
			LoadAndParseFromOnlineSettings();
		}

		private void LoadAndParseFromOnlineSettings()
		{
			string valueString;
			if (_onlineSettings.TryGetValue("video_reward_cooldowns", out valueString))
			{
				string[] array = valueString.Split(',');
				string[] array2 = array;
				foreach (string text in array2)
				{
					Cooldown cooldown = Parse(text.Trim());
					_cooldowns.Add(cooldown);
				}
			}
		}

		private Cooldown Parse(string data)
		{
			string[] array = data.Split(':');
			Cooldown cooldown = new Cooldown();
			cooldown.id = array[0];
			cooldown.totalCharges = int.Parse(array[1]);
			cooldown.seconds = int.Parse(array[2]);
			return cooldown;
		}

		private static bool HasCharge(Cooldown cd)
		{
			if (cd.totalCharges == -1)
			{
				return true;
			}
			TimeSpan timeSpan = TimeSpan.FromSeconds(cd.seconds);
			if (!(CDateTime.UtcNow < cd.startTime + timeSpan))
			{
				cd.usedCharges = 0;
			}
			return cd.usedCharges < cd.totalCharges;
		}

		public static bool HasCharge(string id)
		{
			Cooldown cooldown = instance._cooldowns.Get(id);
			return cooldown != null && HasCharge(cooldown);
		}

		public static void Consume(string id)
		{
			Cooldown cooldown = instance._cooldowns.Get(id);
			if (cooldown != null && HasCharge(cooldown))
			{
				if (cooldown.usedCharges == 0)
				{
					cooldown.startTime = CDateTime.UtcNow;
				}
				cooldown.usedCharges++;
			}
		}

		public static int GetChargesUsed(string id)
		{
			Cooldown cooldown = instance._cooldowns.Get(id);
			return cooldown.usedCharges;
		}
	}
}
