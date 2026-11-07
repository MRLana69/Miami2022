using Info;
using JetBrains.Annotations;
using UI.Generic.Layout;
using UI.SlideIns;
using UnityEngine;

namespace UI.Screens.FreeStuffScreenInternal
{
	public class FreeStuffSocialContainer : MonoBehaviour
	{
		[SerializeField]
		private UISprite _backgroundSprite;

		[SerializeField]
		private UISprite _outlineSprite;

		[SerializeField]
		private UISprite _glowSprite;

		[SerializeField]
		private UISprite _iconSprite;

		[SerializeField]
		private UISprite _logoSprite;

		[SerializeField]
		private UISprite _buttonCoinSprite;

		[SerializeField]
		private UISprite _logoPouchSprite;

		[SerializeField]
		private UILabel _buttonLabel;

		[SerializeField]
		private UILabel _buttonGetLabel;

		[SerializeField]
		private UILabel _titleLabel;

		[SerializeField]
		private UILabel _noPriceLabel;

		[SerializeField]
		private HorizontalLayout _buttonLayout;

		[SerializeField]
		private HorizontalLayout _buttonLayoutCoins;

		private EarnCoinsData.EarnCurrencyProfile _earnCurrencyProfile;

		private const string FACEBOOK_LOGO_NAME = "social_facebook_logo";

		private const string TWITTER_LOGO_NAME = "social_twitter_logo";

		private const string YOUTUBE_LOGO_NAME = "social_youtube_logo";

		private const string KILOO_LOGO_NAME = "icon_kiloo_logo";

		private const string SYBO_LOGO_NAME = "icon_sybo_logo";

		private const string CUSTOM_LINK_LOGO_NAME = "social_generic_link_icon";

		public void Init(EarnCoinsData.EarnCurrencyProfile earnCurrencyProfile)
		{
			_earnCurrencyProfile = earnCurrencyProfile;
			SetColors();
			SetIcon();
			SetTitle();
			if (_earnCurrencyProfile.defaultAmountOfCoins > 0)
			{
				_buttonLayoutCoins.gameObject.SetActive(true);
				_noPriceLabel.gameObject.SetActive(false);
				_buttonCoinSprite.gameObject.SetActive(true);
				_buttonGetLabel.gameObject.SetActive(true);
				_logoPouchSprite.gameObject.SetActive(true);
				_buttonLabel.gameObject.SetActive(true);
				_buttonGetLabel.text = Strings.Get("COIN_BUTTON_EARNER_GET");
				_buttonLabel.text = _earnCurrencyProfile.defaultAmountOfCoins.ToString();
				_buttonLayoutCoins.Align(true);
			}
			else
			{
				_buttonCoinSprite.gameObject.SetActive(false);
				_buttonGetLabel.gameObject.SetActive(false);
				_logoPouchSprite.gameObject.SetActive(false);
				_buttonLabel.gameObject.SetActive(false);
				_buttonLayoutCoins.gameObject.SetActive(false);
				if (!string.IsNullOrEmpty(_earnCurrencyProfile.buttonString))
				{
					_noPriceLabel.gameObject.SetActive(true);
					_noPriceLabel.text = _earnCurrencyProfile.buttonString;
				}
				else
				{
					_noPriceLabel.gameObject.SetActive(false);
					Debug.LogError("ButtonLabel is not set for: " + _earnCurrencyProfile.id);
				}
			}
			_buttonLayout.Align(true);
		}

		private void SetColors()
		{
			_backgroundSprite.gradientTop = ColorData.instance.tuna;
			_backgroundSprite.gradientBottom = ColorData.instance.pigeon;
			_outlineSprite.gradientTop = NGUIMath.HexToColor(1199743743u);
			_outlineSprite.gradientBottom = NGUIMath.HexToColor(1721876991u);
			_glowSprite.color = ColorData.instance.pigeon;
			_titleLabel.color = NGUIMath.HexToColor(5143295u);
			_titleLabel.effectColor = NGUIMath.HexToColor(uint.MaxValue);
		}

		private void SetIcon()
		{
			switch (_earnCurrencyProfile.id)
			{
			case EarnCoinsData.Id.FacebookKiloo:
			case EarnCoinsData.Id.FacebookSybo:
				_iconSprite.spriteName = "social_facebook_logo";
				break;
			case EarnCoinsData.Id.TwitterKiloo:
			case EarnCoinsData.Id.TwitterSybo:
				_iconSprite.spriteName = "social_twitter_logo";
				break;
			case EarnCoinsData.Id.YoutubeKiloo:
			case EarnCoinsData.Id.YoutubeSybo:
				_iconSprite.spriteName = "social_youtube_logo";
				break;
			case EarnCoinsData.Id.CustomLink:
				_iconSprite.spriteName = "social_generic_link_icon";
				break;
			case EarnCoinsData.Id.Review:
			case EarnCoinsData.Id.AdColony:
			case EarnCoinsData.Id.VideoAdCycle:
			case EarnCoinsData.Id.FacebookLogin:
			case EarnCoinsData.Id.FacebookWeeklyGift:
			case EarnCoinsData.Id.RewardForVideo:
				break;
			}
		}

		private void SetTitle()
		{
			switch (_earnCurrencyProfile.id)
			{
			case EarnCoinsData.Id.FacebookKiloo:
			case EarnCoinsData.Id.TwitterKiloo:
			case EarnCoinsData.Id.YoutubeKiloo:
				_logoSprite.gameObject.SetActive(true);
				_titleLabel.gameObject.SetActive(false);
				_logoSprite.spriteName = "icon_kiloo_logo";
				break;
			case EarnCoinsData.Id.FacebookSybo:
			case EarnCoinsData.Id.TwitterSybo:
			case EarnCoinsData.Id.YoutubeSybo:
				_logoSprite.gameObject.SetActive(true);
				_titleLabel.gameObject.SetActive(false);
				_logoSprite.spriteName = "icon_sybo_logo";
				break;
			case EarnCoinsData.Id.CustomLink:
				_logoSprite.gameObject.SetActive(false);
				_titleLabel.gameObject.SetActive(true);
				_titleLabel.text = _earnCurrencyProfile.title;
				break;
			case EarnCoinsData.Id.Review:
			case EarnCoinsData.Id.AdColony:
			case EarnCoinsData.Id.VideoAdCycle:
			case EarnCoinsData.Id.FacebookLogin:
			case EarnCoinsData.Id.FacebookWeeklyGift:
			case EarnCoinsData.Id.RewardForVideo:
				break;
			}
		}

		[UsedImplicitly]
		public void OnClick()
		{
			EarnCoinsData.Trigger(_earnCurrencyProfile);
			FreeStuffScreen.updateScreen = true;
			if (_earnCurrencyProfile.defaultAmountOfCoins > 0)
			{
				UIScreenController.Instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.OfferWall,
					stringValue = _earnCurrencyProfile.defaultAmountOfCoins.ToString(),
					purchase = false
				});
			}
		}
	}
}
