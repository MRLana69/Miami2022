using System.Collections.Generic;
using Data.Managers;
using InApp;
using InAppPurchase;
using Info;
using Kiloo.Common.Backend.PurchaseValidation;
using Kiloo.Common.Timer;
using UI.Generic;
using UI.Generic.Layout;
using UI.RenderTexture;
using UnityEngine;

namespace UI.Popups
{
	public class WelcomePackPopup : UIPopup
	{
		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _subtitle;

		[SerializeField]
		private GameObject _hoverBoardRenderObject;

		[SerializeField]
		private UILabel _boardName;

		[SerializeField]
		private DefaultRenderTextureSetup _mainCellRenderTextureSetup;

		[SerializeField]
		private UISprite _bgSprite;

		[Header("Cell 1")]
		[SerializeField]
		private UILabel _cell1Label;

		[SerializeField]
		private UISprite _cell1Bg;

		[Header("Cell 2")]
		[SerializeField]
		private UILabel _cell2Label;

		[SerializeField]
		private UISprite _cell2Bg;

		[Header("Cell 3")]
		[SerializeField]
		private UILabel _cell3Label;

		[SerializeField]
		private UISprite _cell3Bg;

		[Header("BuyButton")]
		[SerializeField]
		private UILabel _buyButtonLabel;

		[Header("Timer")]
		[SerializeField]
		private TimerHelper _timerPrefab;

		[SerializeField]
		private UIWidget _timerContainer;

		private TimerHelper _timer;

		private InAppItemModel _inAppPack;

		private string _lastScreenNameWhenPressingBuy;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		private void LoadInAppPackIfNotLoaded()
		{
			if (_inAppPack == null && !string.IsNullOrEmpty(WelcomePackManager.welcomePackInAppID))
			{
				_inAppPack = InAppData.inAppData[WelcomePackManager.welcomePackInAppID];
			}
		}

		protected override void Init()
		{
			SetColors();
		}

		private void SetColors()
		{
			_title.color = ColorData.instance.midnight;
			_subtitle.color = ColorData.instance.peacock;
			_boardName.color = NGUIMath.HexToColor(2298490367u);
			_boardName.effectColor = ColorData.instance.lightShadow;
			_bgSprite.gradientTop = NGUIMath.HexToColor(3707855871u);
			_bgSprite.gradientBottom = NGUIMath.HexToColor(4289763839u);
			_cell1Bg.color = NGUIMath.HexToColor(2835368959u);
			_cell2Bg.color = NGUIMath.HexToColor(2835368959u);
			_cell3Bg.color = NGUIMath.HexToColor(2835368959u);
			UILabel cell1Label = _cell1Label;
			Color white = Color.white;
			_cell3Label.color = white;
			white = white;
			_cell2Label.color = white;
			cell1Label.color = white;
			UILabel cell1Label2 = _cell1Label;
			white = ColorData.instance.shadow;
			_cell3Label.effectColor = white;
			white = white;
			_cell2Label.effectColor = white;
			cell1Label2.effectColor = white;
		}

		protected override void Show()
		{
			_title.text = Strings.Get("WELCOME_PACK_TITLE");
			_subtitle.text = Strings.Get("LIMITED_TIME_OFFER");
			if (!WelcomePackManager.IsWelcomePackActive())
			{
				Timer.ExecuteNextFrame(ClosePopup);
			}
		}

		private void OnEnable()
		{
			if (InAppManager.isInitialized)
			{
				InAppManager.instance.onPurchaseSuccess += OnPurchaseSuccess;
			}
			if (WelcomePackManager.IsWelcomePackActive())
			{
				LoadInAppPackIfNotLoaded();
			}
			SetupModels();
			SetupTimer();
			if (_inAppPack != null && _inAppPack.bundleContents != null)
			{
				SetupLabels(_inAppPack.bundleContents);
			}
		}

		private void OnDisable()
		{
			if (InAppManager.isInitialized)
			{
				InAppManager.instance.onPurchaseSuccess -= OnPurchaseSuccess;
			}
		}

		public override void GainFocus()
		{
			if (_hoverBoardRenderObject != null)
			{
				_hoverBoardRenderObject.layer = Layers.Instance._2DOVERLAYCLIP;
			}
		}

		public override void LoseFocus()
		{
			if (_hoverBoardRenderObject != null)
			{
				_hoverBoardRenderObject.layer = Layers.Instance._2DGUI;
			}
		}

		private void SetupLabels(List<InAppItemModel.BundleItem> content)
		{
			_title.text = Strings.Get("WELCOME_PACK_TITLE");
			_boardName.text = Strings.Get(Boards.boardData[content[0].boardId].name);
			_cell1Label.text = content[1].amount.ToString();
			_cell2Label.text = "x" + content[2].amount;
			_cell3Label.text = "x" + content[3].amount;
			_buyButtonLabel.text = InAppData.inAppData[WelcomePackManager.welcomePackInAppID].localizedPrice;
		}

		private void SetupTimer()
		{
			if (_timer == null)
			{
				_timer = Object.Instantiate(_timerPrefab, _timerContainer.transform);
				_timer.SetTimerColors(ColorData.instance.mahogany, ColorData.instance.sun);
				_timer.SetAlignment(HorizontalAlignment.Center);
				_timer.transform.localPosition = Vector3.zero;
				_timer.StartTimer(WelcomePackManager.welcomePackTimeLeft);
			}
		}

		private void SetupModels()
		{
			_mainCellRenderTextureSetup.Setup(new CameraSettings
			{
				backgroundColor = new Color(1f, 1f, 1f, 0f)
			});
		}

		public void OnPurchaseClick()
		{
			InAppManager.instance.onItemRestoreFinished += OnItemRestoreFinished;
			InAppManager.instance.RestoreNonConsumablePurchases(InAppData.GetInAppSpecialBundles());
			UIScreenController instance = UIScreenController.Instance;
			if (instance.isShowingPopup)
			{
				_lastScreenNameWhenPressingBuy = instance.GetCurrentPopupName();
			}
			else
			{
				_lastScreenNameWhenPressingBuy = instance.GetTopScreenName();
			}
		}

		private void OnPurchaseClickInPopup(UIScreenController uiScreenController)
		{
		}

		private void OnItemRestoreFinished(RestoreFinishedResult result)
		{
			InAppManager.instance.onItemRestoreFinished -= OnItemRestoreFinished;
			if (!BoardManager.IsBoardOwned(Boards.BoardId.starterPackBoard01))
			{
				InAppManager.instance.StartPurchase(WelcomePackManager.welcomePackInAppID);
			}
		}

		public void OnCloseButtonClicked()
		{
			Flurry.LogEventWithAParameter("welcome_pack_purchased", "source_of_purchase", "Closed");
			ClosePopup();
		}

		private void OnPurchaseSuccess(string productId, bool wasRestored, IPurchaseValidationResult validationResult)
		{
			if (!(productId == "com.kiloo.subwaysurfers.welcomepacktier01") && !(productId == "com.kiloo.subwaysurfers.welcomepacktier02"))
			{
				return;
			}
			Flurry.LogEventWithAParameter("welcome_pack_purchased", "source_of_purchase", "Purchased");
			ClosePopup();
			if (!wasRestored)
			{
				UIScreenController.Instance.ShowRewardAnimationForBundlePurchase(_inAppPack.bundleContents, productId);
				if (validationResult != null && validationResult.receiptStatus == ReceiptStatus.Valid)
				{
					Flurry.LogEventWithAParameter("welcome_pack_purchased", "source_of_purchase", _lastScreenNameWhenPressingBuy);
				}
			}
		}
	}
}
