using Info;
using UI.MessageCenter;
using UI.Screens.RewardManagement;

namespace UI
{
	public class MystBoxButtonTest : UIBasicButton
	{
		public bool doubleBox;

		protected override void Send()
		{
			RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.MysteryBox, ObtainableItemSource.FreeUnlock);
			if (doubleBox)
			{
				RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.MysteryBox, ObtainableItemSource.FreeUnlock);
			}
		}
	}
}
