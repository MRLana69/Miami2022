using UnityEngine;

namespace UI.Generic
{
	[RequireComponent(typeof(UIWidget))]
	public class WidgetAlphaAnimation : MonoBehaviour
	{
		[SerializeField]
		private AnimationCurve _curve;

		private float _progress;

		private UIWidget _widget;

		private float _fullCurveLength;

		private void Awake()
		{
			_widget = GetComponent<UIWidget>();
			_progress = Random.Range(0f, 1f);
			_fullCurveLength = _curve.keys[_curve.length - 1].time * 2f;
		}

		private void Update()
		{
			_progress += Time.unscaledDeltaTime;
			_widget.alpha = _curve.Evaluate(_progress);
			if (_progress >= _fullCurveLength)
			{
				_progress -= _fullCurveLength;
			}
		}
	}
}
