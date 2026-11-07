using JetBrains.Annotations;
using SYBO.Subway.Info;
using UI.Popups;
using UnityEngine;

public class TutorialCollectFromFriendsPopup : CharacterPopup
{
	[SerializeField]
	private GameObject _centerAnchor;

	[SerializeField]
	private UILabel _topTitle;

	[SerializeField]
	private UILabel _buttonText;

	[SerializeField]
	private GameObject _tutorialMultiLinesPrefab;

	private TutorialMultilineLabelsHelper _tutorialLinesHelper;

	protected override float screenOffset
	{
		get
		{
			return -13f;
		}
	}

	protected override void Init()
	{
		base.Init();
		_tutorialLinesHelper = TutorialMultilineLabelsHelper.AddToGameObject(_centerAnchor, _tutorialMultiLinesPrefab);
	}

	protected override void Show()
	{
		base.Show();
		SetCharacter(Characters.CharacterId.slick, new Vector2(39f, -5f), null, 19f, false, null, false);
		_topTitle.text = Strings.Get("POPUP_TUTORIAL_COLLECT_FROM_FRIENDS_TITLE");
		_buttonText.text = Strings.Get("POPUP_TUTORIAL_COLLECT_FROM_FRIENDS_BUTTON");
		_tutorialLinesHelper.SetText("POPUP_TUTORIAL_COLLECT_FROM_FRIENDS_LINE1BIG", "POPUP_TUTORIAL_COLLECT_FROM_FRIENDS_LINE1SMALL", "POPUP_TUTORIAL_COLLECT_FROM_FRIENDS_LINE2BIG", "POPUP_TUTORIAL_COLLECT_FROM_FRIENDS_LINE2SMALL", "POPUP_TUTORIAL_COLLECT_FROM_FRIENDS_LINE3BIG", "POPUP_TUTORIAL_COLLECT_FROM_FRIENDS_LINE3SMALL");
	}

	protected override void Hide()
	{
		HideCharacter();
	}

	[UsedImplicitly]
	public void OkClicked()
	{
		Flurry.LogOkPressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
		UIScreenController.Instance.CloseActivePopups(screenName);
	}

	[UsedImplicitly]
	public void CloseClicked()
	{
		Flurry.LogClosePressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
		UIScreenController.Instance.CloseActivePopups(screenName);
	}
}
