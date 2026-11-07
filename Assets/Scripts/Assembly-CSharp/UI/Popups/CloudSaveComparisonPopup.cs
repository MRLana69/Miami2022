using System.Collections.Generic;
using System.Linq;
using Cloud;
using Cloud.Internal;
using JetBrains.Annotations;
using Kiloo.Common.Timer;
using NaughtyAttributes;
using UI.Generic.Layout;
using UI.Popups.CloudSaveComparisonPopupInternal;
using UI.Popups.CloudSaveConfirmationPopupInternal;
using UI.SlideIns;
using UnityEngine;

namespace UI.Popups
{
	public class CloudSaveComparisonPopup : UIPopup
	{
		private const float SLIDEIN_DELAY = 0.5f;

		[SerializeField]
		private UILabel _headline;

		[SerializeField]
		private UILabel _subline;

		[SerializeField]
		private UILabel _subtext;

		[SerializeField]
		private CloudSaveConflictBlock _conflictPrefab;

		[SerializeField]
		private Layout _conflictLayout;

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
			SetupConflict();
		}

		[Button(null)]
		private void SetupBasic()
		{
			ColorData instance = ColorData.instance;
			_headline.color = instance.midnight;
			_subline.color = instance.peacock;
			_subtext.color = instance.peacock;
			if (!Application.isPlaying)
			{
				base.gameObject.SetActive(false);
				base.gameObject.SetActive(true);
			}
			_headline.text = Strings.Get("CLOUDSAVE_COMPARISON_HEADER");
			_subline.text = Strings.Get("CLOUDSAVE_COMPARISON_DESCRIPTION");
			_subtext.text = "[u]" + Strings.Get("CLOUDSAVE_COMPARISON_INFOBUTTON");
		}

