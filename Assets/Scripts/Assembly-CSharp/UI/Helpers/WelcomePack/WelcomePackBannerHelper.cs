using System.Collections.Generic;
using Data.Managers;
using InApp;
using InAppPurchase;
using Info;
using Kiloo.Common.Backend.PurchaseValidation;
using UI.Generic;
using UI.Generic.Layout;
using UI.RenderTexture;
using UI.Screens;
using UI.Screens.StoreScreenInternal;
using UnityEngine;

namespace UI.Helpers.WelcomePack
{
	public class WelcomePackBannerHelper : MonoBehaviour
	{
		[Header("MainCell")]
		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _boardName;

		[SerializeField]
		private DefaultRenderTextureSetup _mainCellRenderTextureSetup;

		[Header("Cell 1")]
		[SerializeField]
		private UILabel _cell1Label;

		[Header("Cell 2")]
		[SerializeField]
		private UILabel _cell2Label;

		[Header("Cell 3")]
		[SerializeField]
		private UILabel _cell3Label;

		[Header("BuyButton")]
		[SerializeField]
		private UIButtonOverlayOff _buyButton;

		[SerializeField]
		private Layout _buyButtonLayout;

		[SerializeField]
		private UILabel _buyButtonLabel;

		[SerializeField]
		private UISprite _buyButtonCheckedSprite;

		[Header("Timer")]
		[SerializeField]
		private UILabel _timerTextLabel;

		[SerializeField]
		private AlignTimer _alignTimer;

		private bool _initiated;

		private string _lastScreenNameWhenPressingBuy;

		private TimerHelper _timer;

		private InAppItemModel _inAppPack;

		private void OnEnable()
		{
			LoadInAppPackIfNotLoaded();
			if (InAppManager.isInitialized)
			{
				InAppManager.instance.onPurchaseSuccess += OnPurchaseSuccess;
			}
			if (_initiated)
			{
				UpdateButton();
			}
		}

		public void Init()
		{
			_initiated = true;
			Setup();
		}

		private void Update()
		{
			if (WelcomePackManager.welcomePackTimeLeft.TotalSeconds <= 0.0)
			{
				StoreScreen.RefreshOnNextShow();
			}
			_alignTimer.SetTimeRemaining(WelcomePackManager.welcomePackTimeLeft);
		}

		private void LoadInAppPackIfNotLoaded()
		{
			if (_inAppPack == null && !string.IsNullOrEmpty(WelcomePackManager.welcomePackInAppID))
			{
				_inAppPack = InAppData.inAppData[WelcomePackManager.welcomePackInAppID];
			}
		}

		private void Setup()
		{
			SetupModels();
			SetupLabels(_inAppPack.bundleContents);
			UpdateButton();
		}

		private void UpdateButton()
		{
			if (BoardManager.IsBoardOwned(Boards.BoardId.starterPackBoard01))
			{
				GoToPurchasedState();
			}
			else
			{
				GoToBuyState();
			}
		}

		private void SetupModels()
		{
			_mainCellRenderTextureSetup.Setup(new CameraSettings
			{
				backgroundColor = new Color(1f, 1f, 1f, 0f)
			});
		}

		private void SetupLabels(List<InAppItemModel.BundleItem> content)
		{
			_title.text = Strings.Get("WELCOME_PACK_TITLE");
			_title.color = ColorData.instance.white;
			_title.effectColor = ColorData.instance.shadow;
			_boardName.text = Strings.Get(Boards.boardData[content[0].boardId].name);
			_boardName.color = NGUIMath.HexToColor(2298490367u);
			_boardName.effectColor = ColorData.instance.white;
			_cell1Label.text = content[1].amount.ToString();
			_cell2Label.text = "x" + content[2].amount;
			_cell3Label.text = "x" + content[3].amount;
			UILabel cell1Label = _cell1Label;
			Color white = ColorData.instance.white;
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
			_timerTextLabel.text = Strings.Get("TIMER_ENDS_IN");
			_alignTimer.SetTimerColor(ColorData.instance.mahogany);
			_alignTimer.SetTimeRemaining(WelcomePackManager.welcomePackTimeLeft);
		}

		private void GoToBuyState()
		{
			_buyButton.GoToNormalState();
			_buyButtonLabel.text = InAppData.inAppData[WelcomePackManager.welcomePackInAppID].localizedPrice;
			_buyButtonCheckedSprite.gameObject.SetActive(false);
			_buyButtonLayout.Align(true);
		}

		private void GoToPurchasedState()
		{
			_buyButton.GoToNotAvailableState();
			_buyButtonLabel.text = Strings.Get("STORE_CELL_PURCHASED");
			_buyButtonCheckedSprite.gameObject.SetActive(true);
			_buyButtonLayout.Align(true);
		}

		private void OnDisable()
		{
			if (InAppManager.isInitialized)
			{
				InAppManager.instance.onPurchaseSuccess -= OnPurchaseSuccess;
			}
		}

		public void OnPurchaseClick()
		{
			InAppManager.instance.onItemRestoreFinished += OnItemRestoreFinished;
			InAppManager.instance.RestoreNonConsumablePurchases(InAppData.GetInAppSpecialBundles());
			UIScreenController instance = UIScreenController.Instance;
			if (instance.isShowingPopup)
			{
				OnPurchaseClickInPopup(instance);
			}
			else
			{
				_lastScreenNameWhenPressingBuy = instance.GetTopScreenName();
			}
		}

		private void OnItemRestoreFinished(RestoreFinishedResult result)
		{
			InAppManager.instance.onItemRestoreFinished -= OnItemRestoreFinished;
			if (!BoardManager.IsBoardOwned(Boards.BoardId.starterPackBoard01))
			{
				InAppManager.instance.StartPurchase(WelcomePackManager.welcomePackInAppID);
			}
			else
			{
				GoToPurchasedState();
			}
		}

		private void OnPurchaseClickInPopup(UIScreenController uiScreenController)
		{
			_lastScreenNameWhenPressingBuy = "BoardScreen";
			_lastScreenNameWhenPressingBuy = uiScreenController.GetCurrentPopupName();
		}

		private void OnPurchaseSuccess(string productId, bool wasRestored, IPurchaseValidationResult validationResult)
		{
			if (!(productId == "com.kiloo.subwaysurfers.welcomepacktier01") && !(productId == "com.kiloo.subwaysurfers.welcomepacktier02"))
			{
				return;
			}
			if (!wasRestored)
			{
				UIScreenController.Instance.ShowRewardAnimationForBundlePurchase(_inAppPack.bundleContents, productId);
				if (validationResult != null && validationResult.receiptStatus == ReceiptStatus.Valid)
				{
					Flurry.LogEventWithAParameter("welcome_pack_purchased", "source_of_purchase", _lastScreenNameWhenPressingBuy);
				}
			}
			GoToPurchasedState();
		}
	}
}
