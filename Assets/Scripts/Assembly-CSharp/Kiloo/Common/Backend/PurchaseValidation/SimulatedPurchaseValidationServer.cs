using System;
using System.Text;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Kiloo.Common.Backend.PurchaseValidation
{
	public class SimulatedPurchaseValidationServer : IPurchaseValidationServer
	{
		private class Result : IPurchaseValidationResult
		{
			public ReceiptStatus receiptStatus { get; internal set; }

			public string purchaseToken { get; internal set; }

			public Error error { get; internal set; }
		}

		public Coroutine ValidatePurchase(PurchaseValidationRequest purchaseValidationRequest, Action<PurchaseValidationRequest> completeHandler)
		{
			return Wait.For(0.5f).UnscaledSeconds().ThenInvoke(() =>
			{
				purchaseValidationRequest.result = new Result
				{
					receiptStatus = ReceiptStatus.Valid,
					purchaseToken = Convert.ToBase64String(Encoding.UTF8.GetBytes("I'm a simulated purchase token")),
					error = null
				};
				if (completeHandler != null)
				{
					completeHandler(purchaseValidationRequest);
				}
			})
				.StartCoroutine();
		}
	}
}
