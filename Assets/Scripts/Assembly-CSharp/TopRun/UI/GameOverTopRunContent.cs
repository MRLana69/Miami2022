using System;
using System.Collections;
using System.Collections.Generic;
using Info;
using Kiloo;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using SYBO.Subway.Info;
using TopRun.Internal;
using UI;
using UI.Generic.Layout;
using UI.KScrollView;
using UnityEngine;
using UnityEngine.Serialization;

namespace TopRun.UI
{
	public class GameOverTopRunContent : MonoBehaviour
	{
		private class TopRunListAdapter : ListAdapter<TopRunCell>
		{
			private GameObject _playerPrefab;

			private GameObject _rankHeaderPrefab;

			private GameObject _spacePrefab;

			private GameObject _emptyCell;

			private const int PLAYER_PREFAB = 0;

			private const int RANK_PREFAB = 1;

			private const int SPACE_CELL = 2;

			private const int EMPTY_CELL = 3;

			private const int FACEBOOK_BUTTON = 4;

			public TopRunListAdapter(List<TopRunCell> topRunListT, GameObject playerCell, GameObject headerCell, GameObject spaceCell, GameObject emptyCellPrefab)
				: base(topRunListT)
			{
				_playerPrefab = playerCell;
				_rankHeaderPrefab = headerCell;
				_spacePrefab = spaceCell;
				_emptyCell = emptyCellPrefab;
			}

			public void SetNewDataList(List<TopRunCell> topRunListT)
			{
				base.dataList = topRunListT;
				NotifyDataSetChanged();
			}

			public override int GetModelType(int index)
			{
				if (GetDataItem(index).cellType == TopRunCellType.SPACE_CELL)
				{
					return 2;
				}
				if (GetDataItem(index).cellType == TopRunCellType.HEADER)
				{
					return 1;
				}
				if (GetDataItem(index).cellType == TopRunCellType.EMPTY_CELL)
				{
					return 3;
				}
				if (GetDataItem(index).cellType == TopRunCellType.FACEBOOK_BUTTON)
				{
					return 4;
				}
				return 0;
			}

			public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass = false)
			{
				Container container2 = container;
				TopRunCell topRunCell = GetDataItem(dataIndex);
				if (container2 == null)
				{
					TopRunCellReferences topRunCellReferences = new TopRunCellReferences();
					if (topRunCell.cellType == TopRunCellType.HEADER)
					{
						container2 = new Container(_rankHeaderPrefab);
						RankHeader component = container2.prefabInstance.GetComponent<RankHeader>();
						topRunCellReferences.rankHeaderScript = component;
						container2.controlComponent = topRunCellReferences;
					}
					else if (topRunCell.cellType == TopRunCellType.SPACE_CELL)
					{
						container2 = new Container(_spacePrefab);
					}
					else if (topRunCell.cellType == TopRunCellType.EMPTY_CELL)
					{
						container2 = new Container(_emptyCell);
						TopRunEmptyCell component2 = container2.prefabInstance.GetComponent<TopRunEmptyCell>();
						topRunCellReferences.topRunEmptyCellScript = component2;
						container2.controlComponent = topRunCellReferences;
					}
					else
					{
						container2 = new Container(_playerPrefab);
						TopRunPlayerCell topRunPlayerCellScript = (topRunCellReferences.topRunPlayerCellScript = container2.prefabInstance.GetComponent<TopRunPlayerCell>());
						topRunCell.topRunPlayerCellScript = topRunPlayerCellScript;
						container2.controlComponent = topRunCellReferences;
						container2.onRecycleContainer = () =>
						{
							topRunCell.OnUpdateCell = null;
						};
					}
				}
				if (topRunCell.cellType == TopRunCellType.HEADER)
				{
					TopRunCellReferences topRunCellReferences2 = (TopRunCellReferences)container2.controlComponent;
					topRunCellReferences2.rankHeaderScript.InitHeader(topRunCell.rank, topRunCell.bestScore);
				}
				else if (topRunCell.cellType != TopRunCellType.SPACE_CELL)
				{
					if (topRunCell.cellType == TopRunCellType.PLAYER)
					{
						TopRunCellReferences topRunCellReferences3 = (TopRunCellReferences)container2.controlComponent;
						topRunCellReferences3.topRunPlayerCellScript.InitCell(topRunCell.player, topRunCell.useDarkColour, topRunCell.rank, topRunCell.isLocalUser);
						if (!isMeasurePass)
						{
							topRunCell.OnUpdateCell = topRunCellReferences3.topRunPlayerCellScript.Refresh;
						}
					}
					else
					{
						TopRunCellReferences topRunCellReferences4 = (TopRunCellReferences)container2.controlComponent;
						topRunCellReferences4.topRunEmptyCellScript.InitCell(topRunCell.rank);
					}
				}
				return container2;
			}
		}

