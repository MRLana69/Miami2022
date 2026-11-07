using Data.Managers;
using Data.Save;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Utility;
using SYBO.Subway.Characters;
using SYBO.Subway.Info;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Screens.CharacterScreenInternal
{
	public class ScrollListCharacterUI : MonoBehaviour
	{
		public bool isEnabled;

		private Characters.CharacterId _cachedId;

		private Characters.Model _cachedCharacterModel;

		private Camera _3DClipCam;

		private Camera _2DOverlayCam;

		private bool _hasInited3D;

		private Transform _cachedTransform;

		private Transform _headTransform;

		private Transform _chestTransform;

		[Header("Top")]
		[FormerlySerializedAs("headTrack")]
		[SerializeField]
		private Transform _headTrack;

		[FormerlySerializedAs("xOffset")]
		[SerializeField]
		private float _xOffset = 1f;

		[FormerlySerializedAs("yOffset")]
		[SerializeField]
		private float _yOffset = 1f;

		[FormerlySerializedAs("_newSprite")]
		[SerializeField]
		private UISprite _newBubble;

		[FormerlySerializedAs("limitedSprite")]
		[SerializeField]
		private UISprite _limitedBubble;

		[SerializeField]
		private UISprite _limitedIcon;

		[FormerlySerializedAs("_giftSprite")]
		[SerializeField]
		private UISprite _giftBubble;

		[FormerlySerializedAs("_eventSpriteBubble")]
		[SerializeField]
		private UISprite _eventBubble;

		[SerializeField]
		private UISprite _collectBubble;

		[Header("Bottom")]
		[FormerlySerializedAs("feetTrack")]
		[SerializeField]
		private Transform _feetTrack;

		[FormerlySerializedAs("ownedSprite")]
		[SerializeField]
		private UISprite _ownedSprite;

		[FormerlySerializedAs("selectedSprite")]
		[SerializeField]
		private UISprite _selectedSprite;

		[SerializeField]
		private GameObject _loadingUIContainer;

		private Characters.CharacterId? _eventCharId;

		private bool _isCharacterOwned;

		public void Init(Characters.CharacterId id, bool isCentered, Transform transformToTrack)
		{
			Init2D(id, isCentered);
			Init3D(transformToTrack);
		}

		public void Init2D(Characters.CharacterId id, bool isCentered)
		{
			_cachedId = id;
			_cachedCharacterModel = Characters.characterData[_cachedId];
			_3DClipCam = UIScreenController.Instance.Camera3DClip;
			_2DOverlayCam = UIScreenController.Instance.CameraOverlay2dClipping;
			_cachedTransform = base.transform;
			RefreshUI(isCentered);
			_isCharacterOwned = CharacterStickerManager.IsCharacterUnlocked(_cachedId, _cachedCharacterModel);
			UpdateSelection(_isCharacterOwned);
			_limitedBubble.color = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;
			_limitedIcon.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
			_collectBubble.color = GlobalColors.FIRST_PURCHASE_BONUS_BREADCRUMP_BUBBLE_COLOR;
		}

		public void Init3D(Transform transformToTrack)
		{
			_headTransform = TransformUtility.FindChild(transformToTrack, Character.Instance.CharacterModel.HeadJoint.name);
			_chestTransform = TransformUtility.FindChild(transformToTrack, Character.Instance.CharacterModel.Hip.name);
			_hasInited3D = true;
			_loadingUIContainer.SetActive(false);
		}

		public void OnModelDestroyed()
		{
			_hasInited3D = false;
			_headTransform = null;
			_chestTransform = null;
			_loadingUIContainer.SetActive(true);
		}

		private void OnEnable()
		{
			_eventCharId = EventCharacter();
			SSEventsManagerController.OnSSEventStartedHandler += EnableEventRelatedElements;
			SSEventsManagerController.OnSSEventFinishedHandler += EnableEventRelatedElements;
		}

		private void OnDisable()
		{
			SSEventsManagerController.OnSSEventStartedHandler -= EnableEventRelatedElements;
			SSEventsManagerController.OnSSEventFinishedHandler -= EnableEventRelatedElements;
		}

		private void Start()
		{
			CharacterManager.characterUnlocked += HandleCharacterUnlocked;
		}

		private void OnDestroy()
		{
			CharacterManager.characterUnlocked -= HandleCharacterUnlocked;
		}

		private void EnableEventRelatedElements()
		{
			_eventCharId = EventCharacter();
			RefreshUI(false);
		}

		public void CenteredInMenu()
		{
			if (!CharacterStickerManager.HasCharacterBeenSeen(_cachedId))
			{
				CharacterManager.MarkCharacterAsSeen(_cachedId);
			}
			RefreshUI(true);
		}

		public void RefreshUI(bool isCenterCharacter)
		{
			if (!isEnabled)
			{
				return;
			}
			if (isCenterCharacter)
			{
				_limitedBubble.gameObject.SetActive(false);
				_giftBubble.gameObject.SetActive(false);
				_newBubble.gameObject.SetActive(false);
				_eventBubble.gameObject.SetActive(false);
				_collectBubble.gameObject.SetActive(false);
				return;
			}
			bool flag = Characters.characterData[_cachedId].unlockType == PurchaseData.UnlockType.RewardForInApp && UserSave.hasPurchasedOrRestored && !CharacterStickerManager.IsCharacterUnlocked(_cachedId);
			Characters.CharacterId? eventCharId = _eventCharId;
			int num;
			if (eventCharId.HasValue)
			{
				Characters.CharacterId? eventCharId2 = _eventCharId;
				num = ((eventCharId2.GetValueOrDefault() == _cachedId && eventCharId2.HasValue) ? 1 : 0);
			}
			else
			{
				num = 0;
			}
			bool flag2 = (byte)num != 0;
			bool flag3 = CharacterStickerManager.HasCharacterFreeTheme(_cachedId);
			bool flag4 = !UserSave.isFreshInstall && ((Characters.characterData[_cachedId].isNewInThisUpdate && !CharacterStickerManager.HasCharacterBeenSeen(_cachedId) && !CharacterManager.IsCharacterOwned(_cachedId)) || CharacterStickerManager.HasCharacterNewTheme(_cachedId));
			bool flag5 = Characters.characterData[_cachedId].hasOnlineSettings && !CharacterStickerManager.IsCharacterUnlocked(_cachedId);
			_collectBubble.gameObject.SetActive(flag);
			_eventBubble.gameObject.SetActive(!flag && flag2);
			_giftBubble.gameObject.SetActive(!flag && !flag2 && flag3);
			_newBubble.gameObject.SetActive(!flag && !flag2 && !flag3 && flag4);
			_limitedBubble.gameObject.SetActive(!flag && !flag2 && !flag3 && !flag4 && flag5);
		}

		private Characters.CharacterId? EventCharacter()
		{
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			if (activeEventManager != null)
			{
				SSEventType eventType = activeEventManager.GetEventType();
				if (eventType == SSEventType.ExpiredLimitedCharacter || eventType == SSEventType.CharacterDay)
				{
					bool found;
					Characters.CharacterId eventValue = activeEventManager.GetEventValue<Characters.CharacterId>(out found);
					if (found)
					{
						return eventValue;
					}
				}
			}
			return null;
		}

		[UsedImplicitly]
		private void Update()
		{
			if (isEnabled)
			{
				UpdateSelection(_isCharacterOwned);
			}
		}

		private void UpdateSelection(bool shouldShowOwned)
		{
			bool flag = PlayerSave.currentCharacterId == _cachedId;
			_selectedSprite.gameObject.SetActive(flag);
			_ownedSprite.gameObject.SetActive(!flag && shouldShowOwned);
			if (shouldShowOwned && _limitedBubble.isVisible)
			{
				_limitedBubble.gameObject.SetActive(false);
			}
		}

		private void HandleCharacterUnlocked(Characters.CharacterId characterID, bool wasRestore)
		{
			if (characterID == _cachedId)
			{
				_isCharacterOwned = CharacterStickerManager.IsCharacterUnlocked(_cachedId, _cachedCharacterModel);
			}
		}

		[UsedImplicitly]
		private void LateUpdate()
		{
			if (_hasInited3D && isEnabled)
			{
				Vector3 position = new Vector3(_chestTransform.position.x, _headTransform.position.y * _yOffset, _chestTransform.position.z);
				Vector3 position2 = _3DClipCam.WorldToScreenPoint(position);
				Vector3 position3 = _2DOverlayCam.ScreenToWorldPoint(position2);
				position3 = _cachedTransform.parent.InverseTransformPoint(position3);
				_headTrack.localPosition = new Vector3(position3.x, position3.y, _cachedTransform.localPosition.z);
				Vector3 position4 = new Vector3(_chestTransform.position.x * _xOffset, _chestTransform.position.y, _chestTransform.position.z);
				Vector3 position5 = _3DClipCam.WorldToScreenPoint(position4);
				Vector3 position6 = _2DOverlayCam.ScreenToWorldPoint(position5);
				position6 = _cachedTransform.parent.InverseTransformPoint(position6);
				position6 = new Vector3(position6.x + 20f, _cachedTransform.localPosition.y - 20f, _cachedTransform.localPosition.z);
				_feetTrack.localPosition = position6;
			}
		}
	}
}
