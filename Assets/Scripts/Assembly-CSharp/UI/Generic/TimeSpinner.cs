using System.Collections;
using Kiloo.Common.Utility;
using UnityEngine;

namespace UI.Generic
{
	[RequireComponent(typeof(UISprite))]
	public class TimeSpinner : MonoBehaviour
	{
		private enum SpinState
		{
			Zero = 0,
			Three = 1,
			Six = 2,
			Nine = 3,
			Twelve = 4,
			Fifteen = 5,
			Eighteen = 6,
			TwentyOne = 7
		}

		[SerializeField]
		private bool _animate = true;

		private UISprite _sprite;

		private Coroutine _cachedCoroutine;

		private bool _coroutineRunning;

		private SpinState _currentState;

		private void Awake()
		{
			_sprite = GetComponent<UISprite>();
		}

		private void OnEnable()
		{
			if (_animate)
			{
				Animate(true);
			}
		}

		private void OnDisable()
		{
			_coroutineRunning = false;
		}

		public void Animate(bool animateNow)
		{
			_animate = animateNow;
			if (_cachedCoroutine != null && !_animate)
			{
				StopCoroutine(_cachedCoroutine);
				_coroutineRunning = false;
			}
			if (_animate && base.isActiveAndEnabled && !_coroutineRunning)
			{
				_cachedCoroutine = StartCoroutine(Animate());
			}
		}

		private IEnumerator Animate()
		{
			float animationDuration = 0.5f;
			while (_animate)
			{
				_coroutineRunning = true;
				switch (_currentState)
				{
				case SpinState.Zero:
					_sprite.invert = true;
					yield return AdvanceToTarget(0.25f, animationDuration);
					_currentState = SpinState.Three;
					break;
				case SpinState.Three:
					_sprite.invert = true;
					yield return AdvanceToTarget(0.5f, animationDuration);
					_currentState = SpinState.Six;
					break;
				case SpinState.Six:
					yield return AdvanceToTarget(0.75f, animationDuration);
					_currentState = SpinState.Nine;
					break;
				case SpinState.Nine:
					_sprite.invert = true;
					yield return AdvanceToTarget(1f, animationDuration);
					_currentState = SpinState.Twelve;
					break;
				case SpinState.Twelve:
					_sprite.invert = false;
					yield return AdvanceToTarget(0.75f, animationDuration);
					_currentState = SpinState.Fifteen;
					break;
				case SpinState.Fifteen:
					_sprite.invert = false;
					yield return AdvanceToTarget(0.5f, animationDuration);
					_currentState = SpinState.Eighteen;
					break;
				case SpinState.Eighteen:
					yield return AdvanceToTarget(0.25f, animationDuration);
					_currentState = SpinState.TwentyOne;
					break;
				case SpinState.TwentyOne:
					_sprite.invert = false;
					yield return AdvanceToTarget(0f, animationDuration);
					_currentState = SpinState.Zero;
					break;
				}
			}
			yield return null;
			_coroutineRunning = false;
		}

		private IEnumerator AdvanceToTarget(float target, float duration)
		{
			float currentDuration = 0f;
			float startFillAmount = _sprite.fillAmount;
			while (currentDuration < duration)
			{
				currentDuration += Time.unscaledDeltaTime;
				_sprite.fillAmount = Mathf.LerpUnclamped(startFillAmount, target, Easings.QuinticEaseOut(currentDuration / duration));
				yield return null;
			}
			while (currentDuration < 1f)
			{
				currentDuration += Time.unscaledDeltaTime;
				yield return null;
			}
		}
	}
}
