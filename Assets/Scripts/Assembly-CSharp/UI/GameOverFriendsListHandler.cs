using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data.Save;
using Info;
using Kiloo.Common.Extensions;
using UI.FriendsList;
using UI.KScrollView;
using UI.Screens;
using UI.SlideIns;
using UnityEngine;

namespace UI
{
	public class GameOverFriendsListHandler : MonoBehaviour
	{
		private class FriendsListAdapter : ListAdapter<GameOverFriendsListCell>
		{
			private readonly GameObject _friendPrefab;

			private GameOverFriendsListHandler _listHandler;

			private const int FRIEND_PREFAB = 0;

			public FriendsListAdapter(List<GameOverFriendsListCell> contentList, GameObject newFriendPrefab, GameOverFriendsListHandler listHandler)
				: base(contentList)
			{
				_friendPrefab = newFriendPrefab;
				_listHandler = listHandler;
			}

			public override int GetModelType(int index)
			{
				FriendsListCellType cellType = GetDataItem(index).cellType;
				if (cellType == FriendsListCellType.Friend)
				{
					return 0;
				}
				return 0;
			}

			public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass)
			{
				Container container2 = container;
				GameOverFriendsListCell dataItem = GetDataItem(dataIndex);
				if (container2 == null)
				{
					FriendsListReferences friendsListReferences = new FriendsListReferences();
					switch (dataItem.cellType)
					{
					case FriendsListCellType.Friend:
					{
						container2 = new Container(_friendPrefab);
						GameOverCellHelper component5 = container2.prefabInstance.GetComponent<GameOverCellHelper>();
						friendsListReferences.cellHelper = component5;
						container2.controlComponent = friendsListReferences;
						break;
					}
					case FriendsListCellType.Dummy:
					{
						container2 = new Container(_friendPrefab);
						GameOverCellHelper component4 = container2.prefabInstance.GetComponent<GameOverCellHelper>();
						friendsListReferences.cellHelper = component4;
						container2.controlComponent = friendsListReferences;
						break;
					}
					case FriendsListCellType.Player:
					{
						container2 = new Container(_friendPrefab);
						GameOverCellHelper component3 = container2.prefabInstance.GetComponent<GameOverCellHelper>();
						friendsListReferences.cellHelper = component3;
						container2.controlComponent = friendsListReferences;
						break;
					}
					case FriendsListCellType.AddFriends:
					{
						container2 = new Container(_friendPrefab);
						GameOverCellHelper component2 = container2.prefabInstance.GetComponent<GameOverCellHelper>();
						friendsListReferences.cellHelper = component2;
						container2.controlComponent = friendsListReferences;
						break;
					}
					default:
					{
						SDebug.LogError("No cell type match for: " + dataItem.cellType);
						container2 = new Container(_friendPrefab);
						GameOverCellHelper component = container2.prefabInstance.GetComponent<GameOverCellHelper>();
						friendsListReferences.cellHelper = component;
						container2.controlComponent = friendsListReferences;
						break;
					}
					}
				}
				FriendsListReferences friendsListReferences2 = (FriendsListReferences)container2.controlComponent;
				switch (dataItem.cellType)
				{
				case FriendsListCellType.Friend:
					friendsListReferences2.cellHelper.InitFriend(dataItem.friend, dataItem.rank);
					break;
				case FriendsListCellType.Dummy:
					friendsListReferences2.cellHelper.InitDummy(dataItem.dummy);
					break;
				case FriendsListCellType.Player:
					friendsListReferences2.cellHelper.InitLocalUser(dataItem.rank, dataItem.isPlayerRanked);
					break;
				case FriendsListCellType.AddFriends:
					friendsListReferences2.cellHelper.InitAddFriends(_listHandler.FacebookClicked);
					break;
				default:
					SDebug.LogError("No cell type match for: " + dataItem.cellType);
					break;
				}
				return container2;
			}
		}

		[SerializeField]
		private GameObject _cellPrefab;

		[SerializeField]
		private ScrollView _scrollView;

		[SerializeField]
		private UIPanel _scrollViewPanel;

		[SerializeField]
		private AnimationCurve _scrollViewAnimationCurve;

		[SerializeField]
		private float _scrollViewAnimationTime;

		[SerializeField]
		private float _scrollViewAnimationStartDelay;

		private const int POSITION_OFFSET_IN_LIST = 2;

		private const int VISIBLE_ITEMS_IN_LIST = 6;

		private bool _initialized;

		private readonly List<GameOverFriendsListCell> _contentList = new List<GameOverFriendsListCell>();

		private FriendsListAdapter _adapter;

		private bool _facebookLoginRunning;

		private bool _hasPeekedList;

