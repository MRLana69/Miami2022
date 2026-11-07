using System.Collections;
using UnityEngine;

public class MagicCards : CharacterCustomizationProp
{
	[SerializeField]
	private Renderer _card1;

	[SerializeField]
	private Renderer _card2;

	[SerializeField]
	private Renderer _card3;

	[SerializeField]
	private Renderer _card4;

	private CharacterRig _characterRig;

	[SerializeField]
	private string _requiredAnimationName = string.Empty;

	public override void Initialize(CharacterRig characterRig, bool alternateSetup)
	{
		_characterRig = characterRig;
		base.transform.parent = characterRig.AttachPoint1.transform;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
		if (!alternateSetup)
		{
			StartCoroutine(Play());
			return;
		}
		_card1.enabled = false;
		_card2.enabled = false;
		_card3.enabled = false;
		_card4.enabled = false;
	}

	private IEnumerator Play()
	{
		_card1.enabled = false;
		_card2.enabled = false;
		_card3.enabled = false;
		_card4.enabled = false;
		while (!_characterRig.CharacterAnimation.IsPlaying(_requiredAnimationName))
		{
			yield return null;
		}
		yield return new WaitForSeconds(1.32f);
		_card1.enabled = true;
		_card4.enabled = true;
		yield return new WaitForSeconds(0.96f);
		_card1.enabled = false;
		_card4.enabled = false;
		yield return new WaitForSeconds(0.4f);
		_card2.enabled = true;
		_card3.enabled = true;
		yield return new WaitForSeconds(1.28f);
		_card2.enabled = false;
		_card3.enabled = false;
	}
}
