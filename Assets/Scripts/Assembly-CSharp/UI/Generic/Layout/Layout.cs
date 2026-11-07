using JetBrains.Annotations;
using Kiloo.Common.Timer;
using UnityEngine;

namespace UI.Generic.Layout
{
	public class Layout : MonoBehaviour
	{
		public enum AlignMode
		{
			Off = 0,
			OnEnable = 1,
			OnUpdate = 2,
			OnLateUpdate = 3
		}

		[HideInInspector]
		[UsedImplicitly]
		public bool autosizeWidget;

		[HideInInspector]
		[UsedImplicitly]
		public float autosizeWidgetMargin;

		[HideInInspector]
		[UsedImplicitly]
		public float autosizeWidgetMinWidth;

		[HideInInspector]
		[UsedImplicitly]
		public UIWidget targetWidget;

		[HideInInspector]
		[UsedImplicitly]
		public bool adjustWidgetPosition = true;

		[UsedImplicitly]
		public AlignMode mode;

		[UsedImplicitly]
		public int padding;

		[SerializeField]
		private bool _alignNow;

		[SerializeField]
		protected bool _snapToLabelContent;

		protected UIPanel _panel;

		protected float _calculatedHeight;

		protected float _calculatedWidth;

		public float calculatedHeight
		{
			get
			{
				return _calculatedHeight;
			}
		}

		public float calculateWidth
		{
			get
			{
				return _calculatedWidth;
			}
		}

		[UsedImplicitly]
		private void Start()
		{
			if (Application.isPlaying && mode != AlignMode.Off)
			{
				Align_Internal();
				switch (mode)
				{
				}
			}
		}

		[UsedImplicitly]
		private void OnEnable()
		{
			if (Application.isPlaying && mode == AlignMode.OnEnable)
			{
				Align_Internal();
			}
		}

		[UsedImplicitly]
		private void Update()
		{
			if ((Application.isPlaying && mode == AlignMode.OnUpdate) || (Application.isEditor && !Application.isPlaying))
			{
				Align_Internal();
			}
			if (_alignNow)
			{
				_alignNow = false;
				Align_Internal();
			}
		}

		[UsedImplicitly]
		private void LateUpdate()
		{
			if (Application.isPlaying && mode == AlignMode.OnLateUpdate)
			{
				Align_Internal();
			}
		}

		public void Align(bool once = false)
		{
			Align_Internal();
			if (!once)
			{
				Timer.ExecuteNextFrame(Align_Internal);
			}
		}

		public void SetToAlignModeForSpecifiedDuration(AlignMode alignMode, float duration)
		{
			mode = alignMode;
			Timer.ExecuteWithDelay(duration, () =>
			{
				mode = AlignMode.Off;
			});
		}

		protected virtual void Align_Internal()
		{
		}
	}
}
