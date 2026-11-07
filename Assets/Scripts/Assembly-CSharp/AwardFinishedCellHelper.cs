using Missions;
using UnityEngine;

public class AwardFinishedCellHelper : MonoBehaviour
{
	private const string AWARD_COMPLETE_DESCRIPTION_HEX_COLOR = "[549133]";

	private const string AWARD_COMPLETE_VALUE_HEX_COLOR = "[015A01]";

	private const string END_COLOR_SYMBOL = "[-]";

	private static readonly Color32 COMPLETED_TITLE_TEXT_COLOR = new Color32(1, 90, 1, byte.MaxValue);

	private static readonly Color32 COMPLETED_TITLE_TEXT_SHADOW_COLOR = new Color32(232, 248, 228, byte.MaxValue);

	private static readonly Color32 COMPLETE_SEGMENT_BACKGROUND_COLOR = new Color32(146, 231, 127, 180);

	[SerializeField]
	private UILabel _missionTitle;

	[SerializeField]
	private UILabel _missionTextDescription;

	[SerializeField]
	private GameObject _missionIconsDescriptionParent;

	[SerializeField]
	private UISprite _cellBackground;

	[SerializeField]
	private AwardCollectibleHelper _mainGoalElementsHelper;

	[SerializeField]
	private UISprite _mysteryBoxIcon;

	[SerializeField]
	private AwardCollectibleHelper[] _goalIcons = new AwardCollectibleHelper[4];

	private string[] _mysteryBoxIconNames = new string[2] { "icon_small_mysteryBox", "icon_small_superMysteryBox" };

	private const string COMPLETED_ITEM_BACKGROUND_SPRITE_NAME = "icon_award_bg_completed";

	private const string COMPLETED_GOAL_ICON_BACKGROUND_SPRITE_NAME = "icon_award_bg_completed";

	private Award _currentAward;

	public void InitCell(Award award)
	{
		if (award != null)
		{
			_currentAward = award;
		}
		SetupCellStructure();
		SetupCellContent();
		SetupCellColors();
	}

	private void SetupCellStructure()
	{
		_missionTextDescription.gameObject.SetActive(_currentAward.awardsData.hasDescription);
		_missionIconsDescriptionParent.SetActive(!_currentAward.awardsData.hasDescription);
	}

	private void SetupCellContent()
	{
		AwardGoal[] goals = _currentAward.goals;
		_missionTitle.text = Strings.Get(_currentAward.awardsData.title);
		_mainGoalElementsHelper.SetupAwardCollectible(FindBackgroundToUse(true), "icon_diamond_can", true, true);
		if (_currentAward.awardsData.hasDescription)
		{
			string key = ((!_currentAward.awardsData.hasAnotherDescriptionForOneItemGoal || goals[0].goal != 1) ? _currentAward.awardsData.description : _currentAward.awardsData.descriptionForOneItemGoal);
			_missionTextDescription.text = string.Format(Strings.Get(key), "[549133]", "[-]", "[015A01]", goals[0].goal, "[-]", "[549133]", "[-]");
		}
		else
		{
			for (int i = 0; i < goals.Length; i++)
			{
				_goalIcons[i].SetupAwardCollectible(FindBackgroundToUse(false), goals[i].iconName, false, true);
			}
			if (_currentAward.awardsData.id == AwardId.COLLECT_OLD_TROPHY_ITEMS_MB)
			{
				_mysteryBoxIcon.spriteName = _mysteryBoxIconNames[0];
			}
			else if (_currentAward.awardsData.id == AwardId.COLLECT_OLD_TROPHY_ITEMS_SMB)
			{
				_mysteryBoxIcon.spriteName = _mysteryBoxIconNames[1];
			}
		}
	}

	private void SetupCellColors()
	{
		_missionTextDescription.color = Color.white;
		_cellBackground.color = COMPLETE_SEGMENT_BACKGROUND_COLOR;
		_missionTitle.color = COMPLETED_TITLE_TEXT_COLOR;
		_missionTitle.effectColor = COMPLETED_TITLE_TEXT_SHADOW_COLOR;
	}

	private string FindBackgroundToUse(bool mainGoalIcon)
	{
		return (!mainGoalIcon) ? "icon_award_bg_completed" : "icon_award_bg_completed";
	}
}
