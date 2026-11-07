using UnityEngine;

namespace SYBO.Subway
{
	public class PerformanceLevelObjectDestroy : MonoBehaviour
	{
		[SerializeField]
		private DeviceInfo.PerformanceLevel _minDevicePerformance;

		private void Awake()
		{
			if (!DeviceInfo.Instance.IsDeviceCapablePerformance(_minDevicePerformance))
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
