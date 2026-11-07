using System.Collections.Generic;
using Info;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using UI.Generic.Layout;
using UI.Screens.UpgradeScreenInternal;
using UnityEngine;

namespace UI.Popups
{
	public class UpgradePopup : UIPopup
	{
		[SerializeField]
		private UILabel _header;

		[SerializeField]
		private UIScrollView _scrollView;

		[SerializeField]
		private UpgradeContainer _upgradePrefab;

		[SerializeField]
		private Layout _contentLayout;

		private List<UpgradeContainer> _upgradeContainers = new List<UpgradeContainer>();

		private const int SEGMENT_SPACING_HEIGHT = 13;

		private const int ELEMENT_SPACING_HEIGHT = 8;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		public override void BackButtonPressed()
		{
			UIScreenController.Instance.ClosePopup();
		}

		public override string GetFlurryName()
		{
			return "UpgradesUI_quick";
		}

		protected override void Init()
		{
			CreateUpgrades();
			_contentLayout.Align(true);
			_scrollView.transform.Reset();
			FrameTimer.ExecuteWithDelay(1, _scrollView.ResetPosition);
			_header.text = Strings.Get("BOOSTS_TITLE");
			_header.color = ColorData.instance.midnight;
		}

		protected override void Show()
		{
			RefreshContent();
		}

		private void RefreshContent()
		{
			_upgradeContainers.ForEach((UpgradeContainer u) =>
			{
				u.Refresh();
			});
		}

		private void CreateUpgrades()
		{
			UpgradeType[] consumableUpgradeTypes = Upgrades.consumableUpgradeTypes;
			foreach (UpgradeType upgradeType in consumableUpgradeTypes)
			{
				_upgradeContainers.Add(CreateUpgrade(upgradeType));
				CreateSpacer(8);
			}
			UpgradeType[] permanentUpgradeTypes = Upgrades.permanentUpgradeTypes;
			foreach (UpgradeType upgradeType2 in permanentUpgradeTypes)
			{
				_upgradeContainers.Add(CreateUpgrade(upgradeType2));
				CreateSpacer(8);
			}
			CreateSpacer(5);
		}

		private UpgradeContainer CreateUpgrade(UpgradeType upgradeType)
		{
			UpgradeContainer upgradeContainer = Object.Instantiate(_upgradePrefab, _contentLayout.transform);
			upgradeContainer.Setup(new UpgradeContainer.UpgradePayload
			{
				upgradeType = upgradeType
			});
			return upgradeContainer;
		}

		private void CreateSpacer(int height)
		{
			UIWidget uIWidget = _contentLayout.gameObject.AddWidget<UIWidget>();
			uIWidget.height = height;
			uIWidget.width = 100;
		}
	}
}