		[SerializeField]
		private UILabel _titleLabel;

		[SerializeField]
		private UISprite _flagSprite;

		[SerializeField]
		private UILabel _countryLabel;

		[SerializeField]
		private Layout _countryLayout;

		[SerializeField]
		private GameObject _playerPrefab;

		[SerializeField]
		private GameObject _rankHeaderPrefab;

		[SerializeField]
		private GameObject _spaceCell;

		[SerializeField]
		private GameObject _emptyCell;

		[SerializeField]
		private FacebookLoginPromotion _facebookLoginContent;

		[SerializeField]
		private GameObject _normalStateContainer;

		[SerializeField]
		private TopRunNormalStateHelper _normalStatePrefab;

		private TopRunNormalStateHelper _normalStateHelper;

		[FormerlySerializedAs("_resultState")]
		[SerializeField]
		private GameObject _resultStateContainer;

		[SerializeField]
		private GameObject _resultStatePrefab;

		private GameObject _resultStateElement;

		[SerializeField]
		private GameObject _offlineState;

		[SerializeField]
		public GameObject loadingOverlay;

		[SerializeField]
		private UISprite _loadingOverlayBackground;

		[SerializeField]
		private ScrollView _topRunListView;

		[SerializeField]
		private UIPanel _scrollPanel;

		[SerializeField]
		private GameObject _globalStateNormal;

		[SerializeField]
		private GameObject _globalStateMaintenance;

		[Header("Maintenance State")]
		[SerializeField]
		private UILabel _maintenanceLabel1;

		[SerializeField]
		private UILabel _maintenanceLabel2;

		[SerializeField]
		private UILabel _maintenanceLabel3;

		[SerializeField]
		private Layout _maintenanceLabelLayout;

		[SerializeField]
		private UIWidget _maintenanceCharacterGuide;

		private CharacterScreenController _maintenanceCharacter;

		private TopRunListAdapter _adapter;

		private TopRunStates _currentState = TopRunStates.Offline;

		private bool _isLoading = true;

		private List<TopRunCell> _contentList = new List<TopRunCell>();

		private const int NO_MEDAL_RANK_THRESHOLD = 4;

		private const int SCROLL_PANEL_NORMAL_OFFSET = -5;

		private const int SCROLL_PANEL_RESULTS_OFFSET = -55;

		private const int POSITION_OFFSET_IN_LIST = 2;

		private bool _hasInitialised;

		private bool _alreadyShowingPopup;

		private void Init()
		{
			_hasInitialised = true;
			_facebookLoginContent.gameObject.SetActive(false);
			SetListContent();
			if (_contentList != null)
			{
				_adapter = new TopRunListAdapter(_contentList, _playerPrefab, _rankHeaderPrefab, _spaceCell, _emptyCell);
			}
			_normalStateHelper = UnityEngine.Object.Instantiate(_normalStatePrefab, _normalStateContainer.transform);
			_normalStateHelper.transform.Reset();
			_resultStateElement = UnityEngine.Object.Instantiate(_resultStatePrefab, _resultStateContainer.transform);
			_resultStateElement.transform.Reset();
		}

