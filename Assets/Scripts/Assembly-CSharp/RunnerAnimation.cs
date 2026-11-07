using SYBO.Subway;
using UnityEngine;

public class RunnerAnimation : MonoBehaviour
{
	private static bool addedListeners;

	public float AnimationSpeedUpFactor = 0.5f;

	private AnimationEventBroadcaster animationEventBroadcaster;

	private void Awake()
	{
		animationEventBroadcaster = base.gameObject.GetComponentInChildren<AnimationEventBroadcaster>();
		animationEventBroadcaster.SendSetAnimationSpeedEvent += SetAnimationSpeedEvent;
	}

	public void SetAnimationSpeedEvent(AnimationEvent animEvent)
	{
		if (!addedListeners)
		{
			addedListeners = true;
			SetSpeed(animEvent.animationState, 1f + (Game.Instance.CurrentSpeedNormalized - 1f) * AnimationSpeedUpFactor);
		}
	}

	private void SetSpeed(AnimationState animationState, float speed)
	{
		animationState.speed = speed;
	}
}
