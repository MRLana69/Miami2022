using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OutfitKeyframeBehaviour : MonoBehaviour
{
	private bool _initialized;

	private Animation _targetAnimation;

	public ParticleSystem[] TargetObjects;

	public List<KeyFrameAction> Actions;

	private void Start()
	{
		if (!_initialized)
		{
			Initialize();
		}
	}

	private void OnDisable()
	{
		if (_targetAnimation != null)
		{
			_targetAnimation.gameObject.GetComponent<AnimationEventBroadcaster>().SendDoKeyframeAnimation -= DoKeyframeAnimation;
		}
	}

	private void Initialize()
	{
		_initialized = true;
		_targetAnimation = GetAnimationInAncestors(base.transform);
		_targetAnimation.gameObject.GetComponent<AnimationEventBroadcaster>().SendDoKeyframeAnimation += DoKeyframeAnimation;
		int num = 0;
		foreach (KeyFrameAction action in Actions)
		{
			if (_targetAnimation[action.clip] != null)
			{
				AnimationEvent animationEvent = new AnimationEvent();
				animationEvent.messageOptions = SendMessageOptions.RequireReceiver;
				if (_targetAnimation[action.clip] != null)
				{
					animationEvent.time = (float)action.KeyFrame / _targetAnimation[action.clip].clip.frameRate;
				}
				animationEvent.intParameter = num;
				animationEvent.functionName = "DoKeyframeAnimation";
				Globals.TryAddAnimationEvent(_targetAnimation, action.clip, animationEvent);
			}
			num++;
		}
	}

	private Animation GetAnimationInAncestors(Transform transform)
	{
		Animation component = transform.GetComponent<Animation>();
		if (component != null)
		{
			return component;
		}
		Transform parent = transform.parent;
		if (parent != null)
		{
			return GetAnimationInAncestors(parent);
		}
		return null;
	}

	private void DoKeyframeAnimation(int soundIndex)
	{
		KeyFrameAction keyFrameAction = Actions[soundIndex];
		if (TargetObjects == null || _targetAnimation == null || !Actions.Any((KeyFrameAction a) => _targetAnimation[a.clip] != null && _targetAnimation.IsPlaying(a.clip) && a.KeyFrame == (int)(_targetAnimation[a.clip].clip.frameRate * _targetAnimation[a.clip].time)))
		{
			return;
		}
		ParticleSystem[] targetObjects = TargetObjects;
		foreach (ParticleSystem particleSystem in targetObjects)
		{
			if (!(particleSystem == null))
			{
				if (keyFrameAction.state)
				{
					particleSystem.Play();
				}
				else
				{
					particleSystem.Stop();
				}
			}
		}
	}
}
