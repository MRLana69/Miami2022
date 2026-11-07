using Info;
using UI.MessageCenter;

public class DescriptionButtonHelper : UIBasicButton
{
	private UpgradeType _type;

	public void Init(UpgradeType type)
	{
		_type = type;
	}

	protected override void Send()
	{
		if (_type == UpgradeType.mysterybox)
		{
			bool flag = UIScreenController.Instance.GetCurrentPopupName() == "UpgradePopup";
			UIScreenController.Instance.PushPopup("PrizeMBPopup");
			string argValue = ((!flag) ? "Boost" : "QuickBoost");
			Flurry.LogEventWithAParameter("Mysterybox view prices", "Screen Name", argValue);
		}
		else if (_type == UpgradeType.tokenmysterybox)
		{
			string currentPopupName = UIScreenController.Instance.GetCurrentPopupName();
			string argValue2 = ((currentPopupName == "UpgradePopup") ? "QuickBoostPopup" : ((!(currentPopupName == "CelebrationPopup")) ? "BoostScreen" : "CelebrationPopup"));
			UIScreenController.Instance.PushPopup("TokenMysteryBoxPrizesPopup");
			Flurry.LogEventWithAParameter("Character token box view prices", "ScreenName", argValue2);
		}
	}
}
