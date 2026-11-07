using System.Diagnostics;
using System.Text;
using Kiloo.Common.Backend.PurchaseValidation;
using UnityEngine;

namespace InAppPurchase.Internal
{
	public class KindlePurchaseProvider : MonoBehaviour, IPurchaseProvider
	{
		private const string BRIDGE_DELEGATE_GAMEOBJECT_NAME = "KindleInAppCallback";

		private const string FINISHED_TRANSACTION_ID_PREFIX = "SHOULD_CONSUME_";

		private const string SEPARATOR_CHAR = ";";

		private static readonly char[] SEPARATOR_FOR_STRING_SPLIT = new char[1] { ';' };

		private PurchaseDelegate _purchaseDelegate;

		private ItemRestoredDelegate _itemRestoredDelegate;

		private ItemRefundedDelegate _itemRefundedDelegate;

		private ProductInfoRequestDelegate _productRequestDelegate;

		private RestorePurchaseFinishedDelegate _restorePurchaseFinishedDelegate;

		private AndroidJavaObject _iabInstance;

		private GameObject _callbackGameObject;

		public bool IsValidationServerSpecified
		{
			get
			{
				return false;
			}
		}

		public GameObject CreateCallbackGameObject()
		{
			GameObject gameObject = new GameObject("KindleInAppCallback");
			Object.DontDestroyOnLoad(gameObject);
			KindlePurchaseProviderCallbackListener kindlePurchaseProviderCallbackListener = gameObject.AddComponent<KindlePurchaseProviderCallbackListener>();
			kindlePurchaseProviderCallbackListener.Setup(ProductRequestSuccessful, ProductRequestFailed, ProductRestoreFinished, ProductRestoreFailed, ProductRestoreFinishedButNothingHasBeenRestored, ProductRestoredSuccessfully, ProductPurchasedSuccessfully, ProductPurchaseInvalidId, ProductPurchaseFailed, ProductRefunded);
			return gameObject;
		}

		public void Init(IInAppManager inAppManager, IPurchaseValidationServer purchaseValidationServer)
		{
			_purchaseDelegate = inAppManager.PurchaseRequestCallback;
			_productRequestDelegate = inAppManager.ProductInfoRequestCallback;
			_itemRestoredDelegate = inAppManager.ItemRestoredCallback;
			_itemRefundedDelegate = inAppManager.ItemRefundedCallback;
			_restorePurchaseFinishedDelegate = inAppManager.RestoreRequestFinishedCallback;
			if (_callbackGameObject == null)
			{
				_callbackGameObject = CreateCallbackGameObject();
			}
			if (_iabInstance == null)
			{
				_iabInstance = new AndroidJavaObject("com.kiloo.kindleinapppurchase.KindleManager");
			}
			_iabInstance.Set("debugLogsEnabled", false);
			inAppManager.InitFinishedCallback(true);
		}

		public bool CanMakePurchases()
		{
			return _iabInstance.Call<bool>("isIabPossible", new object[0]);
		}

		public void RequestProductInfo(string[] productIds)
		{
			_iabInstance.Call<string>("QueryProductData", new object[1] { string.Join(";", productIds) });
		}

		public bool RestoreNonConsumablePurchases(string[] productIds)
		{
			_iabInstance.Call<string>("InitiateRestore", new object[0]);
			return true;
		}

		public bool StartPurchase(string productId)
		{
			return StartPurchase(productId, null);
		}

		public bool StartPurchase(string productId, string cookie)
		{
			_iabInstance.Call<string>("InitiatePurchase", new object[1] { productId });
			return true;
		}

		public void FinishPurchase(string productId, string transactionId, string purchaseToken)
		{
			_iabInstance.Call("NotifyPurchaseFulfillment", transactionId, true);
		}

		public void ProductRequestSuccessful(string message)
		{
			ProductRequestResultInfo[] productInfo;
			if (TryParseProductInfo(message, out productInfo))
			{
				StringBuilder stringBuilder = new StringBuilder("KindlePurchaseProvider ProductInfo:\n");
				ProductRequestResultInfo[] array = productInfo;
				for (int i = 0; i < array.Length; i++)
				{
					ProductRequestResultInfo productRequestResultInfo = array[i];
					stringBuilder.AppendLine("    productId: " + productRequestResultInfo.productId + "  localized price: " + productRequestResultInfo.localizedPrice);
				}
				ProductInfoRequestDelegate productRequestDelegate = _productRequestDelegate;
				if (productRequestDelegate != null)
				{
					productRequestDelegate(true, productInfo);
				}
			}
			else
			{
				LogError("KindlePurchaseProvider ERROR: Malformed message from native code: " + message);
			}
		}

		public void ProductRequestFailed(string message)
		{
			ProductInfoRequestDelegate productRequestDelegate = _productRequestDelegate;
			if (productRequestDelegate != null)
			{
				productRequestDelegate(false, new ProductRequestResultInfo[0]);
			}
		}

		public void ProductRestoredSuccessfully(string message)
		{
			HandlePurchaseEnded(message, InternalPurchaseResult.Restored);
		}

