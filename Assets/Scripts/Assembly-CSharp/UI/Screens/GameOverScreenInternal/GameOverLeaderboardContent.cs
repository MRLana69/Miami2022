using System;
using System.Collections.Generic;
using Data.Save;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using UI.KScrollView;
using UI.Leaderboard;
using UI.SlideIns;
using UnityEngine;

namespace UI.Screens.GameOverScreenInternal
{
	public class GameOverLeaderboardContent : MonoBehaviour
	{
		private class LeaderboardListAdapter : ListAdapter<LeaderboardCell>
		{
			public class AdapterPayload
			{
				public List<LeaderboardCell> contentList;

				public GameObject friendPrefab;

				public GameObject playerPrefab;

				public GameObject spacePrefab;

				public GameObject headerPrefab;

				public Action<Transform> adjustOverlay;
			}

			private readonly AdapterPayload _payload;

			public LeaderboardListAdapter(AdapterPayload payload)
				: base(payload.contentList)
			{
				_payload = payload;
			}

			public override int GetModelType(int index)
			{
				return (int)GetDataItem(index).cellType;
			}

			public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass = false)
			{
				Container container2 = container;
				LeaderboardCell dataItem = GetDataItem(dataIndex);
				if (container2 == null)
				{
					LeaderboardCellReferences leaderboardCellReferences = new LeaderboardCellReferences();
					switch (dataItem.cellType)
					{
					case LeaderboardCellType.Friend:
					{
						container2 = new Container(_payload.friendPrefab);
						FriendHelperHighScore component5 = container2.prefabInstance.GetComponent<FriendHelperHighScore>();
						leaderboardCellReferences.friendHelper = component5;
						container2.controlComponent = leaderboardCellReferences;
						container2.prefabInstance.GetComponent<UIDragScrollView>().enabled = !dataItem.lockList;
						break;
					}
					case LeaderboardCellType.Player:
					{
						container2 = new Container(_payload.playerPrefab);
						FriendHelperHighScore component4 = container2.prefabInstance.GetComponent<FriendHelperHighScore>();
						leaderboardCellReferences.friendHelper = component4;
						container2.controlComponent = leaderboardCellReferences;
						container2.prefabInstance.GetComponent<UIDragScrollView>().enabled = !dataItem.lockList;
						break;
					}
					case LeaderboardCellType.DummyFriend:
					{
						container2 = new Container(_payload.friendPrefab);
						FriendHelperHighScore component3 = container2.prefabInstance.GetComponent<FriendHelperHighScore>();
						leaderboardCellReferences.friendHelper = component3;
						container2.controlComponent = leaderboardCellReferences;
						break;
					}
					case LeaderboardCellType.Space:
						container2 = new Container(_payload.spacePrefab);
						container2.prefabInstance.GetComponent<UIDragScrollView>().enabled = !dataItem.lockList;
						break;
					case LeaderboardCellType.Header:
					{
						container2 = new Container(_payload.headerPrefab);
						LeaderboardHeaderHelper component2 = container2.prefabInstance.GetComponent<LeaderboardHeaderHelper>();
						leaderboardCellReferences.headerHelper = component2;
						container2.controlComponent = leaderboardCellReferences;
						container2.prefabInstance.GetComponent<UIDragScrollView>().enabled = !dataItem.lockList;
						break;
					}
					default:
					{
						SDebug.LogError("No cell type match for: " + dataItem.cellType);
						container2 = new Container(_payload.friendPrefab);
						FriendHelperHighScore component = container2.prefabInstance.GetComponent<FriendHelperHighScore>();
						leaderboardCellReferences.friendHelper = component;
						container2.controlComponent = leaderboardCellReferences;
						break;
					}
					}
				}
				if (dataItem.adjustOverlay)
				{
					_payload.adjustOverlay.SafeInvoke(container2.modelTransform);
				}
				LeaderboardCellReferences leaderboardCellReferences2 = (LeaderboardCellReferences)container2.controlComponent;
				switch (dataItem.cellType)
				{
				case LeaderboardCellType.Friend:
					leaderboardCellReferences2.friendHelper.InitFriend(dataItem.friend, dataItem.rank, dataItem.useDarkColor);
					dataItem.OnUpdateCell = leaderboardCellReferences2.friendHelper.Refresh;
					break;
				case LeaderboardCellType.Player:
					leaderboardCellReferences2.friendHelper.InitLocalUser(dataItem.rank, dataItem.useDarkColor, dataItem.isPlayerRanked);
					dataItem.OnUpdateCell = leaderboardCellReferences2.friendHelper.Refresh;
					break;
				case LeaderboardCellType.DummyFriend:
					leaderboardCellReferences2.friendHelper.InitDummy(dataItem.dummyFriend, dataItem.rank, dataItem.useDarkColor);
					break;
				case LeaderboardCellType.Header:
					leaderboardCellReferences2.headerHelper.Setup(dataItem.headerText, dataItem.headerColor);
					break;
				default:
					SDebug.LogError("No cell type match for: " + dataItem.cellType);
					break;
				case LeaderboardCellType.Space:
					break;
				}
				return container2;
			}
		}

