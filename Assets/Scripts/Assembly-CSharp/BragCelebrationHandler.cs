using System.Collections;
using System.Collections.Generic;
using Data.Save;
using Kiloo.Common.Extensions;
using UnityEngine;
using Utilities;

public class BragCelebrationHandler : MonoBehaviour
{
	public enum State
	{
		NotCalculated = 0,
		BeatenOnlySelf = 1,
		BeatenFriendsOnlyGcOnline = 2,
		BeatenFriendsOnlineAll = 3
	}

	[SerializeField]
	private GameObject _noFriendsStateGo;

	[SerializeField]
	private GameObject _gcFriendsStateGo;

	[SerializeField]
	private GameObject _allFriendsStateGo;

	[SerializeField]
	private GameObject _friendPicture;

	[SerializeField]
	private UILabel _newHighScoreTopTitle;

	[SerializeField]
	private UILabel _highScoreLabel;

	[SerializeField]
	private UILabel _justBeatLabel;

	[SerializeField]
	private UILabel _firstBeatenFriendLabel;

	[SerializeField]
	private UILabel _beatenFriendsLeftLabel;

	[SerializeField]
	private UILabel _bragButtonTitle;

	[SerializeField]
	private UILabel _competeWithFriendsButtonTitle;

	[SerializeField]
	private UITexture _friendPictureTexture;

	[SerializeField]
	private GameObject _bragGo;

	private Friend __bestSurpassedFriend;

	private List<Friend> __surpassedFriends;

	private State _socialState;

	private Friend _bestSurpassedFriend
	{
		get
		{
			if (__bestSurpassedFriend == null)
			{
				foreach (Friend surpassedFriend in _surpassedFriends)
				{
					if (__bestSurpassedFriend == null || (__bestSurpassedFriend != null && surpassedFriend.score > __bestSurpassedFriend.score))
					{
						__bestSurpassedFriend = surpassedFriend;
					}
				}
			}
			return __bestSurpassedFriend;
		}
	}

	private List<Friend> _surpassedFriends
	{
		get
		{
			if (__surpassedFriends == null)
			{
				__surpassedFriends = new List<Friend>();
				Friend[] array = ((!Application.isEditor) ? SocialManager.instance.FriendsSortedByScore() : Globals.GetDebugFriends(50));
				int bragScoreThreshold = PlayerSave.bragScoreThreshold;
				int highScore = PlayerSave.highScore;
				Friend[] array2 = array;
				foreach (Friend friend in array2)
				{
					if (friend.score >= bragScoreThreshold && friend.score < highScore)
					{
						__surpassedFriends.Add(friend);
					}
				}
			}
			return __surpassedFriends;
		}
	}

	public State state
	{
		get
		{
			if (_socialState == State.NotCalculated)
			{
				CalculateState();
			}
			return _socialState;
		}
	}

	private void OnValidate()
	{
		if (_noFriendsStateGo == null || _gcFriendsStateGo == null || _allFriendsStateGo == null || _newHighScoreTopTitle == null || _highScoreLabel == null || _justBeatLabel == null || _firstBeatenFriendLabel == null || _beatenFriendsLeftLabel == null || _bragButtonTitle == null || _competeWithFriendsButtonTitle == null || _friendPictureTexture == null || _bragGo == null)
		{
			SDebug.LogError("Set all variables in editor from BragCelebrationHandler prefab", this);
		}
	}

	private void Awake()
	{
		if (SafeArea.IsRestricted)
		{
			float posY = _newHighScoreTopTitle.transform.localPosition.y - (float)SafeArea.OFFSET_TOP;
			_newHighScoreTopTitle.transform.SetLocalPosY(posY);
			float posY2 = _highScoreLabel.transform.localPosition.y - (float)SafeArea.OFFSET_TOP;
			_highScoreLabel.transform.SetLocalPosY(posY2);
		}
		_newHighScoreTopTitle.alpha = 0f;
		_highScoreLabel.alpha = 0f;
		_justBeatLabel.alpha = 0f;
		_competeWithFriendsButtonTitle.text = Strings.Get("BRAG_CELEBRATION_COMPETE_WITH_FRIENDS_BUTTON");
		_bragButtonTitle.text = Strings.Get("BRAG_CELEBRATION_BRAG_BUTTON");
		_friendPicture.SetActive(false);
		_noFriendsStateGo.SetActive(false);
		_gcFriendsStateGo.SetActive(false);
		_allFriendsStateGo.SetActive(false);
		StartCoroutine(UpdateUI());
	}

