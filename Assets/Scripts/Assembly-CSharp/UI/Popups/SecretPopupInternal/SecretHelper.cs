using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Popups.SecretPopupInternal
{
	public class SecretHelper : MonoBehaviour
	{
		[FormerlySerializedAs("SecretValue")]
		[SerializeField]
		private UILabel _secretValue;

		[FormerlySerializedAs("SecretInfo")]
		[SerializeField]
		private UILabel _secretInfo;

		[FormerlySerializedAs("SecretTitle")]
		[SerializeField]
		private UILabel _secretTitle;

		[FormerlySerializedAs("SecretInfo2")]
		[SerializeField]
		private UILabel _secretInfo2;

		[FormerlySerializedAs("SecretValue2")]
		[SerializeField]
		private UILabel _secretValue2;

		public void UpdateInfo(string info, string value)
		{
			_secretInfo.text = info;
			_secretValue.text = value;
		}

		public void UpdateTitle(string title)
		{
			_secretTitle.text = title;
		}

		public void UpdateInfoDouble(string info, string value, string info2, string value2)
		{
			_secretInfo.text = info;
			_secretValue.text = value;
			_secretInfo2.text = info2;
			_secretValue2.text = value2;
		}
	}
}