		private void OnEnable()
		{
			if (!_hasInitialised)
			{
				Init();
			}
			TopRunEventsHandler.instance.OnTopRunChangedEvent += TournamentChanged;
			TopRunRoot.manager.playerInfoChanged += OnPlayerChanged;
			UIScreenController.OnApplicationResumed += OnApplicationResumed;
			UIScreenController.OnPopupShown += OnPopupShown;
			UIScreenController.OnLastPopupClosed += OnLastPopupClosed;
			_isLoading = true;
			SetCurrentState();
			if (!UIScreenController.Instance.justPaidOutTopRun)
			{
				loadingOverlay.SetActive(true);
				RefreshOrFetchNewTournament();
			}
			SetScreenTop_UI();
			ForceUpdateAnchors();
			if (_topRunListView != null && _adapter != null)
			{
				_topRunListView.SetAdapter(_adapter);
			}
			if (!UIScreenController.Instance.justPaidOutTopRun)
			{
				ScrollToMyPosition();
			}
		}

		private void OnDisable()
		{
			TopRunEventsHandler.instance.OnTopRunChangedEvent -= TournamentChanged;
			TopRunRoot.manager.playerInfoChanged -= OnPlayerChanged;
			UIScreenController.OnApplicationResumed -= OnApplicationResumed;
			UIScreenController.OnPopupShown -= OnPopupShown;
			UIScreenController.OnLastPopupClosed -= OnLastPopupClosed;
			loadingOverlay.SetActive(false);
			UIScreenController.Instance.justPaidOutTopRun = false;
			DestroyMaintenanceCharacterModel();
		}

		private void OnPopupShown(string screenName)
		{
			if (!_alreadyShowingPopup)
			{
				_alreadyShowingPopup = true;
			}
		}

		private void OnLastPopupClosed()
		{
			_alreadyShowingPopup = false;
			if (UIScreenController.Instance.justPaidOutTopRun)
			{
				StartCoroutine(RefreshAdapterListDelayed(5));
				StartCoroutine(AnimateMedalAfterSeconds(0.5f));
			}
		}

		private void TournamentChanged(object sender, EventArgs eventArg)
		{
			if (base.gameObject.activeSelf && base.gameObject.activeInHierarchy && !TopRunRoot.manager.isRefreshing)
			{
				_isLoading = false;
				TopRunRewardHelper.RefreshPendingResults();
				SetCurrentState();
				RefreshAdapterIfDataChanged();
			}
		}

		private void OnApplicationResumed()
		{
			_isLoading = true;
			if (!UIScreenController.Instance.justPaidOutTopRun)
			{
				loadingOverlay.SetActive(true);
			}
			SetCurrentState();
			if (!UIScreenController.Instance.justPaidOutTopRun)
			{
				RefreshOrFetchNewTournament();
			}
		}

		private void RefreshAdapterIfDataChanged()
		{
			if (_adapter != null)
			{
				SetListContent();
				if (_contentList != null)
				{
					_adapter.SetNewDataList(_contentList);
				}
				SetScreenTop_UI();
				ForceUpdateAnchors();
			}
			ScrollToMyPosition();
			loadingOverlay.SetActive(false);
		}

		private IEnumerator RefreshAdapterListDelayed(int frames)
		{
			int frame = 0;
			while (frame < frames)
			{
				frame++;
				yield return new WaitForEndOfFrame();
			}
			if (_adapter != null && _contentList != null)
			{
				_adapter.SetNewDataList(_contentList);
			}
			ScrollToMyPosition();
			loadingOverlay.SetActive(false);
		}

		private IEnumerator AnimateMedalAfterSeconds(float seconds)
		{
			UIScreenController.Instance.justPaidOutTopRun = false;
			yield return new WaitForSeconds(seconds);
			int indexToScroll = GetMyIndexInTheList();
			if (indexToScroll >= 0)
			{
				TopRunPlayerCell playerScriptFromList = GetPlayerScriptFromList();
				if (playerScriptFromList != null)
				{
					playerScriptFromList.AnimateMedalIfNeeded();
				}
			}
		}

		private TopRunPlayerCell GetPlayerScriptFromList()
		{
			foreach (TopRunCell data in _adapter.dataList)
			{
				if (data.topRunPlayerCellScript != null && data.topRunPlayerCellScript.IsLocalUser())
				{
					return data.topRunPlayerCellScript;
				}
			}
			return null;
		}

		private void OnPlayerChanged(object o, PlayerInfoChangedEventArgs eventArg = null)
		{
			if (eventArg == null)
			{
				return;
			}
			IPlayer player = eventArg.player;
			foreach (TopRunCell content in _contentList)
			{
				if (content.player != null && content.player.publicKey == player.publicKey)
				{
					content.player = player;
					content.UpdateCell();
					break;
				}
			}
		}

