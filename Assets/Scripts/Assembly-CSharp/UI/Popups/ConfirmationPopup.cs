using System.Collections.Generic;
using AppRefresh;
using Data.Save;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.LocalizationManager;
using Kiloo.Common.Timer;
using SYBO.Components.Utilities;
using UnityEngine;

namespace UI.Popups
{
	public class ConfirmationPopup : UIPopup
	{
		private const string LEGACY_PRIVACY_POLICY_POPUP_NAME = "PrivacyPolicyPopup";

		public static ConfirmationPopupVariant variant;

		public static Locale requestedLocale;

		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _description;

		[SerializeField]
		private UILabel _greenButtonText;

		[SerializeField]
		private UILabel _redButtonText;

		private Dictionary<ConfirmationPopupVariant, ConfirmationPopupVariantObject> _confirmationPopupVariants;

		private static ConfirmationPopup _instance;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			CreateInstance(this);
			SetVariants();
			_title.color = ColorData.instance.midnight;
			_description.color = ColorData.instance.peacock;
		}

		protected override void Show()
		{
			if (IsVariantValid(variant))
			{
				_title.text = _confirmationPopupVariants[variant].titleText;
				_redButtonText.text = _confirmationPopupVariants[variant].redButtonText;
				_greenButtonText.text = _confirmationPopupVariants[variant].greenButtonText;
				_description.text = _confirmationPopupVariants[variant].getDescriptionText();
			}
		}

		public override void ClosePopup()
		{
			base.ClosePopup();
			if (variant == ConfirmationPopupVariant.OpenPrivacyPolicy)
			{
				Flurry.LogClosePressedOnPopup("POPUP Screen PrivacyPolicyPopup");
			}
		}

		[UsedImplicitly]
		public void GreenClicked()
		{
			if (IsVariantValid(variant))
			{
				_confirmationPopupVariants[variant].greenButtonAction();
			}
		}

		[UsedImplicitly]
		public void RedClicked()
		{
			if (IsVariantValid(variant))
			{
				_confirmationPopupVariants[variant].redButtonAction();
			}
		}

		private void QuitApplication()
		{
			SingletonMonoBehaviour<AudioStateLoop>.Instance.ForceStopMusic();
			UIScreenController.Instance.ignoreLastPopupClosed = true;
			UIScreenController.Instance.CloseActivePopups(screenName);
			if (!Application.isEditor)
			{
				Timer.ExecuteNextFrame(Application.Quit);
				return;
			}
			UIScreenController.Instance.CloseActivePopups(screenName);
			UIScreenController.Instance.QueueMessage("Stopping Playmode...");
		}

		private void CancelRun()
		{
			InventorySave.RewardUnrewardedCoins();
			UIScreenController.Instance.CloseActivePopups(screenName);
			UIScreenController.Instance.GoToMainMenuFromGame(base.gameObject);
		}

		private void ChangeLanguage()
		{
			PreferencesSave.language = requestedLocale.combined;
			AppRefreshHandler.StartRefresh(RefreshType.UI, () =>
			{
				UIScreenController.Instance.PushPopup("SettingsPopup");
				UIScreenController.Instance.PushPopup("LanguageSelectionPopup");
			});
		}

		private void ContactSupport()
		{
			UIScreenController.Instance.CloseActivePopups(screenName);
			Timer.ExecuteNextFrame(() =>
			{
				Application.OpenURL(Globals.GetContactSupportUrl());
			});
		}

		private void OpenPrivacyPolicy()
		{
			Flurry.LogOkPressedOnPopup("POPUP Screen PrivacyPolicyPopup");
			UIScreenController.Instance.CloseActivePopups(screenName);
			Timer.ExecuteNextFrame(() =>
			{
				Application.OpenURL(Globals.GetPrivacyPolicyUrl());
			});
		}

		private string GetLanguageDescriptionText()
		{
			string result = string.Empty;
			if (requestedLocale != null)
			{
				result = string.Format(Strings.Get("LANGUAGE_SELECTION_CONFIRMATION_POPUP_DESCRIPTION"), "[c]", ColorData.instance.royale.ToUIHex(), requestedLocale.nativeName, "[-][/c]");
			}
			return result;
		}

		private void SetVariants()
		{
			_confirmationPopupVariants = new Dictionary<ConfirmationPopupVariant, ConfirmationPopupVariantObject>
			{
				{
					ConfirmationPopupVariant.QuitApplication,
					new ConfirmationPopupVariantObject(Strings.Get("QUIT_APPLICATION_POPUP_TITLE"), Strings.Get("QUIT_APPLICATION_POPUP_DESCRIPTION").ToString, Strings.Get("QUIT_APPLICATION_POPUP_NO"), Strings.Get("QUIT_APPLICATION_POPUP_YES"), _instance.ClosePopup, _instance.QuitApplication)
				},
				{
					ConfirmationPopupVariant.CancelRun,
					new ConfirmationPopupVariantObject(Strings.Get("HOME_CONFIRMATION_POPUP_TITLE"), Strings.Get("HOME_CONFIRMATION_POPUP_DESCRIPTION").ToString, Strings.Get("HOME_CONFIRMATION_POPUP_NO"), Strings.Get("HOME_CONFIRMATION_POPUP_YES"), _instance.ClosePopup, _instance.CancelRun)
				},
				{
					ConfirmationPopupVariant.ContactSupport,
					new ConfirmationPopupVariantObject(Strings.Get("CONTACT_SUPPORT_POPUP_TITLE"), Strings.Get("CONTACT_SUPPORT_POPUP_DESCRIPTION").ToString, Strings.Get("CONTACT_SUPPORT_POPUP_NO"), Strings.Get("CONTACT_SUPPORT_POPUP_YES"), _instance.ClosePopup, _instance.ContactSupport)
				},
				{
					ConfirmationPopupVariant.OpenPrivacyPolicy,
					new ConfirmationPopupVariantObject(Strings.Get("PRIVACY_POLICY_POPUP_TITLE"), Strings.Get("PRIVACY_POLICY_POPUP_DESCRIPTION").ToString, Strings.Get("PRIVACY_POLICY_POPUP_NO"), Strings.Get("PRIVACY_POLICY_POPUP_YES"), _instance.ClosePopup, _instance.OpenPrivacyPolicy)
				},
				{
					ConfirmationPopupVariant.LanguageSelection,
					new ConfirmationPopupVariantObject(Strings.Get("LANGUAGE_SELECTION_CONFIRMATION_POPUP_TITLE"), _instance.GetLanguageDescriptionText, Strings.Get("OK"), Strings.Get("CANCEL"), _instance.ChangeLanguage, _instance.ClosePopup)
				}
			};
		}

		private bool IsVariantValid(ConfirmationPopupVariant confirmationPopupVariant)
		{
			if (_confirmationPopupVariants != null && _confirmationPopupVariants.ContainsKey(confirmationPopupVariant))
			{
				return true;
			}
			if (confirmationPopupVariant != ConfirmationPopupVariant.NotSet)
			{
				SDebug.LogError("ERROR - Attempted to use an undefined ConfirmationPopup variant! Fix in ConfirmationPopup.cs > _confirmationPopupVariants");
				ClosePopup();
			}
			return false;
		}

		private static void CreateInstance(ConfirmationPopup confirmationPopup)
		{
			if (_instance == null)
			{
				_instance = confirmationPopup;
			}
		}
	}
}
