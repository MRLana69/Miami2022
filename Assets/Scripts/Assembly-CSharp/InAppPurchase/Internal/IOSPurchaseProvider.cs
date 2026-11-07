using System;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;
using Kiloo.Common.Backend.PurchaseValidation;
using UnityEngine;

namespace InAppPurchase.Internal
{
	public class IOSPurchaseProvider : IPurchaseProvider
	{
		private const string BRIDGE_DELEGATE_GAMEOBJECT_NAME = "IOSPurchaseBridge_callback";

		private const char SEPARATOR_CHAR = ';';

		private static readonly char[] SEPARATOR_FOR_STRING_SPLIT = new char[1] { ';' };

		private IInAppManager _inAppManager;

		private PurchaseDelegate _purchaseDelegate;

		private ItemRestoredDelegate _itemRestoredDelegate;

		private CheckedItemRestoredDelegate _checkedItemRestoredDelegate;

		private ProductInfoRequestDelegate _productRequestDelegate;

		private RestorePurchaseFinishedDelegate _restorePurchaseFinishedDelegate;

		private IPurchaseValidationServer _purchaseValidationServer;

		private GameObject _callbackGameObject;

		public bool IsValidationServerSpecified
		{
			get
			{
				return _purchaseValidationServer != null;
			}
		}

		public void Init(IInAppManager inAppManager, IPurchaseValidationServer purchaseValidationServer)
		{
			_inAppManager = inAppManager;
			_purchaseDelegate = inAppManager.PurchaseRequestCallback;
			_productRequestDelegate = inAppManager.ProductInfoRequestCallback;
			_itemRestoredDelegate = inAppManager.ItemRestoredCallback;
			_checkedItemRestoredDelegate = inAppManager.CheckedItemRestoredCallback;
			_restorePurchaseFinishedDelegate = inAppManager.RestoreRequestFinishedCallback;
			_purchaseValidationServer = purchaseValidationServer;
			IOSPurchaseBridge_EnableDebugLogs("false");
			if (_callbackGameObject == null)
			{
				_callbackGameObject = new GameObject("IOSPurchaseBridge_callback");
				UnityEngine.Object.DontDestroyOnLoad(_callbackGameObject);
				IOSPurchaseProviderCallbackListener iOSPurchaseProviderCallbackListener = _callbackGameObject.AddComponent<IOSPurchaseProviderCallbackListener>();
				iOSPurchaseProviderCallbackListener.Setup(ProductRequestSuccessful, ProductRequestFailed, PurchasePurchased, PurchaseRestored, PurchaseFailed, RestoreFinished);
			}
			IOSPurchaseBridge_Init();
			_inAppManager.InitFinishedCallback(true);
		}

		public bool CanMakePurchases()
		{
			return IOSPurchaseBridge_CanMakePayments();
		}

		public void RequestProductInfo(string[] productIds)
		{
			IOSPurchaseBridge_RunProductsRequest(ArrayToString(productIds));
		}

		public bool StartPurchase(string productId)
		{
			return StartPurchase(productId, null);
		}

		public bool StartPurchase(string productId, string cookie)
		{
			if (string.IsNullOrEmpty(cookie))
			{
				LogWarning("Cookies for the purchase is not supported for iOS.");
			}
			return IOSPurchaseBridge_StartPurchase(productId);
		}

		public void FinishPurchase(string productId, string transactionId, string purchaseToken)
		{
			IOSPurchaseBridge_FinishTransaction(productId, transactionId);
		}

		public bool RestoreNonConsumablePurchases(string[] productIds)
		{
			return IOSPurchaseBridge_RestoreNonConsumablePurchases();
		}

