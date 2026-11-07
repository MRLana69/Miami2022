using System;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;
using Kiloo.Common.Backend;
using Kiloo.Common.Backend.PurchaseValidation;

namespace InAppPurchase.Internal
{
	internal static class PurchaseValidationExtensions
	{
		private class Result : IPurchaseValidationResult
		{
			public ReceiptStatus receiptStatus { get; internal set; }

			public string purchaseToken { get; internal set; }

			public Error error { get; internal set; }
		}

		private static bool _hasDetectedAnyInvalidPurchases;

		public static bool HasDetectedAnyInvalidPurchases(this IPurchaseValidationServer purchaseValidationServer)
		{
			return _hasDetectedAnyInvalidPurchases;
		}

		public static void ValidateTransaction(this IPurchaseValidationServer purchaseValidationServer, string productId, string transactionReceiptBase64, [NotNull] Action<IPurchaseValidationResult> completeHandler)
		{
			if (purchaseValidationServer != null)
			{
				PurchaseValidationRequest purchaseValidationRequest = new PurchaseValidationRequest
				{
					productId = productId,
					receipt = transactionReceiptBase64
				};
				if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(transactionReceiptBase64) || _hasDetectedAnyInvalidPurchases)
				{
					Result result = new Result();
					result.receiptStatus = ReceiptStatus.Invalid;
					result.purchaseToken = Convert.ToBase64String(Encoding.UTF8.GetBytes("Assumed invalid based on previous purchase or bad input"));
					result.error = null;
					Result obj = result;
					completeHandler(obj);
					return;
				}
				purchaseValidationServer.ValidatePurchase(purchaseValidationRequest, (PurchaseValidationRequest request) =>
				{
					if (request.result.error != null)
					{
					}
					if (request.result.receiptStatus == ReceiptStatus.Invalid)
					{
						_hasDetectedAnyInvalidPurchases = true;
					}
					completeHandler(purchaseValidationRequest.result);
					if (request.result.receiptStatus != ReceiptStatus.Valid)
					{
					}
				});
			}
			else
			{
				completeHandler(null);
			}
		}

		private static void LogError(string msg)
		{
			SDebug.LogError("[PurchaseValidationExtensions] " + msg);
		}

		[Conditional("ENABLE_PURCHASEVALIDATION_LOGS")]
		private static void Log(string msg)
		{
		}
	}
}
