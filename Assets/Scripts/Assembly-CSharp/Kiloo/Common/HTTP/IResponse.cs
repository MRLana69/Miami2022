using JetBrains.Annotations;

namespace Kiloo.Common.HTTP
{
	public interface IResponse
	{
		int status { get; }

		[NotNull]
		string message { get; }

		[CanBeNull]
		byte[] bytes { get; }

		[NotNull]
		Headers headers { get; }

		ResponseCategory category { get; }
	}
}
