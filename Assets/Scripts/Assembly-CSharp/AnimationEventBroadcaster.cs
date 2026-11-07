using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class AnimationEventBroadcaster : MonoBehaviour
{
	public delegate void SendDoKeyframeAnimationDelegate(int soundIndex);

	public delegate void SendPlayKeyframeAnimationDelegate(int soundIndex);

	public delegate void SendSetAnimationSpeedEventDelegate(AnimationEvent animEvent);

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private SendDoKeyframeAnimationDelegate SendDoKeyframeAnimation__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private SendPlayKeyframeAnimationDelegate SendPlayKeyframeAnimation__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private SendSetAnimationSpeedEventDelegate SendSetAnimationSpeedEvent__BackingField;

	public event SendDoKeyframeAnimationDelegate SendDoKeyframeAnimation
	{
		add
		{
			SendDoKeyframeAnimationDelegate sendDoKeyframeAnimationDelegate = SendDoKeyframeAnimation__BackingField;
			SendDoKeyframeAnimationDelegate sendDoKeyframeAnimationDelegate2;
			do
			{
				sendDoKeyframeAnimationDelegate2 = sendDoKeyframeAnimationDelegate;
				sendDoKeyframeAnimationDelegate = Interlocked.CompareExchange(ref SendDoKeyframeAnimation__BackingField, (SendDoKeyframeAnimationDelegate)Delegate.Combine(sendDoKeyframeAnimationDelegate2, value), sendDoKeyframeAnimationDelegate);
			}
			while ((object)sendDoKeyframeAnimationDelegate != sendDoKeyframeAnimationDelegate2);
		}
		remove
		{
			SendDoKeyframeAnimationDelegate sendDoKeyframeAnimationDelegate = SendDoKeyframeAnimation__BackingField;
			SendDoKeyframeAnimationDelegate sendDoKeyframeAnimationDelegate2;
			do
			{
				sendDoKeyframeAnimationDelegate2 = sendDoKeyframeAnimationDelegate;
				sendDoKeyframeAnimationDelegate = Interlocked.CompareExchange(ref SendDoKeyframeAnimation__BackingField, (SendDoKeyframeAnimationDelegate)Delegate.Remove(sendDoKeyframeAnimationDelegate2, value), sendDoKeyframeAnimationDelegate);
			}
			while ((object)sendDoKeyframeAnimationDelegate != sendDoKeyframeAnimationDelegate2);
		}
	}

	public event SendPlayKeyframeAnimationDelegate SendPlayKeyframeAnimation
	{
		add
		{
			SendPlayKeyframeAnimationDelegate sendPlayKeyframeAnimationDelegate = SendPlayKeyframeAnimation__BackingField;
			SendPlayKeyframeAnimationDelegate sendPlayKeyframeAnimationDelegate2;
			do
			{
				sendPlayKeyframeAnimationDelegate2 = sendPlayKeyframeAnimationDelegate;
				sendPlayKeyframeAnimationDelegate = Interlocked.CompareExchange(ref SendPlayKeyframeAnimation__BackingField, (SendPlayKeyframeAnimationDelegate)Delegate.Combine(sendPlayKeyframeAnimationDelegate2, value), sendPlayKeyframeAnimationDelegate);
			}
			while ((object)sendPlayKeyframeAnimationDelegate != sendPlayKeyframeAnimationDelegate2);
		}
		remove
		{
			SendPlayKeyframeAnimationDelegate sendPlayKeyframeAnimationDelegate = SendPlayKeyframeAnimation__BackingField;
			SendPlayKeyframeAnimationDelegate sendPlayKeyframeAnimationDelegate2;
			do
			{
				sendPlayKeyframeAnimationDelegate2 = sendPlayKeyframeAnimationDelegate;
				sendPlayKeyframeAnimationDelegate = Interlocked.CompareExchange(ref SendPlayKeyframeAnimation__BackingField, (SendPlayKeyframeAnimationDelegate)Delegate.Remove(sendPlayKeyframeAnimationDelegate2, value), sendPlayKeyframeAnimationDelegate);
			}
			while ((object)sendPlayKeyframeAnimationDelegate != sendPlayKeyframeAnimationDelegate2);
		}
	}

	public event SendSetAnimationSpeedEventDelegate SendSetAnimationSpeedEvent
	{
		add
		{
			SendSetAnimationSpeedEventDelegate sendSetAnimationSpeedEventDelegate = SendSetAnimationSpeedEvent__BackingField;
			SendSetAnimationSpeedEventDelegate sendSetAnimationSpeedEventDelegate2;
			do
			{
				sendSetAnimationSpeedEventDelegate2 = sendSetAnimationSpeedEventDelegate;
				sendSetAnimationSpeedEventDelegate = Interlocked.CompareExchange(ref SendSetAnimationSpeedEvent__BackingField, (SendSetAnimationSpeedEventDelegate)Delegate.Combine(sendSetAnimationSpeedEventDelegate2, value), sendSetAnimationSpeedEventDelegate);
			}
			while ((object)sendSetAnimationSpeedEventDelegate != sendSetAnimationSpeedEventDelegate2);
		}
		remove
		{
			SendSetAnimationSpeedEventDelegate sendSetAnimationSpeedEventDelegate = SendSetAnimationSpeedEvent__BackingField;
			SendSetAnimationSpeedEventDelegate sendSetAnimationSpeedEventDelegate2;
			do
			{
				sendSetAnimationSpeedEventDelegate2 = sendSetAnimationSpeedEventDelegate;
				sendSetAnimationSpeedEventDelegate = Interlocked.CompareExchange(ref SendSetAnimationSpeedEvent__BackingField, (SendSetAnimationSpeedEventDelegate)Delegate.Remove(sendSetAnimationSpeedEventDelegate2, value), sendSetAnimationSpeedEventDelegate);
			}
			while ((object)sendSetAnimationSpeedEventDelegate != sendSetAnimationSpeedEventDelegate2);
		}
	}

	private void DoKeyframeAnimation(int soundIndex)
	{
		if (SendDoKeyframeAnimation__BackingField != null)
		{
			SendDoKeyframeAnimation__BackingField(soundIndex);
		}
	}

	private void PlayKeyframeAnimation(int soundIndex)
	{
		if (SendPlayKeyframeAnimation__BackingField != null)
		{
			SendPlayKeyframeAnimation__BackingField(soundIndex);
		}
	}

	private void SetAnimationSpeedEvent(AnimationEvent animEvent)
	{
		if (SendSetAnimationSpeedEvent__BackingField != null)
		{
			SendSetAnimationSpeedEvent__BackingField(animEvent);
		}
	}
}
