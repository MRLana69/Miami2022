using Kiloo.Common.Backend.PurchaseValidation;

namespace InAppPurchase
{
	public interface IInAppManager
	{
		void InitFinishedCallback(bool success);

		string getInAppAPIKey();

		IsIAPConsumableValue IsProductConsumable(string productID);

		void ProductInfoRequestCallback(bool success, ProductRequestResultInfo[] productInfo);

		void PurchaseRequestCallback(string productId, string transactionId, string transactionIdBase64, string cookie, PurchaseResult result, IPurchaseValidationResult validationResult);

		void ItemRestoredCallback(string productId, string transactionId, string cookie);

		void CheckedItemRestoredCallback(string productId, string transactionId, string cookie, IPurchaseValidationResult validationResult);

		void RestoreRequestFinishedCallback(RestoreFinishedResult result);

		void ItemRefundedCallback(string productid, string transactionid, string cookie, IPurchaseValidationResult validationResult);
	}
}