		private void Init()
		{
			SetListContent();
			if (_contentList != null)
			{
				_adapter = new FriendsListAdapter(_contentList, _cellPrefab, this);
			}
			_initialized = true;
		}

		public void Reset()
		{
			_hasPeekedList = false;
		}

		private void OnEnable()
		{
			if (!_initialized)
			{
				Init();
			}
			SocialManager.friendsChangedHandler += ReloadFriends;
			SocialManager.instance.UpdateFriendScores((bool didLoadFriends) =>
			{
				ReloadFriends();
			});
			GameOverScreen.onCountUpStarted += ScrollToMyPosition;
		}

		private void OnDisable()
		{
			SocialManager.friendsChangedHandler -= ReloadFriends;
			GameOverScreen.onCountUpStarted -= ScrollToMyPosition;
			if (_facebookLoginRunning)
			{
				HouseKeeper.SetFacebookPayoutActionToDefault();
				_facebookLoginRunning = false;
			}
		}

		public void ReloadFriends()
		{
			SetListContent();
			if (_scrollView != null && _adapter != null)
			{
				_scrollView.SetAdapter(_adapter);
			}
			if (Application.isEditor)
			{
				SetInitialListSelection();
				return;
			}
			LeaderboardScreenManager.LeaderboardScreenState screenState = LeaderboardScreenManager.Instance.screenState;
			if (screenState == LeaderboardScreenManager.LeaderboardScreenState.Facebook || screenState == LeaderboardScreenManager.LeaderboardScreenState.GameCenter || screenState == LeaderboardScreenManager.LeaderboardScreenState.FacebookAndGamecenter)
			{
				SetInitialListSelection();
			}
		}

		private void SetInitialListSelection()
		{
			int num = GetMyIndexInTheList() - 2;
			if (num < 0)
			{
				num = 0;
			}
			else if (num > _contentList.Count - 6)
			{
				num = _contentList.Count - 6;
			}
			Container firstVisibleContainerOnScreen = _scrollView.GetFirstVisibleContainerOnScreen();
			if (firstVisibleContainerOnScreen != null)
			{
				float offset = _scrollView.rootPanel.finalClipRegion.z * 0.5f - firstVisibleContainerOnScreen.width * 0.5f;
				_scrollView.SetSelection(num, ContainerPivot.Top, offset);
			}
		}

		private void ScrollToMyPosition()
		{
			int num = GetMyIndexInTheList() - 2;
			if (num < 0)
			{
				num = 0;
			}
			else if (num > _contentList.Count - 6)
			{
				num = _contentList.Count - 6;
			}
			Container firstVisibleContainerOnScreen = _scrollView.GetFirstVisibleContainerOnScreen();
			if (firstVisibleContainerOnScreen == null)
			{
				return;
			}
			bool flag = _contentList.Count - num > 6;
			float offset = _scrollView.rootPanel.finalClipRegion.z * 0.5f - firstVisibleContainerOnScreen.width * 0.5f;
			if (!_hasPeekedList && flag)
			{
				_hasPeekedList = true;
				_scrollView.SetSelection(num + 1, ContainerPivot.Top, offset);
				_scrollView.uiScrollView.enabled = false;
				StartCoroutine(PeekListAnimation(firstVisibleContainerOnScreen.width, _scrollViewAnimationTime, () =>
				{
					_scrollView.uiScrollView.enabled = true;
				}));
			}
			else if (!_hasPeekedList && num > 0)
			{
				_hasPeekedList = true;
				_scrollView.SetSelection(num - 1, ContainerPivot.Top, offset);
				_scrollView.uiScrollView.enabled = false;
				StartCoroutine(PeekListAnimation(0f - firstVisibleContainerOnScreen.width, _scrollViewAnimationTime, () =>
				{
					_scrollView.uiScrollView.enabled = true;
				}));
			}
		}

		private IEnumerator PeekListAnimation(float distance, float duration, Action onComplete)
		{
			yield return new WaitForSeconds(_scrollViewAnimationStartDelay);
			float time = 0f;
			Vector3 startPosition = _scrollView.transform.localPosition;
			Vector3 endPosition = startPosition + new Vector3(distance, 0f, 0f);
			Vector2 startPanelOffset = _scrollViewPanel.clipOffset;
			Vector2 endPanelOffset = startPanelOffset - new Vector2(distance, 0f);
			for (; time < duration; time += Time.deltaTime)
			{
				float lerpValue = _scrollViewAnimationCurve.Evaluate(time / duration);
				_scrollView.transform.localPosition = Vector3.LerpUnclamped(startPosition, endPosition, lerpValue);
				_scrollViewPanel.clipOffset = Vector2.LerpUnclamped(startPanelOffset, endPanelOffset, lerpValue);
				yield return null;
			}
			_scrollView.transform.localPosition = endPosition;
			_scrollViewPanel.clipOffset = endPanelOffset;
			onComplete.SafeInvoke();
		}

