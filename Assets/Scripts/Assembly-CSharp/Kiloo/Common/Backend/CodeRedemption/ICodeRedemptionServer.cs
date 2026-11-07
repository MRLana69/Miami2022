using UnityEngine;

namespace Kiloo.Common.Backend.CodeRedemption
{
	internal interface ICodeRedemptionServer<out T>
	{
		Coroutine GetPayloadAsync(string code, GetPayloadCompleteHandler<T> handler);

		Coroutine PostPayoutConfirmationAsync(string code, PostPayoutConfirmationCompleteHandler handler);
	}
}
