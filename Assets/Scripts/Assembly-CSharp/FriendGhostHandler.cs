using System;
using System.Collections.Generic;
using Data.Save;
using Info;
using Missions;
using SYBO.Subway;
using TopRun;
using UnityEngine;

public class FriendGhostHandler : MonoBehaviour
{
	private IManager _tournamentManager;

	public FriendGhostHelper helper;

	public Texture dummyImage;

	private Challenger[] challengersDescending;

	private Challenger currentChallenger;

	private List<IPlayer> topRunPlayers;

	private bool _localUserHighscoreInserted;

	private bool _localUserTopRunInserted;

	private bool _insertedAnyUser;

	private bool _passedLocalHighScore;

	private bool _passedLocalTopRunScore;

	private bool _gameRunning;

	private int _currentThreshold;

	private int _currentChallengerIndex = -1;

	private int _localHighestScore;

	private int _localTopRunScore;

	private int _numberOfCharactersInScore;

	private Color localPlayerColor = new Color(1f, 219f / 255f, 0f, 1f);

	private Color friendColor = Color.white;

	private Color topRunHighscoreTitleColor = Color.white;

	private Color normalHighScoreTitleColor = new Color(1f, 219f / 255f, 0f, 1f);

	private void Init()
	{
		Game.OnGameStarted += NewGame;
		Settings.onTopRunChallengerSettingChanged += RefreshListOnSettingsChanged;
	}

	private void OnDestroy()
	{
		Game.OnGameStarted -= NewGame;
		Settings.onTopRunChallengerSettingChanged -= RefreshListOnSettingsChanged;
	}

	private void Awake()
	{
		Init();
		NewGame();
	}

	public void NewGame()
	{
		_localHighestScore = PlayerSave.highScore;
		_gameRunning = true;
		_localUserHighscoreInserted = false;
		_localUserTopRunInserted = false;
		_passedLocalHighScore = false;
		_passedLocalTopRunScore = false;
		_currentThreshold = 0;
		GameStats.Instance.ResetGame();
		helper.GameOver();
		_tournamentManager = TopRunRoot.manager;
		if (_tournamentManager != null)
		{
			_localTopRunScore = _tournamentManager.score;
		}
		if (OnboardingSave.isTutorialCompleted)
		{
			if (Settings.optionTopRunChallengers && IsTopRunTournamentRunning())
			{
				challengersDescending = GetCombinedAndSortedChallengers(GetFriends(), GetTopRunPlayers());
			}
			else
			{
				challengersDescending = GetSortedFriends();
			}
			_currentChallengerIndex = getNextChallengerWithHigherScoreIndex(challengersDescending, GameStats.Instance.Score);
			helper.NewGame();
			SetNewChallenger();
			helper.AnimateIn();
		}
	}

	private bool IsTopRunTournamentRunning()
	{
		_tournamentManager = TopRunRoot.manager;
		if (_tournamentManager != null && _tournamentManager.tournament != null && !_tournamentManager.tournament.hasResult && !_tournamentManager.tournament.isFinished)
		{
			return true;
		}
		return false;
	}

	private void RefreshListOnSettingsChanged(bool settingValue)
	{
		_localUserHighscoreInserted = false;
		_localUserTopRunInserted = false;
		helper.Reset();
		if (settingValue && IsTopRunTournamentRunning())
		{
			challengersDescending = GetCombinedAndSortedChallengers(GetFriends(), GetTopRunPlayers());
		}
		else
		{
			challengersDescending = GetSortedFriends();
		}
		SetNewChallenger();
		helper.AnimateIn();
	}

	private Challenger[] GetFriends()
	{
		Friend[] array = new Friend[0];
		Globals.LogInState loginState = Globals.GetLoginState();
		bool flag = loginState == Globals.LogInState.Facebook || loginState == Globals.LogInState.Both;
		bool flag2 = (Application.platform == RuntimePlatform.IPhonePlayer || Application.isEditor) && (loginState == Globals.LogInState.Both || loginState == Globals.LogInState.GameCenter);
		bool flag3;
		if (Application.isEditor)
		{
			array = Globals.GetDebugFriends(2);
			flag3 = true;
		}
		else if (flag || flag2)
		{
			array = SocialManager.instance.FriendsSortedByScore();
			flag3 = false;
		}
		else
		{
			flag3 = false;
		}
		Challenger[] array2 = new Challenger[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			Challenger challenger = new Challenger();
			if (array[i].fbProfile != null)
			{
				challenger.ProfilePicture = array[i].fbProfile.image;
			}
			if (flag3)
			{
				challenger.ChallengerType = Challenger.ChallengerTypes.debugFriend;
			}
			else
			{
				challenger.ChallengerType = Challenger.ChallengerTypes.friend;
			}
			challenger.Score = array[i].score;
			array2[i] = challenger;
		}
		return array2;
	}