		public void ProductRestoreFinished()
		{
			RestorePurchaseFinishedDelegate restorePurchaseFinishedDelegate = _restorePurchaseFinishedDelegate;
			if (restorePurchaseFinishedDelegate != null)
			{
				restorePurchaseFinishedDelegate(RestoreFinishedResult.Completed);
			}
		}

		public void ProductRestoreFailed()
		{
			RestorePurchaseFinishedDelegate restorePurchaseFinishedDelegate = _restorePurchaseFinishedDelegate;
			if (restorePurchaseFinishedDelegate != null)
			{
				restorePurchaseFinishedDelegate(RestoreFinishedResult.Failed);
			}
		}

		public void ProductRestoreFinishedButNothingHasBeenRestored()
		{
			RestorePurchaseFinishedDelegate restorePurchaseFinishedDelegate = _restorePurchaseFinishedDelegate;
			if (restorePurchaseFinishedDelegate != null)
			{
				restorePurchaseFinishedDelegate(RestoreFinishedResult.NothingToRestore);
			}
		}

		public void ProductPurchasedSuccessfully(string message)
		{
			HandlePurchaseEnded(message, InternalPurchaseResult.Completed);
		}

		public void ProductPurchaseInvalidId(string message)
		{
			HandlePurchaseEnded(message, InternalPurchaseResult.ItemUnavailable);
		}

		public void ProductPurchaseFailed(string message)
		{
			HandlePurchaseEnded(message, InternalPurchaseResult.Canceled);
		}

		public void ProductRefunded(string message)
		{
			HandlePurchaseEnded(message, InternalPurchaseResult.Refunded);
		}

		private void ValidateTransaction(string productId, string transactionId, string transactionRecieptBase64, InternalPurchaseResult internalPurchaseResult)
		{
		}

		private void HandlePurchaseEnded(string message, InternalPurchaseResult internalPurchaseResult)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			string productId;
			string transactionId;
			if (TryParsePurchaseEndedMessage(message, out productId, out transactionId))
			{
				switch (internalPurchaseResult)
				{
				case InternalPurchaseResult.Restored:
					HandleRestore(productId, transactionId);
					break;
				case InternalPurchaseResult.Refunded:
					HandleRefund(productId, transactionId, empty2);
					break;
				default:
					HandleDefaultCase(productId, transactionId, empty, empty2, internalPurchaseResult);
					break;
				}
			}
			else
			{
				LogError("KindlePurchaseProvider ERROR: Malformed message from native code: " + message);
			}
		}

		private void HandleRestore(string productId, string transactionId)
		{
			ItemRestoredDelegate itemRestoredDelegate = _itemRestoredDelegate;
			if (itemRestoredDelegate != null)
			{
				itemRestoredDelegate(productId, transactionId, null);
			}
		}

		private void HandleRefund(string productId, string transactionId, string cookie)
		{
			ItemRefundedDelegate itemRefundedDelegate = _itemRefundedDelegate;
			if (itemRefundedDelegate != null)
			{
				itemRefundedDelegate(productId, transactionId, cookie, null);
			}
		}

		private void HandleDefaultCase(string productId, string transactionId, string transactionRecieptBase64, string cookie, InternalPurchaseResult internalPurchaseResult)
		{
			PurchaseDelegate purchaseDelegate = _purchaseDelegate;
			if (purchaseDelegate != null)
			{
				purchaseDelegate(productId, transactionId, transactionRecieptBase64, cookie, internalPurchaseResult.ToPurchaseResult(), null);
			}
		}

		private bool TryParseProductInfo(string message, out ProductRequestResultInfo[] productInfo)
		{
			string[] array = message.Split(SEPARATOR_FOR_STRING_SPLIT);
			int result;
			if (array.Length >= 1 && int.TryParse(array[0], out result))
			{
				int num = 1 + result * 4;
				if (array.Length == num)
				{
					ProductRequestResultInfo[] array2 = new ProductRequestResultInfo[result];
					for (int i = 0; i < result; i++)
					{
						int num2 = 1 + i * 4;
						string productId = array[num2];
						string localizedPrice = array[num2 + 1];
						string title = array[num2 + 2];
						string description = array[num2 + 3];
						array2[i].Init(productId, localizedPrice, title, description);
					}
					productInfo = array2;
					return true;
				}
			}
			productInfo = new ProductRequestResultInfo[0];
			return false;
		}

		private bool TryParsePurchaseEndedMessage(string message, out string productId, out string transactionId)
		{
			string[] array = message.Split(SEPARATOR_FOR_STRING_SPLIT);
			if (array.Length == 2)
			{
				productId = array[0];
				transactionId = array[1];
				return true;
			}
			productId = string.Empty;
			transactionId = string.Empty;
			return false;
		}

		private static void LogError(string msg)
		{
			SDebug.LogError("[KindlePurchaseProvider] " + msg);
		}

		private static void LogWarning(string msg)
		{
		}

		[Conditional("IAP_DEBUG_LOGS")]
		private static void Log(string msg)
		{
		}
	}
}
