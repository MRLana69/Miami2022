using Kiloo.Common.Extensions;
using UnityEngine;

namespace UI.Popups.CloudSaveAdvancedComparisonPopupInternal
{
	public class CloudSaveAdvancedComparisonRow : MonoBehaviour
	{
		public class CloudSaveComparisonRowPayload
		{
			public string name;

			public string localValue;

			public string onlineValue;

			public bool showBackground;

			public bool isLocalImprovement;

			public bool isOnlineImprovement;
		}

		[SerializeField]
		private UILabel _name;

		[SerializeField]
		private UILabel _localValue;

		[SerializeField]
		private UILabel _onlineValue;

		[SerializeField]
		private UISprite _background;

		public void SetupRow(CloudSaveComparisonRowPayload payload)
		{
			_name.text = payload.name;
			_localValue.text = payload.localValue;
			_onlineValue.text = payload.onlineValue;
			ColorTextField(_localValue, payload.isLocalImprovement);
			ColorTextField(_onlineValue, payload.isOnlineImprovement);
			_background.gameObject.SetActive(payload.showBackground);
			_background.color = ColorData.instance.smurf.WithAlpha(0.4f);
		}

		private static void ColorTextField(UILabel label, bool isImprovement)
		{
			ColorData instance = ColorData.instance;
			if (isImprovement)
			{
				label.effectStyle = UILabel.Effect.Shadow;
				label.color = instance.grape;
				label.effectColor = instance.royale;
			}
			else
			{
				label.effectStyle = UILabel.Effect.None;
				label.color = instance.midnight;
			}
		}
	}
}
