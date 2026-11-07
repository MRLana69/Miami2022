using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using SYBO.Components.Utilities;
using SYBO.Subway;
using SYBO.Subway.Cameras;
using SYBO.Subway.Characters;
using SYBO.Subway.Rendering;
using SYBO.Subway.Routes;
using UnityEngine;

public class Revive : MonoBehaviour
{
	public delegate void OnSwitchToRunningDelegate();

	public delegate void OnReviveDelegate();

	private Game game;

	private Character character;

	public float WaitForParticlesDelay = 0.144f;

	public float RemoveObstaclesDistance = 250f;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnSwitchToRunningDelegate OnSwitchToRunning__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnReviveDelegate OnRevive__BackingField;

	private MainCamera _mainCamera;

	private ParticleSystem _reviveParticles;

	private ReviveFX _reviveFX;

	[SerializeField]
	private AnimationClip ReviveAnimationClip;

	private static Revive instance;

	public static Revive Instance
	{
		get
		{
			return instance ?? (instance = Utils.FindObject<Revive>());
		}
	}

	public event OnSwitchToRunningDelegate OnSwitchToRunning
	{
		add
		{
			OnSwitchToRunningDelegate onSwitchToRunningDelegate = OnSwitchToRunning__BackingField;
			OnSwitchToRunningDelegate onSwitchToRunningDelegate2;
			do
			{
				onSwitchToRunningDelegate2 = onSwitchToRunningDelegate;
				onSwitchToRunningDelegate = Interlocked.CompareExchange(ref OnSwitchToRunning__BackingField, (OnSwitchToRunningDelegate)Delegate.Combine(onSwitchToRunningDelegate2, value), onSwitchToRunningDelegate);
			}
			while ((object)onSwitchToRunningDelegate != onSwitchToRunningDelegate2);
		}
		remove
		{
			OnSwitchToRunningDelegate onSwitchToRunningDelegate = OnSwitchToRunning__BackingField;
			OnSwitchToRunningDelegate onSwitchToRunningDelegate2;
			do
			{
				onSwitchToRunningDelegate2 = onSwitchToRunningDelegate;
				onSwitchToRunningDelegate = Interlocked.CompareExchange(ref OnSwitchToRunning__BackingField, (OnSwitchToRunningDelegate)Delegate.Remove(onSwitchToRunningDelegate2, value), onSwitchToRunningDelegate);
			}
			while ((object)onSwitchToRunningDelegate != onSwitchToRunningDelegate2);
		}
	}

	public event OnReviveDelegate OnRevive
	{
		add
		{
			OnReviveDelegate onReviveDelegate = OnRevive__BackingField;
			OnReviveDelegate onReviveDelegate2;
			do
			{
				onReviveDelegate2 = onReviveDelegate;
				onReviveDelegate = Interlocked.CompareExchange(ref OnRevive__BackingField, (OnReviveDelegate)Delegate.Combine(onReviveDelegate2, value), onReviveDelegate);
			}
			while ((object)onReviveDelegate != onReviveDelegate2);
		}
		remove
		{
			OnReviveDelegate onReviveDelegate = OnRevive__BackingField;
			OnReviveDelegate onReviveDelegate2;
			do
			{
				onReviveDelegate2 = onReviveDelegate;
				onReviveDelegate = Interlocked.CompareExchange(ref OnRevive__BackingField, (OnReviveDelegate)Delegate.Remove(onReviveDelegate2, value), onReviveDelegate);
			}
			while ((object)onReviveDelegate != onReviveDelegate2);
		}
	}

	private void Start()
	{
		game = Game.Instance;
		character = Character.Instance;
		_reviveParticles = CharacterCamera.Instance.MainCamera.ReviveEffectTransform.GetComponent<ParticleSystem>();
		Renderer component = _reviveParticles.GetComponent<Renderer>();
		component.material.renderQueue = RenderQueues.TransparentOverlay;
		_reviveFX = Character.Instance.ReviveFX;
		_reviveFX.gameObject.SetActive(false);
	}

	public void SendRevive(bool revivedWithAds, bool adsWasAvailable, bool adsFailed)
	{
		StartCoroutine(ReviveNow(revivedWithAds, adsWasAvailable, adsFailed));
	}

	public void SendSkipRevive()
	{
		StartCoroutine(game.SkipRevive());
	}

	private IEnumerator ReviveNow(bool revivedWithAds, bool adsWasAvailable, bool adsFailed)
	{
		_reviveParticles.gameObject.SetActive(true);
		_reviveParticles.Play();
		SingletonMonoBehaviour<AudioStateLoop>.Instance.PlayReviveSound();
		float timeLeft = WaitForParticlesDelay;
		while (timeLeft > 0f)
		{
			timeLeft -= Time.deltaTime;
			yield return null;
		}
		SingletonMonoBehaviour<RouteManager>.Instance.CharacterRoute.Builder.HideLogic(character.GamePosition.z, RemoveObstaclesDistance * Game.Instance.CurrentSpeedNormalized);
		if (OnRevive__BackingField != null)
		{
			OnRevive__BackingField();
		}
		game.Revive(revivedWithAds, adsWasAvailable, adsFailed);
		if (OnSwitchToRunning__BackingField != null)
		{
			OnSwitchToRunning__BackingField();
		}
		_reviveFX.gameObject.SetActive(true);
		_reviveFX.ReviveAnimation.Play();
		float timeLeftFX = ReviveAnimationClip.length;
		while (timeLeftFX > 0f)
		{
			timeLeftFX -= Time.deltaTime;
			yield return null;
		}
		_reviveFX.gameObject.SetActive(false);
		_reviveParticles.gameObject.SetActive(false);
	}
}
