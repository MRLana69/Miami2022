using System;
using System.Collections.Generic;
using SYBO.Subway;
using SYBO.Subway.Characters;
using SYBO.Subway.StateSystem;
using SYBO.Subway.StateSystem.States;
using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
	[SerializeField]
	private AudioClipInfo DodgeLeft;

	[SerializeField]
	private AudioClipInfo TeleboardDodgeLeft;

	[SerializeField]
	private AudioClipInfo H_Left;

	[SerializeField]
	private AudioClipInfo H_Right;

	[SerializeField]
	private AudioClipInfo StumbleSound;

	[SerializeField]
	private AudioClipInfo StumbleBushSound;

	[SerializeField]
	private AudioClipInfo StumbleSideSound;

	[SerializeField]
	private AudioClipInfo PowerupSound;

	[SerializeField]
	private AudioClipInfo PowerdownSound;

	[SerializeField]
	private AudioClipInfo[] TurboHeadstartSound;

	[SerializeField]
	private AudioClipInfo[] ScooreBoosterSound;

	[SerializeField]
	public AudioClipInfo DodgeRight;

	[SerializeField]
	public AudioClipInfo TeleboardDodgeRight;

	private Hoverboard hoverboard;

	private Character character;

	private AudioStateLoop audioStateLoop;

	private Game game;

	private Dictionary<Character.StumbleType, AudioClipInfo> stumbleClips;

	public void Awake()
	{
		hoverboard = Hoverboard.Instance;
		character = GetComponent<Character>();
		character.OnChangeTrack += HandleOnChangeTrack;
		character.OnStumble += HandleOnStumble;
		stumbleClips = new Dictionary<Character.StumbleType, AudioClipInfo>();
		stumbleClips.Add(Character.StumbleType.Normal, StumbleSound);
		stumbleClips.Add(Character.StumbleType.Bush, StumbleBushSound);
		stumbleClips.Add(Character.StumbleType.Side, StumbleSideSound);
		game = Game.Instance;
		game.OnTurboHeadstartInput += HandleOnTurboHeadstartInput;
		JetpackState state = GameStateMachine.Instance.GetState<JetpackState>(4);
		state.OnStart = (Action)Delegate.Combine(state.OnStart, new Action(JetpackOnStart));
		state.OnStop = (Action)Delegate.Combine(state.OnStop, new Action(JetpackOnStop));
		HeadstartState state2 = GameStateMachine.Instance.GetState<HeadstartState>(3);
		state2.OnStart = (Action)Delegate.Combine(state2.OnStart, new Action(JetpackOnStart));
		state2.OnStop = (Action)Delegate.Combine(state2.OnStop, new Action(JetpackOnStop));
		PogostickState state3 = GameStateMachine.Instance.GetState<PogostickState>(5);
		state3.OnStart = (Action)Delegate.Combine(state3.OnStart, new Action(JetpackOnStart));
		state3.OnStop = (Action)Delegate.Combine(state3.OnStop, new Action(PogostickStop));
		audioStateLoop = Utils.FindObject<AudioStateLoop>();
		SlideinPowerupHelper.OnScoreBoostActivated += HandleOnScoreBoostActivated;
	}

	private void HandleOnScoreBoostActivated(int scoreboosterTier)
	{
		AudioClipInfo audioClip = ScooreBoosterSound[scoreboosterTier - 1];
		So.Instance.playSound(audioClip);
	}

	private void HandleOnTurboHeadstartInput(int version)
	{
		AudioClipInfo audioClipInfo = TurboHeadstartSound[version];
		if (audioClipInfo != null)
		{
			So.Instance.playSound(audioClipInfo);
		}
	}

	private void HandleOnChangeTrack(Character.OnChangeTrackDirection direction)
	{
		if (direction == Character.OnChangeTrackDirection.Left)
		{
			if (hoverboard.isActive)
			{
				if (hoverboard.IsTeleporter && !game.IsInPogostickMode && !game.IsInJetpackMode)
				{
					So.Instance.playSound(TeleboardDodgeLeft);
				}
				else
				{
					So.Instance.playSound(H_Left);
				}
			}
			else
			{
				So.Instance.playSound(DodgeLeft);
			}
		}
		else if (hoverboard.isActive)
		{
			if (hoverboard.IsTeleporter && !game.IsInPogostickMode && !game.IsInJetpackMode)
			{
				So.Instance.playSound(TeleboardDodgeRight);
			}
			else
			{
				So.Instance.playSound(H_Right);
			}
		}
		else
		{
			So.Instance.playSound(DodgeRight);
		}
	}

	private void HandleOnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName, bool safeStumble)
	{
		So.Instance.playSound(stumbleClips[stumbleType]);
	}

	private void JetpackOnStart()
	{
		So.Instance.playSound(PowerupSound);
		audioStateLoop.ChangeLoop(AudioState.Jetpack);
	}

	private void JetpackOnStop()
	{
		audioStateLoop.ChangeLoop(AudioState.JetpackStop);
		So.Instance.playSound(PowerdownSound);
	}

	private void PogostickStop()
	{
		JetpackOnStop();
	}
}
