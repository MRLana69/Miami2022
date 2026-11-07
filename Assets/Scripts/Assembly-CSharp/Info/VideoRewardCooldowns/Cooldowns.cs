using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;

namespace Info.VideoRewardCooldowns
{
	public class Cooldowns
	{
		public const string HOVERBOARD_ID = "hoverboard";

		public const string COINS_ID = "coins";

		public const string SCORE_BOOSTER_ID = "score_booster";

		public const string HEADSTART_ID = "headstart";

		public const string DOUBLE_COINS_VIDEO_ID = "doublecoinsvideo";

		private Dictionary<string, Cooldown> _defaultCooldowns = new Dictionary<string, Cooldown>
		{
			{
				"hoverboard",
				new Cooldown
				{
					id = "hoverboard",
					totalCharges = 2,
					seconds = 28800
				}
			},
			{
				"doublecoinsvideo",
				new Cooldown
				{
					id = "doublecoinsvideo",
					totalCharges = -1,
					seconds = 0
				}
			},
			{
				"coins",
				new Cooldown
				{
					id = "coins",
					totalCharges = 2,
					seconds = 28800
				}
			},
			{
				"score_booster",
				new Cooldown
				{
					id = "score_booster",
					totalCharges = 2,
					seconds = 28800
				}
			},
			{
				"headstart",
				new Cooldown
				{
					id = "headstart",
					totalCharges = 2,
					seconds = 28800
				}
			}
		};

		public Dictionary<string, Cooldown> activeCooldowns = new Dictionary<string, Cooldown>();

		public void Add(Cooldown cooldown)
		{
			if (activeCooldowns.ContainsKey(cooldown.id))
			{
				Cooldown cooldown2 = activeCooldowns[cooldown.id];
				cooldown2.totalCharges = cooldown.totalCharges;
				cooldown2.seconds = cooldown.seconds;
			}
			else
			{
				activeCooldowns[cooldown.id] = cooldown;
			}
		}

		[CanBeNull]
		public Cooldown Get(string id)
		{
			if (activeCooldowns.ContainsKey(id))
			{
				return activeCooldowns[id];
			}
			if (_defaultCooldowns.ContainsKey(id))
			{
				Cooldown cooldown = _defaultCooldowns[id];
				activeCooldowns.Add(cooldown.id, cooldown);
				return activeCooldowns[id];
			}
			SDebug.LogError("[Cooldown] Could not find: " + id);
			return null;
		}

		public string AsString()
		{
			return string.Join(", ", activeCooldowns.Values.Select((Cooldown x) => "{0}:{1}:{2}".Populate(x.id, x.totalCharges, x.seconds)).ToArray());
		}
	}
}