		[SerializeField]
		private GameObject _friendPrefab;

		[SerializeField]
		private FacebookLoginPromotion _facebookContent;

		[SerializeField]
		private GameObject _spacePrefab;

		[SerializeField]
		private GameObject _headerPrefab;

		[SerializeField]
		private UILabel _titleLabel;

		[SerializeField]
		private ScrollView _scrollView;

		[SerializeField]
		private UIPanel _scrollPanel;

		[SerializeField]
		private UIWidget _overlaySizer;

		[SerializeField]
		private GameObject _lockContent;

		private const int GC_FRIENDS_THRESHOLD = 4;

		private List<LeaderboardCell> _contentList = new List<LeaderboardCell>();

		private LeaderboardListAdapter _adapter;

		private bool _facebookLoginRunning;

		private const int POSITION_OFFSET_IN_LIST = 2;

		private bool _hasInitialised;

		private void Init()
		{
			_hasInitialised = true;
			SetListContent();
			if (_contentList != null)
			{
				_adapter = new LeaderboardListAdapter(new LeaderboardListAdapter.AdapterPayload
				{
					contentList = _contentList,
					friendPrefab = _friendPrefab,
					playerPrefab = _friendPrefab,
					spacePrefab = _spacePrefab,
					headerPrefab = _headerPrefab,
					adjustOverlay = AdjustLockOverlay
				});
			}
		}

		private void OnEnable()
		{
			if (!_hasInitialised)
			{
				Init();
			}
			SocialManager.friendsChangedHandler += ReloadFriends;
			UIScreenController.OnApplicationResumed += ReloadFriends;
			SocialManager.instance.UpdateFriendScores((bool didLoadFriends) =>
			{
				ReloadFriends();
			});
			ReloadFriends();
			_titleLabel.text = Strings.Get("HIGH_SCORES_TITLE");
			_titleLabel.color = ColorData.instance.midnight;
			CoroutineUtil.StartCoroutine(CoroutineUtil.ExecuteActionNextFrame(ReloadFriends));
		}

		private void OnDisable()
		{
			SocialManager.friendsChangedHandler -= ReloadFriends;
			UIScreenController.OnApplicationResumed -= ReloadFriends;
			if (_facebookLoginRunning)
			{
				HouseKeeper.SetFacebookPayoutActionToDefault();
				_facebookLoginRunning = false;
			}
		}

		private void ScrollToMyPosition()
		{
			int num = GetMyIndexInTheList() - 2;
			if (num > 0)
			{
				Container firstVisibleContainerOnScreen = _scrollView.GetFirstVisibleContainerOnScreen();
				if (firstVisibleContainerOnScreen != null)
				{
					float offset = _scrollView.rootPanel.baseClipRegion.w * 0.5f - firstVisibleContainerOnScreen.height * 0.5f;
					_scrollView.SetSelection(num, ContainerPivot.Top, offset);
				}
			}
		}

		private int GetMyIndexInTheList()
		{
			int result = -1;
			if (_contentList != null)
			{
				for (int i = 0; i < _contentList.Count; i++)
				{
					if (_contentList[i].cellType == LeaderboardCellType.Player)
					{
						result = i;
					}
				}
			}
			return result;
		}

		private void UpdateAnchors()
		{
			_scrollPanel.ResetAndUpdateAnchors();
		}

		private void ReloadFriends()
		{
			SetListContent();
			UpdateAnchors();
			if (_scrollView != null && _adapter != null)
			{
				_scrollView.SetAdapter(_adapter);
			}
			_overlaySizer.ResetAnchors();
			LeaderboardScreenManager.LeaderboardScreenState screenState = LeaderboardScreenManager.Instance.screenState;
			Friend[] array = ((!Application.isEditor) ? SocialManager.instance.FriendsSortedByScore() : Globals.GetDebugFriends(1));
			bool flag = screenState == LeaderboardScreenManager.LeaderboardScreenState.Facebook || screenState == LeaderboardScreenManager.LeaderboardScreenState.FacebookAndGamecenter;
			if (((screenState == LeaderboardScreenManager.LeaderboardScreenState.GameCenter && array.Length >= 4) || flag) && screenState != LeaderboardScreenManager.LeaderboardScreenState.Offline)
			{
				ScrollToMyPosition();
			}
		}

