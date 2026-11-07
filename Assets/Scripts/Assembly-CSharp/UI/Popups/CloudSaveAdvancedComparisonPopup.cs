using System.Collections.Generic;
using Cloud;
using Cloud.Internal;
using Kiloo.Common.Extensions;
using NaughtyAttributes;
using UI.Generic.Layout;
using UI.Popups.CloudSaveAdvancedComparisonPopupInternal;
using UnityEngine;

namespace UI.Popups
{
	public class CloudSaveAdvancedComparisonPopup : UIPopup
	{
		[SerializeField]
		private UILabel _localHeadline;

		[SerializeField]
		private UILabel _onlineHeadline;

		[SerializeField]
		private UISprite _headerBackground;

		[SerializeField]
		private UISprite _contentBackground;

		[SerializeField]
		private UISprite _localBannerBackground;

		[SerializeField]
		private UISprite _onlineBannerBackground;

		[SerializeField]
		private CloudSaveAdvancedComparisonRow _comparisonRowPrefab;

		[SerializeField]
		private Layout _rowLayout;

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
			SetupComparison();
		}

		[Button(null)]
		private void SetupBasic()
		{
			ColorData instance = ColorData.instance;
			_localHeadline.color = instance.white;
			_localHeadline.effectColor = instance.royale;
			_onlineHeadline.color = instance.white;
			_onlineHeadline.effectColor = instance.royale;
			_headerBackground.color = instance.smurf;
			_contentBackground.color = instance.sky;
			_localBannerBackground.color = instance.grass;
			_onlineBannerBackground.color = instance.grass;
			base.gameObject.Blink();
			_localHeadline.text = Strings.Get("CLOUDSAVE_ADVANCED_LOCAL_HEADER");
			_onlineHeadline.text = Strings.Get("CLOUDSAVE_ADVANCED_CLOUD_HEADER");
		}

		[Button(null)]
		private void SetupComparison()
		{
			CloudSaveConflictResolver conflictResolver = CloudSave.conflictResolver;
			if (conflictResolver == null)
			{
				return;
			}
			List<CloudSaveComparisonRow> comparisonRows = conflictResolver.GetComparisonRows();
			_rowLayout.transform.DestroyChildren();
			CloudSaveAdvancedComparisonRow cloudSaveAdvancedComparisonRow = Object.Instantiate(_comparisonRowPrefab, _rowLayout.transform);
			int num = 0;
			cloudSaveAdvancedComparisonRow.SetupRow(new CloudSaveAdvancedComparisonRow.CloudSaveComparisonRowPayload
			{
				name = CloudSaveDecorator.GetName(CloudSaveComparisonElement.DeviceSource),
				localValue = conflictResolver.sourceMine,
				onlineValue = conflictResolver.sourceTheirs,
				showBackground = (++num % 2 == 0)
			});
			cloudSaveAdvancedComparisonRow = Object.Instantiate(_comparisonRowPrefab, _rowLayout.transform);
			cloudSaveAdvancedComparisonRow.SetupRow(new CloudSaveAdvancedComparisonRow.CloudSaveComparisonRowPayload
			{
				name = CloudSaveDecorator.GetName(CloudSaveComparisonElement.LastModifiedTime),
				localValue = CloudSaveDecorator.FormatDateTime(conflictResolver.lastModifiedMine),
				onlineValue = CloudSaveDecorator.FormatDateTime(conflictResolver.lastModifiedTheirs),
				showBackground = (++num % 2 == 0)
			});
			foreach (CloudSaveComparisonRow item in comparisonRows)
			{
				cloudSaveAdvancedComparisonRow = Object.Instantiate(_comparisonRowPrefab, _rowLayout.transform);
				cloudSaveAdvancedComparisonRow.SetupRow(new CloudSaveAdvancedComparisonRow.CloudSaveComparisonRowPayload
				{
					name = CloudSaveDecorator.GetName(item.id),
					localValue = CloudSaveDecorator.FormatValue(item.id, item.localValue),
					onlineValue = CloudSaveDecorator.FormatValue(item.id, item.onlineValue),
					showBackground = (++num % 2 == 0),
					isOnlineImprovement = item.isOnlineImprovement,
					isLocalImprovement = item.isLocalImprovement
				});
			}
			_rowLayout.Align();
			_onlineBannerBackground.gameObject.SetActive(conflictResolver.recommendation == CloudSaveConflictResolver.Recommendation.Theirs);
			_localBannerBackground.gameObject.SetActive(conflictResolver.recommendation == CloudSaveConflictResolver.Recommendation.Mine);
		}
	}
}
