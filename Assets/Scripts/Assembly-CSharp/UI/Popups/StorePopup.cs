using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InApp;
using Info;
using Kiloo.Common.Backend.PurchaseValidation;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using Kiloo.Common.Utility;
using UI.Generic.Layout;
using UI.Screens;
using UI.Screens.StoreScreenInternal;
using UnityEngine;

namespace UI.Popups
{
	public class StorePopup : UIPopup
	{
		[SerializeField]
		private UIPanel _basePanel;

		[SerializeField]
		private UILabel _header;

		[SerializeField]
		private UIScrollView _scrollView;

		[SerializeField]
		private UIPanel _scrollViewPanel;

		[SerializeField]
		private StoreContainerSmall _containerPrefab;

		[SerializeField]
		private Layout _containerLayout;

		[SerializeField]
		private Layout _contentLayoutCoins;

		[SerializeField]
		private Layout _contentLayoutKeys;

		[SerializeField]
		private BoomBotBanner _boombotBanner;

		[SerializeField]
		private bool _shouldAnimate;

		[SerializeField]
		private AnimationCurve _scrollViewAnimationCurve;

		[SerializeField]
		private float _scrollViewAnimationHeight;

		[SerializeField]
		private float _scrollViewAnimationTime;

		[SerializeField]
		private float _scrollViewAnimationStartDelay;

		private int _baseBottomAnchor;

		private bool wasStoreDiscountActiveLastTime;

		private bool _subscribedToEvents;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		public override void BackButtonPressed()
		{
			UIScreenController.Instance.ClosePopup();
		}

		public override string GetFlurryName()
		{
			return "CoinsUI_quick";
		}

		protected override void Init()
		{
			_baseBottomAnchor = _scrollView.panel.bottomAnchor.absolute;
			CreatePurchases();
			wasStoreDiscountActiveLastTime = InAppManager.IsAnyCurrencyDiscountActive || InAppManager.IsSpecialSaleActive;
			_contentLayoutCoins.Align(true);
			_contentLayoutKeys.Align(true);
			_containerLayout.Align(true);
			_scrollView.transform.Reset();
			FrameTimer.ExecuteWithDelay(1, _scrollView.ResetPosition);
			_header.text = Strings.Get("STORE_TITLE");
			_header.color = ColorData.instance.midnight;
			if (FirstPurchaseBonusManager.isFirstPurchaseBonusActive)
			{
				FirstPurchaseBonusManager.StartStateCheckCoroutine();
				FirstPurchaseBonusManager.onFirstPurchaseBonusBecameInactive += OnFirstPurchaseBonusBecameInactive;
			}
			VerifyAndFixSubLayers();
		}

		private void SubscribeToEvents()
		{
			if (!_subscribedToEvents)
			{
				OnlineSettings.instance.AddOnFinishedDownloadingOnlineSettingsHandler(OnOnlineSettingsFinishedDownloading);
				if (InAppManager.instance != null)
				{
					InAppManager.instance.onProductRequestDone += OnProductRequestDone;
					InAppManager.instance.onPurchaseSuccess += OnPurchaseSuccess;
				}
				_subscribedToEvents = true;
			}
		}

		private void UnsubscribeFromEvents()
		{
			if (_subscribedToEvents)
			{
				OnlineSettings.instance.RemoveOnFinishedDownloadingOnlineSettingsHandler(OnOnlineSettingsFinishedDownloading);
				if (InAppManager.instance != null)
				{
					InAppManager.instance.onProductRequestDone -= OnProductRequestDone;
					InAppManager.instance.onPurchaseSuccess -= OnPurchaseSuccess;
				}
				_subscribedToEvents = false;
			}
		}

		private void OnFirstPurchaseBonusBecameInactive()
		{
			UpdateBoomBotBanner();
			Timer.ExecuteNextFrame(() =>
			{
				_scrollView.ResetPosition();
			});
		}

		private void VerifyAndFixSubLayers()
		{
			IOrderedEnumerable<UIPanel> orderedEnumerable = from p in GetComponentsInChildren<UIPanel>()
				orderby p.transform.GetHierarchyDepth()
				select p;
			foreach (UIPanel item in orderedEnumerable)
			{
				if (item.depth < _basePanel.depth)
				{
					item.depth += _basePanel.depth;
				}
			}
		}

