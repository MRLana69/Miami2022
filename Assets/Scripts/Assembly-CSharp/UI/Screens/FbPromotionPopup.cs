using System.Collections;
using System.Collections.Generic;
using Info;
using Kiloo.Common.Extensions;
using NaughtyAttributes;
using UI.Generic.Layout;
using UI.Popups;
using UI.RenderTexture;
using UI.SlideIns;
using UnityEngine;

namespace UI.Screens
{
	public class FbPromotionPopup : UIPopup
	{
		[SerializeField]
		private UILabel _headlineLabel;

		[SerializeField]
		private UILabel _sublineLabel;

		[SerializeField]
		private UISprite _0BackgroundSprite;

		[SerializeField]
		private UISprite _1BackgroundSprite;

		[SerializeField]
		private UISprite _coinSignSprite;

		[SerializeField]
		private UISprite _coinSignTriangleSprite;

		[SerializeField]
		private UISprite _coinSignBGSprite;

		[SerializeField]
		private UISprite _coinSignTriangleBGSprite;

		[SerializeField]
		private UILabel _coinSignAmountLabel;

		[SerializeField]
		private UILabel _coinSignTextLabel;

		[SerializeField]
		private Layout _coinSignLayout;

		[SerializeField]
		private UISprite _dinoSignSprite;

		[SerializeField]
		private UISprite _dinoSignTriangleSprite;

		[SerializeField]
		private UISprite _dinoSignBGSprite;

		[SerializeField]
		private UISprite _dinoSignTriangleBGSprite;

		[SerializeField]
		private UILabel _dinoSignTextLabel;

		[SerializeField]
		private UISprite _coinGlowSprite;

		[SerializeField]
		private UILabel _buttonLabel;

		[SerializeField]
		private GameObject _facebookLogoSprite;

		[SerializeField]
		private List<GameObject> _starPosition;

		[SerializeField]
		private GameObject _starPrefab;

		[SerializeField]
		private float _starCountDown;

		[SerializeField]
		private Animation _buttonBounce;

		[SerializeField]
		private Animation _buttonSlideStar;

		[SerializeField]
		private FacebookDinoLogoRenderTextureSetup _dinoCharacterRenderTextureSetup;

		private List<GameObject> _stars;

		private bool _runAnimations;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Show()
		{
			if (SocialManager.instance.facebookIsLoggedIn)
			{
				UIScreenController.Instance.ClosePopup(screenName);
				return;
			}
			SetColors();
			Localization();
			if (_stars == null)
			{
				_stars = new List<GameObject>();
			}
			if (_starPosition.Count > 0 && _stars.Count <= 0)
			{
				_runAnimations = true;
				ActivateAnimations();
			}
		}

		protected override void Hide()
		{
			FacebookRewardsInfo.PayoutRewardIfPossible();
		}

		private void OnEnable()
		{
			ActivateDinoLogoAnimation();
		}

		private void Localization()
		{
			_headlineLabel.text = Strings.Get("FACEBOOK_PROMOTION_POPUP_HEADLINE");
			_sublineLabel.text = Strings.Get("FACEBOOK_PROMOTION_POPUP_SUBLINE");
			_coinSignAmountLabel.text = string.Format(Strings.Get("FACEBOOK_PROMOTION_COIN_CALLOUT_AMOUNT"), FacebookRewardsInfo.facebookRewardPack[FacebookRewardsInfo.FacebookRewardType.Coins]);
			_coinSignTextLabel.text = Strings.Get("FACEBOOK_PROMOTION_COIN_CALLOUT");
			_buttonLabel.text = Strings.Get("FACEBOOK_REWARDS_POPUP_CONNECT_BUTTON_TEXT");
			_facebookLogoSprite.SetActive(!SocialManager.instance.facebookIsLoggedIn);
			_coinSignLayout.Align();
		}

