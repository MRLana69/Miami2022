using UnityEngine;

public class Spraycan : CharacterCustomizationProp
{
	public override void Initialize(CharacterRig characterRig, bool alternateSetup)
	{
		base.transform.parent = characterRig.RightHand;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
	}
}
