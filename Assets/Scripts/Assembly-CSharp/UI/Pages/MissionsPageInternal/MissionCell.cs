using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data.Save;
using JetBrains.Annotations;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.JsonUtil;
using Kiloo.Common.Timer;
using Kiloo.Common.Utility;
using Missions;
using NaughtyAttributes;
using SYBO.Subway;
using UI.Breadcrumbs;
using UI.Generic;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.Pages.MissionsPageInternal
{
	public class MissionCell : MonoBehaviour, IBuyButton
	{
		public enum PayloadSource
		{
			PauseMenu = 0,
			ChallengesScreen = 1
		}

		public class MissionCellPayload
		{
			public int cellHeight;

			public int missionIndex;

			public int headerHeight;

			public MissionInfo missionInfo;

			public Action<int> onMissionCellAnimationComplete;

			public PayloadSource source;
		}

		[SerializeField]
		private GameObject[] _hideWhenPopupsActive;

		[Header("Top section")]
		[SerializeField]
		private UISprite _headerSprite;

		[SerializeField]
		private UILabel _missionText;

		[SerializeField]
		private UILabel _missionSkipText;

		[SerializeField]
		private UILabel _skipCost;

		[SerializeField]
		private UILabel _videoLabel;

		[SerializeField]
		private UILabel _missionProgress;

		[SerializeField]
		private UISlider _progressSlider;

		[SerializeField]
		private GameObject _progressComplete;

		[SerializeField]
		private UISprite _progressFlashSprite;

		[SerializeField]
		private UILabel _completedLabel;

		[SerializeField]
		private UISprite _checkmarkSprite;

		[SerializeField]
		private List<UISprite> _completedGlows;

		[SerializeField]
		private UIButton _skipWithKeysButton;

		[SerializeField]
		private UIButton _skipWithVideoButton;

		[Header("Bottom section")]
		[SerializeField]
		private UISprite _backgroundSprite;

		[SerializeField]
		private UIWidget _inProgressDisplay;

		[SerializeField]
		private UIWidget _completedDisplay;

		[Space]
		[SerializeField]
		private GameObject _animatedTVIconPrefab;

		[SerializeField]
		private UIWidget _animatedTVIconContainer;

		private MissionCellPayload _payload;

		private int _currentProgress;

		private bool _hasHookedUpEvents;

		private bool _isNewlyCreated;

		private const int MIN_BG_HEIGHT = 80;

		private const int MIN_BG_HEIGHT_FOR_CHALLENGES_SCREEN = 109;

		private const int BG_TRIGGER_HEIGHT = 88;

		private const int MIN_HEADER_HEIGHT = 45;

		public const int MAX_HEADER_HEIGHT = 50;

		public bool hasSkippedMission { get; private set; }

		public bool isVideoPlaying { get; private set; }

		public void Init(MissionCellPayload payload)
		{
			hasSkippedMission = false;
			_payload = payload;
			_backgroundSprite.height = payload.cellHeight;
			_headerSprite.height = ((payload.headerHeight <= 0) ? CalculateHeaderHeight() : payload.headerHeight);
			_backgroundSprite.gameObject.SetActive(CanShowBottomPanel());
			HookEvents();
			AlignCompletedText();
			CreateTVIcon();
			Refresh();
			_isNewlyCreated = true;
		}

		private void HookEvents()
		{
			if (!_hasHookedUpEvents)
			{
				_hasHookedUpEvents = true;
				MissionsManager.Instance.onMissionProgressed += OnMissionProgress;
				AdSystemManager.onFailedToShowVideo += OnShowVideoFailure;
				UIScreenController.Instance.OnOverlayManuallyClosed += OnVideoOverlayClosed;
				UIScreenController.OnPopupShown += OnPopupShown;
				UIScreenController.OnLastPopupClosed += OnLastPopupClosed;
				UIScreenController.OnApplicationResumed += ShowSkipButton;
			}
		}

		private void UnhookEvents()
		{
			if (_hasHookedUpEvents)
			{
				_hasHookedUpEvents = false;
				MissionsManager.Instance.onMissionProgressed -= OnMissionProgress;
				AdSystemManager.onFailedToShowVideo -= OnShowVideoFailure;
				UIScreenController.Instance.OnOverlayManuallyClosed -= OnVideoOverlayClosed;
				UIScreenController.OnPopupShown -= OnPopupShown;
				UIScreenController.OnLastPopupClosed -= OnLastPopupClosed;
				UIScreenController.OnApplicationResumed -= ShowSkipButton;
			}
		}

		private void OnDisable()
		{
			_isNewlyCreated = false;
		}

		private void OnLastPopupClosed()
		{
			GameObject[] hideWhenPopupsActive = _hideWhenPopupsActive;
			foreach (GameObject gameObject in hideWhenPopupsActive)
			{
				gameObject.SetActive(true);
			}
		}

		private void OnPopupShown(string popupName)
		{
			GameObject[] hideWhenPopupsActive = _hideWhenPopupsActive;
			foreach (GameObject gameObject in hideWhenPopupsActive)
			{
				gameObject.SetActive(false);
			}
		}

		private void AlignCompletedText()
		{
			_completedLabel.text = Strings.Get("MISSION_PAGE_COMPLETED");
			_completedLabel.gameObject.SetActive(true);
			_checkmarkSprite.gameObject.SetActive(true);
			_completedDisplay.GetComponent<HorizontalLayout>().Align();
			bool wasComplete = _payload.missionInfo.complete;
			Timer.ExecuteNextFrame(() =>
			{
				if (!wasComplete)
				{
					_completedLabel.gameObject.SetActive(false);
					_checkmarkSprite.gameObject.SetActive(false);
				}
			});
		}

		private void CreateTVIcon()
		{
			GameObject gameObject = _animatedTVIconContainer.gameObject.AddChild(_animatedTVIconPrefab);
			gameObject.GetComponent<FitSpriteToWidget>().widgetContainer = _animatedTVIconContainer;
			gameObject.GetComponent<FitSpriteToWidget>().FitNow();
		}

		private void OnDestroy()
		{
			UnhookEvents();
		}

		private bool IsBottomBoxActive()
		{
			return _backgroundSprite.gameObject.activeInHierarchy;
		}

		private bool CanShowBottomPanel()
		{
			return _payload.cellHeight > 80;
		}

		private void UpdateText()
		{
			MissionInfo missionInfo = _payload.missionInfo;
			int goal = missionInfo.mission.goal;
			int number = missionInfo.mission.goal - missionInfo.progress;
			string key = ((missionInfo.mission.goal <= 1) ? missionInfo.template.descriptionSingle : missionInfo.template.description);
			_missionText.text = Strings.Get(key).Populate(AddNumberColor(goal), AddNumberColor(number));
			_skipCost.text = GetSkipCost().ToString();
			_videoLabel.text = Strings.Get("FREE_WITH_EXCLAMATION");
			_missionSkipText.text = Strings.Get("MISSION_PAGE_SKIP_TEXT");
		}

		private static string AddNumberColor(int number)
		{
			string text = ColorData.instance.sun.ToUIHex();
			return "[c]{0}{1}[-][/c]".Populate(text, number);
		}

		private int CalculateHeaderHeight()
		{
			return (_backgroundSprite.height <= 88) ? 45 : 50;
		}

		private void UpdateProgress()
		{
			_missionProgress.text = _payload.missionInfo.progress.ToString();
			_progressSlider.value = (float)_payload.missionInfo.progress / (float)_payload.missionInfo.mission.goal;
			_currentProgress = _payload.missionInfo.progress;
		}

		private int GetSkipCost()
		{
			return MissionsData.skipData[_payload.missionIndex].GetPrice();
		}

		[UsedImplicitly]
		public void OnBuyClicked()
		{
			_skipWithKeysButton.isEnabled = false;
			hasSkippedMission = true;
			PurchaseHandler.Instance.PurchaseSkipMission(_payload.missionIndex, this);
			Breadcrumb.Consume(Breadcrumb.BreadcrumbEvent.MissionSet);
		}

		[UsedImplicitly]
		public void OnSkipWithVideoClicked()
		{
			AdLocationInfo currentLocationInfo = MissionsManager.GetCurrentLocationInfo(Game.Instance.IsInGame.Value);
			AdTrackingSystemLogger.LogVideoAdButtonClick(currentLocationInfo);
			hasSkippedMission = true;
			AdSystemManager.Show(currentLocationInfo, OnFinishedWatchingVideo);
			isVideoPlaying = true;
			Breadcrumb.Consume(Breadcrumb.BreadcrumbEvent.MissionSet);
		}

		private void OnShowVideoFailure(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
		{
			if (isVideoPlaying)
			{
				OnFinishedWatchingVideo();
			}
		}

		private void OnVideoOverlayClosed()
		{
			if (isVideoPlaying)
			{
				OnFinishedWatchingVideo();
			}
		}

		private void OnFinishedWatchingVideo()
		{
			if (isVideoPlaying)
			{
				_skipWithVideoButton.isEnabled = false;
				TrackSuccessfulSkipFlurry(true);
				MissionsManager.Instance.SkipMission(_payload.missionIndex, true);
				AnimateSkip();
				isVideoPlaying = false;
			}
		}

		private void OnMissionProgress(int missionIndex, float completionFactor)
		{
			if (base.gameObject.activeInHierarchy && missionIndex == _payload.missionIndex && !hasSkippedMission)
			{
				_skipWithKeysButton.isEnabled = false;
				_skipWithVideoButton.isEnabled = false;
				StartCoroutine(AnimateBackgroundProgress((completionFactor * (float)_payload.missionInfo.mission.goal).RoundToInt()));
			}
		}

		public void OnPurchaseSuccessful()
		{
			TrackSuccessfulSkipFlurry();
			MissionsManager.Instance.SkipMission(_payload.missionIndex, false);
			AnimateSkip();
		}

		[Button(null)]
		[UsedImplicitly]
		public void AnimateSkip()
		{
			int completedMissionsCount = MissionsManager.Instance.GetMissionInfo().Count((MissionInfo m) => m.complete);
			StartCoroutine(CompleteAnimationInternal(completedMissionsCount));
		}

		private void TrackSuccessfulSkipFlurry(bool skippedWithVideo = false)
		{
			string text = MissionsManager.Instance.currentMissionSet.ToString();
			text = text + "-" + _payload.missionIndex;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			bool flag = ((_payload.source != PayloadSource.ChallengesScreen) ? (_payload.cellHeight > 80) : (_payload.cellHeight > 109));
			dictionary.Add("Mission Set and Index", text);
			dictionary.Add("Mission Skipped From", _payload.source.ToString());
			dictionary.Add("Can skip from pause", flag.ToString());
			dictionary2.Add("Mission Set and Index", text);
			if (MissionsManager.Instance.videoButtonState != MissionsManager.VideoButtonTrackingState.NotSet)
			{
				string empty = string.Empty;
				empty = (skippedWithVideo ? "Used" : ((MissionsManager.Instance.videoButtonState != MissionsManager.VideoButtonTrackingState.Fill) ? MissionsManager.Instance.videoButtonState.ToString() : ("AvailableFor_" + PlayerSave.currentMissionSet + "-" + PlayerSave.currentSkipForVideoMissionIndex + "_NotUsed")));
				dictionary.Add("Video", empty);
				dictionary2.Add("Video", empty);
			}
			Flurry.LogEventWithSeveralParameters("Boost Mission Skip purchased", dictionary2);
			Flurry.LogEventWithSeveralParameters("Boost Mission Skip info", dictionary);
		}

		public void OnPurchaseFailure()
		{
			hasSkippedMission = false;
			_skipWithKeysButton.isEnabled = true;
			_skipWithVideoButton.isEnabled = true;
		}

		public void RefreshIfNotNewlyCreated(MissionInfo info)
		{
			if (!_isNewlyCreated)
			{
				_payload.missionInfo = info;
				Refresh();
			}
			_isNewlyCreated = false;
		}

		public void Refresh()
		{
			hasSkippedMission = false;
			RefreshState();
			UpdateText();
			UpdateProgress();
			ResetAnimations();
			ShowSkipButton();
		}

		public void Refresh(MissionInfo info)
		{
			_payload.missionInfo = info;
			Refresh();
		}

		private void RefreshState()
		{
			bool missionComplete = _payload.missionInfo.complete;
			_backgroundSprite.color = ((!missionComplete) ? ColorData.instance.pigeon : ColorData.instance.lime);
			_progressComplete.SetActive(missionComplete && !IsBottomBoxActive());
			_missionProgress.gameObject.SetActive(!_payload.missionInfo.complete || IsBottomBoxActive());
			_completedLabel.gameObject.SetActive(missionComplete);
			_checkmarkSprite.gameObject.SetActive(missionComplete);
			_completedGlows.ForEach((UISprite s) =>
			{
				s.gameObject.SetActive(missionComplete);
				s.alpha = 1f;
			});
			_inProgressDisplay.gameObject.SetActive(!missionComplete);
			_skipWithKeysButton.isEnabled = !missionComplete;
			_skipWithVideoButton.isEnabled = !missionComplete;
		}

		private void ShowSkipButton()
		{
			if (!hasSkippedMission)
			{
				if (!MissionsManager.Instance.CanThisMissionSkipWithVideo(_payload))
				{
					_skipWithKeysButton.gameObject.SetActive(true);
					_skipWithVideoButton.gameObject.SetActive(false);
				}
				else
				{
					_skipWithKeysButton.gameObject.SetActive(false);
					_skipWithVideoButton.gameObject.SetActive(true);
				}
				LogAttemptToShowButtonFlow();
			}
		}

		private void LogAttemptToShowButtonFlow()
		{
			MissionsManager.VideoButtonTrackingState videoButtonState = MissionsManager.Instance.videoButtonState;
			if (videoButtonState == MissionsManager.VideoButtonTrackingState.Fill || videoButtonState == MissionsManager.VideoButtonTrackingState.NoFill)
			{
				AdLocationInfo currentLocationInfo = MissionsManager.GetCurrentLocationInfo(Game.Instance.IsInGame.Value);
				AdTrackingSystemLogger.LogAttemptToShowButtonFlow(currentLocationInfo);
			}
		}

		[Button(null)]
		[UsedImplicitly]
		private void ResetAnimations()
		{
			UpdateProgress();
			_checkmarkSprite.transform.localScale = Vector3.one;
			_completedLabel.gameObject.transform.localScale = Vector3.one;
			_missionProgress.transform.localScale = Vector3.one;
			_progressFlashSprite.alpha = 1f;
			_progressFlashSprite.gameObject.SetActive(false);
		}

		private IEnumerator AnimateBackgroundProgress(int finalProgress)
		{
			if (finalProgress < _payload.missionInfo.mission.goal)
			{
				StartCoroutine(AnimateProgressBarCoroutine(finalProgress));
				yield return new WaitForSecondsRealtime(1f);
				_skipWithKeysButton.isEnabled = true;
				_skipWithVideoButton.isEnabled = true;
			}
			else
			{
				int completedMissionsCount = MissionsManager.Instance.GetMissionInfo().Count((MissionInfo m) => m.complete);
				StartCoroutine(CompleteAnimationInternal(completedMissionsCount));
			}
		}

		private IEnumerator CompleteAnimationInternal(int completedMissionsCount)
		{
			StartCoroutine(AnimateProgressBarCoroutine(_payload.missionInfo.mission.goal));
			yield return new WaitForSecondsRealtime(1f);
			StartCoroutine(AnimateBottomPanelCoroutine());
			yield return new WaitForSecondsRealtime(0.8f);
			_payload.onMissionCellAnimationComplete.SafeInvoke(completedMissionsCount);
		}

		private IEnumerator AnimateProgressBarCoroutine(int finalProgress)
		{
			float animateDuration = 1f;
			float currentDuration = 0f;
			float startProgress = _currentProgress;
			float firstInterval = animateDuration / 2f;
			float secondInterval = animateDuration / 2f;
			while (currentDuration < firstInterval)
			{
				currentDuration += Time.unscaledDeltaTime;
				float p = currentDuration / firstInterval;
				float currentValue = Mathf.Lerp(startProgress, finalProgress, p);
				_missionProgress.text = currentValue.RoundToInt().ToString();
				_currentProgress = currentValue.RoundToInt();
				_progressSlider.value = currentValue / (float)_payload.missionInfo.mission.goal;
				yield return null;
			}
			currentDuration = 0f;
			_progressFlashSprite.gameObject.SetActive(true);
			while (currentDuration < secondInterval)
			{
				currentDuration += Time.unscaledDeltaTime;
				float p2 = currentDuration / secondInterval;
				_progressFlashSprite.alpha = 1f - p2;
				float plot = Easings.ElasticEaseOut(p2);
				if (p2 > 372f / 1023f)
				{
					plot--;
					plot /= 5f;
					plot++;
				}
				Vector3 newScale = plot * Vector3.one;
				_missionProgress.transform.localScale = newScale;
				yield return null;
			}
		}

		private IEnumerator AnimateBottomPanelCoroutine()
		{
			float animateDuration = 1f;
			float currentDuration = 0f;
			_inProgressDisplay.gameObject.SetActive(false);
			_checkmarkSprite.gameObject.SetActive(false);
			_completedLabel.gameObject.SetActive(true);
			_backgroundSprite.color = ColorData.instance.lime;
			_completedGlows.ForEach((UISprite s) =>
			{
				s.gameObject.SetActive(true);
				s.alpha = 0f;
			});
			float firstInterval = animateDuration / 2f;
			float secondInterval = animateDuration / 2f;
			while (currentDuration < firstInterval)
			{
				currentDuration += Time.unscaledDeltaTime;
				float p = currentDuration / secondInterval;
				float plot = Easings.ElasticEaseOut(p);
				Vector3 newScale = plot * Vector3.one;
				_completedLabel.gameObject.transform.localScale = newScale;
				float alphaPlot = Easings.CubicEaseIn(p);
				_completedGlows.ForEach((UISprite s) =>
				{
					s.alpha = alphaPlot;
				});
				yield return null;
			}
			_completedLabel.gameObject.transform.localScale = Vector3.one;
			currentDuration = 0f;
			_checkmarkSprite.gameObject.SetActive(true);
			_completedGlows.ForEach((UISprite s) =>
			{
				s.alpha = 1f;
			});
			while (currentDuration < secondInterval)
			{
				currentDuration += Time.unscaledDeltaTime;
				float p2 = currentDuration / secondInterval;
				float plot2 = Easings.ElasticEaseOut(p2);
				Vector3 newScale2 = plot2 * Vector3.one;
				_checkmarkSprite.transform.localScale = newScale2;
				yield return null;
			}
			_checkmarkSprite.transform.localScale = Vector3.one;
		}
	}
}
