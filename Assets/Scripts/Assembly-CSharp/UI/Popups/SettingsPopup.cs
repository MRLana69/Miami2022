using System.Collections.Generic;
using Cloud;
using Cloud.Internal;
using Data.Save;
using Info;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Plugins.Notifications;
using NaughtyAttributes;
using SYBO.Subway;
using TopRun;
using UI.Breadcrumbs;
using UI.Generic.Layout;
using UI.Popups.SettingsPopupInternal;
using UI.SlideIns;
using UnityEngine;

namespace UI.Popups
{
	public class SettingsPopup : UIPopup
	{
		[SerializeField]
		private UISprite _fill;

		[SerializeField]
		private UISprite _outline;

		[SerializeField]
		private VerticalLayout _layout;

		[SerializeField]
		private UILabel _titleLabel;

		[SerializeField]
		private UILabel _version;

		[SerializeField]
		private UILabel _contactInfo;

		[SerializeField]
		private UIScrollView _scrollView;

		[SerializeField]
		private GameObject _debugButton;

		[SerializeField]
		private SettingsButton _settingsButtonPrefab;

		[SerializeField]
		private SettingsLogInFacebookButton _facebookLoginButtonPrefab;

		[SerializeField]
		private UISprite _settingsDividerPrefab;

		[SerializeField]
		private UIPanel _panel;

		private SettingsButton _topRunChallengersButton;

		private SettingsButton _facebookLogoutButton;

		private SettingsLogInFacebookButton _facebookLoginButton;

		private SettingsButton _reminderButton;

		private SettingsButton _supportButton;

		private SettingsButton _languageSelectionButton;

		private SettingsButton _withdrawConsentButton;

		private SettingsButton _onlineSaveButton;

		private SettingsButton _promoCodeButton;

		private List<UISprite> _extraDividers = new List<UISprite>();

		private const int BACKGROUND_FILL_HEIGHT_NORMAL = 432;

		private const int BACKGROUND_FILL_HEIGHT_TUTORIAL = 310;

		private const int BACKGROUND_FILL_HEIGHT_TUTORIAL_OFFSET = 60;

		private float _lastScrollPositionY;

		private float _instantiatedPanelYPosition;

		private int _previousButtonAmount;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			SetupBasic();
			_version.text = "v" + DeviceUtility.GetBundleVersion();
			_contactInfo.text = Globals.GetContactSupportShortUrl();
			if (!Globals.DEBUG_POPUP || !Globals.DEBUG)
			{
				Object.Destroy(_debugButton);
			}
			LoadAllButtons();
		}

		[Button(null)]
		public override void GainFocus()
		{
			// Ao voltar do menu de escolha do treino, atualiza o que está escrito nos botões
			foreach (SettingsButton button in _layout.GetComponentsInChildren<SettingsButton>(true))
			{
				button.RefreshAll();
			}
			_layout.gameObject.SetActive(false);
			_layout.gameObject.SetActive(true);
		}

		[Button(null)]
		private void SetupBasic()
		{
			_titleLabel.text = Strings.Get("SETTINGS_TITLE");
			_titleLabel.color = ColorData.instance.midnight;
		}

		private UISprite AddDivider()
		{
			UISprite uISprite = Object.Instantiate(_settingsDividerPrefab, _layout.transform);
			uISprite.color = ColorData.instance.push;
			uISprite.alpha = 0.5f;
			return uISprite;
		}

