using SYBO.Common.Managers;
using SYBO.Components.Utilities;
using UnityEngine;

namespace Debugging
{
	[RequireComponent(typeof(UILabel))]
	public class FPSCounter : MonoBehaviour
	{
		public UILabel OptionalMinFPSLabel;

		public float UpdateInterval = 0.5f;

		private UILabel _label;

		private float accum;

		private int frames;

		private float timeleft;

		private float maxFrame;

		private bool _visible;

		private static FPSCounter _instance;

		private GameObject _prefabInstanceRoot;

		public bool Visible
		{
			get
			{
				return _visible;
			}
		}

		public static FPSCounter Instance
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
			timeleft = UpdateInterval;
		}

		private void Update()
		{
			if (!_visible)
			{
				return;
			}
			timeleft -= Time.deltaTime;
			accum += SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale / Time.deltaTime;
			frames++;
			maxFrame = Mathf.Max(maxFrame, Time.deltaTime);
			if ((double)timeleft <= 0.0)
			{
				float num = accum / (float)frames;
				Color color = ((num >= 30f) ? Color.green : ((!(num > 10f)) ? Color.yellow : Color.red));
				_label.text = string.Format("{0:F1} FPS", num);
				_label.color = color;
				timeleft = UpdateInterval;
				accum = 0f;
				frames = 0;
				if (OptionalMinFPSLabel != null)
				{
					float num2 = 1f / maxFrame;
					Color color2 = ((num2 >= 30f) ? Color.green : ((!(num2 > 10f)) ? Color.yellow : Color.red));
					OptionalMinFPSLabel.text = string.Format("{0:F1} MIN", num2);
					OptionalMinFPSLabel.color = color2;
					maxFrame = 0f;
				}
			}
		}

		public static FPSCounter Create(GameObject anchor)
		{
			GameObject prefab = Resources.Load("Prefabs/Special/FpsCounter", typeof(GameObject)) as GameObject;
			GameObject gameObject = anchor.AddChild(prefab);
			FPSCounter componentInChildren = gameObject.GetComponentInChildren<FPSCounter>();
			componentInChildren._prefabInstanceRoot = gameObject;
			return componentInChildren;
		}

		public void ToggleVisible(bool visible)
		{
			_visible = visible;
			_prefabInstanceRoot.gameObject.SetActive(visible);
		}
	}
}
