using Kiloo.Common.Backend.PurchaseValidation;

namespace InAppPurchase
{
	public delegate void PurchaseDelegate(string productId, string transactionId, string transactionIdBase64, string cookie, PurchaseResult result, IPurchaseValidationResult validationResult);
}