		private void LoadAllButtons()
		{
			SettingsButton settingsButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
			settingsButton.name = "musicButton";
			settingsButton.InitButton(new SettingsButton.SettingsButtonPayload
			{
				iconName = "icon_musicOn",
				iconOffset = new Vector3(-2f, 0f, 0f),
				onClick = Settings.ToggleCurrentMusicOption,
				setOfflineIndicator = (GameObject go) =>
				{
					go.SetActive(!Settings.optionMusic);
				},
				setTitle = (UILabel label) =>
				{
					label.text = Strings.Get("SETTINGS_POPUP_MUSIC");
				},
				setDescription = (UILabel desc) =>
				{
					desc.text = Strings.Get((!Settings.optionMusic) ? "SETTINGS_POPUP_OFF" : "SETTINGS_POPUP_ON");
				}
			});
			SettingsButton settingsButton2 = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
			settingsButton2.name = "sfxButton";
			settingsButton2.InitButton(new SettingsButton.SettingsButtonPayload
			{
				iconName = "icon_soundOn",
				onClick = Settings.ToggleCurrentSfxOption,
				setOfflineIndicator = (GameObject go) =>
				{
					go.SetActive(!Settings.optionSfx);
				},
				setTitle = (UILabel label) =>
				{
					label.text = Strings.Get("SETTINGS_POPUP_SFX");
				},
				setDescription = (UILabel desc) =>
				{
					desc.text = Strings.Get((!Settings.optionSfx) ? "SETTINGS_POPUP_OFF" : "SETTINGS_POPUP_ON");
				}
			});
			AddDivider();
			SettingsButton trainingModeButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
			trainingModeButton.name = "trainingModeButton";
			trainingModeButton.InitButton(new SettingsButton.SettingsButtonPayload
			{
				iconName = "icon_upgrades_superSneakers",
				onClick = () =>
				{
					TrainingPickerPopup.target = TrainingPickerPopup.PickerTarget.Mode;
					UIScreenController.Instance.PushPopup("TrainingPickerPopup");
				},
				setOfflineIndicator = (GameObject go) =>
				{
					go.SetActive(false);
				},
				setTitle = (UILabel label) =>
				{
					label.text = TrainingOptions.Tr("TRAINING_MODE");
				},
				setDescription = (UILabel desc) =>
				{
					desc.text = TrainingOptions.GetName(TrainingOptions.CurrentMode);
				},
				setIcon = TrainingOptions.FitIcon
			});
			SettingsButton impossibleTypeButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
			impossibleTypeButton.name = "impossibleTypeButton";
			impossibleTypeButton.InitButton(new SettingsButton.SettingsButtonPayload
			{
				iconName = "icon_trophy",
				onClick = () =>
				{
					TrainingPickerPopup.target = TrainingPickerPopup.PickerTarget.ImpossibleType;
					UIScreenController.Instance.PushPopup("TrainingPickerPopup");
				},
				setOfflineIndicator = (GameObject go) =>
				{
					go.SetActive(false);
				},
				setTitle = (UILabel label) =>
				{
					label.text = TrainingOptions.Tr("IMPOSSIBLE_TYPE");
				},
				setDescription = (UILabel desc) =>
				{
					desc.text = TrainingOptions.GetName(TrainingOptions.CurrentImpossibleType);
				},
				setIcon = TrainingOptions.FitIcon
			});
			SettingsButton noDeathButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
			noDeathButton.name = "noDeathButton";
			noDeathButton.InitButton(new SettingsButton.SettingsButtonPayload
			{
				iconName = "icon_upgrades_hoverboard",
				onClick = TrainingOptions.ToggleNoDeath,
				setOfflineIndicator = (GameObject go) =>
				{
					go.SetActive(!TrainingOptions.NoDeath);
				},
				setTitle = (UILabel label) =>
				{
					label.text = TrainingOptions.Tr("NO_DEATH");
				},
				setDescription = (UILabel desc) =>
				{
					desc.text = Strings.Get((!TrainingOptions.NoDeath) ? "SETTINGS_POPUP_OFF" : "SETTINGS_POPUP_ON");
				},
				setIcon = TrainingOptions.FitIcon
			});
			// Modo original do site (?mode=original): sem os botões de treino
			if (TrainingOptions.OriginalMode)
			{
				trainingModeButton.gameObject.SetActive(false);
				impossibleTypeButton.gameObject.SetActive(false);
				noDeathButton.gameObject.SetActive(false);
			}
			SettingsButton autoBoostButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
			autoBoostButton.name = "autoBoostButton";
			autoBoostButton.InitButton(new SettingsButton.SettingsButtonPayload
			{
				iconName = "icon_upgrades_headstart2000",
				onClick = TrainingOptions.ToggleAutoBoost,
				setOfflineIndicator = (GameObject go) =>
				{
					go.SetActive(!TrainingOptions.AutoBoost);
				},
				setTitle = (UILabel label) =>
				{
					label.text = TrainingOptions.Tr("AUTO_BOOST");
				},
				setDescription = (UILabel desc) =>
				{
					desc.text = Strings.Get((!TrainingOptions.AutoBoost) ? "SETTINGS_POPUP_OFF" : "SETTINGS_POPUP_ON");
				},
				setIcon = TrainingOptions.FitIcon
			});
			SettingsButton tripleTapButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
			tripleTapButton.name = "tripleTapButton";
			tripleTapButton.InitButton(new SettingsButton.SettingsButtonPayload
			{
				iconName = "icon_hoverboard",
				onClick = TrainingOptions.ToggleTripleTapBoard,
				setOfflineIndicator = (GameObject go) =>
				{
					go.SetActive(!TrainingOptions.TripleTapBoard);
				},
				setTitle = (UILabel label) =>
				{
					label.text = TrainingOptions.Tr("TRIPLE_TAP");
				},
				setDescription = (UILabel desc) =>
				{
					desc.text = Strings.Get((!TrainingOptions.TripleTapBoard) ? "SETTINGS_POPUP_OFF" : "SETTINGS_POPUP_ON");
				},
				setIcon = TrainingOptions.FitIcon
			});
			SettingsButton coinGlowButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
			coinGlowButton.name = "coinGlowButton";
			coinGlowButton.InitButton(new SettingsButton.SettingsButtonPayload
			{
				iconName = "icon_coin_large",
				onClick = TrainingOptions.ToggleCoinGlow,
				setOfflineIndicator = (GameObject go) =>
				{
					go.SetActive(!TrainingOptions.CoinGlow);
				},
				setTitle = (UILabel label) =>
				{
					label.text = TrainingOptions.Tr("COIN_GLOW");
				},
				setDescription = (UILabel desc) =>
				{
					desc.text = Strings.Get((!TrainingOptions.CoinGlow) ? "SETTINGS_POPUP_OFF" : "SETTINGS_POPUP_ON");
				},
				setIcon = TrainingOptions.FitIcon
			});
			SettingsButton coinTextureButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
			coinTextureButton.name = "coinTextureButton";
			coinTextureButton.InitButton(new SettingsButton.SettingsButtonPayload
			{
				iconName = "icon_coin_large",
				onClick = TrainingOptions.ToggleCoinTexture,
				setOfflineIndicator = (GameObject go) =>
				{
					go.SetActive(false);
				},
				setTitle = (UILabel label) =>
				{
					label.text = TrainingOptions.Tr("COIN_TEXTURE");
				},
				setDescription = (UILabel desc) =>
				{
					desc.text = TrainingOptions.Tr(TrainingOptions.CoinTextureAtual ? "COIN_CURRENT" : "COIN_CLASSIC");
				},
				setIcon = TrainingOptions.FitIcon
			});
			SettingsButton graphicsButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
			graphicsButton.name = "graphicsButton";
			graphicsButton.InitButton(new SettingsButton.SettingsButtonPayload
			{
				iconName = "icon_sparkle_star",
				onClick = TrainingOptions.ToggleGraphics,
				setOfflineIndicator = (GameObject go) =>
				{
					go.SetActive(false);
				},
				setTitle = (UILabel label) =>
				{
					label.text = TrainingOptions.Tr("GRAPHICS");
				},
				setDescription = (UILabel desc) =>
				{
					desc.text = TrainingOptions.GraphicsName(TrainingOptions.GraphicsLevel);
				},
				setIcon = TrainingOptions.FitIcon
			});
			AddDivider();
			_languageSelectionButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
			_languageSelectionButton.name = "languageSelectionButton";
			_languageSelectionButton.InitButton(new SettingsButton.SettingsButtonPayload
			{
				iconName = "icon_language_settings",
				iconOffset = new Vector3(-1f, 0f, 0f),
				onClick = () =>
				{
					UIScreenController.Instance.PushPopup("LanguageSelectionPopup");
				},
				setOfflineIndicator = (GameObject go) =>
				{
					go.SetActive(false);
				},
				setTitle = (UILabel label) =>
				{
					label.text = Strings.Get("SETTINGS_POPUP_LANGUAGE_TITLE");
				},
				setDescription = (UILabel desc) =>
				{
					desc.text = Strings.language.locale.nativeName;
				}
			});
			_facebookLogoutButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
			_facebookLogoutButton.name = "fbLogOutButton";
			_facebookLogoutButton.InitButton(new SettingsButton.SettingsButtonPayload
			{
				iconName = "settings_facebook_icon",
				onClick = () =>
				{
					SocialManager.instance.FacebookLogout();
					UIScreenController.Instance.QueueSlideIn(new SlideIn
					{
						type = SlideInData.SlideInType.Facebook,
						facebookState = UISlideInFacebook.FacebookSlideInState.LoggedOut
					});
					IManager manager = TopRunRoot.manager;
					if (manager != null)
					{
						manager.RefreshTournament();
					}
					LoggedOutOfFacebook();
				},
				setOfflineIndicator = (GameObject go) =>
				{
					go.SetActive(false);
				},
				setTitle = (UILabel label) =>
				{
					label.text = Strings.Get("SETTINGS_POPUP_FB_TITLE");
				},
				descriptionID = "SETTINGS_POPUP_FB_LOG_OUT_DESC"
			});
			_facebookLoginButton = Object.Instantiate(_facebookLoginButtonPrefab, _layout.transform);
			_facebookLoginButton.name = "facebookLoginButton";
			_facebookLoginButton.InitButton(this);
			_reminderButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
			_reminderButton.name = "reminderButton";
			_reminderButton.InitButton(new SettingsButton.SettingsButtonPayload
			{
				iconName = "icon_reminder",
				onClick = Settings.ToggleOptionReminders,
				setOfflineIndicator = (GameObject go) =>
				{
					go.SetActive(!Settings.optionReminder);
				},
				setTitle = (UILabel label) =>
				{
					label.text = Strings.Get("SETTINGS_POPUP_REMINDERS");
				},
				setDescription = (UILabel desc) =>
				{
					desc.text = Strings.Get((!Settings.optionReminder) ? "SETTINGS_POPUP_DISABLED" : "SETTINGS_POPUP_ENABLED");
				}
			});
			AddDivider();
			_withdrawConsentButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
			_withdrawConsentButton.name = "withdrawConsentButton";
			_withdrawConsentButton.InitButton(new SettingsButton.SettingsButtonPayload
			{
				iconName = "icon_consent",
				iconOffset = new Vector3(0f, 1f, 0f),
				onClick = () =>
				{
					UIScreenController.Instance.PushPopup("WithdrawConsentPopup");
				},
				setOfflineIndicator = (GameObject go) =>
				{
					go.SetActive(false);
				},
				setTitle = (UILabel label) =>
				{
					label.text = Strings.Get("SETTINGS_POPUP_WITHDRAW_CONSENT");
				},
				setDescription = (UILabel desc) =>
				{
					desc.gameObject.SetActive(false);
				}
			});
			SettingsButton settingsButton4 = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
			settingsButton4.name = "privacyPolicyButton";
			settingsButton4.InitButton(new SettingsButton.SettingsButtonPayload
			{
				iconName = "popup_settings_i",
				iconOffset = new Vector3(0f, 1f, 0f),
				onClick = () =>
				{
					ConfirmationPopup.variant = ConfirmationPopupVariant.OpenPrivacyPolicy;
					UIScreenController.Instance.PushPopup("ConfirmationPopup");
				},
				setOfflineIndicator = (GameObject go) =>
				{
					go.SetActive(false);
				},
				setTitle = (UILabel label) =>
				{
					label.text = Strings.Get("SETTINGS_POPUP_PRIVACY");
				},
				descriptionID = "SETTINGS_POPUP_OPEN_BROWSER"
			});
		}

