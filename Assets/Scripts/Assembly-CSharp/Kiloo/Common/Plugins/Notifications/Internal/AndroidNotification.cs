using System.Collections.Generic;
using Kiloo.Common.Serialization.Parsing;

namespace Kiloo.Common.Plugins.Notifications.Internal
{
	internal class AndroidNotification : INotification
	{
		private object _payload;

		public IDictionary<string, object> payload
		{
			get
			{
				Dictionary<string, object> dictionary = _payload as Dictionary<string, object>;
				if (dictionary == null)
				{
					string text = _payload as string;
					if (text != null)
					{
						_payload = KilooJson.Deserialize(text);
						dictionary = _payload as Dictionary<string, object>;
					}
				}
				return dictionary;
			}
		}

		public object this[string key]
		{
			get
			{
				IDictionary<string, object> dictionary = payload;
				object value;
				if (dictionary != null && dictionary.TryGetValue(key, out value))
				{
					return value;
				}
				return null;
			}
		}

		public string alert
		{
			get
			{
				return this["alert"] as string;
			}
		}

		public int badge
		{
			get
			{
				object obj = this["badge"];
				if (obj is long)
				{
					return (int)(long)obj;
				}
				return 0;
			}
		}

		public string sound
		{
			get
			{
				return this["sound"] as string;
			}
		}

		public AndroidNotification(string payload)
		{
			_payload = payload;
		}
	}
}
