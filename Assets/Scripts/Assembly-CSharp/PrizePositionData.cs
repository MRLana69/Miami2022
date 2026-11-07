using System.Collections.Generic;
using Kiloo.Common.Extensions;
using UnityEngine;
using WeeklyHunt;

public static class PrizePositionData
{
	public struct TransformValues
	{
		public Vector3 PositionOffset;

		public Vector3 RotationOffset;

		public Vector3 ScaleOffset;

		public TransformValues(Vector3 positionOffset, Vector3 rotation, Vector3 scale)
		{
			PositionOffset = positionOffset;
			RotationOffset = rotation;
			ScaleOffset = scale;
		}
	}

	public static Vector3 HOVERBOARD_POSITION_OFFSET = new Vector3(0f, 0f, 0f);

	public static Vector3 HOVERBOARD_SCALE_OFFSET = new Vector3(1f, 1f, 1f);

	public static Vector3 HOVERBOARD_ROTATION_OFFSET = new Vector3(336f, 152f, 27f);

	public static Vector3 KEY_POSITION_OFFSET = new Vector3(0f, 0f, 0f);

	public static Vector3 KEY_SCALE_OFFSET = new Vector3(1f, 1f, 1f);

	public static Vector3 KEY_ROTATION_OFFSET = new Vector3(13f, 204f, 319f);

	public static Vector3 COIN_POSITION_OFFSET = new Vector3(0f, 0f, 0f);

	public static Vector3 COIN_SCALE_OFFSET = new Vector3(1f, 1f, 1f);

	public static Vector3 COIN_ROTATION_OFFSET = new Vector3(0f, 10f, 5f);

	public static Vector3 SCORE_BOOSTER_POSITION_OFFSET = new Vector3(0f, 0f, 0f);

	public static Vector3 SCORE_BOOSTER_SCALE_OFFSET = new Vector3(1f, 1f, 1f);

	public static Vector3 SCORE_BOOSTER_ROTATION_OFFSET = new Vector3(14f, 202f, 326.8f);

	public static Vector3 HEADSTART_POSITION_OFFSET = new Vector3(0f, 0f, 0f);

	public static Vector3 HEADSTART_SCALE_OFFSET = new Vector3(1f, 1f, 1f);

	public static Vector3 HEADSTART_ROTATION_OFFSET = new Vector3(13f, 204f, 319f);

	public static Vector3 SUPER_MYSTERY_BOX_POSITION_OFFSET = new Vector3(0f, 0f, 0f);

	public static Vector3 SUPER_MYSTERY_BOX_SCALE_OFFSET = new Vector3(0.9f, 0.9f, 0.9f);

	public static Vector3 SUPER_MYSTERY_BOX_ROTATION_OFFSET = new Vector3(-7.5f, -135f, -7.5f);

	public static Vector3 WEEKLY_TOKEN_MYSTERY_BOX_POSITION_OFFSET = new Vector3(0f, -0.7f, 0f);

	public static Vector3 WEEKLY_TOKEN_MYSTERY_BOX_SCALE_OFFSET = new Vector3(0.9f, 0.9f, 0.9f);

	public static Vector3 WEEKLY_TOKEN_MYSTERY_BOX_ROTATION_OFFSET = new Vector3(-7.5f, -135f, -7.5f);

	public static Vector3 UNKNOWN_REWARD_POSITION_OFFSET = new Vector3(0f, 0f, 0f);

	public static Vector3 UNKNOWN_REWARD_SCALE_OFFSET = new Vector3(1f, 1f, 1f);

	public static Vector3 UNKNOWN_REWARD_ROTATION_OFFSET = new Vector3(0f, 20f, 0f);

	public static Vector3 CHRISTMAS2014_BOARD_POSITION = new Vector3(7f, 9f, 0f);

	public static Vector3 CHRISTMAS2014_BOARD_SCALE = new Vector3(14f, 14f, 14f);

	public static Vector3 CHRISTMAS2014_BOARD_ROTATION = new Vector3(-15f, 300.6f, 28f);

	public static Vector3 RIO_HUNT_BOARD_POSITION = new Vector3(2f, 12f, 0f);

	public static Vector3 RIO_HUNT_BOARD_SCALE = new Vector3(14f, 14f, 14f);

	public static Vector3 RIO_HUNT_BOARD_ROTATION = new Vector3(17f, 134f, -53f);

	public static Vector3 CHERRY2017_BOARD_POSITION = new Vector3(1f, 2f, 0f);