		private void ScrollToMyPosition()
		{
			int num = GetMyIndexInTheList() - 2;
			if (num >= 0)
			{
				Container firstVisibleContainerOnScreen = _topRunListView.GetFirstVisibleContainerOnScreen();
				if (firstVisibleContainerOnScreen != null)
				{
					float offset = _topRunListView.rootPanel.baseClipRegion.w * 0.5f - firstVisibleContainerOnScreen.height;
					_topRunListView.SetSelection(num, ContainerPivot.Top, offset);
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
					if (_contentList[i].cellType == TopRunCellType.PLAYER && _contentList[i].isLocalUser)
					{
						result = i;
					}
				}
			}
			return result;
		}

		private void RefreshOrFetchNewTournament()
		{
			IManager manager = TopRunRoot.manager;
			bool flag = true;
			TopRunRewardHelper.RefreshPendingResults();
			if (manager != null)
			{
				if (manager.tournament != null && manager.nextTournamentFetchTime != DateTime.MinValue && manager.nextTournamentFetchTime < CDateTime.UtcNow && manager.tournament.hasResult)
				{
					manager.FetchNextTournament();
					flag = false;
				}
				if (flag)
				{
					manager.RefreshTournament();
				}
			}
		}

		private void SetCurrentState()
		{
			DestroyMaintenanceCharacterModel();
			IManager manager = TopRunRoot.manager;
			if (manager != null && manager.tournament != null)
			{
				if (TopRunRewardHelper.ShouldShowResults())
				{
					_currentState = TopRunStates.Results;
					_scrollPanel.topAnchor.target = _normalStateContainer.transform;
					_scrollPanel.topAnchor.relative = 0f;
					_scrollPanel.topAnchor.absolute = -55;
					_resultStateContainer.SetActive(true);
					_normalStateContainer.SetActive(true);
					_offlineState.SetActive(false);
					_normalStateHelper.InitNormalState();
				}
				else
				{
					_currentState = TopRunStates.Normal;
					_scrollPanel.topAnchor.target = _normalStateContainer.transform;
					_scrollPanel.topAnchor.relative = 0f;
					_scrollPanel.topAnchor.absolute = -5;
					_resultStateContainer.SetActive(false);
					_normalStateContainer.SetActive(true);
					_offlineState.SetActive(false);
					_normalStateHelper.InitNormalState();
				}
			}
			else if (!_isLoading && TopRunMaintenance.IsActive())
			{
				_currentState = TopRunStates.OfflineMaintenance;
				_resultStateContainer.SetActive(false);
				_normalStateContainer.SetActive(false);
				_offlineState.SetActive(false);
				SetMaintenanceLabels();
				CreateMaintenanceCharacterModel();
			}
			else
			{
				_currentState = TopRunStates.Offline;
				_scrollPanel.topAnchor.target = _offlineState.transform;
				_scrollPanel.topAnchor.relative = 0f;
				_scrollPanel.topAnchor.absolute = -5;
				_resultStateContainer.SetActive(false);
				_normalStateContainer.SetActive(false);
				_offlineState.SetActive(true);
				TopRunOfflineStateHelper componentInChildren = _offlineState.GetComponentInChildren<TopRunOfflineStateHelper>();
				if (componentInChildren != null)
				{
					componentInChildren.SetState(_isLoading ? TopRunOfflineStateHelper.OfflineState.Loading : TopRunOfflineStateHelper.OfflineState.TryAgain, null, this);
				}
			}
			bool flag = _currentState == TopRunStates.OfflineMaintenance;
			_globalStateMaintenance.SetActive(flag);
			_globalStateNormal.SetActive(!flag);
		}

		private void ForceUpdateAnchors()
		{
			_scrollPanel.ResetAndUpdateAnchors();
		}

		private void SetScreenTop_UI()
		{
			_titleLabel.text = Strings.Get("TOPRUN_TITLE");
			_titleLabel.color = ColorData.instance.midnight;
			_countryLabel.color = ColorData.instance.peacock;
			_flagSprite.spriteName = CountryManager.instance.GetCurrentTournamentCountry().flag;
			_countryLabel.text = Strings.Get(CountryManager.instance.GetCurrentTournamentCountry().displayNameID).ToUpper();
			_countryLayout.Align();
		}

