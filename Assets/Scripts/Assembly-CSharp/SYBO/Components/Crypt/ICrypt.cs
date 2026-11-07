namespace SYBO.Components.Crypt
{
	public interface ICrypt
	{
		string Encrypt(string unencrypted);

		string Decrypt(string encrypted);

		byte[] Encrypt(byte[] buffer);

		byte[] Decrypt(byte[] buffer);
	}
}
