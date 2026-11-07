using System;

namespace SYBO.Components.Analytics
{
	public interface IPostAnalytics
	{
		string BaseURL { get; set; }

		void PostAnalyticsEvents(byte[] analyticsJsonBytes, int uncompressedLength, string uncompressedMD5Hash, Action<ProcessAnalyticsEventsResponse> onFinished);
	}
}