		protected override void Show()
		{
			SubscribeToEvents();
			bool flag = InAppManager.IsAnyCurrencyDiscountActive || InAppManager.IsSpecialSaleActive;
			if (flag != wasStoreDiscountActiveLastTime)
			{
				CreatePurchases();
			}
			UpdateBoomBotBanner();
			if (_shouldAnimate)
			{
				_scrollView.MoveRelative(Vector3.up * _scrollViewAnimationHeight);
				_scrollView.enabled = false;
				StartCoroutine(PeekListAnimation(0f - _scrollViewAnimationHeight, _scrollViewAnimationTime, () =>
				{
					_scrollView.enabled = true;
				}));
				_shouldAnimate = false;
			}
			else
			{
				_scrollView.enabled = true;
			}
			if (StoreABManager.StoreNeedsRefresh(StoreABManager.StoreOrigin.StorePopup))
			{
				RefreshPurchaseCells();
			}
		}

		protected override void Hide()
		{
			UnsubscribeFromEvents();
		}

		private void UpdateBoomBotBanner()
		{
			if (FirstPurchaseBonusManager.isFirstPurchaseBonusActive)
			{
				_scrollView.panel.bottomAnchor.absolute = _baseBottomAnchor + 66;
				_boombotBanner.gameObject.SetActive(true);
				_boombotBanner.Setup();
			}
			else
			{
				_scrollView.panel.bottomAnchor.absolute = _baseBottomAnchor;
				_boombotBanner.gameObject.SetActive(false);
			}
			_scrollView.panel.ResetAndUpdateAnchors();
			_scrollView.UpdatePosition();
			_scrollView.gameObject.SetActive(false);
			_scrollView.gameObject.SetActive(true);
		}

		private void CreatePurchases()
		{
			RemoveAllEntries();
			List<InAppItemModel> list = InAppManager.GetActiveInAppOffers().Values.ToList();
			StoreABManager.VerifyCoinList(list);
			foreach (InAppItemModel item in list)
			{
				StoreContainerSmall storeContainerSmall = ((item.itemType != InAppItemModel.ItemType.Coins) ? CreatePurchaseEntry(_contentLayoutKeys.transform) : CreatePurchaseEntry(_contentLayoutCoins.transform));
				StoreContainerSmall.StoreContainerSmallPayload cellPayloadForOffer = ShopCellManager.GetCellPayloadForOffer(item);
				storeContainerSmall.Setup(cellPayloadForOffer);
			}
		}

		private void RefreshPurchaseCells()
		{
			CreatePurchases();
			Timer.ExecuteNextFrame(() =>
			{
				_contentLayoutCoins.Align();
			});
			Timer.ExecuteNextFrame(() =>
			{
				_contentLayoutKeys.Align();
			});
			Timer.ExecuteNextFrame(() =>
			{
				_containerLayout.Align();
			});
		}

		private StoreContainerSmall CreatePurchaseEntry(Transform parent)
		{
			return UnityEngine.Object.Instantiate(_containerPrefab, parent);
		}

		private void RemoveAllEntries()
		{
			_contentLayoutCoins.transform.KillChildren();
			_contentLayoutKeys.transform.KillChildren();
		}

		private IEnumerator PeekListAnimation(float distance, float duration, Action onComplete)
		{
			yield return CoroutineUtil.WaitForRealSeconds(_scrollViewAnimationStartDelay);
			float time = 0f;
			Vector3 startPosition = _scrollView.transform.localPosition;
			Vector3 endPosition = startPosition + new Vector3(0f, distance, 0f);
			Vector2 startPanelOffset = _scrollViewPanel.clipOffset;
			Vector2 endPanelOffset = startPanelOffset - new Vector2(0f, distance);
			for (; time < duration; time += Time.unscaledDeltaTime)
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

		private void OnOnlineSettingsFinishedDownloading()
		{
			RefreshPurchaseCells();
		}

		private void OnProductRequestDone()
		{
			RefreshPurchaseCells();
		}

		private void OnPurchaseSuccess(string productId, bool wasRestored, IPurchaseValidationResult validationResult)
		{
			RefreshPurchaseCells();
		}
	}
}