		private void SetListContent()
		{
			IManager manager = TopRunRoot.manager;
			bool isFacebookButtonAdded = false;
			bool flag = false;
			if (_currentState == TopRunStates.Normal || _currentState == TopRunStates.Results)
			{
				bool flag2;
				int num;
				Player myPlayerProfile;
				if (manager.score > 0)
				{
					flag = true;
					flag2 = false;
					num = manager.tournament.GetRankForScore(manager.score);
					myPlayerProfile = GetMyPlayerProfile();
				}
				else
				{
					flag2 = true;
					myPlayerProfile = GetMyPlayerProfile(false);
					num = 4;
				}
				_contentList.Clear();
				int num2 = 0;
				for (int i = 0; i < manager.tournament.rankCount; i++)
				{
					List<IPlayer> list = new List<IPlayer>(manager.tournament.GetPlayersForRank(i));
					if (i >= 4 && list.Count <= 0 && !flag2 && manager.tournament.GetRankForScore(manager.score) < 4)
					{
						continue;
					}
					int num3 = 0;
					if (i <= 4)
					{
						if (i != 0 || (i == 0 && isFacebookButtonAdded))
						{
							_contentList.Add(new TopRunCell
							{
								cellType = TopRunCellType.SPACE_CELL
							});
						}
						_contentList.Add(new TopRunCell
						{
							cellType = TopRunCellType.HEADER,
							rank = i + 1,
							bestScore = manager.tournament.GetBestScoreForRank(i)
						});
					}
					else
					{
						num3 = num2;
					}
					if (list.Count <= 0)
					{
						if (num == i && flag)
						{
							_contentList.Add(new TopRunCell
							{
								cellType = TopRunCellType.PLAYER,
								player = myPlayerProfile,
								useDarkColour = true,
								rank = i + 1,
								isLocalUser = true
							});
							AddFacebookButtonAfterMe(ref isFacebookButtonAdded);
							flag = false;
						}
						else if (i <= 4 && (i != 4 || !flag2))
						{
							_contentList.Add(new TopRunCell
							{
								cellType = TopRunCellType.EMPTY_CELL,
								rank = i + 1
							});
						}
					}
					else
					{
						foreach (IPlayer item in list)
						{
							if (flag && num == i && myPlayerProfile.score >= item.score)
							{
								_contentList.Add(new TopRunCell
								{
									cellType = TopRunCellType.PLAYER,
									player = myPlayerProfile,
									useDarkColour = (num3 % 2 == 0),
									rank = i + 1,
									isLocalUser = true
								});
								AddFacebookButtonAfterMe(ref isFacebookButtonAdded);
								flag = false;
								num3++;
							}
							_contentList.Add(new TopRunCell
							{
								cellType = TopRunCellType.PLAYER,
								player = item,
								useDarkColour = (num3 % 2 == 0),
								rank = i + 1
							});
							num3++;
						}
					}
					if (i == num && flag)
					{
						_contentList.Add(new TopRunCell
						{
							cellType = TopRunCellType.PLAYER,
							player = myPlayerProfile,
							useDarkColour = (num3 % 2 == 0),
							rank = i + 1,
							isLocalUser = true
						});
						flag = false;
						AddFacebookButtonAfterMe(ref isFacebookButtonAdded);
					}
					num2 = num3;
				}
				if (flag2)
				{
					_contentList.Add(new TopRunCell
					{
						cellType = TopRunCellType.PLAYER,
						player = myPlayerProfile,
						useDarkColour = (num2 % 2 == 0),
						rank = num + 1,
						isLocalUser = true
					});
					AddFacebookButtonAfterMe(ref isFacebookButtonAdded);
				}
				return;
			}
			if (_currentState == TopRunStates.OfflineMaintenance)
			{
				_contentList.Clear();
				return;
			}
			_contentList.Clear();
			for (int j = 0; j < 4; j++)
			{
				if (j != 0)
				{
					_contentList.Add(new TopRunCell
					{
						cellType = TopRunCellType.SPACE_CELL
					});
				}
				_contentList.Add(new TopRunCell
				{
					cellType = TopRunCellType.HEADER,
					rank = j + 1,
					bestScore = -1
				});
				_contentList.Add(new TopRunCell
				{
					cellType = TopRunCellType.EMPTY_CELL,
					rank = j + 1
				});
			}
		}

