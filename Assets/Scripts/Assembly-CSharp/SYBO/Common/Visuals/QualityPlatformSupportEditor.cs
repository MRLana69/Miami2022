namespace SYBO.Common.Visuals
{
	public class QualityPlatformSupportEditor : IQualityPlatformSupport
	{
		public void Initialize()
		{
		}

		public bool IsSupported(QualityFeature feature)
		{
			return DeviceInfo.Instance.IsDeviceCapablePerformance(DeviceInfo.PerformanceLevel.High);
		}
	}
}
