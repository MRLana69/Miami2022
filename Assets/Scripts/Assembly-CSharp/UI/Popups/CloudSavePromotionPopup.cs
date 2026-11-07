using System.Collections.Generic;
using System.Linq;
using Cloud;
using Cloud.Internal;
using JetBrains.Annotations;
using Kiloo.Common.Timer;
using NaughtyAttributes;
using UI.Generic.Layout;
using UI.Popups.CloudSaveConfirmationPopupInternal;
using UI.SlideIns;
using UnityEngine;

namespace UI.Popups
{
	public class CloudSavePromotionPopup : UIPopup
	{
		private const float SLIDEIN_DELAY = 0.5f;

		[SerializeField]
		private CloudSaveProgressCell _cellPrefab;

		[SerializeField]
		private Layout _cellLayout;

		[SerializeField]
		private UILabel _headline;

		[SerializeField]
		private UILabel _subline;

		[SerializeField]
		private UILabel _bannerTopLabel;

		[SerializeField]
		private UILabel _bannerBottomLabel;

		[SerializeField]
		private UILabel _subtext;

		[SerializeField]
		private UILabel _buttonLabel;

		[SerializeField]
		private UISprite _banner;

		[SerializeField]
		private UITexture _skyline;

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
		}

		protected override void Show()
		{
			_subline.text = string.Format(Strings.Get("CLOUDSAVE_PROMOTION_DESCRIPTION"), CloudSave.userName);
			SetupConflict();
		}

		protected override void Hide()
		{
			UIScreenController.Instance.QueueSlideIn(new SlideIn
			{
				type = SlideInData.SlideInType.CloudSave,
				cloudSaveState = CloudSave.state
			});
		}

		[Button(null)]
		private void SetupBasic()
		{
			ColorData instance = ColorData.instance;
			_headline.color = instance.midnight;
			_subline.color = instance.peacock;
			_banner.color = instance.yolk;
			_bannerTopLabel.color = instance.mahogany;
			_bannerBottomLabel.color = instance.safran;
			_subtext.color = instance.peacock;
			_skyline.gradientTop = instance.royale;
			_skyline.gradientBottom = instance.smurf;
			_skyline.color = instance.white;
			_skyline.applyGradient = true;
			_buttonLabel.color = instance.white;
			_buttonLabel.effectColor = instance.shadow;
			if (!Application.isPlaying)
			{
				base.gameObject.SetActive(false);
				base.gameObject.SetActive(true);
			}
			_headline.text = Strings.Get("CLOUDSAVE_PROMOTION_TITLE");
			_bannerTopLabel.text = Strings.Get("CLOUDSAVE_PROMOTION_LABELTITLE");
			_bannerBottomLabel.text = Strings.Get("CLOUDSAVE_PROMOTION_LABELSUBTITLE");
			_buttonLabel.text = Strings.Get("CLOUDSAVE_PROMOTION_BUTTON");
			_subtext.text = "[u]" + Strings.Get("CLOUDSAVE_COMPARISON_INFOBUTTON");
		}

		private void SetupConflict()
		{
			CloudSaveConflictResolver conflictResolver = CloudSave.conflictResolver;
			conflictResolver.RefreshLocalUserData();
			List<CloudSaveComparisonRow> comparisonRows = conflictResolver.GetComparisonRows();
			comparisonRows = comparisonRows.OrderByDescending((CloudSaveComparisonRow r) => r.isOnlineImprovement).ToList();
			_cellLayout.transform.DestroyChildren();
			int width = _cellLayout.GetComponent<UIWidget>().width;
			int num = 0;
			CloudSaveProgressCell cloudSaveProgressCell = Object.Instantiate(_cellPrefab, _cellLayout.transform);
			cloudSaveProgressCell.SetupCell(new CloudSaveProgressCell.CloudSaveProgressCellPayload
			{
				iconName = CloudSaveDecorator.GetIconName(CloudSaveComparisonElement.LastModifiedTime),
				text = CloudSaveDecorator.FormatDateTime(conflictResolver.lastModifiedTheirs),
				showBackground = (++num % 2 == 1),
				isImprovement = false,
				contentLeftOffset = 70,
				backgroundWidth = width,
				backgroundHeight = 36
			});
			for (int num2 = 0; num2 < 2; num2++)
			{
				cloudSaveProgressCell = Object.Instantiate(_cellPrefab, _cellLayout.transform);
				cloudSaveProgressCell.SetupCell(new CloudSaveProgressCell.CloudSaveProgressCellPayload
				{
					iconName = CloudSaveDecorator.GetIconName(comparisonRows[num2].id),
					text = CloudSaveDecorator.FormatValueWithTitle(comparisonRows[num2].id, comparisonRows[num2].onlineValue),
					isImprovement = comparisonRows[num2].isOnlineImprovement,
					contentLeftOffset = 70,
					backgroundWidth = width,
					showBackground = (++num % 2 == 1),
					backgroundHeight = 36
				});
			}
			_cellLayout.Align();
		}

		[UsedImplicitly]
		public void MoreInformationClicked()
		{
			UIScreenController.Instance.PushPopup("CloudSaveAdvancedComparisonPopup");
			Flurry.LogEventWithAParameter("CloudSave Advanced Comparison Popup", "Source", "FromPromotionPopup");
		}

		[UsedImplicitly]
		public void ButtonClicked()
		{
			CloudSaveConfirmationPopupPayload.PreparePayload(CloudSaveConfirmationPopupPayload.ConfirmationState.LoadProgress, () =>
			{
				CloudSaveConflictResolver conflictResolver = CloudSave.conflictResolver;
				conflictResolver.ResolveToTheirs();
				Timer.ExecuteWithDelay(0.5f, () =>
				{
					UIScreenController.Instance.QueueSlideIn(new SlideIn
					{
						type = SlideInData.SlideInType.CloudSaveMessage,
						stringValue = Strings.Get("CLOUDSAVE_SLIDEIN_CONFLICT_RESOLVED_CLOUD")
					});
				});
				Dictionary<string, string> parameters = new Dictionary<string, string>
				{
					{ "FromPromotionPopup", "UsingTheirs" },
					{ "Source", "FromPromotionPopup" }
				};
				Flurry.LogEventWithSeveralParameters("CloudSave Conflict Resolved", parameters);
			}, () =>
			{
				ClosePopup();
			});
			UIScreenController.Instance.PushPopup("CloudSaveConfirmationPopup");
		}
	}
}
