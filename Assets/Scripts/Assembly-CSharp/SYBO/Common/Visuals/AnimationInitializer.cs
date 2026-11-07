using System;
using UnityEngine;

namespace SYBO.Common.Visuals
{
	[RequireComponent(typeof(Animation))]
	public class AnimationInitializer : MonoBehaviour
	{
		[Serializable]
		public struct Config
		{
			[Tooltip("Min value of randomized animation start time. Clamped based on default animation clip length.")]
			[SerializeField]
			public float StartTimeMin;

			[Tooltip("Max value of randomized animation start time. Clamped based on default animation clip length.")]
			[SerializeField]
			public float StartTimeMax;
		}

		[SerializeField]
		private Config _config;

		private Animation _animation;

		private bool _initialized;

		private void OnEnable()
		{
			Initialize();
			InitializeAnimation(_config);
		}

		private void OnValidate()
		{
			_animation = GetComponent<Animation>();
			_config = ClampToDefaultAnimationLength(_config, true);
		}

		private void Initialize()
		{
			if (!_initialized)
			{
				_animation = GetComponent<Animation>();
				_initialized = true;
			}
		}

		public void InitializeAnimation(Config config)
		{
			if (!(_animation == null) && !(_animation.clip == null))
			{
				_config = ClampToDefaultAnimationLength(config);
				float num = config.StartTimeMax - config.StartTimeMin;
				float value = config.StartTimeMin + num * UnityEngine.Random.value;
				AnimationState animationState = _animation[_animation.clip.name];
				value = Mathf.Clamp(value, 0f, animationState.length);
				_animation[animationState.clip.name].time = value;
			}
		}

		public Config ClampToDefaultAnimationLength(Config config, bool validating = false)
		{
			if (_animation == null || _animation.clip == null)
			{
				return config;
			}
			AnimationState animationState = _animation[_animation.clip.name];
			config.StartTimeMin = Mathf.Clamp(config.StartTimeMin, 0f, animationState.length);
			config.StartTimeMax = Mathf.Clamp(config.StartTimeMax, 0f, animationState.length);
			return config;
		}
	}
}
