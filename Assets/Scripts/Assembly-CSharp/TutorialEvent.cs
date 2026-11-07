using System.Collections;
using Data.Save;
using JetBrains.Annotations;
using NaughtyAttributes;
using Onboarding;
using SYBO.Subway;
using SYBO.Subway.Characters;
using UnityEngine;

public class TutorialEvent : MonoBehaviour
{
	public enum TutorialText
	{
		None = 0,
		FreshMoves = 1,
		YouRock = 2
	}

	public enum TutorialAction
	{
		ShowArrow = 0,
		ShowHoverboardClickAnimation = 1,
		ShowText = 2
	}

	private Game game;

	[SerializeField]
	private TutorialAction _tutorialAction;

	[SerializeField]
	[ShowIf("IsTutorialActionEqualToShowText")]
	private TutorialText tutorialText;

	[SerializeField]
	private float direction;

	[SerializeField]
	private float time = 1f;

	[SerializeField]
	private bool endTutorial;

	[SerializeField]
	private bool allowHoverboard;

	private Hoverboard hoverboard;

	private Character character;

	private bool _initialized;

	private GameObject _mesh;

	private GameObject mesh
	{
		get
		{
			if (_mesh == null)
			{
				_mesh = Camera.main.gameObject.transform.Find("arrow").gameObject;
			}
			return _mesh;
		}
	}

	[UsedImplicitly]
	private bool IsTutorialActionEqualToShowText()
	{
		return _tutorialAction == TutorialAction.ShowText;
	}

	private void Awake()
	{
		game = Game.Instance;
		hoverboard = Hoverboard.Instance;
	}

	private void Start()
	{
		if (!(game == null) && !_initialized)
		{
			character = game.Character;
			_initialized = true;
		}
	}

	private IEnumerator ShowArrow()
	{
		mesh.transform.rotation = Quaternion.AngleAxis(direction, new Vector3(0f, 0f, 1f)) * Quaternion.Euler(0f, 180f, 0f);
		mesh.SetActive(true);
		Vector3 pos = new Vector3(0f, 0f, 20f);
		yield return StartCoroutine(pTween.To(time, (float t) =>
		{
			mesh.transform.localPosition = Vector3.Lerp(pos - mesh.transform.up * 5f, pos + mesh.transform.up * 5f, t);
			mesh.GetComponent<Renderer>().material.mainTextureOffset = Vector2.Lerp(Vector2.zero, new Vector2(0f, -0.035f), t);
			if (!game.IsInGame.Value)
			{
				mesh.transform.localPosition = new Vector3(2000f, 2000f, 0f);
			}
		}));
		mesh.SetActive(false);
	}

	private void OnDisable()
	{
		if (_mesh != null)
		{
			_mesh.SetActive(false);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (!character.StopColliding && collider.gameObject.name.Equals("Character"))
		{
			ExecuteTutorialAction();
			if (allowHoverboard)
			{
				hoverboard.IsAllowed = true;
			}
			if (endTutorial)
			{
				Flurry.LogEvent("Tutorial exit");
				Game.Instance.IsRunningOnTutorialTrack = false;
				OnboardingSave.isTutorialCompleted = true;
				OnboardingTracking.EnableOnboardingTracking(OnboardingTracking.ScreenType.Gameover);
				OnboardingTracking.EnableOnboardingTracking(OnboardingTracking.ScreenType.FrontScreen);
			}
		}
	}

	private void ExecuteTutorialAction()
	{
		switch (_tutorialAction)
		{
		case TutorialAction.ShowArrow:
			StartCoroutine(ShowArrow());
			break;
		case TutorialAction.ShowHoverboardClickAnimation:
			HoverboardClickAnimationManager.CheckIfShouldShowClickAnimation();
			break;
		case TutorialAction.ShowText:
			UIScreenController.Instance.QueueMessage(GetStringForTutorial());
			break;
		default:
			Debug.LogError("Couldn't execute tutorial action : " + _tutorialAction);
			break;
		}
	}

	private string GetStringForTutorial()
	{
		string key = string.Empty;
		switch (tutorialText)
		{
		case TutorialText.FreshMoves:
			key = "TUTORIAL_FRESH_MOVES";
			break;
		case TutorialText.YouRock:
			key = "TUTORIAL_YOU_ROCK";
			break;
		}
		return Strings.Get(key);
	}
}
