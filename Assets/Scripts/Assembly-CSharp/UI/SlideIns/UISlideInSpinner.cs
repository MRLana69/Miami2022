using UnityEngine;
using UnityEngine.Serialization;

namespace UI.SlideIns
{
	public class UISlideInSpinner : UISlideIn
	{
		[FormerlySerializedAs("messageLabel")]
		[SerializeField]
		private UILabel _messageLabel;

		[FormerlySerializedAs("spinner")]
		[SerializeField]
		private UISprite _spinner;

		private Transform _cachedSpinnerTransform;

		private readonly Vector3 _singleStepRotation = new Vector3(0f, 0f, -45f);

		private const float SINGLE_STEP_THRESHOLD = 0.1f;

		private int _currentStep;

		private float _currentDuration;

		private const int STEP_COUNT = 8;

		private void SetupSlidein(string message)
		{
			base.gameObject.SetActive(true);
			_messageLabel.text = message;
			SlideIn();
		}

		private void Update()
		{
			if (_cachedSpinnerTransform == null)
			{
				_cachedSpinnerTransform = _spinner.cachedTransform;
			}
			_currentDuration += Time.deltaTime;
			if (_currentDuration > 0.1f)
			{
				_currentStep++;
				_currentStep %= 8;
				Vector3 singleStepRotation = _singleStepRotation;
				_cachedSpinnerTransform.Rotate(singleStepRotation);
				_currentDuration -= 0.1f;
			}
		}
	}
}
