using JetBrains.Annotations;
using Kiloo.Common.Ads;
using Kiloo.Common.JsonUtil;
using UI.FreeStuff;
using UnityEngine;

namespace UI.Popups
{
	public class SaveMeVideoPopup : UIPopup
	{
		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private RewardForVideoButton _videoButton;

		protected override void Init()
		{
			_title.text = Strings.Get("SAVE_ME_VIDEO_POPUP_TITLE");
			_videoButton.Init(new VideoButtonPayload
			{
				title = "FREE_WITH_EXCLAMATION",
				isAvailable = () => true,
				onVideoWatched = () =>
				{
					SaveMeManager.OnFinishedWatchingVideo("SaveMeVideoPopup");
				},
				adLocation = AdLocations.REVIVAL_VIDEOS,
				firstColor = ColorData.instance.grass,
				firstColorDark = ColorData.instance.avocado,
				firstHighlightColor = ColorData.instance.lime,
				secondColor = ColorData.instance.grass,
				secondColorDark = ColorData.instance.avocado,
				secondHighlightColor = ColorData.instance.lime,
				titleColor = ColorData.instance.white,
				descriptionColor = ColorData.instance.white,
				autoPlayOnFill = true
			});
		}

		protected override void Show()
		{
			SaveMeManager.videoIsPlaying = false;
			AdSystemManager.onFailedToShowVideo += OnShowVideoFailure;
			UIScreenController.Instance.OnOverlayManuallyClosed += OnVideoOverlayClosed;
			AdSystemManager.onDismissedVideo += OnDismissedVideo;
			if (SaveMeManager.isRevivalVideoCached)
			{
				PlayAdVideo();
			}
			else
			{
				UpdateButtonStates();
			}
		}

		protected override void Hide()
		{
			AdSystemManager.onFailedToShowVideo -= OnShowVideoFailure;
			UIScreenController.Instance.OnOverlayManuallyClosed -= OnVideoOverlayClosed;
			AdSystemManager.onDismissedVideo -= OnDismissedVideo;
		}

		public override void BackButtonPressed()
		{
			if (!UIScreenController.Instance.IsVideoAdsOverlayVisible())
			{
				OnCloseClicked();
			}
		}

		public override bool ShouldShowTopBar()
		{
			return false;
		}

		private void UpdateButtonStates()
		{
			_videoButton.RefreshState();
		}

		private void PlayAdVideo()
		{
			SaveMeManager.videoIsPlaying = true;
			SaveMeManager.PostSaveMeClickEvent(SaveMeManager.ClickAction.WatchAd);
			AdTrackingSystemLogger.LogVideoAdButtonClick(AdLocations.REVIVAL_VIDEOS);
			AdSystemManager.Show(AdLocations.REVIVAL_VIDEOS, () =>
			{
				SaveMeManager.OnFinishedWatchingVideo("SaveMeVideoPopup");
			});
		}

		private void OnVideoOverlayClosed()
		{
			SaveMeManager.OnFinishedWatchingVideo("SaveMeVideoPopup");
		}

		private void OnShowVideoFailure(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationsInfo)
		{
			SaveMeManager.adsFailed = true;
			SaveMeManager.OnFinishedWatchingVideo("SaveMeVideoPopup");
		}

		private void OnDismissedVideo(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
		{
			if (adLocationInfo.id == AdLocations.REVIVAL_VIDEOS.id)
			{
				SaveMeManager.videoIsPlaying = false;
				OnCloseClicked();
			}
		}

		[UsedImplicitly]
		public void OnCloseClicked()
		{
			UIScreenController.Instance.ClosePopup();
			UIScreenController.Instance.PushPopup("SaveMePopup");
		}
	}
}
