using System.Collections;
using System.Collections.Generic;

namespace Kiloo.Common.Backend.Cloud
{
	public interface ICloudUserData : IEnumerable<KeyValuePair<string, string>>, IEnumerable
	{
		string this[string key] { get; }
	}
}
