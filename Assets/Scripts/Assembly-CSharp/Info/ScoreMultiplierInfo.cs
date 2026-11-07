using System;
using System.Diagnostics;
using System.Threading;
using Data.Save;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace Info
{
	public static class ScoreMultiplierInfo
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onScoreMultiplierChanged__BackingField;

		public static int scoreMultiplier
		{
			get
			{
				int num = rawMultiplier;
				if (GameStats.Instance.CharacterDayEventActivated)
				{
					num += 5;
				}
				if (GameStats.Instance.ScoreBoosterActivated)
				{
					switch (GameStats.Instance.ScoreBoosterTier)
					{
					case 1:
						num += 5;
						break;
					case 2:
						num += 6;
						break;
					case 3:
						num += 7;
						break;
					default:
						num += 5;
						break;
					}
				}
				if (PlusMultiplier.plusMultiplierEnabled)
				{
					num += 10;
				}
				if (DoubleScoreMultiplier.doubleScoreActive)
				{
					num *= 2;
				}
				return num;
			}
		}

		public static int rawMultiplier
		{
			get
			{
				return Mathf.Clamp(PlayerSave.currentMissionSet + 1, 1, 30);
			}
		}

		public static event Action onScoreMultiplierChanged
		{
			add
			{
				Action action = onScoreMultiplierChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onScoreMultiplierChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onScoreMultiplierChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onScoreMultiplierChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static void TriggerOnScoreMultiplierChanged()
		{
			onScoreMultiplierChanged__BackingField.SafeInvoke();
		}
	}
}
