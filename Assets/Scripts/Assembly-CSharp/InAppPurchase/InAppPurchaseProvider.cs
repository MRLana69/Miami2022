using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using InAppPurchase.Internal;
using Kiloo.Common.Backend.PurchaseValidation;
using UnityEngine;

namespace InAppPurchase
{
	public class InAppPurchaseProvider : IPurchaseProvider, IInAppManager
	{
		private IPurchaseProvider _provider;

		private IAPPluginInitializationState _currentInitializationState;

		private IInAppManager _inAppManager;

		private Dictionary<string, ProductRequestResultInfo> _latestProductInfo = new Dictionary<string, ProductRequestResultInfo>();

		private static InAppPurchaseProvider _instance;

		public static InAppPurchaseProvider instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new InAppPurchaseProvider();
				}
				return _instance;
			}
		}

		public bool isInstanced
		{
			get
			{
				return _instance != null;
			}
		}

		public static bool IsValidationSupportedOnCurrentPlatform
		{
			get
			{
				return true;
			}
		}

		public bool IsValidationServerSpecified
		{
			get
			{
				return _provider != null && _provider.IsValidationServerSpecified;
			}
		}

		private InAppPurchaseProvider()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				_provider = new IOSPurchaseProvider();
				break;
			case RuntimePlatform.Android:
				_provider = new AndroidPurchaseProvider();
				break;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WebGLPlayer:
			case RuntimePlatform.WindowsEditor:
				_provider = new EditorPurchaseProvider();
				break;
			default:
				LogError("There's no provider for the current platform.");
				break;
			}
		}

		public ProductRequestResultInfo[] GetLatestRequestedProductInfo()
		{
			return (ProductRequestResultInfo[])_latestProductInfo.Values.ToArray().Clone();
		}

		public bool TryGetLatestRequestedProductInfoForProductId(string productId, out ProductRequestResultInfo productInfo)
		{
			foreach (ProductRequestResultInfo value in _latestProductInfo.Values)
			{
				if (value.productId.Equals(productId))
				{
					productInfo = value;
					return true;
				}
			}
			productInfo = default(ProductRequestResultInfo);
			return false;
		}

		public void Init(IInAppManager inAppManager, IPurchaseValidationServer purchaseValidationServer = null)
		{
			if (_provider != null && _currentInitializationState == IAPPluginInitializationState.NotInitialized)
			{
				_inAppManager = inAppManager;
				_currentInitializationState = IAPPluginInitializationState.Initializing;
				_provider.Init(this, purchaseValidationServer);
			}
			else if (_currentInitializationState != IAPPluginInitializationState.Initializing && _currentInitializationState != IAPPluginInitializationState.InitializedSuccessfully && _currentInitializationState != IAPPluginInitializationState.NotInitialized)
			{
			}
			if (_provider != null)
			{
				if (IsValidationSupportedOnCurrentPlatform && !_provider.IsValidationServerSpecified)
				{
					LogError("Expected to have validation server specified for current platform");
				}
				else if (!Application.isEditor && !IsValidationSupportedOnCurrentPlatform && _provider.IsValidationServerSpecified)
				{
					LogError("Expected to NOT have validation server specified for current platform. Update InAppPurchaseProvider constructor");
				}
			}
		}

		public bool CanMakePurchases()
		{
			if (_currentInitializationState != IAPPluginInitializationState.InitializedSuccessfully)
			{
				return false;
			}
			return _provider.CanMakePurchases();
		}

		public void RequestProductInfo(string[] productIds)
		{
			if (CanMakePurchases())
			{
				string[] productIds2 = productIds.Distinct().ToArray();
				_provider.RequestProductInfo(productIds2);
			}
		}

		public bool StartPurchase(string productId)
		{
			if (!CanMakePurchases())
			{
				_inAppManager.PurchaseRequestCallback(productId, string.Empty, string.Empty, string.Empty, PurchaseResult.Failed, null);
				return false;
			}
			return _provider.StartPurchase(productId, null);
		}

		public bool StartPurchase(string productId, string cookie)
		{
			if (!CanMakePurchases())
			{
				_inAppManager.PurchaseRequestCallback(productId, string.Empty, string.Empty, cookie, PurchaseResult.Failed, null);
				return false;
			}
			return _provider.StartPurchase(productId, cookie);
		}

		public bool RestoreNonConsumablePurchases(string[] productIds)
		{
			if (!CanMakePurchases())
			{
				_inAppManager.RestoreRequestFinishedCallback(RestoreFinishedResult.Failed);
				return false;
			}
			return _provider.RestoreNonConsumablePurchases(productIds);
		}

		public void FinishPurchase(string productId, string transactionId, string purchaseToken)
		{
			if (CanMakePurchases())
			{
				_provider.FinishPurchase(productId, transactionId, purchaseToken);
			}
		}

		public void InitFinishedCallback(bool success)
		{
			if (!success)
			{
				_currentInitializationState = IAPPluginInitializationState.NotInitialized;
			}
			else
			{
				_currentInitializationState = IAPPluginInitializationState.InitializedSuccessfully;
			}
			_inAppManager.InitFinishedCallback(success);
		}

		public string getInAppAPIKey()
		{
			return _inAppManager.getInAppAPIKey();
		}

		public IsIAPConsumableValue IsProductConsumable(string productID)
		{
			return _inAppManager.IsProductConsumable(productID);
		}

		public void ProductInfoRequestCallback(bool success, ProductRequestResultInfo[] productInfo)
		{
			if (success)
			{
				for (int i = 0; i < productInfo.Length; i++)
				{
					ProductRequestResultInfo value = productInfo[i];
					if (_latestProductInfo.ContainsKey(value.productId))
					{
						_latestProductInfo[value.productId] = value;
					}
					else
					{
						_latestProductInfo.Add(value.productId, value);
					}
				}
			}
			_inAppManager.ProductInfoRequestCallback(success, productInfo);
		}

		public void PurchaseRequestCallback(string productId, string transactionId, string transationIdBase64, string cookie, PurchaseResult result, IPurchaseValidationResult validationResult)
		{
			_inAppManager.PurchaseRequestCallback(productId, transactionId, transationIdBase64, cookie, result, validationResult);
		}

		public void ItemRestoredCallback(string productId, string transactionId, string cookie)
		{
			_inAppManager.ItemRestoredCallback(productId, transactionId, cookie);
		}

		public void CheckedItemRestoredCallback(string productId, string transactionId, string cookie, IPurchaseValidationResult validationResult)
		{
			_inAppManager.CheckedItemRestoredCallback(productId, transactionId, cookie, validationResult);
		}

		public void ItemRefundedCallback(string productid, string transactionid, string cookie, IPurchaseValidationResult validationResult)
		{
			_inAppManager.ItemRefundedCallback(productid, transactionid, cookie, validationResult);
		}

		public void RestoreRequestFinishedCallback(RestoreFinishedResult result)
		{
			_inAppManager.RestoreRequestFinishedCallback(result);
		}

		private static void LogError(string msg)
		{
			SDebug.LogError("[InAppPurchaseProvider] " + msg);
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
