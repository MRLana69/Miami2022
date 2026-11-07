using Kiloo.Common.Backend.PurchaseValidation;

namespace InAppPurchase
{
	public delegate void ItemRefundedDelegate(string productId, string transactionId, string cookie, IPurchaseValidationResult validationResult);
}
