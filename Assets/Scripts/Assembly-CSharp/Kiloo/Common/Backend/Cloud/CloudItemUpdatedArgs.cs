using System;

namespace Kiloo.Common.Backend.Cloud
{
	public class CloudItemUpdatedArgs : EventArgs
	{
		public string file { get; private set; }

		public ICloudItem item { get; private set; }

		public CloudItemUpdatedArgs(ICloudItem item, string file)
		{
			this.item = item;
			this.file = file;
		}
	}
}
