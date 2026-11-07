using System.Collections.Generic;
using UnityEngine;

public class KeyframeBehaviour : MonoBehaviour
{
	private Animation TargetAnimation;

	public ParticleSystem[] TargetObjects;

	public List<KeyFrameAction> Actions;

	public void Initialize()
	{
		TargetAnimation = base.gameObject.GetComponentInChildren<Animation>();
		TargetAnimation.gameObject.GetComponent<AnimationEventBroadcaster>().SendDoKeyframeAnimation += DoKeyframeAnimation;
		int num = 0;
		foreach (KeyFrameAction action in Actions)
		{
			if (TargetAnimation[action.clip] != null)
			{
				AnimationEvent animationEvent = new AnimationEvent();
				animationEvent.messageOptions = SendMessageOptions.RequireReceiver;
				if (TargetAnimation[action.clip] != null)
				{
					animationEvent.time = (float)action.KeyFrame / TargetAnimation[action.clip].clip.frameRate;
				}
				animationEvent.intParameter = num;
				animationEvent.functionName = "DoKeyframeAnimation";
				Globals.TryAddAnimationEvent(TargetAnimation, action.clip, animationEvent);
			}
			num++;
		}
	}

	private void DoKeyframeAnimation(int soundIndex)
	{
		KeyFrameAction keyFrameAction = Actions[soundIndex];
		if (TargetObjects == null)
		{
			return;
		}
		ParticleSystem[] targetObjects = TargetObjects;
		foreach (ParticleSystem particleSystem in targetObjects)
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
