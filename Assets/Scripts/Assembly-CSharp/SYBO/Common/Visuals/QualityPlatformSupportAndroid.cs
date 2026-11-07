using System.Collections.Generic;

namespace SYBO.Common.Visuals
{
	public class QualityPlatformSupportAndroid : IQualityPlatformSupport
	{
		private enum AndroidQualityCategory
		{
			HighPerformanceDevice = 0
		}

		private Dictionary<QualityFeature, AndroidQualityCategory> _qualityFeatureCategory = new Dictionary<QualityFeature, AndroidQualityCategory> { 
		{
			QualityFeature.EnvironmentFX,
			AndroidQualityCategory.HighPerformanceDevice
		} };

		private Dictionary<AndroidQualityCategory, bool> _qualityCategorySupport = new Dictionary<AndroidQualityCategory, bool>();

		public void Initialize()
		{
			_qualityCategorySupport.Add(AndroidQualityCategory.HighPerformanceDevice, DeviceInfo.Instance.IsDeviceCapablePerformance(DeviceInfo.PerformanceLevel.High));
			foreach (AndroidQualityCategory key in _qualityCategorySupport.Keys)
			{
			}
		}

		public bool IsSupported(QualityFeature feature)
		{
			AndroidQualityCategory value;
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
