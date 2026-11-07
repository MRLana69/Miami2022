namespace SYBO.Common.Benchmarking
{
	public class FPSReportEntry
	{
		public float AvgFPS;

		public float MinFrameTime;

		public float MaxFrameTime;

		public FPSReportEntry(float avgFPS, float minFrameTime, float maxFrameTime)
		{
			AvgFPS = avgFPS;
			MinFrameTime = minFrameTime;
			MaxFrameTime = maxFrameTime;
		}
	}
}
