using System;

namespace Kiloo.Common.Ads
{
	public class AdProviderInfo : IComparable<AdProviderInfo>
	{
		public float weight { get; set; }

		public int priority { get; set; }

		public string providerID { get; set; }

		public string providerTier { get; set; }

		public int CompareTo(AdProviderInfo other)
		{
			if (other == null)
			{
				return 1;
			}
			if (providerTier.Equals(other.providerTier))
			{
				return 0;
			}
			if (priority == other.priority)
			{
				return weight.CompareTo(other.weight);
			}
			return priority.CompareTo(other.priority);
		}
	}
}
