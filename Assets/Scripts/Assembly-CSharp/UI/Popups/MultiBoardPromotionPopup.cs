using System.Collections.ObjectModel;
using JetBrains.Annotations;
using SYBO.Subway.Promotion;
using UI.Generic;
using UI.Generic.Layout;
using UI.Screens;
using UnityEngine;

namespace UI.Popups
{
	public class MultiBoardPromotionPopup : UIPopup
	{
		[Header("Top")]
		[SerializeField]
		private UILabel _labelTitle;

		[SerializeField]
		private UILabel _labelSubtitle;

		[SerializeField]
		private TimerHelper _timerPrefab;

		[SerializeField]
		private UIWidget _timerContainer;

		[SerializeField]
		private Layout _timerLayout;

		[SerializeField]
		private UITexture _imageSticker;

		[Header("Bottom")]
		[SerializeField]
		private UILabel _labelGotoShop;

		[Header("Entries")]
		[SerializeField]
		private MultiboardPromotionEntryView _leftEntryView;

		[SerializeField]
		private MultiboardPromotionEntryView _centerEntryView;

		[SerializeField]
		private MultiboardPromotionEntryView _rightEntryView;

		private TimerHelper _timer;

		private MultiBoardPromotion _promotion;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			_promotion = PromotionManager.Instance.TryGetCurrentPromotion<MultiBoardPromotion>();
			if (_promotion == null)
			{
				Debug.LogError("Current Promotion is not a multi board promotion.");
				return;
			}
			InitEntryViews();
			InitTimer();
			InitTop();
			InitBottom();
		}

		private void InitEntryViews()
		{
			ReadOnlyCollection<MultiBoardBundlePromotionEntry> allEntries = _promotion.AllEntries;
			_leftEntryView.Init(allEntries[0]);
			_centerEntryView.Init(allEntries[1]);
			_rightEntryView.Init(allEntries[2]);
		}

		private void InitTimer()
		{
			_timer = Object.Instantiate(_timerPrefab, _timerContainer.transform);
			_timer.SetTimerColors(_promotion.TimerTextColor, _promotion.TimerBgColor);
			_timer.SetAlignment(HorizontalAlignment.Center);
		}

		private void InitTop()
		{
			_labelTitle.text = Strings.Get(_promotion.TitleId);
			_labelSubtitle.text = Strings.Get(_promotion.SubtitleId);
			_labelTitle.color = _promotion.TitleTextColor;
			_labelSubtitle.color = _promotion.SubtitleTextColor;
			_imageSticker.mainTexture = _promotion.StickerTexture;
			_timer.StartTimer(_promotion.TimeLeft);
			_timerLayout.Align();
		}

		private void InitBottom()
		{
			_labelGotoShop.text = Strings.Get("POPUP_PROMOTION_GOTO_SHOP");
		}

		[UsedImplicitly]
		public void OnGoToShop()
		{
			UIScreenController.Instance.PushScreen("StoreScreen");
			ClosePopup();
		}

		[UsedImplicitly]
		public void OnCloseClicked()
		{
			ClosePopup();
		}
	}
}
