namespace InAppPurchase
{
	public struct ProductRequestResultInfo
	{
		public string productId;

		public string localizedPrice;

		public string title;

		public string description;

		public void Init(string productId, string localizedPrice, string title, string description)
		{
			this.productId = productId;
			this.localizedPrice = localizedPrice;
			this.title = title;
			this.description = description;
		}
	}
}
