using System;
using System.IO;

namespace Kiloo.Common.Backend.Cloud
{
	public interface ICloudItem
	{
		string name { get; }

		string revision { get; }

		DateTime? lastModified { get; }

		bool isModified { get; }

		bool isOutOfDate { get; }

		string sourceDescription { get; }

		string sourceInstallationId { get; }

		int dataVersion { get; }

		ICloudUserData userData { get; }

		Stream GetDataStream();
	}
}
