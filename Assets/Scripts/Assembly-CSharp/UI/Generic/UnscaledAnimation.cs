using System.Collections;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace UI.Generic
{
	public class UnscaledAnimation : MonoBehaviour
	{
		[SerializeField]
		private bool _playAutomatically;

		[SerializeField]
		private bool _loop;

		[SerializeField]
		private AnimationClip _clip;

		private IEnumerator _coroutine;

		private Animation _animation;

		public AnimationClip clip
		{
			get
			{
				return _clip;
			}
			set
			{
				_clip = value;
			}
		}

		public bool playAutomatically
		{
			get
			{
				return _playAutomatically;
			}
			set
			{
				_playAutomatically = value;
			}
		}

		public bool loop
		{
			get
			{
				return _loop;
			}
			set
			{
				_loop = value;
			}
		}

		public void OnEnable()
		{
			if (_playAutomatically)
			{
				StartAnimating();
			}
		}

		public void OnDisable()
		{
			StopAnimating();
		}

		public void StartAnimating(float initialPercentageSkip = 0f)
		{
			if (_coroutine == null)
			{
				if (_clip == null)
				{
					SDebug.LogError("AnimationClip was not set, unable to play animation!", base.gameObject);
					return;
				}
				SetupAnimationIfNeeded();
				_coroutine = Animation(initialPercentageSkip);
				StartCoroutine(_coroutine);
			}
		}

		public void StopAnimating()
		{
			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
			}
			_coroutine = null;
		}

		private void SetupAnimationIfNeeded()
		{
			if (_animation == null)
			{
				_animation = base.gameObject.GetOrAdd<Animation>();
				_animation.clip = _clip;
				_animation.playAutomatically = false;
				_animation.wrapMode = WrapMode.Default;
				_animation.AddClip(_clip, _clip.name);
			}
		}

		private IEnumerator Animation(float initialPercentageSkip = 0f)
		{
			AnimationClip animClip = _animation.GetClip(_clip.name);
			float maxTime = animClip.length;
			float time = maxTime * initialPercentageSkip;
			while (true)
			{
				if (time < maxTime)
				{
					animClip.SampleAnimation(_animation.gameObject, time);
					yield return null;
					time += Time.unscaledDeltaTime;
					continue;
				}
				time = 0f;
				if (_loop)
				{
					continue;
				}
				break;
			}
		}
	}
}
