namespace Cloud.Internal
{
	public class CloudSaveComparisonRow
	{
		public CloudSaveComparisonElement id;

		public float localValue;

		public float onlineValue;

		public bool isLocalImprovement;

		public bool isOnlineImprovement;

		public CloudSaveComparisonRow(CloudSaveComparisonElement id, int localValue, int onlineValue)
		{
			this.id = id;
			this.localValue = localValue;
			this.onlineValue = onlineValue;
			isLocalImprovement = localValue > onlineValue;
			isOnlineImprovement = localValue < onlineValue;
		}

		public CloudSaveComparisonRow(CloudSaveComparisonElement id, float localValue, float onlineValue)
		{
			this.id = id;
			this.localValue = localValue;
			this.onlineValue = onlineValue;
			isLocalImprovement = localValue > onlineValue;
			isOnlineImprovement = localValue < onlineValue;
		}
	}
}
