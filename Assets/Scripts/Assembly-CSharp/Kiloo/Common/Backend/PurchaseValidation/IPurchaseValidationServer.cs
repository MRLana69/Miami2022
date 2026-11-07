using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Kiloo.Common.Backend.PurchaseValidation
{
	public interface IPurchaseValidationServer
	{
		Coroutine ValidatePurchase([NotNull] PurchaseValidationRequest purchaseValidationRequest, [CanBeNull] Action<PurchaseValidationRequest> completeHandler = null);
	}
}