		private string ArrayToString(string[] array)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i]);
				if (i < array.Length - 1)
				{
					stringBuilder.Append(';');
				}
			}
			return stringBuilder.ToString();
		}

		private void HandlePurchaseEnded(string message, InternalPurchaseResult internalPurchaseResult)
		{
			string productId;
			string transactionId;
			string transactionRecieptBase;
			if (TryParsePurchaseEndedMessage(message, out productId, out transactionId, out transactionRecieptBase))
			{
				switch (internalPurchaseResult)
				{
				case InternalPurchaseResult.Completed:
					HandlePurchase(productId, transactionId, transactionRecieptBase, internalPurchaseResult);
					return;
				case InternalPurchaseResult.Restored:
					HandleRestore(productId, transactionId, transactionRecieptBase);
					return;
				}
				PurchaseDelegate purchaseDelegate = _purchaseDelegate;
				if (purchaseDelegate != null)
				{
					purchaseDelegate(productId, transactionId, transactionRecieptBase, null, internalPurchaseResult.ToPurchaseResult(), null);
				}
			}
			else
			{
				LogError("IOSPurchaseProvider ERROR: Malformed message from native code: " + message);
			}
		}

		private void HandlePurchase(string productId, string transactionId, string transactionRecieptBase64, InternalPurchaseResult internalPurchaseResult)
		{
			ValidateTransaction(productId, transactionRecieptBase64, (IPurchaseValidationResult validationResult) =>
			{
				PurchaseDelegate purchaseDelegate = _purchaseDelegate;
				if (purchaseDelegate != null)
				{
					purchaseDelegate(productId, transactionId, transactionRecieptBase64, null, internalPurchaseResult.ToPurchaseResult(), validationResult);
				}
			});
		}

		private void HandleRestore(string productId, string transactionId, string transactionRecieptBase64)
		{
			InvokeItemRestore(productId, transactionId);
			InvokeCheckedItemRestore(productId, transactionId, transactionRecieptBase64);
		}

		private void InvokeItemRestore(string productId, string transactionId)
		{
			ItemRestoredDelegate itemRestoredDelegate = _itemRestoredDelegate;
			if (itemRestoredDelegate != null)
			{
				itemRestoredDelegate(productId, transactionId, null);
			}
		}

		private void InvokeCheckedItemRestore(string productId, string transactionId, string transactionRecieptBase64)
		{
			CheckedItemRestoredDelegate handler = _checkedItemRestoredDelegate;
			if (handler != null && _purchaseValidationServer != null)
			{
				ValidateTransaction(productId, transactionRecieptBase64, (IPurchaseValidationResult validationResult) =>
				{
					handler(productId, transactionId, null, validationResult);
				});
			}
		}

		private void ValidateTransaction(string productId, string transactionReciptBase64, [NotNull] Action<IPurchaseValidationResult> completeHandler)
		{
			if (_purchaseValidationServer != null)
			{
				_purchaseValidationServer.ValidateTransaction(productId, transactionReciptBase64, completeHandler);
			}
		}

		private bool TryParsePurchaseEndedMessage(string message, out string productId, out string transactionId, out string transactionRecieptBase64)
		{
			string[] array = message.Split(SEPARATOR_FOR_STRING_SPLIT);
			if (array.Length == 3)
			{
				productId = array[0];
				transactionId = array[1];
				transactionRecieptBase64 = array[2];
				return true;
			}
			productId = string.Empty;
			transactionId = string.Empty;
			transactionRecieptBase64 = string.Empty;
			return false;
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

		private InternalPurchaseResult GetProperPurchaseResult(string errorCode)
		{
			int result;
			if (int.TryParse(errorCode, out result))
			{
				switch (result)
				{
				case 0:
					return InternalPurchaseResult.Completed;
				case 1:
					return InternalPurchaseResult.Failed;
				case 2:
					return InternalPurchaseResult.Canceled;
				case 3:
					return InternalPurchaseResult.AlreadyOwned;
				case 4:
					return InternalPurchaseResult.ItemUnavailable;
				default:
					return InternalPurchaseResult.Failed;
				}
			}
			return InternalPurchaseResult.Failed;
		}

		private void ProductRequestSuccessful(string message)
		{
			ProductRequestResultInfo[] productInfo;
			if (TryParseProductInfo(message, out productInfo))
			{
				StringBuilder stringBuilder = new StringBuilder("IOSPurchaseProvider ProductInfo:\n");
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
				LogError("IOSPurchaseProvider ERROR: Malformed message from native code: " + message);
			}
		}

		private void ProductRequestFailed()
		{
			ProductInfoRequestDelegate productRequestDelegate = _productRequestDelegate;
			if (productRequestDelegate != null)
			{
				productRequestDelegate(false, new ProductRequestResultInfo[0]);
			}
		}

		private void PurchasePurchased(string message)
		{
			HandlePurchaseEnded(message, InternalPurchaseResult.Completed);
		}

		private void PurchaseRestored(string message)
		{
			HandlePurchaseEnded(message, InternalPurchaseResult.Restored);
		}

		private void PurchaseFailed(string message)
		{
			string[] array = message.Split(SEPARATOR_FOR_STRING_SPLIT);
			InternalPurchaseResult internalPurchaseResult = InternalPurchaseResult.Failed;
			if (array.Length == 4)
			{
				internalPurchaseResult = GetProperPurchaseResult(array[3]);
				message = array[0] + ';' + array[1] + ';' + array[2];
			}
			HandlePurchaseEnded(message, internalPurchaseResult);
		}

		private void RestoreFinished(string message)
		{
			int result;
			int.TryParse(message, out result);
			RestoreFinishedResult result2;
			switch (result)
			{
			case 0:
				result2 = RestoreFinishedResult.Failed;
				break;
			case 1:
				result2 = RestoreFinishedResult.NothingToRestore;
				break;
			case 2:
				result2 = RestoreFinishedResult.Completed;
				break;
			case 3:
				result2 = RestoreFinishedResult.Canceled;
				break;
			default:
				result2 = RestoreFinishedResult.Failed;
				break;
			}
			RestorePurchaseFinishedDelegate restorePurchaseFinishedDelegate = _restorePurchaseFinishedDelegate;
			if (restorePurchaseFinishedDelegate != null)
			{
				restorePurchaseFinishedDelegate(result2);
			}
		}

		private static void LogError(string msg)
		{
			SDebug.LogError("[IOSPurchaseProvider] " + msg);
		}

		private static void LogWarning(string msg)
		{
		}

		[Conditional("IAP_DEBUG_LOGS")]
		private static void Log(string msg)
		{
		}

		private static void IOSPurchaseBridge_Init()
		{
		}

		private static bool IOSPurchaseBridge_CanMakePayments()
		{
			return false;
		}

		private static void IOSPurchaseBridge_RunProductsRequest(string productIdsList)
		{
		}

		private static bool IOSPurchaseBridge_StartPurchase(string productId)
		{
			return false;
		}

		private static void IOSPurchaseBridge_FinishTransaction(string productId, string transactionId)
		{
		}

		private static bool IOSPurchaseBridge_RestoreNonConsumablePurchases()
		{
			return false;
		}

		private static bool IOSPurchaseBridge_EnableDebugLogs(string enabledBoolString)
		{
			return false;
		}
	}
}
