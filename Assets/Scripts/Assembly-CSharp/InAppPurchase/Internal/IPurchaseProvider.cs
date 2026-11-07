using Kiloo.Common.Backend.PurchaseValidation;

namespace InAppPurchase.Internal
{
	public interface IPurchaseProvider
	{
		bool IsValidationServerSpecified { get; }

		void Init(IInAppManager inApManager, IPurchaseValidationServer validationServer = null);

		bool CanMakePurchases();

		void RequestProductInfo(string[] productIds);

		bool StartPurchase(string productId);

		bool StartPurchase(string productId, string cookie);

		void FinishPurchase(string productId, string transactionId, string purchaseToken);

		bool RestoreNonConsumablePurchases(string[] productIds);
	}
}