		protected override void Show()
		{
			bool tutorialCompleted = OnboardingSave.isTutorialCompleted;
			AddHooks();
			_reminderButton.gameObject.SetActive(tutorialCompleted && Notifications.instance.areNotificationsEnabled);
			_withdrawConsentButton.gameObject.SetActive(!AgeRestriction.isRestricted);
			_languageSelectionButton.gameObject.SetActive(Game.IsInstantiated && !Game.Instance.IsInGame.Value);
			_extraDividers.ForEach((UISprite s) =>
			{
				s.gameObject.SetActive(tutorialCompleted);
			});
			if (!tutorialCompleted || AgeRestriction.isRestricted || Game.Instance.IsInGame.Value)
			{
				_facebookLogoutButton.gameObject.SetActive(false);
				_facebookLoginButton.gameObject.SetActive(false);
			}
			else
			{
				Globals.LogInState loginState = Globals.GetLoginState();
				if (loginState != Globals.LogInState.Facebook && loginState != Globals.LogInState.Both)
				{
					_facebookLogoutButton.gameObject.SetActive(false);
					_facebookLoginButton.gameObject.SetActive(false);
				}
				else
				{
					_facebookLogoutButton.gameObject.SetActive(false);
					_facebookLoginButton.gameObject.SetActive(false);
				}
			}
			UpdatePopupDimension(tutorialCompleted);
			_outline.UpdateAnchors();
			_version.UpdateAnchors();
			_contactInfo.UpdateAnchors();
			_layout.Align();
			_scrollView.ResetPosition();
			ScrollToPreviousPositionIfNeeded();
		}

