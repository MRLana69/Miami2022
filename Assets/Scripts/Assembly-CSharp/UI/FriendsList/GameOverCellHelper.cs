using System;
using Data.Save;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace UI.FriendsList
{
	public class GameOverCellHelper : MonoBehaviour
	{
		[Header("Containers")]
		[SerializeField]
		private GameObject _normalFriendContainer;

		[SerializeField]
		private GameObject _addFriendsContainer;

		[Header("Friend")]
		[SerializeField]
		private UISprite _cellBackground;

		[SerializeField]
		private UITexture _friendPicture;

		[SerializeField]
		private UISprite _rankBackground;

		[SerializeField]
		private UILabel _rankNumber;

		[SerializeField]
		private UILabel _friendName;

		[SerializeField]
		private UILabel _friendScore;

		[SerializeField]
		private UISprite _dummyImage;

		[SerializeField]
		private UISprite _dummyBackground;

		[Header("Add Friends")]
		[SerializeField]
		private UILabel _addFriendsLabel;

		private const string DUMMY_SCORE_PLACEHOLDER = "--";

		private bool _isLocalUser;

		private bool _isDummy;

		private Friend _friend;

		private bool _initialized;

		private Action _onAddFriendsClickedAction;

		public void InitLocalUser(int ranking, bool isPlayerRanked = true)
		{
			ResetCell();
			ColorData instance = ColorData.instance;
			_isLocalUser = true;
			_cellBackground.color = instance.midnight;
			_friendName.text = SocialManager.instance.localDisplayName;
			_friendName.color = instance.white;
			_friendScore.text = PlayerSave.highScore.ToString();
			_friendScore.color = instance.white;
			_rankNumber.text = ranking.ToString();
			_rankBackground.gameObject.SetActive(isPlayerRanked);
			_rankNumber.gameObject.SetActive(isPlayerRanked);
			UpdateCellPicture();
			_friendPicture.shader = Shader.Find("Unlit/Transparent Colored (AlphaClip)");
			SocialManager.instance.AddLocalUserUpdatedHandler(OnLocalUserUpdated);
			_initialized = true;
		}

		public void InitFriend(Friend friend, int ranking)
		{
			ResetCell();
			ColorData instance = ColorData.instance;
			_friend = friend;
			_cellBackground.color = instance.pigeon;
			_friendName.text = _friend.name;
			_friendName.color = instance.ocean;
			_friendScore.text = ((!_isDummy) ? friend.score.ToString() : "--");
			_friendScore.color = instance.midnight;
			_rankNumber.text = ranking.ToString();
			_rankBackground.gameObject.SetActive(!_isDummy);
			_rankNumber.gameObject.SetActive(!_isDummy);
			UpdateCellPicture();
			_friend.updated += OnFriendUpdatedHandler;
			_initialized = true;
		}

		public void InitDummy(LeaderboardDummyData.LeaderboardDummy dummy)
		{
			ResetCell();
			ColorData instance = ColorData.instance;
			_isDummy = true;
			_cellBackground.color = instance.pigeon;
			_friendName.text = Strings.Get(dummy.name);
			_friendName.color = instance.ocean;
			_friendScore.text = "--";
			_friendScore.color = instance.midnight;
			_rankBackground.gameObject.SetActive(false);
			_rankNumber.gameObject.SetActive(false);
			UpdateCellPicture();
			_initialized = true;
		}

		public void InitAddFriends(Action onClickAction)
		{
			ResetCell();
			_normalFriendContainer.SetActive(false);
			_addFriendsContainer.SetActive(true);
			_onAddFriendsClickedAction = onClickAction;
		}

		[UsedImplicitly]
		public void OnAddFriendsClicked()
		{
			_onAddFriendsClickedAction.SafeInvoke();
		}

		private void UpdateCellPicture()
		{
			if (_isLocalUser)
			{
				if (SocialManager.instance.localUserImage != null)
				{
					_friendPicture.mainTexture = SocialManager.instance.localUserImage;
					ShowDummyPicture(false);
				}
				else
				{
					ShowDummyPicture(true);
				}
			}
			else if (!_isDummy)
			{
				if (_friend != null && _friend.image != null)
				{
					_friendPicture.mainTexture = _friend.image;
					ShowDummyPicture(false);
				}
				else
				{
					ShowDummyPicture(true);
				}
			}
			else
			{
				ShowDummyPicture(true);
			}
		}

		private void ShowDummyPicture(bool showDummy)
		{
			_friendPicture.gameObject.SetActive(!showDummy);
			_dummyImage.gameObject.SetActive(showDummy);
			_dummyBackground.gameObject.SetActive(showDummy);
		}

		private void ResetCell()
		{
			RemoveDelegatesIfNecessary();
			_addFriendsLabel.text = Strings.Get("GAMEOVER_ADD_FRIENDS");
			_isLocalUser = false;
			_isDummy = false;
			_initialized = false;
			_onAddFriendsClickedAction = null;
			_normalFriendContainer.SetActive(true);
			_addFriendsContainer.SetActive(false);
		}

		private void OnDestroy()
		{
			RemoveDelegatesIfNecessary();
		}

		private void RemoveDelegatesIfNecessary()
		{
			if (_initialized)
			{
				if (_isLocalUser)
				{
					SocialManager.instance.RemoveLocalUserUpdatedHandler(OnLocalUserUpdated);
				}
				else if (!_isDummy)
				{
					_friend.updated -= OnFriendUpdatedHandler;
				}
			}
		}

		private void OnLocalUserUpdated()
		{
			if (SocialManager.instance.localUserImage != null)
			{
				_friendPicture.mainTexture = SocialManager.instance.localUserImage;
			}
		}

		private void OnFriendUpdatedHandler()
		{
			UpdateCellPicture();
		}
	}
}
