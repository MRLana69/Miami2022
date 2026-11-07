using Missions;
using SYBO.Subway.Characters;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
	private GameStats stats;

	private Character character;

	public void Start()
	{
		stats = GameStats.Instance;
		character = GetComponent<Character>();
		character.OnJump += OnJump;
		character.OnRoll += OnRoll;
		character.OnPassedObstacle += OnPassedObstacle;
		character.OnJumpOverTrain += OnJumpOverTrain;
		character.OnCriticalHit += OnCriticalHit;
		character.OnStumble += OnStumble;
	}

	private void OnJump(float jumpHeight, bool willPlaySound)
	{
		stats.JumpCountIncrement();
	}

	private void OnRoll()
	{
		stats.RollCountIncrement();
		if (character.TrackIndexCurrent == 0)
		{
			stats.RollsLeftTrackCountIncrement();
		}
		if (character.TrackIndexCurrent == 1)
		{
			stats.RollsCenterTrackCountIncrement();
		}
		if (character.TrackIndexCurrent == 2)
		{
			stats.RollsRightTrackCountIncrement();
		}
	}

	private void OnPassedObstacle(Character.ObstaclePassType type)
	{
		switch (type)
		{
		case Character.ObstaclePassType.Jump:
			stats.JumpBarrierCountIncrement();
			break;
		case Character.ObstaclePassType.JumpHigh:
			stats.JumpBarrierCountIncrement();
			stats.JumpHighBarrierCountIncrement();
			break;
		case Character.ObstaclePassType.Roll:
			stats.DodgeBarrierCountIncrement();
			break;
		}
	}

	private void OnJumpOverTrain()
	{
		stats.JumpsOverTrainsCountIncrement();
	}

	private void OnCriticalHit(Character.CriticalHitType type)
	{
		switch (type)
		{
		case Character.CriticalHitType.Train:
			stats.TrainHitCountIncrement();
			break;
		case Character.CriticalHitType.Barrier:
			stats.BarrierHitCountIncrement();
			break;
		case Character.CriticalHitType.MovingTrain:
			stats.MovingTrainHitCount++;
			break;
		}
	}

	private void OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName, bool safeStumble)
	{
		if (colliderName.Contains("lightSignal"))
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.BumpLightSignal);
		}
		else if (colliderName.Contains("bush") || colliderName.Contains("powerbox"))
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.BumpBush);
		}
		else if (colliderName.Contains("ramp_side"))
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.BumpTrain);
		}
		else if (colliderName.Contains("blocker_jump"))
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.BumpBarrier);
		}
		else if (colliderName.Contains("blocker_roll"))
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.BumpBarrier);
		}
		else if (colliderName.Contains("blocker_standard"))
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.BumpBarrier);
		}
		else
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.BumpTrain);
		}
	}
}
