namespace SYBO.Components.Analytics
{
	public class AsyncRes
	{
		public volatile bool IsReady;

		public bool ExceptionOccured;

		public int ChunkLength;

		public string UncompressedMD5Hash;

		public object Result;
	}
}
