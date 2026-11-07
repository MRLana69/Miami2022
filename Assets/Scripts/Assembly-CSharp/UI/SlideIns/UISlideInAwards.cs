using Missions;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInAwards : UISlideIn
	{
		[SerializeField]
		private UISprite _icon;

		[SerializeField]
		private UILabel _textLine1;

		[SerializeField]
		private Layout _layout;

		[SerializeField]
		private GameObject _starObj;

		private const string CAN_BRONZE = "icon_bronze_can";

		private const string CAN_SILVER = "icon_silver_can";

		private const string CAN_GOLD = "icon_gold_can";

		private const string CAN_DIAMOND = "icon_diamond_can";

		public override void SetupSlideIn(SlideIn slideIn)
		{
			base.gameObject.SetActive(true);
			switch (slideIn.awardTierType)
			{
			case AwardTierType.Bronze:
				_icon.spriteName = "icon_bronze_can";
				_starObj.SetActive(false);
				break;
			case AwardTierType.Silver:
				_icon.spriteName = "icon_silver_can";
				_starObj.SetActive(false);
				break;
			case AwardTierType.Gold:
				_icon.spriteName = "icon_gold_can";
				_starObj.SetActive(false);
				break;
			case AwardTierType.Diamond:
				_icon.spriteName = "icon_diamond_can";
				_starObj.SetActive(true);
				break;
			}
			_textLine1.text = slideIn.stringValue;
			_layout.Align();
			SlideIn();
		}
	}
}
