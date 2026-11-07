using UnityEngine;

public class SpecialAssetAttachment : CharacterCustomizationProp
{
	public enum AttachmentPoint
	{
		LeftHand = 0,
		RightHand = 1,
		Head = 2,
		AttachmentPoint1 = 3,
		AttachmentPoint2 = 4,
		AttachmentPoint3 = 5,
		AttachmentPoint4 = 6,
		AttachmentPoint5 = 7,
		AttachmentPoint6 = 8,
		AttachmentPoint7 = 9,
		AttachmentPoint8 = 10,
		AttachmentPoint9 = 11
	}

	[SerializeField]
	private AttachmentPoint attachmentPoint;

	public override void Initialize(CharacterRig characterRig, bool alternateSetup)
	{
		if (attachmentPoint == AttachmentPoint.LeftHand)
		{
			base.transform.parent = characterRig.LeftFoot;
		}
		else if (attachmentPoint == AttachmentPoint.RightHand)
		{
			base.transform.parent = characterRig.RightHand;
		}
		else if (attachmentPoint == AttachmentPoint.Head)
		{
			base.transform.parent = characterRig.HeadJoint;
		}
		else if (attachmentPoint == AttachmentPoint.AttachmentPoint1)
		{
			base.transform.parent = characterRig.AttachPoint1;
		}
		else if (attachmentPoint == AttachmentPoint.AttachmentPoint2)
		{
			base.transform.parent = characterRig.AttachPoint2;
		}
		else if (attachmentPoint == AttachmentPoint.AttachmentPoint3)
		{
			base.transform.parent = characterRig.AttachPoint3;
		}
		else if (attachmentPoint == AttachmentPoint.AttachmentPoint4)
		{
			base.transform.parent = characterRig.AttachPoint4;
		}
		else if (attachmentPoint == AttachmentPoint.AttachmentPoint5)
		{
			base.transform.parent = characterRig.AttachPoint5;
		}
		else if (attachmentPoint == AttachmentPoint.AttachmentPoint6)
		{
			base.transform.parent = characterRig.AttachPoint6;
		}
		else if (attachmentPoint == AttachmentPoint.AttachmentPoint7)
		{
			base.transform.parent = characterRig.AttachPoint7;
		}
		else if (attachmentPoint == AttachmentPoint.AttachmentPoint8)
		{
			base.transform.parent = characterRig.AttachPoint8;
		}
		else if (attachmentPoint == AttachmentPoint.AttachmentPoint9)
		{
			base.transform.parent = characterRig.AttachPoint9;
		}
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = base.transform.rotation;
		base.transform.localScale = Vector3.one;
	}
}
