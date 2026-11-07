using System.Collections.Generic;
using Info;
using SYBO.Subway.Info;
using SYBO.Subway.Promotion;
using UI.RenderTexture;
using UI.Screens;
using UnityEngine;

namespace UI.Popups
{
	public class LimitedBoardBundlePopup : LimitedBoardPopup
	{
		[Header("Buy Button")]
		[SerializeField]
		private int _buyButtonWidth = 143;

		[SerializeField]
		private int _buyButtonHeight = 58;

		[SerializeField]
		private Vector3 _buttonOffset;

		[Header("Render Texture")]
		[SerializeField]
		private UITexture _renderTexture;

		[SerializeField]
		private Vector3 _characterPositionOffset;

		[SerializeField]
		private Vector3 _rotationOffset;

		[SerializeField]
		private Vector3 _boardPositionOffset;

		[Space(10f)]
		[SerializeField]
		private UILabel _viewLabel;

		[SerializeField]
		private UISprite _closeButton;

		[SerializeField]
		private string _bundlePopup;

		[Header("Ribbon")]
		[SerializeField]
		private UILabel _ribbonLabel;

		private CharacterBoardBundleRenderTextureSetup _textureSetup;

		private DuoPromotion _promotion;

		protected override void Init()
		{
			base.Init();
			if (_boardSelectButton != null)
			{
				UIWidget component = _boardSelectButton.GetComponent<UIWidget>();
				component.width = _buyButtonWidth;
				component.height = _buyButtonHeight;
				component.transform.localPosition += _buttonOffset;
			}
			_promotion = PromotionManager.Instance.TryGetCurrentPromotion<DuoPromotion>();
			if (_promotion != null)
			{
				SetupModel(new DuoPromotionEntry[2] { _promotion.Entry1, _promotion.Entry2 });
				_viewLabel.text = Strings.Get("STORE_EVENT_CELL_BUTTON_VIEW");
				_ribbonLabel.text = Strings.Get(_promotion.BanneTextid);
			}
			else
			{
				SDebug.LogError("Could not get Duo promotion");
			}
		}

		protected override void Show()
		{
			base.Show();
			_closeButton.color = Color.white;
		}

		public void OnViewBundleClicked()
		{
			UIScreenController.Instance.CloseAllPopups();
			UIScreenController.Instance.PushPopup(_bundlePopup);
		}

		private void SetupModel(DuoPromotionEntry[] payload)
		{
			CharacterBoardBundleRenderTextureSetup characterBoardBundleRenderTextureSetup = new CharacterBoardBundleRenderTextureSetup();
			List<RenderTextureObject> list = new List<RenderTextureObject>();
			List<Characters.CharacterId> list2 = new List<Characters.CharacterId>();
			foreach (DuoPromotionEntry duoPromotionEntry in payload)
			{
				list2.Add(duoPromotionEntry.CharacterId);
				list.Add(new RenderTextureObject
				{
					animationClip = duoPromotionEntry.CharacterAnimation,
					position = duoPromotionEntry.PopupCharacterPosition,
					rotation = duoPromotionEntry.PopupCharacterRotation,
					scale = duoPromotionEntry.PopupCharacterScale,
					prefab = UIModelController.Instance.ModelPrefab
				});
				list.Add(new RenderTextureObject
				{
					position = duoPromotionEntry.PopupBoardPosition + _boardPositionOffset,
					rotation = duoPromotionEntry.PopupBoardRotation,
					scale = duoPromotionEntry.PopupBoardScale,
					prefab = (Resources.Load("Prefabs/HoverboardAssets/" + Boards.boardData[duoPromotionEntry.BoardId].boardModelName) as GameObject),
					animationDelay = Random.Range(0f, 0.5f),
					animationClip = duoPromotionEntry.BoardAnimation
				});
			}
			list[0].position = new Vector3(22f, -85f, -180f);
			list[0].rotation = new Vector3(0f, -160f, 0f);
			list[0].scale = new Vector3(12f, 12f, 12f);
			list[1].position += new Vector3(0f, -10f, 0f);
			list[2].position = new Vector3(-30f, -88f, -152f);
			list[2].rotation = new Vector3(-1f, 178f, -2f);
			list[2].scale = new Vector3(13f, 13f, 13f);
			characterBoardBundleRenderTextureSetup.Setup(list2, _renderTexture, list, new CameraSettings
			{
				backgroundColor = new Color(1f, 1f, 1f, 0f)
			});
		}
	}
}
