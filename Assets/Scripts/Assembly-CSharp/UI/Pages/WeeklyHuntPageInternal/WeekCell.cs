using System;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using UnityEngine;
using WeeklyHunt;

namespace UI.Pages.WeeklyHuntPageInternal
{
	public class WeekCell : MonoBehaviour
	{
		public class WeekCellPayload
		{
			public Action<WeeklyHuntData.PrizePeriodId> clicked;

			public WeeklyHuntData.PrizePeriodId periodId;

			public string label = "N/A";
		}

		[SerializeField]
		private UISprite _cellBG;

		[SerializeField]
		private UISprite _dottedLine;

		[SerializeField]
		private UILabel _weekLabel;

		[SerializeField]
		private UILabel _statusLabel;

		[SerializeField]
		private Collider _collider;

		private int _inactiveBottomAnchorOffset = 3;

		private int _activeBottomAnchorOffset = -14;

		private Action<WeeklyHuntData.PrizePeriodId> _clickedAction;

		private WeeklyHuntData.PrizePeriodId _periodId;

		public WeeklyHuntData.PrizePeriodId periodId
		{
			get
			{
				return _periodId;
			}
		}

		public void Init(WeekCellPayload payload)
		{
			_clickedAction = payload.clicked;
			_periodId = payload.periodId;
			_weekLabel.text = payload.label;
			_dottedLine.color = GlobalColorsGenerated.WEEKLYHUNT_DIVIDER;
		}

		public void SetCellActive(bool active)
		{
			_dottedLine.gameObject.SetActive(active);
			_collider.enabled = !active;
			if (active)
			{
				_cellBG.bottomAnchor.absolute = _activeBottomAnchorOffset;
				_cellBG.applyGradient = false;
				_cellBG.color = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_BG_GRADIENT_TOP;
				_weekLabel.color = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_TITLE_TEXT;
				_weekLabel.effectColor = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_TITLE_TEXT_SHADOW;
				_statusLabel.color = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_STATUS_TEXT;
				_statusLabel.effectColor = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_STATUS_TEXT_SHADOW;
			}
			else
			{
				_cellBG.bottomAnchor.absolute = _inactiveBottomAnchorOffset;
				_cellBG.applyGradient = true;
				_cellBG.color = Color.white;
				_cellBG.gradientTop = GlobalColorsGenerated.WEEKLYHUNT_REWARD_BG_GRADIENT_TOP;
				_cellBG.gradientBottom = GlobalColorsGenerated.WEEKLYHUNT_REWARD_BG_GRADIENT_BOTTOM;
				_weekLabel.color = GlobalColorsGenerated.WEEKLYHUNT_INACTIVE_TITLE_TEXT;
				_weekLabel.effectColor = GlobalColorsGenerated.WEEKLYHUNT_INACTIVE_TITLE_TEXT_SHADOW;
				_statusLabel.color = GlobalColorsGenerated.WEEKLYHUNT_INACTIVE_STATUS_TEXT;
				_statusLabel.effectColor = GlobalColorsGenerated.WEEKLYHUNT_INACTIVE_STATUS_TEXT_SHADOW;
			}
		}

		public void UpdateStatus()
		{
			switch (WeeklyHuntManager.GetPeriodStatus(_periodId))
			{
			case WeeklyHuntData.PeriodStatus.Current:
				_statusLabel.text = Strings.Get("WEEKLY_HUNT_CURRENT");
				break;
			case WeeklyHuntData.PeriodStatus.Expired:
				_statusLabel.text = Strings.Get("WEEKLY_HUNT_EVENT_ENDED_SHORT");
				break;
			case WeeklyHuntData.PeriodStatus.Unstarted:
				_statusLabel.text = Strings.Get("WEEKLY_HUNT_UPCOMING");
				break;
			}
		}

		[UsedImplicitly]
		public void OnClicked()
		{
			_clickedAction.SafeInvoke(_periodId);
		}
	}
}
