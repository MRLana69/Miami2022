using Kiloo.Common.Utility;
using UnityEngine;

namespace UI
{
	public class TutorialIconAnimation : MonoBehaviour
	{
		public enum AnimationState
		{
			None = 0,
			Scaling = 1,
			Blinking = 2,
			Waiting = 3
		}

		private const float SCALE_DURATION = 1f;

		private const float WAIT_DURATION = 2f;

		private const float BLINK_DURATION = 1f;

		private readonly Vector3 _startScale = Vector3.zero;

		private readonly Vector3 _endScale = Vector3.one;

		[SerializeField]
		private UISprite _mainIcon;

		[SerializeField]
		private UISprite _blinkIcon;

		private string _id;

		private AnimationState _animationState;

		private float _currentAnimationTime;

		public void InitAnimation(string id)
		{
			_id = id;
			_mainIcon.alpha = 1f;
			_blinkIcon.alpha = 0f;
		}

		public void ShowIcon(bool show)
		{
			if (show)
			{
				_mainIcon.gameObject.SetActive(true);
				_blinkIcon.gameObject.SetActive(true);
				if (_animationState == AnimationState.None)
				{
					if (!PlayerPrefs.HasKey(_id + "hasScaled"))
					{
						SetAnimationState(AnimationState.Scaling);
						PlayerPrefs.SetInt(_id + "hasScaled", 1);
					}
					else
					{
						SetAnimationState(AnimationState.Blinking);
					}
				}
			}
			else
			{
				_mainIcon.gameObject.SetActive(false);
				_blinkIcon.gameObject.SetActive(false);
				_animationState = AnimationState.None;
			}
		}

		public void SetAnimationState(AnimationState state)
		{
			_animationState = state;
			_currentAnimationTime = 0f;
		}

		private void Update()
		{
			if (_animationState == AnimationState.None)
			{
				return;
			}
			if (_animationState == AnimationState.Scaling)
			{
				if (_currentAnimationTime < 1f)
				{
					float num = Mathf.Max(0f, Easings.ElasticEaseOut(_currentAnimationTime / 1f) * 2f - 1f);
					Vector3 localScale = Vector3.LerpUnclamped(_startScale, _endScale, num);
					_mainIcon.cachedTransform.localScale = localScale;
					float alpha = num - 1f;
					_blinkIcon.alpha = alpha;
					_currentAnimationTime += Time.deltaTime;
				}
				else
				{
					_mainIcon.cachedTransform.localScale = _endScale;
					_blinkIcon.alpha = 0f;
					_animationState = AnimationState.Waiting;
					_currentAnimationTime = 0f;
				}
			}
			if (_animationState == AnimationState.Blinking)
			{
				if (_currentAnimationTime < 1f)
				{
					float alpha2 = ((!(_currentAnimationTime < 0.5f)) ? Mathf.SmoothStep(2f, 0f, _currentAnimationTime / 1f) : Mathf.SmoothStep(0f, 2f, _currentAnimationTime / 1f));
					_blinkIcon.alpha = alpha2;
					_currentAnimationTime += Time.deltaTime;
				}
				else
				{
					_blinkIcon.alpha = 0f;
					_animationState = AnimationState.Waiting;
					_currentAnimationTime = 0f;
				}
			}
			if (_animationState == AnimationState.Waiting)
			{
				if (_currentAnimationTime < 2f)
				{
					_currentAnimationTime += Time.deltaTime;
					return;
				}
				_animationState = AnimationState.Blinking;
				_currentAnimationTime = 0f;
			}
		}
	}
}
