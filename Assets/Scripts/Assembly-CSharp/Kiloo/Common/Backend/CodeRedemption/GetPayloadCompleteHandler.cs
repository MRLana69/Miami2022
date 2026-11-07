using JetBrains.Annotations;

namespace Kiloo.Common.Backend.CodeRedemption
{
	public delegate void GetPayloadCompleteHandler<in T>([CanBeNull] T payload, [CanBeNull] string errorMessage);
}
