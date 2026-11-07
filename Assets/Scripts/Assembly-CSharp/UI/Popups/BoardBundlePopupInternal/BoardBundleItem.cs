using System;
using System.Collections;
using System.Collections.Generic;
using Info;
using SYBO.Subway.Promotion;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.Popups.BoardBundlePopupInternal
{
	[Serializable]
	public class BoardBundleItem
	{
		private const float DEFAULT_SCALE_FACTOR = 11f;

		private const float TRAIL_PARTICLE_SYSTEM_START_ROTATION = 150f;

		private const float TRAIL_PARTICLE_SYSTEM_END_ROTATION = 210f;

		private const float TRAIL_PARTICLE_SYSTEM_RATE_OVER_TIME = 3f;

		private static readonly Vector3 MODEL_POSITION = new Vector3(5.5f, -2f, 50f);

		private static readonly Vector3 MODEL_LOCAL_ROTATION = new Vector3(27f, -87f, 15f);

		private static readonly Vector3 FX_POSITION = new Vector3(3.78f, 0.29f, 0f);

		private static readonly Vector3 TRAIL_SCALE = new Vector3(1f, 1f, 1f);

		private static readonly Quaternion TRAIL_ROTATION = Quaternion.Euler(-49.18f, 2.7f, 16.308f);

		private static readonly Vector3 TRAIL_PARTICLE_SYSTEM_POSITION = new Vector3(2.54f, -0.8f, 0f);

		[SerializeField]
		private Transform _boardAnchor;

		[SerializeField]
		private Transform _boardBottomAnchor;

		[SerializeField]
		private HorizontalLayout _boardLayouter;

		[SerializeField]
		private BundleIcon _boardIcon;

		[SerializeField]
		private UILabel _boardPriceLabel;

		[SerializeField]
		private List<BundleIcon> _bundleIcons;

		[SerializeField]
		private UISprite _background;

		[SerializeField]
		private List<UISprite> _bgFlares;

		[SerializeField]
		private UISprite _bgFlash;

		[SerializeField]
		private UISprite _bestSplat;

		[SerializeField]
		private GameObject _starsDecoration;

		private Boards.BoardId _boardId;

		private ATeleboardMenuEffect _teleboardMenuEffect;

		private GameObject _boardRoot;

		public void Init(Boards.BoardId boardId, string price)
		{
			_boardId = boardId;
			_boardPriceLabel.text = price;
			_boardIcon.SetupDefaultTheme(boardId);
		}

		public void GainFocus()
		{
			_boardAnchor.gameObject.SetActive(true);
			_boardBottomAnchor.gameObject.SetActive(true);
		}

		public void Show()
		{
			_boardRoot.transform.localScale = Vector3.one * 11f;
			if (_teleboardMenuEffect != null)
			{
				_teleboardMenuEffect.StopAllCoroutines();
				_teleboardMenuEffect.StartCoroutine(TeleboardEffect());
			}
		}

		public void LoseFocus()
		{
			_boardAnchor.gameObject.SetActive(false);
			_boardBottomAnchor.gameObject.SetActive(false);
		}

		public void SetupBoard(List<int> themes)
		{
			SetupBoard(_boardAnchor, _boardId, themes);
		}

		private void SetupBoard(Transform modelPosition, Boards.BoardId boardId, List<int> themes)
		{
			Boards.Board board = Boards.boardData[boardId];
			_boardRoot = HoverboardModelPreviewFactory.Instance.GetHoverboardModelPreview(board.boardModelName, boardId);
			_boardRoot.name = board.name;
			Transform transform = _boardRoot.transform;
			transform.parent = modelPosition;
			transform.localPosition = MODEL_POSITION;
			transform.localScale = Vector3.one * 11f;
			transform.localEulerAngles = MODEL_LOCAL_ROTATION;
			HoverboardBundle componentInChildren = transform.GetComponentInChildren<HoverboardBundle>();
			if (componentInChildren.CustomSets[0].VanityMenuFxPrefab != null && (themes == null || themes.Count == 0))
			{
				GameObject gameObject = transform.GetChild(0).gameObject.AddChild(componentInChildren.CustomSets[0].VanityMenuFxPrefab);
				ParticleSystem componentInChildren2 = gameObject.GetComponentInChildren<ParticleSystem>();
				if (componentInChildren2 != null)
				{
					SetupTrail(gameObject, FX_POSITION);
					SetupTrailParticleSystem(componentInChildren2);
				}
			}
			if (themes != null)
			{
				if (themes.Contains(1) && componentInChildren.CustomSets[1].VanityMenuFxPrefab != null)
				{
					if (componentInChildren.CustomSets[1].baseModelPrefab != null)
					{
						GameObject gameObject2 = transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
						if (gameObject2 != null)
						{
							UnityEngine.Object.Destroy(gameObject2);
							transform.GetChild(0).gameObject.AddChild(componentInChildren.CustomSets[1].baseModelPrefab);
						}
					}
					if (componentInChildren.CustomSets[1].modelPrefab != null)
					{
						transform.GetChild(0).gameObject.AddChild(componentInChildren.CustomSets[1].modelPrefab);
					}
					GameObject gameObject3 = transform.GetChild(0).gameObject.AddChild(componentInChildren.CustomSets[1].VanityMenuFxPrefab);
					ParticleSystem componentInChildren3 = gameObject3.GetComponentInChildren<ParticleSystem>();
					if (componentInChildren3 != null)
					{
						SetupTrail(gameObject3, TRAIL_PARTICLE_SYSTEM_POSITION);
						SetupTrailParticleSystem(componentInChildren3);
					}
				}
				if (themes.Contains(2) && componentInChildren.CustomSets[2].modelPrefab != null)
				{
					transform.GetChild(0).gameObject.AddChild(componentInChildren.CustomSets[2].modelPrefab);
				}
				if (themes.Contains(2) && componentInChildren.CustomSets[2].ExtraMenuFXPrefab != null)
				{
					_teleboardMenuEffect = ((GameObject)null).AddChild(componentInChildren.CustomSets[2].ExtraMenuFXPrefab).GetComponent<ATeleboardMenuEffect>();
					_teleboardMenuEffect.transform.SetParent(transform.parent, true);
					_teleboardMenuEffect.transform.localScale /= 40f;
					_teleboardMenuEffect.transform.localPosition = Vector3.zero;
					if (_teleboardMenuEffect != null)
					{
						_teleboardMenuEffect.Initialize(_boardRoot.transform, true);
						_teleboardMenuEffect.StopAllCoroutines();
						_teleboardMenuEffect.StartCoroutine(TeleboardEffect());
					}
				}
			}
			Utility.SetLayerRecursively(modelPosition, 29);
		}

		public void SetUpPresentation(PromotionPresentation presentation)
		{
			_background.gradientTop = presentation.BackgroundColorTop;
			_background.gradientBottom = presentation.BackgroundColorBottom;
			int count = _bgFlares.Count;
			for (int i = 0; i < count; i++)
			{
				_bgFlares[i].color = presentation.BeamColor;
			}
			_bgFlash.color = presentation.FlashColor;
		}

		public void SetUpSplat(Color splatBgColor, Color splatLabelColor)
		{
			if (_bestSplat != null)
			{
				_bestSplat.color = splatBgColor;
				_bestSplat.GetComponentInChildren<UILabel>().color = splatLabelColor;
			}
		}

		public void ShowStars(bool show)
		{
			if (_starsDecoration != null)
			{
				_starsDecoration.SetActive(show);
			}
		}

		private IEnumerator TeleboardEffect()
		{
			while (true)
			{
				float nextSplashTime = UnityEngine.Random.Range(1.5f, 3f);
				yield return new WaitForSeconds(nextSplashTime);
				_boardRoot.transform.localScale = Vector3.zero;
				_teleboardMenuEffect.StartTeleboardEffect(0.4f, null);
				yield return new WaitForSeconds(0.4f);
				_boardRoot.transform.localScale = Vector3.one * 11f;
			}
		}

		private void SetupTrail(GameObject trail, Vector3 trailPosition)
		{
			ParticleSystem[] componentsInChildren = trail.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].transform.localPosition = Vector3.zero;
			}
			trail.transform.localPosition = trailPosition;
			trail.transform.localScale = TRAIL_SCALE;
			trail.transform.localRotation = TRAIL_ROTATION;
		}

		private void SetupTrailParticleSystem(ParticleSystem trailParticleSystem)
		{
			ParticleSystem.MainModule main = trailParticleSystem.main;
			trailParticleSystem.transform.localPosition = Vector3.zero;
			main.startRotation = new ParticleSystem.MinMaxCurve(2.6179938f, 3.6651914f);
			ParticleSystem.EmissionModule emission = trailParticleSystem.emission;
			emission.rateOverTime = 3f;
		}

		public void SetupThemeInfo(int theme, int powerIndex)
		{
			SetupThemeInfo(_boardId, theme, powerIndex);
		}

		private void SetupThemeInfo(Boards.BoardId boardId, int theme, int powerIndex)
		{
			BundleIcon bundleIcon = _bundleIcons[powerIndex];
			if (theme > 0)
			{
				bundleIcon.SetupTheme(boardId, theme);
			}
			else
			{
				bundleIcon.SetupDefaultTheme(boardId);
			}
		}

		public void Align()
		{
			_boardLayouter.Align();
		}
	}
}
