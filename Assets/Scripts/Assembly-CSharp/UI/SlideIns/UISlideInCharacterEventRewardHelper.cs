using System.Linq;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInCharacterEventRewardHelper : UISlideIn
	{
		[SerializeField]
		private UILabel _label;

		[SerializeField]
		private Layout _layout;

		public override void SetupSlideIn(SlideIn slideIn)
		{
			base.gameObject.SetActive(true);
			SetLabel(slideIn.stringValue);
			_layout.Align();
			SlideIn();
		}

		private void SetLabel(string payload)
		{
			string arg = string.Join(" ", (from x in payload.ToCharArray()
				select x.ToString()).ToArray());
			_label.text = string.Format(Strings.Get("SLIDE_IN_CHARACTER_EVENT_COMPLETE"), arg);
		}
	}
}
