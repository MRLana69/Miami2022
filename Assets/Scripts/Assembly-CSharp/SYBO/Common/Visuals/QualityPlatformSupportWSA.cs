using System.Collections.Generic;

namespace SYBO.Common.Visuals
{
	public class QualityPlatformSupportWSA : IQualityPlatformSupport
	{
		private enum WSAQualityCategory
		{
			HighPerformanceDevice = 0
		}

		private Dictionary<QualityFeature, WSAQualityCategory> _qualityFeatureCategory = new Dictionary<QualityFeature, WSAQualityCategory> { 
		{
			QualityFeature.EnvironmentFX,
			WSAQualityCategory.HighPerformanceDevice
		} };

		private Dictionary<WSAQualityCategory, bool> _qualityCategorySupport = new Dictionary<WSAQualityCategory, bool>();

		public void Initialize()
		{
			_qualityCategorySupport.Add(WSAQualityCategory.HighPerformanceDevice, DeviceInfo.Instance.IsDeviceCapablePerformance(DeviceInfo.PerformanceLevel.High));
		}

		public bool IsSupported(QualityFeature feature)
		{
			WSAQualityCategory value;
			if (!_qualityFeatureCategory.TryGetValue(feature, out value))
			{
				return false;
			}
			bool value2 = false;
			if (!_qualityCategorySupport.TryGetValue(value, out value2))
			{
			}
			return value2;
		}
	}
}
