using Kiloo.Common.Extensions;
using UI;
using UnityEngine;

public class RestoredContentHelper : MonoBehaviour
{
	[SerializeField]
	private UISprite _objSprite;

	[SerializeField]
	private UILabel _objLabel;

	public void Setup(string spriteName, string text)
	{
		base.transform.SetLocalPosX(-66f);
		_objSprite.spriteName = spriteName;
		_objLabel.text = text;
		_objLabel.color = ColorData.instance.white;
		_objLabel.effectColor = ColorData.instance.shadow;
	}
}
