using System;
using System.Collections.Generic;
using System.Linq;
using Data.Save;
using Info;
using UI.KScrollView;
using UI.SlideIns;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace UI.Screens
{
	public class FriendsOnlineScreen : UIScreen
	{
		private class CollectCoinsListAdapter : ListAdapter<CollectCoinsCell>
		{
			private readonly GameObject _friendPrefab;

			private readonly GameObject _recommendPrefab;

			private readonly GameObject _spacePrefab;

			private const int FRIEND_PREFAB = 0;

			private const int FACEBOOK_PREFAB = 1;

			private const int RECOMMEND_PREFAB = 2;

			private const int SPACE_PREFAB = 3;

			public CollectCoinsListAdapter(List<CollectCoinsCell> contentList, GameObject newFriendPrefab, GameObject newRecommendPrefab, GameObject newSpacePrefab)
				: base(contentList)
			{
				_friendPrefab = newFriendPrefab;
				_recommendPrefab = newRecommendPrefab;
				_spacePrefab = newSpacePrefab;
			}

			public override int GetModelType(int index)
			{
				switch (GetDataItem(index).cellType)
				{
				case CollectCoinsCellType.FRIEND:
					return 0;
				case CollectCoinsCellType.DUMMYFRIEND:
					return 0;
				case CollectCoinsCellType.FACEBOOK_BUTTON:
					return 1;
				case CollectCoinsCellType.RECOMMEND_BUTTON:
					return 2;
				case CollectCoinsCellType.SPACE:
					return 3;
				default:
					return 0;
				}
			}

			public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass = false)
			{
				Container container2 = container;
				CollectCoinsCell dataItem = GetDataItem(dataIndex);
				if (container2 == null)
				{
					CollectCoinsCellReferences collectCoinsCellReferences = new CollectCoinsCellReferences();
					switch (dataItem.cellType)
					{
					case CollectCoinsCellType.FRIEND:
					{
						container2 = new Container(_friendPrefab);
						FriendHelperCrew component3 = container2.prefabInstance.GetComponent<FriendHelperCrew>();
						collectCoinsCellReferences.friendHelper = component3;
						container2.controlComponent = collectCoinsCellReferences;
						container2.prefabInstance.GetComponent<UIDragScrollView>().enabled = !dataItem.disableScrolling;
						break;
					}
					case CollectCoinsCellType.DUMMYFRIEND:
					{
						container2 = new Container(_friendPrefab);
						FriendHelperCrew component2 = container2.prefabInstance.GetComponent<FriendHelperCrew>();
						collectCoinsCellReferences.friendHelper = component2;
						container2.controlComponent = collectCoinsCellReferences;
						break;
					}
					case CollectCoinsCellType.RECOMMEND_BUTTON:
						container2 = new Container(_recommendPrefab);
						break;
					case CollectCoinsCellType.SPACE:
						container2 = new Container(_spacePrefab);
						break;
					default:
					{
						SDebug.LogError("No cell type match for: " + dataItem.cellType);
						container2 = new Container(_friendPrefab);
						FriendHelperCrew component = container2.prefabInstance.GetComponent<FriendHelperCrew>();
						collectCoinsCellReferences.friendHelper = component;
						container2.controlComponent = collectCoinsCellReferences;
						break;
					}
					}
				}
				CollectCoinsCellReferences collectCoinsCellReferences2 = (CollectCoinsCellReferences)container2.controlComponent;
				switch (dataItem.cellType)
				{
				case CollectCoinsCellType.FRIEND:
				{
					collectCoinsCellReferences2.friendHelper.InitFriend(dataItem.friend, dataItem.useDarkColour);
					dataItem.OnUpdateCell = collectCoinsCellReferences2.friendHelper.Refresh;
					UIDragScrollView component4 = container2.prefabInstance.GetComponent<UIDragScrollView>();
					component4.enabled = !dataItem.disableScrolling;
					break;
				}
				case CollectCoinsCellType.DUMMYFRIEND:
					collectCoinsCellReferences2.friendHelper.InitDummyFriend(dataItem.dummyFriend, dataItem.useDarkColour);
					dataItem.OnUpdateCell = collectCoinsCellReferences2.friendHelper.Refresh;
					break;
				default:
					SDebug.LogError("No cell type match for: " + dataItem.cellType);
					break;
				case CollectCoinsCellType.RECOMMEND_BUTTON:
				case CollectCoinsCellType.SPACE:
					break;
				}
				return container2;
			}
		}

		[FormerlySerializedAs("friendPrefab")]
		[SerializeField]
		private GameObject _friendPrefab;

		[FormerlySerializedAs("facebookContentObject")]
		[SerializeField]
		private FacebookLoginPromotion _facebookContentObject;

		[FormerlySerializedAs("recommendPrefab")]
		[SerializeField]
		private GameObject _recommendPrefab;

		[FormerlySerializedAs("spacePrefab")]
		[SerializeField]
		private GameObject _spacePrefab;

		[FormerlySerializedAs("CrewHeader")]
		[SerializeField]
		private UILabel _crewHeader;

		private readonly List<CollectCoinsCell> _contentList = new List<CollectCoinsCell>();

		private CollectCoinsListAdapter _adapter;

		[SerializeField]
		private ScrollView _collectCoinsListView;

		[SerializeField]
		private UIPanel _scrollPanel;

		[SerializeField]
		private GameObject _lockContent;

		private bool _facebookLoginRunning;

		protected override void Init()
		{
			base.Init();
			HandleSafeArea();
			SetListContent();
			if (_contentList != null)
			{
				_adapter = new CollectCoinsListAdapter(_contentList, _friendPrefab, _recommendPrefab, _spacePrefab);
			}
		}

		private void HandleSafeArea()
		{
			if (SafeArea.IsRestricted)
			{
				_scrollPanel.topAnchor.absolute += -SafeArea.OFFSET_TOP;
				_scrollPanel.bottomAnchor.absolute += SafeArea.OFFSET_BOTTOM;
				_scrollPanel.ResetAndUpdateAnchors();
				_crewHeader.ResetAndUpdateAnchors();
			}
		}

		protected override void Show()
		{
			base.Show();
			SocialManager.friendsChangedHandler += ReloadFriends;
			UIScreenController.OnApplicationResumed += ReloadFriends;
			SocialManager.instance.UpdateFriendScores((bool didLoadFriends) =>
			{
				ReloadFriends();
			});
			ReloadFriends();
			_crewHeader.text = Strings.Get("COLLECT_TITLE");
		}

		private void UpdateAnchors()
		{
			_crewHeader.ResetAndUpdateAnchors();
			_scrollPanel.ResetAndUpdateAnchors();
		}

		protected override void Hide()
		{
			base.Hide();
			SocialManager.friendsChangedHandler -= ReloadFriends;
			UIScreenController.OnApplicationResumed -= ReloadFriends;
			if (_facebookLoginRunning)
			{
				HouseKeeper.SetFacebookPayoutActionToDefault();
				_facebookLoginRunning = false;
			}
		}

		private void ReloadFriends()
		{
			SetListContent();
			UpdateAnchors();
			if (_collectCoinsListView != null && _adapter != null)
			{
				_collectCoinsListView.SetAdapter(_adapter);
			}
		}

		private void ResetFriendList()
		{
			_contentList.Clear();
		}

		private void SetListContent()
		{
			ResetFriendList();
			CollectCoinsScreenManager.CollectCoinsScreenState screenState = CollectCoinsScreenManager.Instance.screenState;
			switch (screenState)
			{
			case CollectCoinsScreenManager.CollectCoinsScreenState.Offline:
				SetOfflineContent();
				break;
			case CollectCoinsScreenManager.CollectCoinsScreenState.Facebook:
			case CollectCoinsScreenManager.CollectCoinsScreenState.GameCenter:
			case CollectCoinsScreenManager.CollectCoinsScreenState.FacebookAndGamecenter:
				SetOnlineContent(screenState);
				break;
			default:
				SetOfflineContent();
				break;
			}
		}

		private void SetOfflineContent()
		{
			int nElements = 0;
			SetActiveFacebookLoginButton(true);
			AddDummiesToList(ref nElements, false);
			AddOverlayAndLockList(true);
		}

		private void SetOnlineContent(CollectCoinsScreenManager.CollectCoinsScreenState screenState)
		{
			Friend[] array = ((!Application.isEditor) ? SocialManager.instance.FriendsSortedByCash() : Globals.GetDebugFriends(30));
			bool flag = screenState == CollectCoinsScreenManager.CollectCoinsScreenState.Facebook || screenState == CollectCoinsScreenManager.CollectCoinsScreenState.FacebookAndGamecenter;
			bool flag2 = screenState == CollectCoinsScreenManager.CollectCoinsScreenState.GameCenter && array.Count() >= 3;
			bool flag3 = (!flag2 && !flag) || screenState == CollectCoinsScreenManager.CollectCoinsScreenState.Offline;
			bool activeFacebookLoginButton = false;
			int nElements = 0;
			if (flag)
			{
				AddDummiesToList(ref nElements, true);
			}
			if (flag2 && !flag)
			{
				activeFacebookLoginButton = true;
			}
			AddFriendsToList(array, ref nElements, flag3);
			if (!flag2 && !flag)
			{
				activeFacebookLoginButton = true;
				AddDummiesToList(ref nElements, false);
			}
			AddOverlayAndLockList(flag3);
			if (flag && SocialManager.facebookDialogsSupported)
			{
				AddRecommendButtonToList(ref nElements);
			}
			AddSpaceToList();
			SetActiveFacebookLoginButton(activeFacebookLoginButton);
		}

		private void SetActiveFacebookLoginButton(bool active)
		{
			_facebookContentObject.gameObject.SetActive(active);
			if (active)
			{
				_facebookContentObject.Init("FriendsUI", FacebookClicked);
			}
		}

		private void AddDummiesToList(ref int nElements, bool onlyCollectible)
		{
			CollectCoinsDummy[] collectCoinsDummyData = UserSave.collectCoinsDummyData;
			CollectCoinsDummy[] array = collectCoinsDummyData;
			foreach (CollectCoinsDummy collectCoinsDummy in array)
			{
				if ((!onlyCollectible && ((collectCoinsDummy.hasBeenCollected && collectCoinsDummy.fakeProgress > 0) || !collectCoinsDummy.hasBeenCollected)) || (onlyCollectible && !collectCoinsDummy.hasBeenCollected))
				{
					_contentList.Add(new CollectCoinsCell
					{
						cellType = CollectCoinsCellType.DUMMYFRIEND,
						dummyFriend = collectCoinsDummy,
						useDarkColour = (nElements % 2 == 0)
					});
					nElements++;
				}
			}
		}

		private void AddOverlayAndLockList(bool enable)
		{
			_lockContent.SetActive(enable);
		}

		private void AddSpaceToList()
		{
			_contentList.Add(new CollectCoinsCell
			{
				cellType = CollectCoinsCellType.SPACE
			});
		}

		private void AddFriendsToList(Friend[] friends, ref int nElement, bool lockList)
		{
			for (int i = 0; i < friends.Length; i++)
			{
				_contentList.Add(new CollectCoinsCell
				{
					cellType = CollectCoinsCellType.FRIEND,
					friend = friends[i],
					useDarkColour = (nElement % 2 == 0),
					disableScrolling = lockList
				});
				nElement++;
			}
		}

		private void AddRecommendButtonToList(ref int nElements)
		{
			_contentList.Add(new CollectCoinsCell
			{
				cellType = CollectCoinsCellType.RECOMMEND_BUTTON
			});
			nElements++;
		}

		public void FacebookClicked()
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
				if (!FacebookRewardsInfo.PayoutRewardIfPossible())
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
