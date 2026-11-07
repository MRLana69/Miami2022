using System;
using UnityEngine;

public class PropBlendShapeController : MonoBehaviour
{
	[Serializable]
	private class BlendShapeLink
	{
		public int BlendShapeIndex;

		public int BlendShapeControllerIndex;
	}

	[SerializeField]
	private BlendShapeLink[] _blendShapeLinks;

	private SkinnedMeshRenderer _renderer;

	private AvatarAnimations _avatarAnimations;

	private void Start()
	{
		_renderer = GetComponent<SkinnedMeshRenderer>();
		_avatarAnimations = GetAvatarAnimationsInAncestors(base.transform.parent);
	}

	private void Update()
	{
		BlendShapeLink[] blendShapeLinks = _blendShapeLinks;
		foreach (BlendShapeLink blendShapeLink in blendShapeLinks)
		{
			_renderer.SetBlendShapeWeight(blendShapeLink.BlendShapeIndex, AvatarAnimations.NormalizeBlendShape(_avatarAnimations.GetBlendShapeValue(blendShapeLink.BlendShapeControllerIndex)) * 100f);
		}
	}

	private AvatarAnimations GetAvatarAnimationsInAncestors(Transform transform)
	{
		AvatarAnimations component = transform.GetComponent<AvatarAnimations>();
		if (component != null)
		{
			return component;
		}
		Transform parent = transform.parent;
		if (parent != null)
		{
			return GetAvatarAnimationsInAncestors(parent);
		}
		return null;
	}
}
