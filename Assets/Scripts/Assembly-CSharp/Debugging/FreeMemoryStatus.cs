using Kiloo.Common.Plugins.DeviceUtility;
using UnityEngine;

namespace Debugging
{
	public class FreeMemoryStatus : MonoBehaviour
	{
		public float UpdateInterval = 0.5f;

		private UILabel _label;

		private float _timeleft;

		private static FreeMemoryStatus _instance;

		private GameObject _prefabInstanceRoot;

		public static FreeMemoryStatus Instance
		{
			get
			{
				return _instance;
			}
		}

		private void Awake()
		{
			_instance = this;
			_label = GetComponent<UILabel>();
			_timeleft = UpdateInterval;
		}

		private void Update()
		{
			_timeleft -= Time.deltaTime;
			if ((double)_timeleft <= 0.0)
			{
				int deviceFreeMemory = DeviceUtility.GetDeviceFreeMemory();
				Color color = ((deviceFreeMemory >= 100) ? Color.green : ((deviceFreeMemory <= 50) ? Color.yellow : Color.red));
				_label.text = string.Format("{0} MB Free", deviceFreeMemory);
				_label.color = color;
				_timeleft = UpdateInterval;
			}
		}

		public static FreeMemoryStatus Create(GameObject anchor)
		{
			GameObject prefab = Resources.Load("Prefabs/Special/FreeMemoryStatus", typeof(GameObject)) as GameObject;
			GameObject gameObject = anchor.AddChild(prefab);
			FreeMemoryStatus componentInChildren = gameObject.GetComponentInChildren<FreeMemoryStatus>();
			componentInChildren._prefabInstanceRoot = gameObject;
			return componentInChildren;
		}

		public void ToggleVisible(bool visible)
		{
			_prefabInstanceRoot.gameObject.SetActive(visible);
		}
	}
}
