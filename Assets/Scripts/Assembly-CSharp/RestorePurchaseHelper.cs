using InApp;
using JetBrains.Annotations;
using UnityEngine;

public class RestorePurchaseHelper : MonoBehaviour
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private UILabel buttonLabel;

	[UsedImplicitly]
	public void OnButtonClick()
	{
		InAppManager.instance.RestoreNonConsumablePurchases(InAppData.GetAllProductIds());
	}

	private void Awake()
	{
		titleLabel.text = Strings.Get("RESTORE_BUTTON_TITLE");
		descriptionLabel.text = Strings.Get("RESTORE_BUTTON_DESCRIPTION");
		buttonLabel.text = Strings.Get("RESTORE_BUTTON_BUTTON_TEXT");
	}
}
