using System;

public struct ConfirmationPopupVariantObject
{
	public string titleText;

	public Func<string> getDescriptionText;

	public string greenButtonText;

	public string redButtonText;

	public Action greenButtonAction;

	public Action redButtonAction;

	public ConfirmationPopupVariantObject(string titleText, Func<string> getDescriptionText, string greenButtonText, string redButtonText, Action greenButtonAction, Action redButtonAction)
	{
		this.titleText = titleText;
		this.getDescriptionText = getDescriptionText;
		this.greenButtonText = greenButtonText;
		this.redButtonText = redButtonText;
		this.greenButtonAction = greenButtonAction;
		this.redButtonAction = redButtonAction;
	}
}
