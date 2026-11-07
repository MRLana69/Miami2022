using System;
using System.Diagnostics;
using System.Threading;
using Data.Save;
using JetBrains.Annotations;
using Missions;
using UI;
using UnityEngine;

public class FriendHelperCrew : MonoBehaviour
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<int> OnFriendCoinsReceived__BackingField;

	[SerializeField]
	private GameObject _collectButton;

	[SerializeField]
	private UILabel _collectButtonLabel;

	[SerializeField]
	private GameObject _progress;

	[SerializeField]
	private UILabel _friendName;

	[SerializeField]
	private UITexture _friendPicture;

	[SerializeField]
	private UISprite _friendBackground;

	[SerializeField]
	private FriendCrewPokeHelper _pokeHelper;

	[SerializeField]
	private Texture2D _dummyImage;

	[SerializeField]
	private Texture2D _dummyFriendImage;

	[SerializeField]
	private UISprite _coinpouchSprite;

	private Friend _friend;

	private CollectCoinsDummy _dummyFriend;

	private const int RUNS_TO_COLLECT = 25;

	private const int REWARD_MIN = 250;

	private const int REWARD_MAX = 350;

	private readonly Color32 CELL_BACKGROUND_COLOR_CLEAR = new Color32(184, 215, 237, 127);

	private readonly Color32 CELL_BACKGROUND_COLOR_DARK = new Color32(120, 170, 206, 127);

	private readonly Vector3 COLLECT_POSITION = new Vector3(75f, -22f, 0f);

	private bool _isDummyFriend;

	private bool _hasAddedFriendUpdateHandler;

	public static event Action<int> OnFriendCoinsReceived
	{
		add
		{
			Action<int> action = OnFriendCoinsReceived__BackingField;
			Action<int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnFriendCoinsReceived__BackingField, (Action<int>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<int> action = OnFriendCoinsReceived__BackingField;
			Action<int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnFriendCoinsReceived__BackingField, (Action<int>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public void InitDummyFriend(CollectCoinsDummy dummyFriend, bool backgroundActive = false)
	{
		if (!SocialManager.instance.facebookIsLoggedIn)
		{
			_coinpouchSprite.spriteName = "ic_lock";
		}
		_isDummyFriend = true;
		_dummyFriend = dummyFriend;
		if (!backgroundActive)
		{
			_friendBackground.color = CELL_BACKGROUND_COLOR_CLEAR;
		}
		else
		{
			_friendBackground.color = CELL_BACKGROUND_COLOR_DARK;
		}
		if (!Strings.IsMapped(dummyFriend.name))
		{
			dummyFriend.AssignRandomDummyFriendName();
		}
		_friendName.text = Strings.Get(dummyFriend.name);
		_friendName.color = ColorData.instance.midnight;
		_friendPicture.mainTexture = _dummyFriendImage;
		_friendPicture.shader = Shader.Find("Unlit/Transparent Colored (AlphaClip)");
		if (!dummyFriend.hasBeenCollected)
		{
			_progress.SetActive(false);
			_collectButton.SetActive(true);
			_collectButtonLabel.text = Strings.Get("TOPRUN_COLLECT_BUTTON_TITLE");
			_collectButton.GetComponent<UIButtonMessage>().target = base.gameObject;
			_collectButton.transform.localPosition = COLLECT_POSITION;
			_pokeHelper.DeactivatePoke();
			return;
		}
		_progress.SetActive(true);
		_collectButton.SetActive(false);
		FriendProgressHelper component = _progress.GetComponent<FriendProgressHelper>();
		if (dummyFriend.fakeProgress > 25)
		{
			dummyFriend.fakeProgress -= 25;
		}
		component.label.text = string.Format(Strings.Get("TOPRUN_COLLECT_PROGRESS"), dummyFriend.fakeProgress, 25);
		component.slider.value = (float)dummyFriend.fakeProgress / 25f;
		_pokeHelper.DeactivatePoke();
	}

	public void InitFriend(Friend friend, bool backgroundActive = false)
	{
		if (!SocialManager.instance.facebookIsLoggedIn && !SocialManager.instance.gamecenterIsLoggedIn)
		{
			_coinpouchSprite.spriteName = "ic_lock";
		}
		_isDummyFriend = false;
		_friend = friend;
		if (!backgroundActive)
		{
			_friendBackground.color = CELL_BACKGROUND_COLOR_CLEAR;
		}
		else
		{
			_friendBackground.color = CELL_BACKGROUND_COLOR_DARK;
		}
		_friendName.text = _friend.name;
		_friendName.color = ColorData.instance.midnight;
		if (_friend.image != null)
		{
			_friendPicture.mainTexture = _friend.image;
		}
		else
		{
			_friendPicture.mainTexture = _dummyImage;
		}
		_friendPicture.shader = Shader.Find("Unlit/Transparent Colored (AlphaClip)");
		_friend.updated += OnFriendUpdatedHandler;
		_hasAddedFriendUpdateHandler = true;
		if (_friend.gamesToCashIn >= 25)
		{
			_progress.SetActive(false);
			_collectButton.SetActive(true);
			_collectButtonLabel.text = Strings.Get("TOPRUN_COLLECT_BUTTON_TITLE");
			_collectButton.GetComponent<UIButtonMessage>().target = base.gameObject;
			_collectButton.transform.localPosition = COLLECT_POSITION;
			_pokeHelper.DeactivatePoke();
			return;
		}
		_progress.SetActive(true);
		_collectButton.SetActive(false);
		FriendProgressHelper component = _progress.GetComponent<FriendProgressHelper>();
		component.label.text = string.Format(Strings.Get("TOPRUN_COLLECT_PROGRESS"), Mathf.Clamp(_friend.gamesToCashIn, 0, 25), 25);
		component.slider.value = (float)_friend.gamesToCashIn / 25f;
		if (friend.shouldBePoked)
		{
			_pokeHelper.ActivatePoke(friend);
			return;
		}
		if (_friend.status.lastPokeTime == DateTime.MinValue)
		{
			SocialManager.instance.SetPokeFirstTime(_friend);
		}
		_pokeHelper.DeactivatePoke();
	}

	[UsedImplicitly]
	public void CollectReward()
	{
		OnboardingSave.hasCollectedFromFriends = true;
		if (_isDummyFriend)
		{
			int num = PayoutAndReturnReward();
			_progress.SetActive(true);
			_collectButton.SetActive(false);
			FriendProgressHelper component = _progress.GetComponent<FriendProgressHelper>();
			component.label.text = string.Format(Strings.Get("TOPRUN_COLLECT_PROGRESS"), 0, 25);
			component.slider.value = 0f;
			UIScreenController.Instance.SpawnCollectText(component.GetCoinPouchGlobalPosition(), num.ToString());
			_dummyFriend.hasBeenCollected = true;
			SaveManager.Save();
		}
		else if (_friend != null)
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.CollectCoinPouch);
			SocialManager.instance.CollectFriendReward(_friend);
			int num2 = PayoutAndReturnReward();
			_progress.SetActive(true);
			_collectButton.SetActive(false);
			FriendProgressHelper component2 = _progress.GetComponent<FriendProgressHelper>();
			component2.label.text = string.Format(Strings.Get("TOPRUN_COLLECT_PROGRESS"), _friend.gamesToCashIn, 25);
			component2.slider.value = (float)_friend.gamesToCashIn / 25f;
			UIScreenController.Instance.SpawnCollectText(component2.GetCoinPouchGlobalPosition(), num2.ToString());
			SaveManager.Save();
			Flurry.LogEvent("Friend reward collect");
		}
		else
		{
			SDebug.LogError("FriendHelperCrew.CollectReward called for non-dummy friend, but _friend is not set", this);
		}
	}

	private int PayoutAndReturnReward()
	{
		int num = UnityEngine.Random.Range(250, 350);
		InventorySave.IncreaseCoins(num, "FriendReward");
		Stat.CoinsFriends.AddValue(num);
		if (OnFriendCoinsReceived__BackingField != null)
		{
			OnFriendCoinsReceived__BackingField(num);
		}
		return num;
	}

	public void Refresh()
	{
		OnFriendUpdatedHandler();
	}

	private void OnFriendUpdatedHandler()
	{
		if (_friend != null)
		{
			if (_friend.image != null)
			{
				_friendPicture.mainTexture = _friend.image;
			}
			if (!string.IsNullOrEmpty(_friend.name) && _friendName != null)
			{
				_friendName.text = _friend.name;
				_friendName.color = ColorData.instance.midnight;
			}
		}
		else
		{
			SDebug.LogError("FriendHelperCrew.OnFriendUpdatedHandler called but _friend is not set", this);
		}
	}

	private void OnDestroy()
	{
		if (_hasAddedFriendUpdateHandler)
		{
			if (_friend != null)
			{
				_friend.updated -= OnFriendUpdatedHandler;
				_hasAddedFriendUpdateHandler = false;
			}
			else
			{
				SDebug.LogError("FriendHelperCrew.OnDestroy called but _friend is not set and we have a handler to remove", this);
			}
		}
	}
}