	private void Setup()
	{
		_friendPicture.SetActive(true);
		switch (state)
		{
		case State.BeatenOnlySelf:
			_friendPicture.SetActive(false);
			SetStateGameObjectActive(_noFriendsStateGo);
			break;
		case State.BeatenFriendsOnlyGcOnline:
			SetStateGameObjectActive(_gcFriendsStateGo);
			break;
		case State.BeatenFriendsOnlineAll:
			SetStateGameObjectActive(_allFriendsStateGo);
			break;
		}
	}

	private IEnumerator UpdateUI()
	{
		if (_bestSurpassedFriend != null)
		{
			_firstBeatenFriendLabel.text = _bestSurpassedFriend.name;
			if (!Application.isEditor)
			{
				SocialManager.instance.DownloadLargePictureForUser(_bestSurpassedFriend, OnLargePictureDownloaded);
			}
		}
		_newHighScoreTopTitle.text = Strings.Get("BRAG_CELEBRATION_NEW_HIGHSCORE");
		_highScoreLabel.text = PlayerSave.highScore.ToString();
		_highScoreLabel.alpha = 1f;
		_newHighScoreTopTitle.alpha = 1f;
		yield return new WaitForSeconds(1f);
		if (state != State.BeatenOnlySelf)
		{
			_justBeatLabel.text = Strings.Get("BRAG_CELEBRATION_JUST_BEAT");
			_justBeatLabel.alpha = 1f;
			AnimateUIGameObjectAlpha(_friendPicture, 0f, 0f);
			_friendPicture.SetActive(true);
			AnimateUIGameObjectAlpha(_friendPicture, 0.5f, 1f);
			if (_surpassedFriends.Count > 1)
			{
				_beatenFriendsLeftLabel.gameObject.SetActive(true);
				_beatenFriendsLeftLabel.text = string.Format(Strings.Get("BRAG_CELEBRATION_FRIENDS_BEAT_LEFT"), _surpassedFriends.Count - 1);
			}
			else
			{
				_beatenFriendsLeftLabel.gameObject.SetActive(false);
			}
			if (!SocialManager.facebookDialogsSupported)
			{
				_bragGo.SetActive(false);
			}
		}
		Setup();
	}

	private void CalculateState()
	{
		SocialManager instance = SocialManager.instance;
		if (Application.isEditor)
		{
			if (Globals.DEBUG_USE_DEBUG_LOGIN_STATE)
			{
				_socialState = ((_bestSurpassedFriend == null) ? State.BeatenOnlySelf : State.BeatenFriendsOnlineAll);
			}
			else
			{
				_socialState = State.BeatenOnlySelf;
			}
		}
		else if (instance.facebookIsLoggedIn)
		{
			_socialState = ((_bestSurpassedFriend == null) ? State.BeatenOnlySelf : State.BeatenFriendsOnlineAll);
		}
		else if (instance.gamecenterIsLoggedIn)
		{
			_socialState = ((_bestSurpassedFriend == null) ? State.BeatenOnlySelf : State.BeatenFriendsOnlyGcOnline);
		}
		else
		{
			_socialState = State.BeatenOnlySelf;
		}
	}

	private void SetStateGameObjectActive(GameObject go)
	{
		_noFriendsStateGo.SetActive(false);
		_gcFriendsStateGo.SetActive(false);
		_allFriendsStateGo.SetActive(false);
		go.SetActive(true);
	}

	private void AnimateUIGameObjectAlpha(GameObject go, float duration, float toAlpha)
	{
		UIWidget[] componentsInChildren = go.GetComponentsInChildren<UIWidget>(true);
		UIWidget[] array = componentsInChildren;
		foreach (UIWidget label in array)
		{
			StartCoroutine(AnimateWidgetAlpha(label, duration, toAlpha));
		}
	}

	private IEnumerator AnimateWidgetAlpha(UIWidget label, float duration, float toAlpha)
	{
		float fromAlpha = label.alpha;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			label.alpha = Mathf.Lerp(fromAlpha, toAlpha, factor);
			yield return null;
		}
	}

	private void OnLargePictureDownloaded(Friend friend, Texture2D picture)
	{
		if (_friendPictureTexture != null)
		{
			_friendPictureTexture.mainTexture = picture;
		}
	}

	private void BragBtnClicked()
	{
		if (_surpassedFriends != null && _surpassedFriends.Count > 0)
		{
			SocialManager.instance.BragFacebook(_surpassedFriends);
		}
	}

	private void LoginToFacebookBtnClicked()
	{
		SocialManager.instance.FacebookLogin(null);
	}
}
