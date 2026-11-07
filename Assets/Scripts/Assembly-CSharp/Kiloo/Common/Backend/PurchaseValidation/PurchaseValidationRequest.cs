namespace Kiloo.Common.Backend.PurchaseValidation
{
	public class PurchaseValidationRequest
	{
		public string productId;

		public string receipt;

		public IPurchaseValidationResult result;
	}
}
