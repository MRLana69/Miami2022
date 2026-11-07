namespace Kiloo.Google
{
	public class GoogleSignInPluginRequest
	{
		public bool silent { get; set; }

		public int errorCode { get; set; }

		public string errorMessage { get; set; }

		public IGoogleProfile profile { get; set; }
	}
}
