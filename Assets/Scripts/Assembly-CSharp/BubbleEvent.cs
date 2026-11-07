using System.Collections;
using UnityEngine;

public class BubbleEvent : CharacterCustomizationProp
{
	private Animation _animation;

	[SerializeField]
	private float randomMax = 5f;

	[SerializeField]
	private float randomMin = 2f;

	public override void Initialize(CharacterRig characterRig, bool alternateSetup)
	{
		base.transform.parent = characterRig.HeadJoint;
		base.transform.localPosition = new Vector3(0f, 0.12f, -1.35f);
		base.transform.localRotation = Quaternion.Euler(-3.44f, -180f, 0f);
		base.transform.localScale = Vector3.one;
		_animation = GetComponent<Animation>();
		StartCoroutine(BlowBubbles());
	}

	private IEnumerator BlowBubbles()
	{
		yield return new WaitForSeconds(Random.Range(randomMin, randomMax));
		_animation.Play();
		StartCoroutine(BlowBubbles());
	}
}