	private Challenger[] GetTopRunPlayers()
	{
		_tournamentManager = TopRunRoot.manager;
		List<IPlayer> list = new List<IPlayer>();
		if (_tournamentManager != null && _tournamentManager.tournament != null)
		{
			for (int i = 0; i < _tournamentManager.tournament.rankCount; i++)
			{
				list.AddRange(_tournamentManager.tournament.GetPlayersForRank(i));
			}
		}
		IPlayer[] array = list.ToArray();
		Challenger[] array2 = new Challenger[array.Length];
		for (int j = 0; j < array.Length; j++)
		{
			Challenger challenger = new Challenger();
			if (array[j].facebookProfile != null)
			{
				challenger.ProfilePicture = array[j].facebookProfile.image;
			}
			challenger.IsBothTopRunAndFacebookFriend = array[j].isFriend;
			challenger.Score = array[j].score;
			challenger.ChallengerType = Challenger.ChallengerTypes.topRunPlayer;
			array2[j] = challenger;
		}
		if (_tournamentManager != null)
		{
			array2 = Array.FindAll(array2, (Challenger element) => element.Score > _tournamentManager.score);
		}
		return array2;
	}

	private Challenger[] GetCombinedAndSortedChallengers(Challenger[] friends, Challenger[] topRuns)
	{
		Challenger[] array = new Challenger[friends.Length + topRuns.Length];
		Array.Copy(friends, 0, array, 0, friends.Length);
		Array.Copy(topRuns, 0, array, friends.Length, topRuns.Length);
		Array.Sort(array, (Challenger x, Challenger y) => y.Score - x.Score);
		return array;
	}

	private Challenger[] GetSortedFriends()
	{
		Challenger[] friends = GetFriends();
		Array.Sort(friends, (Challenger x, Challenger y) => y.Score - x.Score);
		return friends;
	}

	private int getNextChallengerWithHigherScoreIndex(Challenger[] sortedChallengerList, int thisPlayerScore)
	{
		if (sortedChallengerList == null)
		{
			return -1;
		}
		if (sortedChallengerList.Length == 0)
		{
			return -1;
		}
		for (int num = sortedChallengerList.Length - 1; num >= 0; num--)
		{
			if (sortedChallengerList[num].Score > thisPlayerScore)
			{
				return num;
			}
		}
		return -1;
	}

	private void Update()
	{
		FriendGhostHelper.AnimatingState currentAnimationState = helper.currentAnimationState;
		if (currentAnimationState == FriendGhostHelper.AnimatingState.AnimatingOut || currentAnimationState == FriendGhostHelper.AnimatingState.OffScreen || currentAnimationState == FriendGhostHelper.AnimatingState._notset || !_gameRunning || helper.noFriendsLeftToGhost || !OnboardingSave.isTutorialCompleted)
		{
			return;
		}
		if (GameStats.Instance.Score > _currentThreshold && currentAnimationState == FriendGhostHelper.AnimatingState.OnScreen)
		{
			helper.points.text = "0";
			PassThreshold();
		}
		else
		{
			if (!_insertedAnyUser)
			{
				return;
			}
			int score = GameStats.Instance.Score;
			if (_localUserHighscoreInserted && !_passedLocalHighScore)
			{
				if (_localHighestScore >= score)
				{
					helper.points.text = string.Empty + CalculateGapToFriendsScore(_localHighestScore, score);
				}
				else
				{
					_passedLocalHighScore = true;
				}
			}
			else if (_localUserTopRunInserted && !_passedLocalTopRunScore)
			{
				if (_localTopRunScore >= score)
				{
					helper.points.text = string.Empty + CalculateGapToFriendsScore(_localTopRunScore, score);
				}
				else
				{
					_passedLocalTopRunScore = true;
				}
			}
			else if (challengersDescending != null && _currentChallengerIndex != -1 && currentChallenger != null && currentChallenger.Score > score)
			{
				helper.points.text = string.Empty + CalculateGapToFriendsScore(currentChallenger.Score, score);
			}
			if (_numberOfCharactersInScore != helper.points.text.Length)
			{
				_numberOfCharactersInScore = helper.points.text.Length;
				ScaleHelperBackground();
			}
		}
	}

	private int CalculateGapToFriendsScore(int scoreTo, int currentScore)
	{
		return scoreTo - currentScore;
	}

	private void PassThreshold()
	{
		helper.AnimateOut();
	}

	public void FinishedAnimatingOut()
	{
		if (_gameRunning)
		{
			SetNewChallenger();
		}
	}

