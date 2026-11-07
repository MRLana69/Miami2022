using JetBrains.Annotations;
using UnityEngine;

namespace UI.Helpers
{
	public class LocalizationTitleHelper : MonoBehaviour
	{
		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _subTitle;

		[SerializeField]
		private string _titleStringId;

		[SerializeField]
		private string _subTitleStringId;

		[SerializeField]
		[UsedImplicitly]
		private int _titleStringIdIndex;

		[SerializeField]
		[UsedImplicitly]
		private int _subTitleStringIdIndex;

		private void OnEnable()
		{
			_title.text = Strings.Get(_titleStringId);
			_subTitle.text = Strings.Get(_subTitleStringId);
		}
	}
}
