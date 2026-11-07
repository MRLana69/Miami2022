using System;
using System.Collections;
using System.Collections.Generic;
using Kiloo.Common.Extensions;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace UI
{
	public class FacebookLoginPromotion : MonoBehaviour
	{
		[FormerlySerializedAs("_tilte")]
		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _subtitle;

		[SerializeField]
		private UISprite _backgroundSprite;

		[SerializeField]
		private GameObject _loginButton;

		[SerializeField]
		private DinoLogoCharacterHelper _dinoLogoPrefab;

		[SerializeField]
		private float _starAnimationPause;

		[SerializeField]
		private float _starCountDown;

		[SerializeField]
		private List<GameObject> _starPosition;

		[SerializeField]
		private GameObject _starPrefab;

		[SerializeField]
		private GameObject _reconnectContent;

		[SerializeField]
		private UIWidget _modelContainer;

		[SerializeField]
		private int _subPanelOffset;

		private List<GameObject> _stars = new List<GameObject>();

		private DinoLogoCharacterHelper _dino;

		private bool _runAnimations;

		private Action _clickedAction;

		public void Init(string screen, Action clickedAction = null)
		{
			if (SafeArea.IsRestricted)
			{
				GetComponent<UIWidget>().ResetAndUpdateAnchors();
			}
			_clickedAction = clickedAction;
			if (_reconnectContent != null)
			{
				_reconnectContent.SetActive(false);
			}
			ActivateLoginButton(screen, clickedAction);
			SetColors();
			switch (screen)
			{
			case "TopRunUI":
				_title.text = Strings.Get("FACEBOO_PROMOTION_TOPRUN_TITEL");
				_subtitle.text = Strings.Get("FACEBOO_PROMOTION_TOPRUN_SUBTITEL");
				break;
			case "LeaderboardUI":
				_title.text = Strings.Get("FACEBOOK_PROMOTION_LEADERBOARD_TITEL");
				_subtitle.text = Strings.Get("FACEBOOK_PROMOTION_LEADERBOARD_SUBTITEL");
				CreateDinoLogo();
				break;
			case "FriendsUI":
				_title.text = Strings.Get("FACEBOOK_PROMOTION_COLLECT_TITEL");
				_subtitle.text = Strings.Get("FACEBOOK_PROMOTION_COLLECT_SUBTITEL");
				CreateDinoLogo();
				break;
			}
			if (_starPosition.Count > 0 && _stars.Count <= 0)
			{
				_runAnimations = true;
				if (base.gameObject.activeInHierarchy)
				{
					StartCoroutine(ActivateStars());
				}
			}
		}

		[Button(null)]
		private void SetColors()
		{
			ColorData instance = ColorData.instance;
			_title.color = instance.white;
			_title.effectColor = instance.shadow;
			_subtitle.color = instance.sky;
			_subtitle.effectColor = instance.shadow;
			_backgroundSprite.color = instance.ocean;
			if (!Application.isPlaying)
			{
				base.gameObject.SetActive(false);
				base.gameObject.SetActive(true);
			}
		}

		private IEnumerator ActivateStars()
		{
			foreach (GameObject item in _starPosition)
			{
				_stars.Add(UnityEngine.Object.Instantiate(_starPrefab, item.transform.position, Quaternion.Euler(0f, 0f, 0f), item.transform));
			}
			yield return new WaitForSeconds(2f);
			while (_runAnimations)
			{
				foreach (GameObject star in _stars)
				{
					int children = star.transform.childCount;
					for (int i = 0; i < children; i++)
					{
						star.transform.GetChild(i).GetComponent<Animation>().Play();
					}
					yield return new WaitForSeconds(_starCountDown);
				}
				yield return new WaitForSeconds(_starAnimationPause);
			}
		}

		private void OnDisable()
		{
			if (_stars == null)
			{
				return;
			}
			_runAnimations = false;
			foreach (GameObject star in _stars)
			{
				UnityEngine.Object.Destroy(star);
			}
			_stars.Clear();
		}

		private void CreateDinoLogo()
		{
			if (_dino == null)
			{
				_dino = UnityEngine.Object.Instantiate(_dinoLogoPrefab, _modelContainer.transform);
				_dino.transform.Reset();
				_dino.Init(new DinoLogoCharacterHelper.DinoLogoPayload
				{
					panelOffset = _subPanelOffset
				});
			}
		}

		private void ActivateLoginButton(string screen, Action clickedAction = null)
		{
			switch (screen)
			{
			case "TopRunUI":
				_loginButton.GetComponent<FacebookLoginButtonOnline>().Init(SocialManager.instance.facebookIsLoggedIn, false, SocialManager.instance.gamecenterIsLoggedIn, true);
				break;
			case "LeaderboardUI":
			case "FriendsUI":
				_loginButton.GetComponent<FriendsLoginFBButton>().Init(clickedAction, true, true, false, ActivateReconnectBar);
				break;
			}
		}

		private void ActivateReconnectBar()
		{
			_reconnectContent.SetActive(true);
			_reconnectContent.GetComponent<FacebookPromotionReconnect>().Init(new FacebookPromotionReconnect.ReconnectPayload
			{
				clickedAction = _clickedAction,
				panelOffset = _subPanelOffset
			});
			base.gameObject.SetActive(false);
		}
	}
}
