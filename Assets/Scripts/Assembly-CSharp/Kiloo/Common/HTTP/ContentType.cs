using System;
using System.Collections.Generic;

namespace Kiloo.Common.HTTP
{
	public class ContentType
	{
		public readonly string type;

		public readonly string subType;

		private readonly IDictionary<string, string> _parameters;

		public IEnumerable<KeyValuePair<string, string>> parameters
		{
			get
			{
				return _parameters;
			}
		}

		public ContentType(string type, string subType, IDictionary<string, string> parameters)
		{
			this.type = type;
			this.subType = subType;
			_parameters = ((parameters == null) ? new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) : new Dictionary<string, string>(parameters, StringComparer.InvariantCultureIgnoreCase));
		}

		public string GetParameter(string name)
		{
			string value;
			if (_parameters.TryGetValue(name, out value))
			{
				return value;
			}
			return null;
		}
	}
}
