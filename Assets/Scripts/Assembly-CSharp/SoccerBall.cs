using UnityEngine;

public class SoccerBall : CharacterCustomizationProp
{
	public override void Initialize(CharacterRig characterRig, bool alternateSetup)
	{
		base.transform.parent = characterRig.AttachPoint1.transform;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
	}
}