		private void AddHooks()
		{
			UIScreenController.OnApplicationResumed += OnApplicationResumed;
		}

		private void UpdatePopupDimension(bool tutorialCompleted)
		{
			int num;
			if (!tutorialCompleted)
			{
				num = 310;
				if (_languageSelectionButton.gameObject.activeSelf)
				{
					num += 60;
				}
				if (_withdrawConsentButton.gameObject.activeSelf)
				{
					num += 60;
				}
			}
			else
			{
				num = 432;
			}
			num = 432; // com os botões de treino a lista é sempre do tamanho cheio (rola)
			_fill.height = num;
			UIRect[] componentsInChildren = _fill.GetComponentsInChildren<UIRect>();
			UIRect[] array = componentsInChildren;
			foreach (UIRect uIRect in array)
			{
				uIRect.ResetAndUpdateAnchors();
			}
		}

		protected override void Hide()
		{
			UpdatePopupDimension(OnboardingSave.isTutorialCompleted);
			RemoveHooks();
			SaveCurrentScrollPosition();
		}

		private void RemoveHooks()
		{
			UIScreenController.OnApplicationResumed -= OnApplicationResumed;
		}

		private void OnApplicationResumed()
		{
			_reminderButton.gameObject.SetActive(OnboardingSave.isTutorialCompleted && Notifications.instance.areNotificationsEnabled);
			_layout.Align();
		}

