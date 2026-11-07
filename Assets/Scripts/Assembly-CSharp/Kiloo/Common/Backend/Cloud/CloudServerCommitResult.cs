using System.Collections.Generic;

namespace Kiloo.Common.Backend.Cloud
{
	public class CloudServerCommitResult : CloudServerResult
	{
		public IDictionary<string, CloudServerCommitInfo> commitInfo;
	}
}
