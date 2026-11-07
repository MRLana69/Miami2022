using System;
using System.Linq;
using Data.Managers;
using Data.Save;
using Info;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Screens.BoardScreenInternal
{
	public class HoverboardOverlayHelper : MonoBehaviour
	{
		private Boards.BoardId _cachedBoardId;

		private bool _currentlySelected;

		private int _placementIndex;

		private Transform _trackerTransform;

		private Camera _3DClipCam;

		private Camera _2DOverlayCam;

		private Transform _cachedTransform;

		private float _topOverlayMinX = 100f;

		private float _topOverlayMaxX = 110f;

		private float _currentDistFromCenter;

		private bool _dirtyDist;

		private bool _hasEventBeenTried;

		private Boards.BoardId? _eventBoardId;

		[Header("Top")]
		[FormerlySerializedAs("topOverlay")]
		[SerializeField]
		private Transform _topOverlay;

		[SerializeField]
		private UISprite _newBubble;

		[SerializeField]
		private UISprite _limitedBubble;

		[SerializeField]
		private UISprite _limitedIcon;

		[SerializeField]
		private UISprite _giftBubble;

		[SerializeField]
		private UISprite _eventBubble;

		[Header("Bottom")]
		[FormerlySerializedAs("bottomOverlay")]
		[SerializeField]
		private Transform _bottomOverlay;

		[SerializeField]
		private UISprite _ownedSprite;

		[SerializeField]
		private UISprite _selectedSprite;

		[SerializeField]
		private UISprite _trialSprite;

		private bool _boardIsOwned;

		private bool _shouldShowTrial;

		private bool _shouldShowSelected;

		public float currentDistFromCenter
		{
			set
			{
				if (Math.Abs(_currentDistFromCenter - value) > 0.0001f)
				{
					_currentDistFromCenter = value;
					_dirtyDist = true;
				}
			}
		}

		public void Init(int index, Boards.BoardId id, Transform transformToTrack)
		{
			_dirtyDist = true;
			_placementIndex = index;
			_cachedBoardId = id;
			_3DClipCam = UIScreenController.Instance.Camera3DClip;
			_2DOverlayCam = UIScreenController.Instance.CameraOverlay2dClipping;
			_cachedTransform = base.transform;
			_trackerTransform = transformToTrack;
			_boardIsOwned = BoardManager.IsBoardOwned(_cachedBoardId);
			RefreshSelected(_currentlySelected);
			RefreshBottomIndicators();
			_limitedBubble.color = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;
			_limitedIcon.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
		}

		private void OnEnable()
		{
			_eventBoardId = EventBoard();
			SSEventsManagerController.OnSSEventStartedHandler += RefreshEvent;
			SSEventsManagerController.OnSSEventFinishedHandler += RefreshEvent;
			UIScreenController.OnApplicationResumed += RefreshEvent;
		}

		private void OnDisable()
		{
			SSEventsManagerController.OnSSEventStartedHandler -= RefreshEvent;
			SSEventsManagerController.OnSSEventFinishedHandler -= RefreshEvent;
			UIScreenController.OnApplicationResumed -= RefreshEvent;
		}

		private void Start()
		{
			BoardManager.boardUnlocked += HandleBoardUnlock;
		}

		private void Destroy()
		{
			BoardManager.boardUnlocked -= HandleBoardUnlock;
		}

		private void HandleBoardUnlock(Boards.BoardId boardID)
		{
			_boardIsOwned = BoardManager.IsBoardOwned(_cachedBoardId);
		}

		private void RefreshEvent()
		{
			_eventBoardId = EventBoard();
			_hasEventBeenTried = SSEventsManagerController.instance.HasTheCurrentEventBeenTried();
			RefreshSelected(_currentlySelected);
		}

		public void SelectedInMenu()
		{
			if (!BoardManager.HasBoardBeenSeen(_cachedBoardId))
			{
				BoardManager.MarkHoverboardAsSeen(_cachedBoardId);
			}
			RefreshSelected(true);
		}

		public void RefreshSelected(bool selected)
		{
			_currentlySelected = selected;
			if (selected)
			{
				_eventBubble.gameObject.SetActive(false);
				_giftBubble.gameObject.SetActive(false);
				_newBubble.gameObject.SetActive(false);
				_limitedBubble.gameObject.SetActive(false);
				return;
			}
			Boards.BoardId? eventBoardId = _eventBoardId;
			bool flag = eventBoardId.GetValueOrDefault() == _cachedBoardId && eventBoardId.HasValue;
			bool flag2 = (IsBoardFree(_cachedBoardId) && !_boardIsOwned) || BoardHasFreeThemes(_cachedBoardId);
			bool flag3 = Boards.boardData[_cachedBoardId].isNewInThisUpdate && !UserSave.isFreshInstall && (!BoardManager.HasBoardBeenSeen(_cachedBoardId) || (BoardManager.DoesBoardHaveThemes(_cachedBoardId) && !BoardManager.AreAllNewThemesSeenForBoard(_cachedBoardId))) && !_boardIsOwned;
			bool flag4 = Boards.boardData[_cachedBoardId].hasOnlineTimeLimit && !_boardIsOwned;
			_eventBubble.gameObject.SetActive(flag);
			_giftBubble.gameObject.SetActive(!flag && flag2);
			_newBubble.gameObject.SetActive(!flag && !flag2 && flag3);
			_limitedBubble.gameObject.SetActive(!flag && !flag2 && !flag3 && flag4);
		}

		private bool BoardHasFreeThemes(Boards.BoardId boardId)
		{
			if (!HoverboardThemes.hoverboardCustomThemes.ContainsKey(boardId))
			{
				return false;
			}
			return HoverboardThemes.hoverboardCustomThemes[boardId].Any((HoverboardThemes.HoverboardTheme ht) => (ht.unlockType == PurchaseData.UnlockType.Coins || ht.unlockType == PurchaseData.UnlockType.Keys) && ht.price == 0 && !BoardManager.IsThemeOwned(boardId, ht.identifier));
		}

		private Boards.BoardId? EventBoard()
		{
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			if (activeEventManager != null)
			{
				SSEventType eventType = activeEventManager.GetEventType();
				if (eventType == SSEventType.SpecialPowerBoard || eventType == SSEventType.ExpiredLimitedBoard)
				{
					bool found;
					Boards.BoardId eventValue = activeEventManager.GetEventValue<Boards.BoardId>(out found);
					if (found)
					{
						return eventValue;
					}
				}
			}
			return null;
		}

		public int GetIndex()
		{
			return _placementIndex;
		}

		public Boards.BoardId GetBoardType()
		{
			return _cachedBoardId;
		}

		private void Update()
		{
			RefreshBottomIndicators();
			if (_limitedBubble.gameObject.activeSelf && _boardIsOwned)
			{
				_limitedBubble.gameObject.SetActive(false);
			}
			if (_giftBubble.gameObject.activeSelf)
			{
				_giftBubble.gameObject.SetActive((IsBoardFree(_cachedBoardId) && !_boardIsOwned) || BoardHasFreeThemes(_cachedBoardId));
			}
		}

		private void RefreshBottomIndicators()
		{
			int shouldShowTrial;
			if (_hasEventBeenTried)
			{
				Boards.BoardId? eventBoardId = _eventBoardId;
				if (eventBoardId.GetValueOrDefault() == _cachedBoardId && eventBoardId.HasValue)
				{
					shouldShowTrial = ((PlayerSave.currentBoardId == _cachedBoardId) ? 1 : 0);
					goto IL_0041;
				}
			}
			shouldShowTrial = 0;
			goto IL_0041;
			IL_0041:
			_shouldShowTrial = (byte)shouldShowTrial != 0;
			_shouldShowSelected = PlayerSave.currentBoardId == _cachedBoardId;
			_trialSprite.gameObject.SetActive(_shouldShowTrial);
			_selectedSprite.gameObject.SetActive(!_shouldShowTrial && _shouldShowSelected);
			_ownedSprite.gameObject.SetActive(!_shouldShowTrial && !_shouldShowSelected && _boardIsOwned);
		}

		private void LateUpdate()
		{
			if (_dirtyDist)
			{
				_dirtyDist = false;
				Vector3 vector = _cachedTransform.parent.InverseTransformPoint(_2DOverlayCam.ScreenToWorldPoint(_3DClipCam.WorldToScreenPoint(_trackerTransform.position)));
				float y = Mathf.SmoothStep(_topOverlayMaxX, _topOverlayMinX, _currentDistFromCenter);
				_topOverlay.localPosition = new Vector3(vector.x, y, _cachedTransform.localPosition.z);
				_bottomOverlay.localPosition = new Vector3(vector.x, vector.y, _cachedTransform.localPosition.z);
			}
		}

		private bool IsBoardFree(Boards.BoardId boardId)
		{
			Boards.Board board = Boards.boardData[boardId];
			return (board.unlockType == PurchaseData.UnlockType.Coins || board.unlockType == PurchaseData.UnlockType.Keys) && board.price == 0;
		}
	}
}
