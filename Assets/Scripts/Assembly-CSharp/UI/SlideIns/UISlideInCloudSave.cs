using Cloud;
using Cloud.Internal;
using NaughtyAttributes;
using UI.Generic;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInCloudSave : UISlideIn
	{
		[SerializeField]
		protected UILabel _topLine;

		[SerializeField]
		protected UILabel _bottomLine;

		[SerializeField]
		protected HorizontalLayout _layout;

		[SerializeField]
		protected UISprite _icon;

		[SerializeField]
		protected UISprite _accentIcon;

		[SerializeField]
		protected FitSpriteToWidget _fitter;

		protected SlideIn _payload;

		public override void SetupSlideIn(SlideIn slideIn)
		{
			_payload = slideIn;
			SetupSlideIn(slideIn.cloudSaveState);
		}

		private void SetupSlideIn(CloudSaveState state)
		{
			SetupBasic();
			SetupSpecific(state);
			_fitter.FitNow();
			_accentIcon.gameObject.SetActive(state == CloudSaveState.Active || state == CloudSaveState.Inactive || state == CloudSaveState.OptedOut);
			base.gameObject.SetActive(true);
			_layout.Align(true);
			if (Application.isPlaying)
			{
				SlideIn();
			}
		}

		[Button(null)]
		protected virtual void SetupBasic()
		{
			_topLine.color = ColorData.instance.white;
			_topLine.effectColor = ColorData.instance.midnight;
			_bottomLine.color = ColorData.instance.sun;
			_bottomLine.effectColor = ColorData.instance.midnight;
			if (!Application.isPlaying)
			{
				base.gameObject.SetActive(false);
				base.gameObject.SetActive(true);
			}
		}

		protected virtual void SetupSpecific(CloudSaveState state)
		{
			switch (state)
			{
			case CloudSaveState.Active:
				_topLine.text = Strings.Get("CLOUDSAVE_SLIDEIN_WELCOMEBACK_TOP");
				_bottomLine.text = string.Format(Strings.Get("CLOUDSAVE_USERNAME"), CloudSave.userName) + "!";
				_icon.spriteName = "ic_cloud";
				_accentIcon.spriteName = "ic_check";
				break;
			case CloudSaveState.Error:
				_topLine.text = Strings.Get("CLOUDSAVE_SLIDEIN_CONNECTIONERROR_TOP");
				_bottomLine.text = Strings.Get("CLOUDSAVE_SLIDEIN_CONNECTIONERROR_BOTTOM");
				_icon.spriteName = "ic_brdrcmb_error";
				break;
			case CloudSaveState.Conflict:
				_topLine.text = Strings.Get("CLOUDSAVE_STATUS_CONFLICT_INFO");
				_bottomLine.text = Strings.Get("CLOUDSAVE_SLIDEIN_CONNECTIONERROR_BOTTOM");
				_icon.spriteName = "ic_brdrcmb_conflict";
				break;
			case CloudSaveState.Inactive:
			case CloudSaveState.OptedOut:
				_topLine.text = Strings.Get("CLOUDSAVE_SLIDEIN_DISCONNECT_TOP");
				_bottomLine.text = Strings.Get("CLOUDSAVE_SLIDEIN_DISCONNECT_BOTTOM");
				_icon.spriteName = "ic_cloud";
				_accentIcon.spriteName = "ic_x_red";
				break;
			}
		}

		[Button(null)]
		private void SetupOk()
		{
			SetupSlideIn(CloudSaveState.Active);
		}

		[Button(null)]
		private void SetupError()
		{
			SetupSlideIn(CloudSaveState.Error);
		}

		[Button(null)]
		private void SetupConflict()
		{
			SetupSlideIn(CloudSaveState.Conflict);
		}

		[Button(null)]
		private void SetupDisconnected()
		{
			SetupSlideIn(CloudSaveState.Inactive);
		}
	}
}
