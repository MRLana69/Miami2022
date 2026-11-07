using System;
using System.Collections;
using JetBrains.Annotations;
using Kiloo.Common.HTTP;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Kiloo.Common.Backend.PurchaseValidation
{
	public sealed class PurchaseValidationServer : IPurchaseValidationServer
	{
		[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
		private class ApiPurchaseValidationResponse : IPurchaseValidationResult
		{
			public ReceiptStatus receiptStatus { get; set; }

			public string purchaseToken { get; set; }

			public Error error { get; set; }
		}

		[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
		private class ApiPurchaseValidationRequest
		{
			public string productId { get; set; }

			public string receipt { get; set; }
		}

		private const string API_POST_VALIDATE_PURCHASE = "api/v1/purchasevalidation";

		private readonly IJsonSerializer _jsonSerializer;

		[NotNull]
		public Uri baseUri { get; set; }

		public bool debug { get; set; }

		public float timeout { get; set; }

		public PurchaseValidationServer([NotNull] Uri baseUri, [NotNull] IJsonSerializer jsonSerializer, bool debug = false, float timeout = 0f)
		{
			this.baseUri = baseUri;
			this.debug = debug;
			this.timeout = timeout;
			_jsonSerializer = jsonSerializer;
		}

		public Coroutine ValidatePurchase(PurchaseValidationRequest purchaseValidationRequest, Action<PurchaseValidationRequest> completeHandler)
		{
			return CoroutineUtil.StartCoroutine(ValidatePurchaseCoroutine(purchaseValidationRequest, completeHandler));
		}

		private IEnumerator ValidatePurchaseCoroutine(PurchaseValidationRequest purchaseValidationRequest, Action<PurchaseValidationRequest> completeHandler)
		{
			purchaseValidationRequest.result = null;
			IRequest request = RequestExtensions.WithJsonContent(jsonData: new ApiPurchaseValidationRequest
			{
				productId = purchaseValidationRequest.productId,
				receipt = purchaseValidationRequest.receipt
			}, request: KilooWebClient.CreatePost(new Uri(baseUri, "api/v1/purchasevalidation")).WithTimeout(timeout), jsonSerializer: _jsonSerializer);
			if (debug)
			{
				request.headers.Set("Kiloo-Debug", "true");
			}
			yield return request.SendAsync();
			ApiPurchaseValidationResponse purchaseValidationResponse;
			if (request.response != null && request.response.TryParseJson<ApiPurchaseValidationResponse>(out purchaseValidationResponse, _jsonSerializer))
			{
				purchaseValidationRequest.result = purchaseValidationResponse;
			}
			else
			{
				purchaseValidationRequest.result = new ApiPurchaseValidationResponse
				{
					error = new Error
					{
						category = ErrorCategory.HTTP.ToString(),
						code = ((request.response != null) ? request.response.status : 0),
						message = "Purchase validation request failed: " + request.statusMessage
					}
				};
			}
			if (completeHandler != null)
			{
				completeHandler(purchaseValidationRequest);
			}
		}
	}
}
