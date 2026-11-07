using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using UI.Generic;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.Popups.CloudSaveComparisonPopupInternal
{
	public class CloudSaveConflictBlock : MonoBehaviour
	{
		public class ConflictBlockPayload
		{
			public string buttonText;

			public string topSpriteName;

			public bool isImprovement;

			public List<CloudSaveProgressCell.CloudSaveProgressCellPayload> cellPayloads = new List<CloudSaveProgressCell.CloudSaveProgressCellPayload>();

			public Action onButtonClicked;

			public void OnButtonClicked()
			{
				onButtonClicked.SafeInvoke();
			}
		}

		[SerializeField]
		private CloudSaveProgressCell _cellPrefab;

		[SerializeField]
		private UISprite _banner;

		[SerializeField]
		private FitSpriteToWidget _topIconFitter;

		[SerializeField]
		private UISprite _topIcon;

		[SerializeField]
		private UILabel _bannerTopLabel;

		[SerializeField]
		private UILabel _bannerBottomLabel;

		[SerializeField]
		private UILabel _buttonLabel;

		[SerializeField]
		private Layout _cellLayout;

		private ConflictBlockPayload _payload;

		public void Setup(ConflictBlockPayload payload)
		{
			_payload = payload;
			SetupBasic();
			_topIcon.spriteName = _payload.topSpriteName;
			_topIconFitter.FitNow();
			_buttonLabel.text = _payload.buttonText;
			_banner.gameObject.SetActive(_payload.isImprovement);
			_cellLayout.transform.DestroyChildren();
			int num = 0;
			foreach (CloudSaveProgressCell.CloudSaveProgressCellPayload cellPayload in _payload.cellPayloads)
			{
				cellPayload.backgroundWidth = _cellLayout.GetComponent<UIWidget>().width;
				cellPayload.showBackground = ++num % 2 == 1;
				cellPayload.contentLeftOffset = 3;
				CloudSaveProgressCell cloudSaveProgressCell = UnityEngine.Object.Instantiate(_cellPrefab, _cellLayout.transform);
				cloudSaveProgressCell.SetupCell(cellPayload);
			}
			_cellLayout.Align();
		}

		private void SetupBasic()
		{
			ColorData instance = ColorData.instance;
			_bannerTopLabel.text = Strings.Get("CLOUDSAVE_COMPARISON_LABEL_HEADER");
			_bannerBottomLabel.text = Strings.Get("CLOUDSAVE_COMPARISON_LABEL_SUBHEADER");
			_banner.color = instance.grass;
			_bannerTopLabel.color = instance.glow;
			_bannerBottomLabel.color = instance.sun;
		}

		[UsedImplicitly]
		public void ButtonClicked()
		{
			_payload.OnButtonClicked();
		}
	}
}