		private void AddFacebookButtonAfterMe(ref bool isFacebookButtonAdded)
		{
			if (!isFacebookButtonAdded && !SocialManager.instance.facebookIsLoggedIn && !AgeRestriction.isRestricted)
			{
				isFacebookButtonAdded = true;
				UIWidget component = _facebookLoginContent.GetComponent<UIWidget>();
				_scrollPanel.bottomAnchor.absolute = component.height + component.bottomAnchor.absolute;
				_facebookLoginContent.gameObject.SetActive(true);
				_facebookLoginContent.Init("TopRunUI");
			}
			else if (!isFacebookButtonAdded)
			{
				_scrollPanel.bottomAnchor.absolute = 6;
				_facebookLoginContent.gameObject.SetActive(false);
			}
		}

		private Player GetMyPlayerProfile(bool hasScore = true)
		{
			Player player = new Player();
			player.score = (hasScore ? TopRunRoot.manager.score : 0);
			player.isFriend = true;
			Player player2 = player;
			if (SocialManager.instance.facebookState == SocialManager.FacebookState.LoggedIn && SocialManager.instance.facebookProfile != null)
			{
				player2.facebookProfile = SocialManager.instance.facebookProfile;
			}
			return player2;
		}

		private void SetMaintenanceLabels()
		{
			string line;
			string line2;
			string line3;
			TopRunMaintenance.GetMessage(out line, out line2, out line3);
			_maintenanceLabel1.text = line;
			_maintenanceLabel2.text = line2;
			_maintenanceLabel3.text = line3;
			_maintenanceLabelLayout.Align();
		}

		private void DestroyMaintenanceCharacterModel()
		{
			if (_maintenanceCharacter != null)
			{
				UnityEngine.Object.Destroy(_maintenanceCharacter.gameObject);
			}
		}

		private void CreateMaintenanceCharacterModel()
		{
			if (!(_maintenanceCharacter != null))
			{
				CharacterModelPreviewFactory.Instance.GetSimpleScreenModel(Characters.CharacterId.slick, 0, (CharacterScreenController csc) =>
				{
					_maintenanceCharacter = csc;
					GameObject objectToPosition = PlaceModelUnderAnchor(csc.gameObject, UIModelController.Instance.TutorialPopupAnchor.transform, 31);
					Vector3 zero = Vector3.zero;
					Vector3 newRotation = new Vector3(9f, -165f, 0f);
					Vector3 newScale = new Vector3(13f, 13f, 13f);
					_maintenanceCharacter.Freeze = 0f;
					PositionModelOnSprite(_maintenanceCharacterGuide, objectToPosition, zero, newRotation, newScale);
				});
			}
		}

		private static GameObject PlaceModelUnderAnchor(GameObject model, Transform anchor, int layer)
		{
			NGUITools.SetLayer(model, layer);
			model.transform.parent = anchor;
			model.transform.localPosition = Vector3.zero;
			model.transform.localEulerAngles = Vector3.zero;
			model.transform.localScale = Vector3.one;
			return model;
		}

		private static void PositionModelOnSprite(UIWidget tracker, GameObject objectToPosition, Vector3 positionOffset, Vector3 newRotation, Vector3 newScale)
		{
			Timer.ExecuteNextFrame(() =>
			{
				Vector3 position = tracker.transform.position;
				Vector3 vector = UIScreenController.Instance.Camera2DNormal.WorldToScreenPoint(position);
				vector = new Vector3(vector.x, vector.y, 1f);
				Vector3 position2 = UIScreenController.Instance.Camera3D.ScreenToWorldPoint(vector);
				objectToPosition.transform.position = position2;
				objectToPosition.transform.localPosition = objectToPosition.transform.localPosition + positionOffset;
				objectToPosition.transform.localRotation = Quaternion.Euler(newRotation);
				objectToPosition.transform.localScale = newScale;
			});
		}
	}
}