		private int GetMyIndexInTheList()
		{
			int result = -1;
			if (_contentList != null)
			{
				for (int i = 0; i < _contentList.Count; i++)
				{
					if (_contentList[i].cellType == FriendsListCellType.Player)
					{
						result = i;
					}
				}
			}
			return result;
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

		private void ResetFriendList()
		{
			_contentList.Clear();
		}

		private void SetAgeRestrictedContent()
		{
			SetPlayerAndDummies(true);
		}

		private void SetOfflineContent()
		{
			SetPlayerAndDummies();
		}

		private void SetPlayerAndDummies(bool isAgeRestricted = false)
		{
			int nElements = 0;
			int rankCount = 1;
			InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, false);
			if (!isAgeRestricted)
			{
				InsertAddFriendsToLeaderboard(ref nElements);
			}
			InsertDummiesToLeaderboard(LeaderboardDummyData.dummyData, ref nElements);
		}

		private void SetOnlineContent(LeaderboardScreenManager.LeaderboardScreenState screenState)
		{
			if (Application.isEditor)
			{
				Friend[] debugFriends = Globals.GetDebugFriends(30);
				SetOnlineContentListForLeaderboard(debugFriends, false);
				return;
			}
			Friend[] friends = SocialManager.instance.FriendsSortedByScore();
			switch (screenState)
			{
			case LeaderboardScreenManager.LeaderboardScreenState.Facebook:
			case LeaderboardScreenManager.LeaderboardScreenState.FacebookAndGamecenter:
				SetOnlineContentListForLeaderboard(friends, true);
				break;
			case LeaderboardScreenManager.LeaderboardScreenState.GameCenter:
				SetOnlineContentListForLeaderboard(friends, false);
				break;
			}
		}

		private void SetOnlineContentListForLeaderboard(Friend[] friends, bool isFbLogged)
		{
			bool flag = false;
			int nElements = 0;
			int rankCount = 1;
			int num = friends.Count();
			if (num > 0)
			{
				for (int i = 0; i < friends.Length; i++)
				{
					if (!flag && PlayerSave.highScore >= friends[i].score)
					{
						InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, true);
						flag = true;
						if (!isFbLogged)
						{
							InsertAddFriendsToLeaderboard(ref nElements);
						}
					}
					InsertFriendScoreToLeaderboard(friends[i], ref rankCount, ref nElements);
				}
				if (!flag)
				{
					InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, true);
				}
			}
			else
			{
				InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, true);
			}
		}

		private void InsertPlayerScoreToLeaderboard(ref int rankCount, ref int nElements, bool isRanked)
		{
			_contentList.Add(new GameOverFriendsListCell
			{
				cellType = FriendsListCellType.Player,
				rank = rankCount,
				isPlayerRanked = isRanked
			});
			if (isRanked)
			{
				rankCount++;
			}
			nElements++;
		}

		private void InsertFriendScoreToLeaderboard(Friend newFriend, ref int rankCount, ref int nElements)
		{
			_contentList.Add(new GameOverFriendsListCell
			{
				cellType = FriendsListCellType.Friend,
				friend = newFriend,
				rank = rankCount
			});
			rankCount++;
			nElements++;
		}

		private void InsertAddFriendsToLeaderboard(ref int nElements)
		{
			_contentList.Add(new GameOverFriendsListCell
			{
				cellType = FriendsListCellType.AddFriends
			});
			nElements++;
		}

		private void InsertDummiesToLeaderboard(List<LeaderboardDummyData.LeaderboardDummy> dummies, ref int nElements)
		{
			foreach (LeaderboardDummyData.LeaderboardDummy dummy in dummies)
			{
				_contentList.Add(new GameOverFriendsListCell
				{
					cellType = FriendsListCellType.Dummy,
					dummy = dummy
				});
				nElements++;
			}
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
				if (FacebookRewardsInfo.CanPayoutReward())
				{
					FacebookRewardsInfo.PayoutRewardIfPossible();
				}
				else
				{
					UIScreenController.Instance.QueueSlideIn(new SlideIn
					{
						type = SlideInData.SlideInType.Facebook,
						facebookState = UISlideInFacebook.FacebookSlideInState.ConnectSucceeded
					});
				}
			}
			else if (_facebookLoginRunning)
			{
				OnFacebookLoginResult(false);
			}
			HouseKeeper.SetFacebookPayoutActionToDefault();
			_facebookLoginRunning = false;
		}

		public void OnFacebookLoginResult(bool loggedIn)
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