		private void ResetFriendList()
		{
			_contentList.Clear();
		}

		private void SetListContent()
		{
			ResetFriendList();
			LeaderboardScreenManager.LeaderboardScreenState screenState = LeaderboardScreenManager.Instance.screenState;
			switch (screenState)
			{
			case LeaderboardScreenManager.LeaderboardScreenState.Offline:
				SetOfflineContent();
				break;
			case LeaderboardScreenManager.LeaderboardScreenState.Facebook:
			case LeaderboardScreenManager.LeaderboardScreenState.GameCenter:
			case LeaderboardScreenManager.LeaderboardScreenState.FacebookAndGamecenter:
				SetOnlineContent(screenState);
				break;
			case LeaderboardScreenManager.LeaderboardScreenState.AgeRestricted:
				SetAgeRestrictedContent();
				break;
			default:
				SetOfflineContent();
				break;
			}
		}

		private void SetAgeRestrictedContent()
		{
			int nElements = 0;
			int rankCount = 1;
			InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, false, false, false);
			SetActiveFacebookLoginButton(false);
			AddOverlayAndLockList(false);
		}

		private void SetOfflineContent()
		{
			int nElements = 0;
			int rankCount = 1;
			InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, false, true, true);
			SetActiveFacebookLoginButton(true);
			AddDummiesToList(ref rankCount, ref nElements);
			AddOverlayAndLockList(true);
		}

		private void SetOnlineContent(LeaderboardScreenManager.LeaderboardScreenState screenState)
		{
			Friend[] array = ((!Application.isEditor) ? SocialManager.instance.FriendsSortedByScore() : Globals.GetDebugFriends(30));
			bool lockList = screenState == LeaderboardScreenManager.LeaderboardScreenState.GameCenter && array.Length < 4;
			bool facebookButtonActive = false;
			switch (screenState)
			{
			case LeaderboardScreenManager.LeaderboardScreenState.Facebook:
			case LeaderboardScreenManager.LeaderboardScreenState.FacebookAndGamecenter:
				SetOnlineContentListForLeaderboard(true, array, lockList, ref facebookButtonActive);
				break;
			case LeaderboardScreenManager.LeaderboardScreenState.GameCenter:
				SetOnlineContentListForLeaderboard(false, array, lockList, ref facebookButtonActive);
				break;
			}
			SetActiveFacebookLoginButton(facebookButtonActive);
		}

		private void SetOnlineContentListForLeaderboard(bool fbConnected, Friend[] friends, bool lockList, ref bool facebookButtonActive)
		{
			bool flag = false;
			int nElements = 0;
			int rankCount = 1;
			int num = friends.Length;
			bool flag2 = !fbConnected && friends.Length >= 4;
			bool flag3 = !fbConnected && friends.Length < 4;
			if (num > 0)
			{
				if (!fbConnected)
				{
					AddGameCenterHeaderToList(lockList);
				}
				foreach (Friend friend in friends)
				{
					if (!flag && PlayerSave.highScore >= friend.score)
					{
						InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, true, lockList, false);
						flag = true;
						if (flag2)
						{
							facebookButtonActive = true;
						}
					}
					InsertFriendScoreToLeaderboard(friend, ref rankCount, ref nElements, lockList);
				}
				if (!flag)
				{
					InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, true, lockList, false);
					if (flag2)
					{
						facebookButtonActive = true;
					}
				}
				if (flag3)
				{
					facebookButtonActive = true;
					AddSpaceToList(lockList);
					AddSpaceToList(lockList);
					AddFacebookHeaderToList(lockList);
					AddDummiesToList(ref rankCount, ref nElements);
					AddOverlayAndLockList(true);
				}
				else
				{
					AddOverlayAndLockList(false);
				}
				AddSpaceToList(lockList);
			}
			else
			{
				InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, true, true, true);
				if (!fbConnected)
				{
					facebookButtonActive = true;
					AddDummiesToList(ref rankCount, ref nElements);
					AddOverlayAndLockList(true);
				}
				else
				{
					AddOverlayAndLockList(false);
				}
			}
		}

		private void InsertPlayerScoreToLeaderboard(ref int rankCount, ref int nElements, bool isRanked, bool listLocked, bool adjustOverlay)
		{
			_contentList.Add(new LeaderboardCell
			{
				cellType = LeaderboardCellType.Player,
				rank = rankCount,
				useDarkColor = (nElements % 2 == 0),
				isPlayerRanked = isRanked,
				lockList = listLocked,
				adjustOverlay = adjustOverlay
			});
			if (isRanked)
			{
				rankCount++;
			}
			nElements++;
		}

		private void InsertFriendScoreToLeaderboard(Friend newFriend, ref int rankCount, ref int nElements, bool listLocked)
		{
			_contentList.Add(new LeaderboardCell
			{
				cellType = LeaderboardCellType.Friend,
				friend = newFriend,
				useDarkColor = (nElements % 2 == 0),
				rank = rankCount,
				lockList = listLocked
			});
			rankCount++;
			nElements++;
		}

		private void AddSpaceToList(bool listLocked)
		{
			_contentList.Add(new LeaderboardCell
			{
				cellType = LeaderboardCellType.Space,
				lockList = listLocked
			});
		}

		private void AddGameCenterHeaderToList(bool lockList)
		{
			_contentList.Add(new LeaderboardCell
			{
				cellType = LeaderboardCellType.Header,
				headerText = Strings.Get("LEADERBOARD_HEADER_GAMECENTER"),
				headerColor = ColorData.instance.bulbasaur,
				lockList = lockList
			});
		}

		private void AddFacebookHeaderToList(bool lockList)
		{
			_contentList.Add(new LeaderboardCell
			{
				cellType = LeaderboardCellType.Space,
				lockList = lockList,
				adjustOverlay = true
			});
			_contentList.Add(new LeaderboardCell
			{
				cellType = LeaderboardCellType.Header,
				headerText = Strings.Get("LEADERBOARD_HEADER_FACEBOOK"),
				headerColor = ColorData.instance.ocean,
				lockList = lockList
			});
		}

		private void SetActiveFacebookLoginButton(bool active)
		{
			_facebookContent.gameObject.SetActive(active);
			if (active)
			{
				_facebookContent.Init("LeaderboardUI", FacebookClicked);
			}
		}

		private void AddDummiesToList(ref int rankCount, ref int nElements)
		{
			List<LeaderboardDummyData.LeaderboardDummy> dummyData = LeaderboardDummyData.dummyData;
			if (dummyData == null)
			{
				return;
			}
			foreach (LeaderboardDummyData.LeaderboardDummy item in dummyData)
			{
				_contentList.Add(new LeaderboardCell
				{
					cellType = LeaderboardCellType.DummyFriend,
					dummyFriend = item,
					rank = rankCount,
					useDarkColor = (nElements % 2 == 0)
				});
				rankCount++;
				nElements++;
			}
		}

		private void AddOverlayAndLockList(bool enable)
		{
			_lockContent.SetActive(enable);
		}

		private void AdjustLockOverlay(Transform obj)
		{
			_overlaySizer.topAnchor.target = obj;
		}

		private void FacebookClicked()
		{
			HouseKeeper.SetFacebookPayoutAction(FacebookStatusChanged);
			_facebookLoginRunning = true;
			if (SocialManager.instance.FacebookLogin(OnFacebookLoginResult))
			{
				UIScreenController.Instance.FacebookLogIn(true);
			}
		}

		private void FacebookStatusChanged(bool success)
		{
			if (success)
			{
				if (!UserSave.hasPaidOutFacebookReward)
				{
					UIScreenController.Instance.PushPopup("FBPromotionPopup");
				}
				else
				{
					UIScreenController.Instance.QueueSlideIn(new SlideIn
					{
						type = SlideInData.SlideInType.Facebook,
						facebookState = UISlideInFacebook.FacebookSlideInState.ConnectSucceeded
					});
				}
				ReloadFriends();
			}
			else if (_facebookLoginRunning)
			{
				OnFacebookLoginResult(false);
			}
			HouseKeeper.SetFacebookPayoutActionToDefault();
			_facebookLoginRunning = false;
		}

		private void OnFacebookLoginResult(bool loggedIn)
		{
			if (!loggedIn && _facebookLoginRunning)
			{
				_facebookLoginRunning = false;
				QueueFacebookFailSlideIn(OnFailSlideInFinish);
			}
		}

		private void QueueFacebookFailSlideIn(Action onFinishCallback)
		{
			UIScreenController.Instance.QueueSlideIn(new SlideIn
			{
				type = SlideInData.SlideInType.Facebook,
				facebookState = UISlideInFacebook.FacebookSlideInState.ConnectFailed,
				onDidShowCallback = onFinishCallback
			});
		}

		private void OnFailSlideInFinish()
		{
			FacebookStatusChanged(false);
		}
	}
}