	public static Vector3 CHERRY2017_BOARD_ROTATION = new Vector3(-11f, 315f, 55f);

	public static Vector3 CHERRY2017_BOARD_SCALE = new Vector3(11f, 11f, 11f);

	public static Vector3 NORTHPOLE2016_HUNT_BOARD_POSITION = new Vector3(2f, 5f, 0f);

	public static Vector3 NORTHPOLE2016_HUNT_BOARD_SCALE = new Vector3(14f, 14f, 14f);

	public static Vector3 NORTHPOLE2016_HUNT_BOARD_ROTATION = new Vector3(-16f, 300f, 36f);

	public static Vector3 CHICKY_HUNT_BOARD_POSITION = new Vector3(0f, 0f, 0f);

	public static Vector3 CHICKY_HUNT_BOARD_SCALE = new Vector3(1.2f, 1.2f, 1.2f);

	public static Vector3 CHICKY_HUNT_BOARD_ROTATION = new Vector3(-36f, -216f, 30f);

	public static Vector3 EAGLE_HUNT_BOARD_POSITION = new Vector3(0f, 0.5f, 0f);

	public static Vector3 EAGLE_HUNT_BOARD_SCALE = new Vector3(1f, 1f, 1f);

	public static Vector3 EAGLE_HUNT_BOARD_ROTATION = new Vector3(20f, 0f, -24f);

	public static Vector3 PRESENT_POSITION = new Vector3(0f, 0f, 0f);

	public static Vector3 PRESENT_SCALE = new Vector3(5f, 5f, 5f);

	public static Vector3 PRESENT_ROTATION = new Vector3(-45f, 150f, 0f);

	private static Dictionary<WeeklyHuntData.WeeklyHuntPrize, TransformValues?> _cachedPositionData = new Dictionary<WeeklyHuntData.WeeklyHuntPrize, TransformValues?>();

	public static Vector3 rewardRotationOffsetFrom = new Vector3(0f, -25f, 0f);

	public static Vector3 rewardRotationOffsetTo = new Vector3(0f, 5f, 0f);

	public static float rewardRotationDuration = 10f;

	public static TransformValues? GetPositioningDataForUnknownReward(bool hidePrize = false)
	{
		if (hidePrize)
		{
			return new TransformValues(PRESENT_POSITION, PRESENT_ROTATION, PRESENT_SCALE);
		}
		return new TransformValues(UNKNOWN_REWARD_POSITION_OFFSET, UNKNOWN_REWARD_ROTATION_OFFSET, UNKNOWN_REWARD_SCALE_OFFSET);
	}

