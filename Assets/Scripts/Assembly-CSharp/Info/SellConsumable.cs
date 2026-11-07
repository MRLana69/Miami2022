using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;

namespace Info
{
	public static class SellConsumable
	{
		private const string ONLINESETTINGS_SELL_CONSUMABLES_DURING_RUN_KEY = "consumables_sell_during_run";

		private const bool ONLINESETTINGS_SELL_CONSUMABLES_DURING_RUN_DEFAULT = true;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<UpgradeType, int> completed__BackingField;

		public static bool allowSellConsumablesDuringRun
		{
			get
			{
				return OnlineSettings.instance.GetValue("consumables_sell_during_run", true);
			}
		}

		public static SellableConsumable activeConsumable { get; set; }

		public static event Action<UpgradeType, int> completed
		{
			add
			{
				Action<UpgradeType, int> action = completed__BackingField;
				Action<UpgradeType, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref completed__BackingField, (Action<UpgradeType, int>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<UpgradeType, int> action = completed__BackingField;
				Action<UpgradeType, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref completed__BackingField, (Action<UpgradeType, int>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static void Sell(SellableConsumable consumable)
		{
			activeConsumable = consumable;
			UIScreenController.Instance.QueuePopup("ConsumablesPopup");
		}

		public static void CompleteSelling(UpgradeType upgradeType, int amountPurchased)
		{
			completed__BackingField.SafeInvoke(upgradeType, amountPurchased);
		}
	}
}