	public void SetNewChallenger()
	{
		if (!_gameRunning)
		{
			return;
		}
		_insertedAnyUser = false;
		if (challengersDescending != null || _currentChallengerIndex != -1)
		{
			int nextChallengerWithHigherScoreIndex = getNextChallengerWithHigherScoreIndex(challengersDescending, GameStats.Instance.Score);
			_currentChallengerIndex = nextChallengerWithHigherScoreIndex;
			if (_currentChallengerIndex == -1)
			{
				if (_localHighestScore > GameStats.Instance.Score)
				{
					if (InsertLocalUserTopRun())
					{
						_insertedAnyUser = true;
					}
					else if (InsertLocalUserHighScore())
					{
						_insertedAnyUser = true;
					}
				}
			}
			else
			{
				currentChallenger = challengersDescending[_currentChallengerIndex];
				if (_localTopRunScore < currentChallenger.Score)
				{
					if (InsertLocalUserTopRun())
					{
						_insertedAnyUser = true;
					}
					else if (InsertChallenger())
					{
						_insertedAnyUser = true;
					}
				}
				else if (_localHighestScore < currentChallenger.Score)
				{
					if (InsertLocalUserHighScore())
					{
						_insertedAnyUser = true;
					}
					else if (InsertChallenger())
					{
						_insertedAnyUser = true;
					}
				}
				else if (InsertChallenger())
				{
					_insertedAnyUser = true;
				}
			}
		}
		else if (!_localUserTopRunInserted)
		{
			if (InsertLocalUserTopRun())
			{
				_insertedAnyUser = true;
			}
		}
		else if (!_localUserHighscoreInserted && InsertLocalUserHighScore())
		{
			_insertedAnyUser = true;
		}
		if (_insertedAnyUser)
		{
			helper.AnimateIn();
		}
		else
		{
			helper.NoFriendsLeft();
		}
	}

	private bool InsertLocalUserHighScore()
	{
		if (_localHighestScore > GameStats.Instance.Score && !_localUserHighscoreInserted)
		{
			if (SocialManager.instance.localUserImage != null)
			{
				helper.picture.mainTexture = SocialManager.instance.localUserImage;
			}
			else
			{
				helper.picture.mainTexture = dummyImage;
			}
			helper.points.color = localPlayerColor;
			helper.points.text = _localHighestScore.ToString();
			helper.scoreType.color = normalHighScoreTitleColor;
			helper.scoreType.text = Strings.Get("INGAME_UI_HIGHSCORE");
			helper.competitorIcon.enabled = false;
			_localUserHighscoreInserted = true;
			_currentThreshold = _localHighestScore;
			return true;
		}
		return false;
	}

	private bool InsertLocalUserTopRun()
	{
		if (IsTopRunTournamentRunning() && _localTopRunScore > GameStats.Instance.Score && !_localUserTopRunInserted)
		{
			if (SocialManager.instance.localUserImage != null)
			{
				helper.picture.mainTexture = SocialManager.instance.localUserImage;
			}
			else
			{
				helper.picture.mainTexture = dummyImage;
			}
			helper.points.color = localPlayerColor;
			helper.points.text = _localTopRunScore.ToString();
			helper.scoreType.color = topRunHighscoreTitleColor;
			helper.scoreType.text = Strings.Get("INGAME_UI_TOPRUN");
			helper.competitorIcon.enabled = false;
			_localUserTopRunInserted = true;
			_currentThreshold = _localTopRunScore;
			return true;
		}
		return false;
	}

	private bool InsertChallenger()
	{
		int currentChallengerIndex = _currentChallengerIndex;
		if (currentChallengerIndex < 0 || currentChallengerIndex >= challengersDescending.Length)
		{
			return false;
		}
		Challenger challenger = challengersDescending[currentChallengerIndex];
		if (challenger.Score > GameStats.Instance.Score)
		{
			if (challenger.ChallengerType == Challenger.ChallengerTypes.topRunPlayer)
			{
				bool flag;
				if (challenger.ProfilePicture != null)
				{
					helper.picture.mainTexture = challenger.ProfilePicture;
					flag = true;
				}
				else
				{
					helper.picture.mainTexture = dummyImage;
					flag = false;
				}
				helper.scoreType.color = topRunHighscoreTitleColor;
				helper.scoreType.text = Strings.Get("INGAME_UI_TOPRUN");
				helper.competitorIcon.enabled = !challenger.IsBothTopRunAndFacebookFriend && flag;
			}
			else if (challenger.ChallengerType == Challenger.ChallengerTypes.friend)
			{
				if (challenger.ProfilePicture != null)
				{
					helper.picture.mainTexture = challenger.ProfilePicture;
				}
				else
				{
					helper.picture.mainTexture = dummyImage;
				}
				helper.scoreType.color = normalHighScoreTitleColor;
				helper.scoreType.text = Strings.Get("INGAME_UI_HIGHSCORE");
				helper.competitorIcon.enabled = false;
				MissionsManager.Instance.PlayerDidThis(MissionTarget.BeatFriends);
			}
			else
			{
				helper.picture.mainTexture = dummyImage;
				helper.scoreType.color = normalHighScoreTitleColor;
				helper.scoreType.text = Strings.Get("INGAME_UI_HIGHSCORE");
				helper.competitorIcon.enabled = false;
			}
			helper.points.color = friendColor;
			helper.points.text = challenger.Score.ToString();
			_currentThreshold = challenger.Score;
			return true;
		}
		return false;
	}

	private void ScaleHelperBackground()
	{
		float num = Mathf.Abs(helper.points.transform.localPosition.x);
		int defaultBackgroundWidth = helper.defaultBackgroundWidth;
		int num2 = Mathf.RoundToInt((float)helper.points.width + 3f * num);
		if (num2 > defaultBackgroundWidth)
		{
			helper.background.width = num2;
		}
		else
		{
			helper.background.width = helper.defaultBackgroundWidth;
		}
	}
}
