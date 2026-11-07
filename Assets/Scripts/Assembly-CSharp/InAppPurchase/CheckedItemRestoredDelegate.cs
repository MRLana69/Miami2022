using Kiloo.Common.Backend.PurchaseValidation;

namespace InAppPurchase
{
	public delegate void CheckedItemRestoredDelegate(string productId, string transactionId, string cookie, IPurchaseValidationResult validationResult);
}
