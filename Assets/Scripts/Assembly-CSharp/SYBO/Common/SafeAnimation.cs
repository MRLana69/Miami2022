using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace SYBO.Common
{
	public class SafeAnimation
	{
		public class SafeAnimationEventEventArgs : EventArgs
		{
			private string _message;

			public string Message
			{
				get
				{
					return _message;
				}
			}

			public SafeAnimationEventEventArgs(string message)
			{
				_message = message;
			}
		}

		private bool _loggingEnabled;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<SafeAnimationEventEventArgs> OnSafeAnimationEvent__BackingField;

		private Animation _animation;

		public Animation Animation
		{
			get
			{
				return _animation;
			}
			set
			{
				_animation = value;
			}
		}

		public event EventHandler<SafeAnimationEventEventArgs> OnSafeAnimationEvent
		{
			add
			{
				EventHandler<SafeAnimationEventEventArgs> eventHandler = OnSafeAnimationEvent__BackingField;
				EventHandler<SafeAnimationEventEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnSafeAnimationEvent__BackingField, (EventHandler<SafeAnimationEventEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<SafeAnimationEventEventArgs> eventHandler = OnSafeAnimationEvent__BackingField;
				EventHandler<SafeAnimationEventEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnSafeAnimationEvent__BackingField, (EventHandler<SafeAnimationEventEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public SafeAnimation(Animation animation, bool loggingEnabled)
		{
			_animation = animation;
			_loggingEnabled = loggingEnabled;
			DebugLog("SafeAnimation() animation=", animation);
		}

		public void DebugLog(string methodName, object message = null)
		{
			if (!_loggingEnabled)
			{
			}
		}

		public void SetSpeed(string animationClipName, float speed)
		{
			try
			{
				_animation[animationClipName].speed = speed;
			}
			catch (Exception)
			{
			}
		}

		public void SetSpeed(AnimationState animationState, float speed)
		{
			animationState.speed = speed;
		}

		public void SetRotation(Quaternion rotation)
		{
			_animation.transform.rotation = rotation;
		}

		public void SetLocalRotation(Quaternion rotation)
		{
			_animation.transform.localRotation = rotation;
		}

		public bool HasAnimationClip(string animationClipName)
		{
			return animationClipName != null && _animation.GetClip(animationClipName) != null;
		}

		public AnimationClip GetAnimationClip(string animationClipName)
		{
			return _animation.GetClip(animationClipName);
		}

		public AnimationState GetAnimationState(string animationStateName)
		{
			return _animation[animationStateName];
		}

		public bool HasAnimationState(string animationStateName)
		{
			return GetAnimationState(animationStateName) != null;
		}

		public void AddClip(AnimationClip animationClip, string newName)
		{
			_animation.AddClip(animationClip, newName);
		}

		public void CrossFadeQueued(string animationName)
		{
			_animation.CrossFadeQueued(animationName);
			DebugLog("CrossFadeQueued", animationName);
			RaiseOnSafeAnimationEvent("CrossFadeQueued", animationName);
		}

		public void CrossFadeQueued(string animationName, float fadeLength)
		{
			_animation.CrossFadeQueued(animationName, fadeLength);
			DebugLog("CrossFadeQueued", animationName + " , " + fadeLength);
			RaiseOnSafeAnimationEvent("CrossFadeQueued", animationName + " , " + fadeLength);
		}

		public void CrossFade(string animationName)
		{
			_animation.CrossFade(animationName);
			DebugLog("CrossFade", animationName);
			RaiseOnSafeAnimationEvent("CrossFade", animationName);
		}

		public void TryAddAnimationEvent(Animation animationInstance, string animationName, AnimationEvent animationEvent)
		{
			Globals.TryAddAnimationEvent(animationInstance, animationName, animationEvent);
		}

		public void CrossFade(string animationName, float fadeLength)
		{
			_animation.CrossFade(animationName, fadeLength);
			DebugLog("CrossFade", animationName + " , " + fadeLength);
			RaiseOnSafeAnimationEvent("CrossFade", animationName + " , " + fadeLength);
		}

		public void SetEnabled(string animationName, bool isEnabled)
		{
			_animation[animationName].enabled = isEnabled;
		}

		public void Stop()
		{
			_animation.Stop();
			DebugLog("Stop");
			RaiseOnSafeAnimationEvent("Stop");
		}

		public void PlayQueued(string animationName)
		{
			_animation.PlayQueued(animationName);
			DebugLog("PlayQueued", animationName);
			RaiseOnSafeAnimationEvent("PlayQueued", animationName);
		}

		public void Play(string animationName)
		{
			_animation.Play(animationName);
			DebugLog("Play", animationName + " , " + animationName);
			RaiseOnSafeAnimationEvent("Play", animationName + " , " + animationName);
		}

		public void Play()
		{
			_animation.Play();
			DebugLog("Play");
			RaiseOnSafeAnimationEvent("Play");
		}

		public bool IsPlaying(string animationName)
		{
			return _animation.IsPlaying(animationName);
		}

		public bool HasAnimation()
		{
			return _animation != null;
		}

		private void RaiseOnSafeAnimationEvent(string methodName, object message = null)
		{
			string message2 = methodName + "() " + message;
			if (OnSafeAnimationEvent__BackingField != null)
			{
				OnSafeAnimationEvent__BackingField(this, new SafeAnimationEventEventArgs(message2));
			}
		}
	}
}