		public void LoggedOutOfFacebook()
		{
			if (_facebookLogoutButton != null)
			{
				_facebookLogoutButton.gameObject.SetActive(false);
			}
			if (_facebookLoginButton != null)
			{
				_facebookLoginButton.gameObject.SetActive(false);
			}
			_layout.Align();
		}

		public void ConnectedToFacebook()
		{
			if (_facebookLogoutButton != null)
			{
				_facebookLogoutButton.gameObject.SetActive(false);
			}
			if (_facebookLoginButton != null)
			{
				_facebookLoginButton.gameObject.SetActive(false);
			}
			_layout.Align();
		}

		private void SaveCurrentScrollPosition()
		{
			_lastScrollPositionY = _instantiatedPanelYPosition + _panel.clipOffset.y;
			_previousButtonAmount = GetAmountOfButtons();
		}

		private void ScrollToPreviousPositionIfNeeded()
		{
			_instantiatedPanelYPosition = _panel.transform.localPosition.y;
			if (GetAmountOfButtons() == _previousButtonAmount)
			{
				_scrollView.MoveRelative(new Vector3(0f, 0f - _lastScrollPositionY, 0f));
			}
		}

		private int GetAmountOfButtons()
		{
			SettingsButton[] componentsInChildren = _layout.GetComponentsInChildren<SettingsButton>(false);
			if (componentsInChildren != null)
			{
				return componentsInChildren.Length;
			}
			return -1;
		}
	}
}
