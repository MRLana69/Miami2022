using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace UI.Popups.WelcomeBackRewardPopupInternal
{
	public abstract class WelcomeBackRewardPopupBox : MonoBehaviour
	{
		protected const string ITEM_FORMAT = "{0} {1}";

		[SerializeField]
		protected WelcomeBackRewardItemButton _welcomeBackRewardItemButtonPrefab;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action OnItemGiven__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnItemButtonClicked OnButtonClicked__BackingField;

		public event Action OnItemGiven
		{
			add
			{
				Action action = OnItemGiven__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnItemGiven__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnItemGiven__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnItemGiven__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event OnItemButtonClicked OnButtonClicked
		{
			add
			{
				OnItemButtonClicked onItemButtonClicked = OnButtonClicked__BackingField;
				OnItemButtonClicked onItemButtonClicked2;
				do
				{
					onItemButtonClicked2 = onItemButtonClicked;
					onItemButtonClicked = Interlocked.CompareExchange(ref OnButtonClicked__BackingField, (OnItemButtonClicked)Delegate.Combine(onItemButtonClicked2, value), onItemButtonClicked);
				}
				while ((object)onItemButtonClicked != onItemButtonClicked2);
			}
			remove
			{
				OnItemButtonClicked onItemButtonClicked = OnButtonClicked__BackingField;
				OnItemButtonClicked onItemButtonClicked2;
				do
				{
					onItemButtonClicked2 = onItemButtonClicked;
					onItemButtonClicked = Interlocked.CompareExchange(ref OnButtonClicked__BackingField, (OnItemButtonClicked)Delegate.Remove(onItemButtonClicked2, value), onItemButtonClicked);
				}
				while ((object)onItemButtonClicked != onItemButtonClicked2);
			}
		}

		protected WelcomeBackRewardItemButton SpawnButton(WelcomeBackRewardItemButton welcomeBackRewardItemButtonPrefab, Transform parent)
		{
			WelcomeBackRewardItemButton welcomeBackRewardItemButton = UnityEngine.Object.Instantiate(welcomeBackRewardItemButtonPrefab, parent);
			welcomeBackRewardItemButton.transform.localPosition = Vector3.zero;
			return welcomeBackRewardItemButton;
		}

		protected string GetDescriptionOfItem(WelcomeBackRewardItemPayload welcomeBackRewardItemPayload)
		{
			return GetDescriptionOfItem(welcomeBackRewardItemPayload.itemType, welcomeBackRewardItemPayload.multipliedItemAmount);
		}

		protected string GetDescriptionOfItem(CelebrationRewardType itemType, int amount)
		{
			return string.Format("{0} {1}", amount.ToString(), itemType.ToString());
		}

		protected void InvokeOnItemGiven()
		{
			OnItemGiven__BackingField.SafeInvoke();
		}

		protected void InvokeOnButtonClicked(WelcomeBackRewardItemPayload welcomeBackRewardItemPayload)
		{
			if (OnButtonClicked__BackingField != null)
			{
				OnButtonClicked__BackingField(welcomeBackRewardItemPayload);
			}
		}
	}
}
