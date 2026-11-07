namespace Kiloo.Common.Backend.PurchaseValidation
{
	public interface IPurchaseValidationResult
	{
		ReceiptStatus receiptStatus { get; }

		string purchaseToken { get; }

		Error error { get; }
	}
}
