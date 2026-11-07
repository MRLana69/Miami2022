using System;
using Cloud.Internal;
using Info;
using Missions;

namespace UI.SlideIns
{
	public class SlideIn
	{
		public SlideInData.SlideInType type;

		public string stringValue;

		public AwardTierType awardTierType;

		public UISlideInFacebook.FacebookSlideInState facebookState;

		public Action onDidShowCallback;

		public int value;

		public MysteryBox.Type mysteryBoxType;

		public CloudSaveState cloudSaveState;

		public SellableConsumable consumableReward;

		public bool purchase;

		public string iconName;
	}
}
