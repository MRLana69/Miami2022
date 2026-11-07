using System;
using UnityEngine;

namespace InAppPurchase.Internal
{
	public class AndroidPurchaseProviderCallbackListener : MonoBehaviour
	{
		private Action _onInitSuccessful;

		private Action _onInitFailed;

		private Action<string> _onProductRequestSuccessful;

		private Action _onProductRequestFailed;

		private Action<string> _onPurchasePurchased;

		private Action<string> _onPurchaseRestored;

		private Action<string> _onPurchaseRefunded;

		private Action<string> _onPurchaseFailed;

		private Action<string> _onRestoreFinished;

		private Action<string> _onConsumeFinished;

		public void Setup(Action onInitSuccessful, Action onInitFailed, Action<string> onProductRequestSuccessful, Action onProductRequestFailed, Action<string> onPurchasePurchased, Action<string> onPurchaseRestored, Action<string> onPurchaseRefunded, Action<string> onPurchaseFailed, Action<string> onRestoreFinished, Action<string> onConsumeFinished)
		{
			_onInitSuccessful = onInitSuccessful;
			_onInitFailed = onInitFailed;
			_onProductRequestSuccessful = onProductRequestSuccessful;
			_onProductRequestFailed = onProductRequestFailed;
			_onPurchasePurchased = onPurchasePurchased;
			_onPurchaseRefunded = onPurchaseRefunded;
			_onPurchaseRestored = onPurchaseRestored;
			_onPurchaseFailed = onPurchaseFailed;
			_onRestoreFinished = onRestoreFinished;
			_onConsumeFinished = onConsumeFinished;
		}

		private void AndroidPurchaseProvider_InitSuccessful(string notUsed)
		{
			FireEmptyEvent(_onInitSuccessful);
		}

		private void AndroidPurchaseProvider_InitFailed(string notUsed)
		{
			FireEmptyEvent(_onInitFailed);
		}

		private void AndroidPurchaseProvider_ProductRequestSuccessful(string message)
		{
			FireEventWithMessage(_onProductRequestSuccessful, message);
		}

		private void AndroidPurchaseProvider_ProductRequestFailed(string notUsed)
		{
			FireEmptyEvent(_onProductRequestFailed);
		}

		private void AndroidPurchaseProvider_PurchasePurchased(string message)
		{
			FireEventWithMessage(_onPurchasePurchased, message);
		}

		private void AndroidPurchaseProvider_PurchaseRestored(string message)
		{
			FireEventWithMessage(_onPurchaseRestored, message);
		}

		private void AndroidPurchaseProvider_PurchaseRefunded(string message)
		{
			FireEventWithMessage(_onPurchaseRefunded, message);
		}

		private void AndroidPurchaseProvider_PurchaseFailed(string message)
		{
			FireEventWithMessage(_onPurchaseFailed, message);
		}

		private void AndroidPurchaseProvider_RestoreFinished(string message)
		{
			FireEventWithMessage(_onRestoreFinished, message);
		}

		private void AndroidPurchaseProvider_ConsumeFinished(string message)
		{
			FireEventWithMessage(_onConsumeFinished, message);
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
