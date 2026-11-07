using System.Collections;
using UnityEngine;

public class FanFoldOut : CharacterCustomizationProp
{
	[SerializeField]
	private float _flipTime = 0.2f;

	[SerializeField]
	private Renderer _foldIn;

	[SerializeField]
	private Renderer _foldOut;

	[SerializeField]
	private string _requiredAnimationName = string.Empty;

	private CharacterRig _characterRig;

	public override void Initialize(CharacterRig characterRig, bool alternateSetup)
	{
		_characterRig = characterRig;
		base.transform.parent = characterRig.RightHand;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
		if (!alternateSetup)
		{
			StartCoroutine(Play());
		}
		else
		{
			_foldOut.enabled = true;
		}
	}

	private IEnumerator Play()
	{
		while (!_characterRig.CharacterAnimation.IsPlaying(_requiredAnimationName))
		{
			yield return null;
		}
		yield return new WaitForSeconds(_flipTime);
		_foldIn.enabled = false;
		_foldOut.enabled = true;
	}
}
