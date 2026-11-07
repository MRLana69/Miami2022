using System;
using UnityEngine;

public class KindlePurchaseProviderCallbackListener : MonoBehaviour
{
	private Action<string> _onProductRequestSuccessful;

	private Action<string> _onProductRequestFailed;

	private Action _onRestoreFinished;

	private Action _onRestoreFailed;

	private Action _onRestoreFinishedNothingToRestore;

	private Action<string> _onRestoreSuccessful;

	private Action<string> _onPurchaseSuccessful;

	private Action<string> _onPurchaseFailedInvalidId;

	private Action<string> _onPurchaseFailed;

	private Action<string> _onPurchaseRefunded;

	public void Setup(Action<string> onProductRequestSuccessful, Action<string> onProductRequestFailed, Action onRestoreFinished, Action onRestoreFailed, Action onRestoreFinishedNothingToRestore, Action<string> onRestoreSuccessful, Action<string> onPurchaseSuccessful, Action<string> onPurchaseFailedInvalidId, Action<string> onPurchaseFailed, Action<string> onPurchaseRefunded)
	{
		_onProductRequestSuccessful = onProductRequestSuccessful;
		_onProductRequestFailed = onProductRequestFailed;
		_onRestoreFinished = onRestoreFinished;
		_onRestoreFailed = onRestoreFailed;
		_onRestoreFinishedNothingToRestore = onRestoreFinishedNothingToRestore;
		_onRestoreSuccessful = onRestoreSuccessful;
		_onPurchaseSuccessful = onPurchaseSuccessful;
		_onPurchaseFailedInvalidId = onPurchaseFailedInvalidId;
		_onPurchaseFailed = onPurchaseFailed;
		_onPurchaseRefunded = onPurchaseRefunded;
	}

	public void ProductRequestSuccessful(string message)
	{
		FireEventWithMessage(_onProductRequestSuccessful, message);
	}

	public void ProductRequestFailed(string message)
	{
		FireEventWithMessage(_onProductRequestFailed, message);
	}

	public void ProductRestoredSuccessfully(string message)
	{
		FireEventWithMessage(_onRestoreSuccessful, message);
	}

	public void ProductRestoreFinished(string empty)
	{
		FireEmptyEvent(_onRestoreFinished);
	}

	public void ProductRestoreFinishedButNothingHasBeenRestored(string empty)
	{
		FireEmptyEvent(_onRestoreFinishedNothingToRestore);
	}

	public void ProductRestoreFailed(string empty)
	{
		FireEmptyEvent(_onRestoreFailed);
	}

	public void ProductPurchasedSuccessfully(string message)
	{
		FireEventWithMessage(_onPurchaseSuccessful, message);
	}

	public void ProductPurchaseInvalidId(string message)
	{
		FireEventWithMessage(_onPurchaseFailedInvalidId, message);
	}

	public void ProductPurchaseFailed(string message)
	{
		FireEventWithMessage(_onPurchaseFailed, message);
	}

	public void ProductRefunded(string message)
	{
		FireEventWithMessage(_onPurchaseRefunded, message);
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
