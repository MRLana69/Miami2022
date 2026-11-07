using Cloud.Internal;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInCloudSaveMessage : UISlideInCloudSave
	{
		protected override void SetupBasic()
		{
			_topLine.color = ColorData.instance.white;
			_topLine.effectColor = ColorData.instance.midnight;
			_bottomLine.color = ColorData.instance.grape;
			_bottomLine.effectColor = ColorData.instance.midnight;
			_topLine.text = Strings.Get("CLOUDSAVE_SLIDEIN_CONFLICT_RESOLVED_HEADER");
			_icon.spriteName = "ic_cloud";
			_accentIcon.spriteName = "ic_check";
			if (!Application.isPlaying)
			{
				base.gameObject.SetActive(false);
				base.gameObject.SetActive(true);
			}
		}

		protected override void SetupSpecific(CloudSaveState state)
		{
			_bottomLine.text = _payload.stringValue;
		}
	}
}