		[Button(null)]
		private void SetupConflict()
		{
			CloudSaveConflictResolver resolver = CloudSave.conflictResolver;
			if (resolver == null)
			{
				return;
			}
			resolver.RefreshLocalUserData();
			List<CloudSaveComparisonRow> comparisonRows = resolver.GetComparisonRows();
			comparisonRows = comparisonRows.OrderByDescending((CloudSaveComparisonRow r) => (double)Mathf.Abs(r.localValue - r.onlineValue) > 0.5).ToList();
			_conflictLayout.transform.DestroyChildren();
			string source = string.Empty;
			switch (resolver.recommendation)
			{
			case CloudSaveConflictResolver.Recommendation.None:
				source = "FromComparisonRecommendingNone";
				break;
			case CloudSaveConflictResolver.Recommendation.Mine:
				source = "FromComparisonRecommendingMine";
				break;
			case CloudSaveConflictResolver.Recommendation.Theirs:
				source = "FromComparisonRecommendingTheirs";
				break;
			}
			CloudSaveConflictBlock cloudSaveConflictBlock = Object.Instantiate(_conflictPrefab, _conflictLayout.transform);
			CloudSaveConflictBlock.ConflictBlockPayload conflictBlockPayload = new CloudSaveConflictBlock.ConflictBlockPayload();
			conflictBlockPayload.topSpriteName = "ic_phone";
			conflictBlockPayload.buttonText = Strings.Get("CLOUDSAVE_COMPARISON_KEEPLOCAL");
			conflictBlockPayload.isImprovement = resolver.recommendation == CloudSaveConflictResolver.Recommendation.Mine;
			conflictBlockPayload.cellPayloads = new List<CloudSaveProgressCell.CloudSaveProgressCellPayload>
			{
				new CloudSaveProgressCell.CloudSaveProgressCellPayload
				{
					iconName = CloudSaveDecorator.GetIconName(CloudSaveComparisonElement.LastModifiedTime),
					text = CloudSaveDecorator.FormatDateTime(resolver.lastModifiedMine)
				}
			};
			conflictBlockPayload.onButtonClicked = () =>
			{
				CloudSaveConfirmationPopupPayload.PreparePayload(CloudSaveConfirmationPopupPayload.ConfirmationState.KeepLocal, () =>
				{
					resolver.ResolveToMine();
					UIScreenController.Instance.QueueSlideIn(new SlideIn
					{
						type = SlideInData.SlideInType.CloudSaveMessage,
						stringValue = Strings.Get("CLOUDSAVE_SLIDEIN_CONFLICT_RESOLVED_LOCAL")
					});
					Dictionary<string, string> parameters = new Dictionary<string, string>
					{
						{ "Source", source },
						{ source, "UsingMine" }
					};
					Flurry.LogEventWithSeveralParameters("CloudSave Conflict Resolved", parameters);
					UIScreenController.Instance.ClosePopup();
					UIScreenController.Instance.ClosePopup();
					UIScreenController.Instance.ClosePopup();
				}, () =>
				{
					UIScreenController.Instance.ClosePopup();
				});
				UIScreenController.Instance.PushPopup("CloudSaveConfirmationPopup");
			};
			CloudSaveConflictBlock.ConflictBlockPayload conflictBlockPayload2 = conflictBlockPayload;
			for (int num = 0; num < 2; num++)
			{
				conflictBlockPayload2.cellPayloads.Add(new CloudSaveProgressCell.CloudSaveProgressCellPayload
				{
					iconName = CloudSaveDecorator.GetIconName(comparisonRows[num].id),
					text = CloudSaveDecorator.FormatValueWithTitle(comparisonRows[num].id, comparisonRows[num].localValue),
					isImprovement = comparisonRows[num].isLocalImprovement
				});
			}
			cloudSaveConflictBlock.Setup(conflictBlockPayload2);
			CloudSaveConflictBlock cloudSaveConflictBlock2 = Object.Instantiate(_conflictPrefab, _conflictLayout.transform);
			conflictBlockPayload = new CloudSaveConflictBlock.ConflictBlockPayload();
			conflictBlockPayload.topSpriteName = "ic_cloud";
			conflictBlockPayload.buttonText = Strings.Get("CLOUDSAVE_COMPARISON_KEEPCLOUD");
			conflictBlockPayload.isImprovement = resolver.recommendation == CloudSaveConflictResolver.Recommendation.Theirs;
			conflictBlockPayload.cellPayloads = new List<CloudSaveProgressCell.CloudSaveProgressCellPayload>
			{
				new CloudSaveProgressCell.CloudSaveProgressCellPayload
				{
					iconName = CloudSaveDecorator.GetIconName(CloudSaveComparisonElement.LastModifiedTime),
					text = CloudSaveDecorator.FormatDateTime(resolver.lastModifiedTheirs)
				}
			};
			conflictBlockPayload.onButtonClicked = () =>
			{
				CloudSaveConfirmationPopupPayload.PreparePayload(CloudSaveConfirmationPopupPayload.ConfirmationState.KeepOnline, () =>
				{
					resolver.ResolveToTheirs();
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
						{ "Source", source },
						{ source, "UsingTheirs" }
					};
					Flurry.LogEventWithSeveralParameters("CloudSave Conflict Resolved", parameters);
				}, () =>
				{
					UIScreenController.Instance.ClosePopup();
				});
				UIScreenController.Instance.PushPopup("CloudSaveConfirmationPopup");
			};
			CloudSaveConflictBlock.ConflictBlockPayload conflictBlockPayload3 = conflictBlockPayload;
			for (int num2 = 0; num2 < 2; num2++)
			{
				conflictBlockPayload3.cellPayloads.Add(new CloudSaveProgressCell.CloudSaveProgressCellPayload
				{
					iconName = CloudSaveDecorator.GetIconName(comparisonRows[num2].id),
					text = CloudSaveDecorator.FormatValueWithTitle(comparisonRows[num2].id, comparisonRows[num2].onlineValue),
					isImprovement = comparisonRows[num2].isOnlineImprovement
				});
			}
			cloudSaveConflictBlock2.Setup(conflictBlockPayload3);
			_conflictLayout.Align();
		}

		[UsedImplicitly]
		public void MoreInformationClicked()
		{
			UIScreenController.Instance.PushPopup("CloudSaveAdvancedComparisonPopup");
			Flurry.LogEventWithAParameter("CloudSave Advanced Comparison Popup", "Source", "FromComparisonPopup");
		}
	}
}