	public static TransformValues? GetPrizePositioningData(WeeklyHuntData.PrizeData prize, bool hidePrize = false)
	{
		WeeklyHuntData.WeeklyHuntPrize weeklyHuntPrize = WeeklyHuntManager.DeterminePrize(prize);
		if (_cachedPositionData.ContainsKey(weeklyHuntPrize))
		{
			return _cachedPositionData[weeklyHuntPrize];
		}
		TransformValues? transformValues = null;
		if (hidePrize)
		{
			transformValues = new TransformValues(PRESENT_POSITION, PRESENT_ROTATION, PRESENT_SCALE);
		}
		else
		{
			switch (weeklyHuntPrize)
			{
			case WeeklyHuntData.WeeklyHuntPrize.Key:
				transformValues = new TransformValues(KEY_POSITION_OFFSET, KEY_ROTATION_OFFSET, KEY_SCALE_OFFSET);
				break;
			case WeeklyHuntData.WeeklyHuntPrize.Coin:
				transformValues = new TransformValues(COIN_POSITION_OFFSET, COIN_ROTATION_OFFSET, COIN_SCALE_OFFSET);
				break;
			case WeeklyHuntData.WeeklyHuntPrize.SuperMysteryBox:
				transformValues = new TransformValues(SUPER_MYSTERY_BOX_POSITION_OFFSET, SUPER_MYSTERY_BOX_ROTATION_OFFSET, SUPER_MYSTERY_BOX_SCALE_OFFSET);
				break;
			case WeeklyHuntData.WeeklyHuntPrize.TokenMysteryBox:
				transformValues = new TransformValues(WEEKLY_TOKEN_MYSTERY_BOX_POSITION_OFFSET, WEEKLY_TOKEN_MYSTERY_BOX_ROTATION_OFFSET, WEEKLY_TOKEN_MYSTERY_BOX_SCALE_OFFSET);
				break;
			case WeeklyHuntData.WeeklyHuntPrize.Hoverboard:
				transformValues = new TransformValues(HOVERBOARD_POSITION_OFFSET, HOVERBOARD_ROTATION_OFFSET, HOVERBOARD_SCALE_OFFSET);
				break;
			case WeeklyHuntData.WeeklyHuntPrize.ScoreBooster:
				transformValues = new TransformValues(SCORE_BOOSTER_POSITION_OFFSET, SCORE_BOOSTER_ROTATION_OFFSET, SCORE_BOOSTER_SCALE_OFFSET);
				break;
			case WeeklyHuntData.WeeklyHuntPrize.Headstart:
				transformValues = new TransformValues(HEADSTART_POSITION_OFFSET, HEADSTART_ROTATION_OFFSET, HEADSTART_SCALE_OFFSET);
				break;
			case WeeklyHuntData.WeeklyHuntPrize.ChristmasBoard:
				transformValues = new TransformValues(CHRISTMAS2014_BOARD_POSITION, CHRISTMAS2014_BOARD_ROTATION, CHRISTMAS2014_BOARD_SCALE);
				break;
			case WeeklyHuntData.WeeklyHuntPrize.RioBoard2:
				transformValues = new TransformValues(RIO_HUNT_BOARD_POSITION, RIO_HUNT_BOARD_ROTATION, RIO_HUNT_BOARD_SCALE);
				break;
			case WeeklyHuntData.WeeklyHuntPrize.NorthpoleBoard2016:
				transformValues = new TransformValues(NORTHPOLE2016_HUNT_BOARD_POSITION, NORTHPOLE2016_HUNT_BOARD_ROTATION, NORTHPOLE2016_HUNT_BOARD_SCALE);
				break;
			case WeeklyHuntData.WeeklyHuntPrize.CherryBoard:
				transformValues = new TransformValues(CHERRY2017_BOARD_POSITION, CHERRY2017_BOARD_ROTATION, CHERRY2017_BOARD_SCALE);
				break;
			case WeeklyHuntData.WeeklyHuntPrize.ChickyBoard:
				transformValues = new TransformValues(CHICKY_HUNT_BOARD_POSITION, CHICKY_HUNT_BOARD_ROTATION, CHICKY_HUNT_BOARD_SCALE);
				break;
			case WeeklyHuntData.WeeklyHuntPrize.EagleBoard:
				transformValues = new TransformValues(EAGLE_HUNT_BOARD_POSITION, EAGLE_HUNT_BOARD_ROTATION, EAGLE_HUNT_BOARD_SCALE);
				break;
			}
			if (transformValues.HasValue)
			{
				_cachedPositionData.Add(weeklyHuntPrize, transformValues);
			}
			else
			{
				SDebug.LogError("Unable to find prize position data for type: " + weeklyHuntPrize);
			}
		}
		return transformValues;
	}

	public static void PositionModel(TransformValues? positionData, Transform rewardTransform, UIWidget prizeGuideWidget = null, bool hidePrize = false)
	{
		Vector3 vector = default(Vector3);
		Vector3 vector2 = default(Vector3);
		if ((bool)prizeGuideWidget)
		{
			vector = prizeGuideWidget.transform.position;
			vector2 = UIScreenController.Instance.Camera2DNormal.WorldToScreenPoint(vector);
		}
		vector2 = new Vector3(vector2.x, vector2.y, 3f);
		Vector3 newPosition = (hidePrize ? UIScreenController.Instance.Camera3DClip.ScreenToWorldPoint(vector2) : UIScreenController.Instance.Camera3D.ScreenToWorldPoint(vector2));
		FitAndApplyOffsets(rewardTransform, newPosition, positionData, prizeGuideWidget, hidePrize);
	}

	public static void FitAndApplyOffsets(Transform rewardTransform, Vector3 newPosition, TransformValues? offsetValues, UIWidget targetWidget = null, bool isPresent = false)
	{
		rewardTransform.position = newPosition;
		Transform target = ((!isPresent) ? UIScreenController.Instance.Camera3D.transform : UIScreenController.Instance.Camera3DClip.transform);
		rewardTransform.LookAt(target);
		rewardTransform.Rotate(offsetValues.Value.RotationOffset);
		if (targetWidget != null)
		{
			rewardTransform.FitToWidget(targetWidget);
		}
		rewardTransform.localScale = Vector3.Scale(rewardTransform.localScale, offsetValues.Value.ScaleOffset);
		Vector3 vector = Vector3.Scale(offsetValues.Value.PositionOffset, rewardTransform.localScale);
		rewardTransform.localPosition += vector;
	}
}
