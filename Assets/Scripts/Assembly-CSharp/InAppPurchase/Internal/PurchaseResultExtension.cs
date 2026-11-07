namespace InAppPurchase.Internal
{
	public static class PurchaseResultExtension
	{
		public static PurchaseResult ToPurchaseResult(this InternalPurchaseResult internalPurchaseResult)
		{
			PurchaseResult result = PurchaseResult.Failed;
			switch (internalPurchaseResult)
			{
			case InternalPurchaseResult.Completed:
				result = PurchaseResult.Completed;
				break;
			case InternalPurchaseResult.Failed:
				result = PurchaseResult.Failed;
				break;
			case InternalPurchaseResult.Canceled:
				result = PurchaseResult.Canceled;
				break;
			case InternalPurchaseResult.AlreadyOwned:
				result = PurchaseResult.AlreadyOwned;
				break;
			case InternalPurchaseResult.ItemUnavailable:
				result = PurchaseResult.ItemUnavailable;
				break;
			}
			return result;
		}
	}
}
