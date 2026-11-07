using UnityEngine;

public class MeFooter : MonoBehaviour
{
	[SerializeField]
	private UILabel _menuLabel;

	[SerializeField]
	private UILabel _charactersLabel;

	[SerializeField]
	private UILabel _boarsLabel;

	[SerializeField]
	private UILabel _awardsLabel;

	private void OnEnable()
	{
		_menuLabel.text = Strings.Get("FOOTER_MENU_BUTTON_TITLE");
		_charactersLabel.text = Strings.Get("ME_FOOTER_CHARACTERS");
		_boarsLabel.text = Strings.Get("ME_FOOTER_BOARDS");
		_awardsLabel.text = Strings.Get("AWARDS_TITLE");
	}
}
