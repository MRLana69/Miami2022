namespace Kiloo.Google.Internal
{
	public interface IGoogleConnectResult
	{
		string errorMessage { get; }

		int errorCode { get; }
	}
}
