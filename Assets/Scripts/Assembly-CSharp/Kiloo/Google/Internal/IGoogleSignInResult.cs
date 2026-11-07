namespace Kiloo.Google.Internal
{
	public interface IGoogleSignInResult
	{
		IGoogleProfile profile { get; }

		string errorMessage { get; }

		int errorCode { get; }
	}
}
