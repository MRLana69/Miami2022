using System;
using Data.Save;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Plugins.DeviceUtility;
using SYBO.Subway.Info;
using UI.Generic.Layout;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Popups
{
	public class UpdateAppPopup : CharacterPopup
	{
		private static bool _isUpdateNowOnScreen;

		private static bool _hasShownThisSession;

		[FormerlySerializedAs("title")]
		[SerializeField]
		private UILabel _title;

		[FormerlySerializedAs("buttonLabel")]
		[SerializeField]
		private UILabel _buttonLabel;

		[FormerlySerializedAs("changeListLabel")]
		[SerializeField]
		private UILabel _changeListLabel;

		[SerializeField]
		private Layout _buttonLayout;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			_title.text = Strings.Get("UPDATE_APP_POPUP_TITLE");
			_buttonLabel.text = Strings.Get("UPDATE_APP_POPUP_BUTTON");
			_buttonLayout.Align();
			_changeListLabel.color = ColorData.instance.midnight;
		}

		protected override void Show()
		{
			SetCharacter(Characters.CharacterId.slick, new Vector2(41f, 1f), null, 19f, false, null, false);
			string latestVersionChangelog = OnlineSettingsManager.localizedData.latestVersionChangelog;
			if (latestVersionChangelog != null)
			{
				_changeListLabel.text = latestVersionChangelog;
				return;
			}
			SDebug.LogError("Showing NewVersion popup, but no changeList found in OnlineSettings", this);
			_changeListLabel.text = Strings.Get("UPDATE_APP_POPUP_CHANGELOG_DEFAULT");
		}

		protected override void Hide()
		{
			HideCharacter();
		}

		[UsedImplicitly]
		public void CloseClicked()
		{
			ClosePopup();
			_isUpdateNowOnScreen = false;
			Flurry.LogClosePressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
		}

		[UsedImplicitly]
		public void OkClicked()
		{
			Application.OpenURL(Globals.AppStoreURL);
			ClosePopup();
			_isUpdateNowOnScreen = false;
			Flurry.LogOkPressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
		}

		public static void AllowAgainThisSession()
		{
			_hasShownThisSession = false;
		}

		public override void GainFocus()
		{
			ShowCharacter();
		}

		public override void LoseFocus()
		{
			HideCharacter();
		}

		public static void ShowIfNeeded()
		{
			if (_isUpdateNowOnScreen || !(UIScreenController.Instance.GetCurrentPopupName() != "EventsPopup") || string.IsNullOrEmpty(Globals.AppStoreURL) || !OnboardingSave.isTutorialCompleted)
			{
				return;
			}
			string latestVersion = OnlineSettingsManager.data.latestVersion;
			if (latestVersion == null)
			{
				return;
			}
			string latestVersionAndroidMinOsVersion = OnlineSettingsManager.data.latestVersionAndroidMinOsVersion;
			bool flag = true;
			if (latestVersionAndroidMinOsVersion != null)
			{
				string oSVersion = DeviceUtility.GetOSVersion();
				try
				{
					if (Utility.CompareVersions(oSVersion, latestVersionAndroidMinOsVersion) < 0)
					{
						flag = false;
					}
				}
				catch (FormatException ex)
				{
					SDebug.LogError("Failed to parse versions for comparison: " + oSVersion + " and " + latestVersionAndroidMinOsVersion + "  : " + ex);
				}
			}
			string bundleVersion = DeviceUtility.GetBundleVersion();
			bool flag2 = false;
			try
			{
				if (Utility.CompareVersions(bundleVersion, latestVersion) < 0)
				{
					flag2 = true;
				}
			}
			catch (FormatException ex2)
			{
				SDebug.LogError("Failed to parse versions for comparison: " + bundleVersion + " and " + latestVersion + "  : " + ex2);
			}
			if (flag && flag2 && !_hasShownThisSession)
			{
				UIScreenController.Instance.QueuePopup("UpdateAppPopup");
				_isUpdateNowOnScreen = true;
				_hasShownThisSession = true;
			}
		}
	}
}
