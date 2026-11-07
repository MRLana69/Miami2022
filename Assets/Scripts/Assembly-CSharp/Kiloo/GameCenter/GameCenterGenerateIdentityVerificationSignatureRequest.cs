namespace Kiloo.GameCenter
{
	public class GameCenterGenerateIdentityVerificationSignatureRequest : GameCenterRequest
	{
		public string publicKeyUrl;

		public byte[] signature;

		public byte[] salt;

		public ulong timestamp;
	}
}
