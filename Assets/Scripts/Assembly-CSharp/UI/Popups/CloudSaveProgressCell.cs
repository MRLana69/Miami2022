using System;
using Cloud.Internal;
using NaughtyAttributes;
using UI.Generic;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.Popups
{
	public class CloudSaveProgressCell : MonoBehaviour
	{
		public class CloudSaveProgressCellPayload
		{
			public string iconName;

			public string text;

			public bool isImprovement;

			public bool showBackground;

			public int backgroundWidth;

			public int contentLeftOffset;

			public int backgroundHeight;
		}

		[SerializeField]
		private UISprite _background;

		[SerializeField]
		private UISprite _icon;

		[SerializeField]
		private UISprite _improvementIcon;

		[SerializeField]
		private UILabel _label;

		[SerializeField]
		private Layout _contentLayout;

		[SerializeField]
		private FitSpriteToWidget _iconFitter;

		public void SetupCell(CloudSaveProgressCellPayload payload)
		{
			_label.text = payload.text;
			_icon.spriteName = payload.iconName;
			_iconFitter.FitNow();
			_improvementIcon.gameObject.SetActive(payload.isImprovement);
			ColorData instance = ColorData.instance;
			_label.effectColor = instance.royale;
			_label.color = ((!payload.isImprovement) ? instance.white : instance.grape);
			_background.gameObject.SetActive(payload.showBackground);
			_background.color = instance.boston;
			_background.alpha = 0.4f;
			_contentLayout.Align();
			if (payload.backgroundWidth > 0)
			{
				GetComponent<UIWidget>().width = payload.backgroundWidth;
				_background.ResetAndUpdateAnchors();
				_contentLayout.Align();
			}
			if (payload.backgroundHeight > 0)
			{
				GetComponent<UIWidget>().height = payload.backgroundHeight;
				_background.ResetAndUpdateAnchors();
				_contentLayout.Align();
			}
			UIWidget component = _contentLayout.GetComponent<UIWidget>();
			component.leftAnchor.absolute = payload.contentLeftOffset;
			component.ResetAndUpdateAnchors();
		}

		[Button(null)]
		private void SetAsMultiplier()
		{
			_label.text = CloudSaveDecorator.FormatValueWithTitle(CloudSaveComparisonElement.Multiplier, 1f);
			_icon.spriteName = CloudSaveDecorator.GetIconName(CloudSaveComparisonElement.Multiplier);
			_iconFitter.FitNow();
		}

		[Button(null)]
		private void SetAsHighscore()
		{
			_label.text = CloudSaveDecorator.FormatValueWithTitle(CloudSaveComparisonElement.Highscore, 100000f);
			_icon.spriteName = CloudSaveDecorator.GetIconName(CloudSaveComparisonElement.Highscore);
			_iconFitter.FitNow();
		}

		[Button(null)]
		private void SetAsKeys()
		{
			_label.text = CloudSaveDecorator.FormatValueWithTitle(CloudSaveComparisonElement.Keys, 5f);
			_icon.spriteName = CloudSaveDecorator.GetIconName(CloudSaveComparisonElement.Keys);
			_iconFitter.FitNow();
		}

		[Button(null)]
		private void SetAsUpgrades()
		{
			_label.text = CloudSaveDecorator.FormatValueWithTitle(CloudSaveComparisonElement.Upgrades, 3f);
			_icon.spriteName = CloudSaveDecorator.GetIconName(CloudSaveComparisonElement.Upgrades);
			_iconFitter.FitNow();
		}

		[Button(null)]
		private void SetAsAwards()
		{
			_label.text = CloudSaveDecorator.FormatValueWithTitle(CloudSaveComparisonElement.Awards, 1f);
			_icon.spriteName = CloudSaveDecorator.GetIconName(CloudSaveComparisonElement.Awards);
			_iconFitter.FitNow();
		}

		[Button(null)]
		private void SetAsCharacters()
		{
			_label.text = CloudSaveDecorator.FormatValueWithTitle(CloudSaveComparisonElement.Characters, 1f);
			_icon.spriteName = CloudSaveDecorator.GetIconName(CloudSaveComparisonElement.Characters);
			_iconFitter.FitNow();
		}

		[Button(null)]
		private void SetAsBoards()
		{
			_label.text = CloudSaveDecorator.FormatValueWithTitle(CloudSaveComparisonElement.Boards, 1f);
			_icon.spriteName = CloudSaveDecorator.GetIconName(CloudSaveComparisonElement.Boards);
			_iconFitter.FitNow();
		}

		[Button(null)]
		private void SetAsOutfits()
		{
			_label.text = CloudSaveDecorator.FormatValueWithTitle(CloudSaveComparisonElement.Outfits, 1f);
			_icon.spriteName = CloudSaveDecorator.GetIconName(CloudSaveComparisonElement.Outfits);
			_iconFitter.FitNow();
		}

		[Button(null)]
		private void SetAsBoardThemes()
		{
			_label.text = CloudSaveDecorator.FormatValueWithTitle(CloudSaveComparisonElement.BoardThemes, 1f);
			_icon.spriteName = CloudSaveDecorator.GetIconName(CloudSaveComparisonElement.BoardThemes);
			_iconFitter.FitNow();
		}

		[Button(null)]
		private void SetAsLastModifiedTime()
		{
			_label.text = CloudSaveDecorator.FormatDateTime(DateTime.Now);
			_icon.spriteName = CloudSaveDecorator.GetIconName(CloudSaveComparisonElement.LastModifiedTime);
			_iconFitter.FitNow();
		}
	}
}