		[Button(null)]
		private void SetColors()
		{
			ColorData instance = ColorData.instance;
			_headlineLabel.color = instance.midnight;
			_sublineLabel.color = instance.peacock;
			_0BackgroundSprite.color = new Color32(139, 205, 234, byte.MaxValue);
			_1BackgroundSprite.color = new Color32(174, 148, 234, byte.MaxValue);
			_coinSignSprite.color = new Color32(byte.MaxValue, 204, 0, byte.MaxValue);
			_coinSignTriangleSprite.color = new Color32(byte.MaxValue, 204, 0, byte.MaxValue);
			_coinSignBGSprite.color = new Color32(189, 20, 33, byte.MaxValue);
			_coinSignTriangleBGSprite.color = new Color32(189, 20, 33, byte.MaxValue);
			_coinSignTextLabel.color = new Color32(189, 20, 33, byte.MaxValue);
			_coinSignAmountLabel.color = new Color32(189, 20, 33, byte.MaxValue);
			_dinoSignSprite.color = new Color32(28, 103, 158, byte.MaxValue);
			_dinoSignTriangleSprite.color = new Color32(28, 103, 158, byte.MaxValue);
			_dinoSignBGSprite.color = new Color32(0, 74, 128, byte.MaxValue);
			_dinoSignTriangleBGSprite.color = new Color32(0, 74, 128, byte.MaxValue);
			_dinoSignTextLabel.color = instance.white;
			_coinGlowSprite.color = new Color32(byte.MaxValue, 233, 144, byte.MaxValue);
			base.gameObject.Blink();
		}

		private void OnDisable()
		{
			_runAnimations = false;
			if (_stars == null)
			{
				return;
			}
			foreach (GameObject star in _stars)
			{
				Object.Destroy(star);
			}
			_stars.Clear();
		}

		private void ActivateAnimations()
		{
			foreach (GameObject item in _starPosition)
			{
				_stars.Add(Object.Instantiate(_starPrefab, item.transform.position, Quaternion.Euler(0f, 0f, 0f), item.transform));
			}
			StartCoroutine(RunAnimations());
		}

		private IEnumerator RunAnimations()
		{
			yield return new WaitForSeconds(2f);
			float starAnimationTime = 0f;
			while (_runAnimations)
			{
				for (int x = 0; x < _stars.Count - 1; x++)
				{
					int children = _stars[x].transform.childCount;
					for (int i = 0; i < children; i++)
					{
						_stars[x].transform.GetChild(i).GetComponent<Animation>().Play();
						if (starAnimationTime <= 0f)
						{
							starAnimationTime = _stars[x].transform.GetChild(i).GetComponent<Animation>().clip.length;
						}
					}
					yield return new WaitForSeconds(_starCountDown);
				}
				yield return new WaitForSeconds(starAnimationTime - _starCountDown);
				_buttonBounce.Play();
				yield return new WaitForSeconds(_buttonBounce.clip.length);
				int glideStarChildren = _stars[_stars.Count - 1].transform.childCount;
				for (int j = 0; j < glideStarChildren; j++)
				{
					_stars[_stars.Count - 1].transform.GetChild(j).GetComponent<Animation>().Play();
				}
				_buttonSlideStar.Play();
				yield return new WaitForSeconds(4f);
			}
		}

		private void ActivateDinoLogoAnimation()
		{
			_dinoCharacterRenderTextureSetup.Setup();
		}

		public void OnConnectClicked()
		{
			if (SocialManager.instance.facebookIsLoggedIn)
			{
				UIScreenController.Instance.CloseActivePopups(screenName);
				return;
			}
			if (SocialManager.instance.FacebookLogin(OnFacebookLoginResult))
			{
				UIScreenController.Instance.FacebookLogIn(true);
			}
			UIScreenController.Instance.CloseActivePopups(screenName);
		}

		private void OnFacebookLoginResult(bool loggedIn)
		{
			if (!loggedIn)
			{
				UIScreenController.Instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.Facebook,
					facebookState = UISlideInFacebook.FacebookSlideInState.ConnectFailed
				});
			}
		}
	}
}
