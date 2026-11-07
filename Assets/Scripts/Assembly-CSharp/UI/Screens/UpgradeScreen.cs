using System.Collections.Generic;
using Info;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using SYBO.Subway.Info;
using UI.Generic.Layout;
using UI.Screens.UpgradeScreenInternal;
using UI.ShopInternal;
using UnityEngine;
using Utilities;

namespace UI.Screens
{
	public class UpgradeScreen : UIScreen
	{
		[SerializeField]
		private UIScrollView _scrollView;

		[SerializeField]
		private UpgradeContainer _upgradePrefab;

		[SerializeField]
		private Layout _contentLayout;

		[SerializeField]
		private SegmentDivider _segmentDividerPrefab;

		[SerializeField]
		private ShopHeader _shopHeaderPrefab;

		private List<UpgradeContainer> _upgradeContainers = new List<UpgradeContainer>();

		private const int SEGMENT_SPACING_HEIGHT = 13;

		private const int ELEMENT_SPACING_HEIGHT = 8;

		private float _animationTime;

		private int _updatedAnimationTimeOnFrame;

		protected override void Init()
		{
			HandleSafeArea();
			CreateHeader();
			CreateUpgrades();
			_contentLayout.Align(true);
			_scrollView.transform.Reset();
			FrameTimer.ExecuteWithDelay(1, _scrollView.ResetPosition);
		}

		private void HandleSafeArea()
		{
			UIPanel panel = _scrollView.panel;
			if (SafeArea.IsRestricted)
			{
				panel.topAnchor.absolute += -SafeArea.OFFSET_TOP;
				panel.bottomAnchor.absolute += SafeArea.OFFSET_BOTTOM;
			}
			panel.ResetAndUpdateAnchors();
		}

		protected override void Show()
		{
			RefreshContent();
		}

		private void RefreshContent()
		{
			if (_footerHandler != null)
			{
				_footerHandler.HandleStickers();
			}
			_upgradeContainers.ForEach((UpgradeContainer u) =>
			{
				u.Refresh();
			});
		}

		public override string GetFlurryName()
		{
			return "UpgradesUI_shop";
		}

		private void UpdateTimeIfNecessary()
		{
			if (_updatedAnimationTimeOnFrame != Time.frameCount)
			{
				_animationTime += Time.deltaTime;
				_updatedAnimationTimeOnFrame = Time.frameCount;
			}
		}

		private void CreateHeader()
		{
			ShopHeader shopHeader = Object.Instantiate(_shopHeaderPrefab, _contentLayout.transform);
			shopHeader.Setup(new ShopHeader.ShopHeaderPayload
			{
				headerText = Strings.Get("BOOSTS_TITLE"),
				segmentDividerPayload = new SegmentDivider.SegmentDividerPayload
				{
					headerText = Strings.Get("UPGRADE_SCREEN_SINGLE_USE"),
					backgroundColor = ColorData.instance.storm,
					textColor = ColorData.instance.white,
					textShadowColor = ColorData.instance.shadow
				},
				showBoomBot = false,
				headerCharacter = Characters.CharacterId.yutani
			});
		}

		private void CreateUpgrades()
		{
			CreateSpacer(13);
			UpgradeType[] consumableUpgradeTypes = Upgrades.consumableUpgradeTypes;
			foreach (UpgradeType upgradeType in consumableUpgradeTypes)
			{
				_upgradeContainers.Add(CreateUpgrade(upgradeType));
				CreateSpacer(8);
			}
			CreateSpacer(5);
			CreateSegmentDivider(new SegmentDivider.SegmentDividerPayload
			{
				headerText = Strings.Get("UPGRADES_TITLE"),
				backgroundColor = ColorData.instance.storm,
				textColor = ColorData.instance.white,
				textShadowColor = ColorData.instance.shadow
			});
			CreateSpacer(13);
			UpgradeType[] permanentUpgradeTypes = Upgrades.permanentUpgradeTypes;
			foreach (UpgradeType upgradeType2 in permanentUpgradeTypes)
			{
				_upgradeContainers.Add(CreateUpgrade(upgradeType2));
				CreateSpacer(8);
			}
			CreateSpacer(5);
		}

		private void CreateSegmentDivider(SegmentDivider.SegmentDividerPayload payload)
		{
			SegmentDivider segmentDivider = Object.Instantiate(_segmentDividerPrefab, _contentLayout.transform);
			segmentDivider.Setup(payload);
		}

		private UpgradeContainer CreateUpgrade(UpgradeType upgradeType)
		{
			UpgradeContainer upgradeContainer = Object.Instantiate(_upgradePrefab, _contentLayout.transform);
			upgradeContainer.Setup(new UpgradeContainer.UpgradePayload
			{
				upgradeType = upgradeType,
				getAnimationTime = () =>
				{
					UpdateTimeIfNecessary();
					return _animationTime;
				}
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
