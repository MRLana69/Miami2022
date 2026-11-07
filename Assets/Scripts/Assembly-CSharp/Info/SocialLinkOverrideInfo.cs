using System.Collections.Generic;

namespace Info
{
	public class SocialLinkOverrideInfo
	{
		public readonly bool enabled = true;

		public readonly string link;

		public readonly Dictionary<string, string> title;

		public readonly Dictionary<string, string> buttonString;

		public readonly int reward = -1;
	}
}
