using JetBrains.Annotations;
using UI.Screens;

public class HiddenSecretButton : UIScreen
{
	private const int STANDARD_NUMBER_OF_CLICKS_TO_OPEN = 3;

	private const int DEBUG_NUMBER_OF_CLICKS_TO_OPEN = 1;

	private int _taps;

	[UsedImplicitly]
	private void OnClick()
	{
		_taps++;
		if (_taps == (Globals.DEBUG ? 1 : 3))
		{
			UIScreenController.Instance.PushPopup("SecretPopup");
			_taps = 0;
		}
	}
}
