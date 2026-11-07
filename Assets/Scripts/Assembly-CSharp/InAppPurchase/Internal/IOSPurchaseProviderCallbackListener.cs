using System;
using JetBrains.Annotations;
using UnityEngine;

namespace InAppPurchase.Internal
{
	public class IOSPurchaseProviderCallbackListener : MonoBehaviour
	{
		private Action<string> _onProductRequestSuccessful;

		private Action _onProductRequestFailed;

		private Action<string> _onPurchasePurchased;

		private Action<string> _onPurchaseRestored;

		private Action<string> _onPurchaseFailed;

		private Action<string> _onRestoreFinished;

		public void Setup(Action<string> onProductRequestSuccessful, Action onProductRequestFailed, Action<string> onPurchasePurchased, Action<string> onPurchaseRestored, Action<string> onPurchaseFailed, Action<string> onRestoreFinished)
		{
			_onProductRequestSuccessful = onProductRequestSuccessful;
			_onProductRequestFailed = onProductRequestFailed;
			_onPurchasePurchased = onPurchasePurchased;
			_onPurchaseRestored = onPurchaseRestored;
			_onPurchaseFailed = onPurchaseFailed;
			_onRestoreFinished = onRestoreFinished;
		}

		[UsedImplicitly]
		private void IOSPurchaseProvider_ProductRequestSuccessful(string message)
		{
			FireEventWithMessage(_onProductRequestSuccessful, message);
		}

		[UsedImplicitly]
		private void IOSPurchaseProvider_ProductRequestFailed()
		{
			FireEmptyEvent(_onProductRequestFailed);
		}

		[UsedImplicitly]
		private void IOSPurchaseProvider_PurchasePurchased(string message)
		{
			FireEventWithMessage(_onPurchasePurchased, message);
		}

		[UsedImplicitly]
		private void IOSPurchaseProvider_PurchaseRestored(string message)
		{
			FireEventWithMessage(_onPurchaseRestored, message);
		}

		[UsedImplicitly]
		private void IOSPurchaseProvider_PurchaseFailed(string message)
		{
			FireEventWithMessage(_onPurchaseFailed, message);
		}

		[UsedImplicitly]
		private void IOSPurchaseProvider_RestoreFinished(string message)
		{
			FireEventWithMessage(_onRestoreFinished, message);
		}

		private void FireEventWithMessage(Action<string> handler, string message)
		{
			if (handler != null)
			{
				handler(message);
			}
		}

		private void FireEmptyEvent(Action handler)
		{
			if (handler != null)
			{
				handler();
			}
		}
	}
}
