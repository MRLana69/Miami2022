using System.IO;
using Kiloo.Common.Utility;

namespace Kiloo.Common.Backend.Cloud
{
	public static class CloudItemExtensions
	{
		public static byte[] GetBytes(this ICloudItem item)
		{
			using (Stream stream = item.GetDataStream())
			{
				if (stream != null)
				{
					return stream.ReadAllBytes();
				}
			}
			return null;
		}

		public static int WriteToStream(this ICloudItem item, Stream stream)
		{
			using (Stream stream2 = item.GetDataStream())
			{
				return GjStreamUtil.CopyCount(stream2, stream);
			}
		}
	}
}
