using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Common.Visuals
{
	public class QualityPlatformSupportIOS : IQualityPlatformSupport
	{
		private enum IPhoneQualityCategory
		{
			HighPerformanceDevice = 0
		}

		private bool _isInitialized;

		private Dictionary<IPhoneQualityCategory, bool> _qualityCategorySupport = new Dictionary<IPhoneQualityCategory, bool>();

		private Dictionary<QualityFeature, IPhoneQualityCategory> _qualityFeatureCategory = new Dictionary<QualityFeature, IPhoneQualityCategory> { 
		{
			QualityFeature.EnvironmentFX,
			IPhoneQualityCategory.HighPerformanceDevice
		} };

		public void Initialize()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;
				_qualityCategorySupport.Add(IPhoneQualityCategory.HighPerformanceDevice, DeviceInfo.Instance.IsDeviceCapablePerformance(DeviceInfo.PerformanceLevel.High));
				bool flag = SystemInfo.operatingSystem.Contains("iPhone OS 7") || SystemInfo.operatingSystem.Contains("iOS 7");
				bool flag2 = SystemInfo.graphicsDeviceName.Contains("Apple A7");
				if (flag && !flag2)
				{
				}
			}
		}

		public bool IsSupported(QualityFeature feature)
		{
			Initialize();
			IPhoneQualityCategory value;
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
