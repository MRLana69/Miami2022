using System;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace Kiloo.Common.Ads
{
	public class DebugAdGUI : MonoBehaviour
	{
		private const int DEFAULT_CLOSE_BUTTON_SIZE = 60;

		private const int VIDEO_LENGTH = 5;

		private const int DEFAULT_TEXT_FONT_SIZE = 20;

		private bool _showVideo;

		private bool _showInterstitial;

		private string _adProvider;

		private float _timeLeftForVideo;

		private int _closeButtonSize;

		private GUIStyle _interstitialGuiStyle;

		private GUIStyle _videoGuiStyle;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action finishedWatchingVideo__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<AdType> dismissedAd__BackingField;

		public AdCacheSlotInfo adCacheSlotInfo { get; private set; }

		public AdLocationInfo adLocationInfo { get; private set; }

		public event Action finishedWatchingVideo
		{
			add
			{
				Action action = finishedWatchingVideo__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref finishedWatchingVideo__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = finishedWatchingVideo__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref finishedWatchingVideo__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<AdType> dismissedAd
		{
			add
			{
				Action<AdType> action = dismissedAd__BackingField;
				Action<AdType> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref dismissedAd__BackingField, (Action<AdType>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<AdType> action = dismissedAd__BackingField;
				Action<AdType> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref dismissedAd__BackingField, (Action<AdType>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		[UsedImplicitly]
		private void Awake()
		{
			GUIStyle gUIStyle = new GUIStyle();
			gUIStyle.fontSize = 20;
			gUIStyle.alignment = TextAnchor.UpperCenter;
			_closeButtonSize = 60;
			SetupInterstitialGuiStyle(gUIStyle);
			SetupVideoGuiStyle(gUIStyle);
		}

		private void SetupInterstitialGuiStyle(GUIStyle baseGuiStyle)
		{
			_interstitialGuiStyle = new GUIStyle(baseGuiStyle);
			_interstitialGuiStyle.normal.textColor = Color.yellow;
			_interstitialGuiStyle.normal.background = MakeTexture(1, 1, new Color(0f, 0f, 0f, 0.88f));
		}

		private void SetupVideoGuiStyle(GUIStyle baseGuiStyle)
		{
			_videoGuiStyle = new GUIStyle(baseGuiStyle);
			_videoGuiStyle.normal.textColor = Color.green;
		}

		public void SetGuiProperties(int closeButtonSize, int textFontSize)
		{
			_closeButtonSize = closeButtonSize;
			_interstitialGuiStyle.fontSize = textFontSize;
			_videoGuiStyle.fontSize = textFontSize;
		}

		public void ShowInterstitial(AdCacheSlotInfo adSlot, AdLocationInfo adLocation, string adProvider)
		{
			_showInterstitial = true;
			_showVideo = false;
			_adProvider = adProvider;
			adCacheSlotInfo = adSlot;
			adLocationInfo = adLocation;
		}

		public void ShowVideo(AdCacheSlotInfo adSlot, AdLocationInfo adLocation, string adProvider)
		{
			_showVideo = true;
			_showInterstitial = false;
			_adProvider = adProvider;
			_timeLeftForVideo = 5f;
			adCacheSlotInfo = adSlot;
			adLocationInfo = adLocation;
		}

		[UsedImplicitly]
		private void OnGUI()
		{
			Color color = GUI.color;
			int depth = GUI.depth;
			GUI.depth = int.MinValue;
			if (_showInterstitial)
			{
				GUI.color = Color.yellow;
				GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), "Interstitial from " + _adProvider, _interstitialGuiStyle);
				if (CloseButton())
				{
					_showInterstitial = false;
					dismissedAd__BackingField.SafeInvoke(AdType.Interstitial);
				}
				GUI.color = color;
			}
			else if (_showVideo)
			{
				GUI.color = Color.green;
				GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), "Video from " + _adProvider + "\n" + _timeLeftForVideo, _videoGuiStyle);
				_timeLeftForVideo -= Time.unscaledDeltaTime;
				if (_timeLeftForVideo <= 0f)
				{
					_showVideo = false;
					finishedWatchingVideo__BackingField.SafeInvoke();
					dismissedAd__BackingField.SafeInvoke(AdType.Video);
				}
				else if (CloseButton())
				{
					_showVideo = false;
					dismissedAd__BackingField.SafeInvoke(AdType.Video);
				}
				GUI.color = color;
			}
			GUI.depth = depth;
		}

		private Texture2D MakeTexture(int width, int height, Color color)
		{
			Color[] array = new Color[width * height];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = color;
			}
			Texture2D texture2D = new Texture2D(width, height);
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}

		private bool CloseButton()
		{
			return GUI.Button(new Rect(Screen.width - _closeButtonSize, 0f, _closeButtonSize, _closeButtonSize), "X");
		}
	}
}
