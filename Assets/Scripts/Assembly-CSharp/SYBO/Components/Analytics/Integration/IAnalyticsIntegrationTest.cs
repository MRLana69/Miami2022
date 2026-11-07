namespace SYBO.Components.Analytics.Integration
{
	public interface IAnalyticsIntegrationTest
	{
		AnalyticsManager Manager { get; }

		void SimulateOnlineSettingsDownload();

		void GenerateFakeSessionEvents(int count);
	}
}
