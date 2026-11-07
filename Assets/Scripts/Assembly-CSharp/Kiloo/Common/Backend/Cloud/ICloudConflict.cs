namespace Kiloo.Common.Backend.Cloud
{
	public interface ICloudConflict
	{
		string name { get; }

		ICloudItem mine { get; }

		ICloudItem theirs { get; }

		ICloudItem resolution { get; }

		CloudConflictResolution resolveMethod { get; }

		void ResolveUsingMine();

		void ResolveUsingTheirs();

		void ResolveWith(byte[] data, ICloudUserData userData);
	}
}
