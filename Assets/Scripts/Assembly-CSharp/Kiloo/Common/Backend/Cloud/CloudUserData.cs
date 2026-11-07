using System;
using System.Collections;
using System.Collections.Generic;

namespace Kiloo.Common.Backend.Cloud
{
	public class CloudUserData : ICloudUserData, IEnumerable<KeyValuePair<string, string>>, IEnumerable
	{
		private readonly IDictionary<string, string> _dict;

		public string this[string key]
		{
			get
			{
				return _dict[key];
			}
		}

		public CloudUserData()
		{
			_dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}

		public CloudUserData(IDictionary<string, string> dict)
		{
			_dict = new Dictionary<string, string>(dict, StringComparer.OrdinalIgnoreCase);
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return _dict.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_dict).GetEnumerator();
		}

		public void Add(string key, string value)
		{
			_dict.Add(key, value);
		}
	}
}
